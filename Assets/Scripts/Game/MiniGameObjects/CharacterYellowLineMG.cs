/******************************************************************************
*  @file       CharacterYellowLineMG.cs
*  @brief      Handles a character instance in the YellowLine Minigame
*  @author     Lori
*  @date       August 21, 2015
*      
*  @par [explanation]
*		> Detects flicks
*		> Knows its position after flicking
******************************************************************************/

#region Namespaces

using UnityEngine;

#endregion // Namespaces

public class CharacterYellowLineMG : InteractiveObject
{
	#region Public Interface

	public delegate void OnFlickDelegate();

	/// <summary>
	/// Initializes this instance
	/// </summary>
	/// <param name="onFlick">On flick.</param>
	public void Initialize(OnFlickDelegate onFlick)
	{
		m_onFlick = onFlick;
		InitializeInput();
		m_animator = gameObject.GetComponentInChildren<Animator>();
	}

	/// <summary>
	/// Notifies the pause.
	/// </summary>
	public override void NotifyPause()
	{
		base.NotifyPause();
		if (m_animator != null)
		{
			m_animator.speed = 0f;
		}
	}
	
	/// <summary>
	/// Notifies the unpause.
	/// </summary>
	public override void NotifyUnpause()
	{
		base.NotifyUnpause();
		if (m_animator != null)
		{
			m_animator.speed = 1f;
		}
	}

	/// <summary>
	/// Gets a value indicating whether this instance is flicked.
	/// </summary>
	/// <value><c>true</c> if this instance is flicked; otherwise, <c>false</c>.</value>
	public bool IsFlicked
	{
		get { return m_isFlicked; }
	}

	/// <summary>
	/// Starts the fall animation.
	/// </summary>
	public void StartFallAnimation()
	{
		// Put at the back of the train
		m_spriteRenderer.sortingOrder = m_spriteRenderer.sortingOrder - 5;
		m_shadow.SetActive(false);
		m_dottedOval.SetActive(false);
		m_animator.Play("CharacterYellowLineMGSad");
	}

	/// <summary>
	/// Updates the fall animation.
	/// </summary>
	public void UpdateFallAnimation()
	{
		transform.Translate(Vector3.down * m_fallSpeed * Time.deltaTime, Space.World);
		transform.Rotate(Vector3.forward * m_fallRotateSpeed * Time.deltaTime);
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private	GameObject	m_shadow				= null;
	[SerializeField] private	GameObject	m_dottedOval			= null;
	[SerializeField] private	float		m_dottedOvalPosOffset	= 1.7f;	// Account for character's height
	[SerializeField] private	float		m_fallSpeed				= 10f;
	[SerializeField] private	float		m_fallRotateSpeed		= 1080f;
	

	#endregion // Serialized Variables

	#region Movement

	private		Animator			m_animator				= null;

	/// <summary>
	/// Moves the character to the dotted area.
	/// </summary>
	private void MoveToDottedArea()
	{
		if (m_dottedOval != null)
		{
			Vector3 newPos = m_dottedOval.transform.position + (Vector3.up * m_dottedOvalPosOffset);
			m_dottedOval.SetActive(false);
			transform.position = newPos;
		}
	}

	#endregion // Movement

	#region Input

	private		OnFlickDelegate		m_onFlick				= null;
	private		bool				m_isFlicked				= false;

	/// <summary>
	/// Initializes the input.
	/// </summary>
	private void InitializeInput()
	{
		if (m_flickGesture != null)
		{
			AddFlickDelegate(OnCharacterFlick);
		}
		m_isFlicked = false;
	}
	
	/// <summary>
	/// Called when the character has been flicked
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void OnCharacterFlick(object sender, System.EventArgs e)
	{
		if (m_flickGesture.ScreenFlickVector.y < 0)
		{
			if (m_onFlick != null)
			{
				m_isFlicked = true;
				MoveToDottedArea();
				Delete();
				m_onFlick();
			}
		}
	}

	#endregion // Input
}
