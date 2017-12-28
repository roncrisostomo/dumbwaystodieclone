/******************************************************************************
*  @file       GameManagerSceneMaster.cs
*  @brief      Handles the lifetime of a single scene
*  @author     Lori
*  @date       July 23, 2015
*      
*  @par [explanation]
*		> Loads and unloads resources for one scene
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class GameManagerSceneMaster : SceneMasterBase
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
		if (m_gameManager == null)
		{
			Debug.LogError("GameManager Scene: Game Manager instance not found.");
			return;
		}

		if (m_soundManager == null)
		{
			Debug.LogError("GameManager Scene: Sound Manager instance not found.");
			return;
		}

		// Initialize the game settings
		m_gameManager.InitializeGameSettings();

		// Go to the next scene
		Main.Instance.NotifySwitchScene(m_nextScene);
	}
	
	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private GameManager 			m_gameManager	= null;
	[SerializeField] private SoundManager			m_soundManager	= null;
	[SerializeField] private SceneInfo.SceneEnum	m_nextScene		= SceneInfo.SceneEnum.TITLE;

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
