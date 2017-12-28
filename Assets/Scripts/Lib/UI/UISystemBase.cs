/******************************************************************************
*  @file       UISystemBase.cs
*  @brief      Abstract base class for UI system classes
*  @author     
*  @date       July 21, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public abstract class UISystemBase : MonoBehaviour
{
	#region Public Interface

	public abstract bool Initialize();

	public bool IsInitialized
	{
		get { return m_isInitialized; }
	}

	#endregion // Public Interface

	#region Variables

	protected bool m_isInitialized = false;

	#endregion // Variables
}
