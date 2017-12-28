/******************************************************************************
*  @file       Hand.cs
*  @brief      Handles the hand in the Balloon MiniGame
*  @author     Ron
*  @date       August 9, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class Hand : MonoBehaviour
{
	#region Public Interface

	/// <summary>
	/// Initializes this instance.
	/// </summary>
	/// <param name="sceneMaster">Scene master.</param>
	public void Initialize(BalloonMGSceneMaster sceneMaster)
	{
		m_sceneMaster = sceneMaster;
	}

	/// <summary>
	/// Adds to the collected balloons.
	/// </summary>
	public void Collect(Balloon balloon)
	{
		if (m_collectedBalloonCount < m_balloonStringTails.Length)
		{
			m_balloonStringTails[m_collectedBalloonCount].SetActive(true);
			
			m_collectedBalloonCount++;

			// If there is at least one balloon collected, stop the hand animation
			if (m_handAnimator.speed > 0.0f)
			{
				m_handAnimator.speed = 0.0f;
				// Reset to the first sprite in the animation
				m_handAnimator.Play("Hand", 0, 1.0f);
			}

			// Notify scene master of how many balloons have been collected so far
			m_sceneMaster.NotifyBalloonCollect(m_collectedBalloonCount);
		}
	}

	/// <summary>
	/// Gets where the balloon strings would be connected to the hand.
	/// </summary>
	public Vector3 BalloonGrabPos
	{
		get { return m_balloonGrabPos.position; }
	}

	/// <summary>
	/// Gets the number of balloons currently collected.
	/// </summary>
	public int CollectedBalloonCount
	{
		get { return m_collectedBalloonCount; }
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private Transform		m_balloonGrabPos		= null;
	[SerializeField] private GameObject[] 	m_balloonStringTails	= null;
	[SerializeField] private Animator		m_handAnimator			= null;

	#endregion // Serialized Variables

	#region Variables

	private BalloonMGSceneMaster m_sceneMaster = null;

	private int m_collectedBalloonCount = 0;

	#endregion // Variables

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
