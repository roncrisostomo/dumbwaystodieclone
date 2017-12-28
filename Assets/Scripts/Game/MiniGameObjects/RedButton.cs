/******************************************************************************
*  @file       RedButton.cs
*  @brief      Handles a red button instance in the Button Minigame
*  @author     Lori
*  @date       July 20, 2015
*      
*  @par [explanation]
*		> Detects button presses
*		> Adjusts shadow depending on button state
******************************************************************************/

#region Namespaces

using UnityEngine;

#endregion // Namespaces

public class RedButton : InteractiveObject
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

	#endregion // Public Interface

	#region Serialized Variables
		
	#endregion // Serialized Variables

	#region Input

	private		OnPressDelegate		m_onPress				= null;

	/// <summary>
	/// Initializes the input.
	/// </summary>
	private void InitializeInput()
	{
		AddPressDelegate(OnRedButtonPress);
		RemoveReleaseDelegate(OnObjectRelease);
	}
	
	/// <summary>
	/// Called when the button has been pressed
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void OnRedButtonPress(object sender, System.EventArgs e)
	{
		if (m_onPress != null)
		{
			m_onPress();
		}
	}
	
	#endregion // Input

}
