using Godot;
using System;

public partial class Rover_View : Control
{
	public Rover Get_Rover()
	{
		return GetNode("%World")
			.GetNode<Rover>("%Rover");
	}
}
