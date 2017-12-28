/******************************************************************************
*  @file       DoorMGSceneMaster.cs
*  @brief      Handles the Door MiniGame scene
*  @author     Lori
*  @date       July 28, 2015
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

public class DoorMGSceneMaster : MiniGameSceneMasterBase
{
	#region Public Interface

	#endregion // Public Interface

	#region Serialized Variables

	[Header("Character Movement")]
	[SerializeField] private	float[]				m_characterMovementDelayPerLevel	= null;
	[SerializeField] private	float[]				m_characterAppearanceTimePerLevel	= null;
	[SerializeField] private	float[]				m_characterMovementSpeedPerLevel	= null;
	[SerializeField] private	float				m_characterYPosOffset				= -0.2f;
	[SerializeField] private	float				m_characterMoveDistance				= 1.0f;
	[Header("Instance References")]
	[SerializeField] private	CharacterDoorMG		m_panda								= null;
	[SerializeField] private	CharacterDoorMG[]	m_psychos							= null;
	[SerializeField] private	Door[]				m_doors								= null;
	[Header("Ending Animation")]
	[SerializeField] private	GameObject			m_endScreen							= null;
	[SerializeField] private	GameObject			m_winVisual							= null;
	[SerializeField] private	GameObject			m_loseVisual						= null;
	[SerializeField] private	Animator			m_endScreenAnimator					= null;

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
		InitializeCharacterMovementVariables();
		InitializeDoorsAndCharacters();
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

	}

	#region Doors and Characters

	/// <summary>
	/// Initializes the doors and characters.
	/// </summary>
	private void InitializeDoorsAndCharacters()
	{
		// Pre-compute the character position offset vector
		Vector3 posOffset = Vector3.up * m_characterYPosOffset;

		// Assign a character (panda/psycho) to each door
		uint doorCount = (uint)m_doors.Length;
		uint pandaDoorIndex = (uint)Random.Range(0, doorCount);
		uint psychoCounter = 0;
		for (uint i = 0; i < doorCount; ++i)
		{
			Vector3 characterPos = m_doors[i].GetPeekAreaPosition() + posOffset;
			if (i == pandaDoorIndex)
			{
				m_doors[i].Initialize(OnPressPandaDoor);
				if (m_panda != null)
				{
					// Panda asset has to be a little lower..
					m_panda.Initialize(characterPos + (Vector3.down * 0.03f), m_characterMoveSpeed,
					                   m_characterMoveDistance, GetRandomMoveDelay(), m_characterAppearanceTime);
					AddToInteractiveObjectList(m_panda);
				}
			}
			else
			{
				m_doors[i].Initialize(OnPressPsychoDoor);
				if (m_psychos != null && psychoCounter < m_psychos.Length)
				{
					m_psychos[psychoCounter].Initialize(characterPos, m_characterMoveSpeed,
					                                    m_characterMoveDistance, GetRandomMoveDelay(),
					                                    m_characterAppearanceTime);
					AddToInteractiveObjectList(m_psychos[psychoCounter]);
					++psychoCounter;
				}
			}
			AddToInteractiveObjectList(m_doors[i]);
		}
	}
	
	/// <summary>
	/// Called when a Panda door has been pressed
	/// </summary>
	private void OnPressPandaDoor ()
	{
		StopGame(true);
	}
	
	/// <summary>
	/// Called when a Psycho door has been pressed
	/// </summary>
	private void OnPressPsychoDoor ()
	{
		StopGame(false);
	}

	#endregion // Doors and Characters
	
	#region Character Movement
	
	private		float				m_characterMoveSpeed		= 0f;
	private		float				m_characterMoveDelay		= 0f;
	private		float				m_characterMoveDuration		= 0f;
	private		float				m_characterAppearanceTime	= 0f;

	/// <summary>
	/// Initializes the character movement variables.
	/// </summary>
	private void InitializeCharacterMovementVariables()
	{
		if (m_level < m_characterMovementSpeedPerLevel.Length)
		{
			m_characterMoveSpeed = m_characterMovementSpeedPerLevel[m_level];
		}
		else
		{
			m_characterMoveSpeed = m_characterMovementSpeedPerLevel[m_characterMovementSpeedPerLevel.Length - 1];
		}
		if (m_level < m_characterMovementDelayPerLevel.Length)
		{
			m_characterMoveDelay = m_characterMovementDelayPerLevel[m_level];
		}
		else
		{
			m_characterMoveDelay = m_characterMovementDelayPerLevel[m_characterMovementDelayPerLevel.Length - 1];
		}
		if (m_level < m_characterAppearanceTimePerLevel.Length)
		{
			m_characterAppearanceTime = m_characterAppearanceTimePerLevel[m_level];
		}
		else
		{
			m_characterAppearanceTime = m_characterAppearanceTimePerLevel[m_characterAppearanceTimePerLevel.Length - 1];
		}
		m_characterMoveDuration = m_characterMoveDistance / m_characterMoveSpeed;
	}

	/// <summary>
	/// Gets a random move delay value.
	/// </summary>
	/// <returns>The random move delay.</returns>
	private float GetRandomMoveDelay()
	{
		int rand = Random.Range(0, 3);
		switch (rand)
		{
		default:
		case 0:
			return m_characterMoveDelay;

		case 1:
			return m_characterMoveDelay + m_characterMoveDuration;

		case 2:
			return m_characterMoveDelay + (m_characterMoveDuration * 2f);
		}
	}
	
	#endregion // Character Movement

	#endregion // Gameplay

	#region Ending Animation

	/// <summary>
	/// Starts the win animation.
	/// </summary>
	protected override void StartWinAnimation()
	{
		InitializeDoorAnimation();
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
		InitializeDoorAnimation();
	}

	/// <summary>
	/// Updates the lose animation.
	/// </summary>
	protected override void UpdateLoseAnimation()
	{
		
	}

	private void InitializeDoorAnimation()
	{
		m_endScreen.SetActive(true);
		m_winVisual.SetActive(IsGameWon);
		m_loseVisual.SetActive(!IsGameWon);
		AddToAnimatorList(m_endScreenAnimator);
		AddToSoundObjectList(Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.DOOR_OPEN));
	}

	#endregion // Ending Animation
}
