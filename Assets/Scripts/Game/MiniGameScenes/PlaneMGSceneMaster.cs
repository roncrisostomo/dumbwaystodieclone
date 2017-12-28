/******************************************************************************
*  @file       PlaneMGSceneMaster.cs
*  @brief      Handles the Plane MiniGame scene
*  @author     Lori
*  @date       August 18, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript;

#endregion // Namespaces

public class PlaneMGSceneMaster : MiniGameSceneMasterBase
{
	#region Public Interface

	#endregion // Public Interface

	#region Serialized Variables

	[Header("Input")]
	[SerializeField] private	MicrophoneInput		m_micInput				= null;
	[SerializeField] private	float				m_minLoudness			= 20.0f;
	[SerializeField] private	float				m_maxLoudness			= 60.0f;
	[SerializeField] private	float				m_inputDelayTime		= 1.0f;
	[Header("Plane")]
	[SerializeField] private	GameObject			m_plane					= null;
	[SerializeField] private	GameObject			m_planeShadow			= null;
	[SerializeField] private	GameObject			m_planeTip				= null;
	[SerializeField] private	float[]				m_fallAccelPerLevel		= null;
	[SerializeField] private	float				m_riseSpeedMin			= 3.0f;
	[SerializeField] private	float				m_riseSpeedMax			= 8.0f;
	[SerializeField] private	float				m_riseAngle				= 15.0f;
	[SerializeField] private	float				m_fallAngle				= -15.0f;
	[SerializeField] private	float				m_riseHeightMax			= 2.0f;
	[SerializeField] private	float				m_fallHeightMin			= -2f;
	[Header("Animation")]
	[SerializeField] private	Animator			m_characterAnim			= null;
	[SerializeField] private	Cloud[]				m_clouds				= null;
	[SerializeField] private	float				m_planeCrashRotateDur	= 0.2f;
	[SerializeField] private	float				m_planeWinSpeed			= 10.0f;
	[SerializeField] private	float				m_planeRotSpeed			= 360.0f;
	[SerializeField] private	float				m_characterFlingSpeed	= 10.0f;
	[SerializeField] private	float				m_cloudSpeedFactor		= 0.25f;

	#endregion // Serialized Variables

	#region Resource Loading

	/// <summary>
	/// Loads the resources.
	/// </summary>
	protected override bool LoadResources()
	{
		m_micInput.StartMicInput();
		return true;
	}

	/// <summary>
	/// Unloads the resources.
	/// </summary>
	protected override bool UnloadResources()
	{
		return true;
	}

	#endregion // Resource Loading

	#region Level and Game Timer

	/// <summary>
	/// Raises the time run out event.
	/// </summary>
	protected override void OnTimeRunOut()
	{
		StopGame(true);
	}

	#endregion // Level and Game Timer

	#region Gameplay
	
	/// <summary>
	/// Starts the game.
	/// </summary>
	protected override void StartGame()
	{
		// Initialize the acceleration
		int level = Mathf.Clamp((int)m_level, 0, m_fallAccelPerLevel.Length - 1);
		m_fallAcceleration = m_fallAccelPerLevel[level];

		// Disable the default animation
		m_characterAnim.speed = 0f;

		// Add animators to list
		AddToAnimatorList(m_characterAnim);
		if (m_clouds != null)
		{
			foreach (Cloud cloud in m_clouds)
			{
				AddToInteractiveObjectList(cloud);
			}
		}

		// Play the plane sound
		m_planeSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.PLANE_FLY);
		AddToSoundObjectList(m_planeSound);
	}

	/// <summary>
	/// Updates the game.
	/// </summary>
	protected override void UpdateGame()
	{
		UpdateInput();
		if (m_isInputReady)
		{
			UpdatePlaneSpeed();
			UpdatePlaneRotation();
			UpdatePlanePosition();
		}

		if (m_plane.transform.position.y <= m_fallHeightMin)
		{
			StopGame(false);
		}
	}

	/// <summary>
	/// Raises the stop game event.
	/// </summary>
	protected override void OnStopGame()
	{
		m_micInput.StopMicInput();
	}

	#region Plane

	#region Microphone Input
	
	private			float			m_currentLoudness	= 0f;
	private			float			m_inputDelayTimer	= 0f;
	private			bool			m_isInputReady		= false;

	/// <summary>
	/// Updates the input.
	/// </summary>
	private void UpdateInput()
	{
		if (m_isInputReady)
		{
			m_currentLoudness = m_micInput.GetLoudness();
		}
		else
		{
			m_inputDelayTimer += Time.deltaTime;
			if (m_inputDelayTimer >= m_inputDelayTime)
			{
				m_isInputReady = true;
			}
		}
	}
	
	#endregion // Microphone Input

	#endregion // Plane

	private		float		m_planeSpeed		= 0f;
	private		float		m_fallAcceleration	= -3.0f;
	private		SoundObject	m_planeSound		= null;

	/// <summary>
	/// Updates the plane speed.
	/// </summary>
	private void UpdatePlaneSpeed()
	{
		if (!m_isInputReady)
		{
			return;
		}

		if (m_currentLoudness >= + m_minLoudness)
		{
			float perc = (m_currentLoudness - m_minLoudness) / (m_maxLoudness - m_minLoudness);
			m_planeSpeed = Mathf.Lerp(m_riseSpeedMin, m_riseSpeedMax, perc);
		}
		else
		{
			if (m_planeSpeed > 0f)
			{
				m_planeSpeed = 0f;
			}
			m_planeSpeed += m_fallAcceleration * Time.deltaTime;
		}
	}

	/// <summary>
	/// Updates the plane rotation.
	/// </summary>
	private void UpdatePlaneRotation()
	{
		if (m_inputDelayTimer < m_inputDelayTime)
		{
			return;
		}

		float perc = (m_planeSpeed + m_riseSpeedMax) / (m_riseSpeedMax * 2);
		float zRot = Mathf.Lerp (m_fallAngle, m_riseAngle, perc);
		m_plane.transform.SetRotZ(zRot);
	}

	/// <summary>
	/// Updates the plane position.
	/// </summary>
	private void UpdatePlanePosition()
	{
		if (m_inputDelayTimer < m_inputDelayTime)
		{
			return;
		}

		m_plane.transform.Translate(Vector3.up * m_planeSpeed * Time.deltaTime, Space.World);
		m_plane.transform.SetPosY(Mathf.Clamp(m_plane.transform.position.y, m_fallHeightMin, m_riseHeightMax));
	}
	
	#endregion // Gameplay

	#region Ending Animation

	private		float		m_planeCrashRotateTimer		= 0f;

	/// <summary>
	/// Starts the win animation.
	/// </summary>
	protected override void StartWinAnimation()
	{
		m_plane.transform.SetRotZ(m_riseAngle);
		m_planeShadow.SetActive(false);
		m_characterAnim.Play("Character_Green_Yeah");
	}

	/// <summary>
	/// Updates the win animation.
	/// </summary>
	protected override void UpdateWinAnimation()
	{
		m_plane.transform.Translate(Vector3.right * m_planeWinSpeed * Time.deltaTime, Space.Self);

		if (m_endingAnimationTimer >= m_endingAnimationDuration)
		{
			RemoveFromSoundObjectList(m_planeSound);
			m_planeSound.Delete();
			m_planeSound = null;
		}
		Vector3 cloudMoveVector = m_plane.transform.TransformDirection(Vector3.left);
		cloudMoveVector *= m_cloudSpeedFactor * Time.deltaTime;
		foreach(Cloud cloud in m_clouds)
		{
			cloud.transform.Translate(cloudMoveVector * cloud.MoveSpeed);
		}
	}
	
	/// <summary>
	/// Starts the lose animation.
	/// </summary>
	protected override void StartLoseAnimation()
	{
		m_characterAnim.Play("Character_Green_Sad");
		m_planeCrashRotateTimer = 0f;

		AddToSoundObjectList(Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.PLANE_CRASH));
		AddToSoundObjectList(Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.PLANE_FLING));
		RemoveFromSoundObjectList(m_planeSound);
		m_planeSound.Delete();
		m_planeSound = null;
	}

	/// <summary>
	/// Updates the lose animation.
	/// </summary>
	protected override void UpdateLoseAnimation()
	{
		if (m_planeCrashRotateTimer < m_planeCrashRotateDur)
		{
			m_planeCrashRotateTimer += Time.deltaTime;
			m_plane.transform.RotateAround(m_planeTip.transform.position, Vector3.back, m_planeRotSpeed * Time.deltaTime);
		}
		m_plane.transform.Translate(Vector3.right * 2f * Time.deltaTime, Space.World);
		m_planeShadow.transform.SetPosX(m_plane.transform.position.x);
		m_planeShadow.transform.SetScaleX(Mathf.Lerp(4.0f, 3.0f, m_endingAnimationTimer/m_endingAnimationDuration));
		m_characterAnim.transform.Translate(Vector3.up * m_characterFlingSpeed * Time.deltaTime, Space.Self);
	}

	#endregion // Ending Animation
}
