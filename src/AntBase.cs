using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class AntBase : CharacterBody2D
{
	public AudioStreamPlayer2D AttackAudio;

	protected AnimationPlayer AnimPlayer;
	protected Area2D EngageArea;
	protected Timer EngageTimer;
	protected CollisionShape2D Capsule;
	protected AnimatedSprite2D Sprite;

	protected NavigationAgent2D NavAgent;
	protected Rid NavMap;
	protected Rid NavRegion;
	protected List<Vector2> NavPath = new List<Vector2>();

	protected Game2D GameNode;

	public RandomNumberGenerator Rand = new RandomNumberGenerator();

	
	enum AntState {
		Idle,
		Walking,
		Gathering,
		Attacking
	}

	AntState State = AntState.Idle;
	Node2D Target;
	public int Health = 100;
	protected int Damage = 50;

	public const float VelocityMultiplier = 12000;
	
	public override void _Ready()
	{
		base._Ready();

		AttackAudio = GetNode<AudioStreamPlayer2D>("AttackAudio");

		NavAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		Capsule = GetNode<CollisionShape2D>("CollisionShape2D");
		GameNode = GetTree().Root.GetNode<Game2D>("Game2D");
		EngageArea = GetNode<Area2D>("EngageArea");
		Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		AnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		AnimPlayer.Play("idle");

		EngageTimer = GetNode<Timer>("EngageTimer");
		EngageTimer.Start();

	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (State == AntState.Attacking) {
			return;
		}
		else if (State == AntState.Gathering) {
			return;
		}

		if (Target == null) {
			if (NavAgent.IsNavigationFinished()) {
				Velocity = Vector2.Zero;
				ToIdle();
			}
			else {
				ToIdle();
			}
		}
		else {
			if (NavAgent.IsNavigationFinished()) {
				Velocity = Vector2.Zero;
				
			}
			else if (State == AntState.Attacking) {

			}
			else if (State == AntState.Gathering) {

			}
			else {
				NavAgent.TargetPosition = Target.Position;
			}

		}


		Vector2 nextPosition = NavAgent.GetNextPathPosition();
		Vector2 direction = Position.DirectionTo(nextPosition);

		Velocity = direction * VelocityMultiplier * (float)delta;
		MoveAndSlide();
		
		float velSize = Velocity.Length();
		if (velSize == 0) {
			ToIdle();
		}
		else {
			float radians = Velocity.Angle();
			float angle = (float)(radians * (180.0 / Math.PI));

			// GD.Print(angle);

			if (angle >= -30 && angle < 30) {
				AnimPlayer.Play("walk_side");
				
				Capsule.Scale = new Vector2(1, 1);
			}
			else if (angle >= 30 && angle < 60) {
				AnimPlayer.Play("walk_down_diagonal");
				Capsule.Scale = new Vector2(1, 1);
			}
			else if (angle >= 60 && angle < 120) {
				AnimPlayer.Play("walk_down");
				Capsule.Scale = new Vector2(1, 1);
			}
			else if (angle >= 120 && angle < 150) {
				AnimPlayer.Play("walk_down_diagonal");
				Capsule.Scale = new Vector2(-1, 1);
			}
			else if (angle >= 150 || angle < -150) {
				AnimPlayer.Play("walk_side");
				Capsule.Scale = new Vector2(-1, 1);
			}
			else if (angle >= -150 && angle < -120) {
				AnimPlayer.Play("walk_up_diagonal");
				Capsule.Scale = new Vector2(-1, 1);
			}
			else if (angle >= -120 && angle < -60) {
				AnimPlayer.Play("walk_up");
				Capsule.Scale = new Vector2(1, 1);
			}
			else if (angle >= -60 && angle < -30) {
				AnimPlayer.Play("walk_up_diagonal");
				Capsule.Scale = new Vector2(1, 1);
			}
			
			State = AntState.Walking;
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
			Node2D otherParent = otherArea.GetParent<Node2D>();

			if (Target != null && otherParent == Target) {
				GD.Print("DETECTED");
				ToAttacking();
			}
		}

		NavAgent.TargetPosition = Position;
	}

	private void OnEngageAreaExited(Area2D otherArea)
	{
		Node2D otherParent = otherArea.GetParent<Node2D>();

		if (Target != null && otherParent == Target) {
			NavAgent.TargetPosition = otherArea.Position;
		}
		else {
			ToIdle();
		}
	}


	public virtual void ToIdle() {
		State = AntState.Idle;

		String currentAnim = AnimPlayer.CurrentAnimation;
		if (currentAnim != "idle")
			AnimPlayer.Play("idle");
	}
	public virtual void ToWalking() {
		State = AntState.Walking;

		String currentAnim = AnimPlayer.CurrentAnimation;
		if (currentAnim != "walk")
			AnimPlayer.Play("walk");
	}
	public virtual void ToGathering() {
		State = AntState.Gathering;

		String currentAnim = AnimPlayer.CurrentAnimation;
		if (currentAnim != "gather")
			AnimPlayer.Play("gather");
	}
	public virtual void ToAttacking() {
		State = AntState.Attacking;

		String currentAnim = AnimPlayer.CurrentAnimation;
		if (currentAnim != "attack")
			AnimPlayer.Play("attack");
	}

	public virtual void SetTarget(Node2D node) {
		GD.Print("SetTarget");
		Target = node;
		NavAgent.TargetPosition = node.Position;
	}

	public virtual void MoveToLocation(Vector2 location) {
		GD.Print("MoveToLocation");
		Target = null;
		NavAgent.TargetPosition = location;
	}


	public virtual void MouseLeftClick() {
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
		// if (Input.IsActionJustPressed("LeftClick")) MouseLeftClick();

	}

	protected virtual void OnEngageTimeout()
	{
		if (State == AntState.Gathering) {
			GameNode.ChangeFoodStock(10);
		}

		if (State == AntState.Attacking) {
			EnemyAnt enemy = (EnemyAnt) Target;
			enemy.Target = this;
			enemy.ToAttacking();
			if (enemy.GetDamage(35)) {
				ToIdle();
				Target = null;
				NavAgent.TargetPosition = Position;
			}
		}

	}

	public void SetColor(String code) {
		if (Sprite != null) 
			Sprite.SelfModulate = new Color(code);
	}

	public void Message(String str) {
		GD.Print(str);
	}

	public bool GetDamage(int amount) {
		Health -= amount;

		if (Health <= 0 ) {
			GameNode.SelectedAnts.Remove(this);
			Free();
			return true;
		}

		else return false;
	}


}




