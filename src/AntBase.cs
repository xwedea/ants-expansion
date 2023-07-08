using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class AntBase : CharacterBody2D
{
	protected AnimationPlayer AnimPlayer;
	protected Area2D EngageArea;
	protected Timer EngageTimer;

	protected NavigationAgent2D NavAgent;
	protected Rid NavMap;
	protected Rid NavRegion;
	protected List<Vector2> NavPath = new List<Vector2>();

	protected Game2D GameNode;
	
	enum AntState {
		Idle,
		Walking,
		Gathering,
		Attacking
	}

	AntState State = AntState.Idle;
	Node2D Target;

	public const float VelocityMultiplier = 12000;
	
	public override void _Ready()
	{
		base._Ready();

		NavAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		EngageArea = GetNode<Area2D>("EngageArea");
		GameNode = GetTree().Root.GetNode<Game2D>("Game2D");

		AnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		AnimPlayer.Play("idle");

		EngageTimer = GetNode<Timer>("EngageTimer");
		EngageTimer.Start();

	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (NavAgent.IsNavigationFinished()) {
			Velocity = Vector2.Zero;
			return;
		}
		
		Vector2 nextPosition = NavAgent.GetNextPathPosition();
		Vector2 direction = Position.DirectionTo(nextPosition);

		Velocity = direction * VelocityMultiplier * (float)delta;
		MoveAndSlide();
		
		float velSize = Velocity.Length();
		if (velSize > 0) {
			ToWalking();
		}
		else {
			ToIdle();
		}

	}

	public override void _Process(double delta)
	{

	}

	private void OnEngageAreaEntered(Area2D otherArea)
	{
		if (Target == null) return;

		if (otherArea.IsInGroup("resource_area")) {
			GD.Print("Gathering Entered");
			ToGathering();
		}
		else if (otherArea.IsInGroup("enemy_area")) {
			ToAttacking();
		}

		NavAgent.TargetPosition = Position;
	}

	private void OnEngageAreaExited(Area2D otherArea)
	{
		
	}


	public void ToIdle() {
		State = AntState.Idle;

		String currentAnim = AnimPlayer.CurrentAnimation;
		if (currentAnim != "idle")
			AnimPlayer.Play("idle");
	}
	public void ToWalking() {
		State = AntState.Walking;

		String currentAnim = AnimPlayer.CurrentAnimation;
		if (currentAnim != "walk")
			AnimPlayer.Play("walk");
	}
	public void ToGathering() {
		State = AntState.Gathering;

		String currentAnim = AnimPlayer.CurrentAnimation;
		if (currentAnim != "gather")
			AnimPlayer.Play("gather");
	}
	public void ToAttacking() {
		State = AntState.Attacking;

		String currentAnim = AnimPlayer.CurrentAnimation;
		if (currentAnim != "attack")
			AnimPlayer.Play("attack");
	}


	public void MouseLeftClick() {
		Vector2 mousePos= GetGlobalMousePosition();
		NavAgent.TargetPosition = mousePos;

		// Set target
		PhysicsPointQueryParameters2D query = new PhysicsPointQueryParameters2D();
		query.Position = mousePos;

		PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
		Array<Dictionary> collisionObjects = spaceState.IntersectPoint(query, 1);
		if (collisionObjects.Count > 0) {
			Variant value;
			if (collisionObjects[0].TryGetValue("collider", out value)) {
				Node2D node = (Node2D) value.Obj;
				if (node.IsInGroup("resource") || node.IsInGroup("enemy")) {
					Target = node;
				}
			}
		}
		else {
			Target = null;
		}

	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (Input.IsActionJustPressed("LeftClick")) MouseLeftClick();

	}

	private void OnEngageTimeout()
	{
		if (State == AntState.Gathering) {
			GameNode.ChangeFoodStock(10);
		}

	}
	
}




