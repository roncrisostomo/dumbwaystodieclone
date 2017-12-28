/******************************************************************************
*  @file       Cloud.cs
*  @brief      Handles a cloud instance in the Plane minigame
*  @author     Lori 
*  @date       August 7, 2015
*      
*  @par [explanation]
*		> Moves the cloud
******************************************************************************/

#region Namespaces

using UnityEngine;

#endregion // Namespaces

public class Cloud : InteractiveObject
{
	#region Public Interface

	/// <summary>
	/// Deletes this instance
	/// </summary>
	public override void Delete()
	{
		base.Delete();
		m_isMoving = false;
	}

	/// <summary>
	/// Notifies the pause.
	/// </summary>
	public override void NotifyPause()
	{
		base.NotifyPause();
		m_isPaused = true;
	}
	
	/// <summary>
	/// Notifies the unpause.
	/// </summary>
	public override void NotifyUnpause()
	{
		base.NotifyUnpause();
		m_isPaused = false;
	}

	/// <summary>
	/// Gets the move speed.
	/// </summary>
	/// <value>The move speed.</value>
	public float MoveSpeed
	{
		get { return m_moveSpeedLeft; }
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private	float		m_moveSpeedLeft		= 0f;
	[SerializeField] private	float		m_minXPos			= 0f;
	[SerializeField] private	float		m_maxXPos			= 0f;

	#endregion // Serialized Variables

	#region MonoBehaviour
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	protected override void Update ()
	{
		if (m_isPaused)
		{
			return;
		}

		base.Update();

		if (m_isMoving)
		{
			transform.Translate(Vector3.left * m_moveSpeedLeft * Time.deltaTime);
			if (transform.position.x <= m_minXPos)
			{
				transform.SetPosX(m_maxXPos);
			}
		}
	}

	#endregion // MonoBehaviour

	#region State

	private	bool		m_isPaused			= false;
	private	bool		m_isMoving			= true;

	#endregion // State
}
