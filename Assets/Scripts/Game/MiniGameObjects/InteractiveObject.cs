/******************************************************************************
*  @file       InteractiveObject.cs
*  @brief      (Base) class for a tappable/draggable in-game sprite
*  @author     Lori, Ron
*  @date       July 29, 2015
*      
*  @par [explanation]
*		> Combined+edited UIElement and UIButton for in-game use!
*		> Allows the object to be pressed
*		> Allows the object to be dragged
*		> Allows the object to be flicked
*		> Accepts press and release delegates
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript;
using TouchScript.Gestures;
using TouchScript.Behaviors;
using TouchScript.Gestures.Simple;

#endregion // Namespaces

public class InteractiveObject : MonoBehaviour
{
	#region Public Interface
	
	// Getters
	public bool IsPressable
	{
		get { return m_isPressable; }
	}
	public bool IsDraggable
	{
		get { return m_isDraggable; }
	}
	public bool IsFlickable
	{
		get { return m_isFlickable; }
	}
	public bool IsPressed
	{
		get { return m_isPressed; }
	}
	
	/// <summary>
	/// Adds the press delegate.
	/// </summary>
	/// <returns><c>true</c>, if pressed delegate was added, <c>false</c> otherwise.</returns>
	/// <param name="pressDelegate">Function delegate to call on button press.</param>
	public bool AddPressDelegate(System.EventHandler<System.EventArgs> pressDelegate)
	{
		if (m_pressGesture != null && m_pressDelegateSet.Add(pressDelegate))
		{
			m_pressGesture.Pressed += pressDelegate;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Adds the release delegate.
	/// </summary>
	/// <returns><c>true</c>, if released delegate was added, <c>false</c> otherwise.</returns>
	/// <param name="releaseDelegate">Function delegate to call on button release.</param>
	public bool AddReleaseDelegate(System.EventHandler<System.EventArgs> releaseDelegate)
	{
		if (m_pressGesture != null)
		{
			return m_releaseDelegateSet.Add(releaseDelegate);
		}
		return false;
	}

	/// <summary>
	/// Adds the flick delegate.
	/// </summary>
	/// <returns><c>true</c>, if flick delegate was added, <c>false</c> otherwise.</returns>
	/// <param name="flickDelegate">Flick delegate.</param>
	public bool AddFlickDelegate(System.EventHandler<System.EventArgs> flickDelegate)
	{
		if (m_flickGesture != null && m_flickDelegateSet.Add(flickDelegate))
		{
			m_flickGesture.Flicked += flickDelegate;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Removes the press delegate.
	/// </summary>
	/// <returns><c>true</c>, if pressed delegate was removed, <c>false</c> otherwise.</returns>
	/// <param name="pressDelegate">Pressed delegate to remove.</param>
	public bool RemovePressDelegate(System.EventHandler<System.EventArgs> pressDelegate)
	{
		if (m_pressDelegateSet.Contains(pressDelegate))
		{
			m_pressDelegateSet.Remove(pressDelegate);
			m_pressGesture.Pressed -= pressDelegate;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Removes the release delegate.
	/// </summary>
	/// <returns><c>true</c>, if released delegate was removed, <c>false</c> otherwise.</returns>
	/// <param name="releaseDelegate">Released delegate to remove.</param>
	public bool RemoveReleaseDelegate(System.EventHandler<System.EventArgs> releaseDelegate)
	{
		if (m_releaseDelegateSet.Contains(releaseDelegate))
		{
			m_releaseDelegateSet.Remove(releaseDelegate);
			return true;
		}
		return false;
	}

	/// <summary>
	/// Removes the flick delegate.
	/// </summary>
	/// <returns><c>true</c>, if flick delegate was removed, <c>false</c> otherwise.</returns>
	/// <param name="flickDelegate">Flick delegate.</param>
	public bool RemoveFlickDelegate(System.EventHandler<System.EventArgs> flickDelegate)
	{
		if (m_flickGesture != null && m_flickDelegateSet.Add(flickDelegate))
		{
			m_flickGesture.Flicked -= flickDelegate;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Sets the sprite for unpressed state.
	/// </summary>
	/// <param name="unpressedSprite">Unpressed sprite.</param>
	public void SetUnpressedSprite(Sprite unpressedSprite)
	{
		m_unpressedSprite = unpressedSprite;
	}

	/// <summary>
	/// Sets the sprite for pressed state.
	/// </summary>
	/// <param name="pressedSprite">Pressed sprite.</param>
	public void SetPressedSprite(Sprite pressedSprite)
	{
		m_pressedSprite = pressedSprite;
	}

	/// <summary>
	/// Delete this instance.
	/// </summary>
	public virtual void Delete()
	{
		if (m_pressGesture != null)
		{
			foreach (System.EventHandler<System.EventArgs> pressDelegate in m_pressDelegateSet)
			{
				m_pressGesture.Pressed -= pressDelegate;
			}
			m_pressDelegateSet.Clear();
			m_releaseDelegateSet.Clear();
		}
		if (m_flickGesture != null)
		{
			foreach (System.EventHandler<System.EventArgs> flickDelegate in m_flickDelegateSet)
			{
				m_flickGesture.Flicked -= flickDelegate;
			}
			m_flickDelegateSet.Clear();
		}
		if (m_collider2D != null)
		{
			m_collider2D.enabled = false;
		}
	}

	/// <summary>
	/// Notifies the pause.
	/// </summary>
	public virtual void NotifyPause()
	{
		if (m_collider2D != null)
		{
			m_collider2D.enabled = false;
		}
	}

	/// <summary>
	/// Notifies the unpause.
	/// </summary>
	public virtual void NotifyUnpause()
	{
		if (m_collider2D != null)
		{
			m_collider2D.enabled = true;
		}
	}

	#endregion // Public Interface

	#region Serialized Variables

	// Can object be pressed?
	[SerializeField] protected	bool		m_isPressable		= false;
	// Can obect be dragged?
	[SerializeField] protected	bool		m_isDraggable		= false;
	// Can object be flicked?
	[SerializeField] protected	bool		m_isFlickable		= false;
	// Normal sprite
	[SerializeField] protected	Sprite		m_unpressedSprite	= null;
	// Same as unpressed sprite if left empty
	[SerializeField] protected	Sprite		m_pressedSprite		= null;

	#endregion // Serialized Variables

	#region MonoBehaviour

	/// <summary>
	/// Awake this instance.
	/// </summary>
	protected virtual void Awake()
	{
		InitializeComponents();
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	protected virtual void Start()
	{
	
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	protected virtual void Update()
	{
		// Enable Gesture components as needed
		UpdateIsPressableField();
		UpdateIsDraggableField();
		UpdateIsFlickableField();

		UpdateActiveTouch();
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	protected virtual void OnDestroy()
	{

	}

	#endregion // MonoBehaviour

	#region Components
	
	protected	SpriteRenderer 		m_spriteRenderer		= null;
	protected	Collider2D			m_collider2D			= null;
	protected	PressGesture		m_pressGesture			= null;
	protected	SimplePanGesture	m_simplePanGesture		= null;
	protected	FlickGesture		m_flickGesture			= null;
	protected	Transformer2D 		m_transformer2D 		= null;

	protected	bool				m_isPressablePrev		= false;
	protected	bool				m_isDraggablePrev		= false;
	protected	bool				m_isFlickablePrev		= false;

	/// <summary>
	/// Initialize all components.
	/// </summary>
	protected virtual void InitializeComponents()
	{
		m_isPressablePrev = m_isPressable;
		m_isDraggablePrev = m_isDraggable;
		m_isFlickablePrev = m_isFlickable;

		// Make sure element has a SpriteRenderer component
		m_spriteRenderer = this.GetComponent<SpriteRenderer>();
		if (m_spriteRenderer == null)
		{
			m_spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
			if (m_spriteRenderer == null)
			{
				m_spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
			}
		}
		// Make sure the elment has a Collider2D component if it needs one
		if (m_isPressable || m_isDraggable || m_isFlickable)
		{
			AddOrGetComponentCollider2D();
		}
		// Make sure element has a PressGesture component if it needs one
		if (m_isPressable)
		{
			m_pressGesture = this.gameObject.AddComponentNoDupe<PressGesture>();
			AddPressDelegate(OnObjectPress);
			AddReleaseDelegate(OnObjectRelease);
		}
		// Make sure element has a SimplePanGesture component if it needs one
		// Make sure element has a Transformer2D component if it needs one
		if (m_isDraggable)
		{
			m_simplePanGesture = this.gameObject.AddComponentNoDupe<SimplePanGesture>();
			m_transformer2D = this.gameObject.AddComponentNoDupe<Transformer2D>();
		}
		// Make sure element has a FlickGesture component if it needs one
		if (m_isFlickable)
		{
			m_flickGesture = this.gameObject.AddComponentNoDupe<FlickGesture>();
		}

		// No need for ReleaseGesture - this script has a different implementation for release
	}

	/// <summary>
	/// Updates the IsPressable property
	/// Components are added/enabled/disabled as needed
	/// </summary>
	protected void UpdateIsPressableField()
	{
		if (m_isPressable == m_isPressablePrev)
		{
			return;
		}

		if (m_isPressable)
		{
			AddOrGetComponentCollider2D();
			m_pressGesture = this.gameObject.AddComponentNoDupe<PressGesture>();
			m_pressGesture.enabled = true;
			AddPressDelegate(OnObjectPress);
			AddReleaseDelegate(OnObjectRelease);
		}
		else
		{
			if (m_pressGesture != null)
			{
				RemovePressDelegate(OnObjectPress);
				RemoveReleaseDelegate(OnObjectRelease);
				m_pressGesture.enabled = false;
			}
		}

		m_isPressablePrev = m_isPressable;
	}

	/// <summary>
	/// Updates the IsDraggable property
	/// Components are added/enabled/disabled as needed
	/// </summary>
	protected void UpdateIsDraggableField()
	{
		if (m_isDraggable == m_isDraggablePrev)
		{
			return;
		}

		if (m_isDraggable)
		{
			AddOrGetComponentCollider2D();
			m_simplePanGesture = this.gameObject.AddComponentNoDupe<SimplePanGesture>();
			m_transformer2D = this.gameObject.AddComponentNoDupe<Transformer2D>();
			m_simplePanGesture.enabled = true;
		}
		else
		{
			if (m_simplePanGesture != null)
			{
				m_simplePanGesture.enabled = false;
			}
		}

		m_isDraggablePrev = m_isDraggable;
	}

	/// <summary>
	/// Updates the IsFlickable property
	/// Components are added/enabled/disabled as needed
	/// </summary>
	protected void UpdateIsFlickableField()
	{
		if (m_isFlickable == m_isFlickablePrev)
		{
			return;
		}
		
		if (m_isFlickable)
		{
			AddOrGetComponentCollider2D();
			m_flickGesture = this.gameObject.AddComponentNoDupe<FlickGesture>();
			m_flickGesture.enabled = true;
		}
		else
		{
			if (m_flickGesture != null)
			{
				m_flickGesture.enabled = false;
			}
		}
		
		m_isFlickablePrev = m_isFlickable;
	}
	
	/// <summary>
	/// Adds or gets a Collider2D component
	/// Collider added is a trigger box collider
	/// </summary>
	protected void AddOrGetComponentCollider2D()
	{
		m_collider2D = this.gameObject.AddDerivedIfNoBase<Collider2D, BoxCollider2D>();
		m_collider2D.isTrigger = true;
	}
	
	#endregion // Components

	#region Input Handling

	protected	ITouch 			m_activeTouch	= null;
	protected	bool			m_isPressed		= false;
	
	/// <summary>
	/// Updates the touch that is on this object.
	/// </summary>
	protected void UpdateActiveTouch()
	{
		if (m_activeTouch == null)
		{
			return;
		}
		// If touch is released or is moved off this object, trigger the release event
		if (!TouchManager.Instance.ActiveTouches.Contains(m_activeTouch) ||
		    m_activeTouch.Hit == null || m_activeTouch.Hit.Transform != this.transform)
		{
			foreach (System.EventHandler<System.EventArgs> releaseDelegate in m_releaseDelegateSet)
			{
				releaseDelegate(this, new System.EventArgs());
			}
		}
	}
	
	#endregion // Input Handling

	#region Delegates
	
	protected HashSet<System.EventHandler<System.EventArgs>> m_pressDelegateSet 
		= new HashSet<System.EventHandler<System.EventArgs>>();
	protected HashSet<System.EventHandler<System.EventArgs>> m_releaseDelegateSet
		= new HashSet<System.EventHandler<System.EventArgs>>();
	protected HashSet<System.EventHandler<System.EventArgs>> m_flickDelegateSet 
		= new HashSet<System.EventHandler<System.EventArgs>>();
	
	/// <summary>
	/// Raises the press event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected void OnObjectPress(object sender, System.EventArgs e)
	{
		// Process only one touch per button
		if (m_activeTouch != null)
		{
			return;
		}

		if (m_pressedSprite != null)
		{
			m_spriteRenderer.sprite = m_pressedSprite;
		}
		m_isPressed = true;
		
		// Find and store a reference to the touch on this button
		foreach (ITouch touch in m_pressGesture.ActiveTouches)
		{
			if (m_pressGesture.HasTouch(touch))
			{
				m_activeTouch = touch;
				break;
			}
		}
	}
	
	/// <summary>
	/// Raises the release event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected void OnObjectRelease(object sender, System.EventArgs e)
	{
		if (m_unpressedSprite != null)
		{
			m_spriteRenderer.sprite = m_unpressedSprite;
		}
		m_activeTouch = null;
		m_isPressed = false;
	}
	
	#endregion // Delegates

	#region Sprite

	/// <summary>
	/// Makes the sprite face right.
	/// </summary>
	protected void MakeSpriteFaceRight ()
	{
		transform.SetScaleX(Mathf.Abs(transform.localScale.x));
	}

	/// <summary>
	/// Makes the sprite face right.
	/// </summary>
	protected void MakeSpriteFaceLeft ()
	{
		transform.SetScaleX(-Mathf.Abs(transform.localScale.x));
	}

	#endregion // Sprite
}
