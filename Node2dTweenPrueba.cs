using Godot;
using System;

public partial class Node2dTweenPrueba : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var sprite = GetNode<Sprite2D>("Sprite2D");
		var tween = CreateTween();
		tween.TweenProperty(sprite, "position", new Vector2(200, 200), 1.0f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
