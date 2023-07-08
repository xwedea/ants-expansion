using Godot;
using System;
using System.Collections.Generic;

public partial class QueenAnt : AntBase
{
		
	public override void _Ready()
	{
		base._Ready();

	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (NavAgent.IsNavigationFinished()) return;

		Vector2 nextPosition = NavAgent.GetNextPathPosition();
		Vector2 direction = Position.DirectionTo(nextPosition);

		Velocity = direction * Speed;
		MoveAndSlide();
		
	}


}
