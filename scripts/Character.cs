namespace ProyectoInventario.scripts;

using Godot;
using System;

public partial class Character : Node2D
{
	public string PjName { get; set; }
	public int MaxHp { get; set; }
	public int CurrentHp { get; set; }
	public int Attack { get; set; }
	public int Armor { get; set; }
	public int Speed { get; set; }
	public bool IsDefending { get; set; }

	public bool IsAlive => CurrentHp > 0;

	public Character()
	{
	}

	public void Initialize(string name, int maxHp, int attack, int armor, int speed)
	{
		PjName = name;
		MaxHp = maxHp;
		CurrentHp = maxHp;
		Attack = attack;
		Armor = armor;
		Speed = speed;
		IsDefending = false;
	}

	public int CalculateDamage(int opponentArmor)
	{
		return Math.Max(CurrentHp - opponentArmor, 1);
	}

	public void ReceiveDamage(int damage)
	{
		if (IsDefending)
		{
			damage /= 2;
		}
		CurrentHp = Math.Max(CurrentHp - damage, 0);
		IsDefending = false;
	}
    
	public void Defend()
	{
		IsDefending = true;
	}
}