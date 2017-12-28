/******************************************************************************
*  @file       GlueMGSceneMaster.cs
*  @brief      Handles the Glue MiniGame scene
*  @author     Ron
*  @date       August 16, 2015
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

public class GlueMGSceneMaster : MiniGameSceneMasterBase
{
	#region Public Interface

	/// <summary>
	/// Notifies that the character has fallen.
	/// </summary>
	public void NotifyCharacterFell()
	{
		StopGame(false);
	}
	
	/// <summary>
	/// Notifies of scene pause.
	/// </summary>
	public override void NotifyPause()
	{
		base.NotifyPause();

		m_tiltController.Pause();
	}
	
	/// <summary>
	/// Notifies of scene unpause.
	/// </summary>
	public override void NotifyUnpause()
	{
		base.NotifyUnpause();

		m_tiltController.Unpause();
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private float[]		m_sensitivityPerLevel = new float[]{ 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };
	[SerializeField] private TiltController	m_tiltController = null;

	[SerializeField] private GameObject		m_endScreen		= null;
	[SerializeField] private Animator		m_endAnimWin	= null;
	[SerializeField] private Animator		m_endAnimChar	= null;

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
		m_tiltController.Initialize(this, m_sensitivityPerLevel[m_level]);
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
		m_tiltController.SetGameOver();
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
		AddToAnimatorList(m_endAnimWin);
		AddToAnimatorList(m_endAnimChar);
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

	}

	/// <summary>
	/// Updates the lose animation.
	/// </summary>
	protected override void UpdateLoseAnimation()
	{
		
	}

	#endregion // Ending Animation
}
