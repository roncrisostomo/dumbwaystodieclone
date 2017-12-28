/******************************************************************************
*  @file       GameManager.cs
*  @brief      Oversees the mini games and scoring
*  @author     Lori
*  @date       August 20, 2015
*      
*  @par [explanation]
*		> Holds and computes the score
*		> Loads mini game sequences
*		> Calls ScoreUI and ResultsSceneMaster
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#endregion // Namespaces

public class GameManager : MonoBehaviour
{
	#region Public Interface

	/// <summary>
	/// Initialize this instance.
	/// </summary>
	public void InitializeGameSettings()
	{
		Locator.GetSoundManager().SetActiveSounds(m_gameData.SoundsOn);
	}
	
	/// <summary>
	/// Gets a value indicating whether this instance has pledged.
	/// </summary>
	/// <value><c>true</c> if this instance has pledged; otherwise, <c>false</c>.</value>
	public bool HasPledged
	{
		get { return m_gameData.PledgeDone; }
	}

	/// <summary>
	/// Notifies GameManager that the pledge has been taken
	/// </summary>
	/// <returns><c>true</c>, if pledge taken was notified, <c>false</c> otherwise.</returns>
	public void NotifyPledgeTaken()
	{
		m_gameData.PledgeDone = true;
		SaveGameData();
	}

	/// <summary>
	/// Sets a value indicating whether this instance has sounds.
	/// </summary>
	/// <value><c>true</c> if this instance has sounds; otherwise, <c>false</c>.</value>
	public bool HasSounds
	{
		get { return m_gameData.SoundsOn; }
	}

	/// <summary>
	/// Notifies the toggle sounds.
	/// </summary>
	/// <returns><c>true</c>, if toggle sounds was notifyed, <c>false</c> otherwise.</returns>
	public void NotifyToggleSounds()
	{
		SetActiveSounds(!m_gameData.SoundsOn);
	}

	/// <summary>
	/// Sets the active sounds.
	/// </summary>
	/// <param name="areSoundsOn">If set to <c>true</c> are sounds on.</param>
	public void SetActiveSounds(bool areSoundsOn)
	{
		m_gameData.SoundsOn = areSoundsOn;
		SaveGameData();
		Locator.GetSoundManager().SetActiveSounds(m_gameData.SoundsOn);
	}

	/// <summary>
	/// Gets a value indicating whether this instance is plane game included.
	/// </summary>
	/// <value><c>true</c> if this instance is plane game included; otherwise, <c>false</c>.</value>
	public bool IsPlaneGameIncluded
	{
		get { return m_gameData.IncludePlaneGame; }
	}

	/// <summary>
	/// Notifies the toggle include plane game.
	/// </summary>
	public void NotifyToggleIncludePlaneGame()
	{
		SetIncludePlaneGame(!m_gameData.IncludePlaneGame);
	}

	/// <summary>
	/// Sets the include plane game.
	/// </summary>
	/// <param name="isPlaneGameIncluded">If set to <c>true</c> is plane game included.</param>
	public void SetIncludePlaneGame(bool isPlaneGameIncluded)
	{
		m_gameData.IncludePlaneGame = isPlaneGameIncluded;
		SaveGameData();
	}

	/// <summary>
	/// Notifies the reset progress.
	/// </summary>
	public void NotifyResetProgress ()
	{
		Debug.Log("Resetting progress...");

		m_gameData.HighScore = 0;
		m_gameData.ItemUnlockIndex = 0;
		m_gameData.PledgeDone = false;
		SaveGameData();
	}

	/// <summary>
	/// Starts a new sequence of mini games.
	/// </summary>
	public void NotifyPlayButtonPressed()
	{
		if (Main.Instance.IsSceneInitialized)
		{
			StartMiniGameSequence();
		}
	}

	
	/// <summary>
	/// Notifies the mini game ended.
	/// </summary>
	public void NotifyMiniGameEnded(bool isStagePassed)
	{
		StartScoreState(isStagePassed);
	}

	/// <summary>
	/// Notifies the mini game quit.
	/// </summary>
	public void NotifyMiniGameQuit()
	{
		if (m_state != State.METAGAME_LOADING)
		{
			m_state = State.METAGAME_LOADING;
			Main.Instance.NotifySwitchScene(SceneInfo.SceneEnum.MAIN_MENU);
		}
	}

	/// <summary>
	/// Notifies the mini game paused.
	/// </summary>
	public void NotifyMiniGamePaused()
	{
		Locator.GetSoundManager().NotifyPause();
		if (m_state == State.SCORE)
		{
			Main.Instance.GetScoreUI.Pause();
		}
		if (m_state == State.FASTER)
		{
			Main.Instance.GetFasterUI.Pause();
		}
		m_isPaused = true;
	}

	/// <summary>
	/// Notifies the mini game unpaused.
	/// </summary>
	public void NotifyMiniGameUnpaused()
	{
		Locator.GetSoundManager().NotifyUnpause();
		if (m_state == State.SCORE)
		{
			Main.Instance.GetScoreUI.Unpause();
		}
		if (m_state == State.FASTER)
		{
			Main.Instance.GetFasterUI.Unpause();
		}
		m_isPaused = false;
	}

	/// <summary>
	/// Gets the current level.
	/// </summary>
	/// <value>The current level.</value>
	public uint CurrentLevel
	{
		get { return m_currentLevel; }
	}

	/// <summary>
	/// Gets the maximum level.
	/// </summary>
	/// <value>The maximum level.</value>
	public uint MaxLevel
	{
		get { return m_maxLevel; }
	}

	#endregion // Public Interface

	#region Serialized Variables

	// TODO: Store default values somewhere else

	[SerializeField] private				uint			m_maxLives					= 3;
	[SerializeField] private				uint			m_currentLives				= 3;
	[SerializeField] private				int				m_currentScore				= 0;
	[SerializeField] private				uint			m_currentLevel				= 0;
	[SerializeField] private				uint			m_maxLevel					= 5;
	[SerializeField] private				uint			m_miniGameCountPerLevel		= 3;
	[SerializeField] private				int				m_miniGameCountPerSet		= 0; // Include all mini games
	[SerializeField] private				int				m_miniGameNoRepeatCount		= 6;
	[SerializeField] private				int[]			m_scoresPerLevel			=
	{
		100
	};
	[SerializeField] private				string[]		m_passBonusScoreNames		= 
	{
		"YAY",
		"DUUUUDE",
		"POINTS FOR STYLE",
		"BECAUSE YOU DESERVE IT",
		"NICE WEATHER TODAY",
		":-)",
		"BECAUSE YOU'RE SPECIAL",
		"TINY BONUS",
		"BOO",
		"BAD LUCK"
	};
	[SerializeField] private				int[]			m_passBonusScores			= 
	{
		41,
		30,
		22,
		19,
		14,
		10,
		4,
		3,
		-35,
		-60,
	};
	[SerializeField] private				string[]		m_failBonusScoreNames		=
	{
		"SILVER LINING",
		"PITY BONUS",
		"POINTS FOR TRYING",
		"SO CLOSE",
		"MIND YOUR LANGUAGE",
		"MUST TRY HARDER",
		"BOO",
		"OUCH!",
		"INSULT TO INJURY",
		"BAD LUCK"
	};
	[SerializeField] private				int[]			m_failBonusScores			=
	{
		99,
		36,
		25,
		17,
		-15,
		-22,
		-35,
		-39,
		-44,
		-60
	};
	[SerializeField] private				float			m_bonusScoreProbability		= 0.5f;
	[SerializeField] private				int				m_maxFasterLevel			= 5;
	[SerializeField] private				float[]			m_fasterDurationPerLevel	=
	{
		0.8f, 0.8f, 0.8f, 1.0f, 0.8f
	};

	#endregion // Serialized Variables

	#region MonoBehaviour

	/// <summary>
	/// Awake this instance.
	/// </summary>
	private void Awake()
	{
		if (Locator.GetGameManager() == null)
		{
			// Pass reference to Locator
			Locator.ProvideGameManager(this);
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			// Self-destruct if an instance is already present
			Destroy(this.gameObject);
		}

		InitializeGameData();
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
		UpdateState();

		CheckEscapeInput();
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	private void OnDestroy()
	{

	}

	/// <summary>
	/// Raises the level loaded event.
	/// </summary>
	/// <param name="loadedLevelIndex">Index of the loaded level in the Build Settings scene list.</param>
	private void OnLevelWasLoaded(int loadedLevelIndex)
	{
		m_isPaused = false;

		switch (m_state)
		{
		case State.MINIGAME_LOADING:
			StartMiniGameInitState();
			break;

		case State.RESULTS_LOADING:
			StartResultsInitState();
			break;

		case State.METAGAME_LOADING:
		case State.NONE:
			StartMetaGameState();
			break;
		}
	}

	#endregion // MonoBehaviour

	#region States

	private enum State
	{
		NONE,
		METAGAME_LOADING,	// Loading a meta game scene
		METAGAME,			// Title, MainMenu, etc.
		MINIGAME_LOADING,	// Loading a mini game scene
		MINIGAME_INIT,		// Waiting for mini game scene to initialize
		MINIGAME,			// Running a mini game scene
		SCORE,				// Showing the score scene
		FASTER,				// Showing the faster scene
		RESULTS_LOADING,	// Loading the results scene
		RESULTS_INIT,		// Waiting for the results scene to initialize
		RESULTS				// Showing the results scene
	}

	private		State			m_state				= State.NONE;
	private		bool			m_isPaused			= false;

	/// <summary>
	/// Updates the state.
	/// </summary>
	private void UpdateState()
	{
		if (m_isPaused)
		{
			return;
		}

		switch (m_state)
		{
		case State.MINIGAME_INIT:
			StartMiniGameState();
			break;
		
		case State.SCORE:
			UpdateScoreState();
			break;
		
		case State.FASTER:
			UpdateFasterState();
			break;

		case State.RESULTS_INIT:
			StartResultsState();
			break;
		
		default:
			break;
		}
	}
	
	#endregion // States

	#region CheckEscapeInput

	/// <summary>
	/// Checks for escape input.
	/// </summary>
	private void CheckEscapeInput()
	{
		// Check if device back button is pressed
		if (Input.GetKeyDown(KeyCode.Escape) && Main.Instance.IsSceneInitialized)
		{
			switch (Main.Instance.GetCurSceneEnum)
			{
			// If in Title scene, quit the game
			case SceneInfo.SceneEnum.TITLE:
				Application.Quit();
				break;
			// If in Main Menu scene, load Title scene
			case SceneInfo.SceneEnum.MAIN_MENU:
				Main.Instance.NotifySwitchScene(SceneInfo.SceneEnum.TITLE);
				break;
			// If in Settings scene, load Main Menu scene
			case SceneInfo.SceneEnum.SETTINGS:
				// Let SettingsSceneMaster handle this
				break;
			// If in Results scene, load Main Menu scene
			case SceneInfo.SceneEnum.RESULTS:
				// Let ResultsSceneMaster handle this
				break;
			// If in a MiniGame scene, pause the game and bring up the pause UI
			case SceneInfo.SceneEnum.MINIGAME_PLANE:
			case SceneInfo.SceneEnum.MINIGAME_BUTTON:
			case SceneInfo.SceneEnum.MINIGAME_DOOR:
			case SceneInfo.SceneEnum.MINIGAME_FIRE:
			case SceneInfo.SceneEnum.MINIGAME_PIRANHA:
			case SceneInfo.SceneEnum.MINIGAME_WASP:
			case SceneInfo.SceneEnum.MINIGAME_YELLOWLINE:
			case SceneInfo.SceneEnum.MINIGAME_BLOOD:
			case SceneInfo.SceneEnum.MINIGAME_FORK:
			case SceneInfo.SceneEnum.MINIGAME_BALLOON:
			case SceneInfo.SceneEnum.MINIGAME_WIRE:
			case SceneInfo.SceneEnum.MINIGAME_SPACE:
			case SceneInfo.SceneEnum.MINIGAME_GLUE:
				Main.Instance.GetPauseUI.PauseGame();
				break;
			default:
				break;
			}
		}
	}

	#endregion // CheckEscapeInput

	#region Mini Games

	private			SceneInfo.SceneEnum			m_currentMiniGameScene		= SceneInfo.SceneEnum.SIZE;
	private			uint						m_miniGameCountThisLevel	= 0;
	private			List<SceneInfo.SceneEnum>	m_miniGameSet				= new List<SceneInfo.SceneEnum>();
	private			Queue<int>					m_recentlyPlayedSetIndex	= new Queue<int>();

	/// <summary>
	/// Starts a mini game sequence.
	/// </summary>
	private void StartMiniGameSequence()
	{
		if (m_state != State.METAGAME && m_state != State.RESULTS)
		{
			// Don't let this spam!
			return;
		}

		// Reset variables
		m_currentMiniGameScene = SceneInfo.SceneEnum.SIZE;
		m_currentLives = m_maxLives;
		m_currentScore = 0;
		m_currentLevel = 0;
		m_miniGameCountThisLevel = 0;

		// Prepare the UI
		Main.Instance.GetScoreUI.Reset();
		Main.Instance.GetFasterUI.Reset();

		// Play the MiniGame BGM
		Locator.GetSoundManager().PlayMiniGameBGM();
		Locator.GetSoundManager().AdjustMiniGameBGMPitch(m_currentLevel);

		// Create a set of minigames
		CreateMiniGameSet();

		// Get a new random minigame
		LoadNextMiniGame();
	}

	/// <summary>
	/// Creates the mini game set.
	/// </summary>
	private void CreateMiniGameSet()
	{
		int actualSetCount = m_miniGameCountPerSet;
		if (m_miniGameCountPerSet <= 0)
		{
			actualSetCount = (int)LastMiniGameScene - (int)FirstMiniGameScene + 1;
		}

		m_miniGameSet.Clear();
		m_miniGameSet.Capacity = actualSetCount;

		// Fill up the set with random mini games
		for (uint i = 0; i < actualSetCount; ++i)
		{
			SceneInfo.SceneEnum mgEnum = FirstMiniGameScene;
			do
			{
				mgEnum = GetRandomMiniGameScene();
			}
			while (m_miniGameSet.Contains(mgEnum));
			m_miniGameSet.Add(mgEnum);
		}
	}
	
	/// <summary>
	/// Goes to next mini game.
	/// </summary>
	private void LoadNextMiniGame()
	{
		// Load the next mini game
		m_currentMiniGameScene = m_miniGameSet[GetNextMiniGameSetIndex()];
		// For testing: Uncomment and replace to have GameManager load only the specified MiniGame
		//m_currentMiniGameScene = SceneInfo.SceneEnum.MINIGAME_PLANE;
		m_state = State.MINIGAME_LOADING;
		
		Main.Instance.NotifySwitchScene(m_currentMiniGameScene);
	}

	/// <summary>
	/// Gets the index of the next mini game set.
	/// </summary>
	/// <returns>The next mini game set index.</returns>
	private int GetNextMiniGameSetIndex()
	{
		// Keep recently played queue count to [noRepeatCount]
		if (m_recentlyPlayedSetIndex.Count == m_miniGameNoRepeatCount)
		{
			m_recentlyPlayedSetIndex.Dequeue();
		}
		int nextGameIndex = 0;
		// Randomly find a game that was not played in the previous [noRepeatCount] games
		do
		{
			nextGameIndex = Random.Range(0, m_miniGameSet.Capacity);
		}
		while (m_recentlyPlayedSetIndex.Contains(nextGameIndex));
		// Add game to the recently played queue
		m_recentlyPlayedSetIndex.Enqueue(nextGameIndex);
		return nextGameIndex;
	}
	
	/// <summary>
	/// Starts the init state of the mini game.
	/// </summary>
	private void StartMiniGameInitState()
	{
		m_state = State.MINIGAME_INIT;
	}

	/// <summary>
	/// Starts the state of the mini game.
	/// </summary>
	private void StartMiniGameState()
	{
		if (!Main.Instance.IsSceneInitialized)
		{
			return;
		}

		m_state = State.MINIGAME;

		Main.Instance.GetFasterUI.Reset();
		Main.Instance.GetScoreUI.Hide();
//		Main.Instance.GetTimerUI.Hide();
		Main.Instance.GetPauseUI.Reset();
	}
	
	/// <summary>
	/// Gets the first mini game scene.
	/// </summary>
	/// <value>The first mini game scene.</value>
	private SceneInfo.SceneEnum FirstMiniGameScene
	{
		get
		{
			// TODO: Update this
			return IsPlaneGameIncluded ?
				SceneInfo.SceneEnum.MINIGAME_PLANE :
				SceneInfo.SceneEnum.MINIGAME_BUTTON;
		}
	}

	/// <summary>
	/// Gets the last mini game scene.
	/// </summary>
	/// <value>The last mini game scene.</value>
	private SceneInfo.SceneEnum LastMiniGameScene
	{
		get
		{
			// TODO: Update this
			return SceneInfo.SceneEnum.MINIGAME_GLUE;
		}
	}

	/// <summary>
	/// Gets a random mini game scene.
	/// </summary>
	/// <returns>The random mini game scene.</returns>
	private SceneInfo.SceneEnum GetRandomMiniGameScene()
	{
		return (SceneInfo.SceneEnum)Random.Range((int)FirstMiniGameScene, (int)LastMiniGameScene + 1);
	}
	
	#endregion // Mini Games

	#region Score

	private				int				m_livesToAdd			= 0;
	private				int				m_scoreToAdd			= 0;
	private				int				m_bonusScoreToAdd		= 0;
	private				string			m_bonusScoreText		= "";

	/// <summary>
	/// Starts the state of the score.
	/// </summary>
	private void StartScoreState(bool isStagePassed)
	{
		m_state = State.SCORE;

		m_livesToAdd = 0;
		m_scoreToAdd = 0;
		m_bonusScoreToAdd = 0;
		m_bonusScoreText = "";
		bool isLoseLife = false;

		if (isStagePassed)
		{
			// Compute the score to add
			uint levelIndex = (uint)Mathf.Clamp(m_currentLevel, 0, m_scoresPerLevel.Length - 1);
			m_scoreToAdd = m_scoresPerLevel[levelIndex];

			// Chance to get bonus score
			if (Random.value <= m_bonusScoreProbability)
			{
				int i = Random.Range(0, m_passBonusScoreNames.Length);
				m_bonusScoreToAdd = m_passBonusScores[i];
				m_bonusScoreText = m_passBonusScoreNames[i];
			}
		}
		else
		{
			m_livesToAdd = -1;
			m_scoreToAdd = 0;
			isLoseLife = true;

			// Chance to get bonus score
			if (Random.value <= m_bonusScoreProbability)
			{
				int i = Random.Range(0, m_failBonusScoreNames.Length);
				m_bonusScoreToAdd = m_failBonusScores[i];
				m_bonusScoreText = m_failBonusScoreNames[i];
			}
		}

		Main.Instance.GetScoreUI.SetScores(m_currentScore, m_scoreToAdd,
		                                   m_bonusScoreToAdd, m_bonusScoreText,
		                                   isLoseLife, (int)m_currentLives + m_livesToAdd);
		Main.Instance.GetScoreUI.Show();
		Main.Instance.GetScoreUI.StartAnimation();

		m_currentScore += m_scoreToAdd + m_bonusScoreToAdd;
		m_currentLives = (uint)((int)m_currentLives + m_livesToAdd);
	}

	/// <summary>
	/// Updates the score state.
	/// </summary>
	private void UpdateScoreState()
	{
		// Wait for score animation to end
		if (Main.Instance.GetScoreUI.IsAnimationDone)
		{
			// Load the next mini game or the results scene if out of lives
			if (m_currentLives > 0)
			{
				StartFasterState();
			}
			else
			{
				LoadResultsScene();
			}
		}
	}

	#endregion // Score

	#region Faster

	private				float				m_fasterStateTimer			= 0f;
	private				float				m_fasterScreenDuration		= 0f;

	/// <summary>
	/// Starts the faster state.
	/// </summary>
	private void StartFasterState()
	{
		// Show the FasterUI if a level has been completed
		m_miniGameCountThisLevel += 1;
		if (m_miniGameCountThisLevel >= m_miniGameCountPerLevel)
		{
			// Show the Faster animation
			if (m_currentLevel < m_maxFasterLevel)
			{
				m_fasterStateTimer = 0f;
				int i = Mathf.Clamp((int)m_currentLevel, 0, m_fasterDurationPerLevel.Length - 1);
				m_fasterScreenDuration = m_fasterDurationPerLevel[i];
				Main.Instance.GetFasterUI.StartFasterAnimation((int)m_currentLevel);
				// Hide other UI
				Main.Instance.GetScoreUI.Hide();
			}
			// Update the level variable
			m_currentLevel = (uint)Mathf.Clamp((int)m_currentLevel + 1, 0, (int)m_maxLevel);
			m_miniGameCountThisLevel = 0;

			m_state = State.FASTER;
		}
		else
		{
			LoadNextMiniGame();
		}
	}

	/// <summary>
	/// Updates the faster state.
	/// </summary>
	private void UpdateFasterState()
	{
		m_fasterStateTimer += Time.deltaTime;
		if (m_fasterStateTimer >= m_fasterScreenDuration)
		{
			// Increase the BGMs pitch
			Locator.GetSoundManager().AdjustMiniGameBGMPitch(m_currentLevel);
			LoadNextMiniGame();
		}
	}

	#endregion // Faster

	#region Results

	/// <summary>
	/// Starts the state of the results init.
	/// </summary>
	private void StartResultsInitState()
	{
		m_state = State.RESULTS_INIT;
	}

	/// <summary>
	/// Starts the results state.
	/// </summary>
	private void StartResultsState()
	{
		if (!Main.Instance.IsSceneInitialized)
		{
			return;
		}

		m_state = State.RESULTS;

		Main.Instance.GetFasterUI.Hide();
		Main.Instance.GetScoreUI.Hide();
		Main.Instance.GetTimerUI.Hide();
		Main.Instance.GetPauseUI.Hide();

		ResultsSceneMaster resultsScene = (ResultsSceneMaster)Locator.GetSceneMaster();
		ResultsUI resultsUI = resultsScene.GetResultsUI;

		// Check if high score
		if (m_gameData.HighScore < m_currentScore)
		{
			m_gameData.HighScore = m_currentScore;
			resultsUI.SetWellDoneResults(m_currentScore, m_gameData.HighScore);
			SaveGameData();
		}
		else
		{
			resultsUI.SetTryHarderResults(m_currentScore, m_gameData.HighScore, 0);
		}

		// Play the MiniGame BGM
		Locator.GetSoundManager().PlayMainMenuBGM();
	}
	
	/// <summary>
	/// Loads the results scene.
	/// </summary>
	private void LoadResultsScene()
	{
		m_state = State.RESULTS_LOADING;

		Main.Instance.NotifySwitchScene(SceneInfo.SceneEnum.RESULTS);
	}

	#endregion // Results

	#region Metagame

	/// <summary>
	/// Starts the metagame state
	/// </summary>
	private void StartMetaGameState()
	{
		if (m_state != State.METAGAME)
		{
			m_state = State.METAGAME;

			Main.Instance.GetFasterUI.Hide();
			Main.Instance.GetScoreUI.Hide();
			Main.Instance.GetTimerUI.Hide();
			Main.Instance.GetPauseUI.Hide();

			// Play the MiniGame BGM
			Locator.GetSoundManager().PlayMainMenuBGM();
		}
	}

	#endregion // Metagame

	#region Game Data

	private		DataSystem		m_dataSystem		= null;
	private		GameData		m_gameData			= new GameData();

	/// <summary>
	/// Initializes the game data.
	/// </summary>
	private void InitializeGameData()
	{
		m_dataSystem = new DataSystem();
		m_dataSystem.Initialize();
		m_gameData = m_dataSystem.GetGameData();
	}

	/// <summary>
	/// Saves the game data.
	/// </summary>
	private void SaveGameData()
	{
		m_dataSystem.SetGameData(m_gameData);
		m_dataSystem.Save();
	}

	#endregion // Game Data
}
