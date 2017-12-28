/******************************************************************************
*  @file       Balloon.cs
*  @brief      Handles a balloon in the Balloon MiniGame
*  @author     Ron
*  @date       August 18, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;
using TouchScript;
using TouchScript.Gestures;

#endregion // Namespaces

public class Balloon : InteractiveObject
{
	#region Public Interface

	/// <summary>
	/// Initializes this balloon.
	/// </summary>
	/// <param name="collectedPos">Position of this balloon when successfully collected (connected to hand).</param>
	/// <param name="balloonGrabPos">Position where this balloon would be connected to the hand.</param>
	public void Initialize(Vector3 collectedPos, Vector3 balloonGrabPos, float colorFactor, int sortingOrder)
	{
		m_collectedPos = collectedPos;
		m_balloonGrabPos = balloonGrabPos;
		m_balloonRenderer.color = new Color(colorFactor, colorFactor, colorFactor, 1.0f);
		m_balloonRenderer.sortingOrder = sortingOrder;
		m_stringRenderer.sortingOrder = sortingOrder + 1;

		RandomizeMoveDir();
		ResetString();

		// Subscribe to input events
		AddPressDelegate(OnBalloonPress);
		this.GetComponent<ReleaseGesture>().Released += OnBalloonRelease;
	}

	/// <summary>
	/// Initializes this balloon for the end animation.
	/// </summary>
	public void InitializeForEndAnim(bool isCollected, bool isGameLost)
	{
		m_isCollected = isCollected;
		m_isGameLost = isGameLost;
		m_isEndAnim = true;
		RandomizeEndAnimMoveDir();
		if (!isCollected)
		{
			ResetString();
		}
	}

	/// <summary>
	/// Notifies the pause.
	/// </summary>
	public override void NotifyPause()
	{
		m_isPaused = true;

		// Pause sound
		if (m_connectSound != null)
		{
			m_connectSound.Pause();
		}

		// Disable collider
		m_isColliderEnabledBeforePause = this.GetComponent<Collider2D>().enabled;
		this.GetComponent<Collider2D>().enabled = false;
	}

	/// <summary>
	/// Notifies the unpause.
	/// </summary>
	public override void NotifyUnpause()
	{
		m_isPaused = false;

		// Unpause sound
		if (m_connectSound != null)
		{
			m_connectSound.Unpause();
		}
		// If balloon is no longer strung in-game on unpause, hide the string
		// If in end animation, no need to reset string
		if (!m_isStrung && !m_isEndAnim)
		{
			ResetString();
		}

		// Re-enable collider if it was enabled before pause
		this.GetComponent<Collider2D>().enabled = m_isColliderEnabledBeforePause;
	}
	
	/// <summary>
	/// Notifies the stop game.
	/// </summary>
	public void NotifyStopGame()
	{
		// Behaviour on game end is similar to that when paused
		NotifyPause();
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private 	SpriteRenderer 	m_balloonRenderer	= null;
	[SerializeField] private 	SpriteRenderer 	m_stringRenderer	= null;
	[SerializeField] private 	Transform 		m_balloonString 	= null;
	[SerializeField] private 	Transform 		m_stringSourcePos 	= null;
	// Speed when balloon is floating unconnected to hand
	[SerializeField] private 	float 			m_idleSpeed 		= 0.2f;
	// Speed at which balloon moves toward "collected" position
	[SerializeField] private 	float 			m_collectSpeed		= 0.3f;
	// Speed of uncollected balloons in end animation
	[SerializeField] private	float			m_endAnimLostSpeed	= 0.35f;

	#endregion // Serialized Variables

	#region Variables

	private bool m_isPaused = false;

	private bool m_isColliderEnabledBeforePause = false;

	#endregion // Variables

	#region Movement

	// Whether string is currently displayed (either connecting balloon to hand, or balloon to touch input)
	private bool 	m_isStrung 			= false;
	private bool	m_isCollected		= false;

	private Vector3 m_moveStartPos		= Vector3.zero;
	private Vector3 m_collectedPos 		= Vector3.zero;
	private Vector3 m_unstrungMoveDir	= Vector3.zero;
	private float	m_moveTime 			= 0.0f;

	/// <summary>
	/// Updates the movement.
	/// </summary>
	private void UpdateMovement()
	{
		// If collected or strung, float toward "collected" position
		if (m_isCollected || m_isStrung)
		{
			m_moveTime += Time.deltaTime;
			this.transform.position = Vector3.Lerp(m_moveStartPos, m_collectedPos, m_moveTime * m_collectSpeed);
		}
		// If not strung, float away from hand
		else
		{
			this.transform.Translate(m_unstrungMoveDir * m_idleSpeed * Time.deltaTime);
		}
	}

	/// <summary>
	/// Gets a random move direction away from the hand.
	/// </summary>
	private void RandomizeMoveDir()
	{
		const float DELTA_ANGLE = 20.0f;
		Vector3 moveDir = new Vector3(1.0f, 1.0f, 0.0f);
		moveDir = Quaternion.Euler(0.0f, 0.0f, Random.Range(-DELTA_ANGLE, DELTA_ANGLE)) * moveDir;
		m_unstrungMoveDir = moveDir;
	}

	#endregion // Movement

	#region String

	private Vector3 m_balloonGrabPos = Vector3.zero;

	private ITouch m_balloonTouch = null;

	/// <summary>
	/// Maintains the string connection between balloon and hand, or balloon and touch.
	/// </summary>
	private void UpdateString()
	{
		if (!m_isStrung)
		{
			return;
		}

		Vector3 stringDestPos = m_balloonGrabPos;
		if (!m_isCollected && m_balloonTouch != null)
		{
			stringDestPos = Camera.main.ScreenToWorldPoint(m_balloonTouch.Position);
			stringDestPos.z = 0.0f;
		}
		// Position string at the center of source and destination
		m_balloonString.position = (m_stringSourcePos.position + stringDestPos) * 0.5f;
		// Scale string to the distance between source and destination
		m_balloonString.SetScaleX(Vector3.Distance(m_stringSourcePos.position, stringDestPos));
		// Rotate string to point from source to destination
		Vector3 sourceToDestDir = Vector3.Normalize(stringDestPos - m_stringSourcePos.position);
		float angle = Mathf.Atan(sourceToDestDir.y / sourceToDestDir.x) * Mathf.Rad2Deg;
		m_balloonString.SetRotZ(angle);
	}

	/// <summary>
	/// Resets the string to hidden state.
	/// </summary>
	private void ResetString()
	{
		m_balloonString.position = m_stringSourcePos.position;
		m_balloonString.eulerAngles = Vector3.zero;
		m_balloonString.SetScaleX(0.0f);
	}

	#endregion // String

	#region Hand
	
	/// <summary>
	/// Checks if balloon touch connects with the hand.
	/// </summary>
	private void CheckConnectWithHand()
	{
		if (m_isCollected || !m_isStrung || m_balloonTouch == null)
		{
			return;
		}
		// Check if touch that selected this balloon connects with the hand
		RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(m_balloonTouch.Position), Vector2.zero);
		foreach (RaycastHit2D hit in hits)
		{
			if (hit.transform == null)
			{
				continue;
			}

			Hand hand = hit.transform.GetComponent<Hand>();
			if (hand == null)
			{
				continue;
			}

			hand.Collect(this);
			m_isCollected = true;

			// Stop connect sound
			m_connectSound.Stop();
			// Play grab sound
			Locator.GetSoundSystem().PlayOneShot(SoundInfo.SFXID.BALLOON_GRAB);

			// Unsubscribe from input events
			RemovePressDelegate(OnBalloonPress);
			this.GetComponent<ReleaseGesture>().Released -= OnBalloonRelease;

			// Disable collider to allow other balloons to be grabbed
			m_collider2D.enabled = false;

			break;
		}
	}

	#endregion // Hand

	#region Input Handling
	
	private SoundObject m_connectSound = null;

	/// <summary>
	/// Raises the balloon press event.
	/// </summary>
	private void OnBalloonPress(object sender, System.EventArgs e)
	{
		// Process only one touch per balloon
		if (m_isStrung)
		{
			return;
		}
		m_isStrung = true;

		// Play connect sound
		if (m_connectSound == null)
		{
			m_connectSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.BALLOON_CONNECT);
		}
		else
		{
			m_connectSound.Play();
		}

		// Initialize move variables
		m_moveStartPos = this.transform.position;

		// Find and store a reference to the touch
		foreach (ITouch touch in m_pressGesture.ActiveTouches)
		{
			if (m_pressGesture.HasTouch(touch))
			{
				m_balloonTouch = touch;
				break;
			}
		}
	}

	/// <summary>
	/// Raises the balloon release event.
	/// </summary>
	private void OnBalloonRelease(object sender, System.EventArgs e)
	{
		m_isStrung = false;

		// Stop connect sound
		m_connectSound.Stop();

		RandomizeMoveDir();
		// When game is paused, just leave the string shown, then hide it when the game is unpaused.
		if (!m_isPaused)
		{
			ResetString();
		}

		// Initialize move variables
		m_moveStartPos = this.transform.position;
		m_moveTime = 0.0f;
	}

	#endregion // Input Handling

	#region End Animation

	private bool 	m_isEndAnim 		= false;
	private bool 	m_isGameLost 		= false;
	private Vector3 m_endAnimMoveDir	= Vector3.zero;

	/// <summary>
	/// Updates movement when in the end animation.
	/// </summary>
	private void UpdateEndAnimMovement()
	{
		this.transform.Translate(m_endAnimMoveDir * m_endAnimLostSpeed * Time.deltaTime);
	}

	/// <summary>
	/// Randomizes movement in the end animation.
	/// </summary>
	private void RandomizeEndAnimMoveDir()
	{
		const float DELTA_ANGLE = 20.0f;
		Vector3 moveDir = Vector3.zero;
		float randomAngle = Random.Range(-DELTA_ANGLE, DELTA_ANGLE);
		// If not collected, float away from the character
		if (!m_isCollected)
		{
			moveDir = Quaternion.Euler(0.0f, 0.0f, randomAngle) * new Vector3(1.0f, 1.0f, 0.0f);
			m_endAnimMoveDir = moveDir;
		}
		else
		{
			// If collected but game is lost, straighten up balloon and string rotation, and float up
			if (m_isGameLost)
			{
				this.transform.SetRotZ(0.0f);
				moveDir = Quaternion.Euler(0.0f, 0.0f, randomAngle) * Vector3.up;
				m_endAnimMoveDir = moveDir;
			}
		}
	}

	#endregion // End Animation

	#region MonoBehaviour

	/// <summary>
	/// Awake this instance.
	/// </summary>
	protected override void Awake()
	{
		base.Awake();
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	protected override void Start()
	{
		base.Start();
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	protected override void Update()
	{
		base.Update();

		if (m_isPaused)
		{
			return;
		}

		// Differentiate behaviour of balloons in-game from balloons in the end animation
		if (!m_isEndAnim)
		{
			UpdateMovement();
			UpdateString();
			CheckConnectWithHand();
		}
		else
		{
			UpdateEndAnimMovement();
		}
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	#endregion // MonoBehaviour
}
