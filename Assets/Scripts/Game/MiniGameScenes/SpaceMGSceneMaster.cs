/******************************************************************************
*  @file       SpaceMGSceneMaster.cs
*  @brief      Handles the Space MiniGame scene
*  @author     Ron
*  @date       August 17, 2015
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

public class SpaceMGSceneMaster : MiniGameSceneMasterBase
{
	#region Public Interface

	/// <summary>
	/// Notifies of space man getting his helmet back.
	/// </summary>
	public void NotifyGetHelmet()
	{
		StopGame(true);
	}

	/// <summary>
	/// Notifies of the character head blow up event in the lose animation.
	/// </summary>
	public void NotifyLoseAnimHeadBlowUp()
	{
		//CreateBloodExplosion();
	}

	/// <summary>
	/// Notifies of scene pause.
	/// </summary>
	public override void NotifyPause()
	{
		base.NotifyPause();

		// Pause spaceman
		m_spaceman.Pause();

		// Pause helmet
		m_helmet.Pause();

		// Pause space rocks
		foreach (SpaceRock spaceRock in m_spaceRocks)
		{
			if (spaceRock.gameObject.activeSelf)
			{
				spaceRock.Pause();
			}
		}

		// Pause end animation sounds, etc
		foreach (SpaceEndAnimEvents eventObj in m_endAnimEventObjects)
		{
			eventObj.Pause();
		}

		// Pause blood drops (in lose animation)
		foreach (SpaceBloodDrop bloodDrop in m_bloodDrops)
		{
			bloodDrop.Pause();
		}
	}

	/// <summary>
	/// Notifies of scene unpause.
	/// </summary>
	public override void NotifyUnpause()
	{
		base.NotifyUnpause();
		
		// Unpause spaceman
		m_spaceman.Unpause();
		
		// Unpause helmet
		m_helmet.Unpause();
		
		// Unpause space rocks
		foreach (SpaceRock spaceRock in m_spaceRocks)
		{
			if (spaceRock.gameObject.activeSelf)
			{
				spaceRock.Unpause();
			}
		}

		// Unpause end animation sounds, etc
		foreach (SpaceEndAnimEvents eventObj in m_endAnimEventObjects)
		{
			eventObj.Unpause();
		}

		// Unpause blood drops (in lose animation)
		foreach (SpaceBloodDrop bloodDrop in m_bloodDrops)
		{
			bloodDrop.Unpause();
		}
	}

	#endregion // Public Interface

	#region Serialized Variables

	[Header("Space Rocks")]
	[SerializeField] private	int[]			m_spaceRocksPerLevel 	= new int[]{ 0, 1, 2, 2, 3, 3 };
	// Area where space rocks can be spawned
	[SerializeField] private	Vector3			m_spaceRockSpawnAreaMin	= Vector3.zero;
	[SerializeField] private	Vector3			m_spaceRockSpawnAreaMax	= Vector3.zero;
	// Shift in space rock position above or below the spawn area, to make sure there is
	//	an unblocked path from the spaceman to the helmet
	[SerializeField] private	float			m_spaceRockShiftFromSpawnArea	= 1.0f;
	// Minimum distance between space rocks on spawn
	[SerializeField] private	float			m_minDistBetweenSpaceRocks		= 0.8f;
	// Distance from left and right screen edges where spaceman and helmet can be spawned
	[SerializeField] private	float			m_spacemanAndHelmetSpawnRange 	= 4.0f;
	// Distance from screen edge that objects can be spawned
	[SerializeField] private	float			m_spawnOffsetFromScreenEdge		= 2.0f;
	
	[Header("Scene References")]
	[SerializeField] private	Spaceman		m_spaceman		= null;
	[SerializeField] private	Helmet			m_helmet		= null;
	[SerializeField] private	SpaceRock[]		m_spaceRocks	= null;

	[Header("End Animations")]
	[SerializeField] private	GameObject		m_endScreen		= null;
	[SerializeField] private	Animator		m_endAnimWin	= null;
	[SerializeField] private	Animator		m_endAnimLose	= null;

	[Header("Blood Explosion Lose Animation")]
	[SerializeField] private	SpaceBloodDrop	m_bloodDrop			= null;
	[SerializeField] private	Transform		m_splatterPos		= null;
	[SerializeField] private	int				m_bloodDropCount 	= 10;
	[SerializeField] private 	Vector2 		m_splatterRangeY 	= new Vector2(-0.5f, 1.0f);
	[SerializeField] private 	float 			m_bloodDropSpawnDistFromCenter 	= 2.0f;
	
	[SerializeField] private	SpaceEndAnimEvents[] m_endAnimEventObjects = null;

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

	// Whether landscape auto-rotation was enabled before the Space MiniGame begins 
	private bool m_wasAutoRotateBeforeGame = false;

	/// <summary>
	/// Starts the game.
	/// </summary>
	protected override void StartGame()
	{
		InitializeSpacemanAndHelmet();
		InitializeSpaceRocks();

		// Store auto-rotation state
		m_wasAutoRotateBeforeGame = Screen.autorotateToLandscapeLeft || Screen.autorotateToLandscapeRight;
		// Disable auto-rotation for the duration of this game
		Screen.autorotateToLandscapeLeft = false;
		Screen.autorotateToLandscapeRight = false;
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
		// Disable colliders of all objects
		m_spaceman.GetComponent<Collider2D>().enabled = false;
		m_helmet.GetComponent<Collider2D>().enabled = false;
		foreach (SpaceRock spaceRock in m_spaceRocks)
		{
			spaceRock.GetComponent<Collider2D>().enabled = false;
		}

		// Restore auto-rotation state
		Screen.autorotateToLandscapeLeft = m_wasAutoRotateBeforeGame;
		Screen.autorotateToLandscapeRight = m_wasAutoRotateBeforeGame;
	}

	#endregion // Gameplay

	#region Scene Object Initialization

	/// <summary>
	/// Initializes the spaceman and helmet.
	/// </summary>
	private void InitializeSpacemanAndHelmet()
	{
		// Position spaceman and helmet on opposite ends of the screen (left or right)
		Vector2 min = Locator.GetSceneMaster().UICamera.ScreenMinWorld;
		Vector2 max = Locator.GetSceneMaster().UICamera.ScreenMaxWorld;
		Vector3 leftPos = new Vector3(Random.Range(min.x + m_spawnOffsetFromScreenEdge, min.x + m_spacemanAndHelmetSpawnRange),
		                              Random.Range(min.y + m_spawnOffsetFromScreenEdge, max.y - m_spawnOffsetFromScreenEdge));
		Vector3 rightPos = new Vector3(Random.Range(max.x - m_spawnOffsetFromScreenEdge, max.x - m_spacemanAndHelmetSpawnRange),
		                               Random.Range(min.y + m_spawnOffsetFromScreenEdge, max.y - m_spawnOffsetFromScreenEdge));
		// Randomly choose which side each of the two will be placed
		bool isSpacemanOnLeft = Random.value > 0.5f;
		m_spaceman.transform.position = isSpacemanOnLeft ? leftPos : rightPos;
		m_helmet.transform.position = isSpacemanOnLeft ? rightPos : leftPos;
		// Make spaceman face the side where the helmet is
		m_spaceman.transform.SetScaleX(isSpacemanOnLeft ? 1.0f : -1.0f);
		// Randomize spaceman's starting rotation
		m_spaceman.AnimRoot.SetRotZ(Random.Range(0.0f, 360.0f));
		AddToAnimatorList(m_spaceman.AnimRoot.GetComponent<Animator>());
		// Initialize
		m_spaceman.Initialize(this);
		m_helmet.Initialize();
	}

	/// <summary>
	/// Initializes space rocks.
	/// </summary>
	private void InitializeSpaceRocks()
	{
		// Rocks would typically be spawned in the center of the screen (though it depends on inspector values).
		// Shift them all up or down to make sure there is an unblocked path from the spaceman to the helmet.
		bool shiftRocksUp = Random.value > 0.5f ? true : false;

		for (int spaceRockCount = 0; spaceRockCount < m_spaceRocks.Length; ++spaceRockCount)
		{
			SpaceRock spaceRock = m_spaceRocks[spaceRockCount];
			if (spaceRockCount < m_spaceRocksPerLevel[m_level])
			{
				// Randomize space rock position
				Vector2 spaceRockPos = new Vector2(Random.Range(m_spaceRockSpawnAreaMin.x, m_spaceRockSpawnAreaMax.x),
				                 		           Random.Range(m_spaceRockSpawnAreaMin.y, m_spaceRockSpawnAreaMax.y));

				// Make sure there is enough space between space rocks
				bool isSpaced = true;
				float minDistSq = m_minDistBetweenSpaceRocks * m_minDistBetweenSpaceRocks;
				for (int x = 0; x < spaceRockCount; ++x)
				{
					if ((m_spaceRocks[x].transform.position - (Vector3)spaceRockPos).sqrMagnitude < minDistSq)
					{
						isSpaced = false;
						break;
					}
				}
				// If not spaced enough from other space rocks, re-randomize a new position
				if (!isSpaced)
				{
					spaceRockCount--;
					continue;
				}

				// Shift position up or down
				spaceRockPos.y += m_spaceRockShiftFromSpawnArea * (shiftRocksUp ? 1.0f : -1.0f);

				// Enable and initialize space rock
				spaceRock.transform.position = spaceRockPos;
				spaceRock.gameObject.SetActive(true);
				spaceRock.Initialize();
			}
			else
			{
				// Make sure the rest of the space rocks are disabled
				spaceRock.gameObject.SetActive(false);
			}
		}
	}

	#endregion // Scene Object Initialization

	#region Blood Splatter Lose Animation

	private List<SpaceBloodDrop> m_bloodDrops 	= new List<SpaceBloodDrop>();

	/// <summary>
	/// Creates a blood explosion.
	/// </summary>
	private void CreateBloodExplosion()
	{
		while (m_bloodDrops.Count < m_bloodDropCount)
		{
			// Randomize move direction
			Vector3 moveDir = new Vector3(0.0f, Random.Range(m_splatterRangeY.x, m_splatterRangeY.y), 0.0f);
			// Spawn half of the blood drops on the left side of the character
			if (m_bloodDrops.Count < m_bloodDropCount * 0.5f)
			{
				moveDir.x = -1.0f;
			}
			// Spawn the other blood drops on the right side
			else
			{
				moveDir.x = 1.0f;
			}
			moveDir = moveDir.normalized;
			// Spawn and initialize blood drop
			SpaceBloodDrop bloodDrop = SpawnBloodDrop();
			bloodDrop.Initialize(moveDir);
			// Position blood drop a certain distance from the center along the move direction
			bloodDrop.transform.position = m_splatterPos.position + moveDir * m_bloodDropSpawnDistFromCenter;
		}
	}

	/// <summary>
	/// Spawns a blood drop.
	/// </summary>
	/// <returns>The blood drop.</returns>
	private SpaceBloodDrop SpawnBloodDrop()
	{
		GameObject bloodDropObj = GameObject.Instantiate(m_bloodDrop.gameObject) as GameObject;
		bloodDropObj.SetActive(true);

		SpaceBloodDrop bloodDrop = bloodDropObj.AddComponentNoDupe<SpaceBloodDrop>();
		m_bloodDrops.Add(bloodDrop);
		return bloodDrop;
	}

	#endregion // Blood Splatter Lose Animation

	#region Ending Animation

	/// <summary>
	/// Starts the win animation.
	/// </summary>
	protected override void StartWinAnimation()
	{
		m_endScreen.SetActive(true);
		m_endAnimWin.gameObject.SetActive(true);
		m_endAnimLose.gameObject.SetActive(false);
		AddToAnimatorList(m_endAnimWin);
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
		m_endAnimWin.gameObject.SetActive(false);
		m_endAnimLose.gameObject.SetActive(true);
		AddToAnimatorList(m_endAnimLose);
	}

	/// <summary>
	/// Updates the lose animation.
	/// </summary>
	protected override void UpdateLoseAnimation()
	{
		
	}

	#endregion // Ending Animation
}
