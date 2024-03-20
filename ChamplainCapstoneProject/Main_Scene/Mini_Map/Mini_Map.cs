using Godot;
using System;
using System.ComponentModel.DataAnnotations.Schema;

public partial class Mini_Map : Control
{
	private PackedScene scn_Scan_Marker = GD.Load<PackedScene>
		("res://Main_Scene/Mini_Map/Scanner_Marker/Scanner_Marker.tscn");

	private Camera2D cam;	

	private Label lbl_Location;
	private Label lbl_Direction;

	private Rover_Marker rvr_Mark;	
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cam = GetNode<Camera2D>("%Camera2D");		

		lbl_Location = GetNode<Label>(
			"PanelContainer/VBoxContainer/Lable_HBoxContainer/Location_MarginContainer/Location_PanelContainer/Location_Label");
		lbl_Direction = GetNode<Label>(
			"PanelContainer/VBoxContainer/Lable_HBoxContainer/Direction_MarginContainer/Direction_PanelContainer/Direction_Label");
				
		rvr_Mark = GetNode<Rover_Marker>("%Rover_Marker");

		Subscribe_To_Events();
	}
	// end _Ready()	

	private void Subscribe_To_Events()
	{
		Rover rvr = GetParent().GetNode<Rover_View>("%Rover_View").Get_Rover();

		GPS pos = rvr.GetNode<GPS>("GPS");
		pos.GPSLocationUpdated += (string loc, int pre, Vector2 vec) => Set_Location(loc);
		pos.GPSDirectionUpdated += (string dir, Vector2 vec) => Set_Direction(dir);

		Scanner scnr = rvr.GetNode<Scanner>("Scanner");
		scnr.ScanMade += (Vector2 pos) => Add_Scanner_Ping(pos);		
	}
	// end Subscribe_To_Events()

	private void Set_Location(string location)
	{
		lbl_Location.Text = location;
	}
	// end Set_Location()

	private void Set_Direction(string direction)
	{
		lbl_Direction.Text = direction;
	}
	// end Set_Direction()

	private void Add_Scanner_Ping(Vector2 pos)
	{				
		if(pos != Vector2.Zero)
		{	
			cam.AddChild(scn_Scan_Marker.Instantiate());
			cam.GetChild<Scanner_Marker>(-1).Set_Pos(pos);		
		}
	}
	// end Add_Scanner_Ping()	
}
