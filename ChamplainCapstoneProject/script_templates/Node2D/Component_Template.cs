// meta-name: Component Template
// meta-description: Predefined template for Component derived classes
// meta-default: true
// meta-space-indent: 4

using Godot;
using System;
using System.Security.Principal;


public partial class ClassName : Component
{
	public override void _Ready()
	{
		base._Ready();
		Subscribe_To_Events();
	}
	// end _Ready()

	public override void Step(double deltaT)
	{

	}
	// end Step()

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
