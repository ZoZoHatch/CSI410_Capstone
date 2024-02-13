using Godot;
using System;


public partial class Chasis : Component
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
			return true;	// Return true if command was handled by base component
		}

		EmitSignal(SignalName.CommandNotRecognized, str_Sender, command);
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

