/******************************************************************************
*  @file       TransformExtensions.cs
*  @brief      Extensions for Unity's Transform component
*  @author     Ron
*  @date       July 22, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System;

#endregion // Namespaces

public static class TransformExtensions
{
	#region Public Interface

	/// <summary>
	/// Set the x component of the transform's position.
	/// </summary>
	public static void SetPosX(this Transform transform, float x)
	{
		Vector3 newPosition = new Vector3(x, transform.position.y, transform.position.z);
		transform.position = newPosition;
	}

	/// <summary>
	/// Set the y component of the transform's position.
	/// </summary>
	public static void SetPosY(this Transform transform, float y)
	{
		Vector3 newPosition = new Vector3(transform.position.x, y, transform.position.z);
		transform.position = newPosition;
	}

	/// <summary>
	/// Set the z component of the transform's position.
	/// </summary>
	public static void SetPosZ(this Transform transform, float z)
	{
		Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, z);
		transform.position = newPosition;
	}

	/// <summary>
	/// Set the x component of the transform's rotation (in euler angles).
	/// </summary>
	public static void SetRotX(this Transform transform, float x)
	{
		Vector3 newRotation = new Vector3(x, transform.eulerAngles.y, transform.eulerAngles.z);
		transform.eulerAngles = newRotation;
	}

	/// <summary>
	/// Set the y component of the transform's rotation (in euler angles).
	/// </summary>
	public static void SetRotY(this Transform transform, float y)
	{
		Vector3 newRotation = new Vector3(transform.eulerAngles.x, y, transform.eulerAngles.z);
		transform.eulerAngles = newRotation;
	}

	/// <summary>
	/// Set the z component of the transform's rotation (in euler angles).
	/// </summary>
	public static void SetRotZ(this Transform transform, float z)
	{
		Vector3 newRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, z);
		transform.eulerAngles = newRotation;
	}

	/// <summary>
	/// Set the x component of the transform's local scale.
	/// </summary>
	public static void SetScaleX(this Transform transform, float x)
	{
		Vector3 newScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
		transform.localScale = newScale;
	}

	/// <summary>
	/// Set the y component of the transform's local scale.
	/// </summary>
	public static void SetScaleY(this Transform transform, float y)
	{
		Vector3 newScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
		transform.localScale = newScale;
	}

	/// <summary>
	/// Set the z component of the transform's local scale.
	/// </summary>
	public static void SetScaleZ(this Transform transform, float z)
	{
		Vector3 newScale = new Vector3(transform.localScale.x, transform.localScale.y, z);
		transform.localScale = newScale;
	}

	#endregion // Public Interface
}
