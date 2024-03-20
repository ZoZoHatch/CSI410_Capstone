using Godot;
using System;
using System.Security.Principal;


public partial class Follow_Rover : Camera2D
{
	Rover rvr;
	public override void _Ready()
	{
		base._Ready();	
		rvr = GetParent().GetNode<Rover>("%Rover");	
	}
    // end _Ready()

    public override void _Process(double delta)
    {
        base._Process(delta);
		Position = rvr.Position;
    }
	// end _Process()

}

