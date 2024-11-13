namespace ProyectoInventario.scripts;

using Godot;
using System;

public partial class Character : Node2D
{
	public string PjName { get; set; }
	public int MaxHp { get; private set; }
	public int CurrentHp { get; set; }
	public int Attack { get; set; }
	public int Armor { get; set; }
	public int Speed { get; set; }
	public bool IsAlive => CurrentHp > 0;
	public bool IsDefending { get; set; }


	public Character(string name, int maxHp, int attack, int armor)
	{

		PjName = name;
		MaxHp = maxHp;
		CurrentHp = maxHp;
		Attack = attack;
		Armor = armor;
		IsDefending = false;
	}

	public int CalculateDamage(int opponentArmor)
	{
		return Math.Max(CurrentHp - opponentArmor, 1); // Daño mínimo 1 para que aunque se pase no haya daños negativos o 0
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
	
	public void Defend(){ IsDefending = true; }
}