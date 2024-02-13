using Godot;
using System;
using System.Collections.Generic;
using System.Net;


public partial class Power_Supply : Component
{
	private List<Energy_Resource> lst_Component_Energy_Resources = new List<Energy_Resource>();

	[Signal]
	public delegate void PowerSupplyEmptiedEventHandler(string sender);

	public override void _Ready()
	{
		base._Ready();		

		// get the Energy_Resource for each component on the rover
		foreach (Node node in GetParent<Rover>().GetChildren())
		{			
			if(node as Component != null)	// Only include nodes that are or inherit from Component class
			{
				lst_Component_Energy_Resources.Add((node as Component).GetNode<Energy_Resource>("Energy_Resource"));							
			}
		}

		// Intialize nrg_Power.int_Max_Energy
		nrg_Power.Init_Max_Energy(8); 

		// set the alocated energy of the power supply to the max energy
		nrg_Power.Adjust_Energy(nrg_Power.int_Max_Energy);

		Subscribe_To_Events();
		
		Pre_Allocate_Energy();
	}
	// end _Ready()

	public override bool Process_Command(string command)
	{
		if(base.Process_Command(command))
		{
			return true;	// Return true if command was handled by base component
		}

		int current_index = 0;

		/*
		// Power_Supply command processing structure:
		// command is processed in 2 character chunks
		// 1st char: address of component who's power is being adjusted
		// 2nd char: the new power level for the component 
		//
		// Allocated power comes from the Power_Supply's available power
		// Power_Supply can't allocate power to itself
		*/

		do
		{
			int address = command[current_index].ToString().ToInt();

			// Check the char at current_index works as a valid component address			
			if(address >= lst_Component_Energy_Resources.Count)
			{
				// throw an exception of it's not valid
				throw new Rover_ComponentException(new Message_Struct(
					Message_Struct.Enum_Message_Types.ERROR,
					str_Sender,
					"Invalid Component Address"));	
			}

			// Check if there isn't a character after the current index
			// i.e. current_index + 1 >= the number of chars in command
			if(current_index + 1 >= command.Length)
			{
				Print_Component_Energy_Info(address);
				EmitSignal(SignalName.CommandProcessed, str_Sender);				
			}
			else
			{
				char val = command[current_index + 1];
				switch(val)
				{
					case '.':
						Print_Component_Energy_Info(address);
						break;

					default:
						Adjust_Component_Energy(address,val.ToString().ToInt());
						Print_Component_Energy_Info(address);
						break;
				}			
				// end switch()				
			}
			// end if() else
			current_index += 2;	// Increment by 2 because Power_Supply processes commands in 2 char chunks
		}
		while(current_index < command.Length);
		

		EmitSignal(SignalName.CommandProcessed, str_Sender, command);
		return true;	// Command processed
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

	private void Adjust_Component_Energy(int index, int new_energy)
	{
		// Check if index is for Power_Supply
		if(index == GetIndex())
		{
			GetParent<Rover>().Send_Message(str_Sender, "Power_Supply cannot adjust it's own power");
			return;
		}

		// Get the difference between component's new energy and it's current energy 
		int int_Delta_Energy = new_energy - lst_Component_Energy_Resources[index].int_Allocated_Energy;

		// Check if Power_Supply has enough energy available to allocate
		if(nrg_Power.int_Allocated_Energy >= int_Delta_Energy)	// should always be tue of Delta_Energy is negative
		{	
			// set the component's energy
			lst_Component_Energy_Resources[index].Adjust_Energy(new_energy);

			// set Power_Supply's energy
			nrg_Power.Adjust_Energy(nrg_Power.int_Allocated_Energy - int_Delta_Energy);	// will increase energy if delta is negative
		}
		else	// not enough power to allocate
		{
			int_Delta_Energy = int_Delta_Energy - nrg_Power.int_Allocated_Energy;	// will always be positive

			// set the component's energy (As much as possible)
			lst_Component_Energy_Resources[index].Adjust_Energy(new_energy - int_Delta_Energy);	

			// set Power_Supply's energy
			nrg_Power.Adjust_Energy(0);
			EmitSignal(SignalName.PowerSupplyEmptied, str_Sender);
		}		
	}
	// end Adjust_Energy()

	// A helper function to adjust the energy levels of components to their minimum value
	// Called only at the end of Ready()
	// and only after Parent Rover is Ready
	private async void Pre_Allocate_Energy()
	{ 
		await ToSignal(GetParent<Rover>(), Node.SignalName.Ready);
		
		GetParent<Rover>().Send_Message(str_Sender, "Allocating mimimum power levels");

		for(int i = 0; i < lst_Component_Energy_Resources.Count; i++)
		{
			if(lst_Component_Energy_Resources[i].int_Min_Energy != 0)
			{
				Adjust_Component_Energy(i, lst_Component_Energy_Resources[i].int_Min_Energy);
			}			
		}
	}
	// end Pre_Allocate_Energy()

	private void Print_Component_Energy_Info(int index)
	{
		GetParent<Rover>().Send_Message(str_Sender, lst_Component_Energy_Resources[index].Print_Info());
	}
	// end Print_Component_Energy_Info()
	
}

