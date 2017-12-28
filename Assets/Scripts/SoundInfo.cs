/******************************************************************************
*  @file       SoundInfo.cs
*  @brief      Holds data for organizing sound resources
*  @author     Ron, Lori
*  @date       August 16, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class SoundInfo
{
	#region Sound Identifiers

	private const string AUDIO_PREFAB_ROOT_PATH = "Audio/Prefabs/";
	private const string SFX_PREFAB_PREFIX = "SFX-";
	private const string BGM_PREFAB_PREFIX = "BGM-";

	// Enum of all sound effects that would be used in the game
	public enum SFXID
	{
		UI_BUTTON,
		UI_BUTTON_PRESS,
		UI_BUTTON_RELEASE,
		FASTER,
		SCORE_COUNT,
		WIN01,
		WIN02,
		WIN03,
		LOSE01,
		LOSE02,
		LOSE03,
		LOSE_LIFE1,
		LOSE_LIFE2,
		PLANE_FLY,
		PLANE_CRASH,
		PLANE_FLING,
		BUTTON_PRESS,
		BUTTON_RAINBOW,
		BUTTON_EXPLOSION,
		DOOR_PEEK,
		DOOR_OPEN,
		FISH_SWIM,
		FISH_FLICK,
		FISH_BITE,
		FIRE_RUNNING,
		FIRE_BURNING,
		FIRE_DEATH,
		YELLOW_DRAG,
		YELLOW_TRAIN,
		WASP,
		WASP_SWAT,
		BLOOD_SHOOT,
		BLOOD_SQUELCH,
		BLOOD_WHISTLE,
		FORK_SLIDE,
		FORK_SWISH,
		FORK_ELECTROCUTE,
		BALLOON_CONNECT,
		BALLOON_GRAB,
		BALLOON_STEP,
		BALLOON_TRAIN,
		WIRE_CONNECT,
		WIRE_PLUGIN,
		WIRE_ZAP,
		WIRE_LIGHTSWITCH,
		WIRE_LIGHTS,
		WIRE_FIRE,
		SPACE_BUMP,
		SPACE_HELMET,
		SPACE_INFLATE,
		SPACE_BLOWUP,
		GLUE_TILT,
		GLUE_FALL,

		SIZE
	}

	// SFX prefab paths
	private string[] m_sfxPrefabPaths =
	{
		"UIButton",
		"UIButtonPress",
		"UIButtonRelease",
		"Faster",
		"ScoreCount",
		"Win01",
		"Win02",
		"Win03",
		"Lose01",
		"Lose02",
		"Lose03",
		"LoseLife1",
		"LoseLife2",
		"Plane-Fly",
		"Plane-Crash",
		"Plane-Fling",
		"Button-Press",
		"Button-Rainbow",
		"Button-Explosion",
		"Door-Peek",
		"Door-Open",
		"Fish-Swim",
		"Fish-Flick",
		"Fish-Bite",
		"Fire-Running",
		"Fire-Burning",
		"Fire-Death",
		"Yellow-Drag",
		"Yellow-Train",
		"Wasp",
		"Wasp-Swat",
		"Blood-Shoot",
		"Blood-Squelch",
		"Blood-Whistle",
		"Fork-Slide",
		"Fork-Swish",
		"Fork-Electrocute",
		"Balloon-Connect",
		"Balloon-Grab",
		"Balloon-Step",
		"Balloon-Train",
		"Wire-Connect",
		"Wire-PlugIn",
		"Wire-Zap",
		"Wire-LightSwitch",
		"Wire-Lights",
		"Wire-Fire",
		"Space-Bump",
		"Space-Helmet",
		"Space-Inflate",
		"Space-BlowUp",
		"Glue-Tilt",
		"Glue-Fall"
	};
	
	// Enum of all background music that would be used in the game
	public enum BGMID
	{
		MAIN_MENU,
		MINI_GAME,
		
		SIZE
	}

	// BGM prefab paths
	private string[] m_bgmPrefabPaths =
	{
		"MainMenu",
		"MiniGame"
	};

	// Types of sounds
	public enum SoundType
	{
		ONE_SHOT,	// Plays once, then is destroyed
		REGULAR,	// Plays once
		BGM			// Plays and loops
	}

	/// <summary>
	/// Gets the SFX prefab path.
	/// </summary>
	/// <returns>The SFX prefab path.</returns>
	/// <param name="sfxID">ID of sound effect.</param>
	public string GetSoundPrefabPath(SFXID sfxID)
	{
		if (sfxID == SFXID.SIZE)
		{
			Debug.Log("Specified item is not an SFX");
			return null;
		}
		return AUDIO_PREFAB_ROOT_PATH + SFX_PREFAB_PREFIX + m_sfxPrefabPaths[(int)sfxID];
	}
	
	/// <summary>
	/// Gets the BGM prefab path.
	/// </summary>
	/// <returns>The BGM prefab path.</returns>
	/// <param name="bgmID">ID of background music.</param>
	public string GetSoundPrefabPath(BGMID bgmID)
	{
		if (bgmID == BGMID.SIZE)
		{
			Debug.Log("Specified item is not a BGM");
			return null;
		}
		return AUDIO_PREFAB_ROOT_PATH + BGM_PREFAB_PREFIX + m_bgmPrefabPaths[(int)bgmID];
	}
	
	#endregion // Sound Identifiers
}
