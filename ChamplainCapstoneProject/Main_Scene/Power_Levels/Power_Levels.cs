using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Serialization;

public partial class Power_Levels : Control
{
	private PackedScene scn_Meter = GD.Load<PackedScene>("res://Main_Scene/Power_Levels/Component_Power_Meter/Power_Meter.tscn");
	private HBoxContainer hbc_Container;
	private Rover rvr;

	public override void _Ready()
	{
		hbc_Container = GetNode<HBoxContainer>("MarginContainer/HBoxContainer");

		rvr = GetNode<Rover_View>("%Rover_View").Get_Rover();

		Subscribe_To_Events();
	}
	// end Ready()	

	private void Subscribe_To_Events()
	{
		rvr.Ready += () => Initialize_Power_Meters();
	}
	// end Subscribe_To_Events()	

	private void Initialize_Power_Meter(Component component)
	{
		hbc_Container.AddChild(scn_Meter.Instantiate());
		hbc_Container.GetChild<Power_Meter>(-1).Set_Lable_Text(component.Name);
		hbc_Container.GetChild<Power_Meter>(-1).Set_Progress_Bar_Max(component.nrg_Power.int_Max_Energy);
		hbc_Container.GetChild<Power_Meter>(-1).Set_Progress_Bar_Min(component.nrg_Power.int_Min_Energy);
	}
	// end Initialize_Power_Meter()

	private void Initialize_Power_Meters()
	{
		foreach (Node child in rvr.GetChildren())
		{
			Component cmpnt = child as Component;
			if(cmpnt != null)
			{
				Initialize_Power_Meter(cmpnt);

				// Subscribe to EnergyAdjusted event to change levels		
				cmpnt.nrg_Power.EnergyAdjusted += (int index, int val) => Adjust_Meter_Level(index, val);

				// Set the meter to the initial energy level
				Adjust_Meter_Level(cmpnt.GetIndex(), cmpnt.nrg_Power.int_Allocated_Energy);
			}
		}
	}
	// end Initialize_Power_Meters()

	private void Adjust_Meter_Level(int index, int val)
	{
		hbc_Container.GetChild<Power_Meter>(index).Set_Progress_Bar_Val(val);
	}	
	// end Adjust_Meter_Level()
}
