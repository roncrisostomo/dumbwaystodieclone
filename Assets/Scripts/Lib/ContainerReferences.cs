/******************************************************************************
*  @file       ContainerReferences.cs
*  @brief      Holds references to container objects in the generic scene hierarcy
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

public class ContainerReferences : MonoBehaviour
{
	#region Public Interface

	public Transform 	TouchDebugger	= null;
	public Transform 	Gameplay		= null;
	public Transform 	DynamicObjects	= null;
	public Transform 	World			= null;
	public Transform 	UI				= null;
	public Transform 	Sound			= null;
	public UICamera		UICamera		= null;

	#endregion // Public Interface
}
