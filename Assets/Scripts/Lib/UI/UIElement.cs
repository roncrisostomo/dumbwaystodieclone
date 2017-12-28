/******************************************************************************
*  @file       UIElement.cs
*  @brief      Base class for all UI elements
*  @author     Ron
*  @date       July 28, 2015
*      
*  @par [explanation]
*		> Holds common methods and fields for UI elements
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;
using TouchScript.Behaviors;
using TouchScript.Gestures.Simple;

#endregion // Namespaces

public class UIElement : MonoBehaviour
{
	#region Public Interface

	public void SetDraggable(bool isDraggable = true)
	{
		m_isDraggable = isDraggable;
	}
	public void SetRotatable(bool isRotatable = true)
	{
		m_isRotatable = isRotatable;
	}
	public void SetScalable(bool isScalable = true)
	{
		m_isScalable = isScalable;
	}
	/*public void SetBlockOtherInput(bool blockOtherInput = true)
	{
		m_blockOtherInput = blockOtherInput;
	}*/

	// Getters
	public bool IsDraggable
	{
		get { return m_isDraggable; }
	}
	public bool IsRotatable
	{
		get { return m_isRotatable; }
	}
	public bool IsScalable
	{
		get { return m_isScalable; }
	}
	/*public bool IsBlockOtherInput
	{
		get { return m_blockOtherInput; }
	}*/

	#endregion // Public Interface

	#region Serialized Variables

	// Can element be dragged?
	[SerializeField] protected	bool		m_isDraggable		= false;
	// Can element be rotated?
	[SerializeField] protected	bool		m_isRotatable		= false;
	// Can element be scaled?
	[SerializeField] protected	bool		m_isScalable		= false;
	// When input is on this element, should input to other UI elements be blocked?
	// TODO: Uncomment when implementation is ready
	//[SerializeField] protected	bool		m_blockOtherInput	= false;

	#endregion // Serialized Variables

	#region Initialization

	/// <summary>
	/// Initialize this instance.
	/// </summary>
	protected void InitializeElement()
	{
		m_spriteRenderer = this.gameObject.AddComponentNoDupe<SpriteRenderer>();
		m_transformer2D = this.gameObject.AddComponentNoDupe<Transformer2D>();
		m_simplePanGesture = this.gameObject.AddComponentNoDupe<SimplePanGesture>();
		m_simpleRotateGesture = this.GetComponent<SimpleRotateGesture>();
		m_simpleScaleGesture = this.gameObject.AddComponentNoDupe<SimpleScaleGesture>();
	}

	#endregion // Initialization

	#region Input Handling

	protected Transformer2D 		m_transformer2D 		= null;
	protected SimplePanGesture		m_simplePanGesture		= null;
	protected SimpleRotateGesture	m_simpleRotateGesture	= null;
	protected SimpleScaleGesture	m_simpleScaleGesture	= null;

	#endregion // Input Handling

	#region Components

	protected SpriteRenderer m_spriteRenderer	= null;

	#endregion // Components

	#region MonoBehaviour

	/// <summary>
	/// Awake this instance.
	/// </summary>
	protected virtual void Awake()
	{
		InitializeElement();
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
		m_simplePanGesture.enabled = m_isDraggable;
		m_simpleRotateGesture.enabled = m_isRotatable;
		m_simpleScaleGesture.enabled = m_isScalable;
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	protected virtual void OnDestroy()
	{

	}

	#endregion // MonoBehaviour
}
