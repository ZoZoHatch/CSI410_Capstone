using Godot;
using System;

public partial class Compass : Node
{
	public enum Enum_Cardinal_Directions
	{
		INVALID_DIR = -1,
		EAST,
		SOUTH_EAST,
		SOUTH,
		SOUTH_WEST,
		WEST,
		NORTH_WEST,
		NORTH,
		NORTH_EAST,	
		FINAL_DIR
	}

	private string[] arr_Cardinal_Dir_Strings = 
	{"East", "Sth_Est", "South", "Sth_Wst", "West", "Nth_Wst", "North", "Nth_Est"};

	// Helper function to convert a direction vector to cardinal direction enum
	public Enum_Cardinal_Directions Get_Direction_From_Vector(Vector2 dir)
	{
		// Get the angle in degrees
		float flt_dir_angle = dir.Angle() * 180 / MathF.PI;

		// Convert negative angle to equivailant positive angle
		if(flt_dir_angle < 0)
		{
			flt_dir_angle += 360.0f;
		}
 
		// The divide by 1/8th of a circle and cast to int
		int cardinal_dir =  (int)MathF.Round(flt_dir_angle / 45.0f);			

		return (Enum_Cardinal_Directions)cardinal_dir;
	}
	// end Get_Direction_From_Vector()

	public string Get_Direction_String(Vector2 dir)
	{
		return arr_Cardinal_Dir_Strings[(int)Get_Direction_From_Vector(dir)];
	}
	// end Get_Direction_String()

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
