/******************************************************************************
*  @file       GameData.cs
*  @brief      Holds static game values
*  @author     Lori
*  @date       July 24, 2015
*      
*  @par [explanation]
*		> Holds the high score and game settings
******************************************************************************/

#region Namespaces

using System.Runtime.InteropServices;

#endregion // Namespaces

// TODO: Special things to do when saving arrays/strings

[StructLayout(LayoutKind.Sequential)]
public struct GameData
{
	public		bool		SoundsOn;
	public		bool		IncludePlaneGame;
	public		bool		PledgeDone;
	public		int			HighScore;
	public		uint		ItemUnlockIndex;

	public void Initialize ()
	{
		SoundsOn = true;
		IncludePlaneGame = true;
		PledgeDone = false;
		HighScore = 0;
		ItemUnlockIndex = 0;
	}
}
