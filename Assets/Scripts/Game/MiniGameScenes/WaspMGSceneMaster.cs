/******************************************************************************
*  @file       WaspMGSceneMaster.cs
*  @brief      Handles the Wasp MiniGame scene
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

public class WaspMGSceneMaster : MiniGameSceneMasterBase
{
	#region Public Interface

	#endregion // Public Interface

	#region Serialized Variables

	[Header("Wasp")]
	[SerializeField] private	uint[]			m_waspCountPerLevel			= null;
	[SerializeField] private	Wasp			m_wasp						= null;
	[Header("Game Animation")]
	[SerializeField] private	Transform		m_browLeft					= null;
	[SerializeField] private	Transform		m_browRight					= null;
	[SerializeField] private	Transform		m_mouth						= null;
	[SerializeField] private	Transform		m_chin						= null;
	[SerializeField] private	Animator[]		m_animators					= null;
	[Header("Camera Shake Animation")]
	[SerializeField] private	float			m_sensitivity				= 0.25f;
	[SerializeField] private	float			m_shakeDuration				= 0.25f;

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
		if (m_activeWaspCount > 0)
		{
			StopGame(false);
		}
		else
		{
			StopGame(true);
		}
	}

	#endregion // Level and Game Timer


	#region Gameplay

	private		Wasp[]		m_wasps				= null;
	private		uint		m_activeWaspCount	= 0;
	private		SoundObject	m_waspSound			= null;
	private		float		m_shakeTimer	= 0f;

	/// <summary>
	/// Starts the game.
	/// </summary>
	protected override void StartGame()
	{
		// Get the wasp count for the current level
		if (m_level < m_waspCountPerLevel.Length)
		{
			m_activeWaspCount = m_waspCountPerLevel[m_level];
		}
		else
		{
			m_activeWaspCount = m_waspCountPerLevel[m_waspCountPerLevel.Length - 1];
		}
		
		// Initialize the wasp array and spawn all the wasps needed
		m_wasps = new Wasp[m_activeWaspCount];
		for (uint i = 0; i < m_activeWaspCount; ++i)
		{
			if (i == 0)
			{
				m_wasps[i] = m_wasp;
			}
			else
			{
				m_wasps[i] = Instantiate(m_wasp);
				m_wasps[i].transform.parent = m_wasp.transform.parent;
			}
			m_wasps[i].Initialize(OnPressWasp);
			AddToInteractiveObjectList(m_wasps[i]);
		}

		// Play the wasp sound
		m_waspSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.WASP);
		AddToSoundObjectList(m_waspSound);

		// Add animators to list
		if (m_animators != null)
		{
			foreach (Animator animator in m_animators)
			{
				AddToAnimatorList(animator);
			}
		}

		// Initialize the shake timer
		m_shakeTimer = m_shakeDuration;
	}

	/// <summary>
	/// Updates the game.
	/// </summary>
	protected override void UpdateGame()
	{
		if (m_shakeTimer < m_shakeDuration)
		{
			m_shakeTimer += Time.deltaTime;
			if (m_shakeTimer >= m_shakeDuration)
			{
				Camera.main.transform.SetPosX(0f);
				Camera.main.transform.SetPosY(0f);
			}
			else
			{
				Camera.main.transform.SetPosX(Random.Range(-m_sensitivity, m_sensitivity));
				Camera.main.transform.SetPosY(Random.Range(-m_sensitivity, m_sensitivity));
			}
		}
	}

	/// <summary>
	/// Raises the stop game event.
	/// </summary>
	protected override void OnStopGame()
	{
	
	}

	/// <summary>
	/// Called when a wasp has been pressed
	/// </summary>
	private void OnPressWasp()
	{
		if (m_activeWaspCount > 0)
		{
			Locator.GetSoundSystem().PlayOneShot(SoundInfo.SFXID.WASP_SWAT);
			--m_activeWaspCount;
			m_shakeTimer = 0f;
			if (m_activeWaspCount == 0)
			{
				m_timer = m_duration - m_shakeDuration;
			}
		}
	}
	
	#endregion // Gameplay

	#region Ending Animation

	private		float		m_spawnWaspAnimTimer			= 0f;
	private		float		m_spawnWaspAnimDuration			= 0.2f;

	/// <summary>
	/// Starts the win animation.
	/// </summary>
	protected override void StartWinAnimation()
	{
		// Change face into a happy one
		m_browLeft.SetScaleY(-m_browLeft.transform.localScale.y);
		m_browLeft.Rotate(Vector3.forward * 35.0f);
		m_browRight.SetScaleY(-m_browRight.transform.localScale.y);
		m_browRight.Rotate(Vector3.forward * -35.0f);
		m_mouth.Rotate(Vector3.forward * 180.0f);
		m_chin.gameObject.SetActive(false);
		
		// Disable wasp sound
		if (m_waspSound != null)
		{
			RemoveFromSoundObjectList(m_waspSound);
			m_waspSound.Delete();
			m_waspSound = null;
		}

		// Hide that last wasp...
		foreach (Wasp wasp in m_wasps)
		{
			if (wasp != null)
			{
				wasp.gameObject.SetActive(false);
			}
		}

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
		m_spawnWaspAnimTimer = m_spawnWaspAnimDuration;
	}

	/// <summary>
	/// Updates the lose animation.
	/// </summary>
	protected override void UpdateLoseAnimation()
	{
		m_spawnWaspAnimTimer += Time.deltaTime;
		if (m_spawnWaspAnimTimer >= m_spawnWaspAnimDuration)
		{
			SpawnRandomWaspAnim();
		}

		if (m_endingAnimationTimer >= m_endingAnimationDuration)
		{
			// Disable wasp sound
			if (m_waspSound != null)
			{
				RemoveFromSoundObjectList(m_waspSound);
				m_waspSound.Delete();
				m_waspSound = null;
			}
		}
	}

	/// <summary>
	/// Spawns a wasp randomly
	/// </summary>
	private void SpawnRandomWaspAnim ()
	{
		Wasp newWasp  = Instantiate(m_wasp);
		newWasp.transform.parent = m_wasp.transform.parent;
		newWasp.Initialize(null);
		newWasp.gameObject.SetActive(true);
		AddToInteractiveObjectList(newWasp);
	}

	#endregion // Ending Animation
}
