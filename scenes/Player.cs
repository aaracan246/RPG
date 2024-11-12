namespace ProyectoInventario.scenes;


using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed;

	private AnimatedSprite2D _animatedSprite2D;

	public override void _Ready()
	{
		// Esto obtiene el nodo AnimatedSprite2D:
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

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

		ControlAnimation(direction);
	}

	private void ControlAnimation(Vector2 direction)
	{
		
		if (direction.Length() > 0)
		{
			// Arriba
			if (direction.Y < 0)
			{
				_animatedSprite2D.Play("moveUp");
				_animatedSprite2D.FlipH = direction.X > 0; 
			}
			// Abajo
			else if (direction.Y > 0)
			{
				_animatedSprite2D.Play("moveLeft");
				_animatedSprite2D.FlipH = direction.X > 0; 
			}
			// Izquierda y Derecha
			else if (direction.X != 0)
			{
				_animatedSprite2D.Play("moveLeft");
				_animatedSprite2D.FlipH = direction.X > 0; 
			}
		}
		else
		{
			// No movimiento: Idle
			if (_animatedSprite2D.Animation != "idle")
			{
				_animatedSprite2D.Play("idle");
			}
		}
	}
}

