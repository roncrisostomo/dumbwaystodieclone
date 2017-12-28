/******************************************************************************
*  @file       SoundManager.cs
*  @brief      Oversees the playing of sounds and music
*  @author     Lori
*  @date       July 27, 2015
*      
*  @par [explanation]
*		> Plays the game BGMs
*		> Plays shared SFXs
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#endregion // Namespaces

public class SoundManager : MonoBehaviour
{
	#region Public Interface

	/// <summary>
	/// Sets a value indicating whether this instance has sounds.
	/// </summary>
	/// <value><c>true</c> if this instance has sounds; otherwise, <c>false</c>.</value>
	public bool HasSounds
	{
		get { return m_soundsOn; }
	}
	
	/// <summary
	/// Sets the active sounds.
	/// </summary>
	/// <param name="areSoundsOn">If set to <c>true</c> are sounds on.</param>
	public void SetActiveSounds(bool areSoundsOn)
	{
		m_soundsOn = areSoundsOn;
		if (m_soundsOn)
		{
			Locator.GetSoundSystem().UnmuteAllSounds();
		}
		else
		{
			Locator.GetSoundSystem().MuteAllSounds(true);
		}
	}
	
	/// <summary>
	/// Notifies the pause.
	/// </summary>
	public void NotifyPause()
	{
		PauseBGM();
		PauseWinLoseSFX();
	}

	/// <summary>
	/// Notifies the unpause.
	/// </summary>
	public void NotifyUnpause()
	{
		UnpauseBGM();
		UnpauseWinLoseSFX();
	}

	/// <summary>
	/// Plays the main menu BGM.
	/// </summary>
	public void PlayMainMenuBGM()
	{
		PlayBGMWithID(SoundInfo.BGMID.MAIN_MENU);
	}

	/// <summary>
	/// Plays the mini game BGM.
	/// </summary>
	public void PlayMiniGameBGM()
	{
		PlayBGMWithID(SoundInfo.BGMID.MINI_GAME);
	}

	/// <summary>
	/// Adjusts the mini game BGM pitch.
	/// </summary>
	/// <param name="currentLevel">Current level.</param>
	public void AdjustMiniGameBGMPitch(uint currentLevel)
	{
		int i = Mathf.Clamp((int)currentLevel, 0, m_miniGameBGMPitch.Length - 1);
		float pitch = m_miniGameBGMPitch[i];
		int bgmID = (int)SoundInfo.BGMID.MINI_GAME;
		if (m_bgmArray[bgmID] != null)
		{
			m_bgmArray[bgmID].SetPitch(pitch);
			m_bgmArray[bgmID].Play(m_restartOnPitchChange);
		}
	}

	/// <summary>
	/// Stops the current bgm.
	/// </summary>
	public void StopBGM()
	{
		if (m_currentBGMID != SoundInfo.BGMID.SIZE)
		{
			m_bgmArray[(int)m_currentBGMID].Stop();
		}
	}

	/// <summary>
	/// Pauses the current bgm.
	/// </summary>
	public void PauseBGM()
	{
		if (m_currentBGMID != SoundInfo.BGMID.SIZE)
		{
			m_bgmArray[(int)m_currentBGMID].Pause();
		}
	}

	/// <summary>
	/// Unpauses the current bgm.
	/// </summary>
	public void UnpauseBGM()
	{
		if (m_currentBGMID != SoundInfo.BGMID.SIZE)
		{
			m_bgmArray[(int)m_currentBGMID].Unpause();
		}
	}

	/// <summary>
	/// Plays the mini game window sound.
	/// </summary>
	public void PlayMiniGameWinSound()
	{
		SoundInfo.SFXID randomSound = (SoundInfo.SFXID)Random.Range((int)SoundInfo.SFXID.WIN01,
		                                                            (int)SoundInfo.SFXID.WIN03 + 1);
		PlayNewWinLoseSFX(randomSound);
	}

	/// <summary>
	/// Plays the mini game window sound.
	/// </summary>
	public void PlayMiniGameLoseSound()
	{
		SoundInfo.SFXID randomSound = (SoundInfo.SFXID)Random.Range((int)SoundInfo.SFXID.LOSE01,
		                                                            (int)SoundInfo.SFXID.LOSE03 + 1);
		PlayNewWinLoseSFX(randomSound);
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private	bool	m_soundsOn				= true;
	[SerializeField] private	float[]	m_miniGameBGMPitch		=
	{
		1.0f,
		1.1f,
		1.25f,
		1.4f,
		1.55f,
		1.7f
	};
	[SerializeField] private	bool	m_restartOnPitchChange	= true;

	#endregion // Serialized Variables

	/// <summary>
	/// Awake this instance.
	/// </summary>
	private void Awake()
	{
		if (Locator.GetSoundManager() == null)
		{
			// Pass reference to Locator
			Locator.ProvideSoundManager(this);
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			// Self-destruct if an instance is already present
			Destroy(this.gameObject);
		}
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
		
	}

	#region BGM

	private		SoundInfo.BGMID		m_currentBGMID		= SoundInfo.BGMID.SIZE;
	private		SoundObject[]		m_bgmArray			= new SoundObject[(int)SoundInfo.BGMID.SIZE];

	/// <summary>
	/// Plays the BGM of the given id
	/// </summary>
	/// <param name="bgmId">id.</param>
	private void PlayBGMWithID(SoundInfo.BGMID bgmID)
	{
		if (bgmID == SoundInfo.BGMID.SIZE)
		{
			return;
		}

		// Stop the current BGM first
		StopBGM();
		
		// Play the new one and update the index
		int i = (int)bgmID;
		if (m_bgmArray[i] == null)
		{
			m_bgmArray[i] = Locator.GetSoundSystem().PlayBGM(bgmID, true);
			m_bgmArray[i].transform.parent = transform;
			if (!m_soundsOn)
			{
				m_bgmArray[i].Mute();
			}
		}
		else
		{
			m_bgmArray[i].Play(true);
		}
		
		// Update the index
		m_currentBGMID = bgmID;
	}

	#endregion // BGM

	#region Win/Lose SFX

	private		SoundObject			m_winLoseSFX		= null;

	/// <summary>
	/// Plays a new win/lose SFX.
	/// </summary>
	/// <param name="sfxID">Sfx I.</param>
	private void PlayNewWinLoseSFX(SoundInfo.SFXID sfxID)
	{
		if (m_winLoseSFX != null)
		{
			m_winLoseSFX.Delete();
		}
		m_winLoseSFX = Locator.GetSoundSystem().PlaySound(sfxID);
	}

	/// <summary>
	/// Pauses the win/lose SFX.
	/// </summary>
	private void PauseWinLoseSFX()
	{
		if (m_winLoseSFX != null)
		{
			m_winLoseSFX.Pause();
		}
	}

	/// <summary>
	/// Unpauses the win/lose SFX.
	/// </summary>
	private void UnpauseWinLoseSFX()
	{
		if (m_winLoseSFX != null)
		{
			m_winLoseSFX.Unpause();
		}
	}

	#endregion // Win/Lose SFX
}
