/******************************************************************************
*  @file       SceneMasterTemplate.cs
*  @brief      Handles the lifetime of a single scene
*  @author     
*  @date       January 1, 2015
*      
*  @par [explanation]
*		> Loads and unloads resources for one scene
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class SceneMasterTemplate : SceneMasterBase
{
	#region Public Interface

	public override bool Load ()
	{
		// Implement
		return true;
	}

	public override bool Unload ()
	{
		// Implement
		return true;
	}

	public override void StartScene ()
	{
		
	}

	#endregion // Public Interface

	#region Serialized Variables

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
