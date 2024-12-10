namespace ProyectoInventario.scripts;


using Godot;
using System;

public partial class GameManager : Node2D
{
	
	public static GameManager Instance { get; private set; }
	
	private Party _playerParty;
	private Party _enemyParty;
	
	private Random _random = new Random();
	
	[Export] private PackedScene _battleScene = GD.Load<PackedScene>("res://scenes/battle.tscn");

	// PJs
	[Export] private PackedScene _avloraScene = GD.Load<PackedScene>("res://scenes/Avlora.tscn");
	[Export] private PackedScene _fredericaScene = GD.Load<PackedScene>("res://scenes/Frederica.tscn");
	[Export] private PackedScene _gustadolphScene = GD.Load<PackedScene>("res://scenes/Gustadolph.tscn");


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			QueueFree();
		}

		InitPlayerParty();
	}

	private void InitPlayerParty() {
		
		_playerParty = new Party();

		
		_playerParty.AddMember(_avloraScene);
		_playerParty.AddMember(_fredericaScene);
		_playerParty.AddMember(_gustadolphScene);
		
	}

	public void CheckForRandomEncounter(float distanceTraveled)
	{
		float chance = 0.2f;

		if (_random.Next() < chance)
		{
			GenerateEnemyParty();
			StartBattle();
		}
	}


	public void EndBattle()
	{
		GetTree().ChangeSceneToFile("scenes/Main.tscn");
	}

	private void GenerateEnemyParty()
	{
		_enemyParty = new Party();
		//_enemyParty.AddMember(new Character("Captain", 100, 10, 8), 0);
		//_enemyParty.AddMember(new Character("Grunt1", 40, 10, 5), 1);
		//_enemyParty.AddMember(new Character("Grunt2", 40, 10, 5), 2);
		
		_enemyParty.AddMember(_avloraScene);
		_enemyParty.AddMember(_fredericaScene);
		_enemyParty.AddMember(_gustadolphScene);
	}

	private void StartBattle()
	{
		if (_battleScene != null)
		{
			var battleInstance = _battleScene.Instantiate<Battle>();

			if (battleInstance != null)
			{
				battleInstance.SetParties(_playerParty, _enemyParty);
				
				GetTree().Root.AddChild(battleInstance); 
				GetTree().SetCurrentScene(battleInstance);
			}
			else
			{
				GD.PrintErr("Error al instanciar la batalla.");
			}
		}
		else
		{
			GD.PrintErr("No hay una escena de batalla asignada en GameManager.");
		}
	}
}
