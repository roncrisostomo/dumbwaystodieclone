/******************************************************************************
*  @file       TimerUI.cs
*  @brief      Handles the timer bar at the top of the screen in minigames
*  @author     Ron
*  @date       July 28, 2015
*      
*  @par [explanation]
*		> Depletes the timer bar in sync with minigame time
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class TimerUI : MonoBehaviour
{
	#region Public Interface

	/// <summary>
	/// Initialize this instance.
	/// </summary>
	public void Initialize()
	{
		// Start hidden
		Hide();

		// Set the initialized flag
		m_isInitialized = true;
	}

	/// <summary>
	/// Resets and starts the timer.
	/// </summary>
	/// <param name="timeLimit">Time limit.</param>
	public void StartTimer(float timeLimit = DEFAULT_TIME_LIMIT)
	{
		ResetTimer();
		SetTimeLimit(timeLimit);
		Show();
		m_isRunning = true;
	}
	
	/// <summary>
	/// Resets the timer.
	/// </summary>
	public void ResetTimer()
	{
		m_timeElapsed = 0.0f;
		UpdatePositionAndScale(Locator.GetSceneMaster().UICamera);
	}

	/// <summary>
	/// Pauses the timer.
	/// </summary>
	public void PauseTimer()
	{
		m_isRunning = false;
	}

	/// <summary>
	/// Unpauses the timer.
	/// </summary>
	public void UnpauseTimer()
	{
		m_isRunning = true;
	}

	/// <summary>
	/// Sets the time limit.
	/// </summary>
	/// <param name="timeLimit">Time limit.</param>
	public void SetTimeLimit(float timeLimit)
	{
		m_timeLimit = timeLimit;
	}

	/// <summary>
	/// Shows the timer.
	/// </summary>
	public void Show()
	{
		m_timerBarFill.enabled = true;
		m_timerBarBG.enabled = true;
	}

	/// <summary>
	/// Hides the timer.
	/// </summary>
	public void Hide()
	{
		m_timerBarFill.enabled = false;
		m_timerBarBG.enabled = false;
	}

	/// <summary>
	/// Positions and scales the timer UI to extend from end to end of the top edge of the screen.
	/// </summary>
	/// <param name="uiCamera">UICamera to use in positioning the timer UI.</param>
	public void UpdatePositionAndScale(UICamera uiCamera)
	{
		// Scale the timer bar to fill the horizontal dimension of the screen
		Vector3 newScale = UIUtils.GetSpriteScaleResizedToScreen(m_timerBarFill, uiCamera.GetCamera, 1.0f, 0.0f);
		// Set height (vertical dimension) to a constant
		newScale.y = m_timerBarHeight;
		// Set new scale
		m_timerBarFill.transform.localScale = newScale;
		m_timerBarBG.transform.localScale = newScale;
		
		// Position timer bar at top edge of screen
		Vector3 newPos = new Vector3(uiCamera.ScreenCenterWorld.x,
		                             uiCamera.ScreenMaxWorld.y - m_timerBarFill.bounds.extents.y, 0.0f);
		// Set new position
		m_timerBarFill.transform.position = newPos;
		m_timerBarBG.transform.position = newPos;
	}
	
	/// <summary>
	/// Gets whether the timer is running.
	/// </summary>
	public bool IsRunning
	{
		get { return m_isRunning; }
	}

	/// <summary>
	/// Gets whether TimerUI is initialized.
	/// </summary>
	public bool IsInitialized
	{
		get { return m_isInitialized; }
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private SpriteRenderer m_timerBarFill 	= null;
	[SerializeField] private SpriteRenderer m_timerBarBG 	= null;

	[SerializeField] private float		m_timeLimit			= DEFAULT_TIME_LIMIT;
	[SerializeField] private float		m_timerBarHeight 	= 0.5f;

	#endregion // Serialized Variables

	#region Variables

	private bool 	m_isInitialized = false;
	private bool 	m_isRunning 	= false;
	private	float	m_timeElapsed	= 0.0f;

	#endregion // Variables

	#region Constants

	private const	float	DEFAULT_TIME_LIMIT	= 5.0f;

	#endregion // Constants

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
		if (m_isRunning)
		{
			// Interpolate fill sprite's scale from full width 
			//	(equal to the BG sprite's scale) to 0 in the given time limit
			m_timeElapsed += Time.deltaTime;
			m_timerBarFill.transform.SetScaleX(Mathf.Lerp(m_timerBarBG.transform.localScale.x,
			                                              0.0f,
			                                              m_timeElapsed / m_timeLimit));
			// Interpolate fill sprite's position from screen center to the left screen edge
			m_timerBarFill.transform.SetPosX(Mathf.Lerp(m_timerBarBG.transform.position.x,
			                                            m_timerBarBG.bounds.min.x,
			                                            m_timeElapsed / m_timeLimit));
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
