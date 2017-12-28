/******************************************************************************
*  @file       SpaceRock.cs
*  @brief      Handles behavior of a space rock in the Space MiniGame
*  @author     Ron
*  @date       August 17, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class SpaceRock : MonoBehaviour
{
	#region Public Interface
	
	/// <summary>
	/// Initializes this instance.
	/// </summary>
	public void Initialize()
	{
		// Get a random initial move direction
		RandomizeMoveDir();

		m_isInitialized = true;
	}
	
	/// <summary>
	/// Collides this instance.
	/// </summary>
	/// <param name="collisionVec">Normal direction of collision.</param>
	public void Collide(Vector3 collisionVec)
	{
		m_moveVec = collisionVec * m_speed;

		// Play bump sound
		Locator.GetSoundSystem().PlayOneShot(SoundInfo.SFXID.SPACE_BUMP);
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
	/// Gets the size of the collider.
	/// </summary>
	public float GetColliderSize()
	{
		return this.GetComponent<Collider2D>().bounds.extents.x;
	}

	#endregion // Public Interface
	
	#region Serialized Variables
	
	[SerializeField] private float m_speed = 0.3f;
	// Distance from screen edge that defines the region this object is constrained within
	[SerializeField] private float m_offsetFromScreenEdge = 1.5f;

	#endregion // Serialized Variables
	
	#region Variables
	
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
		// Clamp horizontal position
		if (thisPos.x < min.x)		thisPos.x = min.x;
		else if (thisPos.x > max.x)	thisPos.x = max.x;
		// Clamp vertical position
		if (thisPos.y < min.y)		thisPos.y = min.y;
		else if (thisPos.y > max.y)	thisPos.y = max.y;
		// Apply clamped position
		this.transform.position = thisPos;
	}

	/// <summary>
	/// Randomizes the move direction.
	/// </summary>
	private void RandomizeMoveDir()
	{
		m_moveVec.x = Random.Range(-1.0f, 1.0f);
		m_moveVec.y = Random.Range(-1.0f, 1.0f);
		m_moveVec = m_moveVec.normalized * m_speed;
	}

	#endregion // Movement
	
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
		KeepWithinScreen();
	}
	
	/// <summary>
	/// Raises the trigger enter 2D event.
	/// </summary>
	private void OnCollisionEnter2D(Collision2D collision)
	{
		Collider2D col = collision.collider;

		// Check collision with other space rocks
		SpaceRock spaceRock = col.GetComponent<SpaceRock>();
		if (spaceRock != null)
		{
			// Collide with rock
			Vector3 collisionVec = Vector3.Normalize(col.transform.position - this.transform.position);
			spaceRock.Collide(collisionVec);

			// Make this rock move the opposite direction
			m_moveVec = -collisionVec * m_speed;
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
