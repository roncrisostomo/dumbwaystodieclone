/******************************************************************************
*  @file       SettingsSceneMaster.cs
*  @brief      Handles the lifetime of a single scene
*  @author     Ron
*  @date       August 20, 2015
*      
*  @par [explanation]
*		> Loads and unloads resources for one scene
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class SettingsSceneMaster : SceneMasterBase
{
	#region Public Interface

	public override bool Load()
	{
		m_isInitialized = true;
		return true;
	}

	public override bool Unload()
	{
		m_isInitialized = false;
		return true;
	}

	public override void StartScene()
	{
		m_settingsUI.Initialize();
	}

	/// <summary>
	/// Notifies of Back To Menu button tapped event.
	/// </summary>
	public void NotifyBackToMenuButtonTapped()
	{
		Main.Instance.NotifySwitchScene(SceneInfo.SceneEnum.MAIN_MENU);
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private SettingsUI m_settingsUI = null;

	#endregion // Serialized Variables

	#region MonoBehaviour

	/// <summary>
	/// Awake this instance.
	/// </summary>
	protected override void Awake()
	{
		base.Awake();
	}
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	protected override void Start()
	{
		base.Start();
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	protected override void Update()
	{
		base.Update();

		// Check escape input
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			// If credits UI or reset dialog is shown, hide it
			if (m_settingsUI.IsCreditsShown())
			{
				m_settingsUI.BackFromCredits();
			}
			else if (m_settingsUI.IsResetDialogShown())
			{
				m_settingsUI.BackFromResetDialog();
			}
			// Else, return to the Main Menu scene
			else
			{
				Main.Instance.NotifySwitchScene(SceneInfo.SceneEnum.MAIN_MENU);
			}
		}
	}
	
	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	#endregion // MonoBehaviour
}
