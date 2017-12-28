/******************************************************************************
*  @file       SoundSystemBase.cs
*  @brief      Abstract base class for sound system classes
*  @author     Ron
*  @date       July 27, 2015
*      
*  @par [explanation]
*		> Initializes sound resources
*		> Manages the life cycle of all SoundObjects
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public abstract class SoundSystemBase : MonoBehaviour
{
	#region Public Interface

	public abstract bool Initialize();
	public abstract void PlayOneShot(SoundInfo.SFXID soundID);
	public abstract SoundObject PlaySound(SoundInfo.SFXID soundID);
	public abstract SoundObject PlayBGM(SoundInfo.BGMID soundID, bool setPersistent = false);
	public abstract void PauseAllSounds(bool includeBGM = true);
	public abstract void UnpauseAllSounds();
	public abstract void MuteAllSounds(bool includeBGM = true);
	public abstract void UnmuteAllSounds();
	public abstract void NotifySoundObjectDeletion(SoundObject oneShotSFX);
	public abstract void DeleteAllSoundObjects(bool includePersistent = true);

	public bool IsInitialized
	{
		get { return m_isInitialized; }
	}

	public bool AreSoundsOn
	{
		get { return m_areSoundsOn; }
	}

	#endregion // Public Interface
	
	#region Variables
	
	protected bool m_isInitialized = false;
	protected bool m_areSoundsOn = true;

	#endregion // Variables
}
