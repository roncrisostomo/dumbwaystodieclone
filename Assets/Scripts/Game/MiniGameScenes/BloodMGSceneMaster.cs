/******************************************************************************
*  @file       BloodMGSceneMaster.cs
*  @brief      Handles the Blood MiniGame scene
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

#endregion // Namespaces

public class BloodMGSceneMaster : MiniGameSceneMasterBase
{
	#region Public Interface
	
	/// <summary>
	/// Notifies of scene pause.
	/// </summary>
	public override void NotifyPause()
	{
		base.NotifyPause();

		// Pause bullet holes
		foreach (BulletHole bulletHole in m_bulletHoles)
		{
			bulletHole.Pause();
		}
		// Pause end anim sounds
		if (m_whistleSound != null)
		{
			m_whistleSound.Pause();
		}
	}
	
	/// <summary>
	/// Notifies of scene unpause.
	/// </summary>
	public override void NotifyUnpause()
	{
		base.NotifyUnpause();

		// Unpause bullet holes
		foreach (BulletHole bulletHole in m_bulletHoles)
		{
			bulletHole.Unpause();
		}
		// Unpause end anim sounds
		if (m_whistleSound != null)
		{
			m_whistleSound.Unpause();
		}
	}

	#endregion // Public Interface

	#region Serialized Variables

	[Header("Bullet hole spawn parameters")]
	// Number of holes that would open for each level
	[SerializeField] private int[] 			m_holesPerLevel 		= new int[]{ 2, 3, 3, 4, 4, 5 };
	// Time from game start until the first hole opens
	[SerializeField] private float 			m_timeUntilFirstHole	= 0.5f;
	[SerializeField] private float 			m_minDistBetweenHoles	= 2.0f;
	[SerializeField] private Vector2 		m_holeSpawnAreaMin		= new Vector2(-5.3f, -3.0f);
	[SerializeField] private Vector2 		m_holeSpawnAreaMax		= new Vector2(5.3f, 2.1f);

	[Header("Instruction text area")]
	[SerializeField] private Vector2		m_textAreaMin			= new Vector2(-4.0f, -2.0f);
	[SerializeField] private Vector2		m_textAreaMax			= new Vector2(4.0f, 2.0f);

	[Header("Blood drop parameters")]
	[SerializeField] private Transform		m_bloodSource			= null;
	[SerializeField] private float			m_initialBloodDropSpeed	= 22.0f;
	[SerializeField] private float			m_bloodDropGravity		= 35.0f;

	[Header("End animations")]
	[SerializeField] private	GameObject		m_endScreen		= null;
	[SerializeField] private	Animator		m_endAnimWin	= null;
	[SerializeField] private	Animator		m_endAnimLose	= null;
	[SerializeField] private	Animator		m_endAnimCommon	= null;
	[SerializeField] private	Animator		m_charAnimWin	= null;
	[SerializeField] private	Animator		m_charAnimLose	= null;

	#endregion // Serialized Variables

	#region Resource Loading

	private GameObject 		m_bloodDropResource			= null;
	private GameObject 		m_bulletHoleResource		= null;

	private const string 	BLOOD_PREFAB_PATH 			= "Prefabs/MiniGameObjects/BloodDrop";
	private const string 	BULLET_HOLE_PREFAB_PATH 	= "Prefabs/MiniGameObjects/BulletHole";

	/// <summary>
	/// Loads the resources.
	/// </summary>
	protected override bool LoadResources()
	{
		// Load bullet hole prefab
		m_bulletHoleResource = Resources.Load(BULLET_HOLE_PREFAB_PATH, typeof(GameObject)) as GameObject;
		if (m_bulletHoleResource == null)
		{
			Debug.LogError("Bullet hole prefab not found at " + BULLET_HOLE_PREFAB_PATH);
			return false;
		}

		// Load blood drop prefab
		m_bloodDropResource = Resources.Load(BLOOD_PREFAB_PATH, typeof(GameObject)) as GameObject;
		if (m_bloodDropResource == null)
		{
			Debug.LogError("Blood prefab not found at " + BLOOD_PREFAB_PATH);
			return false;
		}

		return true;
	}

	/// <summary>
	/// Unloads the resources.
	/// </summary>
	protected override bool UnloadResources()
	{
		// Delete bullet holes
		for (int index = m_bulletHoles.Count - 1; index >= 0; --index)
		{
			m_bulletHoles[index].Delete();
		}
		m_bulletHoles.Clear();

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
		InitializePlaythrough();
	}

	/// <summary>
	/// Updates the game.
	/// </summary>
	protected override void UpdateGame()
	{
		UpdateHoleOpening();

		if (AreAllHolesPlugged())
		{
			StopGame(true);
		}
	}

	/// <summary>
	/// Raises the stop game event.
	/// </summary>
	protected override void OnStopGame()
	{
		// Keep holes in their plugged or unplugged state on game end
		foreach (BulletHole bulletHole in m_bulletHoles)
		{
			bulletHole.IgnoreInput();
		}
	}

	#endregion // Gameplay

	#region Bullet Holes

	private List<BulletHole> m_bulletHoles		= new List<BulletHole>();

	private float		m_holeOpenInterval		= 0.0f;
	private float		m_timeSinceLastHoleOpen	= 0.0f;
	private	int			m_nextHoleToOpen		= 0;

	/// <summary>
	/// Initializes this Blood MiniGame playthrough.
	/// </summary>
	private void InitializePlaythrough()
	{
		int holeCount = m_holesPerLevel[m_level];
		SpawnBulletHoles(holeCount);

		float levelDuration = m_timePerLevel[m_level];
		// Determine intervals at which holes open from the level time
		m_holeOpenInterval = levelDuration / (holeCount + 1);

		// The first hole should open earlier as the game speeds up,
		//	proportional to the game duration decrease per level
		float actualTimeUntilFirstHole = m_timeUntilFirstHole * levelDuration / m_timePerLevel[0];

		// Set timer such that the first hole would open at [m_timeUntilFirstHole]
		m_timeSinceLastHoleOpen = Mathf.Max(m_holeOpenInterval - actualTimeUntilFirstHole, 0.0f);
	}

	/// <summary>
	/// Spawns bullet holes.
	/// </summary>
	/// <param name="holeCount">Number of bullet holes to spawn.</param>
	private void SpawnBulletHoles(int holeCount)
	{
		while (m_bulletHoles.Count < holeCount)
		{
			// Randomize bullet hole position
			Vector2 holePos = new Vector2(Random.Range(m_holeSpawnAreaMin.x, m_holeSpawnAreaMax.x),
			                              Random.Range(m_holeSpawnAreaMin.y, m_holeSpawnAreaMax.y));

			// Spawn the first few bullet holes outside the instruction text area
			if (m_bulletHoles.Count < (float)holeCount * 0.5f)
			{
				// If position is within the instruction text area, re-randomize a new position
				if (holePos.x > m_textAreaMin.x && holePos.x < m_textAreaMax.x &&
				    holePos.y > m_textAreaMin.y && holePos.y < m_textAreaMax.y)
				{
					continue;
				}
			}

			// Make sure there is enough space between holes
			bool isSpaced = true;
			float minDistSq = m_minDistBetweenHoles * m_minDistBetweenHoles;
			foreach (BulletHole hole in m_bulletHoles)
			{
				if ((hole.transform.position - (Vector3)holePos).sqrMagnitude < minDistSq)
				{
					isSpaced = false;
					break;
				}
			}
			// If not spaced enough from other holes, re-randomize a new position
			if (!isSpaced)
			{
				continue;
			}

			// Spawn a bullet hole
			BulletHole bulletHole = SpawnBulletHole(holePos);
			Vector3 bloodDir = Vector3.Normalize((Vector3)holePos - m_bloodSource.position);
			bulletHole.Initialize(this, m_bloodDropResource, m_initialBloodDropSpeed * bloodDir,
			                      m_bloodDropGravity, UICamera.ScreenMinWorld.y - 5.0f);
			// Start bullet hole hidden
			bulletHole.Hide();
		}
	}

	/// <summary>
	/// Spawns a bullet hole.
	/// </summary>
	/// <returns>The bullet hole.</returns>
	private BulletHole SpawnBulletHole(Vector3 position)
	{
		GameObject bulletHoleObj = GameObject.Instantiate(m_bulletHoleResource) as GameObject;
		bulletHoleObj.transform.parent = Gameplay;
		bulletHoleObj.transform.position = position;
		
		BulletHole bulletHole = bulletHoleObj.AddComponentNoDupe<BulletHole>();
		m_bulletHoles.Add(bulletHole);
		return bulletHole;
	}

	/// <summary>
	/// Updates the opening of bullet holes.
	/// </summary>
	private void UpdateHoleOpening()
	{
		m_timeSinceLastHoleOpen += Time.deltaTime;
		if (m_timeSinceLastHoleOpen > m_holeOpenInterval)
		{
			m_timeSinceLastHoleOpen = 0.0f;
			OpenNextHole();
		}
	}

	/// <summary>
	/// Opens the next bullet hole in the "hole open" order.
	/// </summary>
	private void OpenNextHole()
	{
		if (m_nextHoleToOpen >= m_bulletHoles.Count)
		{
			return;
		}

		// Play "shoot" sound
		Locator.GetSoundSystem().PlayOneShot(SoundInfo.SFXID.BLOOD_SHOOT);

		// Open bullet hole
		m_bulletHoles[m_nextHoleToOpen].StartLeaking();

		// Increment "next hole" counter
		m_nextHoleToOpen++;
	}

	/// <summary>
	/// Gets whether all bullet holes are plugged.
	/// </summary>
	private bool AreAllHolesPlugged()
	{
		foreach (BulletHole bulletHole in m_bulletHoles)
		{
			if (!bulletHole.IsPlugged)
			{
				return false;
			}
		}
		return true;
	}

	#endregion // Bullet Holes

	#region Ending Animation

	private SoundObject m_whistleSound = null;

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
		AddToAnimatorList(m_charAnimWin);
		m_whistleSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.BLOOD_WHISTLE);
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
		m_endAnimCommon.gameObject.SetActive(true);
		AddToAnimatorList(m_endAnimLose);
		AddToAnimatorList(m_endAnimCommon);
		AddToAnimatorList(m_charAnimLose);
	}

	/// <summary>
	/// Updates the lose animation.
	/// </summary>
	protected override void UpdateLoseAnimation()
	{
		
	}

	#endregion // Ending Animation
}
