/******************************************************************************
*  @file       CharacterDoorMG.cs
*  @brief      Handles a character instance in the Door Minigame
*  @author     Lori
*  @date       August 18, 2015
*      
*  @par [explanation]
*		> Handles character movement
******************************************************************************/

#region Namespaces

using UnityEngine;

#endregion // Namespaces

public class CharacterDoorMG : InteractiveObject
{
	#region Public Interface

	public delegate void OnFlickDelegate();

	/// <summary>
	/// Initializes this instance
	/// </summary>
	public void Initialize(Vector3 targetPos, float moveSpeed, float moveDistance, float moveDelay, float appearanceTime)
	{
		transform.position = targetPos;
		transform.Translate(Vector3.down * moveDistance);

		m_moveSpeed = moveSpeed;
		m_moveDelay = moveDelay;
		m_appearanceTime = appearanceTime;
		m_moveDuration = moveDistance / m_moveSpeed;
		m_moveTimer = 0f;

		m_state = State.DELAY;
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
	/// Delete this instance.
	/// </summary>
	public override void Delete()
	{
		base.Delete();
		m_state = State.NONE;
	}
	
	#endregion // Public Interface

	#region Serialized Variables

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
		
		UpdateMovement();
	}
	
	#endregion // MonoBehaviour

	#region State

	private enum State
	{
		DELAY,
		UP,
		STAY,
		DOWN,
		NONE
	}

	private			State		m_state				= State.NONE;
	private			bool		m_isPaused			= false;

	#endregion State

	#region Movement

	private			float		m_moveSpeed			= 0f;
	private			float		m_moveTimer			= 0f;
	private			float		m_moveDelay			= 0f;
	private			float		m_moveDuration		= 0f;
	private			float		m_appearanceTime	= 0f;

	/// <summary>
	/// Updates the movement.
	/// </summary>
	private void UpdateMovement()
	{
		if (m_state == State.NONE)
		{
			return;
		}
		
		m_moveTimer += Time.deltaTime;
		
		switch (m_state)
		{
		case State.DELAY:
			if (m_moveTimer >= m_moveDelay)
			{
				m_moveTimer = 0f;
				m_state = State.UP;
				Locator.GetSoundSystem().PlayOneShot(SoundInfo.SFXID.DOOR_PEEK);
			}
			break;
		case State.UP:
			transform.Translate(Vector3.up * m_moveSpeed * Time.deltaTime);
			if (m_moveTimer >= m_moveDuration)
			{
				m_moveTimer = 0f;
				m_state = State.STAY;
			}
			break;
		case State.STAY:
			if (m_moveTimer >= m_appearanceTime)
			{
				m_moveTimer = 0f;
				m_state = State.DOWN;
				Locator.GetSoundSystem().PlayOneShot(SoundInfo.SFXID.DOOR_PEEK);
			}
			break;
		case State.DOWN:
			transform.Translate(Vector3.down * m_moveSpeed * Time.deltaTime);
			if (m_moveTimer >= m_moveDuration)
			{
				m_moveTimer = 0f;
				m_state = State.NONE;
			}
			break;
		}
	}
	
	#endregion // Movement
}
