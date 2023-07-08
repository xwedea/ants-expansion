// using Godot;
// using System;
// using System.Collections.Generic;

// public partial class AntBase : CharacterBody2D
// {
// 	private NavigationAgent2D NavAgent;
// 	private Rid NavMap;
// 	private Rid NavRegion;
// 	private List<Vector2> NavPath = new List<Vector2>();

// 	public const float Speed = 50;
	
// 	public override void _Ready()
// 	{
// 		base._Ready();

// 		// CallDeferred("SetupNavServer");	
// 		SetupNavServer();
// 	}

// 	public void SetupNavServer() {
// 		NavMap = NavigationServer2D.MapCreate();
// 		NavigationServer2D.MapSetActive(NavMap, true);

// 		NavRegion = NavigationServer2D.RegionCreate();
// 		NavigationServer2D.RegionSetTransform(NavRegion, new Transform2D());
// 		NavigationServer2D.RegionSetMap(NavRegion, NavMap);

// 		NavigationRegion2D existingNavRegion = GetNode<NavigationRegion2D>("../NavigationPolygon2D");
// 		NavigationPolygon existingNavPolygon = new NavigationPolygon();
// 		existingNavPolygon = existingNavRegion.NavigationPolygon;


// 		// Vector2[] outline = { new Vector2(0, 0), new Vector2(1000, 0), new Vector2(1000,1000), new Vector2(0, 1000) }; 

// 		NavigationServer2D.RegionSetNavigationPolygon(NavRegion, existingNavPolygon);

// 	}

// 	public void UpdateNavigationPath(Vector2 startPosition, Vector2 targetPosition) {
// 		NavPath = new List<Vector2>(NavigationServer2D.MapGetPath(NavMap, startPosition, targetPosition, true)); 
// 		// NavPath.RemoveAt(0);

// 		GD.Print(NavPath.Count);

// 		// for (int i = 0; i < NavPath.Count; i++) {
// 		// 	GD.Print(NavPath[i].ToString());
// 		// }
// 	}

// 	public override void _Process(double delta)
// 	{
// 		double walkDistance = Speed * (float)delta;
// 		MoveAlongPath((float) walkDistance);
// 	}

// 	public void MoveAlongPath(float distance) {
// 		// Vector2 currentPosition = Position;
		
// 		// while (NavPath.Count > 0) {
// 		// 	float distanceBetweenPoints = currentPosition.DistanceTo(NavPath[0]);

// 		// 	if (distance <= distanceBetweenPoints) {
// 		// 		Position = currentPosition.Lerp(NavPath[0], distance/distanceBetweenPoints);
// 		// 		return;
// 		// 	}

// 		// 	distance -= distanceBetweenPoints;
// 		// 	currentPosition = NavPath[0];
// 		// 	NavPath.RemoveAt(0);
// 		// }

// 		// Position = currentPosition;
// 		// SetProcess(false);
// 	}

// 	public override void _UnhandledInput(InputEvent @event)
// 	{
// 		if (Input.IsActionJustPressed("MoveTo")) {
// 			UpdateNavigationPath(Position, GetGlobalMousePosition());
// 			// GD.Print(GetGlobalMousePosition());
// 		}
// 	}

// }