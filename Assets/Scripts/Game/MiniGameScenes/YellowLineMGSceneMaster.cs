/******************************************************************************
*  @file       YellowLineMGSceneMaster.cs
*  @brief      Handles the YellowLine MiniGame scene
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

public class YellowLineMGSceneMaster : MiniGameSceneMasterBase
{
	#region Public Interface

	#endregion // Public Interface

	#region Serialized Variables

	[Header("Characters")]
	[SerializeField] private	uint[]						m_activeCharactersPerLevel	= null;
	[SerializeField] private	CharacterYellowLineMG[]		m_characters				= null;
	[Header("Train")]
	[SerializeField] private	GameObject					m_train						= null;
	[SerializeField] private	float						m_trainSpeed				= 50f;
	[SerializeField] private	float						m_trainStartXPos			= -25f;

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

	private		uint		m_activeCharacterCount	= 0;

	/// <summary>
	/// Starts the game.
	/// </summary>
	protected override void StartGame()
	{
		// Activate the correct number of characters for the current level
		if (m_level < m_activeCharactersPerLevel.Length)
		{
			m_activeCharacterCount = m_activeCharactersPerLevel[m_level];
		}
		else
		{
			m_activeCharacterCount = m_activeCharactersPerLevel[m_activeCharactersPerLevel.Length - 1];
		}
		
		// Initialize the characters
		uint characterCount = (uint)m_characters.Length;
		uint activeCharacterCounter = m_activeCharacterCount;
		uint inactiveCharacterCounter = characterCount - m_activeCharacterCount;
		for (uint i = 0; i < characterCount; ++i)
		{
			// TODO: So ugly
			if (inactiveCharacterCounter == 0)
			{
				m_characters[i].Initialize(OnCharacterFlicked);
				AddToInteractiveObjectList(m_characters[i]);
				activeCharacterCounter -= 1;
			}
			else if (activeCharacterCounter == 0)
			{
				m_characters[i].gameObject.SetActive(false);
				inactiveCharacterCounter -= 1;
			}
			else
			{
				if (Random.Range(0, 2) == 0)
				{
					m_characters[i].gameObject.SetActive(false);
					inactiveCharacterCounter -= 1;
				}
				else
				{
					m_characters[i].Initialize(OnCharacterFlicked);
					AddToInteractiveObjectList(m_characters[i]);
					activeCharacterCounter -= 1;
				}
			}
		}
		
		// Initialize the train
		m_train.SetActive(false);
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

	/// <summary>
	/// Called when a character has been flicked
	/// </summary>
	private void OnCharacterFlicked ()
	{
		Locator.GetSoundSystem().PlayOneShot(SoundInfo.SFXID.YELLOW_DRAG);
		m_activeCharacterCount -= 1;
		if (m_activeCharacterCount == 0)
		{
			StopGame(true);
		}
	}
	
	#endregion // Gameplay

	#region Ending Animation

	private		CharacterYellowLineMG[]	m_fallingCharacters		= null;
	private		SoundObject				m_trainSound			= null;

	/// <summary>
	/// Starts the win animation.
	/// </summary>
	protected override void StartWinAnimation()
	{
		InitializeTrainMovement();
	}

	/// <summary>
	/// Updates the win animation.
	/// </summary>
	protected override void UpdateWinAnimation()
	{
		UpdateTrainMovement();
	}
	
	/// <summary>
	/// Starts the lose animation.
	/// </summary>
	protected override void StartLoseAnimation()
	{
		InitializeTrainMovement();
		InitializeCharacterFallMovement();
	}

	/// <summary>
	/// Updates the lose animation.
	/// </summary>
	protected override void UpdateLoseAnimation()
	{
		UpdateTrainMovement();
		UpdateCharacterFallMovement();
	}
	
	/// <summary>
	/// Initializes the train movement.
	/// </summary>
	private void InitializeTrainMovement()
	{
		// Initialize train position
		Vector3 trainPos = m_train.transform.position;
		trainPos.x = m_trainStartXPos;
		m_train.transform.position = trainPos;
		m_train.SetActive(true);

		// Play the train sound
		m_trainSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.YELLOW_TRAIN);
		AddToSoundObjectList(m_trainSound);
	}

	/// <summary>
	/// Updates the train movement.
	/// </summary>
	private void UpdateTrainMovement()
	{
		m_train.transform.Translate(Vector3.right * m_trainSpeed * Time.deltaTime);

		if (m_endingAnimationTimer >= m_endingAnimationDuration)
		{
			RemoveFromSoundObjectList(m_trainSound);
			m_trainSound.Delete();
			m_trainSound = null;
		}
	}

	/// <summary>
	/// Initializes the character fall movement.
	/// </summary>
	private void InitializeCharacterFallMovement()
	{
		if (m_activeCharacterCount > 0)
		{
			m_fallingCharacters = new CharacterYellowLineMG[m_activeCharacterCount];
			uint fallCharCounter = 0;
			for (uint i = 0; i < m_characters.Length; ++i)
			{
				if (m_characters[i].gameObject.activeSelf && !m_characters[i].IsFlicked)
				{
					m_fallingCharacters[fallCharCounter++] = m_characters[i];
					m_characters[i].StartFallAnimation();
					if (fallCharCounter == m_activeCharacterCount)
					{
						break;
					}
				}
			}
		}
	}

	/// <summary>
	/// Updates the character fall movement.
	/// </summary>
	private void UpdateCharacterFallMovement()
	{
		if (m_fallingCharacters != null)
		{
			for (uint i = 0; i < m_fallingCharacters.Length; ++i)
			{
				m_fallingCharacters[i].UpdateFallAnimation();
			}
		}
	}

	#endregion // Ending Animation
}
