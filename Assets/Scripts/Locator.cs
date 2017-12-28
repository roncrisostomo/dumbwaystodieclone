/******************************************************************************
*  @file       Locator.cs
*  @brief      Service locator class
*  @author     
*  @date       July 21, 2015
*      
*  @par [explanation]
*		> Provides a global point of access to various services
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public static class Locator
{
	#region Public Interface

	/// <summary>
	/// Initialize the service locator.
	/// </summary>
	public static void Initialize()
	{
		// Initialize service references to "null" services
	}

	// Locators
	public static SceneMasterBase GetSceneMaster()
	{
		return m_sceneMaster;
	}
	public static UISystemBase GetUISystem()
	{
		return m_uiSystem;
	}
	public static SoundSystemBase GetSoundSystem()
	{
		return m_soundSystem;
	}
	public static DataSystemBase GetDataSystem()
	{
		return m_dataSystem;
	}
	public static GameManager GetGameManager()
	{
		return m_gameManager;
	}
	public static SoundManager GetSoundManager()
	{
		return m_soundManager;
	}

	// Providers
	public static void ProvideSceneMaster(SceneMasterBase sceneMaster)
	{
		m_sceneMaster = sceneMaster;
	}
	public static void ProvideUISystem(UISystemBase uiSystem)
	{
		m_uiSystem = uiSystem;
	}
	public static void ProvideSoundSystem(SoundSystemBase soundSystem)
	{
		m_soundSystem = soundSystem;
	}
	public static void ProvideDataSystem(DataSystemBase dataSystem)
	{
		m_dataSystem = dataSystem;
	}
	public static void ProvideGameManager(GameManager gameManager)
	{
		m_gameManager = gameManager;
	}
	public static void ProvideSoundManager(SoundManager soundManager)
	{
		m_soundManager = soundManager;
	}

	#endregion // Public Interface

	#region Serialized Variables

	#endregion // Serialized Variables

	#region References

	private static SceneMasterBase	m_sceneMaster 	= null;
	private static UISystemBase		m_uiSystem 		= null;
	private static SoundSystemBase	m_soundSystem	= null;
	private static DataSystemBase	m_dataSystem	= null;

	// Game-specific
	private static GameManager		m_gameManager	= null;
	private	static SoundManager		m_soundManager	= null;

	#endregion // References
}
