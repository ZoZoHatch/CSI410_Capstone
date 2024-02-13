using Godot;
using System;
using System.ComponentModel.DataAnnotations.Schema;

public partial class Mini_Map : Control
{
	private Label lbl_Location;
	private Label lbl_Direction;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		lbl_Location = GetNode<Label>(
			"PanelContainer/VBoxContainer/Lable_HBoxContainer/Location_MarginContainer/Location_PanelContainer/Location_Label");
		lbl_Direction = GetNode<Label>(
			"PanelContainer/VBoxContainer/Lable_HBoxContainer/Direction_MarginContainer/Direction_PanelContainer/Direction_Label");
		Subscribe_To_Events();
	}
	// end _Ready()	

	private void Subscribe_To_Events()
	{
		GPS pos = GetParent().GetNode<Rover>("%Rover").GetNode<GPS>("GPS");
		pos.GPSLocationUpdated += (string loc, int pre, Vector2 vec) => Set_Location(loc);
		pos.GPSDirectionUpdated += (string dir, Vector2 vec) => Set_Direction(dir);
	}
	// end Subscribe_To_Events()

	public void Set_Location(string location)
	{
		lbl_Location.Text = location;
	}
	// end Set_Location()

	public void Set_Direction(string direction)
	{
		lbl_Direction.Text = direction;
	}
	// end Set_Direction()
}
