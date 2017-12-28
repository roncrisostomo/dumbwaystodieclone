/******************************************************************************
*  @file       UICamera.cs
*  @brief      Handles initialization and control of the UI camera
*  @author     
*  @date       July 15, 2015
*      
*  @par [explanation]
*		> Does nothing for now. Just a placeholder for when it will be needed.
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class UICamera : MonoBehaviour
{
	#region Public Interface

	/// <summary>
	/// Gets the world coordinates at the lower left corner of the screen.
	/// </summary>
	public Vector2 ScreenMinWorld
	{
		get { return m_uiCamera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 1.0f)); }
	}

	/// <summary>
	/// Gets the world coordinates at the center of the screen.
	/// </summary>
	public Vector2 ScreenCenterWorld
	{
		get { return m_uiCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1.0f)); }
	}

	/// <summary>
	/// Gets the world coordinates at the upper right corner of the screen.
	/// </summary>
	public Vector2 ScreenMaxWorld
	{
		get { return m_uiCamera.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 1.0f)); }
	}

	/// <summary>
	/// Gets the camera.
	/// </summary>
	public Camera GetCamera
	{
		get { return m_uiCamera; }
	}

	#endregion // Public Interface

	#region Serialized Variables

	#endregion // Serialized Variables

	#region Camera

	private Camera m_uiCamera = null;

	#endregion // Camera

	#region MonoBehaviour

	/// <summary>
	/// Awake this instance.
	/// </summary>
	private void Awake()
	{
		// Make sure this object has a UI camera component
		m_uiCamera = this.GetComponent<Camera>();
		if (m_uiCamera == null)
		{
			m_uiCamera = this.gameObject.AddComponent<Camera>();
		}
		// Initialize UI camera settings
		m_uiCamera.orthographic = true;
		//m_uiCamera.orthographicSize = Screen.height * 0.5f;
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
		
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	private void OnDestroy()
	{

	}

	#endregion // MonoBehaviour
}
