using Godot;
using System;
using System.Security.Principal;


public partial class Collector : Component
{	
	private Node2D n2d_Anchor;	// the Transform fror the Collector_Pos on the Arm compoent
	private Arm arm;

	public override void _Ready()
	{
		base._Ready();

		arm = GetParent().GetNode<Arm>("Arm");

		// instantiate t2d_Anchor
		n2d_Anchor = arm.GetNode<Node2D>("%Collector_Anchor");		

		Subscribe_To_Events();
	}
	// end _Ready()

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		
		GlobalTransform = n2d_Anchor.GlobalTransform;
	}

	public override bool Process_Command(string command)
	{
		if(base.Process_Command(command))
		{
			return true;	// Return true if command was handled by bas component
		}

		EmitSignal(SignalName.CommandNotRecognized, str_Sender);
		return false;	// Command not processed
	}	
	// end Process_Command()

	protected override void Subscribe_To_Events()
	{
		base.Subscribe_To_Events();
		
		
	}
	// end Subscribe_To_Events()

	protected override void Load_Config_File()
	{

	}
	// end Load_Config_File()
	
}

