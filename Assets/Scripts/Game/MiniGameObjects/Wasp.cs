/******************************************************************************
*  @file       Wasp.cs
*  @brief      Handles a wasp instance in the wasp minigame
*  @author     Lori
*  @date       August 10, 2015
*      
*  @par [explanation]
*		> Detects presses
******************************************************************************/

#region Namespaces

using UnityEngine;

#endregion // Namespaces

public class Wasp : InteractiveObject
{
	#region Public Interface

	public delegate void OnPressDelegate();

	/// <summary>
	/// Initializes this instance
	/// </summary>
	/// <param name="onPress">On press.</param>
	public void Initialize(OnPressDelegate onPress)
	{
		m_onPress = onPress;
		InitializeInput();
		InitializeMovement();
		// Use the unpressed sprite
		if (m_unpressedSprite != null)
		{
			m_spriteRenderer.sprite = m_unpressedSprite;
		}
		// Ensure that the instance is upright
		transform.SetScaleY(Mathf.Abs(transform.localScale.y));
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

	#endregion // Public Interface

	#region Serialized Variables

	[Header("Before Hit")]
	[SerializeField] private	float		m_moveSpeed			= 1.5f;
	[SerializeField] private	Vector2		m_moveLimitsMin		= Vector2.zero;
	[SerializeField] private	Vector2		m_moveLimitsMax		= Vector2.zero;
	[Header("After Hit")]
	[SerializeField] private	float		m_fallSpeed			= 10.0f;
	[SerializeField] private	float		m_fallDuration		= 3.0f;
	[SerializeField] private	float		m_fallDelay			= 0.25f;
	[SerializeField] private	GameObject	m_hitEffect			= null;

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
		case State.MOVING:
			UpdateMovement();
			break;

		case State.PRESSED:
			UpdatePressedState();
			break;

		case State.FALLING:
			UpdateFallingState();
			break;
		}
	}

	#endregion // MonoBehaviour

	#region State

	private		bool		m_isPaused			= false;

	private	enum State
	{
		MOVING,
		PRESSED,
		FALLING,
		DISABLED
	}
	private		State		m_state				= State.MOVING;
	private		float		m_stateTimer		= 0f;

	#endregion // State

	#region Movement

	private		Vector2		m_moveDirection		= Vector2.zero;
	private		float		m_moveDuration		= 1.0f;

	/// <summary>
	/// Initializes the movement.
	/// </summary>
	private void InitializeMovement()
	{
		m_state = State.MOVING;

		// Random initial position
		Vector3 initPos = Vector3.zero;
		initPos.x = Random.Range(m_moveLimitsMin.x, m_moveLimitsMax.x);
		initPos.y = Random.Range(m_moveLimitsMin.y, m_moveLimitsMax.y);
		transform.position = initPos;

		SetNewMovement();
	}

	/// <summary>
	/// Updates the movement.
	/// </summary>
	private void UpdateMovement()
	{
		m_stateTimer += Time.deltaTime;
		Vector3 newPos = transform.position + (Vector3)(m_moveDirection * m_moveSpeed * Time.deltaTime);
		newPos.x = Mathf.Clamp(newPos.x, m_moveLimitsMin.x, m_moveLimitsMax.x);
		newPos.y = Mathf.Clamp(newPos.y, m_moveLimitsMin.y, m_moveLimitsMax.y);
		transform.position = newPos;

		if (m_stateTimer >= m_moveDuration)
		{
			SetNewMovement();
		}
	}

	/// <summary>
	/// Assigns a new movement direction and duration.
	/// </summary>
	private void SetNewMovement()
	{
		// Random move dir and time
		m_moveDirection = Random.insideUnitCircle;
		m_moveDuration = Random.Range(0.25f, 2.0f);
		m_stateTimer = 0f;

		// Make the sprite face its movement direction
		if (m_moveDirection.x > 0)
		{
			MakeSpriteFaceRight();
		}
		else
		{
			MakeSpriteFaceLeft();
		}
	}

	#endregion // Movement

	#region Pressed and Falling States

	/// <summary>
	/// Starts the state of the pressed.
	/// </summary>
	private void StartPressedState()
	{
		m_state = State.PRESSED;
		m_stateTimer = 0f;
		if (m_hitEffect != null)
		{
			m_hitEffect.SetActive(true);
		}
		Delete();
	}

	/// <summary>
	/// Updates the state of the pressed.
	/// </summary>
	private void UpdatePressedState()
	{
		m_stateTimer += Time.deltaTime;
		if (m_stateTimer >= m_fallDelay)
		{
			StartFallingState();
		}
	}

	/// <summary>
	/// Starts the state of the falling.
	/// </summary>
	private void StartFallingState()
	{
		m_state = State.FALLING;
		if (m_hitEffect != null)
		{
			m_hitEffect.SetActive(false);
		}
		transform.SetScaleY(-transform.localScale.y);
	}

	/// <summary>
	/// Updates the state of the falling.
	/// </summary>
	private void UpdateFallingState()
	{
		m_stateTimer += Time.deltaTime;
		if (m_stateTimer >= m_fallDuration)
		{
			m_state = State.DISABLED;
		}
		transform.Translate(Vector3.down * m_fallSpeed * Time.deltaTime);
	}

	#endregion // Pressed and Falling States

	#region Input

	private		OnPressDelegate		m_onPress				= null;

	/// <summary>
	/// Initializes the input.
	/// </summary>
	private void InitializeInput()
	{
		AddPressDelegate(OnWaspPress);
	}
	
	/// <summary>
	/// Called when the wasp has been pressed
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void OnWaspPress(object sender, System.EventArgs e)
	{
		StartPressedState();
		if (m_onPress != null)
		{
			m_onPress();
		}
	}

	#endregion // Input
}
