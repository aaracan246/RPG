using System.Linq;

namespace ProyectoInventario.scripts;

using Godot;
using System;
using System.Collections.Generic;

public partial class Battle : Node2D
{
    private Party _playerParty;
    private Party _enemyParty;
    private int _currentTurn = 0;

    private Node2D _teamPlayerNode;
    private Node2D _teamEnemyNode;

    private enum BattleState
    {
        PlayerTurn,
        EnemyTurn,
        WaitingForInput,
        BattleOver
    }

    private BattleState _currentState;

    public override void _Ready()
    {
        
        _teamPlayerNode = GetNodeOrNull<Node2D>("TeamPlayer");
        _teamEnemyNode = GetNodeOrNull<Node2D>("TeamEnemy");

        if (_teamPlayerNode == null || _teamEnemyNode == null)
        {
            GD.PrintErr("No se encontraron los nodos TeamPlayer o TeamEnemy en la escena de batalla.");
            return;
        }

        GD.Print("Battle ready. Preparing parties...");
        
        // Crear las parties
        _playerParty = CreateParty(true);
        _enemyParty = CreateParty(false);

        GD.Print($"Player members count: {_playerParty.Members.Count}");
        GD.Print($"Enemy members count: {_enemyParty.Members.Count}");

        SetParties(_playerParty, _enemyParty);
    }

    
    private Party CreateParty(bool isPlayerParty)
    {
        var party = new Party();

        if (isPlayerParty)
        {
            GD.Print("Cargando recurso para el jugador...");
            var playerScene = GD.Load<PackedScene>("res://scenes/player_party.tscn");

            if (playerScene != null)
            {
                party.AddMember(playerScene);
            }
            else
            {
                GD.PrintErr("Fallo al cargar el recurso de jugador.");
            }
        }
        else
        {
            GD.Print("Cargando recurso para el enemigo...");
            var enemyScene = GD.Load<PackedScene>("res://scenes/enemy_party.tscn");

            if (enemyScene != null)
            {
                party.AddMember(enemyScene);
            }
            else
            {
                GD.PrintErr("Fallo al cargar el recurso de enemigo.");
            }
        }

        return party;
    }

    public void SetParties(Party playerParty, Party enemyParty)
{
    GD.Print("Setting parties...");
    
    _playerParty = playerParty;
    _enemyParty = enemyParty;

    if (_playerParty == null)
    {
        GD.PrintErr("Player party is null.");
    }

    if (_enemyParty == null)
    {
        GD.PrintErr("Enemy party is null.");
    }

    if (_teamPlayerNode != null && _teamEnemyNode != null)
    {
        GD.Print("Loading player and enemy teams...");
        GD.Print($"Player party members count: {_playerParty?.Members.Count}");
        GD.Print($"Enemy party members count: {_enemyParty?.Members.Count}");
        
        LoadTeam(_teamPlayerNode, _playerParty.GetMembers(), false);
        LoadTeam(_teamEnemyNode, _enemyParty.GetMembers(), true);
    }
    else
    {
        GD.PrintErr("TeamPlayer or TeamEnemy nodes are missing.");
    }
}

private void LoadTeam(Node2D container, List<Character> members, bool flip)
{
    GD.Print($"Loading team members into {container.Name}...");
    
    if (members == null || members.Count == 0)
    {
        GD.PrintErr("No members available to load into the team.");
        return;
    }

    for (int i = 0; i < members.Count; i++)
    {
        GD.Print($"Processing member index {i} for loading...");
        
        var member = members[i];
        if (member == null)
        {
            GD.PrintErr($"Member at index {i} is null.");
            continue;
        }

        GD.Print($"Loading scene for: {member.PjName}");
        var characterScene = GD.Load<PackedScene>(member.SceneFilePath);

        if (characterScene == null)
        {
            GD.PrintErr($"Could not load scene from: {member.SceneFilePath}");
            continue;
        }

        var characterInstance = characterScene.Instantiate<Character>();

        if (characterInstance == null)
        {
            GD.PrintErr($"Could not instantiate scene for {member.PjName}");
            continue;
        }

        GD.Print($"Instantiating character: {member.PjName}");
        characterInstance.PjName = member.PjName;
        characterInstance.MaxHp = member.MaxHp;
        characterInstance.CurrentHp = member.CurrentHp;
        characterInstance.Attack = member.Attack;
        characterInstance.Armor = member.Armor;
        characterInstance.Speed = member.Speed;

        container.AddChild(characterInstance);
        characterInstance.Position = new Vector2(i * 100, 0);
        
        var spriteNode = characterInstance.GetNodeOrNull<Sprite2D>("Sprite2D");
        if (spriteNode == null)
        {
            GD.PrintErr($"Could not find 'Sprite2D' node for {member.PjName}. Check the scene structure.");
        }
        else
        {
            if (flip)
            {
                GD.Print($"Flipping enemy sprite: {member.PjName}");
                spriteNode.FlipH = true;
            }
            else
            {
                GD.Print($"Setting player sprite to face right: {member.PjName}");
                spriteNode.FlipH = false; 
            }
        }
    }
}
    private void TakeTurn()
    {
        GD.Print("Processing turn...");
        
        if (!_playerParty.IsPartyAlive() || !_enemyParty.IsPartyAlive())
        {
            GD.Print(_playerParty.IsPartyAlive() ? "You won!" : "You lose...");
            _currentState = BattleState.BattleOver;
            GameManager.Instance.EndBattle();
            return;
        }

        if (_currentState == BattleState.PlayerTurn)
        {
            ShowPlayerMenu();
        }
        else if (_currentState == BattleState.EnemyTurn)
        {
            TakeEnemyTurn();
            _currentTurn++;
            _currentState = BattleState.PlayerTurn;
            TakeTurn();
        }
    }

    private void ShowPlayerMenu()
    {
        GD.Print("Choose an option:");
        GD.Print("1. Attack");
        GD.Print("2. Skill");
        GD.Print("3. Guard");
        GD.Print("4. Item");
        GD.Print("5. Pass turn");

        _currentState = BattleState.WaitingForInput;
    }

    private void TakeEnemyTurn()
    {
        GD.Print("Enemy is taking its turn...");
    }
}