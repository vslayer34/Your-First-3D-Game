using Godot;
using System;
using YourFirst3DGame.Scripts.Characters;
using YourFirst3DGame.Scripts.Enemies;
using YourFirst3DGame.Scripts.Helper;
using YourFirst3DGame.Scripts.UI;

namespace YourFirst3DGame.Scripts;
public partial class Main : Node
{
	[ExportGroup("Required")]
	[Export]
	public PackedScene MobScene { get; private set; }

	[Export]
	public Timer MobTimer { get; private set; }

	[Export]
	public PathFollow3D SpawnLocation { get; private set; }

	[Export]
	public Player Player { get; private set; }

	[Export]
	public ScoreLabel ScoreLabel { get; private set; }

	[Export]
	public ColorRect RetryScreen { get; private set; }



    // Game Loop Methods---------------------------------------------------------------------------

    public override void _Ready()
    {
        MobTimer.Timeout += OnMobTimerTimeOut;
		Player.Hit += OnPlayerHit;

		RetryScreen.Hide();
    }

	// Reload the game scene if the user pressed enter whil on retry screen
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed(InputActionNames.BuiltIn.UI_ACCEPT) && RetryScreen.Visible)
		{
			GetTree().ReloadCurrentScene();
		}
    }

    public override void _ExitTree()
    {
        Player.Hit -= OnPlayerHit;
    }

    // Signal Methods------------------------------------------------------------------------------

    /// <summary>
    /// Set a new mob position and direction every time out and spawn it to the scene
    /// </summary>
    private void OnMobTimerTimeOut()
	{
		var mobNode = MobScene.Instantiate() as Mob;

		// give the pathfollow3D a random value
		SpawnLocation.ProgressRatio = GD.Randf();

		
		mobNode.Initialize(SpawnLocation.Position, Player != null ? Player.Position : Vector3.Zero);
		AddChild(mobNode);

		// We connect the mob to the score label to update the score upon squashing one.
		mobNode.Squashed += ScoreLabel.OnMobSquashed;
	}

	/// <summary>
	/// Stop spawning enemies when the player dies
	/// </summary>
	private void OnPlayerHit()
	{
		MobTimer.Stop();
		RetryScreen.Show();
	}
}
