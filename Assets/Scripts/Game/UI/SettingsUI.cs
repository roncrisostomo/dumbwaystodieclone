/******************************************************************************
*  @file       SettingsUI.cs
*  @brief      Handles the settings UI
*  @author     Ron
*  @date       August 20, 2015
*      
*  @par [explanation]
*		> Loads the Main Menu scene when Back button is tapped
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class SettingsUI : MonoBehaviour
{
	#region Public Interface

	/// <summary>
	/// Initialize this instance.
	/// </summary>
	public void Initialize()
	{
		// Settings buttons
		var soundButtonInitialState = Locator.GetGameManager().HasSounds ?
										UIToggleButton.ToggleButtonState.STATE1 :
										UIToggleButton.ToggleButtonState.STATE2;
		m_soundButton.Initialize(SoundHandler,
		                         SoundHandler_Crossed,
		                         UIToggleButton.TriggerType.ON_RELEASE,
		                         soundButtonInitialState);
		var includePlaneGameInitialState = Locator.GetGameManager().IsPlaneGameIncluded ?
											UIToggleButton.ToggleButtonState.STATE1 :
											UIToggleButton.ToggleButtonState.STATE2;
		m_includePlaneGameButton.Initialize(IncludePlaneGameHandler,
		                                    IncludePlaneGameHandler_Crossed,
		                                    UIToggleButton.TriggerType.ON_RELEASE,
		                                    includePlaneGameInitialState);
		m_resetProgressButton.Initialize(ResetProgressHandler, UIButton.TriggerType.ON_RELEASE);
		m_creditsButton.Initialize(CreditsHandler, UIButton.TriggerType.ON_RELEASE);
		m_backToMenuButton.Initialize(BackToMenuHandler, UIButton.TriggerType.ON_RELEASE);
		// Reset dialog buttons
		m_resetButton.Initialize(ResetHandler, UIButton.TriggerType.ON_RELEASE);
		m_cancelButton.Initialize(CancelHandler, UIButton.TriggerType.ON_RELEASE);
		// Credits Back button
		m_backButton.Initialize(BackHandler, UIButton.TriggerType.ON_RELEASE);

		// TODO: Delegates should not be in Main
		// Button sound delegates
		m_soundButton.AddSoundDelegates(Main.Instance.UIButtonPressHandler, Main.Instance.UIButtonReleaseHandler);
		m_includePlaneGameButton.AddSoundDelegates(Main.Instance.UIButtonPressHandler, Main.Instance.UIButtonReleaseHandler);
		m_resetProgressButton.AddSoundDelegates(Main.Instance.UIButtonPressHandler, Main.Instance.UIButtonReleaseHandler);
		m_creditsButton.AddSoundDelegates(Main.Instance.UIButtonPressHandler, Main.Instance.UIButtonReleaseHandler);
		m_backToMenuButton.AddSoundDelegates(Main.Instance.UIButtonPressHandler, Main.Instance.UIButtonReleaseHandler);
		m_resetButton.AddSoundDelegates(Main.Instance.UIButtonPressHandler, Main.Instance.UIButtonReleaseHandler);
		m_cancelButton.AddSoundDelegates(Main.Instance.UIButtonPressHandler, Main.Instance.UIButtonReleaseHandler);
		m_backButton.AddSoundDelegates(Main.Instance.UIButtonPressHandler, Main.Instance.UIButtonReleaseHandler);

		// Initialize credits animation
		InitializeCreditsAnimation();

		// Start reset dialog hidden
		ShowResetDialog(false);

		// Start credits UI hidden
		ShowCreditsUI(false);

		// Set the initialized flag
		m_isInitialized = true;
	}
	
	/// <summary>
	/// Closes the credits UI (and shows the settings UI).
	/// </summary>
	public void BackFromCredits()
	{
		ShowCreditsUI(false);
	}

	/// <summary>
	/// Determines whether the credits UI is shown.
	/// </summary>
	public bool IsCreditsShown()
	{
		return m_creditsUIRoot.activeSelf;
	}

	/// <summary>
	/// Closes the reset dialog (and shows the settings UI).
	/// </summary>
	public void BackFromResetDialog()
	{
		ShowResetDialog(false);
	}

	/// <summary>
	/// Determines whether the reset dialog is shown.
	/// </summary>
	public bool IsResetDialogShown()
	{
		return m_resetDialog.activeSelf;
	}

	#endregion // Public Interface

	#region Serialized Variables

	[Header("Settings buttons")]
	[SerializeField] private UIToggleButton	m_soundButton				= null;
	[SerializeField] private UIToggleButton	m_includePlaneGameButton	= null;
	[SerializeField] private UIButton		m_resetProgressButton		= null;
	[SerializeField] private UIButton		m_creditsButton				= null;
	[SerializeField] private UIButton		m_backToMenuButton			= null;

	[Header("Reset progress")]
	[SerializeField] private GameObject		m_resetDialog				= null;
	[SerializeField] private UIButton		m_resetButton				= null;
	[SerializeField] private UIButton		m_cancelButton				= null;

	[Header("Credits")]
	[SerializeField] private GameObject		m_creditsUIRoot				= null;
	[SerializeField] private GameObject		m_scrollingCreditsRoot		= null;
	[SerializeField] private UIButton		m_backButton				= null;
	[SerializeField] private Transform		m_creditsScrollStart		= null;
	[SerializeField] private Transform		m_creditsScrollEnd			= null;
	[SerializeField] private float 			m_creditsScrollSpeed 		= 0.06f;
	[SerializeField] private Transform		m_bsatTrigger				= null;
	[SerializeField] private SpriteRenderer	m_bsatMessage				= null;
	[SerializeField] private Animator 		m_psychoAnimator			= null;
	[SerializeField] private float			m_timeFromBSATToPsychoAnim	= 1.0f;
	[SerializeField] private float 			m_creditsFadeSpeed 			= 3.0f;

	#endregion // Serialized Variables

	#region Variables

	private bool m_isInitialized = false;

	#endregion // Variables

	#region Input Handling

	/// <summary>
	/// Toggles game sounds when Sound button is tapped while in "checked" state.
	/// </summary>
	private void SoundHandler(object sender, System.EventArgs e)
	{
		Locator.GetGameManager().NotifyToggleSounds();
	}

	/// <summary>
	/// Toggles game sounds when Sound button is tapped while in "crossed" state.
	/// </summary>
	private void SoundHandler_Crossed(object sender, System.EventArgs e)
	{
		Locator.GetGameManager().NotifyToggleSounds();
	}

	/// <summary>
	/// Toggles whether the plane minigame is included when Include Plane Game button is tapped while in "checked" state.
	/// </summary>
	private void IncludePlaneGameHandler(object sender, System.EventArgs e)
	{
		Locator.GetGameManager().NotifyToggleIncludePlaneGame();
	}

	/// <summary>
	/// Toggles whether the plane minigame is included when Include Plane Game button is tapped while in "crossed" state.
	/// </summary>
	private void IncludePlaneGameHandler_Crossed(object sender, System.EventArgs e)
	{
		Locator.GetGameManager().NotifyToggleIncludePlaneGame();
	}

	/// <summary>
	/// Displays the reset dialog UI when Reset Progress button is tapped.
	/// </summary>
	private void ResetProgressHandler(object sender, System.EventArgs e)
	{
		ShowResetDialog();
	}

	/// <summary>
	/// Brings up the credits UI when Credits button is tapped.
	/// </summary>
	private void CreditsHandler(object sender, System.EventArgs e)
	{
		ShowCreditsUI();
	}

	/// <summary>
	/// Loads the Main Menu scene when Back To Menu button is tapped.
	/// </summary>
	private void BackToMenuHandler(object sender, System.EventArgs e)
	{
		// Notify SceneMaster
		SettingsSceneMaster sceneMaster = (SettingsSceneMaster)Locator.GetSceneMaster();
		if (sceneMaster != null)
		{
			sceneMaster.NotifyBackToMenuButtonTapped();
		}
	}

	/// <summary>
	/// Resets game progress when Reset button is tapped.
	/// </summary>
	private void ResetHandler(object sender, System.EventArgs e)
	{
		Locator.GetGameManager().NotifyResetProgress();
		ShowResetDialog(false);
	}

	/// <summary>
	/// Hides the reset dialog UI when Cancel button is tapped.
	/// </summary>
	private void CancelHandler(object sender, System.EventArgs e)
	{
		BackFromResetDialog();
	}

	/// <summary>
	/// Hides the credits UI when Back button is tapped.
	/// </summary>
	private void BackHandler(object sender, System.EventArgs e)
	{
		BackFromCredits();
	}

	#endregion // Input Handling

	#region Reset Dialog

	/// <summary>
	/// Shows or hides the reset dialog.
	/// </summary>
	/// <param name="show">If set to <c>true</c> show the reset dialog. Else hide it.</param>
	private void ShowResetDialog(bool show = true)
	{
		// Show/hide reset dialog
		m_resetDialog.SetActive(show);
	}

	#endregion // Reset Dialog

	#region Credits UI

	private UIAnimator m_creditsFader		= null;
	private UIAnimator m_creditsScroller 	= null;
	private UIAnimator m_creditsBSATFader	= null;

	private float m_timeSinceBSATFadeStart	= 0.0f;

	private const string PSYCHO_CREDITS_ANIM_NAME 	= "CreditsPsycho";

	/// <summary>
	/// Initializes the credits animation.
	/// </summary>
	private void InitializeCreditsAnimation()
	{
		// Animator that scrolls the credits
		m_creditsScroller = new UIAnimator(m_scrollingCreditsRoot.transform);
		m_creditsScroller.SetPositionAnimation(m_creditsScrollStart.position, m_creditsScrollEnd.position);
		m_creditsScroller.SetAnimSpeed(m_creditsScrollSpeed);

		// Animator that fades in the credits text when the credits starts scrolling
		m_creditsFader = new UIAnimator(m_scrollingCreditsRoot.transform,
		                                UIAnimator.UIAnimatorType.COLOR,
		                                true, true);
		m_creditsFader.SetAlphaAnimation(0.0f, 1.0f);
		m_creditsFader.SetAnimSpeed(m_creditsFadeSpeed);

		// Animator that fades in the "Be Safe Around Trains" message when the credits scrolling is about to end
		m_creditsBSATFader = new UIAnimator(m_bsatMessage);
		m_creditsBSATFader.SetAlphaAnimation(0.0f, 1.0f);
		m_creditsBSATFader.SetAnimSpeed(m_creditsFadeSpeed);
	}

	/// <summary>
	/// Shows the credits UI.
	/// </summary>
	/// <param name="show">If set to <c>true</c> show the credits UI. Else, hide it.</param>
	private void ShowCreditsUI(bool show = true)
	{
		// Start "Be Safe Around Trains" message hidden
		m_bsatMessage.SetAlpha(0.0f);

		// Start psycho character hidden
		m_psychoAnimator.Play(PSYCHO_CREDITS_ANIM_NAME);
		m_psychoAnimator.speed = 0.0f;
		m_timeSinceBSATFadeStart = 0.0f;
		
		if (show)
		{
			// Start credits animation
			m_creditsFader.AnimateToState2();
			m_creditsScroller.AnimateToState2();
		}
		else
		{
			// Reset animation
			m_creditsFader.ResetToState(UIAnimator.UIAnimationState.STATE1);
			m_creditsScroller.ResetToState(UIAnimator.UIAnimationState.STATE1);
			m_creditsBSATFader.ResetToState(UIAnimator.UIAnimationState.STATE1);
		}

		// Enable/disable UI
		m_creditsUIRoot.SetActive(show);
		m_bsatMessage.gameObject.SetActive(show);
	}

	/// <summary>
	/// Updates the credits animation.
	/// </summary>
	private void UpdateCreditsAnimation()
	{
		if (!m_creditsUIRoot.activeSelf)
		{
			return;
		}

		m_creditsFader.Update(Time.deltaTime);
		m_creditsScroller.Update(Time.deltaTime);
		m_creditsBSATFader.Update(Time.deltaTime);

		switch (m_creditsBSATFader.UIAnimState)
		{
		case UIAnimator.UIAnimationState.STATE1:
			// Show "Be Safe Around Trains" message when credits scrolling is about to end
			if (m_bsatTrigger.position.y > Locator.GetSceneMaster().UICamera.ScreenMaxWorld.y)
			{
				m_bsatMessage.gameObject.SetActive(true);
				m_creditsBSATFader.AnimateToState2();
			}
			break;
		case UIAnimator.UIAnimationState.TO_STATE2:
		case UIAnimator.UIAnimationState.STATE2:
			// Show psycho character when "Be Safe Around Trains" fades in
			m_timeSinceBSATFadeStart += Time.deltaTime;
			if (m_timeSinceBSATFadeStart > m_timeFromBSATToPsychoAnim &&
			    m_psychoAnimator.speed == 0.0f)
			{
				m_psychoAnimator.speed = 1.0f;
			}
			break;
		}
	}

	#endregion // Credits UI

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

		UpdateCreditsAnimation();
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	private void OnDestroy()
	{

	}

	#endregion // MonoBehaviour
}
