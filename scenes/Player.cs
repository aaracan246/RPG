namespace ProyectoInventario.scenes;


using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Vector2.Zero;
		direction.X = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
		direction.Y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");

		if (Mathf.Abs(direction.X) == 1 && Mathf.Abs(direction.Y) == 1)
		{
			direction = direction.Normalized();
		}
		
		Vector2 movement = Speed * direction * (float)delta;

		MoveAndCollide(movement);


	}

}
