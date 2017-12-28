/******************************************************************************
*  @file       Spaceman.cs
*  @brief      Handles behavior of the spaceman in the Space MiniGame
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

public class Spaceman : MonoBehaviour
{
	#region Public Interface

	/// <summary>
	/// Initializes this instance.
	/// </summary>
	public void Initialize(SpaceMGSceneMaster sceneMaster)
	{
		m_sceneMaster = sceneMaster;

		m_initialRotZ = m_animRoot.eulerAngles.z;

		// Set the initialized flag
		m_isInitialized = true;
	}

	/// <summary>
	/// Pauses this instance.
	/// </summary>
	public void Pause()
	{
		m_isPaused = true;
	}

	/// <summary>
	/// Unpauses this instance.
	/// </summary>
	public void Unpause()
	{
		m_isPaused = false;
	}

	/// <summary>
	/// Gets the spaceman's animation root transform.
	/// </summary>
	public Transform AnimRoot
	{
		get { return m_animRoot; }
	}

	#endregion // Public Interface

	#region Serialized Variables

	// Sensitivity to tilt
	[SerializeField] private float 		m_tiltSpeed 	= 10.0f;
	// Speed after bumping into a space rock
	[SerializeField] private float 		m_bumpSpeed 	= 1.5f;
	// Maximum speed the spaceman can achieve
	[SerializeField] private float 		m_maxSpeed 		= 7.0f;
	// Speed at which spaceman rotates from initial rotation to an upright state,
	//	relative to screen orientation
	[SerializeField] private float 		m_rotSpeed		= 0.25f;
	[SerializeField] private Transform 	m_animRoot 		= null;
	// Distance from screen edge that defines the region this object is constrained within
	[SerializeField] private float 		m_offsetFromScreenEdge = 2.0f;

	#endregion // Serialized Variables

	#region Variables

	private SpaceMGSceneMaster m_sceneMaster = null;

	private bool m_isPaused = false;

	private bool m_isInitialized = false;

	#endregion // Variables

	#region Movement

	private Vector3 m_moveVec = Vector3.zero;

	/// <summary>
	/// Updates movement.
	/// </summary>
	private void UpdateMovement()
	{
		// Apply tilt effect
		Vector3 inputAcceleration = Input.acceleration;
		inputAcceleration.z = 0.0f;
		m_moveVec += inputAcceleration * m_tiltSpeed * Time.deltaTime;
		// Clamp speed
		if (m_moveVec.sqrMagnitude > m_maxSpeed*m_maxSpeed)
		{
			m_moveVec = m_moveVec.normalized * m_maxSpeed;
		}
		this.transform.Translate(m_moveVec * Time.deltaTime);
	}

	/// <summary>
	/// Keeps this object within screen view.
	/// </summary>
	private void KeepWithinScreen()
	{
		Vector3 thisPos = this.transform.position;
		Vector2 min = Locator.GetSceneMaster().UICamera.ScreenMinWorld + Vector2.one * m_offsetFromScreenEdge;
		Vector2 max = Locator.GetSceneMaster().UICamera.ScreenMaxWorld - Vector2.one * m_offsetFromScreenEdge;
		// Clamp position, and reset move velocity component in clamped directions
		// Horizontal position
		if (thisPos.x < min.x)		{ thisPos.x = min.x; m_moveVec.x = 0.0f; }
		else if (thisPos.x > max.x)	{ thisPos.x = max.x; m_moveVec.x = 0.0f; }
		// Vertical position
		if (thisPos.y < min.y)		{ thisPos.y = min.y; m_moveVec.y = 0.0f; }
		else if (thisPos.y > max.y)	{ thisPos.y = max.y; m_moveVec.y = 0.0f; }
		// Apply clamped position
		this.transform.position = thisPos;
	}

	#endregion // Movement

	#region Rotation

	private float m_initialRotZ 		= 0.0f;

	private float m_timeSinceRotStart 	= 0.0f;

	/// <summary>
	/// Updates rotation.
	/// </summary>
	private void UpdateRotation()
	{
		// Rotates the spaceman from initial rotation to an upright state relative to screen orientation
		m_timeSinceRotStart += Time.deltaTime;
		float finalRotZ = m_initialRotZ > 180.0f ? 360.0f : 0.0f;
		m_animRoot.SetRotZ(Mathf.Lerp(m_initialRotZ, finalRotZ, m_timeSinceRotStart * m_rotSpeed));
	}

	#endregion // Rotation

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
		if (m_isPaused || !m_isInitialized)
		{
			return;
		}

		UpdateMovement();
		UpdateRotation();
		KeepWithinScreen();
	}

	/// <summary>
	/// Raises the collision enter 2D event.
	/// </summary>
	private void OnCollisionEnter2D(Collision2D collision)
	{
		Collider2D col = collision.collider;

		// Check if space rock
		SpaceRock spaceRock = col.GetComponent<SpaceRock>();
		if (spaceRock != null)
		{
			// Collide with rock
			Vector3 collisionVec = Vector3.Normalize(col.transform.position - this.transform.position);
			spaceRock.Collide(collisionVec);

			// Make spaceman move in opposite direction
			m_moveVec = -collisionVec * Mathf.Min(m_moveVec.magnitude, m_bumpSpeed);
		}

		// Check if helmet
		Helmet helmet = col.GetComponent<Helmet>();
		if (helmet != null)
		{
			m_sceneMaster.NotifyGetHelmet();
		}
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	private void OnDestroy()
	{

	}

	#endregion // MonoBehaviour
}
