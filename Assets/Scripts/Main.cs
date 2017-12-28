/******************************************************************************
*  @file       Main.cs
*  @brief      Handles first-run initialization and scene transitions
*  @author     Ron
*  @date       August 18, 2015
*      
*  @par [explanation]
*		> Initializes systems that need initializing upon loading of the app
*		> In scene transitions, loads resources for the next scene,
*			unloads resources from previous scene, and optionally shows a
*			loading screen during this period
******************************************************************************/

//#define SHOW_STATE_LOGS

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class Main : MonoBehaviour
{
	#region Public Interface

	// Singleton instance
	public static Main Instance
	{
		get { return s_instance; }
	}

	/// <summary>
	/// Notifies Main of request to load the specified scene.
	/// </summary>
	/// <returns><c>true</c>, if scene switch request is granted, <c>false</c> otherwise.</returns>
	/// <param name="nextScene">Scene to switch to.</param>
	/// <param name="fadeOut">Whether scene should fade out before switching to the next scene</param>
	public bool NotifySwitchScene(SceneInfo.SceneEnum nextScene, bool fadeOut = true)
	{
		if (nextScene == SceneInfo.SceneEnum.SIZE)
		{
			Debug.LogWarning("Specified item is not a scene");
			return false;
		}

		// If already loading a new scene, do not load another scene
		if (m_curScene != m_nextScene)
		{
			return false;
		}

		m_nextScene = nextScene;
		m_enableFadeAnim = fadeOut;

		// Start scene loading
		m_initState = InitState.LOAD_SCENE;

		return true;
	}

	/// <summary>
	/// Sets the scene master.
	/// </summary>
	/// <param name="sceneMaster">Scene master.</param>
	public void SetSceneMaster(SceneMasterBase sceneMaster)
	{
		m_sceneMaster = sceneMaster;
	}

	// =========================  TEMPORARY  ==========================
	// TODO: Move these to some other central UI script
	public void UIButtonPressHandler(object sender, System.EventArgs e)
	{
		m_soundSystem.PlayOneShot(SoundInfo.SFXID.UI_BUTTON_PRESS);
	}
	public void UIButtonReleaseHandler(object sender, System.EventArgs e)
	{
		m_soundSystem.PlayOneShot(SoundInfo.SFXID.UI_BUTTON_RELEASE);
	}
	// =========================  TEMPORARY  ==========================

	/// <summary>
	/// Gets the scene enum of the current scene.
	/// </summary>
	public SceneInfo.SceneEnum GetCurSceneEnum
	{
		get { return m_curScene; }
	}

	/// <summary>
	/// Gets the TimerUI.
	/// </summary>
	public TimerUI GetTimerUI
	{
		get { return m_timerUI; }
	}

	/// <summary>
	/// Gets the PauseUI.
	/// </summary>
	public PauseUI GetPauseUI
	{
		get { return m_pauseUI; }
	}

	/// <summary>
	/// Gets the ScoreUI.
	/// </summary>
	public ScoreUI GetScoreUI
	{
		get { return m_scoreUI; }
	}

	/// <summary>
	/// Gets the FasterUI.
	/// </summary>
	public FasterUI GetFasterUI
	{
		get { return m_fasterUI; }
	}

	/// <summary>
	/// Gets whether the scene is initialized and game is ready to run.
	/// </summary>
	public bool IsSceneInitialized
	{
		get { return m_initState == InitState.IDLE; }
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private SceneMasterBase m_sceneMaster = null;

	[SerializeField] private bool m_enableTouchDebugger = false;

	#endregion // Serialized Variables
	
	#region InitState

	// Note: Systems will be initialized in the order they are listed in this enum
	private enum InitState
	{
		NONE = 0,

		INIT_SHARED_SYSTEMS,
		WAIT_INIT_SHARED_SYSTEMS,
		CREATE_SHARED_OBJECTS,

		INIT_LOCATOR,			// Should be after shared system initialization and before scene loading

		LOAD_SCENE,				// When switching scenes, set m_nextScene, then switch state to LOAD_SCENE
		WAIT_LOAD_SCENE,
		UNLOAD_CUR_SCENE,
		WAIT_UNLOAD_CUR_SCENE,
		SWITCH_SCENE,
		LOAD_SCENE_MASTER,
		WAIT_LOAD_SCENE_MASTER,

		DONE,
		IDLE,
	}
	private InitState m_initState = InitState.NONE;

	#endregion // InitState

	#region Variables
	
	private static 	Main 	s_instance 		= null;

	// Common systems
	private UISystemBase 	m_uiSystem 		= null;
	private SoundSystemBase m_soundSystem 	= null;
	private DataSystemBase 	m_dataSystem	= null;

	// Each scene has a Main script which is not destroyed on scene switch,
	//	meaning multiple instances of Main will exist (at least for a while,
	//	until the second instance destroys itself).
	// 	This flag marks the instance that will remain in the game.
	private bool m_isActiveMain	= false;

	#endregion // Variables
	
	#region Initialization

	// Max progress that async operation reaches when scene activation is deferred,
	//	i.e. allowSceneActivation = false
	private const float READY_TO_LOAD_PROGRESS = 0.9f;

	// Whether there should be fade in/out animation when switching scenes
	private bool m_enableFadeAnim = false;

	/// <summary>
	/// Updates the initialization process.
	/// </summary>
	private void UpdateInitState()
	{
		switch (m_initState)
		{
		case InitState.NONE:
			AdvanceInitState();
			break;
		
		case InitState.INIT_SHARED_SYSTEMS:
			// Get references to the shared systems
			m_uiSystem = this.gameObject.AddDerivedIfNoBase<UISystemBase, UISystem>();
			m_soundSystem = this.gameObject.AddDerivedIfNoBase<SoundSystemBase, SoundSystem>();
			m_dataSystem = new DataSystem();

			// Initialize shared systems
			m_uiSystem.Initialize();
			m_soundSystem.Initialize();
			m_dataSystem.Initialize();

#if SHOW_STATE_LOGS
			Debug.Log("Initializing shared systems...");
#endif

			AdvanceInitState();
			break;
		case InitState.WAIT_INIT_SHARED_SYSTEMS:
			// Wait for shared systems to finish initialization
#if SHOW_STATE_LOGS
			Debug.Log("Waiting for shared system initialization to finish...");
#endif

			if (m_uiSystem.IsInitialized &&
			    m_soundSystem.IsInitialized &&
			    m_dataSystem.IsInitialized)
			{
				AdvanceInitState();
			}
			break;
		// TODO: Temporary state - should be moved to different classes
		case InitState.CREATE_SHARED_OBJECTS:
#if SHOW_STATE_LOGS
			Debug.Log("Creating shared objects...");
#endif
			CreateSharedObjects();

			AdvanceInitState();
			break;
		case InitState.INIT_LOCATOR:
#if SHOW_STATE_LOGS
			Debug.Log("Initializing Service Locator...");
#endif
			Locator.ProvideUISystem(m_uiSystem);
			Locator.ProvideSoundSystem(m_soundSystem);
			Locator.ProvideDataSystem(m_dataSystem);

			// If game is loaded from the Main scene (index 0)
			if (Application.loadedLevel == 0)
			{
				AdvanceInitState();
			}
			// If game is loaded from a different scene
			else
			{
				// No need to load a different scene
				// Just load the SceneMaster
				m_initState = InitState.LOAD_SCENE_MASTER;
			}
			break;

		// The following states are cycled through whenever scenes are switched
		case InitState.LOAD_SCENE:
#if SHOW_STATE_LOGS
			Debug.Log("Loading scene...");
#endif
			// Block input while next scene is loading
			m_faderUI.SetBlockInput();

			StartLoading(m_sceneInfo.GetSceneNameOf(m_nextScene));

			AdvanceInitState();
			break;
		case InitState.WAIT_LOAD_SCENE:
			// Wait for scene to finish loading in the background
#if SHOW_STATE_LOGS
			Debug.Log("Waiting for scene to load in the background...");
#endif
			if (m_async != null && m_async.progress >= READY_TO_LOAD_PROGRESS)
			{
				// Start fade out
				if (m_enableFadeAnim)
				{
					m_faderUI.FadeOut(true);
				}

				AdvanceInitState();
			}
			break;
		case InitState.UNLOAD_CUR_SCENE:
			// If starting from Main scene, there will be nothing to unload
#if SHOW_STATE_LOGS
			Debug.Log("Unloading current scene...");
#endif

			if (Application.loadedLevel == 0 || m_sceneMaster.Unload())
			{
				AdvanceInitState();
			}
			break;
		case InitState.WAIT_UNLOAD_CUR_SCENE:
			// If starting from Main scene, there will be nothing to unload
#if SHOW_STATE_LOGS
			Debug.Log("Waiting for current scene to finish unloading...");
#endif
			if (Application.loadedLevel == 0 || !m_sceneMaster.IsInitialized)
			{
				// If scene fading is enabled, wait for scene to fade out first
				if (!m_enableFadeAnim ||
				    (m_enableFadeAnim && m_faderUI.FaderState == FaderUI.FadeAnimationState.FADED_OUT))
				{
					// Clean up non-persistent sounds
					m_soundSystem.DeleteAllSoundObjects(false);

					AdvanceInitState();
				}
			}
			break;
		case InitState.SWITCH_SCENE:
			// Load the next scene
#if SHOW_STATE_LOGS
			Debug.Log("Switching scene...");
#endif
			ActivateScene();
			// Initialization will continue in OnLevelWasLoaded
			break;

		case InitState.LOAD_SCENE_MASTER:
#if SHOW_STATE_LOGS
			Debug.Log("Loading scene master in scene " + Application.loadedLevelName);
#endif
			if (m_sceneMaster.Load())
			{
				// Provide SceneMaster to the service locator
				Locator.ProvideSceneMaster(m_sceneMaster);

				AdvanceInitState();
			}
			break;
		case InitState.WAIT_LOAD_SCENE_MASTER:
#if SHOW_STATE_LOGS
			Debug.Log("Waiting for scene master to load...");
#endif
			if (m_sceneMaster.IsInitialized)
			{
				// Start fade in
				if (m_enableFadeAnim)
				{
					m_faderUI.FadeIn();
				}

				AdvanceInitState();
			}
			break;
		
		case InitState.DONE:
#if SHOW_STATE_LOGS
			if (BuildInfo.IsDebugMode)
			{
				Debug.Log("Main initialization complete");
			}
#endif
			// Switch to IDLE state
			// If the SceneMaster switches the scene, this state change will be overridden
			AdvanceInitState();

			// Update scene enum for the current scene
			m_curScene = m_nextScene;
			// Start the scene - pass control over to the active scene master
			m_sceneMaster.StartScene();
			break;
		case InitState.IDLE:
			break;
		}
	}

	/// <summary>
	/// Advances initialization to the next phase.
	/// </summary>
	private void AdvanceInitState()
	{
		// Move on to the next initialization phase
		if (m_initState != InitState.IDLE)
		{
			m_initState = (InitState)((int)(m_initState + 1));
		}
	}

	/// <summary>
	/// Initializes objects shared between scenes.
	/// </summary>
	private void InitializeSharedObjects()
	{
		// These UI are hidden by default. Call their Show method if they will be needed in a scene.
		if (!m_faderUI.IsInitialized)
		{
			m_faderUI.Initialize();
			m_faderUI.transform.SetPosZ(0.0f);
		}
		if (!m_timerUI.IsInitialized)
		{
			m_timerUI.Initialize();
			m_timerUI.transform.SetPosZ(0.0f);
		}
		if (!m_pauseUI.IsInitialized)
		{
			m_pauseUI.Initialize();
			m_pauseUI.transform.SetPosZ(0.0f);
		}
		if (!m_scoreUI.IsInitialized)
		{
			m_scoreUI.Initialize();
			m_scoreUI.transform.SetPosZ(0.0f);
		}
		if (!m_fasterUI.IsInitialized)
		{
			m_fasterUI.Initialize();
			m_fasterUI.transform.SetPosZ(0.0f);
		}
	}
	
	#endregion // Initialization
	
	#region Shared Objects
	
	private FaderUI 	m_faderUI 	= null;
	private TimerUI 	m_timerUI 	= null;
	private PauseUI 	m_pauseUI 	= null;
	private ScoreUI 	m_scoreUI 	= null;
	private FasterUI 	m_fasterUI 	= null;

	private const string FADER_UI_PREFAB_PATH	= "Prefabs/FaderUI";
	private const string TIMER_UI_PREFAB_PATH	= "Prefabs/TimerUI";
	private const string PAUSE_UI_PREFAB_PATH	= "Prefabs/PauseUI";
	private const string SCORE_UI_PREFAB_PATH	= "Prefabs/ScoreUI";
	private const string FASTER_UI_PREFAB_PATH	= "Prefabs/FasterUI";

	/// <summary>
	/// Creates and initializes the shared objects.
	/// </summary>
	private void CreateSharedObjects()
	{
		m_faderUI = CreateSharedUIFromPrefab<FaderUI>(FADER_UI_PREFAB_PATH);
		m_timerUI = CreateSharedUIFromPrefab<TimerUI>(TIMER_UI_PREFAB_PATH);
		m_pauseUI = CreateSharedUIFromPrefab<PauseUI>(PAUSE_UI_PREFAB_PATH);
		m_scoreUI = CreateSharedUIFromPrefab<ScoreUI>(SCORE_UI_PREFAB_PATH);
		m_fasterUI = CreateSharedUIFromPrefab<FasterUI>(FASTER_UI_PREFAB_PATH);

		m_faderUI.transform.SetPosZ(-100000.0f);	// Instantiate outside camera view
		DontDestroyOnLoad(m_faderUI.gameObject);
		m_faderUI.Initialize();						// Hide (shared UI start hidden)
		m_faderUI.transform.SetPosZ(0.0f);			// Restore position once hidden

		m_timerUI.transform.SetPosZ(-100000.0f);
		DontDestroyOnLoad(m_timerUI.gameObject);
		m_timerUI.Initialize();
		m_timerUI.transform.SetPosZ(0.0f);

		m_pauseUI.transform.SetPosZ(-100000.0f);
		DontDestroyOnLoad(m_pauseUI.gameObject);
		m_pauseUI.Initialize();
		m_pauseUI.transform.SetPosZ(0.0f);

		m_scoreUI.transform.SetPosZ(-100000.0f);
		DontDestroyOnLoad(m_scoreUI.gameObject);
		m_scoreUI.Initialize();
		m_scoreUI.transform.SetPosZ(0.0f);

		m_fasterUI.transform.SetPosZ(-100000.0f);
		DontDestroyOnLoad(m_fasterUI.gameObject);
		m_fasterUI.Initialize();
		m_fasterUI.transform.SetPosZ(0.0f);

		m_timerUI.Hide();
		m_pauseUI.Hide();
		m_scoreUI.Hide();
		m_fasterUI.Hide();
	}

	/// <summary>
	/// Creates a shared UI object from prefab.
	/// </summary>
	/// <returns>The shared UI class.</returns>
	/// <param name="prefabPath">Prefab path.</param>
	/// <typeparam name="T">Shared UI class type.</typeparam>
	private T CreateSharedUIFromPrefab<T>(string prefabPath) where T : Component
	{
		GameObject resource = Resources.Load(prefabPath, typeof(GameObject)) as GameObject;
		if (resource != null)
		{
			GameObject obj = GameObject.Instantiate(resource);
			return obj.AddComponentNoDupe<T>();
		}
		Debug.LogError("No prefab found at " + prefabPath);
		return null;
	}
	
	#endregion // Shared Objects

	#region Scene Management
	
	private AsyncOperation 	m_async		= null;
	private	SceneInfo		m_sceneInfo	= new SceneInfo();

	private SceneInfo.SceneEnum m_nextScene = (SceneInfo.SceneEnum)0;
	private SceneInfo.SceneEnum m_curScene = (SceneInfo.SceneEnum)0;

	private const	string	SCENE_MASTER_OBJ_NAME	= "SceneMaster";
	
	/// <summary>
	/// Starts loading the next scene.
	/// </summary>
	private void StartLoading(string levelName)
	{
		StartCoroutine(Load(levelName));
	}

	/// <summary>
	/// Loads the specified level asynchronously.
	/// </summary>
	/// <param name="levelName">Name of level to load.</param>
	private IEnumerator Load(string levelName)
	{
#if SHOW_STATE_LOGS
		if (BuildInfo.IsDebugMode)
		{
			Debug.Log("Loading " + levelName + " scene...");
		}
#endif
		m_async = Application.LoadLevelAsync(levelName);
		m_async.allowSceneActivation = false;
		yield return m_async;
	}

	/// <summary>
	/// Allows the next scene to activate once ready.
	/// </summary>
	private void ActivateScene()
	{
		m_async.allowSceneActivation = true;
	}
	
	/// <summary>
	/// Finds the SceneMaster in the new scene.
	/// </summary>
	private SceneMasterBase FindSceneMaster()
	{
		GameObject obj = GameObject.Find(SCENE_MASTER_OBJ_NAME);
		if (obj == null)
		{
			Debug.LogError("No SceneMaster object found!");
		}
		SceneMasterBase sceneMaster = obj.GetComponent<SceneMasterBase>();
		if (sceneMaster == null)
		{
			Debug.LogError("No SceneMaster component found!");
		}
		return sceneMaster;
	}

	#endregion // Scene Management

	#region Touch Debugger

	/// <summary>
	/// Updates whether the touch debugger should be active or not.
	/// </summary>
	private void UpdateTouchDebugger()
	{
		if (BuildInfo.IsDebugMode)
		{
			if (m_sceneMaster != null && m_sceneMaster.TouchDebugger != null)
			{
				m_sceneMaster.TouchDebugger.gameObject.SetActive(m_enableTouchDebugger);
			}
		}
	}

	#endregion // Touch Debugger

	#region MonoBehaviour

	/// <summary>
	/// Awake this instance.
	/// </summary>
	private void Awake()
	{
		if (Main.Instance == null)
		{
			s_instance = this;
			// Main is always present in the game
			DontDestroyOnLoad(this.gameObject);
			// Mark this as the only instance of Main allowed in the game
			m_isActiveMain = true;
		}
		else
		{
			// Pass the reference to this scene's SceneMaster to the existing Main instance
			Main.Instance.SetSceneMaster(m_sceneMaster);
			// There can be only one instance of a singleton in a scene
			Destroy(this.gameObject);
		}
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start()
	{
		// Initialize the service locator
		Locator.Initialize();
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update()
	{
		UpdateInitState();

		UpdateTouchDebugger();
	}

	/// <summary>
	/// Raises the level loaded event.
	/// </summary>
	/// <param name="loadedLevelIndex">Index of the loaded level in the Build Settings scene list.</param>
	private void OnLevelWasLoaded(int loadedLevelIndex)
	{
		// Only the active Main instance should handle this event
		if (m_isActiveMain)
		{
#if SHOW_STATE_LOGS
			if (BuildInfo.IsDebugMode)
			{
				Debug.Log("Level loaded!");
			}
#endif

			// TODO: Improve? Currently searches scene master by name
			SetSceneMaster(FindSceneMaster());

			// Proceed with initialization
			AdvanceInitState();
		}
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	private void OnDestroy()
	{
		if (!m_isActiveMain)
		{
			s_instance = null;
		}
	}

	#endregion // MonoBehaviour
}
