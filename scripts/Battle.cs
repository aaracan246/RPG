using System.Linq;

namespace ProyectoInventario.scripts;

using Godot;
using System;
using System.Collections.Generic;

public partial class Battle : Node2D
{
    private Camera2D _camera;
    
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
        
        LoadTeam(_teamPlayerNode, _playerParty.GetMembers(), true);
        LoadTeam(_teamEnemyNode, _enemyParty.GetMembers(), false);
    }
    else
    {
        GD.PrintErr("TeamPlayer or TeamEnemy nodes are missing.");
    }
}

private Party CreateParty(bool isPlayerParty)
{
    if (isPlayerParty)
    {
        GD.Print("Usando party del jugador inicializada por GameManager...");
        return GameManager.Instance.PlayerParty;
    }
    else
    {
        GD.Print("Generando party enemiga...");
        var enemyParty = new Party();

        
        var enemy1 = GameManager.Instance.AvloraScene.Instantiate<Character>();
        enemy1.Initialize("Enemy Avlora", 120, 15, 8, 6); // Inicialización de Avlora
        enemyParty.AddMember(enemy1);

        var enemy2 = GameManager.Instance.FredericaScene.Instantiate<Character>();
        enemy2.Initialize("Enemy Frederica", 120, 15, 8, 6); // Inicialización de Frederica
        enemyParty.AddMember(enemy2);
        
        var enemy3 = GameManager.Instance.GustadolphScene.Instantiate<Character>();
        enemy3.Initialize("Enemy Gustadolph", 120, 15, 8, 6); // Inicialización de Gustadolph
        enemyParty.AddMember(enemy3);

        return enemyParty;
    }
}

private void LoadTeam(Node2D container, List<Character> members, bool flip) // Esto podría ir en una clase separada
{
    GD.Print($"Loading team members into {container.Name}...");
    
    if (members == null || members.Count == 0)
    {
        GD.PrintErr("No members available to load into the team.");
        return;
    }

    const float verticalSpacing = 80.0f; 
    Vector2 startPosition = new Vector2(0, 0); 

    var vboxContainer = GetNode<Control>("ButtonContainer"); // Nodo para botones

    for (int i = 0; i < members.Count; i++)
    {
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

        characterInstance.Position = startPosition + new Vector2(0, i * verticalSpacing);
        container.AddChild(characterInstance);

        // Botón que ataca a este pj
        var attackButton = new Button();
        attackButton.Text = $"Attack {member.PjName}";
        attackButton.Name = $"Button_{member.PjName}";
        vboxContainer.AddChild(attackButton);
        
        attackButton.Pressed += () =>
        {
            StartAttackAnimation(_playerParty.Members[_currentTurn]);
        };

        var spriteNode = characterInstance.GetNodeOrNull<Sprite2D>("Sprite2D");

        if (spriteNode == null)
        {
            GD.PrintErr($"Could not find 'Sprite2D' node for {member.PjName}. Check the scene structure.");
        }
        else
        {
            spriteNode.FlipH = flip; // Flip según sea necesario
        }
        
        var animatedSpriteNode = characterInstance.GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        
        if (animatedSpriteNode == null)
        {
            GD.PrintErr($"Could not find 'AnimatedSprite2D' node for {member.PjName}. Check the scene structure.");
        }
        else
        {
            animatedSpriteNode.FlipH = flip; // Flip según sea necesario
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

    if (_currentState == BattleState.EnemyTurn)
    {
        TakeEnemyTurn();
        
        _currentTurn++;
        _currentState = BattleState.PlayerTurn;
        
        RemoveDefeatedChar(); // Check si murió alguien

        TakeTurn();
    }
}

    
    private void TakeEnemyTurn()
    {
        GD.Print("Enemy is taking its turn...");
    }
    
    // Funciones de combate:
    
    
    public void ProcessTurn()
    {
        GD.Print($"Processing turn for player {_currentTurn}");
    
        // Esperar a la entrada del jugador
        ShowTargetButtons();
    }
    
    private void ShowTargetButtons()
    {
        for (int i = 0; i < _enemyParty.Members.Count; i++)
        {
            var button = GetNode<Button>($"TargetButton_Enemy{i+1}");
            button.Visible = true; // Muestra el botón correspondiente
        }
    }
    
    private void HideTargetButtons()
    {
        for (int i = 0; i < _enemyParty.Members.Count; i++)
        {
            var button = GetNode<Button>($"TargetButton_Enemy{i+1}");
            button.Visible = false; // Oculta el botón después de seleccionar
        }
    }
    
    public void OnTargetButtonPressed(int enemyIndex)
    {
        HideTargetButtons(); // Oculta los botones tras la selección
        var attacker = _playerParty.Members[_currentTurn];
        var target = _enemyParty.Members[enemyIndex];
        StartAttackAnimation(attacker);
    }

    public void StartAttackAnimation(Character attacker)
    {
        var sprite = attacker.GetNode<Sprite2D>("Sprite2D");
        var tween = CreateTween();
        tween.TweenProperty(sprite, "position", new Vector2(200, 200), 1.0f);
    }

        private void EndTurn()
    {
        if (_enemyParty.IsPartyAlive())
        {
            _currentTurn++;
            _currentTurn %= _playerParty.Members.Count;
            _currentState = BattleState.PlayerTurn;
            TakeTurn();
        }
        else
        {
            GD.Print("Player wins!");
            _currentState = BattleState.BattleOver;
            GameManager.Instance.EndBattle();
        }
    }
    
    private void RemoveDefeatedChar()
    {
        foreach (var member in _enemyParty.GetMembers().ToList())
        {
            if (!member.IsAlive)
            {
                GD.Print($"{member.PjName} has been defeated!");
                _enemyParty.RemoveMember(member);
                
                var button = GetNodeOrNull<Button>($"ButtonContainer/Button_{member.PjName}");
                if (button != null)
                {
                    button.Disabled = true;
                    button.Text += " (Defeated)";
                }
            }
        }

        foreach (var member in _playerParty.GetMembers().ToList())
        {
            if (!member.IsAlive)
            {
                GD.Print($"{member.PjName} has been defeated!");
                _playerParty.RemoveMember(member);
            }
        }
    }
    
    private void ShowDamageText(Character target, int damage)
    {
        var damageLabel = new Label
        {
            Text = $"-{damage}",
            Modulate = new Color(1, 0, 0), // Texto rojo
            Scale = new Vector2(2, 2) // Escala grande
        };
        
        damageLabel.Position = target.Position + new Vector2(0, -25); // Falta Ajustar
        AddChild(damageLabel);

        
        var tween = GetTree().CreateTween();
        tween.TweenProperty(damageLabel, "position", damageLabel.Position + new Vector2(0, -50), 1.0f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(damageLabel, "modulate", new Color(1, 0, 0, 0), 1.0f); // "Eliminar" texto
        tween.TweenCallback(Callable.From(() => damageLabel.QueueFree())); // Liberar el nodo al terminar
    }
}