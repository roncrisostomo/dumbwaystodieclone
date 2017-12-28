/******************************************************************************
*  @file       MainMenuSceneMaster.cs
*  @brief      Handles the lifetime of a single scene
*  @author     Ron
*  @date       August 19, 2015
*      
*  @par [explanation]
*		> Loads and unloads resources for one scene
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class MainMenuSceneMaster : SceneMasterBase
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
		m_mainMenuUI.Initialize();
	}

	/// <summary>
	/// Notifies of Play Game button tapped event.
	/// </summary>
	public void NotifyPlayGameButtonTapped()
	{
		// Start loading minigames
		if (Locator.GetGameManager() != null)
		{
			Locator.GetGameManager().NotifyPlayButtonPressed();
		}
	}

	/// <summary>
	/// Notifies of Play Video button tapped event.
	/// </summary>
	public void NotifyPlayVideoButtonTapped()
	{
#if UNITY_EDITOR
		Debug.Log("Play Dumb Ways To Die music video");
#else
		Handheld.PlayFullScreenMovie("DumbWaysToDie.mp4", Color.black,
		                             FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.Fill);
#endif
	}

	/// <summary>
	/// Notifies of Settings button tapped event.
	/// </summary>
	public void NotifySettingsButtonTapped()
	{
		Main.Instance.NotifySwitchScene(SceneInfo.SceneEnum.SETTINGS);
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private MainMenuUI m_mainMenuUI = null;

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
