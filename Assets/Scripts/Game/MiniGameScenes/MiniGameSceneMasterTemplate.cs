/******************************************************************************
*  @file       MiniGameSceneMasteTemplate.cs
*  @brief      Template for a MiniGameSceneMaster
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

public class MiniGameSceneMasterTemplate : MiniGameSceneMasterBase
{
	#region Public Interface

	#endregion // Public Interface

	#region Serialized Variables

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
		// Sample
		StopGame(false);
	}

	#endregion // Level and Game Timer

	#region Gameplay

	/// <summary>
	/// Starts the game.
	/// </summary>
	protected override void StartGame()
	{

	}

	/// <summary>
	/// Updates the game.
	/// </summary>
	protected override void UpdateGame()
	{
		// Sample
		StopGame(true);
	}

	/// <summary>
	/// Raises the stop game event.
	/// </summary>
	protected override void OnStopGame()
	{

	}
	
	#endregion // Gameplay

	#region Ending Animation

	/// <summary>
	/// Starts the win animation.
	/// </summary>
	protected override void StartWinAnimation()
	{
	
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
