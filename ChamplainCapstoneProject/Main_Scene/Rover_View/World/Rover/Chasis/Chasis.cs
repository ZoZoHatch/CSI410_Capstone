using Godot;
using Microsoft.VisualBasic;
using System;


public partial class Chasis : Component
{
	float flt_Max_Health = 100.0f;
	float flt_Current_Health;

	public override void _Ready()
	{
		base._Ready();

		// Intialize nrg_Power.int_Max_Energy
		nrg_Power.Init_Max_Energy(1); 

		// Set nrg_Power.int_Min_Energy
		nrg_Power.Set_Min_Energy(0);

		flt_Current_Health = flt_Max_Health;

		Subscribe_To_Events();
	}
	// end _Ready()
	

	public override bool Process_Command(string command)
	{
		if(base.Process_Command(command))
		{
			return true;	// Return true if command was handled by base component
		}

		/*
		// Chasis's only function is to the potect the Rover's other components.
		//
		// Sending any command will print the current integrity of the Chasis.
		*/

		Print_Integrity();		

		EmitSignal(SignalName.CommandProcessed, str_Sender, command);
		return true;	// Command processed
	}	
	// end Process_Command()

	protected override void Subscribe_To_Events()
	{
		base.Subscribe_To_Events();
		GetParent().GetNode<Motor>("Motor").RoverCollided += (float vel) =>
			Handle_Collision(vel);
	}
	// end Subscribe_To_Events()

	private void Handle_Collision(float col_velocity)
	{
		flt_Current_Health -= col_velocity;

		Rover rvr = GetParent<Rover>();
		if(flt_Current_Health < 0f)	// if health is negative, the rover is disabled
		{
			rvr.Disable_Rover();
			rvr.Send_Emergency_Message(str_Sender, 
				$"Chasis has taken too much damage, the Rover can no longer continue functioning");
		}
		else
		{
			rvr.Send_Emergency_Message(str_Sender, $"Chasis protected the Rover from collision");
			Print_Integrity();
		}
	}
	// end Handle_Velocity()

	private void Print_Integrity()
	{
		int int_Integrity = (int)(flt_Current_Health / flt_Max_Health * 100);
		
		Rover rvr = GetParent<Rover>();
		rvr.Send_Message(str_Sender, $"Chasis's current integrity: {int_Integrity}%");
	}

}

