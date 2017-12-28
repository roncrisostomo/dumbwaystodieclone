/******************************************************************************
*  @file       ForkMGSceneMaster.cs
*  @brief      Handles the Fork MiniGame scene
*  @author     Ron
*  @date       August 18, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript;
using TouchScript.Gestures.Simple;

#endregion // Namespaces

public class ForkMGSceneMaster : MiniGameSceneMasterBase
{
	#region Public Interface

	/// <summary>
	/// Notifies of fork exiting the toaster.
	/// </summary>
	public void NotifyForkExitedToaster()
	{
		StopGame(true);
	}

	/// <summary>
	/// Notifies of fork touching the toaster
	/// </summary>
	public void NotifyForkTouchedToaster()
	{
		StopGame(false);
	}

	/// <summary>
	/// Notifies of scene pause.
	/// </summary>
	public override void NotifyPause()
	{
		base.NotifyPause();

		m_fork.Pause();

		// Pause end anim sounds
		if (m_electrocuteSound != null)
		{
			m_electrocuteSound.Pause();
		}
	}

	/// <summary>
	/// Notifies of scene unpause.
	/// </summary>
	public override void NotifyUnpause()
	{
		base.NotifyUnpause();

		m_fork.Unpause();

		// Unpause end anim sounds
		if (m_electrocuteSound != null)
		{
			m_electrocuteSound.Unpause();
		}
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private	Fork				m_fork				= null;
	[SerializeField] private	Transform			m_toasterExit		= null;

	[SerializeField] private	GameObject			m_endScreen			= null;
	[SerializeField] private	Animator			m_endAnimWin		= null;
	[SerializeField] private	Animator			m_endAnimLose		= null;
	[SerializeField] private	Transform			m_endAnimCharLose	= null;
	[SerializeField] private	float				m_loseAnimSpeed		= 50.0f;
	[SerializeField] private	Color				m_loseBlinkColor	= new Color(0.7f, 0.7f, 0.7f, 0.3f);

	#endregion // Serialized Variables

	#region Resource Loading

	/// <summary>
	/// Loads the resources.
	/// </summary>
	protected override bool LoadResources()
	{
		m_fork.Initialize(this, m_toasterExit.position.y);

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

	}

	/// <summary>
	/// Raises the stop game event.
	/// </summary>
	protected override void OnStopGame()
	{
	
	}

	#endregion // Gameplay

	#region Ending Animation

	private UIAnimator 	m_loseAnimator 		= null;

	private SoundObject m_electrocuteSound	= null;

	/// <summary>
	/// Starts the win animation.
	/// </summary>
	protected override void StartWinAnimation()
	{
		m_endScreen.SetActive(true);
		m_endAnimWin.gameObject.SetActive(true);
		m_endAnimLose.gameObject.SetActive(false);
		AddToAnimatorList(m_endAnimWin);
		Locator.GetSoundSystem().PlayOneShot(SoundInfo.SFXID.FORK_SWISH);
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

		// Use UIAnimator to change character color and alpha as if electrocuted
		m_loseAnimator = new UIAnimator(m_endAnimCharLose, UIAnimator.UIAnimatorType.COLOR, true, true);
		m_loseAnimator.SetColorAnimation(Color.white, m_loseBlinkColor);
		m_loseAnimator.SetAnimSpeed(m_loseAnimSpeed);
		m_loseAnimator.AnimateToState2();

		m_electrocuteSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.FORK_ELECTROCUTE);
	}

	/// <summary>
	/// Updates the lose animation.
	/// </summary>
	protected override void UpdateLoseAnimation()
	{
		if (m_loseAnimator == null)
		{
			return;
		}

		m_loseAnimator.Update(Time.deltaTime);

		// Make lose animation loop until SceneMaster ends the scene
		if (m_loseAnimator.IsInState1)
		{
			m_loseAnimator.AnimateToState2();
		}
		else if (m_loseAnimator.IsInState2)
		{
			m_loseAnimator.AnimateToState1();
		}
	}

	#endregion // Ending Animation
}
