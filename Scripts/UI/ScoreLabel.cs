using Godot;
using System;

namespace YourFirst3DGame.Scripts.UI;
public partial class ScoreLabel : Label
{
	private int _score;



	// Signal Methods------------------------------------------------------------------------------

	public void OnMobSquashed()
	{
		Text = $"Score: {++_score}";
	}
}
