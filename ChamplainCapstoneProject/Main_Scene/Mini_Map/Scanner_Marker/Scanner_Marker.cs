using Godot;
using System;

public partial class Scanner_Marker : Polygon2D
{
	private bool bl_Fade = false;

    public override void _Ready()
    {
        base._Ready();

		Subscribe_To_Events();
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
				
		if(bl_Fade)
		{
			float flt_Life_Time = 5.0f; // in seconds
			Color = new Color(Color.R, 
						Color.G, 
						Color.B, 
						Color.A - ((float)delta / flt_Life_Time));

			if(Color.A < 0)
			{
				QueueFree();
			}
		}		
    }
	// end _Process()

	private void Subscribe_To_Events()
	{
		// listen for the move started event
		// be for starting the fade anim
		GetNode("/root/Main_Scene")
			.GetNode<Rover_View>("%Rover_View")
			.Get_Rover()
			.GetNode<Motor>("Motor")
			.MovementCommandStarted += (int _, int _) =>
			bl_Fade = true;
	} 
	// end _Subscribe_To_Events()

	public void Set_Pos(Vector2 pos)
	{
		Position = pos;
	}
    // end Set_Pos()
}
