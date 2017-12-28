/******************************************************************************
*  @file       PauseUI.cs
*  @brief      Handles the in-game pause UI
*  @author     Ron
*  @date       August 6, 2015
*      
*  @par [explanation]
*		> Handles pause button behavior
*		> Displays the pause UI when pause button is tapped
*		> Hides the pause UI when continue button is tapped
*		> Notifies game to return to title scene when quit button is tapped
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class PauseUI : MonoBehaviour
{
	#region Public Interface

	/// <summary>
	/// Initialize this instance.
	/// </summary>
	public void Initialize()
	{
		if (m_isInitialized)
		{
			return;
		}

		m_pauseButton.Initialize(PauseHandler, UIButton.TriggerType.ON_RELEASE);
		m_continueButton.Initialize(ContinueHandler, UIButton.TriggerType.ON_RELEASE);
		m_quitButton.Initialize(QuitHandler, UIButton.TriggerType.ON_RELEASE);

		// TODO: Delegates should not be in Main
		// Button sound delegates
		m_pauseButton.AddSoundDelegates(Main.Instance.UIButtonPressHandler, Main.Instance.UIButtonReleaseHandler);
		m_continueButton.AddSoundDelegates(Main.Instance.UIButtonPressHandler, Main.Instance.UIButtonReleaseHandler);
		m_quitButton.AddSoundDelegates(Main.Instance.UIButtonPressHandler, Main.Instance.UIButtonReleaseHandler);

		// Position pause button in upper right of screen
		UpdatePauseButtonPosition();

		// Start hidden
		Hide();

		// Set the initialized flag
		m_isInitialized = true;
	}

	/// <summary>
	/// Shows the pause UI.
	/// </summary>
	public void Show()
	{
		m_pauseButton.gameObject.SetActive(true);
		ShowPauseMenu(false);
	}

	/// <summary>
	/// Hides the pause UI.
	/// </summary>
	public void Hide()
	{
		m_pauseButton.gameObject.SetActive(false);
		ShowPauseMenu(false);
	}

	/// <summary>
	/// Resets the pause UI to its default state - pause button shown, everything else hidden.
	/// </summary>
	public void Reset()
	{
		m_isPaused = false;

		// Hide pause menu
		ShowPauseMenu(false);
		// Reset and show pause button
		m_pauseButton.Reset();
		m_pauseButton.gameObject.SetActive(true);
	}

	/// <summary>
	/// Pauses the game.
	/// </summary>
	public void PauseGame()
	{
		if (m_isPaused)
		{
			return;
		}
		m_isPaused = true;
		
		// Bring up pause menu
		ShowPauseMenu();
		
		// Pause game
		Locator.GetSceneMaster().NotifyPause();
	}

	/// <summary>
	/// Gets whether PauseUI is "paused'.
	/// </summary>
	public bool IsPaused
	{
		get { return m_isPaused; }
	}

	/// <summary>
	/// Gets whether PauseUI is initialized.
	/// </summary>
	public bool IsInitialized
	{
		get { return m_isInitialized; }
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private UIButton	m_pauseButton		= null;
	[SerializeField] private UIButton	m_continueButton	= null;
	[SerializeField] private UIButton	m_quitButton		= null;
	[SerializeField] private GameObject	m_pauseOverlay		= null;
	[SerializeField] private GameObject	m_pauseText			= null;
	[SerializeField] private GameObject	m_pauseCharacter	= null;

	[SerializeField] private float 		m_pauseButtonCornerOffset	= 2.0f;

	#endregion // Serialized Variables

	#region Variables

	private bool m_isInitialized = false;
	private bool m_isPaused = false;

	#endregion // Variables

	#region Input Handling

	/// <summary>
	/// Brings up the pause menu when the pause button is tapped.
	/// </summary>
	private void PauseHandler(object sender, System.EventArgs e)
	{
		PauseGame();
	}

	/// <summary>
	/// Closes the pause menu and resumes the game when the continue button is tapped.
	/// </summary>
	private void ContinueHandler(object sender, System.EventArgs e)
	{
		// Return pause UI to default state
		Reset();

		// Resume game
		Locator.GetSceneMaster().NotifyUnpause();
	}

	/// <summary>
	/// Quits the game and loads the main menu when the quit button is tapped.
	/// </summary>
	private void QuitHandler(object sender, System.EventArgs e)
	{
		// Quit game
		Locator.GetSceneMaster().NotifyQuit();
	}

	#endregion // Input Handling

	#region Visibility Handling

	/// <summary>
	/// Shows or hides the pause menu (not including the pause button).
	/// </summary>
	/// <param name="show">If set to <c>true</c> show the pause menu.</param>
	private void ShowPauseMenu(bool show = true)
	{
		m_continueButton.gameObject.SetActive(show);
		m_quitButton.gameObject.SetActive(show);
		m_pauseOverlay.SetActive(show);
		m_pauseText.SetActive(show);
		m_pauseCharacter.SetActive(show);
	}

	#endregion // Visibility Handling

	#region Position

	/// <summary>
	/// Updates the pause button position.
	/// </summary>
	private void UpdatePauseButtonPosition()
	{
		// Get UI camera
		SceneMasterBase sceneMaster = Locator.GetSceneMaster();
		if (sceneMaster != null)
		{
			UICamera uiCamera = sceneMaster.UICamera;

			// (Continue and quit button positions are preset in prefab)
			m_pauseButton.transform.position = new Vector3(uiCamera.ScreenMaxWorld.x - m_pauseButtonCornerOffset,
		    	                                           uiCamera.ScreenMaxWorld.y - m_pauseButtonCornerOffset,
			                                               0.0f);
		}
	}

	#endregion // Position

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
		UpdatePauseButtonPosition();
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	private void OnDestroy()
	{

	}

	#endregion // MonoBehaviour
}
