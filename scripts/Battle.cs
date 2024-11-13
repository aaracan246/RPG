namespace ProyectoInventario.scripts;

using Godot;
using System;
using System.Collections.Generic;

public partial class Battle : Node2D
{
    private Party _playerParty;
    private Party _enemyParty;
    private int _currentTurn = 0;

    private enum BattleState { PlayerTurn, EnemyTurn, WaitingForInput, BattleOver }
    private BattleState _currentState;

    public Battle(Party playerParty, Party enemyParty)
    {
        this._playerParty = playerParty;
        this._enemyParty = enemyParty;
        _currentState = BattleState.PlayerTurn;
    }

    public void StartBattle()
    {
        GD.Print("It begins!");
        while (_playerParty.IsPartyAlive() && _enemyParty.IsPartyAlive())
        {
            TakeTurn();
        }

        GD.Print(_playerParty.IsPartyAlive() ? "You won!" : "You lose...");
    }

    private void TakeTurn()
    {
        if (_currentState == BattleState.BattleOver)
            return;

        if (_currentState == BattleState.PlayerTurn)
        {
            ShowPlayerMenu();
        }
        else if (_currentState == BattleState.EnemyTurn)
        {
            TakeEnemyTurn();
            _currentTurn++;
            _currentState = BattleState.PlayerTurn;
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

    public void HandlePlayerInput(int choice)
    {
        var player = _playerParty.GetRandomAliveMember();

        switch (choice)
        {
            case 1: // Atacar
                var target = _enemyParty.GetRandomAliveMember(); // Meter aquí selector de enemigos
                if (target != null)
                    Attack(player, target);
                break;

            case 2: // Habilidad (placeholder)
                GD.Print($"{player.Name} uses a skill");
                break;

            case 3: // Protegerse
                player.Defend();
                GD.Print($"{player.Name} is defending herself.");
                break;

            case 4: // Item (placeholder)
                GD.Print($"{player.Name} uses an item.");
                break;

            case 5: // Pasar turno
                GD.Print($"{player.Name} does nothing.");
                break;

            default:
                GD.Print("Select a valid option.");
                return;
        }

        _currentState = BattleState.EnemyTurn;
    }

    private void TakeEnemyTurn()
    {
        var attacker = _enemyParty.GetRandomAliveMember();
        var target = _playerParty.GetRandomAliveMember();

        if (attacker != null && target != null)
        {
            Attack(attacker, target);
        }
    }

    private void Attack(Character attacker, Character target)
    {
        int damage = attacker.CalculateDamage(target.Armor);
        target.ReceiveDamage(damage);
        GD.Print($"{attacker.Name} attacks {target.Name} and deals {damage} points of damage.");
    }
}