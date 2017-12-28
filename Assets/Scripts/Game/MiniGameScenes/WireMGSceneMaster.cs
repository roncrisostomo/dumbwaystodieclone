/******************************************************************************
*  @file       WireMGSceneMaster.cs
*  @brief      Handles the Wire MiniGame scene
*  @author     Ron
*  @date       August 18, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript;

#endregion // Namespaces

public class WireMGSceneMaster : MiniGameSceneMasterBase
{
	#region Public Interface

	/// <summary>
	/// Notifies of a connection between two wires.
	/// </summary>
	/// <param name="wire1">Wire1.</param>
	/// <param name="wire2">Wire2.</param>
	public void NotifyWireConnect(Wire wire1, Wire wire2)
	{
		Connect(wire1, wire2);

		m_connectedWireCount++;

		// Game is won when all wire pairs have been connected
		if (m_connectedWireCount == m_wiresToConnectCount)
		{
			StopGame(true);
		}
	}

	/// <summary>
	/// Notifies of a mismatch between two wire sections.
	/// </summary>
	public void NotifyWireMismatch()
	{
		// Game over
		StopGame(false);
	}
	
	/// <summary>
	/// Notifies of scene pause.
	/// </summary>
	public override void NotifyPause()
	{
		base.NotifyPause();

		// Wire instances will be notified of scene pause via their InteractiveObject base class
	
		m_wireEndAnimEvents.Pause();
	}
	
	/// <summary>
	/// Notifies of scene unpause.
	/// </summary>
	public override void NotifyUnpause()
	{
		base.NotifyUnpause();

		// Wire instances will be notified of scene unpause via their InteractiveObject base class
	
		m_wireEndAnimEvents.Unpause();
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private	int[]		m_wirePairsPerLevel 	= new int[]{ 2, 3, 3, 4, 4, 4 };

	[Header("Wires")]
	[SerializeField] private	Wire[]		m_topWires		= null;
	[SerializeField] private	Wire[]		m_bottomWires	= null;
	[Header("Wire connectors")]
	[SerializeField] private	SpriteRenderer[] 	m_connectors	= null;
	[SerializeField] private	Transform 	m_connectorsContainer 	= null;
	[Header("Wire colors")]
	[SerializeField] private 	Color[]		m_wireColors	= null;

	[SerializeField] private	GameObject	m_connectGuide	= null;
	[SerializeField] private	GameObject	m_electricBolt	= null;

	[SerializeField] private	GameObject	m_endScreen		= null;
	[SerializeField] private	Animator	m_endAnimCommon	= null;
	[SerializeField] private	Animator	m_endAnimWin	= null;
	[SerializeField] private	Animator	m_endAnimLose	= null;
	[SerializeField] private	WireEndAnimEvents m_wireEndAnimEvents = null;

	#endregion // Serialized Variables

	#region Resource Loading

	/// <summary>
	/// Loads the resources.
	/// </summary>
	protected override bool LoadResources()
	{
		return true;
	}

	/// <summary>
	/// Unloads the resources.
	/// </summary>
	protected override bool UnloadResources()
	{
		return true;
	}

	#endregion // Resource Loading

	#region Level and Game Timer

	/// <summary>
	/// Raises the time run out event.
	/// </summary>
	protected override void OnTimeRunOut()
	{
		StopGame(false);
	}

	#endregion // Level and Game Timer

	#region Gameplay

	/// <summary>
	/// Starts the game.
	/// </summary>
	protected override void StartGame()
	{
		m_wiresToConnectCount = m_wirePairsPerLevel[m_level];
		
		int wirePairsMax = m_topWires.Length;
		Wire.WireType wireType = Wire.WireType.TYPE0;
		
		for (int wirePairNo = 0; wirePairNo < m_wiresToConnectCount; ++wirePairNo)
		{
			// Randomly select an inactive wire in the top group
			int index = 0;
			do
			{
				index = Random.Range(0, wirePairsMax);
			}
			while (m_topWires[index].gameObject.activeSelf);
			// Enable and initialize top wire
			m_topWires[index].gameObject.SetActive(true);
			m_topWires[index].Initialize(this, wireType, m_wireColors[(int)wireType], m_connectGuide, m_electricBolt);
			AddToInteractiveObjectList(m_topWires[index]);

			// Randomly select an inactive wire in the bottom group
			do
			{
				index = Random.Range(0, wirePairsMax);
			}
			while (m_bottomWires[index].gameObject.activeSelf);
			// Enable and initialize bottom wire
			m_bottomWires[index].gameObject.SetActive(true);
			m_bottomWires[index].Initialize(this, wireType, m_wireColors[(int)wireType], m_connectGuide, m_electricBolt);
			AddToInteractiveObjectList(m_bottomWires[index]);

			// Assign a different wire type to the next wire pair
			wireType = (Wire.WireType)((int)wireType + 1);
		}
	}

	/// <summary>
	/// Updates the game.
	/// </summary>
	protected override void UpdateGame()
	{

	}

	/// <summary>
	/// Raises the stop game event.
	/// </summary>
	protected override void OnStopGame()
	{
		// Notify top wires
		foreach (Wire topWire in m_topWires)
		{
			if (topWire.gameObject.activeSelf)
			{
				topWire.NotifyStopGame();
			}
		}
		// Notify bottom wires
		foreach (Wire bottomWire in m_bottomWires)
		{
			if (bottomWire.gameObject.activeSelf)
			{
				bottomWire.NotifyStopGame();
			}
		}
	}

	#endregion // Gameplay

	#region Wire

	private	int m_wiresToConnectCount	= 0;
	private int m_connectedWireCount 	= 0;

	/// <summary>
	/// Connects the two wires.
	/// </summary>
	/// <param name="wire1">Wire1.</param>
	/// <param name="wire2">Wire2.</param>
	private void Connect(Wire wire1, Wire wire2)
	{
		// Instantiate a connector between the two wires
		int connectorType = Mathf.Abs(wire1.GroupIndex - wire2.GroupIndex);
		GameObject connector = GameObject.Instantiate(m_connectors[connectorType].gameObject) as GameObject;
		connector.SetActive(true);
		connector.transform.parent = m_connectorsContainer;
		// Position the connector in the middle of the two wires
		connector.transform.SetPosX((wire1.transform.position.x + wire2.transform.position.x) * 0.5f);
		
		// If top wire has higher group index than bottom wire, flip the connector in the y axis
		if ((wire1.GetWireGroup() == Wire.WireGroup.TOP && wire1.GroupIndex > wire2.GroupIndex) ||
		    (wire2.GetWireGroup() == Wire.WireGroup.TOP && wire2.GroupIndex > wire1.GroupIndex))
		{
			connector.transform.SetRotY(180.0f);
		}
		
		// Give the connector the same color as the wires it connects
		SpriteRenderer renderer = connector.GetComponent<SpriteRenderer>();
		renderer.color = m_wireColors[(int)wire1.GetWireType()];
	}

	#endregion // Wire

	#region Ending Animation

	/// <summary>
	/// Starts the win animation.
	/// </summary>
	protected override void StartWinAnimation()
	{
		m_endScreen.SetActive(true);
		m_endAnimCommon.gameObject.SetActive(true);
		// The win animator will be activated by the "common" animator
		//m_endAnimWin.gameObject.SetActive(true);
		m_endAnimLose.gameObject.SetActive(false);
		AddToAnimatorList(m_endAnimCommon);
		AddToAnimatorList(m_endAnimWin);

		m_wireEndAnimEvents.SetResult(true);
	}

	/// <summary>
	/// Updates the win animation.
	/// </summary>
	protected override void UpdateWinAnimation()
	{

	}
	
	/// <summary>
	/// Starts the lose animation.
	/// </summary>
	protected override void StartLoseAnimation()
	{
		m_endScreen.SetActive(true);
		m_endAnimCommon.gameObject.SetActive(true);
		m_endAnimWin.gameObject.SetActive(false);
		// The lose animator will be activated by the "common" animator
		//m_endAnimLose.gameObject.SetActive(true);
		AddToAnimatorList(m_endAnimCommon);
		AddToAnimatorList(m_endAnimLose);

		m_wireEndAnimEvents.SetResult(false);
	}

	/// <summary>
	/// Updates the lose animation.
	/// </summary>
	protected override void UpdateLoseAnimation()
	{
		
	}

	#endregion // Ending Animation
}
