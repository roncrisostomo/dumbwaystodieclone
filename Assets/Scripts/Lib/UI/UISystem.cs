/******************************************************************************
*  @file       UISystem.cs
*  @brief      Handles all UI activity in the game
*  @author     
*  @date       July 12, 2015
*      
*  @par [explanation]
*		> Tracks all UIElements
*		> Creates UIElements in runtime (factory pattern)
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class UISystem : UISystemBase
{
	#region Public Interface

	/// <summary>
	/// Initialize this instance.
	/// </summary>
	public override bool Initialize()
	{
		// Implement
		m_isInitialized = true;
		return true;
	}

	#endregion // Public Interface

	#region Serialized Variables

	#endregion // Serialized Variables

	#region Initialization

	#endregion // Initialization

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
