using Godot;
using System;
using System.Diagnostics;
using System.Security.Principal;


public partial class GPS : Component
{
	private Rover rvr;	
	private Compass cps;

	private bool bl_Auto_Print_On_Move = false;
	private bool bl_Auto_Print_On_Turn = false;
	private bool bl_Auto_Print_On_Movement_Finished = false;	
	private bool bl_Print_to_Console = true;

	[Signal]
	public delegate void GPSLocationUpdatedEventHandler(string location, int precision, Vector2 loc_vector);
	[Signal]
	public delegate void GPSDirectionUpdatedEventHandler(string direction, Vector2 dir_vector);

	public override void _Ready()
	{
		base._Ready();

		rvr = GetParent<Rover>();		
		cps = GetNode<Compass>("Compass"); 		
				
		// Intialize nrg_Power.int_Max_Energy
		nrg_Power.Init_Max_Energy(3); 

		// Set nrg_Power.int_Min_Energy
		nrg_Power.Set_Min_Energy(1);

		Subscribe_To_Events();
	}
	// end _Ready()

	public override bool Process_Command(string command)
	{
		if(base.Process_Command(command))
		{
			return true;	// Return true if command was handled by bas component
		}
		
		/*
		// GPS only takes one number a time, and discards any extra numbers
		//
		// 0  -> Print the configuration of the GPS
		// 1 -> Print Rover location to terminal
		// 2 -> Print Rover direction to terminal
		// 3 -> Print both location and direction
		// 4 -> Toggle automatically printing location
		//		when the rover moves
		// 5 -> Toggle automatically printing direction
		//		when the rover moves
		// 6 -> Toggle printing when the rover is finished moving
		// 7 -> Toggle printing to the console
		//		(GPS will still update the Mini-Map)
		//
		// The GPS's precision is determined by how much power it has.
		//		More power == More precision
		*/

		switch (command[0])
		{
			case '0':
				Print_Settings();
				break;

			case '1':
				Print_Rover_Location();
				break;

			case '2':
				Print_Rover_Direction();
				break;

			case '3':
				Print_Location_And_Direction();
				break;

			case '4':
				Toggle_Print_On_Move();
				break;

			case '5':
				Toggle_Print_On_Turn();
				break;

			case '6':
				Toggle_Print_On_Move_Finished();
				break;	

			case '7':
				Toggle_Print_To_Console();
				break;		

			default:
				EmitSignal(SignalName.CommandNotRecognized, str_Sender, command);
				return false;	// command isn't recognized				
		}

		EmitSignal(SignalName.CommandProcessed, str_Sender, command);
		return true;	// Command processed
	}	
	// end Process_Command()

	protected override void Subscribe_To_Events()
	{
		base.Subscribe_To_Events();
		
		Motor mtr = rvr.GetNode<Motor>("Motor");
		mtr.MovementCommandFinished += () => Auto_Print_Dir();
		mtr.MovementCommandFinished += () => Auto_Print_Pos();
		mtr.MovementFinished += () => Auto_Print_On_Finish();	
	}
	// end Subscribe_To_Events()

	protected override void Load_Config_File()
	{

	}
	// end Load_Config_File()	

	private void Print_Settings()
	{
		rvr.Send_Message(str_Sender, $"Print on Move: {bl_Auto_Print_On_Move}");
		rvr.Send_Message(str_Sender, $"Print on Turn: {bl_Auto_Print_On_Turn}");
		rvr.Send_Message(str_Sender, $"Print on Finish: {bl_Auto_Print_On_Movement_Finished}");
		rvr.Send_Message(str_Sender, $"Print to Console: {bl_Print_to_Console}");
	}
	// end Print_Settings()
	
	private void Print_Rover_Location()
	{
		string str_loc = Get_GPS_Precision_Location_String();

		if(bl_Print_to_Console)
		{
			rvr.Send_Message(str_Sender, $"Rover Location: {str_loc}");
		}

		EmitSignal(SignalName.GPSLocationUpdated, str_loc, Get_Precision_Level(), Get_GPS_Precision_Location_Vector());		
	}
	// end Print_Rover_Location()

