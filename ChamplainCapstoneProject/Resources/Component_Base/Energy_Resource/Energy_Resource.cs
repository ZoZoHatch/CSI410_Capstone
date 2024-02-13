using Godot;
using System;
using System.Dynamic;

public partial class Energy_Resource : Node
{
	// Variables for power management
	// Intened to be assigned values in Load_Config_File
	public int int_Max_Energy {get; private set;}
	public int int_Allocated_Energy {get; private set;}

	// Minimum amount of energy that needs inorder to function
	public int int_Min_Energy {get; private set;}
	
	protected string str_Sender;	// string for the Sender field of any messages that a component would need to send the Output_Terminal

	[Signal]
	public delegate void EnergyAdjustedEventHandler(int index, int energy);
	
	[Signal]	// Signal is emitted if int_Available_Energy > int_Max_Energy
	public delegate void EnergyOverchargedEventHandler(string sender);

	public override void _Ready()
	{
		int_Max_Energy = 0;	
		int_Allocated_Energy = 0;

		str_Sender = $"{GetParent().GetIndex()}:{GetParent().Name}";
	}
	// end _Ready()

	public void Adjust_Energy(int new_available_energy)
	{
		int_Allocated_Energy = new_available_energy;

		EmitSignal(SignalName.EnergyAdjusted, GetParent().GetIndex(), int_Allocated_Energy);

		if(Is_Overcharged())
		{
			EmitSignal(SignalName.EnergyOvercharged, str_Sender);
		}
	}
	// end Adjust_Energy()

	public bool Is_Overcharged()
	{
		return int_Allocated_Energy > int_Max_Energy;
	}
	// end Is_OverCharged()

	public bool Has_Enough_Power()
	{
		return int_Allocated_Energy >= int_Min_Energy;
	}
	// end Has_Enough_Power()

	public void Init_Max_Energy(int max)
	{
		if(int_Max_Energy==0)
		{
			int_Max_Energy = max;
		}
	}
	// end Init_Max_Energy()	

	public void Set_Min_Energy(int min)
	{
		int_Min_Energy = min;
	}
	// end Set_Min_Energy()

	public string Print_Info()
	{
		return $"{GetParent().Name} Energy: Allocated:{int_Allocated_Energy} Maximum:{int_Max_Energy}";
	}
	// end Print_Info()
}
