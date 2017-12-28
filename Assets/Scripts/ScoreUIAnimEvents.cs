/******************************************************************************
*  @file       ScoreUIAnimEvents.cs
*  @brief      Holds events for Score UI "lose life" animations
*  @author     Ron
*  @date       August 7, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class ScoreUIAnimEvents : MonoBehaviour
{
	#region Animation Events

	/// <summary>
	/// Plays the sound of life character "falling".
	/// </summary>
	private void PlayLoseLife1Sound()
	{
		Locator.GetSoundSystem().PlayOneShot(SoundInfo.SFXID.LOSE_LIFE1);
	}

	/// <summary>
	/// Plays the sound of life character becoming tombstone.
	/// </summary>
	private void PlayLoseLife2Sound()
	{
		Locator.GetSoundSystem().PlayOneShot(SoundInfo.SFXID.LOSE_LIFE2);
	}

	#endregion // Animation Events
}
