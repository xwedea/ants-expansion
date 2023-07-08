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

	public const float VelocityMultiplier = 12000;
	
	public override void _Ready()
	{
		base._Ready();

		AnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		AnimPlayer.Play("idle");

		NavAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		float velSize = Velocity.Length();

		if (velSize > 0) {
			ToWalking();
		}
		if (velSize == 0) {
			ToIdle();
		}

		if (NavAgent.IsNavigationFinished()) {
			Velocity = Vector2.Zero;
			return;
		}

		Vector2 nextPosition = NavAgent.GetNextPathPosition();
		Vector2 direction = Position.DirectionTo(nextPosition);

		Velocity = direction * VelocityMultiplier * (float)delta;
		MoveAndSlide();
		
	}

	public override void _Process(double delta)
	{
		// if (Input.IsActionJustPressed("Enter")) {
		// 	String currentAnim = AnimPlayer.CurrentAnimation;
		// 	GD.Print(currentAnim);

		// 	if (currentAnim == "idle") {
		// 		ToWalking();
		// 	}
		// 	else if (currentAnim == "walk") {
		// 		ToIdle();
		// 	}
		// }

	}

	public void ToIdle() {
		AnimState = AntState.Idle;

		String currentAnim = AnimPlayer.CurrentAnimation;
		if (currentAnim != "idle")
			AnimPlayer.Play("idle");
	}
	public void ToWalking() {
		AnimState = AntState.Walking;

		String currentAnim = AnimPlayer.CurrentAnimation;
		if (currentAnim != "walk")
			AnimPlayer.Play("walk");
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (Input.IsActionJustPressed("MoveTo")) {
			NavAgent.TargetPosition = GetGlobalMousePosition();
		}
	}

}
