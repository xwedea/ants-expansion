using Godot;
using System;

public partial class AntBase : CharacterBody2D
{
	public const float Speed = 120;

	public Vector2 TargetLocation;
	public bool HaveTargetLocation = false;


	public override void _Ready()
	{
		base._Ready();
		
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Position.DistanceTo(TargetLocation) < 1) {
			HaveTargetLocation = false;
		}

		if (HaveTargetLocation) {
			float distance = Position.DistanceTo(TargetLocation);
			Vector2 direction = Position.DirectionTo(TargetLocation);

			if (distance < Speed) {
				Velocity = direction * distance;
			}
			Velocity = direction * Speed;


			MoveAndSlide();
		}


	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton eventMouseButton) {
			TargetLocation = eventMouseButton.GlobalPosition;
			HaveTargetLocation = true;
		}


	}

	
}
