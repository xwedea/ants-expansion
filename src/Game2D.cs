using Godot;
using System;

public partial class Game2D : Node2D
{

	protected Camera2D GameCamera;
	private const int edgeOffset = 20; 
	public int FoodStock = 50;


	public override void _Ready()
	{
		GameCamera = GetNode<Camera2D>("Camera2D");
	}

	public override void _Process(double delta)
	{
		MoveCamera(delta);
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


}
