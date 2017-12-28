/******************************************************************************
*  @file       FireMGSceneMaster.cs
*  @brief      Handles the Fire MiniGame scene
*  @author     Lori
*  @date       July 28, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript;

#endregion // Namespaces

public class FireMGSceneMaster : MiniGameSceneMasterBase
{
	#region Public Interface

	#endregion // Public Interface

	#region Serialized Variables

	[Header("Input")]
	[SerializeField] private	uint		m_tapCount						= 12;
	[SerializeField] private	float		m_tapCooldown					= 0.1f;
	[Header("Character Movement")]
	[SerializeField] private	float		m_characterScaleIncrement		= 0.2f;
	[SerializeField] private	float		m_characterYPositionIncrement	= -0.3f;
	[SerializeField] private	GameObject	m_character						= null;
	[SerializeField] private	float		m_fireScaleIncrement			= -0.08f;
	[SerializeField] private	float		m_fireYPositionIncrement		= 0.01f;
	[SerializeField] private	GameObject	m_fire							= null;
	[SerializeField] private	GameObject	m_shadow						= null;
	[Header("Game Animation")]
	[SerializeField] private	float		m_fallSpeed						= 10f;
	[SerializeField] private	float		m_fallRotateSpeed				= 1080f;
	[SerializeField] private	Animator	m_characterAnimator				= null;
	[SerializeField] private	Animator	m_fireAnimator					= null;
	[Header("Ending Animation")]
	[SerializeField] private	GameObject	m_endScreen						= null;
	[SerializeField] private	Animator	m_loseAnimator					= null;

	#endregion // Serialized Variables

	#region Resource Loading

	/// <summary>
	/// Loads the resources.
	/// </summary>
	protected override bool LoadResources()
	{
		return true;
	}

	/// <summary>
	/// Unloads the resources.
	/// </summary>
	protected override bool UnloadResources()
	{
		return true;
	}

	#endregion // Resource Loading

	#region Level and Game Timer

	/// <summary>
	/// Raises the time run out event.
	/// </summary>
	protected override void OnTimeRunOut()
	{
		StopGame(false);
	}

	#endregion // Level and Game Timer

	#region Gameplay

	private		uint			m_tapCounter			= 0;
	private		float			m_tapTimer				= 0f;
	private		SoundObject		m_fireBurningSound		= null;

	// Lerp variables
	private		float			m_lerpTimer				= 0f;
	private		float			m_lerpDuration			= 0.15f;
	private		Vector3			m_characterStartPos		= Vector3.zero;
	private		Vector3			m_characterTargetPos	= Vector3.zero;
	private		float			m_characterStartScale	= 1f;
	private		float			m_characterTargetScale	= 1f;
	private		Vector3			m_fireStartPos			= Vector3.zero;
	private		Vector3			m_fireTargetPos			= Vector3.zero;
	private		float			m_fireStartScale		= 1f;
	private		float			m_fireTargetScale		= 1f;

	/// <summary>
	/// Starts the game.
	/// </summary>
	protected override void StartGame()
	{
		m_tapCounter = 0;
		m_tapTimer = m_tapCooldown;

		// Play the fire sound
		m_fireBurningSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.FIRE_BURNING);
		AddToSoundObjectList(m_fireBurningSound);

		// Add animators to list
		AddToAnimatorList(m_characterAnimator);
		AddToAnimatorList(m_fireAnimator);
		
		// Initialize input for the entire screen
		InitializeInput();

		// Initialize the lerp variables
		m_lerpTimer = m_lerpDuration;
		m_characterTargetPos = m_character.transform.position;
		m_characterStartPos = m_characterTargetPos;
		m_characterTargetScale = m_character.transform.localScale.x;
		m_characterStartScale = m_characterTargetScale;
		m_fireTargetPos = m_fire.transform.position;
		m_fireStartPos = m_fireTargetPos;
		m_fireTargetScale = m_fire.transform.localScale.x;
		m_fireStartScale = m_fireTargetScale;
	}

	/// <summary>
	/// Updates the game.
	/// </summary>
	protected override void UpdateGame()
	{
		// Update the tap timer
		m_tapTimer += Time.deltaTime;

		// Lerp the character and fire position and size
		if (m_lerpTimer < m_lerpDuration)
		{
			m_lerpTimer += Time.deltaTime;
			float lerpT = m_lerpTimer / m_lerpDuration;

			if (lerpT >= 1f)
			{
				// Resize and reposition the character and fire
				m_character.transform.localScale = Vector3.one * m_characterTargetScale;
				m_character.transform.position = m_characterTargetPos;
				m_fire.transform.localScale = Vector3.one * m_fireTargetScale;
				m_fire.transform.position = m_fireTargetPos;
			}
			else
			{
				// Resize and reposition the character
				float newScale = Mathf.Lerp(m_characterStartScale, m_characterTargetScale, lerpT);
				Vector3 newPos = Vector3.Lerp(m_characterStartPos, m_characterTargetPos, lerpT);
				m_character.transform.localScale = Vector3.one * newScale;
				m_character.transform.position = newPos;
				
				// Resize and reposition the fire
				newScale = Mathf.Lerp(m_fireStartScale, m_fireTargetScale, lerpT);
				newPos = Vector3.Lerp(m_fireStartPos, m_fireTargetPos, lerpT);
				m_fire.transform.localScale = Vector3.one * newScale;
				m_fire.transform.position = newPos;
				
			}
		}
	}

	/// <summary>
	/// Raises the stop game event.
	/// </summary>
	protected override void OnStopGame()
	{
		DisableInput();
	}

	/// <summary>
	/// Called when the screen has been pressed
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void OnScreenPress(object sender, TouchEventArgs e)
	{
		if (IsPaused)
		{
			return;
		}

		// Do not count UI touches
		// TODO: Need to update if name changes
		// TODO: Don't just check Touches[0]
		if (e.Touches[0].Layer != null && e.Touches[0].Layer.Name == "UICamera")
		{
			return;
		}

		// Do not register taps within the cooldown period
		if (m_tapTimer < m_tapCooldown)
		{
			return;
		}

		// Resize and reposition the character
		m_characterStartPos = m_character.transform.position;
		m_characterTargetPos += Vector3.up * m_characterYPositionIncrement;
		m_characterStartScale = m_character.transform.localScale.x;
		m_characterTargetScale += m_characterScaleIncrement;
		
		// Resize and reposition the fire
		m_fireStartPos = m_fire.transform.position;
		m_fireTargetPos += Vector3.up * m_fireYPositionIncrement;
		m_fireStartScale = m_fire.transform.localScale.x;
		m_fireTargetScale += m_fireScaleIncrement;

		// Set lerp timer to 0 to initialte lerping
		m_lerpTimer = 0f;

		Locator.GetSoundSystem().PlayOneShot(SoundInfo.SFXID.FIRE_RUNNING);
		
		// Update the tap count
		m_tapCounter++;
		m_tapTimer = 0f;
		if (m_tapCounter >= m_tapCount)
		{
			StopGame(true);
		}
	}

	#region Input
	
	/// <summary>
	/// Initializes the input.
	/// </summary>
	private void InitializeInput()
	{
		if (TouchManager.Instance != null)
		{
			TouchManager.Instance.TouchesBegan += OnScreenPress;
		}
	}
	
	/// <summary>
	/// Disables the input.
	/// </summary>
	private void DisableInput()
	{
		if (TouchManager.Instance != null)
		{
			TouchManager.Instance.TouchesBegan -= OnScreenPress;
		}
	}
	
	#endregion // Input

	#endregion // Gameplay

	#region Ending Animation

	/// <summary>
	/// Starts the win animation.
	/// </summary>
	protected override void StartWinAnimation()
	{
		RemoveFromSoundObjectList(m_fireBurningSound);
		m_fireBurningSound.Delete();
		m_fire.SetActive(false);
		m_fireAnimator.speed = 0f;
		RemoveFromAnimatorList(m_fireAnimator);
		m_characterAnimator.Play("CharacterFireMGYeah");
	}

	/// <summary>
	/// Updates the win animation.
	/// </summary>
	protected override void UpdateWinAnimation()
	{
		
	}
	
	/// <summary>
	/// Starts the lose animation.
	/// </summary>
	protected override void StartLoseAnimation()
	{
		m_shadow.SetActive(false);
		m_endScreen.SetActive(true);
		m_characterAnimator.Play("CharacterFireMGSad");
		AddToAnimatorList(m_loseAnimator);
		Locator.GetSoundSystem().PlayOneShot(SoundInfo.SFXID.FIRE_DEATH);
	}

	/// <summary>
	/// Updates the lose animation.
	/// </summary>
	protected override void UpdateLoseAnimation()
	{
		m_character.transform.Translate(Vector3.down * m_fallSpeed * Time.deltaTime, Space.World);
		m_character.transform.Rotate(Vector3.forward * m_fallRotateSpeed * Time.deltaTime);

		if (m_endingAnimationTimer >= m_endingAnimationDuration)
		{
			RemoveFromSoundObjectList(m_fireBurningSound);
			m_fireBurningSound.Delete();
			m_fireBurningSound = null;
		}
	}

	#endregion // Ending Animation
}
