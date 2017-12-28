/******************************************************************************
*  @file       MainMenuUI.cs
*  @brief      Handles the main menu UI
*  @author     Ron
*  @date       July 28, 2015
*      
*  @par [explanation]
*		> Loads the minigames when Play Game button is tapped
*		> Plays the Dumb Ways to Die music video when Play Video button is tapped
*		> Opens the settings menu when Setting button is tapped
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class MainMenuUI : MonoBehaviour
{
	#region Public Interface

	/// <summary>
	/// Initialize this instance.
	/// </summary>
	public void Initialize()
	{
		m_playGameButton.Initialize(PlayGameHandler, UIButton.TriggerType.ON_RELEASE);
		m_playVideoButton.Initialize(PlayVideoHandler, UIButton.TriggerType.ON_RELEASE);
		m_settingsButton.Initialize(SettingsHandler, UIButton.TriggerType.ON_RELEASE);

		// TODO: Delegates should not be in Main
		// Button sound delegates
		m_playGameButton.AddSoundDelegates(Main.Instance.UIButtonPressHandler, Main.Instance.UIButtonReleaseHandler);
		m_playVideoButton.AddSoundDelegates(Main.Instance.UIButtonPressHandler, Main.Instance.UIButtonReleaseHandler);
		m_settingsButton.AddSoundDelegates(Main.Instance.UIButtonPressHandler, Main.Instance.UIButtonReleaseHandler);
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private UIButton	m_playGameButton	= null;
	[SerializeField] private UIButton	m_playVideoButton	= null;
	[SerializeField] private UIButton	m_settingsButton	= null;

	#endregion // Serialized Variables

	#region Input Handling

	/// <summary>
	/// Loads the minigames when Play Game button is tapped.
	/// </summary>
	private void PlayGameHandler(object sender, System.EventArgs e)
	{
		// Notify SceneMaster
		MainMenuSceneMaster sceneMaster = (MainMenuSceneMaster)Locator.GetSceneMaster();
		if (sceneMaster != null)
		{
			sceneMaster.NotifyPlayGameButtonTapped();
		}
	}

	/// <summary>
	/// Plays the Dumb Ways to Die music video when Play Video button is tapped.
	/// </summary>
	private void PlayVideoHandler(object sender, System.EventArgs e)
	{
		// Notify SceneMaster
		MainMenuSceneMaster sceneMaster = (MainMenuSceneMaster)Locator.GetSceneMaster();
		if (sceneMaster != null)
		{
			sceneMaster.NotifyPlayVideoButtonTapped();
		}
	}

	/// <summary>
	/// Opens the Settings menu when Settings button is tapped.
	/// </summary>
	private void SettingsHandler(object sender, System.EventArgs e)
	{
		// Notify SceneMaster
		MainMenuSceneMaster sceneMaster = (MainMenuSceneMaster)Locator.GetSceneMaster();
		if (sceneMaster != null)
		{
			sceneMaster.NotifySettingsButtonTapped();
		}
	}

	#endregion // Input Handling

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
