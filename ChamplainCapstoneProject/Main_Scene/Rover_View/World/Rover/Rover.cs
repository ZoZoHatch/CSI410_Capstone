using Godot;
using System;

public partial class Rover : CharacterBody2D
{
	/*
	// IMPORTANT
	//	For the rover to work, 
	//	It's child nodes need to be sorted so that 
	//	Component nodes are the fist to be indexed
	// IMPORTANT
	*/	
	
	private Rover_View rv;

	public bool bl_Is_Functional = true;
	
	public override void _Ready()
	{	
		// Initialize rv
		rv = GetOwner<Node>().GetOwner<Rover_View>();

		Subscribe_To_Events();
	}
	// end _Ready()

	private void Subscribe_To_Events()
	{
		// Subscribe to component events	
		// Events from componnets get passed through the Rover before going to Rover_View and than
		// Output_Terminal
		foreach(Node child in GetChildren())	
		{
			if(child as Component != null)	// Only include nodes that are or inherit from Component class
			{
				Component cmpnt = (Component)child;	
				cmpnt.CommandProcessed += (string sndr, string cmd) => 
					Send_Message_To_Output_Terminal(new Message_Struct(Message_Struct.Enum_Message_Types.INFORAMTION, sndr, $"Command:{cmd} Finished Processing"));
				cmpnt.FileEncounteredError += (string sndr) => 
					Send_Message_To_Output_Terminal(new Message_Struct(Message_Struct.Enum_Message_Types.ERROR, sndr, "Encountered and error reading file"));
				cmpnt.CommandNotRecognized += (string sndr, string cmd) =>
					Send_Message_To_Output_Terminal(new Message_Struct(Message_Struct.Enum_Message_Types.ERROR, sndr, $"Command:{cmd} not recognized"));
				cmpnt.nrg_Power.EnergyOvercharged += (string sndr) =>
					Send_Message_To_Output_Terminal(new Message_Struct(Message_Struct.Enum_Message_Types.WARNING, sndr, "Componemt is alocated more power than it can handle"));
				cmpnt.ComponentEncounteredProblem += (string sndr, string prblm) =>
					Send_Message_To_Output_Terminal(new Message_Struct(Message_Struct.Enum_Message_Types.WARNING, sndr, prblm));
			}
		}

		// Subscribe to Power_Supply specific events
		GetNode<Power_Supply>("Power_Supply").PowerSupplyEmptied += (string sndr) =>
			Send_Message_To_Output_Terminal(
				new Message_Struct(Message_Struct.Enum_Message_Types.WARNING, sndr, 
				"Power Supply has no more energy to allocate. De-allocate power from another component and try again"));

		// Subscribe to Motor specific events
		Motor mtr = GetNode<Motor>("Motor");
		mtr.MovementFinished += () => Send_Message_To_Output_Terminal(
			new Message_Struct(Message_Struct.Enum_Message_Types.INFORAMTION, $"{mtr.GetIndex()}:{mtr.Name}", 
			"Rover has finished moving"));
		
	}
	// end Subscribe_To_Events()
	
	private void Send_Message_To_Output_Terminal(Message_Struct message)
	{
		rv.Print_Message(message);
	}	
	// end Send_Message_To_Output_Terminal

	public void Send_Message(string sender, string message)
	{
		Send_Message_To_Output_Terminal(new Message_Struct(
			Message_Struct.Enum_Message_Types.INFORAMTION,
			sender,
			message
		));
	}
	// end Print_Message()

	public void Send_Emergency_Message(string sender, string message)
	{
		Send_Message_To_Output_Terminal(new Message_Struct(
			Message_Struct.Enum_Message_Types.EMERGENCY,
			sender,
			message
		));
	}
	// end Send_Emergency_Message()

	public void Print_Help_File(string filename)
	{
		Send_Message_To_Output_Terminal(new Message_Struct(Message_Struct.Enum_Message_Types.INFORAMTION, Name, $"Printing {filename}"));
		rv.Print_Help_File(filename);
	}	
	// end Print_Help_File()

	public void Disable_Rover()
	{
		bl_Is_Functional = false;
	}
}
