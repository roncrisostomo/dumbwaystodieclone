/******************************************************************************
*  @file       Piranha.cs
*  @brief      Handles a piranha instance in the Piranha minigame
*  @author     Lori 
*  @date       July 25, 2015
*      
*  @par [explanation]
*		> Moves the piranha
*		> Detects input from player
******************************************************************************/

#region Namespaces

using UnityEngine;

#endregion // Namespaces

public class Piranha : InteractiveObject
{
	#region Public Interface
	
	public delegate void OnFlickDelegate();
	public delegate void OnBiteDelegate();

	/// <summary>
	/// Initializes this instance
	/// </summary>
	/// <param name="onTap">On tap.</param>
	public void Initialize(OnFlickDelegate onFLick, OnBiteDelegate onBite)
	{
		m_onFlick = onFLick;
		m_onBite = onBite;
		StartMovement();
		m_stateSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.FISH_SWIM);
	}

	/// <summary>
	/// Deletes this instance
	/// </summary>
	public override void Delete()
	{
		base.Delete();
		if (m_state != State.FLICKED)
		{
			m_state = State.DISABLED;
		}
	}

	/// <summary>
	/// Notifies the pause.
	/// </summary>
	public override void NotifyPause()
	{
		base.NotifyPause();
		m_isPaused = true;
		if (m_stateSound != null)
		{
			m_stateSound.Pause();
		}
	}
	
	/// <summary>
	/// Notifies the unpause.
	/// </summary>
	public override void NotifyUnpause()
	{
		base.NotifyUnpause();
		m_isPaused = false;
		if (m_stateSound != null)
		{
			m_stateSound.Unpause();
		}
	}

	/// <summary>
	/// Gets the duration of the bite preparation.
	/// </summary>
	/// <value>The duration of the bite preparation.</value>
	public float BitePreparationDuration
	{
		get { return m_bitePrepDuration; }
	}

	#endregion // Public Interface

	#region Serialized Variables

	[Header("Sprites")]
	[SerializeField] private	Sprite				m_normalSprite				= null;
	[SerializeField] private	Sprite				m_openSprite				= null;
	[SerializeField] private	Sprite				m_closedSprite				= null;
	[Header("Movement")]
	[SerializeField] private	float				m_moveSpeed					= 8.0f;
	[SerializeField] private	Vector2				m_moveLimitsMin				= Vector2.zero;
	[SerializeField] private	Vector2				m_moveLimitsMax				= Vector2.zero;
	[SerializeField] private	float				m_bitePrepDistance			= 2.5f;
	[SerializeField] private	float				m_bitePrepDuration			= 2.0f;
	[SerializeField] private	float				m_bitePrepMoveSpeed			= 0.2f;
	[SerializeField] private	bool				m_isFlickAngleApplied		= false;
	[SerializeField] private	float				m_flickAngleMin				= 45.0f;
	[SerializeField] private	float				m_flickedMoveSpeed			= 1.0f;
	[SerializeField] private	GameObject			m_character					= null;

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

		switch (m_state)
		{
		case State.MOVE:
			UpdateMovement();
			break;

		case State.BITE_PREPARE:
			UpdateBitePreparation();
			break;

		case State.BITE:
			break;

		case State.FLICKED:
			UpdateFlickedState();
			break;

		case State.DISABLED:
			UpdateDisabledState();
			break;
		}
	}

	#endregion // MonoBehaviour

	#region State

	private enum State
	{
		NONE,
		MOVE,
		BITE_PREPARE,
		BITE,
		FLICKED,
		DISABLED
	}

	private		State		m_state				= State.NONE;
	private		bool		m_isPaused			= false;
	private		SoundObject	m_stateSound		= null;

	#endregion // State

	#region Movement
	
	private		Vector2		m_moveDirection		= Vector2.zero;

	/// <summary>
	/// Starts the movement.
	/// </summary>
	private void StartMovement()
	{
		// Random initial position
		Vector3 initPos = Vector3.zero;
		int randArea = Random.Range(0, 3);
		if (randArea == 0)
		{
			// Left edge
			initPos.x = m_moveLimitsMin.x;
			initPos.y = Random.Range(m_moveLimitsMin.y, m_moveLimitsMax.y);
		}
		else if (randArea == 1)
		{
			// Right edge
			initPos.x = m_moveLimitsMax.x;
			initPos.y = Random.Range(m_moveLimitsMin.y, m_moveLimitsMax.y);
		}
		else
		{
			// Bottom edge
			initPos.x = Random.Range(m_moveLimitsMin.x, m_moveLimitsMax.x);
			initPos.y = m_moveLimitsMin.y;
		}
		transform.position = initPos;

		UpdateMoveDirection();

		// Normal sprite
		if (m_normalSprite != null)
		{
			m_spriteRenderer.sprite = m_normalSprite;
		}

		m_state = State.MOVE;
	}

	/// <summary>
	/// Updates the movement.
	/// </summary>
	private void UpdateMovement()
	{
		UpdateMoveDirection();
		transform.Translate((Vector3)(m_moveDirection * m_moveSpeed * Time.deltaTime), Space.World);
//		ClampPosition();

		float distSqrMag = (m_character.transform.position - transform.position).sqrMagnitude;
		if (distSqrMag <= m_bitePrepDistance * m_bitePrepDistance)
		{
			StartBitePreparation();
		}
	}

	/// <summary>
	/// Updates the move direction.
	/// </summary>
	private void UpdateMoveDirection()
	{
		// Move direction is toward the character
		m_moveDirection = Vector3.Normalize(m_character.transform.position - transform.position);
		UpdateSpriteFacing();
	}

	/// <summary>
	/// Updates the sprite facing.
	/// </summary>
	private void UpdateSpriteFacing()
	{
		Vector3 newRot = Vector3.one;
		// Make the sprite face the move direction
		if (m_moveDirection.y >= 0f)
		{
			transform.SetScaleY(Mathf.Abs(transform.localScale.y));
			newRot = Vector3.Cross(Vector3.up, m_moveDirection);
		}
		else
		{
			transform.SetScaleY(-Mathf.Abs(transform.localScale.y));
			newRot = Vector3.Cross(Vector3.down, m_moveDirection);
		}

		// Align the transform with the move direction
		newRot.z = Mathf.Asin(newRot.z) * Mathf.Rad2Deg;
		transform.eulerAngles = newRot;
		if (m_moveDirection.x >= 0f)
		{
			if (m_moveDirection.y >= 0f)
			{
				MakeSpriteFaceRight();
			}
			else
			{
				MakeSpriteFaceLeft();
			}
			transform.Rotate(0f, 0f, 90.0f);
		}
		else
		{
			if (m_moveDirection.y >= 0f)
			{
				MakeSpriteFaceLeft();
			}
			else
			{
				MakeSpriteFaceRight();
			}
			transform.Rotate(0f, 0f, -90.0f);
		}
	}

	/// <summary>
	/// Clamps the position.
	/// </summary>
	private void ClampPosition()
	{
		Vector3 newPos = transform.position;
		newPos.x = Mathf.Clamp(newPos.x, m_moveLimitsMin.x, m_moveLimitsMax.x);
		newPos.y = Mathf.Clamp(newPos.y, m_moveLimitsMin.y, m_moveLimitsMax.y);
		transform.position = newPos;
	}
	
	#endregion // Movement

	#region Bite and Flick

	private		float				m_stateTimer		= 0f;
	private		float				m_flickedStateDur	= 0.5f;
	private		OnFlickDelegate		m_onFlick			= null;
	private		OnBiteDelegate		m_onBite			= null;

	/// <summary>
	/// Starts the bite preparation.
	/// </summary>
	private void StartBitePreparation()
	{
		m_stateTimer = 0f;
		m_isFlickable = true;
		UpdateIsFlickableField();

		if (m_openSprite != null)
		{
			m_spriteRenderer.sprite = m_openSprite;
		}

		AddFlickDelegate(OnPiranhaFlicked);

		m_state = State.BITE_PREPARE;
	}

	/// <summary>
	/// Updates the bite preparation.
	/// </summary>
	private void UpdateBitePreparation()
	{
		UpdateMoveDirection();
		m_stateTimer += Time.deltaTime;

		transform.Translate((Vector3)(m_moveDirection * m_bitePrepMoveSpeed * Time.deltaTime), Space.World);
//		ClampPosition();

		// Bite the character after preparing
		if (m_stateTimer >= m_bitePrepDuration)
		{
			BiteCharacter();
		}
	}

	/// <summary>
	/// Updates the disabled state.
	/// </summary>
	private void UpdateDisabledState()
	{
		m_stateTimer += Time.deltaTime;
		
		transform.Translate((Vector3)(m_moveDirection * m_bitePrepMoveSpeed * Time.deltaTime), Space.World);
	}

	/// <summary>
	/// Starts flicked state
	/// </summary>
	private void StartFlickedState()
	{
		m_state = State.FLICKED;
		m_stateTimer = 0f;
		// m_moveDirection = -m_moveDirection;
		m_moveDirection = m_flickGesture.ScreenFlickVector.normalized;
		UpdateSpriteFacing();
		transform.SetScaleY(-transform.localScale.y);
		
	
		// Normal sprite
		if (m_normalSprite != null)
		{
			m_spriteRenderer.sprite = m_normalSprite;
		}

		m_stateSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.FISH_FLICK);
	}

	/// <summary>
	/// Updates flicked state
	/// </summary>
	private void UpdateFlickedState()
	{
		m_stateTimer += Time.deltaTime;
		float lerpT = m_stateTimer / m_flickedStateDur;
		transform.Translate((Vector3)(m_moveDirection * m_flickedMoveSpeed * Time.deltaTime), Space.World);

		Color color = m_spriteRenderer.color;
		color.a = Mathf.Lerp(1.0f, 0f, lerpT);
		m_spriteRenderer.color = color;
		if (lerpT >= 1.0f)
		{
			gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Bites the character.
	/// </summary>
	private void BiteCharacter()
	{
		m_state = State.BITE;

		m_stateSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.FISH_BITE);

		if (m_closedSprite != null)
		{
			m_spriteRenderer.sprite = m_closedSprite;
		}

		if (m_onBite != null)
		{
			m_onBite();
			m_onBite = null;
		}
	}

	/// <summary>
	/// Called when the piranha is "flicked"
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void OnPiranhaFlicked(object sender, System.EventArgs e)
	{
		if (!m_isFlickAngleApplied || !IsAlongMovementDirection(m_flickGesture.ScreenFlickVector))
		{
			StartFlickedState();

			if (m_onFlick != null)
			{
				m_onFlick();
				m_onFlick = null;
			}

			RemoveFlickDelegate(OnPiranhaFlicked);
		}
	}

	/// <summary>
	/// Determines whether this instance is along movement direction the specified vec.
	/// </summary>
	/// <returns><c>true</c> if this instance is along movement direction the specified vec; otherwise, <c>false</c>.</returns>
	/// <param name="vec">Vec.</param>
	private bool IsAlongMovementDirection(Vector2 vec)
	{
		// Get the angle between the movement vector and the given vector
		float angle = Mathf.Acos(Vector2.Dot(vec.normalized, m_moveDirection)) * Mathf.Rad2Deg;
		// Within m_flickAngleMin, the vector is treated as along the movement direction
		if (angle < Mathf.Abs(m_flickAngleMin) && angle > -Mathf.Abs(m_flickAngleMin))
		{
			return true;
		}
		return false;
	}

	#endregion // Bite and Flick
}
