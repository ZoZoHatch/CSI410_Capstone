using Godot;
using System;

public partial class Direction_Display : PanelContainer
{
	private Label lbl;
	
	public override void _Ready()
	{
		lbl = GetNode<Label>("Direction_Label");

		Subscribe_To_Events();
	}
	// end _Ready()

	private void Subscribe_To_Events()
	{
		Rover rvr = GetParent().GetNode<Rover_View>("%Rover_View").Get_Rover();

		GPS pos = rvr.GetNode<GPS>("GPS");
		pos.GPSDirectionUpdated += (string dir, Vector2 vec) => Set_Direction(dir);				
	}
	// end Subscribe_To_Events()

	private void Set_Direction(string direction)
	{
		lbl.Text = direction;
	}
	// end Set_Direction()
}
