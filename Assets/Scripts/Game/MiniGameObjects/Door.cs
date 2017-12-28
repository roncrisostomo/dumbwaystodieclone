/******************************************************************************
*  @file       Door.cs
*  @brief      Handles a door instance in the Door Minigame
*  @author     Lori
*  @date       July 20, 2015
*      
*  @par [explanation]
*		> Detects presses
*		> Knows where a peeking character should be positioned 
******************************************************************************/

#region Namespaces

using UnityEngine;

#endregion // Namespaces

public class Door : InteractiveObject
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
	}

	/// <summary>
	/// Gets the peek area position.
	/// </summary>
	/// <returns>The peek area position.</returns>
	public Vector3 GetPeekAreaPosition()
	{
		if (m_PeekAreaPosition != null)
		{
			return m_PeekAreaPosition.position;
		}
		return transform.position;
	}
	
	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private	Transform	m_PeekAreaPosition		= null;
		
	#endregion // Serialized Variables

	#region Input

	private		OnPressDelegate		m_onPress				= null;

	/// <summary>
	/// Initializes the input.
	/// </summary>
	private void InitializeInput()
	{
		AddPressDelegate(OnDoorPress);
	}
	
	/// <summary>
	/// Called when the door has been pressed
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void OnDoorPress(object sender, System.EventArgs e)
	{
		if (m_onPress != null)
		{
			m_onPress();
		}
	}

	#endregion // Input
}
