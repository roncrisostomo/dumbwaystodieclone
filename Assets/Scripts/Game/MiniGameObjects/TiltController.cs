/******************************************************************************
*  @file       TiltController.cs
*  @brief      Handles tilt control for the character in the Glue MiniGame
*  @author     Ron
*  @date       August 18, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class TiltController : MonoBehaviour
{
	#region Public Interface

	/// <summary>
	/// Initializes this instance.
	/// </summary>
	/// <param name="sceneMaster">Scene master.</param>
	/// <param name="tiltSensitivity">Tilt sensitivity.</param>
	public void Initialize(GlueMGSceneMaster sceneMaster, float tiltSensitivity)
	{
		m_sceneMaster = sceneMaster;
		m_tiltSensitivity = tiltSensitivity;

		// Set the initialized flag
		m_isInitialized = true;
	}

	/// <summary>
	/// Pauses this instance.
	/// </summary>
	public void Pause()
	{
		m_isPaused = true;

		// Pause the tilt sound
		if (m_tiltSound != null)
		{
			m_tiltSound.Pause();
		}
	}

	/// <summary>
	/// Unpauses this instance.
	/// </summary>
	public void Unpause()
	{
		m_isPaused = false;

		// Unpause the tilt sound
		if (m_tiltSound != null)
		{
			m_tiltSound.Unpause();
		}
	}

	/// <summary>
	/// Sets game over.
	/// </summary>
	public void SetGameOver()
	{
		m_isGameOver = true;

		// Stop tilt sound
		if (m_tiltSound != null)
		{
			m_tiltSound.Stop();
		}
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private Transform 	m_tiltAnchor	= null;
	[SerializeField] private Transform 	m_sprite		= null;
	[SerializeField] private Transform 	m_shadow		= null;
	// Delay from game start until device tilt starts to have in-game effect
	[SerializeField] private float		m_startDelay	= 0.5f;
	// Tilt acceleration
	[SerializeField] private float 		m_tiltSpeed		= 10.0f;
	[SerializeField] private float 		m_maxSpeed		= 12.0f;
	// Tilt angle at which player loses control of character tilt
	[SerializeField] private float 		m_toppleAngle 	= 37.0f;
	// Tilt angle when character has fallen to the ground
	[SerializeField] private float 		m_fallAngle		= 47.0f;

	#endregion // Serialized Variables

	#region Variables

	private GlueMGSceneMaster m_sceneMaster = null;

	private bool 	m_isInitialized = false;
	private bool 	m_isPaused 		= false;
	private bool 	m_isGameOver 	= false;

	private float 	m_timeSinceGameStart = 0.0f;

	#endregion // Variables

	#region CharacterState

	private enum CharacterState
	{
		NONE,
		TILTING,
		TOPPLE,
		FALLEN
	}
	private CharacterState m_state = CharacterState.NONE;

	/// <summary>
	/// Updates the character state.
	/// </summary>
	private void UpdateCharacterState()
	{
		switch (m_state)
		{
		case CharacterState.NONE:
			// Delay until tilt actually affects the game
			m_timeSinceGameStart += Time.deltaTime;
			if (m_timeSinceGameStart > m_startDelay)
			{
				m_tiltSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.GLUE_TILT);

				m_state = CharacterState.TILTING;
			}
			break;
		case CharacterState.TILTING:
			// Check accelerometer input
			UpdateAngularVelocity();
			// Apply angular velocity to character tilt
			UpdateTilt();
			// Check if character topples
			float rotZ = m_tiltAnchor.eulerAngles.z;
			if ((rotZ < 180.0f && rotZ > m_toppleAngle) ||
			    (rotZ > 180.0f && rotZ < 360.0f - m_toppleAngle))
			{
				// Make character continue falling
				m_angularVelZ = Mathf.Sign(m_angularVelZ) * m_maxSpeed;

				m_state = CharacterState.TOPPLE;
			}
			break;
		case CharacterState.TOPPLE:
			// If character topples, player can no longer control character tilt
			UpdateTilt();
			// Check if character falls to the floor
			float rotZ2 = m_tiltAnchor.eulerAngles.z;
			if ((rotZ2 < 180.0f && rotZ2 > m_fallAngle) ||
			    (rotZ2 > 180.0f && rotZ2 < 360.0f - m_fallAngle))
			{
				// Clamp tilt to the fall angle
				float fallRot = (rotZ2 < 180.0f) ? m_fallAngle : 360.0f - m_fallAngle;
				m_tiltAnchor.SetRotZ(fallRot);

				// Stop tilt sound
				m_tiltSound.Stop();

				// Play fall sound
				Locator.GetSoundSystem().PlayOneShot(SoundInfo.SFXID.GLUE_FALL);

				// Notify scene master
				m_sceneMaster.NotifyCharacterFell();

				m_state = CharacterState.FALLEN;
			}
			break;
		case CharacterState.FALLEN:
			// Character has fallen to the ground
			break;
		}
	}

	#endregion // CharacterState

	#region Tilt

	// Effect of device acceleration on the tilt speed
	private float m_tiltSensitivity = 0.0f;

	private float m_angularVelZ = 0.0f;

	private SoundObject m_tiltSound = null;

	/// <summary>
	/// Updates angular velocity using accelerometer input.
	/// </summary>
	private void UpdateAngularVelocity()
	{
		float prevDir = Mathf.Sign(m_angularVelZ);

		m_angularVelZ += Input.acceleration.x * m_tiltSensitivity * Time.deltaTime;

		// If rotation changes direction, re-play tilt sound
		if (prevDir != Mathf.Sign(m_angularVelZ))
		{
			m_tiltSound.Play();
		}

		// Clamp speed
		if (Mathf.Abs(m_angularVelZ) > m_maxSpeed)
		{
			m_angularVelZ = Mathf.Sign(m_angularVelZ) * m_maxSpeed;
		}

		// Tilt sound volume is proportional to angular velocity
		m_tiltSound.SetVolume(Mathf.Abs(m_angularVelZ) / m_maxSpeed);
	}

	/// <summary>
	/// Updates character tilt using angular velocity.
	/// </summary>
	private void UpdateTilt()
	{
		float deltaRotZ = m_angularVelZ * m_tiltSpeed * Time.deltaTime;
		m_tiltAnchor.SetRotZ(m_tiltAnchor.eulerAngles.z - deltaRotZ);
		// Make shadow follow character
		m_shadow.SetPosX(m_sprite.position.x);
	}

	#endregion // Tilt

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
		if (m_isPaused || !m_isInitialized || m_isGameOver)
		{
			return;
		}

		UpdateCharacterState();
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	private void OnDestroy()
	{

	}

	#endregion // MonoBehaviour
}
