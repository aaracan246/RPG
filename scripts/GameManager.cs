namespace ProyectoInventario.scripts;


using Godot;
using System;

public partial class GameManager : Node2D
{
	
	public static GameManager Instance { get; private set; }
	
	private Party _playerParty;
	public Party PlayerParty => _playerParty;
	
	private Party _enemyParty;
	
	private Random _random = new Random();
	
	[Export] private PackedScene _battleScene = GD.Load<PackedScene>("res://scenes/battle.tscn");

	// PJs
	[Export] private PackedScene _avloraScene = GD.Load<PackedScene>("res://scenes/Avlora.tscn");
	[Export] private PackedScene _fredericaScene = GD.Load<PackedScene>("res://scenes/Frederica.tscn");
	[Export] private PackedScene _gustadolphScene = GD.Load<PackedScene>("res://scenes/Gustadolph.tscn");
	
	public PackedScene AvloraScene => _avloraScene;
	public PackedScene FredericaScene => _fredericaScene;
	public PackedScene GustadolphScene => _gustadolphScene;


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

		// Instancia los personajes antes de añadirlos a la party
		var avlora = _avloraScene.Instantiate<Character>();
		avlora.Initialize("Avlora", 150, 20, 10, 5); 

		var frederica = _fredericaScene.Instantiate<Character>();
		frederica.Initialize("Frederica", 100, 15, 5, 7); 

		var gustadolph = _gustadolphScene.Instantiate<Character>();
		gustadolph.Initialize("Gustadolph", 120, 18, 8, 6); 

		// Añade los personajes a la party
		_playerParty.AddMember(avlora);
		_playerParty.AddMember(frederica);
		_playerParty.AddMember(gustadolph);
		
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
		GD.Print("Generando party enemiga...");
		
		var enemy1 = Instance.AvloraScene?.Instantiate<Character>();
		if (enemy1 != null)
		{
			enemy1.Initialize("Avlora", 120, 15, 8, 6);
			_enemyParty.AddMember(enemy1);
			GD.Print("Personaje añadido a la party correctamente: Enemy Avlora");
		}
		else
		{
			GD.PrintErr("Error al cargar Enemy Avlora.");
		}

		var enemy2 = Instance.FredericaScene?.Instantiate<Character>();
		if (enemy2 != null)
		{
			enemy2.Initialize("Frederica", 100, 15, 5, 7);
			_enemyParty.AddMember(enemy2);
			GD.Print("Personaje añadido a la party correctamente: Enemy Frederica");
		}
		else
		{
			GD.PrintErr("Error al cargar Enemy Frederica.");
		}

		var enemy3 = Instance.GustadolphScene?.Instantiate<Character>();
		if (enemy3 != null)
		{
			enemy3.Initialize("Gustadolph", 120, 18, 8, 6);
			_enemyParty.AddMember(enemy3);
			GD.Print("Personaje añadido a la party correctamente: Enemy Gustadolph");
		}
		else
		{
			GD.PrintErr("Error al cargar Enemy Gustadolph.");
		}
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
