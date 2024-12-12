using Godot;
using System;

public partial class Node2dTweenPrueba : Node2D
{

	
	private double _moveTimer = .4f;

	private double _moveTimerReset = .4f;

	private bool _moving = false;
	
	private double _attackTimer = .2f;

	private double _attackTimerReset = .2f;
	
	private bool _attacking = false;
	
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (GetTree().Root.GetNode<Node2D>("Node2D").GetNode<Button>("Button").IsPressed())
		{
			_moving = true;
			var sprite = GetNode<AnimatedSprite2D>("Sprite2D");
			var tween = CreateTween();
			tween.TweenProperty(sprite, "position", new Vector2(200, 0), .4f);
			if (_moving)
			{
				
				_moveTimer -= delta;
				if (_moveTimer <= 0)
				{
					_moving = false;
					_moveTimer = _moveTimerReset;
				}
			}
			_attacking = true;
			if (_attacking)
			{
				var animatedSprite = GetNode<AnimatedSprite2D>("Sprite2D");
				animatedSprite.Play("DownwardSlash");
				_attackTimer -= delta;
				if (_attackTimer <= 0)
				{
					_attacking = false;
					_attackTimer = _attackTimerReset;
				}
			}
			tween.TweenProperty(sprite, "position", new Vector2(-200, 0), 1.0f);
			
		}
		
	}
}
