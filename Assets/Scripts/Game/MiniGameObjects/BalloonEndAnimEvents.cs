/******************************************************************************
*  @file       BalloonEndAnimEvents.cs
*  @brief      Holds events for end animations in the Balloon MiniGame
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

public class BalloonEndAnimEvents : MonoBehaviour
{
	#region Public Interface

	/// <summary>
	/// Pauses this instance.
	/// </summary>
	public void Pause()
	{
		if (m_stepSound != null)
		{
			m_stepSound.Pause();
		}
		if (m_trainSound != null)
		{
			m_trainSound.Pause();
		}
	}

	/// <summary>
	/// Unpauses this instance.
	/// </summary>
	public void Unpause()
	{
		if (m_stepSound != null)
		{
			m_stepSound.Unpause();
		}
		if (m_trainSound != null)
		{
			m_trainSound.Unpause();
		}
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private Animator m_trainAnimator = null;

	#endregion // Serialized Variables

	#region Animation Events

	private SoundObject m_stepSound 	= null;
	private SoundObject m_trainSound 	= null;

	/// <summary>
	/// Plays the sound of character stepping into the tracks.
	/// </summary>
	private void PlayStepSound()
	{
		// Play train sound
		m_stepSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.BALLOON_STEP);
	}

	/// <summary>
	/// Plays the train animation.
	/// </summary>
	private void TriggerTrainApproach()
	{
		// Set camera to perspective to properly show train animation
		Camera.main.orthographic = false;
		// Play train animation
		m_trainAnimator.enabled = true;
		// Play train sound
		m_trainSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.BALLOON_TRAIN);
	}

	/// <summary>
	/// Raises the train animation done event.
	/// </summary>
	private void OnTrainAnimDone()
	{
		// Return camera to orthographic mode when animation is finished
		//Camera.main.orthographic = true;
	}

	#endregion // Animation Events
}
