/******************************************************************************
*  @file       ScoreUITester.cs
*  @brief      
*  @author     
*  @date       July 22, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class ScoreUITester : MonoBehaviour
{
	#region Public Interface

	#endregion // Public Interface

	#region Serialized Variables

	#endregion // Serialized Variables

	#region MonoBehaviour

	/// <summary>
	/// Awake this instance.
	/// </summary>
	private void Awake()
	{

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
		if (Main.Instance != null && Main.Instance.GetScoreUI != null)
		{
			Main.Instance.GetScoreUI.Show();
			shown = true;
		}
	}

	bool shown = false;

	private void OnGUI()
	{
		if (!shown)
		{
			return;
		}

		float width = 150.0f;
		float height = 30.0f;
		float x = (Screen.width - width) * 0.5f;
		float y = 15.0f;
		float spacing = 5.0f;

		if (GUI.Button(new Rect(x, y, width, height), "Success, 3 lives"))
		{
			Main.Instance.GetScoreUI.SetScores(1000, 100, 37, "PANDAS ARE BEST", false, 3);
			Main.Instance.GetScoreUI.StartAnimation();
		}
		y += height + spacing;
		if (GUI.Button(new Rect(x, y, width, height), "Success, 3 lives"))
		{
			Main.Instance.GetScoreUI.SetScores(1137, 100, -137, "BAD LUCK", false, 3);
			Main.Instance.GetScoreUI.StartAnimation();
		}
		y += height + spacing;
		if (GUI.Button(new Rect(x, y, width, height), "Fail, 2 lives"))
		{
			Main.Instance.GetScoreUI.SetScores(1137, 0, 0, "", true, 2);
			Main.Instance.GetScoreUI.StartAnimation();
		}
		y += height + spacing;
		if (GUI.Button(new Rect(x, y, width, height), "Fail, 1 life"))
		{
			Main.Instance.GetScoreUI.SetScores(1137, 0, -20, "INSULT TO INJURY", true, 1);
			Main.Instance.GetScoreUI.StartAnimation();
		}
		y += height + spacing;
		if (GUI.Button(new Rect(x, y, width, height), "Success, 1 life"))
		{
			Main.Instance.GetScoreUI.SetScores(1117, 150, 10, "YAY", false, 1);
			Main.Instance.GetScoreUI.StartAnimation();
		}
		y += height + spacing;
		if (GUI.Button(new Rect(x, y, width, height), "Fail, 0 lives"))
		{
			Main.Instance.GetScoreUI.SetScores(1117, 0, -1000, "GAME OVER", true, 0);
			Main.Instance.GetScoreUI.StartAnimation();
		}
		y += height + spacing;
		if (GUI.Button(new Rect(x, y, width, height), "Reset life UI"))
		{
			Main.Instance.GetScoreUI.ResetLifeUI();
		}
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	private void OnDestroy()
	{

	}

	#endregion // MonoBehaviour
}
