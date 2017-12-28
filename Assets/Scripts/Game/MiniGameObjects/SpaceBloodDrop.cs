/******************************************************************************
*  @file       SpaceBloodDrop.cs
*  @brief      Handles a blood drop instance in the Space MiniGame
*  @author     Ron
*  @date       August 9, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class SpaceBloodDrop : MonoBehaviour
{
	#region Public Interface

	/// <summary>
	/// Initializes this instance.
	/// </summary>
	public void Initialize(Vector3 moveDir)
	{
		m_moveDir = moveDir;

		UpdateRotation();

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
	/// Deletes this instance.
	/// </summary>
	public void Delete()
	{
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private float		m_speed				= 10.0f;

	[SerializeField] private Transform 	m_masterTransform 	= null;

	#endregion // Serialized Variables

	#region Variables

	private bool 	m_isInitialized = false;

	private bool 	m_isPaused 		= false;

	#endregion // Variables

	#region Movement

	private Vector3 m_moveDir		= Vector3.zero;

	/// <summary>
	/// Updates the blood drop movement.
	/// </summary>
	private void UpdateMovement()
	{
		this.transform.Translate(m_moveDir * m_speed * Time.deltaTime);
	}

	/// <summary>
	/// Updates the blood drop rotation to align with the movement vector.
	/// </summary>
	private void UpdateRotation()
	{
		float angle = Mathf.Acos(Vector2.Dot(Vector2.right, m_moveDir.normalized)) * Mathf.Rad2Deg;
		if (m_moveDir.y < 0.0f) angle = -angle;
		m_masterTransform.SetRotZ(angle + 90.0f);
	}

	/// <summary>
	/// Checks if this blood drop should be deleted.
	/// </summary>
	private void CheckDelete()
	{
		// TODO: Is this needed?
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
