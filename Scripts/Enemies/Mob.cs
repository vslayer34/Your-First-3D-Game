using Godot;
using System;


namespace YourFirst3DGame.Scripts.Enemies;
public partial class Mob : CharacterBody3D
{
	[Signal]
	/// <summary>
	/// Emited when the mob is jumped on by the player
	/// </summary>
	public delegate void SquashedEventHandler();



	[ExportGroup("Required Nodes")]
	[Export]
	private VisibleOnScreenNotifier3D _visibilityNotifier;

	[ExportGroup("")]

	[ExportCategory("Mob speed")]
	[Export]
	private float _minSpeed = 10;

	[Export]
	private float _maxSpeed = 18;



    public override void _Ready()
    {
		_visibilityNotifier.ScreenExited += OnVisibilityNotifierScreenExited;    
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
    }


	public void Initialize(Vector3 startPosition, Vector3 playerPosition)
	{
		// Position the mob at the start position and rotate the player randomly towards the player
		LookAtFromPosition(startPosition, playerPosition, Vector3.Up);
		// Rotate this mob randomly within range of -45 and +45 degrees,
		// so that it doesn't move directly towards the player.
		RotateY((float)GD.RandRange(-Mathf.Pi / 4.0f, Mathf.Pi / 4.0f));

		// Calculate random spped and apply it to the velocity of the player
		int randomSpeed = (int)GD.RandRange(_minSpeed, _maxSpeed);
		Velocity = Vector3.Forward * randomSpeed;

		// We then rotate the velocity vector based on the mob's Y rotation
    	// in order to move in the direction the mob is looking.
		Velocity = Velocity.Rotated(Vector3.Up, Rotation.Y);
	}

	public void Squash()
	{
		EmitSignal(SignalName.Squashed);
		QueueFree();
	}

	// Signal Methods------------------------------------------------------------------------------
	private void OnVisibilityNotifierScreenExited()
	{
		QueueFree();
	}
}
