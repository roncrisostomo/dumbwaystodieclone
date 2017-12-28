/******************************************************************************
*  @file       TitleSceneMaster.cs
*  @brief      Handles the lifetime of a single scene
*  @author     
*  @date       July 16, 2015
*      
*  @par [explanation]
*		> Loads and unloads resources for one scene
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class TitleSceneMaster : SceneMasterBase
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
		m_titleUI.Initialize();
	}

	/// <summary>
	/// Notifies of Start button tapped event.
	/// </summary>
	public void NotifyStartButtonTapped()
	{
		Main.Instance.NotifySwitchScene(SceneInfo.SceneEnum.MAIN_MENU);
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private TitleUI m_titleUI = null;

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
