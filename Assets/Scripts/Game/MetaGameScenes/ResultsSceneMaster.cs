/******************************************************************************
*  @file       ResultsSceneMaster.cs
*  @brief      Handles the lifetime of a single scene
*  @author     
*  @date       August 20, 2015
*      
*  @par [explanation]
*		> Loads and unloads resources for one scene
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class ResultsSceneMaster : SceneMasterBase
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
		m_resultsUI.Initialize();
	}

	/// <summary>
	/// Notifies of Play Again button tapped event.
	/// </summary>
	public void NotifyPlayAgainButtonTapped()
	{
		// Start loading minigames
		if (Locator.GetGameManager() != null)
		{
			Locator.GetGameManager().NotifyPlayButtonPressed();
		}
	}

	/// <summary>
	/// Notifies of Main Menu button tapped event.
	/// </summary>
	public void NotifyMainMenuButtonTapped()
	{
		Main.Instance.NotifySwitchScene(SceneInfo.SceneEnum.MAIN_MENU);
	}

	/// <summary>
	/// Gets the results UI.
	/// </summary>
	public ResultsUI GetResultsUI
	{
		get { return m_resultsUI; }
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private ResultsUI m_resultsUI = null;

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
			// If pledge UI is shown, hide it
			if (m_resultsUI.IsPledgeUIShown())
			{
				m_resultsUI.BackFromPledgeUI();
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