	private void Print_Rover_Direction()
	{
		string str_dir = cps.Get_Direction_String(rvr.Transform.X);

		if(bl_Print_to_Console)
		{
			rvr.Send_Message(str_Sender, $"Rover Direction: {str_dir}");
		}	

		EmitSignal(SignalName.GPSDirectionUpdated, str_dir, rvr.Transform.X);
	}
	// end Print_Rover_Direction()

	private void Print_Location_And_Direction()
	{
		Print_Rover_Location();
		Print_Rover_Direction();
	}
	// end Print_Location_And_Direction()

	// Helper function for determining 
	// location precision based on the available power
	private string Get_GPS_Precision_Location_String()
	{
		
		int precision = Get_Precision_Level();	

		// move the decimal of x/y by dividing by precision
		int x = (int)MathF.Round(rvr.Position.X / precision);
		int y = (int)MathF.Round(rvr.Position.Y / precision);
		
		// move the decimal back so that decimal is in the same place as before
		x *= precision;
		y *= precision;

		return $"{x},{y}";	// format the string into x,y cood format
	}
	// end Get_GPS_Precision

	private Vector2 Get_GPS_Precision_Location_Vector()
	{
		
		int precision = Get_Precision_Level();	

		// move the decimal of x/y by dividing by precision
		int x = (int)MathF.Round(rvr.Position.X / precision);
		int y = (int)MathF.Round(rvr.Position.Y / precision);
		
		// move the decimal back so that decimal is in the same place as before
		x *= precision;
		y *= precision;

		return new Vector2(x,y);
	}

	// Helper function to calculate the level of precision
	private int Get_Precision_Level()
	{
		// int_Allocated_Energy == 1 => Round for hundreds place
		// int_Allocated_Energy == 2 => Round for tens place
		// int_Allocated_Energy == 3 => Round for ones place
		// int_Allocated_Energy >= 4 => 0's for both beacuse MathF.Pow() doesn't do negative powers
		return (int)MathF.Pow(10, nrg_Power.int_Max_Energy - nrg_Power.int_Allocated_Energy);
	}
	// end Get_Precision_Level()

	// Helper functions for toggling bools on/off
	private void Toggle_Print_On_Turn()
	{
		bl_Auto_Print_On_Turn = !bl_Auto_Print_On_Turn;
		GetParent<Rover>().Send_Message(str_Sender,
			$"Automatically update direction now set to: {bl_Auto_Print_On_Turn}");
	}

	private void Toggle_Print_On_Move()
	{
		bl_Auto_Print_On_Move = !bl_Auto_Print_On_Move;
		GetParent<Rover>().Send_Message(str_Sender,
			$"Automatically update position now set to: {bl_Auto_Print_On_Move}");
	}

	private void Toggle_Print_On_Move_Finished()
	{
		bl_Auto_Print_On_Movement_Finished = !bl_Auto_Print_On_Movement_Finished;
		GetParent<Rover>().Send_Message(str_Sender,
			$"Automatically update position at the end of movement now set to: {bl_Auto_Print_On_Movement_Finished}");
	}

	private void Toggle_Print_To_Console()
	{
		bl_Print_to_Console = !bl_Print_to_Console;
		GetParent<Rover>().Send_Message(str_Sender,
			$"Printing to the console now set to: {bl_Print_to_Console}\nGPS will still update the Map");
	}
	// end helper functions

	// Helper functions for Auto_Printing
	private void Auto_Print_Pos()
	{
		if(bl_Auto_Print_On_Move && nrg_Power.Has_Enough_Power() && bl_Is_Functional)
		{
			Print_Rover_Location();
		}
	}

	private void Auto_Print_Dir()
	{
		if(bl_Auto_Print_On_Turn && nrg_Power.Has_Enough_Power() && bl_Is_Functional)
		{
			Print_Rover_Direction();
		}
	}

	private void Auto_Print_On_Finish()
	{
		if(bl_Auto_Print_On_Movement_Finished && nrg_Power.Has_Enough_Power() && bl_Is_Functional)
		{
			Print_Location_And_Direction();
		}
	}
	// end Auto_Print functions
}

