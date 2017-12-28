/******************************************************************************
*  @file       ResultsUI.cs
*  @brief      Handles the results UI
*  @author     Ron
*  @date       August 20, 2015
*      
*  @par [explanation]
* 		> Displays the player's total score for this round of minigames
* 		> Displays the player's best score
*		> Loads the Main Menu scene when Main Menu button is tapped
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class ResultsUI : MonoBehaviour
{
	#region Public Interface

	/// <summary>
	/// Initialize this instance.
	/// </summary>
	public void Initialize()
	{
		m_playAgainButton.Initialize(PlayAgainHandler, UIButton.TriggerType.ON_RELEASE);
		m_mainMenuButton.Initialize(MainMenuHandler, UIButton.TriggerType.ON_RELEASE);
		m_pledgeButton.Initialize(PledgeHandler, UIButton.TriggerType.ON_RELEASE);
		m_promiseButton.Initialize(PromiseHandler, UIButton.TriggerType.ON_RELEASE);
		m_cancelButton.Initialize(CancelHandler, UIButton.TriggerType.ON_RELEASE);

		// TODO: Delegates should not be in Main
		// Button sound delegates
		m_playAgainButton.AddSoundDelegates(Main.Instance.UIButtonPressHandler, Main.Instance.UIButtonReleaseHandler);
		m_mainMenuButton.AddSoundDelegates(Main.Instance.UIButtonPressHandler, Main.Instance.UIButtonReleaseHandler);
		m_pledgeButton.AddSoundDelegates(Main.Instance.UIButtonPressHandler, Main.Instance.UIButtonReleaseHandler);
		m_promiseButton.AddSoundDelegates(Main.Instance.UIButtonPressHandler, Main.Instance.UIButtonReleaseHandler);
		m_cancelButton.AddSoundDelegates(Main.Instance.UIButtonPressHandler, Main.Instance.UIButtonReleaseHandler);
		
		// Safety Pledge UI Animator
		//  State 1 - hidden in pledge button position
		//  State 2 - normal display
		m_pledgeUIAnimator = new UIAnimator(m_pledgeRoot.transform);
		m_pledgeUIAnimator.SetPositionAnimation(m_pledgeButton.transform.position, m_pledgeRoot.transform.position);
		m_pledgeUIAnimator.SetScaleAnimation(Vector3.zero, m_pledgeRoot.transform.localScale);
		// Overlay UI Animator
		// 	State 1 - transparent (invisible)
		// 	State 2 - black overlay over the results UI
		m_pledgeOverlayAnimator = new UIAnimator(m_pledgeOverlay);
		m_pledgeOverlayAnimator.SetAlphaAnimation(0.0f, m_pledgeOverlay.color.a);
		// Pledge Thanks Animator
		//	State 1 - hidden in pledge button position
		//	State 2 - normal display
		m_pledgeThanksAnimator = new UIAnimator(m_pledgeThanks.transform);
		m_pledgeThanksAnimator.SetPositionAnimation(m_pledgeButton.transform.position, m_pledgeThanks.transform.position);
		m_pledgeThanksAnimator.SetScaleAnimation(Vector3.zero, m_pledgeThanks.transform.localScale);

		// Set animation speed
		m_pledgeUIAnimator.SetAnimSpeed(m_pledgePopupSpeed);
		m_pledgeOverlayAnimator.SetAnimSpeed(m_pledgePopupSpeed);

		// Start safety pledge UI disabled and "minimized" at the position of the pledge button
		m_pledgeRoot.SetActive(false);
		m_pledgeRoot.transform.position = m_pledgeButton.transform.position;
		m_pledgeRoot.transform.localScale = Vector3.zero;
		// Start pledge overlay at 0 alpha (transparent)
		m_pledgeOverlay.SetAlpha(0.0f);
		// Start safety pledge UI disabled and "minimized" at the position of the pledge button
		m_pledgeThanks.SetActive(false);
		m_pledgeThanks.transform.position = m_pledgeButton.transform.position;
		m_pledgeThanks.transform.localScale = Vector3.zero;

		// If player has already pledged, lock pledge button in "pressed" state
		if (Locator.GetGameManager() != null && Locator.GetGameManager().HasPledged)
		{
			LockPledgeButton();
		}

		// Set the initialized flag
		m_isInitialized = true;
	}

	/// <summary>
	/// Sets the values for the "well done" results UI.
	/// </summary>
	/// <param name="totalScore">Total score.</param>
	/// <param name="bestScore">Best score.</param>
	public void SetWellDoneResults(int totalScore, int bestScore, bool unlockNewCharacter = true)
	{
		// Set values
		m_totalScoreText.text = totalScore.ToString();
		// Set best score text
		if (totalScore == bestScore)
		{
			m_bestScoreText.text = "That's your best score!";
		}
		else
		{
			m_bestScoreText.text = "Your best score is " + bestScore.ToString() + ".";
		}
		// Set well done text
		if (unlockNewCharacter)
		{
			m_tipTextWellDone.text = "Well done!";// You've unlocked\na new character for your\ntrain station.";
		}
		else
		{
			m_tipTextWellDone.text = "Well done!";
		}

		m_playAgainButton.gameObject.SetActive(false);
		m_mainMenuButton.transform.position = m_mainMenuButtonWellDonePos.position;
		m_tipTextWellDone.gameObject.SetActive(true);
		m_tipTextTryHarder.gameObject.SetActive(false);
		m_dialogueBGWellDone.SetActive(true);
		m_dialogueBGTryHarder.SetActive(false);
	}

	/// <summary>
	/// Sets the values for the "try harder" results UI.
	/// </summary>
	/// <param name="totalScore">Total score.</param>
	/// <param name="bestScore">Best score.</param>
	/// <param name="scoreToUnlockCharacter">Score to unlock character.</param>
	public void SetTryHarderResults(int totalScore, int bestScore, int scoreToUnlockCharacter)
	{
		// Set values
		m_totalScoreText.text = totalScore.ToString();
		m_bestScoreText.text = "Your best score is " + bestScore.ToString();
		m_tipTextTryHarder.text = "All characters are unlocked in this version.";//"Score " + scoreToUnlockCharacter.ToString() + " or more to unlock the next character.";

		m_playAgainButton.gameObject.SetActive(true);
		m_mainMenuButton.transform.position = m_mainMenuButtonTryHarderPos.position;
		m_tipTextTryHarder.gameObject.SetActive(true);
		m_tipTextWellDone.gameObject.SetActive(false);
		m_dialogueBGTryHarder.SetActive(true);
		m_dialogueBGWellDone.SetActive(false);
	}

	/// <summary>
	/// Closes the pledge UI.
	/// </summary>
	public void BackFromPledgeUI()
	{
		if (m_pledgeUIAnimator.IsInState2)
		{
			// TODO: Short delay/animation before hiding pledge UI?
			ShowPledgeUI(false);
		}
	}
	
	/// <summary>
	/// Determines whether the pledge UI is shown.
	/// </summary>
	public bool IsPledgeUIShown()
	{
		return !m_pledgeUIAnimator.IsInState1;
	}

	/// <summary>
	/// Gets whether ResultsUI has been initialized.
	/// </summary>
	public bool IsInitialized
	{
		get { return m_isInitialized; }
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private TextMesh	m_totalScoreText	= null;
	[SerializeField] private TextMesh	m_bestScoreText		= null;
	[SerializeField] private TextMesh	m_tipTextWellDone	= null;
	[SerializeField] private TextMesh	m_tipTextTryHarder	= null;
	[SerializeField] private UIButton	m_pledgeButton		= null;
	[SerializeField] private UIButton	m_playAgainButton	= null;
	[SerializeField] private UIButton	m_mainMenuButton	= null;
	
	// Tip text dialogue BG
	[SerializeField] private GameObject	m_dialogueBGWellDone	= null;
	[SerializeField] private GameObject	m_dialogueBGTryHarder	= null;

	// UI positions
	[SerializeField] private Transform	m_mainMenuButtonWellDonePos		= null;
	[SerializeField] private Transform	m_mainMenuButtonTryHarderPos	= null;

	// Safety pledge
	[SerializeField] private SpriteRenderer	m_pledgeOverlay		= null;
	[SerializeField] private GameObject 	m_pledgeRoot		= null;
	[SerializeField] private UIButton		m_promiseButton		= null;
	[SerializeField] private UIButton		m_cancelButton		= null;
	[SerializeField] private float			m_pledgePopupSpeed	= 5.0f;
	[SerializeField] private Sprite 		m_pledgeButtonPressedSprite	= null;

	// Safety pledge thanks
	[SerializeField] private GameObject	m_pledgeThanks			= null;
	[SerializeField] private float		m_pledgeThanksDuration	= 2.0f;

	#endregion // Serialized Variables

	#region Variables

	private bool m_isInitialized = false;

	// Flag used for showing the "thanks" UI the first time the player takes the safety pledge
	private bool m_tookPledgeThisSession = false;

	#endregion // Variables

	#region Input Handling

	/// <summary>
	/// Reloads the minigames when Main Menu button is tapped.
	/// </summary>
	private void PlayAgainHandler(object sender, System.EventArgs e)
	{
		// Notify SceneMaster
		ResultsSceneMaster sceneMaster = (ResultsSceneMaster)Locator.GetSceneMaster();
		if (sceneMaster != null)
		{
			sceneMaster.NotifyPlayAgainButtonTapped();
		}
	}

	/// <summary>
	/// Loads the Main Menu scene when Main Menu button is tapped.
	/// </summary>
	private void MainMenuHandler(object sender, System.EventArgs e)
	{
		// Notify SceneMaster
		ResultsSceneMaster sceneMaster = (ResultsSceneMaster)Locator.GetSceneMaster();
		if (sceneMaster != null)
		{
			sceneMaster.NotifyMainMenuButtonTapped();
		}
	}
	
	/// <summary>
	/// Brings up the Safety Pledge UI when Pledge button is tapped.
	/// </summary>
	private void PledgeHandler(object sender, System.EventArgs e)
	{
		if (Locator.GetGameManager() != null && !Locator.GetGameManager().HasPledged)
		{
			if (m_pledgeUIAnimator.IsInState1)
			{
				ShowPledgeUI();
			}
		}
		else
		{
			if (m_pledgeThanksState == PledgeThanksState.HIDDEN)
			{
				ShowPledgeThanks();
			}
		}
	}

	/// <summary>
	/// Closes the Safety Pledge UI when Promise button is tapped.
	/// </summary>
	private void PromiseHandler(object sender, System.EventArgs e)
	{
		if (m_pledgeUIAnimator.IsInState2)
		{
			Locator.GetGameManager().NotifyPledgeTaken();

			LockPledgeButton();

			// Set flag to be used in showing the "thanks" UI the first time a player takes the pledge
			m_tookPledgeThisSession = true;

			// TODO: Short delay/animation before hiding pledge UI?
			ShowPledgeUI(false);
		}
	}

	/// <summary>
	/// Closes the Safety Pledge UI when Cancel button is tapped.
	/// </summary>
	private void CancelHandler(object sender, System.EventArgs e)
	{
		BackFromPledgeUI();
	}

	#endregion // Input Handling

	#region Safety Pledge

	private UIAnimator m_pledgeUIAnimator		= null;
	private UIAnimator m_pledgeOverlayAnimator	= null;

	/// <summary>
	/// Shows or hides the safety pledge UI.
	/// </summary>
	/// <param name="show">If set to <c>true</c> show the pledge UI.</param>
	private void ShowPledgeUI(bool show = true)
	{
		if (show)
		{
			m_pledgeRoot.SetActive(true);
			m_pledgeOverlay.gameObject.SetActive(true);

			m_pledgeUIAnimator.AnimateToState2();
			m_pledgeOverlayAnimator.AnimateToState2();
		}
		else
		{
			m_pledgeUIAnimator.AnimateToState1();
			m_pledgeOverlayAnimator.AnimateToState1();
		}

		// Enable/Disable buttons behind the pledge UI
		m_playAgainButton.GetComponent<Collider2D>().enabled = !show;
		m_mainMenuButton.GetComponent<Collider2D>().enabled = !show;
		m_pledgeButton.GetComponent<Collider2D>().enabled = !show;
	}

	/// <summary>
	/// Locks the pledge button in pressed state (when player has already pledged).
	/// </summary>
	private void LockPledgeButton()
	{
		m_pledgeButton.SetUnpressedSprite(m_pledgeButtonPressedSprite);
		m_pledgeButton.GetComponent<SpriteRenderer>().sprite = m_pledgeButtonPressedSprite;
	}

	#endregion // Safety Pledge

	#region Pledge Thanks

	private enum PledgeThanksState
	{
		SHOWING,
		SHOWN,
		HIDING,
		HIDDEN
	}
	private PledgeThanksState m_pledgeThanksState = PledgeThanksState.HIDDEN;

	private UIAnimator m_pledgeThanksAnimator = null;
	private float m_timeSinceShown = 0.0f;

	/// <summary>
	/// Shows the pledge thanks UI.
	/// </summary>
	private void ShowPledgeThanks()
	{
		m_pledgeThanks.SetActive(true);
		m_pledgeThanksState = PledgeThanksState.SHOWING;
		m_pledgeThanksAnimator.AnimateToState2();
	}

	/// <summary>
	/// Updates the pledge thanks animation.
	/// </summary>
	private void UpdatePledgeThanksAnimation()
	{
		m_pledgeThanksAnimator.Update(Time.deltaTime);
		switch (m_pledgeThanksState)
		{
		case PledgeThanksState.SHOWING:
			if (m_pledgeThanksAnimator.IsInState2)
			{
				m_pledgeThanksState = PledgeThanksState.SHOWN;
				m_timeSinceShown = 0.0f;
			}
			break;
		case PledgeThanksState.SHOWN:
			m_timeSinceShown += Time.deltaTime;
			if (m_timeSinceShown > m_pledgeThanksDuration)
			{
				m_pledgeThanksState = PledgeThanksState.HIDING;
				m_pledgeThanksAnimator.AnimateToState1();
			}
			break;
		case PledgeThanksState.HIDING:
			if (m_pledgeThanksAnimator.IsInState1)
			{
				m_pledgeThanksState = PledgeThanksState.HIDDEN;
				m_pledgeThanks.SetActive(false);
			}
			break;
		case PledgeThanksState.HIDDEN:
			break;
		}
	}

	#endregion // Pledge Thanks

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
		if (!m_isInitialized)
		{
			return;
		}

		m_pledgeUIAnimator.Update(Time.deltaTime);
		// Show the pledge thanks message the first time the player takes the safety pledge
		if (m_tookPledgeThisSession &&
		    Locator.GetGameManager() != null &&
		    Locator.GetGameManager().HasPledged &&
		    m_pledgeUIAnimator.IsInState1)
		{
			m_tookPledgeThisSession = false;
			ShowPledgeThanks();
		}
		m_pledgeOverlayAnimator.Update(Time.deltaTime);

		UpdatePledgeThanksAnimation();
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	private void OnDestroy()
	{

	}

	#endregion // MonoBehaviour
}
