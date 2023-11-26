using Godot;
using System;

public partial class Mission_Control : Node2D
{
	Terminal_Output tml_Terminal;		
	Rover rvr_Rover;

	/*
	// Signals
	*/	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	// end _Process
	
	/*
	// signal handling
	*/		
	private void _on_key_pad_code_entered(string str_Input)
	{
		tml_Terminal.print_message(str_Input);
		contact_rover(str_Input);
	}
	// end _on_key_pad_code_entered()
	
	private void _on_terminal_output_ready()
	{
		tml_Terminal = GetNode<Terminal_Output>("Terminal_Output");
	}
	// end _on_terminal_output_ready()
	
	private void _on_rover_ready()
	{
		rvr_Rover = GetNode<Rover>("Rover");
	}
	// end _on_rover_ready

	/*
	// command pre-processing
	*/	
	private void contact_rover(string str_command)
	{
		if(str_command.Length < 2)
		{
			tml_Terminal.print_message("command is too short");
			return;
		}		

		rvr_Rover.process_packet(str_command);
	}
	// end contact_rover

}












