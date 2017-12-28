/******************************************************************************
*  @file       BalloonMGSceneMaster.cs
*  @brief      Handles the Balloon MiniGame scene
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

public class BalloonMGSceneMaster : MiniGameSceneMasterBase
{
	#region Public Interface
	
	/// <summary>
	/// Notifies of scene pause.
	/// </summary>
	public override void NotifyPause()
	{
		base.NotifyPause();

		// Balloon instances will be notified of scene pause via their InteractiveObject base class

		// Pause end animation sounds, etc
		foreach (BalloonEndAnimEvents eventObj in m_endAnimEventObjects)
		{
			eventObj.Pause();
		}
	}
	
	/// <summary>
	/// Notifies of scene unpause.
	/// </summary>
	public override void NotifyUnpause()
	{
		base.NotifyUnpause();

		// Balloon instances will be notified of scene unpause via their InteractiveObject base class
	
		// Unpause end animation sounds, etc
		foreach (BalloonEndAnimEvents eventObj in m_endAnimEventObjects)
		{
			eventObj.Unpause();
		}
	}

	/// <summary>
	/// Notifies of balloon connecting to hand.
	/// </summary>
	public void NotifyBalloonCollect(int collectedBalloonCount)
	{
		// Game is won when all balloons have been collected
		if (collectedBalloonCount == m_balloonsPerLevel[m_level])
		{
			m_lastBalloonCollected = true;
		}
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private 	int[] 		m_balloonsPerLevel	= new int[]{ 2, 3, 3, 4, 4, 5 };
	[SerializeField] private	Transform[]	m_collectedBalloonPositions = null;

	[Header("Balloon Spawn Area")]
	[SerializeField] private 	Transform	m_spawnAreaMin		= null;
	[SerializeField] private 	Transform	m_spawnAreaMax		= null;
	// Minimum distance between any two balloons
	[SerializeField] private	float		m_balloonDistMin 	= 1.5f;
	// Maximum distance from any balloon to the center balloon
	[SerializeField] private	float		m_balloonDistMax 	= 2.5f;

	[SerializeField] private	Hand		m_hand				= null;
	[SerializeField] private	Animator	m_handAnimator		= null;

	[Header("End animations")]
	[SerializeField] private	GameObject	m_endScreen			= null;
	[SerializeField] private	Animator	m_endAnimWin		= null;
	[SerializeField] private	Animator	m_endAnimLose		= null;
	[SerializeField] private	Animator	m_endAnimCommon		= null;
	[SerializeField] private	Animator	m_endAnimCharWin	= null;
	[SerializeField] private	Animator	m_endAnimCharLose	= null;

	[SerializeField] private	BalloonEndAnimEvents[] m_endAnimEventObjects = null;

	[SerializeField] private	Balloon[]	m_endAnimCollectedBalloons	= null;
	[SerializeField] private	Balloon[]	m_endAnimLostBalloons		= null;
	
	#endregion // Serialized Variables

	#region Resource Loading

	private GameObject m_balloonResource = null;

	private const string BALLOON_PREFAB_PATH = "Prefabs/MiniGameObjects/Balloon";

	/// <summary>
	/// Loads the resources.
	/// </summary>
	protected override bool LoadResources()
	{
		// Load balloon resource
		m_balloonResource = Resources.Load(BALLOON_PREFAB_PATH, typeof(GameObject)) as GameObject;
		if (m_balloonResource == null)
		{
			Debug.LogError("Prefab not found at " + BALLOON_PREFAB_PATH);
			return false;
		}

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
		if (!m_lastBalloonCollected)
		{
			StopGame(false);
		}
	}

	#endregion // Level and Game Timer

	#region Gameplay

	/// <summary>
	/// Starts the game.
	/// </summary>
	protected override void StartGame()
	{	
		// Spawn balloons
		int balloonCount = m_balloonsPerLevel[m_level];
		SpawnBalloons(balloonCount);

		// Initialize hand
		m_hand.Initialize(this);
		AddToAnimatorList(m_handAnimator);
	}

	/// <summary>
	/// Updates the game.
	/// </summary>
	protected override void UpdateGame()
	{
		UpdateWinAnimDelay();
	}

	/// <summary>
	/// Raises the stop game event.
	/// </summary>
	protected override void OnStopGame()
	{
		// Notifies balloons
		foreach (Balloon balloon in m_balloons)
		{
			balloon.NotifyStopGame();
		}
	}

	#endregion // Gameplay

	#region Balloon

	private List<Balloon> m_balloons = new List<Balloon>();

	/// <summary>
	/// Spawns balloons.
	/// </summary>
	/// <param name="balloonCount">Number of balloons to spawn.</param>
	private void SpawnBalloons(int balloonCount)
	{
		if (balloonCount <= 0)
		{
			return;
		}

		// Spawn balloons in a clump within the spawn area
		Vector3 centerPos = new Vector3(Random.Range(m_spawnAreaMin.position.x, m_spawnAreaMax.position.x),
		                                Random.Range(m_spawnAreaMin.position.y, m_spawnAreaMax.position.y),
		                                0.0f);

		// Spawn and initialize center balloon
		Balloon centerBalloon = SpawnBalloon(centerPos);
		float colorFactor = 1.0f;
		const float DELTA_COLOR_FACTOR = 0.03f;
		int sortingOrder = balloonCount;
		centerBalloon.Initialize(m_collectedBalloonPositions[m_balloons.Count - 1].position,
		                         m_hand.BalloonGrabPos, colorFactor, sortingOrder);
		// Each successive balloon has a darker shade than the previous one
		colorFactor -= DELTA_COLOR_FACTOR;
		// Each successive balloon is placed behind the previous one
		sortingOrder--;
		
		// Spawn the other balloons (if any)
		while (m_balloons.Count < balloonCount)
		{
			// Spawn balloons within a certain distance from the center
			float distFromCenter = Random.Range(m_balloonDistMin, m_balloonDistMax);
			Vector3 dirFromCenter = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0.0f);
			Vector3 pos = centerPos + distFromCenter * dirFromCenter.normalized;

			// Make sure there is enough space between balloons
			bool isSpaced = true;
			float minDistSq = m_balloonDistMin * m_balloonDistMin;
			foreach (Balloon balloon in m_balloons)
			{
				if ((balloon.transform.position - pos).sqrMagnitude < minDistSq)
				{
					isSpaced = false;
					break;
				}
			}
			// If not spaced enough from other balloons, re-randomize a new position
			if (!isSpaced)
			{
				continue;
			}

			// Spawn and initialize balloon
			Balloon newBalloon = SpawnBalloon(pos);
			newBalloon.Initialize(m_collectedBalloonPositions[m_balloons.Count - 1].position,
			                      m_hand.BalloonGrabPos, colorFactor, sortingOrder);
			// Each successive balloon has a darker shade than the previous one
			colorFactor -= DELTA_COLOR_FACTOR;
			// Each successive balloon is placed behind the previous one
			sortingOrder--;
		}
	}

	/// <summary>
	/// Spawns a balloon.
	/// </summary>
	/// <returns>The balloon.</returns>
	/// <param name="pos">Position.</param>
	private Balloon SpawnBalloon(Vector3 pos)
	{
		GameObject balloonObj = GameObject.Instantiate(m_balloonResource) as GameObject;
		balloonObj.transform.parent = DynamicObjects;
		balloonObj.transform.position = pos;

		Balloon balloon = balloonObj.AddComponentNoDupe<Balloon>();
		m_balloons.Add(balloon);
		AddToInteractiveObjectList(balloon);
		return balloon;
	}

	#endregion // Balloon

	#region Ending Animation

	private bool m_lastBalloonCollected = false;

	// Time from collecting the last balloon until the win animation starts
	// This is to allow the string to properly connect to the hand before the win animation
	private	float m_delayUntilWinAnim	= 0.1f;

	/// <summary>
	/// Updates the time delay from collecting the last balloon to the start of the win animation.
	/// </summary>
	private void UpdateWinAnimDelay()
	{
		if (m_lastBalloonCollected)
		{
			m_delayUntilWinAnim -= Time.deltaTime;
			if (m_delayUntilWinAnim < 0.0f)
			{
				StopGame(true);
			}
		}
	}

	/// <summary>
	/// Initializes collected and lost balloons in the end animation.
	/// </summary>
	private void InitializeEndAnimBalloons()
	{
		bool isGameLost = m_hand.CollectedBalloonCount < m_balloons.Count;
		for (int x = 0; x < m_balloons.Count; ++x)
		{
			if (x < m_hand.CollectedBalloonCount)
			{
				m_endAnimCollectedBalloons[x].gameObject.SetActive(true);
				m_endAnimCollectedBalloons[x].InitializeForEndAnim(true, isGameLost);
				AddToInteractiveObjectList(m_endAnimCollectedBalloons[x]);
			}
			else
			{
				m_endAnimLostBalloons[x].gameObject.SetActive(true);
				m_endAnimLostBalloons[x].InitializeForEndAnim(false, isGameLost);
				AddToInteractiveObjectList(m_endAnimLostBalloons[x]);
			}
		}
	}

	/// <summary>
	/// Starts the win animation.
	/// </summary>
	protected override void StartWinAnimation()
	{
		m_endScreen.SetActive(true);
		m_endAnimWin.gameObject.SetActive(true);
		m_endAnimLose.gameObject.SetActive(false);
		m_endAnimCommon.gameObject.SetActive(true);
		AddToAnimatorList(m_endAnimWin);
		AddToAnimatorList(m_endAnimCommon);
		AddToAnimatorList(m_endAnimCharWin);

		InitializeEndAnimBalloons();
	}

	/// <summary>
	/// Updates the win animation.
	/// </summary>
	protected override void UpdateWinAnimation()
	{
		// Return camera to orthographic mode when animation is finished
		if (m_endingAnimationTimer >= m_endingAnimationDuration)
		{
			Camera.main.orthographic = true;
		}
	}
	
	/// <summary>
	/// Starts the lose animation.
	/// </summary>
	protected override void StartLoseAnimation()
	{
		m_endScreen.SetActive(true);
		m_endAnimWin.gameObject.SetActive(false);
		m_endAnimLose.gameObject.SetActive(true);
		m_endAnimCommon.gameObject.SetActive(true);
		AddToAnimatorList(m_endAnimLose);
		AddToAnimatorList(m_endAnimCommon);
		AddToAnimatorList(m_endAnimCharLose);

		InitializeEndAnimBalloons();
	}

	/// <summary>
	/// Updates the lose animation.
	/// </summary>
	protected override void UpdateLoseAnimation()
	{
		
	}

	#endregion // Ending Animation
}
