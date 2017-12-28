/******************************************************************************
*  @file       SoundSystemTester.cs
*  @brief      Testers for SoundSystem features
*  @author     Ron
*  @date       July 26, 2015
*      
*  @par [explanation]
*		> Creates GUI buttons that call various SoundSystem functions
*		> Before using this tester class, certain sound assets should be set up:
*			SFX-Click, SFX-Pizza, BGM-DumbWaysToDieMenuTheme, BGM-MysticForest
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class SoundSystemTester : MonoBehaviour
{
	#region Variables

	private SoundObject m_regularSound = null;
	private SoundObject m_bgm = null;
	private SoundObject m_persistentBgm = null;
	private bool m_allPaused = false;

	#endregion // Variables

	#region MonoBehaviour

	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start()
	{
		DontDestroyOnLoad(this.gameObject);
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update()
	{

	}

	/// <summary>
	/// Raises the GU event.
	/// </summary>
	private void OnGUI()
	{
		float width = 150.0f;
		float height = 30.0f;
		float x = (Screen.width - width) * 0.5f;
		float y = 15.0f;
		float spacing = 5.0f;

		// One-shot
		if (GUI.Button(new Rect(x, y, width, height), "Play one-shot"))
		{
			Locator.GetSoundSystem().PlayOneShot(SoundInfo.SFXID.WASP_SWAT);
		}

		// Regular
		y += height + spacing;
		if (GUI.Button(new Rect(x, y, width, height), "Play regular"))
		{
			if (m_regularSound == null)
			{
				m_regularSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.WIN02);
			}
			else
			{
				if (m_regularSound.IsPlaying)	m_regularSound.Pause();
				else 							m_regularSound.Play();
			}
		}

		// BGM
		y += height + spacing;
		if (GUI.Button(new Rect(x, y, width, height), "Play BGM"))
		{
			if (m_bgm == null)
			{
				m_bgm = Locator.GetSoundSystem().PlayBGM(SoundInfo.BGMID.MAIN_MENU);
			}
			else
			{
				if (m_bgm.IsPlaying)	m_bgm.Pause();
				else 					m_bgm.Unpause();
			}
		}

		// Persistent BGM
		y += height + spacing;
		if (GUI.Button(new Rect(x, y, width, height), "Play persistent BGM"))
		{
			if (m_persistentBgm == null)
			{
				m_persistentBgm = Locator.GetSoundSystem().PlayBGM(SoundInfo.BGMID.MINI_GAME, true);
			}
			else
			{
				if (m_persistentBgm.IsPlaying)	m_persistentBgm.Pause();
				else 							m_persistentBgm.Unpause();
			}
		}

		// Pause
		y += height + spacing;
		if (GUI.Button(new Rect(x, y, width, height), "Pause all sounds"))
		{
			if (!m_allPaused) 	Locator.GetSoundSystem().PauseAllSounds();
			else 				Locator.GetSoundSystem().UnpauseAllSounds();
			
			m_allPaused = !m_allPaused;
		}

		// Delete all sounds
		y += height + spacing;
		if (GUI.Button(new Rect(x, y, width, height), "Delete all sounds"))
		{
			Locator.GetSoundSystem().DeleteAllSoundObjects();
		}

		// Delete all except persistent sounds
		y += height + spacing;
		if (GUI.Button(new Rect(x, y, width, height), "Delete all except\npersistent sounds"))
		{
			Locator.GetSoundSystem().DeleteAllSoundObjects(false);
		}
	}

	#endregion // MonoBehaviour
}
