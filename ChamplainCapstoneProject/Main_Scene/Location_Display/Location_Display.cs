using Godot;
using System;

public partial class Location_Display : PanelContainer
{
	private Label lbl;

	
	public override void _Ready()
	{
		lbl = GetNode<Label>("Location_Label");

		Subscribe_To_Events();
	}
	// end _Ready()

	private void Subscribe_To_Events()
	{
		Rover rvr = GetParent().GetNode<Rover_View>("%Rover_View").Get_Rover();

		GPS pos = rvr.GetNode<GPS>("GPS");
		pos.GPSLocationUpdated += (string loc, int pre, Vector2 vec) => Set_Location(loc);				
	}
	// end Subscribe_To_Events()

	private void Set_Location(string location)
	{
		lbl.Text = location;
	}
	// end Set_Location()
}
