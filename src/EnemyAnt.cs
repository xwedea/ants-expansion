using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class EnemyAnt : AntBase
{
	protected AnimationPlayer AnimPlayer;
	protected Area2D EngageArea;
	protected Timer EngageTimer;
	protected Timer PatrolTimer;
	protected CollisionShape2D Capsule;
	protected AnimatedSprite2D Sprite;

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

		Damage = 15;
		
		NavAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		Capsule = GetNode<CollisionShape2D>("CollisionShape2D");
		GameNode = GetTree().Root.GetNode<Game2D>("Game2D");
		EngageArea = GetNode<Area2D>("EngageArea");
		Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		AnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		AnimPlayer.Play("idle");

		EngageTimer = GetNode<Timer>("EngageTimer");
		EngageTimer.Start();

		PatrolTimer = GetNode<Timer>("PatrolTimer");
		PatrolTimer.WaitTime = Rand.Randf() * 3 + 2;;
		PatrolTimer.Start();

	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (Target != null) {
			ToAttacking();
		}

		if (NavAgent == null) {
			GD.Print("Agent was null");
		}

		NavAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");

		if (NavAgent.IsNavigationFinished()) {
			Velocity = Vector2.Zero;
			return;
		}
		

		if (Target != null) {
			NavAgent.TargetPosition = Target.Position;
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


	public override void ToIdle() {
		State = AntState.Idle;

		String currentAnim = AnimPlayer.CurrentAnimation;
		if (currentAnim != "idle")
			AnimPlayer.Play("idle");
	}
	public override void ToWalking() {
		State = AntState.Walking;

		String currentAnim = AnimPlayer.CurrentAnimation;
		if (currentAnim != "walk")
			AnimPlayer.Play("walk");
	}
	public override void ToGathering() {
		State = AntState.Gathering;

		String currentAnim = AnimPlayer.CurrentAnimation;
		if (currentAnim != "gather")
			AnimPlayer.Play("gather");
	}
	public override void ToAttacking() {
		State = AntState.Attacking;

		String currentAnim = AnimPlayer.CurrentAnimation;
		if (currentAnim != "attack")
			AnimPlayer.Play("attack");
	}

	public override void SetTarget(Node2D node) {
		GD.Print("SetTarget");
		Target = node;
		NavAgent.TargetPosition = node.Position;
	}

	public override void MoveToLocation(Vector2 location) {
		GD.Print("MoveToLocation");
		Target = null;
		NavAgent.TargetPosition = location;
	}


	public override void _UnhandledInput(InputEvent @event)
	{
		return;
	}

	protected override void OnEngageTimeout()
	{
		if (State == AntState.Attacking) {
			EnemyAnt enemy = (EnemyAnt) Target;
			enemy.GetDamage(35);
		}
	}

	public void SetColor(String code) {
		if (Sprite != null) 
			Sprite.SelfModulate = new Color(code);
	}

	public void Message(String str) {
		GD.Print(str);
	}


	private void OnPatrolTimeout()
	{	
		if (Target != null) {
			return;
		}


		float newX = Position.X + Rand.RandiRange(-200, 200);
		float newY = Position.Y + Rand.RandiRange(-200, 200);
		NavAgent.TargetPosition = new Vector2(newX, newY);


		PatrolTimer.WaitTime = Rand.Randf() * 3 + 2;
		PatrolTimer.Start();

	}

}


