using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

public partial class Processor : Component
{
	private List<Component> lst_Rover_Components = new List<Component>();

	
	public override void _Ready()
	{
		base._Ready();

		// Intialize arr_Rover_Components with all the commponent nodes currently on the rover
		foreach (Node node in GetParent<Rover>().GetChildren())
		{			
			if(node as Component != null)	// Only include nodes that are or inherit from Component class
			{
				lst_Rover_Components.Add(node as Component);							
			}
		}

		// Intialize nrg_Power.int_Max_Energy
		nrg_Power.Init_Max_Energy(8); 

		// Set nrg_Power.int_Min_Energy
		nrg_Power.Set_Min_Energy(1);			

		Subscribe_To_Events();
	}
	// end _Ready()

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
	

	public void Process_Key_Pad_Input(string address, string command)
	{
		int int_Address = address.ToInt();
		
		// Check that int_Address is in range
		if (int_Address >= lst_Rover_Components.Count)
		{
			throw new Rover_ComponentException(new Message_Struct(
				Message_Struct.Enum_Message_Types.ERROR,
				str_Sender,
				"Invalid Component Address"));			
		}

		// Check that processor has enough power
		if(!nrg_Power.Has_Enough_Power() &&
		lst_Rover_Components[int_Address].GetType() != typeof(Power_Supply)) // exception made for sending commands to the Power_Supply on low power
		{					
			throw new Rover_ComponentException(new Message_Struct(
				Message_Struct.Enum_Message_Types.WARNING,
				str_Sender,
				"Processor requires more power before it can parse commands:\nPlease allocate more energy from the Power_Supply"));							
		}

		try
		{
			lst_Rover_Components[int_Address].Process_Command(command);	
		}
		catch(Rover_ComponentException e)
		{
			throw e;	// throw any exceptions from components to the rover
		}	
						
	}
	// end Process_Key_Pad_Input()	
}
