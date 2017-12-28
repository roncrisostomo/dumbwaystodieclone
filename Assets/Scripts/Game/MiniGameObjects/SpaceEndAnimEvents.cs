/******************************************************************************
*  @file       SpaceEndAnimEvents.cs
*  @brief      Holds events for end animations in the Space MiniGame
*  @author     Ron
*  @date       August 18, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class SpaceEndAnimEvents : MonoBehaviour
{
	#region Public Interface
	
	/// <summary>
	/// Pauses this instance.
	/// </summary>
	public void Pause()
	{
		if (m_helmetSound != null)
		{
			m_helmetSound.Pause();
		}
		if (m_inflateSound != null)
		{
			m_inflateSound.Pause();
		}
		if (m_blowUpSound != null)
		{
			m_blowUpSound.Pause();
		}
	}
	
	/// <summary>
	/// Unpauses this instance.
	/// </summary>
	public void Unpause()
	{
		if (m_helmetSound != null)
		{
			m_helmetSound.Unpause();
		}
		if (m_inflateSound != null)
		{
			m_inflateSound.Unpause();
		}
		if (m_blowUpSound != null)
		{
			m_blowUpSound.Unpause();
		}
	}
	
	#endregion // Public Interface
	
	#region Serialized Variables

	[SerializeField] private SpaceMGSceneMaster m_sceneMaster = null;

	#endregion // Serialized Variables

	#region Animation Events

	private SoundObject m_helmetSound	= null;
	private SoundObject m_inflateSound 	= null;
	private SoundObject m_blowUpSound 	= null;

	/// <summary>
	/// Plays the sound of character retrieving helmet.
	/// </summary>
	private void PlayGetHelmetSound()
	{
		m_helmetSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.SPACE_HELMET);
	}

	/// <summary>
	/// Plays the sound of character's head enlarging.
	/// </summary>
	private void PlayEnlargeSound()
	{
		m_inflateSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.SPACE_INFLATE);
	}

	/// <summary>
	/// Plays the sound of character's head blowing up.
	/// </summary>
	private void PlayBlowUpSound()
	{
		m_blowUpSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.SPACE_BLOWUP);
	}

	/// <summary>
	/// Raises the event of the character's head blowing up in the lose animation.
	/// </summary>
	private void OnHeadBlowUp()
	{
		if (m_inflateSound != null)
		{
			m_inflateSound.Stop();
		}
		PlayBlowUpSound();
		m_sceneMaster.NotifyLoseAnimHeadBlowUp();
	}

	#endregion // Animation Events
}
