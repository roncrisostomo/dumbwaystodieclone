/******************************************************************************
*  @file       TitleUI.cs
*  @brief      Handles the title UI
*  @author     Ron
*  @date       July 28, 2015
*      
*  @par [explanation]
*		> Loads the main menu scene when Start button is tapped
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class TitleUI : MonoBehaviour
{
	#region Public Interface

	/// <summary>
	/// Initialize this instance.
	/// </summary>
	public void Initialize()
	{
		m_startButton.Initialize(StartHandler, UIButton.TriggerType.ON_RELEASE);

		// TODO: Delegate should not be in Main
		// Button sound delegate
		m_startButton.AddSoundDelegates(Main.Instance.UIButtonPressHandler, Main.Instance.UIButtonReleaseHandler);
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private UIButton m_startButton	= null;

	#endregion // Serialized Variables

	#region Input Handling

	/// <summary>
	/// Loads the main menu scene when Start button is tapped.
	/// </summary>
	private void StartHandler(object sender, System.EventArgs e)
	{
		// Notify SceneMaster
		TitleSceneMaster sceneMaster = (TitleSceneMaster)Locator.GetSceneMaster();
		if (sceneMaster != null)
		{
			sceneMaster.NotifyStartButtonTapped();
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
