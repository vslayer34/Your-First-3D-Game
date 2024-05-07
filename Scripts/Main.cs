using Godot;
using System;
using YourFirst3DGame.Scripts.Characters;
using YourFirst3DGame.Scripts.Enemies;

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



    // Game Loop Methods---------------------------------------------------------------------------

    public override void _Ready()
    {
        MobTimer.Timeout += OnMobTimerTimeOut;
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

		mobNode.Initialize(SpawnLocation.Position, Player.Position);
		AddChild(mobNode);
	}
}
