/******************************************************************************
*  @file       ButtonMGSceneMaster.cs
*  @brief      Handles the Button MiniGame scene
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

public class ButtonMGSceneMaster : MiniGameSceneMasterBase
{
	#region Public Interface

	#endregion // Public Interface

	#region Serialized Variables

	[Header("Button")]
	[SerializeField] private	RedButton		m_redButton		= null;
	[SerializeField] private	Animator		m_buttonAnim	= null;
	[Header("Ending Animation")]
	[SerializeField] private	GameObject		m_endScreen		= null;
	[SerializeField] private	Animator		m_endAnimWin	= null;
	[SerializeField] private	Animator		m_endAnimLose	= null;

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
		StopGame(true);
	}

	#endregion // Level and Game Timer

	#region Gameplay

	/// <summary>
	/// Starts the game.
	/// </summary>
	protected override void StartGame()
	{
		m_redButton.Initialize(OnRedButtonPressed);
		AddToInteractiveObjectList(m_redButton);
		AddToAnimatorList(m_buttonAnim);
	}

	/// <summary>
	/// Updates the game.
	/// </summary>
	protected override void UpdateGame()
	{

	}

	/// <summary>
	/// Raises the stop game event.
	/// </summary>
	protected override void OnStopGame()
	{

	}

	/// <summary>
	/// Called when the red button has been pressed
	/// </summary>
	private void OnRedButtonPressed ()
	{
		m_buttonAnim.Play("ButtonPressed");
		Locator.GetSoundSystem().PlayOneShot(SoundInfo.SFXID.BUTTON_PRESS);
		StopGame(false);
	}

	#endregion // Gameplay

	#region Ending Animation

	/// <summary>
	/// Starts the win animation.
	/// </summary>
	protected override void StartWinAnimation()
	{
		m_endScreen.SetActive(true);
		m_endAnimWin.gameObject.SetActive(true);
		m_endAnimLose.gameObject.SetActive(false);
		AddToAnimatorList(m_endAnimWin);
		AddToSoundObjectList(Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.BUTTON_RAINBOW));
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
		m_endScreen.SetActive(true);
		m_endAnimWin.gameObject.SetActive(false);
		m_endAnimLose.gameObject.SetActive(true);
		AddToAnimatorList(m_endAnimLose);
		AddToSoundObjectList(Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.BUTTON_EXPLOSION));
	}

	/// <summary>
	/// Updates the lose animation.
	/// </summary>
	protected override void UpdateLoseAnimation()
	{
		
	}

	#endregion // Ending Animation
}
