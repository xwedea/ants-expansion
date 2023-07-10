using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Game2D : Node2D
{
	public AudioStreamPlayer2D atmosphereAudio;
	public AudioStreamPlayer2D victoryAudio;

	protected Camera2D GameCamera;
	private const int edgeOffset = 20; 
	public int FoodStock = 50;
	
	public bool IsSelectingArea;
	public Vector2 SelectedTopLeft;
	public List<AntBase> SelectedAnts;

	public string baseAntColor = "ffffff";
	public string selectAntColor = "a30000";

	public override void _Ready()
	{
		atmosphereAudio = GetNode<AudioStreamPlayer2D>("AtmosphereAudio");
		victoryAudio = GetNode<AudioStreamPlayer2D>("VictoryAudio");

		GameCamera = GetNode<Camera2D>("Camera2D");
		SelectedAnts = new List<AntBase>();

		atmosphereAudio.Play(10);
		
	}

	public override void _Process(double delta)
	{
		MoveCamera(delta);

		Array<Node> enemies = GetTree().GetNodesInGroup("enemy");
		// GD.Print(enemies.Count);

		if (enemies.Count == 8) {
			victoryAudio.Play();
		}

	}

	private void MoveCamera(double delta)
    {
    	Vector2 mousePosition = GetViewport().GetMousePosition();
		Vector2 viewSize = GetViewportRect().Size;

		Vector2 direction = Vector2.Zero;
        if (mousePosition.X < edgeOffset){
            direction += new Vector2(-1, 0);
        }
        else if (mousePosition.X > viewSize.X - edgeOffset) {
            direction += new Vector2(1, 0);
        }

        if (mousePosition.Y < edgeOffset) {
            direction += new Vector2(0, -1);
        }
        else if (mousePosition.Y > viewSize.Y - edgeOffset) {
            direction += new Vector2(0, 1);
        }

		GameCamera.Offset += direction * GameCamera.Zoom * 800 * (float) delta;
    }

	
	public void ChangeFoodStock(int amount) {
		FoodStock += 10;
		GD.Print(FoodStock);
	}


	public override void _UnhandledInput(InputEvent @event)
	{
		Vector2 mousePos= GetGlobalMousePosition();

		if (Input.IsActionJustPressed("LeftClick")) {
			PhysicsPointQueryParameters2D query = new PhysicsPointQueryParameters2D();
			query.Position = mousePos;
			PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
			Array<Godot.Collections.Dictionary> collisionObjects = spaceState.IntersectPoint(query, 1);

			Variant value;
			Node2D collisionNode = null;

			
			if (collisionObjects.Count > 0 && collisionObjects[0].TryGetValue("collider", out value) ) {
				collisionNode = (Node2D) value.Obj;
			}

			if (collisionNode != null) {

				if (collisionNode.IsInGroup("own")) {
					AntBase ant = (AntBase) collisionNode;
					if (ant != null) {
						SetAntColors(baseAntColor);
						SelectedAnts.Clear();
						SelectedAnts.Add(ant);
						SetAntColors(selectAntColor);
						return;
					}

				}
				else if (SelectedAnts.Count > 0) {
					if (collisionNode == null) return;

					if (collisionNode.IsInGroup("resource") || collisionNode.IsInGroup("enemy")) {
						foreach (AntBase selectAnt in SelectedAnts) {
							selectAnt.SetTarget(collisionNode);	
						}
						SetAntColors(baseAntColor);
						SelectedAnts.Clear();

					}
					else {
						foreach (AntBase selectAnt in SelectedAnts) {
							selectAnt.MoveToLocation(mousePos);
						}
					}

				}


			}
			else { // nothing in mouse position

				if (SelectedAnts.Count > 0) {
					foreach (AntBase ant in SelectedAnts) {
						ant.MoveToLocation(mousePos);
					}
				}
				else {
					SetAntColors("ffffff");
					SelectedAnts.Clear();
				}



			}

		}

		else if (Input.IsActionJustPressed("RightClick")) {
			SetAntColors("ffffff");
			SelectedAnts.Clear();
		}

	}


	private void SetAntColors(string color) {
		foreach (AntBase ant in SelectedAnts) {
			ant.SetColor(color);
		}
	}

}
