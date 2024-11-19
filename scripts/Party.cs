namespace ProyectoInventario.scripts;

using Godot;
using System;

public partial class Party : Node2D
{
	private Character[] _members;
	private const int MaxPartySize = 4;

	public Party()
	{
		_members = new Character[MaxPartySize];
	}

	public void AddMember(Character character, int position)
	{
		if (position >= 0 && position < MaxPartySize)
		{
			_members[position] = character;
		}
	}

	public bool IsPartyAlive()
	{
		foreach (Character member in _members)
		{
			if (member != null && member.IsAlive) { return true; }
		}
		return false;
	}

	public Character GetRandomAliveMember() // Esto para los enemigos
	{
		var aliveMembers = Array.FindAll(_members, member => member != null && member.IsAlive);
		if (aliveMembers.Length > 0)
		{
			return aliveMembers[GD.Randi() % aliveMembers.Length]; // Esto da un número dentro del array | Randi -> Random
		}

		return null;
	}
}
