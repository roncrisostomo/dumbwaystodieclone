/******************************************************************************
*  @file       MiniGameSceneMasterBase.cs
*  @brief      Base class for MiniGame SceneMasters
*  @author     Lori
*  @date       August 5, 2015
*      
*  @par [explanation]
*		> Handles behavior shared between all mini game scenes
*			- Game level and timer
*			- Win/Lose sounds
*			- Instruction text
*			- Pausing/Unpausing
*			- Interaction with GameManager
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript;

#endregion // Namespaces

public class MiniGameSceneMasterBase : SceneMasterBase
{
	#region Public Interface

	public override sealed bool Load()
	{
		if (LoadResources())
		{
			m_isInitialized = true;
			return true;
		}
		return false;
	}

	public override sealed bool Unload()
	{
		if (UnloadResources())
		{
			m_isInitialized = false;
			return true;
		}
		return false;
	}

	public override sealed void StartScene()
	{
		InitializeGameLevel();
		InitializeGame();
	}

	public override void NotifyPause()
	{
		m_stateBeforePause = m_state;
		m_state = State.PAUSED;

		// Pause UI, InteractiveObjects, Animators, Sounds
		Main.Instance.GetTimerUI.PauseTimer();
		PauseInteractiveObjects();
		PauseAnimators();
		PauseSoundObjects();
		PauseInstructionText();

		if (Locator.GetGameManager() != null)
		{
			Locator.GetGameManager().NotifyMiniGamePaused();
		}
	}

	public override void NotifyUnpause()
	{
		m_state = m_stateBeforePause;

		// Unpause UI, InteractiveObjects, Animators, Sounds
		Main.Instance.GetTimerUI.UnpauseTimer();
		UnpauseInteractiveObjects();
		UnpauseAnimators();
		UnpauseSoundObjects();
		UnpauseInstructionText();

		if (Locator.GetGameManager() != null)
		{
			Locator.GetGameManager().NotifyMiniGameUnpaused();
		}
	}

	public override sealed void NotifyQuit()
	{
		DeleteSoundObjects();
		DeleteInteractiveObjects();
		OnStopGame();
		if (Locator.GetGameManager() != null)
		{
			Locator.GetGameManager().NotifyMiniGameQuit();
		}
	}

	/// <summary>
	/// Gets a value indicating whether this instance is game won.
	/// </summary>
	/// <value><c>true</c> if this instance is game won; otherwise, <c>false</c>.</value>
	public bool IsGameWon
	{
		get { return m_isGameWon; }
	}

	/// <summary>
	/// Gets a value indicating whether this instance is paused.
	/// </summary>
	/// <value><c>true</c> if this instance is paused; otherwise, <c>false</c>.</value>
	public bool IsPaused
	{
		get { return m_state == State.PAUSED; }
	}

	#endregion // Public Interface

	#region Serialized Variables

	// Levels
	[Header("Levels and Game Time")]
	[SerializeField] protected	uint				m_level						= 0;
	[SerializeField] protected	float[]				m_timePerLevel				=
	{
		7f, 6f, 5f, 4f, 3f, 2f
	};

	// Instruction text
	[Header("Instruction Text")]
	[SerializeField] private	Transform			m_instructionText0			= null;
	[SerializeField] private	Transform			m_startTextTransform0		= null;
	[SerializeField] private	Transform			m_endTextTransform0			= null;
	[SerializeField] private	Transform			m_instructionText1			= null;
	[SerializeField] private	Transform			m_startTextTransform1		= null;
	[SerializeField] private	Transform			m_endTextTransform1			= null;
	[SerializeField] private	InstructionHideType	m_textHideType				= InstructionHideType.FADE_OUT;
	/*[SerializeField]*/private	float				m_textStartTime				= 0.15f;
	/*[SerializeField]*/private	float				m_textShowTime				= 2f;
	/*[SerializeField]*/private	float				m_textEndTime				= 0.5f;

	// Ending animation
	[Header("Ending Animation")]
	[SerializeField] protected	float				m_endingAnimationDelay		= 0f;
	[SerializeField] protected	float				m_endingAnimationDuration	= 1f;
	
	#endregion // Serialized Variables

	#region MonoBehaviour

	/// <summary>
	/// Awake this instance.
	/// </summary>
	protected override sealed void Awake ()
	{
		base.Awake();
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	protected override sealed void Start ()
	{
		base.Start();
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	protected override sealed void Update ()
	{
		base.Update();

		switch (m_state)
		{
		case State.PLAYING:
			UpdateGameTimer();
			UpdateInstructionText();
			UpdateGame();
			break;
			
		case State.ENDING:
			UpdateEndingAnimation();
			break;
		}
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	protected override sealed void OnDestroy ()
	{
		base.OnDestroy();
	}

	#endregion // MonoBehaviour

	#region State

	private enum State
	{
		NONE,
		PLAYING,
		ENDING,
		PAUSED
	}

	private		State		m_state				= State.NONE;
	private		State		m_stateBeforePause	= State.NONE;

	#endregion // State

	#region Resource Loading

	/// <summary>
	/// Loads the resources.
	/// </summary>
	protected virtual bool LoadResources()
	{
		return true;
	}

	/// <summary>
	/// Unloads the resources.
	/// </summary>
	protected virtual bool UnloadResources()
	{
		return true;
	}

	#endregion // Resource Loading

	#region Level and Game Timer

	protected		float		m_timer				= 0f;
	protected		float		m_duration			= 0f;

	/// <summary>
	/// Initializes the game level.
	/// </summary>
	private void InitializeGameLevel()
	{
		// Get the current level from GameManager
		if (Locator.GetGameManager() != null)
		{
			m_level = Locator.GetGameManager().CurrentLevel;
		}
		
		// Set the game's duration according to the current level
		if (m_level < m_timePerLevel.Length)
		{
			m_duration = m_timePerLevel[m_level];
		}
		else
		{
			m_duration = m_timePerLevel[m_timePerLevel.Length - 1];
		}
		Main.Instance.GetTimerUI.StartTimer(m_duration);
		
		// Initialize the timer
		m_timer = 0f;
	}

	/// <summary>
	/// Updates the game timer.
	/// </summary>
	private void UpdateGameTimer()
	{
		m_timer += Time.deltaTime;
		if (m_timer >= m_duration)
		{
			OnTimeRunOut();
		}
	}

	/// <summary>
	/// Raises the time run out event.
	/// </summary>
	protected virtual void OnTimeRunOut()
	{
		// Sample
		StopGame(false);
	}

	#endregion // Level and Game Timer

	#region Gameplay

	private		bool		m_isGameWon			= false;

	/// <summary>
	/// Initializes the gameplay.
	/// </summary>
	private void InitializeGame()
	{
		InitializeInstructionText();
		StartGame();
		m_state = State.PLAYING;
	}
	
	/// <summary>
	/// Starts the game.
	/// </summary>
	protected virtual void StartGame()
	{

	}

	/// <summary>
	/// Updates the game.
	/// </summary>
	protected virtual void UpdateGame()
	{
		// Sample
		StopGame(true);
	}

	/// <summary>
	/// Stops the gameplay.
	/// </summary>
	protected void StopGame(bool isGameWon)
	{
		if (m_state == State.PLAYING)
		{
			m_isGameWon = isGameWon;
			DeleteInteractiveObjects();
			OnStopGame();
			Main.Instance.GetTimerUI.Hide();
			DisableInstructionText();
			InitializeEndingAnimation();
		}
	}

	/// <summary>
	/// Raises the stop game event.
	/// </summary>
	protected virtual void OnStopGame()
	{

	}

	#endregion // Gameplay

	#region Ending Animation

	protected		float		m_endingAnimationTimer			= 0f;
	private			bool		m_hasEndingAnimationStarted		= false;

	/// <summary>
	/// Initializes the ending animation.
	/// </summary>
	private void InitializeEndingAnimation()
	{
		m_state = State.ENDING;
	}

	/// <summary>
	/// Updates the ending animation.
	/// </summary>
	private void UpdateEndingAnimation()
	{
		m_endingAnimationTimer += Time.deltaTime;
		if (m_hasEndingAnimationStarted)
		{
			if (m_isGameWon)
			{
				UpdateWinAnimation();
			}
			else
			{
				UpdateLoseAnimation();
			}

			if (m_endingAnimationTimer >= m_endingAnimationDuration)
			{
				StopEndingAnimation();
			}
		}
		else
		{
			if (m_endingAnimationTimer >= m_endingAnimationDelay)
			{
				StartEndingAnimation();
				m_endingAnimationTimer = 0f;
			}
		}
	}
	
	/// <summary>
	/// Starts the ending animation.
	/// </summary>
	private void StartEndingAnimation()
	{
		if (m_isGameWon)
		{
			StartWinAnimation();
			Locator.GetSoundManager().PlayMiniGameWinSound();
		}
		else
		{
			StartLoseAnimation();
			Locator.GetSoundManager().PlayMiniGameLoseSound();
		}
		m_hasEndingAnimationStarted = true;
	}

	/// <summary>
	/// Starts the ending animation.
	/// </summary>
	private void StopEndingAnimation()
	{
		if (Locator.GetGameManager() != null)
		{
			Locator.GetGameManager().NotifyMiniGameEnded(m_isGameWon);
		}
		m_state = State.NONE;
	}
	
	/// <summary>
	/// Starts the win animation.
	/// </summary>
	protected virtual void StartWinAnimation()
	{
		
	}

	/// <summary>
	/// Updates the win animation.
	/// </summary>
	protected virtual void UpdateWinAnimation()
	{
		
	}
	
	/// <summary>
	/// Starts the lose animation.
	/// </summary>
	protected virtual void StartLoseAnimation()
	{
		
	}

	/// <summary>
	/// Updates the lose animation.
	/// </summary>
	protected virtual void UpdateLoseAnimation()
	{
		
	}

	#endregion // Ending Animation

	#region Object Collections
	
	#region Interactive Objects

	private		List<InteractiveObject>	m_interactiveObjList	= new List<InteractiveObject>();

	/// <summary>
	/// Pauses the interactive objects.
	/// </summary>
	private void PauseInteractiveObjects()
	{
		foreach (InteractiveObject interObj in m_interactiveObjList)
		{
			if (interObj != null)
			{
				interObj.NotifyPause();
			}
		}
	}

	/// <summary>
	/// Unpauses the interactive objects.
	/// </summary>
	private void UnpauseInteractiveObjects()
	{
		foreach (InteractiveObject interObj in m_interactiveObjList)
		{
			if (interObj != null)
			{
				interObj.NotifyUnpause();
			}
		}
	}

	/// <summary>
	/// Deletes the interactive objects.
	/// </summary>
	private void DeleteInteractiveObjects()
	{
		foreach (InteractiveObject interObj in m_interactiveObjList)
		{
			if (interObj != null)
			{
				interObj.Delete();
			}
		}
	}

	/// <summary>
	/// Adds to interactive object list.
	/// </summary>
	/// <param name="interObj">Inter object.</param>
	protected void AddToInteractiveObjectList(InteractiveObject interObj)
	{
		m_interactiveObjList.Add(interObj);
	}

	/// <summary>
	/// Removes from interactive object list.
	/// </summary>
	/// <param name="interObj">Inter object.</param>
	protected void RemoveFromInteractiveObjectList(InteractiveObject interObj)
	{
		m_interactiveObjList.Remove(interObj);
	}

	#endregion // Interactive Objects

	#region Animators

	private		List<Animator>			m_animatorList			= new List<Animator>();

	/// <summary>
	/// Pauses the animators.
	/// </summary>
	private void PauseAnimators()
	{
		foreach (Animator animator in m_animatorList)
		{
			if (animator != null)
			{
				animator.speed = 0f;
			}
		}
	}

	/// <summary>
	/// Unpauses the animators.
	/// </summary>
	private void UnpauseAnimators()
	{
		foreach (Animator animator in m_animatorList)
		{
			if (animator != null)
			{
				animator.speed = 1f;
			}
		}
	}

	/// <summary>
	/// Adds to animator list.
	/// </summary>
	/// <param name="animator">Animator.</param>
	protected void AddToAnimatorList(Animator animator)
	{
		m_animatorList.Add(animator);
	}

	/// <summary>
	/// Removes from animator list.
	/// </summary>
	/// <param name="animator">Animator.</param>
	protected void RemoveFromAnimatorList(Animator animator)
	{
		m_animatorList.Remove(animator);
	}

	#endregion // Animators

	#region Sounds Objects

	private		List<SoundObject>		m_soundObjList			= new List<SoundObject>();

	/// <summary>
	/// Pauses the sound objects.
	/// </summary>
	private void PauseSoundObjects()
	{
		foreach (SoundObject soundObj in m_soundObjList)
		{
			if (soundObj != null)
			{
				soundObj.Pause();
			}
		}
	}
	
	/// <summary>
	/// Unpauses the sound objects.
	/// </summary>
	private void UnpauseSoundObjects()
	{
		foreach (SoundObject soundObj in m_soundObjList)
		{
			if (soundObj != null)
			{
				soundObj.Unpause();
			}
		}
	}

	/// <summary>
	/// DEletes the sound objects.
	/// </summary>
	private void DeleteSoundObjects()
	{
		foreach (SoundObject soundObj in m_soundObjList)
		{
			if (soundObj != null)
			{
				soundObj.Delete();
			}
		}
		m_soundObjList.Clear();
	}
	
	/// <summary>
	/// Adds to sound object list.
	/// </summary>
	/// <param name="soundObj">Sound object.</param>
	protected void AddToSoundObjectList(SoundObject soundObj)
	{
		m_soundObjList.Add(soundObj);
	}
	
	/// <summary>
	/// Removes from sound object list.
	/// </summary>
	/// <param name="soundObj">Sound object.</param>
	protected void RemoveFromSoundObjectList(SoundObject soundObj)
	{
		m_soundObjList.Remove(soundObj);
	}

	#endregion // Sound Objects

	#endregion Object Collections

	#region Instruction Text

	private enum InstructionHideType
	{
		NONE,
		FADE_OUT,
		BACK_TO_START
	}

	private	enum InstructionState
	{
		NONE,
		START,
		SHOW,
		END
	}

	private			UIAnimator			m_startAnimator0				= null;
	private			UIAnimator			m_startAnimator1				= null;
	private			UIAnimator			m_endAnimator0					= null;
	private			UIAnimator			m_endAnimator1					= null;
	private			float				m_instructionTimer				= 0f;
	private			InstructionState	m_instructionState				= InstructionState.NONE;

	/// <summary>
	/// Initializes the instruction text.
	/// </summary>
	private void InitializeInstructionText()
	{
		InitializeInstructionText(ref m_instructionText0, ref m_startAnimator0, ref m_endAnimator0,
		                          m_startTextTransform0, m_endTextTransform0);
		InitializeInstructionText(ref m_instructionText1, ref m_startAnimator1, ref m_endAnimator1,
		                          m_startTextTransform1, m_endTextTransform1);
	}

	/// <summary>
	/// Initializes the instruction text.
	/// </summary>
	/// <param name="instructionText">Instruction text.</param>
	/// <param name="startAnimator">Start animator.</param>
	/// <param name="endAnimator">End animator.</param>
	/// <param name="startPos">Start position.</param>
	/// <param name="endPos">End position.</param>
	private void InitializeInstructionText(ref Transform instructionText, ref UIAnimator startAnimator,
	                                       ref UIAnimator endAnimator, Transform startTransform, Transform endTransform)
	{
		if (instructionText != null)
		{
			// Get the start and end positions
			Vector3 startPos = Vector3.zero;
			if (startTransform != null)
			{
				startPos = startTransform.position;
			}
			Vector3 endPos = Vector3.zero;
			if (endTransform != null)
			{
				endPos = endTransform.position;
			}

			// Create the start animation
			startAnimator = new UIAnimator(instructionText, UIAnimator.UIAnimatorType.TRANSFORM, true, true);
			startAnimator.SetPositionAnimation(startPos, endPos);
			startAnimator.SetAnimTime(m_textStartTime);

			// Create the end animation depending on type
			switch (m_textHideType)
			{
			case InstructionHideType.NONE:
				break;
				
			case InstructionHideType.BACK_TO_START:
				endAnimator = new UIAnimator(instructionText, UIAnimator.UIAnimatorType.COLOR, true, true);
				endAnimator.SetPositionAnimation(endPos, startPos);
				endAnimator.SetAnimTime(m_textStartTime);
				break;
				
			case InstructionHideType.FADE_OUT:
				endAnimator = new UIAnimator(instructionText, UIAnimator.UIAnimatorType.COLOR, true, true);
				endAnimator.SetAlphaAnimation(1f, 0f);
				endAnimator.SetAnimTime(m_textEndTime);
				break;
			}
		}
	}

	/// <summary>
	/// Updates the instruction text.
	/// </summary>
	private void UpdateInstructionText()
	{
		switch (m_instructionState)
		{
		case InstructionState.NONE:
			m_instructionState = InstructionState.START;
			EnableInstructionStartAnimations();
			m_instructionTimer = 0f;
			break;
		case InstructionState.START:
			m_instructionTimer += Time.deltaTime;
			UpdateInstructionStartAnimations();
			if (m_instructionTimer >= m_textStartTime)
			{
				m_instructionState = InstructionState.SHOW;
				m_instructionTimer = 0f;
			}
			break;
		case InstructionState.SHOW:
			m_instructionTimer += Time.deltaTime;
			if (m_instructionTimer >= m_textShowTime)
			{
				m_instructionState = InstructionState.END;
				m_instructionTimer = 0f;
				EnableInstructionEndAnimations();
			}
			break;
		case InstructionState.END:
			UpdateInstructionEndAnimations();
			break;
		}
	}

	/// <summary>
	/// Enables the instruction start animations.
	/// </summary>
	private void EnableInstructionStartAnimations()
	{
		if (m_startAnimator0 != null)
		{
			m_startAnimator0.AnimateToState2();
		}
		if (m_startAnimator1 != null)
		{
			m_startAnimator1.AnimateToState2();
		}
	}

	/// <summary>
	/// Updates the instruction start animations.
	/// </summary>
	private void UpdateInstructionStartAnimations()
	{
		if (m_startAnimator0 != null)
		{
			m_startAnimator0.Update(Time.deltaTime);
		}
		if (m_startAnimator1 != null)
		{
			m_startAnimator1.Update(Time.deltaTime);
		}
	}

	/// <summary>
	/// Enables the instruction end animations.
	/// </summary>
	private void EnableInstructionEndAnimations()
	{
		if (m_endAnimator0 != null)
		{
			m_endAnimator0.AnimateToState2();
		}
		if (m_endAnimator1 != null)
		{
			m_endAnimator1.AnimateToState2();
		}
	}

	/// <summary>
	/// Updates the instruction end animations.
	/// </summary>
	private void UpdateInstructionEndAnimations()
	{
		if (m_endAnimator0 != null)
		{
			m_endAnimator0.Update(Time.deltaTime);
		}
		if (m_endAnimator1 != null)
		{
			m_endAnimator1.Update(Time.deltaTime);
		}
	}
	
	/// <summary>
	/// Disable the instruction text.
	/// </summary>
	private void DisableInstructionText()
	{
		if (m_startAnimator0 != null)
		{
			m_startAnimator0.RemoveAllAnimations();
		}
		if (m_startAnimator1 != null)
		{
			m_startAnimator1.RemoveAllAnimations();
		}
		if (m_endAnimator0 != null)
		{
			m_endAnimator0.RemoveAllAnimations();
		}
		if (m_endAnimator1 != null)
		{
			m_endAnimator1.RemoveAllAnimations();
		}
		if (m_instructionText0 != null)
		{
			m_instructionText0.gameObject.SetActive(false);
		}
		if (m_instructionText1 != null)
		{
			m_instructionText1.gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Pauses the instruction text.
	/// </summary>
	private void PauseInstructionText()
	{
		if (m_startAnimator0 != null)
		{
			m_startAnimator0.Pause();
		}
		if (m_startAnimator1 != null)
		{
			m_startAnimator1.Pause();
		}
		if (m_endAnimator0 != null)
		{
			m_endAnimator0.Pause();
		}
		if (m_endAnimator1 != null)
		{
			m_endAnimator1.Pause();
		}
	}

	/// <summary>
	/// Unpauses the instruction text.
	/// </summary>
	private void UnpauseInstructionText()
	{
		if (m_startAnimator0 != null)
		{
			m_startAnimator0.Unpause();
		}
		if (m_startAnimator1 != null)
		{
			m_startAnimator1.Unpause();
		}
		if (m_endAnimator0 != null)
		{
			m_endAnimator0.Unpause();
		}
		if (m_endAnimator1 != null)
		{
			m_endAnimator1.Unpause();
		}
	}

	#endregion // Instruction Text

}
