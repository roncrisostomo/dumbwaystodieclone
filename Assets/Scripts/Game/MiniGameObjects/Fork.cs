/******************************************************************************
*  @file       Fork.cs
*  @brief      Handles the fork object in the Fork MiniGame
*  @author     Ron
*  @date       August 18, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;
using TouchScript;
using TouchScript.Gestures.Simple;

#endregion // Namespaces

public class Fork : MonoBehaviour
{
	#region Public Interface

	/// <summary>
	/// Initializes this instance.
	/// </summary>
	public void Initialize(ForkMGSceneMaster sceneMaster, float toasterExitY)
	{
		m_sceneMaster = sceneMaster;
		m_toasterExitY = toasterExitY;

		// Note the fork and fork panner's starting vertical positions
		m_startY = this.transform.position.y;
		m_pannerStartY = m_forkPannerGesture.transform.position.y;

		// Subscribe to panner gesture events
		m_forkPannerGesture.PanStarted += OnForkSlideStarted;
		m_forkPannerGesture.Panned += OnForkSlide;
		m_forkPannerGesture.PanCompleted += OnForkSlideCompleted;

		// Set the initialized flag
		m_isInitialized = true;
	}

	/// <summary>
	/// Pauses this instance.
	/// </summary>
	public void Pause()
	{
		m_isPaused = true;

		// Temporarily disable panner
		m_forkPannerGesture.enabled = false;
	}

	/// <summary>
	/// Unpauses this instance.
	/// </summary>
	public void Unpause()
	{
		m_isPaused = false;

		// Re-enable panner
		m_forkPannerGesture.enabled = true;
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private SimplePanGesture m_forkPannerGesture = null;

	[SerializeField] private float m_slideDownSpeed	= 1.0f;

	#endregion // Serialized Variables

	#region Variables

	private ForkMGSceneMaster m_sceneMaster = null;

	private bool m_isInitialized = false;

	private bool m_isPaused = false;

	#endregion // Variables

	#region Movement

	private float 		m_startY 			= 0.0f;
	private float		m_pannerStartY		= 0.0f;
	private float 		m_toasterExitY 		= 0.0f;
	private bool 		m_isBeingPulled 	= false;

	private SoundObject m_slideSound 		= null;

	/// <summary>
	/// Raises the fork slide started event.
	/// </summary>
	private void OnForkSlideStarted(object sender, System.EventArgs e)
	{
		m_isBeingPulled = true;

		// Play "slide" sound
		if (m_slideSound == null)
		{
			m_slideSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.FORK_SLIDE);
		}
		else
		{
			if (!m_slideSound.IsPlaying)
			{
				m_slideSound.Play();
			}
		}
	}

	/// <summary>
	/// Raises the fork slide event.
	/// </summary>
	private void OnForkSlide(object sender, System.EventArgs e)
	{
		// Clamp downward panning to the fork panner's initial vertical position
		if (m_forkPannerGesture.transform.position.y < m_pannerStartY)
		{
			m_forkPannerGesture.transform.SetPosY(m_pannerStartY);
		}
	}

	/// <summary>
	/// Raises the fork slide completed event.
	/// </summary>
	private void OnForkSlideCompleted(object sender, System.EventArgs e)
	{
		m_isBeingPulled = false;
	}
	
	/// <summary>
	/// Updates fork behavior outside of movement via pan gesture.
	/// </summary>
	private void UpdateForkBehavior()
	{
		// If fork is not being pulled, let it gradually slide back down the toaster
		if (!m_isBeingPulled)
		{
			this.transform.Translate(Vector3.down * m_slideDownSpeed * Time.deltaTime);
		}
		
		// Clamp the fork's downward movement to its starting vertical position
		if (this.transform.position.y < m_startY)
		{
			this.transform.SetPosY(m_startY);
		}
	}

	/// <summary>
	/// Checks if fork has exited the toaster.
	/// </summary>
	private void CheckToasterExit()
	{
		if (this.GetComponent<Collider2D>().bounds.min.y > m_toasterExitY)
		{
			m_sceneMaster.NotifyForkExitedToaster();
			Disable();
		}
	}

	/// <summary>
	/// Disables fork behaviour.
	/// </summary>
	private void Disable()
	{
		Pause();
		Unsubscribe();
		m_forkPannerGesture.enabled = false;
	}

	/// <summary>
	/// Unsubscribes from panner gesture events
	/// </summary>
	private void Unsubscribe()
	{
		m_forkPannerGesture.PanStarted -= OnForkSlideStarted;
		m_forkPannerGesture.Panned -= OnForkSlide;
		m_forkPannerGesture.PanCompleted -= OnForkSlideCompleted;
	}

	#endregion // Movement

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
		if (m_isPaused || !m_isInitialized)
		{
			return;
		}

		UpdateForkBehavior();
		CheckToasterExit();
	}

	/// <summary>
	/// Raises the trigger enter 2D event.
	/// </summary>
	private void OnTriggerEnter2D(Collider2D col)
	{
		Toaster toaster = col.GetComponent<Toaster>();
		if (toaster != null)
		{
			m_sceneMaster.NotifyForkTouchedToaster();
			Disable();
		}
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	private void OnDestroy()
	{

	}

	#endregion // MonoBehaviour
}
