namespace ProyectoInventario.scripts;


using Godot;
using System;

public partial class GameManager : Node2D
{
	
	public static GameManager Instance { get; private set; }
	
	private Party _playerParty;
	private Party _enemyParty;
	
	private Random _random = new Random();
	
	[Export] private PackedScene _battleScene = GD.Load<PackedScene>("scenes/battle.tscn");
	
	
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
		_playerParty.AddMember(new Character("Avlora", 100, 8, 20), 0);
		_playerParty.AddMember(new Character("Gustadolph", 100, 15, 10), 1);
		_playerParty.AddMember(new Character("Frederica", 50, 20, 5), 2);
		
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
		_enemyParty.AddMember(new Character("Captain", 100, 10, 8), 0);
		_enemyParty.AddMember(new Character("Grunt1", 40, 10, 5), 1);
		_enemyParty.AddMember(new Character("Grunt2", 40, 10, 5), 2);
	}

	private void StartBattle()
	{
		if (_battleScene != null)
		{
			var battleInstance = _battleScene.Instantiate<Battle>();
			
			battleInstance.SetParties(_playerParty, _enemyParty);

			GetTree().ChangeSceneToFile("scenes/battle.tscn");
		}
		else
		{
			GD.Print("There is no battle scene assigned.");
		}
	}
}
