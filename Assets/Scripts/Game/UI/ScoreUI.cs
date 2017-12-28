/******************************************************************************
*  @file       ScoreUI.cs
*  @brief      Handles the score UI
*  @author     Ron
*  @date       August 19, 2015
*      
*  @par [explanation]
* 		> Displays the number of lives left
* 		> Displays the player's score for the last minigame played
* 		> Displays the player's cumulative score
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class ScoreUI : MonoBehaviour
{
	#region Public Interface

	/// <summary>
	/// Initialize this instance.
	/// </summary>
	public void Initialize()
	{
		// Start hidden
		Hide();

		// Set the initialized flag
		m_isInitialized = true;
	}

	/// <summary>
	/// Sets the values for the "well done" results UI.
	/// </summary>
	/// <param name="cumulativeScore">Cumulative score.</param>
	/// <param name="score">Score.</param>
	/// <param name="bonusScore">Bonus score.</param>
	/// <param name="bonusScoreLabel">Bonus score label.</param>
	public void SetScores(int cumulativeScore, int score, int bonusScore, string bonusScoreLabel, bool loseLife, int curLives)
	{
		// Set values for use in animation
		m_cumulativeScore = cumulativeScore;
		m_score = score;
		m_bonusScore = bonusScore;

		// Set values for UI text fields
		m_cumulativeScoreText.text = cumulativeScore.ToString();
		m_scoreText.text = score.ToString();
		if (bonusScore != 0 && !string.IsNullOrEmpty(bonusScoreLabel))
		{
			m_bonusScoreText.text = bonusScore.ToString();
			m_bonusScoreLabelText.text = bonusScoreLabel;
		}
		else
		{
			m_bonusScoreText.text = "";
			m_bonusScoreLabelText.text = "";
		}

		// Play lives animation
		m_activeLifeAnimator = null;
		if (loseLife)
		{
			if (curLives >=0 && curLives < m_lifeAnimators.Length)
			{
				m_activeLifeAnimator = m_lifeAnimators[curLives];
			}
		}

		// Reset anim time trackers
		m_timeSinceAnimStart = 0.0f;
		m_timeSinceWaitStart = 0.0f;
	}

	/// <summary>
	/// Starts the score UI animation.
	/// </summary>
	public void StartAnimation()
	{
		if (m_activeLifeAnimator != null)
		{
			m_activeLifeAnimator.SetTrigger("LoseLife");
		}

		m_aniState = ScoreAnimationState.ADD_SCORE;
	}

	/// <summary>
	/// Resets the life UI.
	/// </summary>
	public void ResetLifeUI()
	{
		foreach (Animator lifeAnimator in m_lifeAnimators)
		{
			lifeAnimator.Play("Empty");
		}
	}

	/// <summary>
	/// Shows the score UI.
	/// </summary>
	public void Show()
	{
		// TODO: Improve
		foreach (Renderer renderer in this.GetComponentsInChildren<Renderer>(true))
		{
			renderer.enabled = true;
		}
	}

	/// <summary>
	/// Hides the score UI.
	/// </summary>
	public void Hide()
	{
		// TODO: Improve
		foreach (Renderer renderer in this.GetComponentsInChildren<Renderer>(true))
		{
			renderer.enabled = false;
		}
	}

	/// <summary>
	/// Pauses the score UI animation.
	/// </summary>
	public void Pause()
	{
		m_isPaused = true;
		m_lifeAnimatorSpeedBeforePause = m_lifeAnimators[0].speed;
		m_charAnimatorSpeedBeforePause = m_characterAnimators[0].speed;

		// Pause the life animators
		foreach (Animator lifeAnimator in m_lifeAnimators)
		{
			lifeAnimator.speed = 0.0f;
		}
		// Pause the character animators
		foreach (Animator characterAnimator in m_characterAnimators)
		{
			characterAnimator.speed = 0.0f;
		}

		// Pause score count sound
		if (m_scoreCountSound != null)
		{
			m_scoreCountSound.Pause();
		}
	}

	/// <summary>
	/// Unpauses the score UI animation.
	/// </summary>
	public void Unpause()
	{
		if (!m_isPaused)
		{
			return;
		}
		m_isPaused = false;

		// Unpause the life animators
		foreach (Animator lifeAnimator in m_lifeAnimators)
		{
			lifeAnimator.speed = m_lifeAnimatorSpeedBeforePause;
		}
		// Unpause the character animators
		foreach (Animator characterAnimator in m_characterAnimators)
		{
			characterAnimator.speed = m_charAnimatorSpeedBeforePause;
		}

		// Unpause score count sound
		if (m_scoreCountSound != null)
		{
			m_scoreCountSound.Unpause();
		}
	}

	/// <summary>
	/// Resets the score UI.
	/// </summary>
	public void Reset()
	{
		m_aniState = ScoreAnimationState.IDLE;
		ResetLifeUI();
		Unpause();
		if (m_scoreCountSound != null)
		{
			m_scoreCountSound.Stop();
		}
	}

	/// <summary>
	/// Gets whether the score UI animation has finished.
	/// </summary>
	public bool IsAnimationDone
	{
		get { return m_aniState == ScoreAnimationState.DONE; }
	}

	/// <summary>
	/// Gets whether ScoreUI is initialized.
	/// </summary>
	public bool IsInitialized
	{
		get { return m_isInitialized; }
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private TextMesh	m_cumulativeScoreText	= null;
	[SerializeField] private TextMesh	m_scoreText				= null;
	[SerializeField] private TextMesh	m_bonusScoreText		= null;
	[SerializeField] private TextMesh	m_bonusScoreLabelText	= null;
	// Animators for the 3 life indicators, labeled one to three from left to right
	[SerializeField] private Animator[]	m_lifeAnimators			= null;
	[SerializeField] private Animator[] m_characterAnimators	= null;

	[SerializeField] private float		m_scoreAnimSpeed		= 250.0f;
	[SerializeField] private float		m_scoreAnimWaitTime		= 0.8f;

	#endregion // Serialized Variables

	#region Variables
	
	private bool m_isInitialized = false;
	private bool m_isPaused = false;

	// Pause variable buffers
	private float m_lifeAnimatorSpeedBeforePause = 0.0f;
	private float m_charAnimatorSpeedBeforePause = 0.0f;

	#endregion // Variables

	#region Score Animation

	private enum ScoreAnimationState
	{
		IDLE = 0,
		ADD_SCORE,
		WAIT_TIME_1,
		ADD_BONUS_SCORE,
		WAIT_TIME_2,
		DONE
	}
	private ScoreAnimationState m_aniState = ScoreAnimationState.IDLE;

	private	Animator m_activeLifeAnimator = null;

	private float	m_timeSinceAnimStart = 0.0f;
	private float 	m_timeSinceWaitStart = 0.0f;

	private float	m_cumulativeScore 	= 0;
	private float	m_score 			= 0;
	private float 	m_bonusScore 		= 0;

	private const	float	MIN_SCORE	= -999999.0f;
	private const	float	MAX_SCORE	= 999999.0f;

	/// <summary>
	/// Updates the score UI animation.
	/// </summary>
	private void UpdateScoreUIAnimation()
	{
		switch (m_aniState)
		{
		case ScoreAnimationState.IDLE:
			break;
		case ScoreAnimationState.ADD_SCORE:
			// Play score count sound
			PlayScoreCountSound();
			// Score is added to cumulative score during score animation
			m_timeSinceAnimStart += Time.deltaTime;
			int score = (int)Mathf.Lerp(m_score, 0.0f, m_timeSinceAnimStart * m_scoreAnimSpeed);
			int cumulativeScore = (int)Mathf.Lerp(m_cumulativeScore, m_cumulativeScore + m_score, m_timeSinceAnimStart * m_scoreAnimSpeed);
			// Update score UI
			m_scoreText.text = score.ToString();
			m_cumulativeScoreText.text = cumulativeScore.ToString();
			// Once scores reach their final values, move on to the next animation state
			if (score == 0 && cumulativeScore == m_cumulativeScore + m_score)
			{
				m_score = score;
				m_cumulativeScore = cumulativeScore;

				// Stop score count sound
				m_scoreCountSound.Stop();

				m_timeSinceAnimStart = 0.0f;
				m_aniState = ScoreAnimationState.WAIT_TIME_1;
			}
			break;
		case ScoreAnimationState.WAIT_TIME_1:
			// Wait for a specified time
			m_timeSinceWaitStart += Time.deltaTime;
			if (m_timeSinceWaitStart > m_scoreAnimWaitTime)
			{
				m_timeSinceWaitStart = 0.0f;
				// If there is no bonus score, go straight to DONE state
				if (m_bonusScore == 0 || m_bonusScoreLabelText.text == "")
				{
					m_aniState = ScoreAnimationState.DONE;
				}
				// Else, animate bonus score
				else
				{
					m_aniState = ScoreAnimationState.ADD_BONUS_SCORE;
				}
			}
			break;
		case ScoreAnimationState.ADD_BONUS_SCORE:
			// Play score count sound
			PlayScoreCountSound();
			// Bonus score is added to (or subtracted from) cumulative score during score animation
			m_timeSinceAnimStart += Time.deltaTime;
			int bonusScore = (int)Mathf.Lerp(m_bonusScore, 0.0f, m_timeSinceAnimStart * m_scoreAnimSpeed);
			int cumulativeScore2 = (int)Mathf.Lerp(m_cumulativeScore, m_cumulativeScore + m_bonusScore, m_timeSinceAnimStart * m_scoreAnimSpeed);
			// Update bonus score UI
			m_bonusScoreText.text = bonusScore.ToString();
			m_cumulativeScoreText.text = cumulativeScore2.ToString();
			// Once scores reach their final values, move on to the next animation state
			if (bonusScore == 0 && cumulativeScore2 == m_cumulativeScore + m_bonusScore)
			{
				m_bonusScore = bonusScore;
				m_cumulativeScore = cumulativeScore2;

				// Stop score count sound
				m_scoreCountSound.Stop();

				m_timeSinceAnimStart = 0.0f;
				m_aniState = ScoreAnimationState.WAIT_TIME_2;
			}
			break;
		case ScoreAnimationState.WAIT_TIME_2:
			// Wait for a specified time
			m_timeSinceWaitStart += Time.deltaTime;
			if (m_timeSinceWaitStart > m_scoreAnimWaitTime)
			{
				m_timeSinceWaitStart = 0.0f;
				m_aniState = ScoreAnimationState.DONE;
			}
			break;
		case ScoreAnimationState.DONE:
			// Score UI animation finished
			break;
		}
	}

	#endregion // Score Animation

	#region Score Count Sound

	private SoundObject m_scoreCountSound = null;

	/// <summary>
	/// Plays the score count sound.
	/// </summary>
	private void PlayScoreCountSound()
	{
		// Lazy init
		if (m_scoreCountSound == null)
		{
			m_scoreCountSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.SCORE_COUNT);
		}
		else
		{
			if (!m_scoreCountSound.IsPlaying)
			{
				m_scoreCountSound.Play();
			}
		}
	}

	#endregion // Score Count Sound

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
		if (m_isPaused)
		{
			return;
		}

		UpdateScoreUIAnimation();
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	private void OnDestroy()
	{

	}

	#endregion // MonoBehaviour
}
