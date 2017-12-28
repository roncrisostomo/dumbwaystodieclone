/******************************************************************************
*  @file       SoundSystem.cs
*  @brief      Handles all sounds in the game
*  @author     Ron
*  @date       August 20, 2015
*      
*  @par [explanation]
*		> Initializes sound resources
*		> Manages the life cycle of all SoundObjects
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

#endregion // Namespaces

public class SoundSystem : SoundSystemBase
{
	#region Public Interface

	/// <summary>
	/// Initialize the sound system.
	/// </summary>
	public override bool Initialize()
	{
		if (LoadSoundResources())
		{
			m_isInitialized = true;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Plays the specified sound once, and deletes the sound object after playback.
	/// </summary>
	public override void PlayOneShot(SoundInfo.SFXID soundID)
	{
		SoundObject oneShotSFX = CreateSoundObject(soundID);
		oneShotSFX.Initialize(this, SoundInfo.SoundType.ONE_SHOT, !m_areSoundsOn);
		oneShotSFX.Play();
	}

	/// <summary>
	/// Plays the specified sound.
	/// </summary>
	public override SoundObject PlaySound(SoundInfo.SFXID soundID)
	{
		SoundObject sound = CreateSoundObject(soundID);
		sound.Initialize(this, SoundInfo.SoundType.REGULAR, !m_areSoundsOn);
		sound.Play();
		return sound;
	}

	/// <summary>
	/// Plays the specified BGM.
	/// </summary>
	public override SoundObject PlayBGM(SoundInfo.BGMID soundID, bool setPersistent = false)
	{
		SoundObject bgm = CreateSoundObject(soundID, setPersistent);
		bgm.Initialize(this, SoundInfo.SoundType.BGM, !m_areSoundsOn);
		bgm.Play();
		return bgm;
	}

	/// <summary>
	/// Pauses all sounds.
	/// </summary>
	public override void PauseAllSounds(bool includeBGM = true)
	{
		foreach (SoundObject soundObject in m_soundObjectList)
		{
			// If includeBGM is false, pause only sounds that are not BGM
			if (includeBGM || (!includeBGM && soundObject.GetSoundType != SoundInfo.SoundType.BGM))
			{
				soundObject.Pause(true);
			}
		}
	}

	/// <summary>
	/// Unpauses all sounds.
	/// </summary>
	public override void UnpauseAllSounds()
	{
		foreach (SoundObject soundObject in m_soundObjectList)
		{
			// Do not unpause sounds that were already paused before PauseAllSounds was called
			if (!soundObject.IsPaused)
			{
				soundObject.Unpause(true);
			}
		}
	}

	/// <summary>
	/// Mutes all sounds.
	/// </summary>
	public override void MuteAllSounds(bool includeBGM = true)
	{
		foreach (SoundObject soundObject in m_soundObjectList)
		{
			// If includeBGM is false, mute only sounds that are not BGM
			if (includeBGM || (!includeBGM && soundObject.GetSoundType != SoundInfo.SoundType.BGM))
			{
				soundObject.Mute(true);
			}
		}
		m_areSoundsOn = false;
	}
	
	/// <summary>
	/// Unmutes all sounds.
	/// </summary>
	public override void UnmuteAllSounds()
	{
		foreach (SoundObject soundObject in m_soundObjectList)
		{
			// Do not unmute sounds that were already muted before MuteAllSounds was called
			if (!soundObject.IsMuted)
			{
				soundObject.Unmute(true);
			}
		}
		m_areSoundsOn = true;
	}

	/// <summary>
	/// Notifies of a sound object's deletion via means external to SoundSystem.
	/// </summary>
	/// <param name="soundObject">Sound object to delete.</param>
	public override void NotifySoundObjectDeletion(SoundObject soundObject)
	{
		if (m_soundObjectList.Contains(soundObject))
		{
			m_soundObjectList.Remove(soundObject);
		}
	}

	/// <summary>
	/// Deletes all sound objects.
	/// </summary>
	/// <param name="includePersistent">If set to <c>true</c> delete persistent sounds as well.</param>
	public override void DeleteAllSoundObjects(bool includePersistent = true)
	{
		foreach (SoundObject soundObject in m_soundObjectList.ToList())
		{
			if (!includePersistent && soundObject.IsPersistent)
			{
				continue;
			}
			soundObject.Delete(true);
			m_soundObjectList.Remove(soundObject);
		}
	}

	#endregion // Public Interface

	#region Serialized Variables

	#endregion // Serialized Variables

	#region Sound Objects

	private List<SoundObject> m_soundObjectList	= new List<SoundObject>();

	/// <summary>
	/// Creates a SoundObject for the specified SFX.
	/// </summary>
	/// <returns>The sound object.</returns>
	/// <param name="sfxID">SFX ID.</param>
	private SoundObject CreateSoundObject(SoundInfo.SFXID sfxID, bool setPersistent = false)
	{
		return CreateSoundObject(m_sfxResources[(int)sfxID], setPersistent);
	}

	/// <summary>
	/// Creates a SoundObject for the specified BGM.
	/// </summary>
	/// <returns>The sound object.</returns>
	/// <param name="sfxID">BGM ID.</param>
	private SoundObject CreateSoundObject(SoundInfo.BGMID bgmID, bool setPersistent = false)
	{
		return CreateSoundObject(m_bgmResources[(int)bgmID], setPersistent);
	}

	/// <summary>
	/// Creates a SoundObject for the specified sound resource.
	/// </summary>
	/// <returns>The sound object.</returns>
	/// <param name="sfxID">Sound resource.</param>
	private SoundObject CreateSoundObject(GameObject soundResource, bool setPersistent = false)
	{
		GameObject obj = GameObject.Instantiate(soundResource) as GameObject;
		SoundObject soundObject = obj.AddComponentNoDupe<SoundObject>();
		if (setPersistent)
		{
			DontDestroyOnLoad(obj);
			soundObject.SetPersistent();
		}
		m_soundObjectList.Add(soundObject);
		return soundObject;
	}

	#endregion // Sound Objects

	#region Sound Resources

	private SoundInfo m_soundInfo = new SoundInfo();

	private GameObject[] m_sfxResources = new GameObject[(int)SoundInfo.SFXID.SIZE];
	private GameObject[] m_bgmResources = new GameObject[(int)SoundInfo.BGMID.SIZE];

	/// <summary>
	/// Loads SFX and BGM sound resources.
	/// </summary>
	private bool LoadSoundResources()
	{
		// Load sound effects
		for (int sfxIndex = 0; sfxIndex < m_sfxResources.Length; ++sfxIndex)
		{
			string sfxPrefabPath = m_soundInfo.GetSoundPrefabPath((SoundInfo.SFXID)sfxIndex);
			if (!LoadSoundResource(sfxPrefabPath, m_sfxResources, sfxIndex))
			{
				return false;
			}
		}
		// Load background music
		for (int bgmIndex = 0; bgmIndex < m_bgmResources.Length; ++bgmIndex)
		{
			string bgmPrefabPath = m_soundInfo.GetSoundPrefabPath((SoundInfo.BGMID)bgmIndex);
			if (!LoadSoundResource(bgmPrefabPath, m_bgmResources, bgmIndex))
			{
				return false;
			}
		}
		// All sound resources loaded successfully
		return true;
	}

	/// <summary>
	/// Loads a sound resource.
	/// </summary>
	/// <returns><c>true</c>, if sound resource was loaded, <c>false</c> otherwise.</returns>
	/// <param name="soundPrefabPath">Sound prefab path.</param>
	/// <param name="soundArray">Array to store sound resource in, if valid.</param>
	/// <param name="soundIndex">Index of sound in array, if valid.</param>
	private bool LoadSoundResource(string soundPrefabPath, GameObject[] soundArray, int soundIndex)
	{
		// Load resource from prefab path
		GameObject soundResource = Resources.Load(soundPrefabPath, typeof(GameObject)) as GameObject;
		// Prefab should exist
		if (soundResource == null)
		{
			if (BuildInfo.IsDebugMode)
			{
				Debug.LogWarning("Invalid prefab path (" + soundPrefabPath + "): " +
				                 "No sound prefab found in specified path");
			}
			return false;
		}
		// Sound resource should have an AudioSource component
		AudioSource audioSource = soundResource.GetComponent<AudioSource>();
		if (audioSource == null)
		{
			if (BuildInfo.IsDebugMode)
			{
				Debug.LogWarning("Invalid sound prefab (" + soundPrefabPath + "): " +
				                 "Sound prefab must have an AudioSource component");
			}
			return false;
		}
		// AudioSource should have an audio clip
		AudioClip audioClip = audioSource.clip;
		if (audioClip == null)
		{
			if (BuildInfo.IsDebugMode)
			{
				Debug.LogWarning("Invalid sound prefab (" + soundPrefabPath + "): " +
				                 "AudioSource must have an audio clip assigned");
			}
			return false;
		}
		// Store resource in array of sound resources
		soundArray[soundIndex] = soundResource;
		// Sound resource loaded successfully
		return true;
	}

	#endregion // Sound Resources

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
		
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	private void OnDestroy()
	{

	}

	#endregion // MonoBehaviour
}
