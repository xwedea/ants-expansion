using Godot;
using System;
using System.Collections.Generic;

public partial class AntBase : CharacterBody2D
{
	protected AnimationPlayer AnimPlayer;

	protected NavigationAgent2D NavAgent;
	protected Rid NavMap;
	protected Rid NavRegion;
	protected List<Vector2> NavPath = new List<Vector2>();

	enum AntState {
		Idle,
		Walking,
		Gathering,
		Attacking
	}

	AntState AnimState = AntState.Idle;

	public const float Speed = 120;
	
	public override void _Ready()
	{
		base._Ready();

		AnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		NavAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (NavAgent.IsNavigationFinished()) return;

		Vector2 nextPosition = NavAgent.GetNextPathPosition();
		Vector2 direction = Position.DirectionTo(nextPosition);

		Velocity = direction * Speed;
		MoveAndSlide();
		
	}

	public override void _Process(double delta)
	{
		if (Velocity.Length() > 5) {
			ToWalking();
		}
		
		if (Velocity.Length() < 1) {
			ToIdle();
		}

	}

	public void ToIdle() {
		AnimState = AntState.Idle;
		AnimPlayer.Play("idle");
	}
	public void ToWalking() {
		AnimState = AntState.Walking;
		AnimPlayer.Play("walk");
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (Input.IsActionJustPressed("MoveTo")) {
			NavAgent.TargetPosition = GetGlobalMousePosition();
		}
	}

}
