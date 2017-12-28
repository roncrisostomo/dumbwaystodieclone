/******************************************************************************
*  @file       UIWindowTester.cs
*  @brief      
*  @author     
*  @date       January 1, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class UIWindowTester : MonoBehaviour
{
	#region Public Interface

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private Transform m_creditsAnimStartPos = null;
	[SerializeField] private Transform m_creditsAnimEndPos = null;

	#endregion // Serialized Variables

	#region Variables

	//private UIWindow m_uiWindow = null;

	private UIAnimator m_creditsAnimator = null;

	#endregion // Variables

	#region MonoBehaviour

	/// <summary>
	/// Awake this instance.
	/// </summary>
	private void Awake()
	{
		//m_uiWindow = this.GetComponent<UIWindow>();
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start()
	{
		m_creditsAnimator = new UIAnimator(this.transform);
		m_creditsAnimator.SetPositionAnimation(m_creditsAnimStartPos.position, m_creditsAnimEndPos.position);
		m_creditsAnimator.SetAnimSpeed(0.1f);
		m_creditsAnimator.AnimateToState2();
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update()
	{
		m_creditsAnimator.Update(Time.deltaTime);
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	private void OnDestroy()
	{

	}

	#endregion // MonoBehaviour
}
