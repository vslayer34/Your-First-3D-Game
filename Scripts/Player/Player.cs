using Godot;
using YourFirst3DGame.Scripts.Helper;
using System;
using YourFirst3DGame.Scripts.Enemies;

namespace YourFirst3DGame.Scripts.Characters;

public partial class Player : CharacterBody3D
{
	[Signal]
	public delegate void HitEventHandler();



	[ExportGroup("Required Nodes")]
	[Export]
	public Area3D MobDetector { get; private set; }

	[Export]
	public AnimationPlayer AnimationPlayer { get; private set; }


	[ExportCategory("Speed and Gravity")]
	[Export]
	/// <summary>
	/// Player Speed
	/// </summary>
	public float Speed { get; private set; } = 14.0f;

	[Export]
	/// <summary>
	/// Downard fall speed when the player is in the air
	/// </summary>
	public float FallAcceleration { get; private set; } = 75.0f;


	[ExportCategory("Jump and Bounce")]
	[Export]
	public float JumpImpulse { get; private set; } = 20.0f;

	[Export]
	public float BounceImpulse { get; private set; } = 16.0f;


	private Vector3 _targetVelocity = Vector3.Zero;
	private Vector3 _inputDirection;

	private Node3D _pivot;


    
	// Game Loop Methods---------------------------------------------------------------------------

	public override void _Ready()
    {
        _pivot = GetNode<Node3D>("Pivot");
		MobDetector.BodyEntered += OnMobDetectorBodyEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        _inputDirection = Vector3.Zero;

		if (Input.IsActionPressed(InputActionNames.User.MOVE_RIGHT))
		{
			_inputDirection.X += 1.0f;
		}
		if (Input.IsActionPressed(InputActionNames.User.MOVE_LEFT))
		{
			_inputDirection.X -= 1.0f;
		}
		if (Input.IsActionPressed(InputActionNames.User.MOVE_BACKWARD))
		{
			_inputDirection.Z += 1.0f;
		}
		if (Input.IsActionPressed(InputActionNames.User.MOVE_FORWARD))
		{
			_inputDirection.Z -= 1.0f;
		}


		// Normalize the vectore and set the rotation of the node accordingly
		if (_inputDirection != Vector3.Zero)
		{
			_inputDirection = _inputDirection.Normalized();
			_pivot.Basis = Basis.LookingAt(_inputDirection);
			AnimationPlayer.SpeedScale = 4.0f;
		}
		else
		{
			AnimationPlayer.SpeedScale = 1.0f;
		}

		// Set the ground velocity
		_targetVelocity.X = _inputDirection.X * Speed;
		_targetVelocity.Z = _inputDirection.Z * Speed;

		// Sewt the fall velocity if the player is on the air
		if (!IsOnFloor())
		{
			_targetVelocity.Y -= FallAcceleration * (float)delta;
		}

		if (IsOnFloor() && Input.IsActionJustPressed(InputActionNames.User.JUMP))
		{
			_targetVelocity.Y = JumpImpulse;
		}

		// Finaly move the character
		Velocity = _targetVelocity;

		CheckCollisions();

		MoveAndSlide();

		// make the character arc when jumping
		_pivot.Rotation = new Vector3(Mathf.Pi / 6.0f * Velocity.Y / JumpImpulse, _pivot.Rotation.Y, _pivot.Rotation.Z);
    }


	// Member Methods------------------------------------------------------------------------------

	private void CheckCollisions()
	{
		// Iterate throught the collision that occured this frame
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			// Get each collision that happened this call
			var collision = GetSlideCollision(i);


			// if collision is mob and not the ground
			if (collision.GetCollider() is Mob mob)
			{
				// the dot product help to determine if the player is hitting the mob from above
				// if so the player squach the mob and bounce up
				if (Vector3.Up.Dot(collision.GetNormal()) > 0.1f)
				{
					mob.Squash();
					_targetVelocity.Y = BounceImpulse;
					break;				// to prevent multible calls in the same frame for the same collision
				}
			}
		}
	}

	/// <summary>
	/// Emit hit signal and delete the player node
	/// </summary>
	private void Die()
	{
		EmitSignal(SignalName.Hit);
		QueueFree();
	}

	// Signal Methods------------------------------------------------------------------------------

	private void OnMobDetectorBodyEntered(Node3D body)
	{
		Die();
	}
}
