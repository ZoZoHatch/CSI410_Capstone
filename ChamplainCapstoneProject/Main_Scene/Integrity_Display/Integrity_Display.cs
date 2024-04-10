using Godot;
using System;

public partial class Integrity_Display : PanelContainer
{
	private Label lbl;
	
	public override void _Ready()
	{
		lbl = GetNode<Label>("Integrity_Label");

		Subscribe_To_Events();
	}
	// end _Ready()

	private void Subscribe_To_Events()
	{
		Rover rvr = GetParent().GetNode<Rover_View>("%Rover_View").Get_Rover();

		Chassis chas = rvr.GetNode<Chassis>("Chassis");
		chas.IntegrityQueried += (int integrity) => Set_Integrity(integrity);				
	}
	// end Subscribe_To_Events()

	private void Set_Integrity(int integrity)
	{
		lbl.Text = integrity.ToString();
	}
}
