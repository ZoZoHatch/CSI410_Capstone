using Godot;
using System;
using System.Security.Principal;


public partial class Center_Camera : Camera2D
{
	public override void _Ready()
	{
		Position = GetParent<SubViewport>().Size / 2;
	}
	// end _Ready()		
}

