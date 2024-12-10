using System.Collections.Generic;
using System.Linq;

namespace ProyectoInventario.scripts;

using Godot;
using System;

public partial class Party : Node2D
{
	private PackedScene[] _memberScenes;
	public PackedScene[] MemberScenes { get; set; }
	
	private List<Character> _members = new List<Character>();
	public List<Character> Members => new List<Character>(_members);
	private const int MaxPartySize = 4;

	public Party()
	{
		MemberScenes = new PackedScene[0];
	}

	public void AddMember(PackedScene scene)
	{
		var instance = scene.Instantiate();
		
		if (instance is Character characterInstance)
		{
			if (_members.Count < MaxPartySize) 
			{
				_members.Add(characterInstance);
				GD.Print("Personaje añadido a la party correctamente.");
			}
			else
			{
				GD.PrintErr("No se puede añadir más personajes. La party está llena.");
			}
		}
		else
		{
			GD.PrintErr("Error: La escena instanciada no es un Character.");
		}
	}

	public List<Character> GetMembers()
	{
		return _members.Where(member => member != null).ToList();
	}
	
	public bool IsPartyAlive()
	{
		foreach (Character member in _members)
		{
			if (member != null && member.IsAlive) { return true; }
		}
		return false;
	}

	public Character GetRandomAliveMember()
	{
		var aliveMembers = _members.Where(member => member != null && member.IsAlive).ToList();

		if (aliveMembers.Count > 0)
		{
			int randomIndex = (int)(GD.Randi() % (uint)aliveMembers.Count);
			return aliveMembers[randomIndex];
		}

		GD.PrintErr("No alive members found!");
		return null;
	}
}
