/******************************************************************************
*  @file       SceneInfo.cs
*  @brief      Holds basic data and methods for game scenes
*  @author     Ron, Lori
*  @date       August 5, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class SceneInfo
{
	#region Scene Identifiers
	
	// Enum of all scenes that can be switched to in the game
	// (does not include Main, as you can only switch *from* it to another scene)
	public enum SceneEnum
	{
		GAME_MANAGER = 0,
		TITLE,
		MAIN_MENU,
		SETTINGS,
		RESULTS,
		MINIGAME_PLANE,
		MINIGAME_BUTTON,
		MINIGAME_DOOR,
		MINIGAME_FIRE,
		MINIGAME_PIRANHA,
		MINIGAME_WASP,
		MINIGAME_YELLOWLINE,
		MINIGAME_BLOOD,
		MINIGAME_FORK,
		MINIGAME_BALLOON,
		MINIGAME_WIRE,
		MINIGAME_SPACE,
		MINIGAME_GLUE,
		SIZE
	}
	
	// The order scene names and master scripts are listed should match GameScene enum
	private string[] m_sceneNames =
	{
		"GameManager",
		"Title",
		"MainMenu",
		"Settings",
		"Results",
		"MiniGame_Plane",
		"MiniGame_Button",
		"MiniGame_Door",
		"MiniGame_Fire",
		"MiniGame_Piranha",
		"MiniGame_Wasp",
		"MiniGame_YellowLine",
		"MiniGame_Blood",
		"MiniGame_Fork",
		"MiniGame_Balloon",
		"MiniGame_Wire",
		"MiniGame_Space",
		"MiniGame_Glue",
	};
	
	#endregion // Scene Identifiers

	#region Public Interface

	/// <summary>
	/// Gets the name of the specified scene.
	/// </summary>
	/// <returns>Name of specified scene.</returns>
	/// <param name="gameScene">Game scene.</param>
	public string GetSceneNameOf(SceneEnum gameScene)
	{
		if (gameScene == SceneEnum.SIZE)
		{
			Debug.Log("Specified item is not a scene");
			return null;
		}
		return m_sceneNames[(int)gameScene];
	}

	#endregion // Public Interface
}
