/******************************************************************************
*  @file       BloodDrop.cs
*  @brief      Handles a blood drop instance in the Blood MiniGame
*  @author     Ron
*  @date       July 31, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class BloodDrop : MonoBehaviour
{
	#region Public Interface

	/// <summary>
	/// Initializes this instance.
	/// </summary>
	/// <param name="bulletHole">Bullet hole handling this instance.</param>
	/// <param name="gravity">Gravity.</param>
	/// <param name="initialVelocity">Initial velocity.</param>
	/// <param name="deleteY">Vertical level past below which this instance would destroy itself.</param>
	public void Initialize(BulletHole bulletHole, float gravity, Vector3 initialVelocity, float deleteY)
	{
		m_bulletHole = bulletHole;
		m_gravity = gravity;
		m_velocity = initialVelocity;
		m_deleteY = deleteY;

		UpdateRotation();
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
	/// Deletes this instance.
	/// </summary>
	/// <param name="deletedByMaster">Set to true if method is called from BulletHole.</param>
	public void Delete(bool deletedByBulletHole = true)
	{
		if (!deletedByBulletHole)
		{
			m_bulletHole.NotifyBloodDropDeletion(this);
		}
		GameObject.Destroy(this.gameObject);
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private Transform m_masterTransform = null;

	#endregion // Serialized Variables

	#region Variables

	private	BulletHole 	m_bulletHole 	= null;
	
	private bool 		m_isPaused 		= false;

	#endregion // Variables

	#region Movement

	private float 		m_gravity 		= 0.0f;
	private Vector3 	m_velocity		= Vector3.zero;
	private float		m_deleteY		= 0.0f;

	/// <summary>
	/// Updates the blood drop movement.
	/// </summary>
	private void UpdateMovement()
	{
		// Move
		this.transform.Translate(m_velocity * Time.deltaTime);
		// Update vertical velocity
		m_velocity.y -= m_gravity * Time.deltaTime;
	}

	/// <summary>
	/// Updates the blood drop rotation to align with the movement vector.
	/// </summary>
	private void UpdateRotation()
	{
		float angle = Mathf.Acos(Vector2.Dot(Vector2.right, m_velocity.normalized)) * Mathf.Rad2Deg;
		if (m_velocity.y < 0.0f) angle = -angle;
		m_masterTransform.SetRotZ(angle + 90.0f);
	}

	/// <summary>
	/// Checks if this blood drop should be deleted.
	/// </summary>
	private void CheckDelete()
	{
		if (this.transform.position.y < m_deleteY)
		{
			Delete(false);
		}
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
		if (m_isPaused)
		{
			return;
		}

		UpdateMovement();
		UpdateRotation();
		CheckDelete();
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	private void OnDestroy()
	{

	}

	#endregion // MonoBehaviour
}
