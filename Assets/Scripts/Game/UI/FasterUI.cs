/******************************************************************************
*  @file       FasterUI.cs
*  @brief      Handles the "faster" UI that appears in between games
*  @author     Ron
*  @date       August 19, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class FasterUI : MonoBehaviour
{
	#region Public Interface

	/// <summary>
	/// Initialize this instance.
	/// </summary>
	public void Initialize()
	{
		// Set the initialized flag
		m_isInitialized = true;
	}

	/// <summary>
	/// Starts the "faster" animation with the specified zero-based index.
	/// </summary>
	/// <param name="fasterType">"Faster" animation type.</param>
	public void StartFasterAnimation(int fasterType)
	{
		Show();
		fasterType = (int)Mathf.Clamp(fasterType, 0, m_fasterUIAnimators.Length - 1);
		m_fasterUIAnimators[fasterType].gameObject.SetActive(true);

		// Play faster SFX
		if (m_fasterSFX == null)
		{
			m_fasterSFX = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.FASTER);
		}
		else
		{
			m_fasterSFX.Play();
		}
	}

	/// <summary>
	/// Stops the faster animation.
	/// </summary>
	public void StopFasterAnimation()
	{
		Hide();
		foreach (Animator anim in m_fasterUIAnimators)
		{
			anim.gameObject.SetActive(false);
		}
	}
	
	/// <summary>
	/// Shows the "faster" UI.
	/// </summary>
	public void Show()
	{
		m_rootObj.SetActive(true);
	}

	/// <summary>
	/// Hides the "faster" UI.
	/// </summary>
	public void Hide()
	{
		m_rootObj.SetActive(false);
	}

	/// <summary>
	/// Pauses the faster UI animation.
	/// </summary>
	public void Pause()
	{
		m_isPaused = true;

		// Assume all faster UI animations have the same animator speed
		m_animSpeedBeforePause = m_fasterUIAnimators[0].speed;
		foreach (Animator anim in this.GetComponentsInChildren<Animator>(true))
		{
			anim.speed = 0.0f;
		}
		// Pause faster SFX
		if (m_fasterSFX != null)
		{
			m_fasterSFX.Pause();
		}
	}

	/// <summary>
	/// Unpauses the faster UI animation.
	/// </summary>
	public void Unpause()
	{
		if (!m_isPaused)
		{
			return;
		}
		m_isPaused = false;

		foreach (Animator anim in this.GetComponentsInChildren<Animator>(true))
		{
			anim.speed = m_animSpeedBeforePause;
		}
		// Unpause faster SFX
		if (m_fasterSFX != null)
		{
			m_fasterSFX.Unpause();
		}
	}

	/// <summary>
	/// Resets ththe faster UI.
	/// </summary>
	public void Reset()
	{
		StopFasterAnimation();
		Unpause();
	}

	/// <summary>
	/// Gets whether FasterUI has been initialized.
	/// </summary>
	public bool IsInitialized
	{
		get { return m_isInitialized; }
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private GameObject 	m_rootObj			= null;
	[SerializeField] private Animator[] 	m_fasterUIAnimators	= null;

	#endregion // Serialized Variables

	#region Variables

	private bool m_isPaused = false;
	private bool m_isInitialized = false;

	private float m_animSpeedBeforePause = 0.0f;

	private SoundObject	m_fasterSFX = null;

	#endregion // Variables

	#region MonoBehaviour
	
	/// <summary>
	/// Awake this instance.
	/// </summary>
	private void Awake()
	{

	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start()
	{
		
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update()
	{
		
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	private void OnDestroy()
	{

	}

	#endregion // MonoBehaviour
}
