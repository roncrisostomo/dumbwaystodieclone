/******************************************************************************
*  @file       PiranhaMGSceneMaster.cs
*  @brief      Handles the Piranha MiniGame scene
*  @author     Lori
*  @date       July 29, 2015
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

public class PiranhaMGSceneMaster : MiniGameSceneMasterBase
{
	#region Public Interface

	#endregion // Public Interface

	#region Serialized Variables

	[Header("Piranha")]
	[SerializeField] private	uint[]			m_piranhaCountPerLevel			= null;
	[SerializeField] private	Piranha			m_piranha						= null;
	[Header("Animation")]
	[SerializeField] private	Animator		m_characterAnimator				= null;
	[SerializeField] private	float			m_characterDrownSpeed			= 5f;
	[SerializeField] private	float			m_characterDrownRotateSpeed		= 720f;

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
		StopGame(true);
	}

	#endregion // Level and Game Timer

	#region Gameplay

	private		Piranha[]	m_piranhas				= null;
	private		uint		m_piranhaCount			= 0;
	private		uint		m_activePiranhaIndex	= 0;
	private		float		m_piranhaSpawnTimer		= 0f;
	private		float		m_piranhaSpawnDuration	= 0f;
	private		uint		m_piranhaFlickCounter	= 0;

	/// <summary>
	/// Starts the game.
	/// </summary>
	protected override void StartGame()
	{
		// Get the piranha count for the current level
		if (m_level < m_piranhaCountPerLevel.Length)
		{
			m_piranhaCount = m_piranhaCountPerLevel[m_level];
		}
		else
		{
			m_piranhaCount = m_piranhaCountPerLevel[m_piranhaCountPerLevel.Length - 1];
		}
		
		// Initialize the piranha array and spawn the first piranha
		m_piranhas = new Piranha[m_piranhaCount];
		m_activePiranhaIndex = 0;
		// Account for the last piranha's bite preparation
		float waitingWindow = m_piranha.BitePreparationDuration;
		if (m_duration < waitingWindow)
		{
			waitingWindow = 0f;
		}
		// Properly space the spawning across the game's duration
		m_piranhaSpawnDuration = (m_duration - waitingWindow) / m_piranhaCount;
		// Deactivate the template piranha
		m_piranha.gameObject.SetActive(false);

		// Spawn the first piranha
		SpawnPiranha();

		// Add the character to the animators list
		AddToAnimatorList(m_characterAnimator);
	}

	/// <summary>
	/// Updates the game.
	/// </summary>
	protected override void UpdateGame()
	{
		// Update piranha spawning
		m_piranhaSpawnTimer += Time.deltaTime;
		if (m_piranhaSpawnTimer >= m_piranhaSpawnDuration)
		{
			SpawnPiranha();
		}
	}

	/// <summary>
	/// Raises the stop game event.
	/// </summary>
	protected override void OnStopGame()
	{
	
	}

	/// <summary>
	/// Spawns a piranha.
	/// </summary>
	private bool SpawnPiranha()
	{
		if (m_activePiranhaIndex >= m_piranhaCount)
		{
			return false;
		}
		
		// Copy existing piranha
		m_piranhas[m_activePiranhaIndex] = Instantiate(m_piranha);
		
		// Initialize the instance
		m_piranhas[m_activePiranhaIndex].gameObject.SetActive(true);
		m_piranhas[m_activePiranhaIndex].transform.parent = m_piranha.transform.parent;
		m_piranhas[m_activePiranhaIndex].Initialize(OnPiranhaFlicked, OnPiranhaBite);

		// Add to list
		AddToInteractiveObjectList(m_piranhas[m_activePiranhaIndex]);

		++m_activePiranhaIndex;
		m_piranhaSpawnTimer = 0f;
		return true;
	}

	/// <summary>
	/// Called when a piranha was flicked
	/// </summary>
	private void OnPiranhaFlicked()
	{
		m_piranhaFlickCounter++;
		if (m_piranhaFlickCounter == m_piranhaCount)
		{
			StopGame(true);
		}
	}

	/// <summary>
	/// Called when a piranha bites
	/// </summary>
	private void OnPiranhaBite()
	{
		StopGame(false);
	}
	
	#endregion // Gameplay

	#region Ending Animation

	/// <summary>
	/// Starts the win animation.
	/// </summary>
	protected override void StartWinAnimation()
	{
	
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
		m_characterAnimator.Play("CharacterPiranhaMGSad");
	}

	/// <summary>
	/// Updates the lose animation.
	/// </summary>
	protected override void UpdateLoseAnimation()
	{
		m_characterAnimator.transform.Translate(Vector3.left * m_characterDrownSpeed * Time.deltaTime, Space.World);
		m_characterAnimator.transform.Rotate(Vector3.forward * m_characterDrownRotateSpeed * Time.deltaTime);
	}

	#endregion // Ending Animation
}
