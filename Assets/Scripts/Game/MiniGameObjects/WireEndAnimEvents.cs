/******************************************************************************
*  @file       WireEndAnimEvents.cs
*  @brief      Holds events for end animations in the Wire MiniGame
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

public class WireEndAnimEvents : MonoBehaviour
{
	#region Public Interface

	/// <summary>
	/// Sets the result of the game, to determine which end animation to play.
	/// </summary>
	/// <param name="isGameWon">If set to <c>true</c> game is won.</param>
	public void SetResult(bool isGameWon)
	{
		m_isGameWon = isGameWon;
	}

	/// <summary>
	/// Pauses this instance.
	/// </summary>
	public void Pause()
	{
		if (m_lightSwitchSound != null)
		{
			m_lightSwitchSound.Pause();
		}
		if (m_lightsSound != null)
		{
			m_lightsSound.Pause();
		}
		if (m_fireSound != null)
		{
			m_fireSound.Pause();
		}
	}
	
	/// <summary>
	/// Unpauses this instance.
	/// </summary>
	public void Unpause()
	{
		if (m_lightSwitchSound != null)
		{
			m_lightSwitchSound.Unpause();
		}
		if (m_lightsSound != null)
		{
			m_lightsSound.Unpause();
		}
		if (m_fireSound != null)
		{
			m_fireSound.Unpause();
		}
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private Animator m_winAnimator 	= null;
	[SerializeField] private Animator m_loseAnimator 	= null;

	#endregion // Serialized Variables

	#region Animation Events

	private bool 		m_isGameWon 		= false;

	private SoundObject m_lightSwitchSound 	= null;
	private SoundObject m_lightsSound 		= null;
	private SoundObject m_fireSound 		= null;

	/// <summary>
	/// Plays the light switch sound.
	/// </summary>
	private void PlayLightSwitchSound()
	{
		m_lightSwitchSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.WIRE_LIGHTSWITCH);
	}

	/// <summary>
	/// Plays the win animation if game is won.
	/// </summary>
	private void CheckPlayWinAnimation()
	{
		if (m_isGameWon)
		{
			// Activate win animator
			m_winAnimator.gameObject.SetActive(true);
			m_winAnimator.enabled = true;
			// Play lights sound
			PlayLightsSound();
		}
	}

	/// <summary>
	/// Plays the lose animation if game is lost.
	/// </summary>
	private void CheckPlayLoseAnimation()
	{
		if (!m_isGameWon)
		{
			// Activate lose animator
			m_loseAnimator.gameObject.SetActive(true);
			m_loseAnimator.enabled = true;
			// Play fire sound
			PlayFireSound();
		}
	}

	/// <summary>
	/// Plays the fire sound.
	/// </summary>
	private void PlayFireSound()
	{
		m_fireSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.WIRE_FIRE);
	}

	/// <summary>
	/// Plays the lights sound.
	/// </summary>
	private void PlayLightsSound()
	{
		m_lightsSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.WIRE_LIGHTS);
	}

	#endregion // Animation Events
}
