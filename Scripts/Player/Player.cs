using Godot;
using YourFirst3DGame.Scripts.Helper;
using System;

namespace YourFirst3DGame.Scripts.Characters;

public partial class Player : CharacterBody3D
{
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

	[Export]
	public float JumpImpulse { get; private set; } = 20.0f;


	private Vector3 _targetVelocity = Vector3.Zero;
	private Vector3 _inputDirection;

	private Node3D _pivot;


    
	public override void _Ready()
    {
        _pivot = GetNode<Node3D>("Pivot");
    }

    public override void _PhysicsProcess(double delta)
    {
        _inputDirection = Vector3.Zero;

		if (Input.IsActionPressed(InputActionNames.MOVE_RIGHT))
		{
			_inputDirection.X += 1.0f;
		}
		if (Input.IsActionPressed(InputActionNames.MOVE_LEFT))
		{
			_inputDirection.X -= 1.0f;
		}
		if (Input.IsActionPressed(InputActionNames.MOVE_BACKWARD))
		{
			_inputDirection.Z += 1.0f;
		}
		if (Input.IsActionPressed(InputActionNames.MOVE_FORWARD))
		{
			_inputDirection.Z -= 1.0f;
		}


		// Normalize the vectore and set the rotation of the node accordingly
		if (_inputDirection != Vector3.Zero)
		{
			_inputDirection = _inputDirection.Normalized();
			_pivot.Basis = Basis.LookingAt(_inputDirection);
		}

		// Set the ground velocity
		_targetVelocity.X = _inputDirection.X * Speed;
		_targetVelocity.Z = _inputDirection.Z * Speed;

		// Sewt the fall velocity if the player is on the air
		if (!IsOnFloor())
		{
			_targetVelocity.Y -= FallAcceleration * (float)delta;
		}

		if (IsOnFloor() && Input.IsActionJustPressed(InputActionNames.JUMP))
		{
			_targetVelocity.Y = JumpImpulse;
		}

		// Finaly move the character
		Velocity = _targetVelocity;

		MoveAndSlide();
    }
}
