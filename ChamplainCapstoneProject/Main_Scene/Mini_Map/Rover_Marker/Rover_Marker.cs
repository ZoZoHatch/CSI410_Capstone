using Godot;
using System;
using System.Dynamic;

public partial class Rover_Marker : Polygon2D
{	
	private enum Enum_Radii
	{
		LEVEL_1 = 1,
		LEVEL_2 = 10,
		LEVEL_3 = 100
	}

	private float flt_Precision_Radius = (float)Enum_Radii.LEVEL_3;
	private int int_Num_Points = 16;

	public override void _Ready()
	{
		Subscribe_To_Events();
	}
	// end _Ready()

	public override void _Draw()
	{
		base._Draw();	    

		DrawArc(Vector2.Zero, 
			flt_Precision_Radius, 
			0.0f, MathF.Tau, 
			int_Num_Points, 
			Color);		
	}
	// end _Draw()

	private void Subscribe_To_Events()
	{
		GPS g = GetOwner<Mini_Map>().GetParent().GetNode<Rover>("%Rover").GetNode<GPS>("GPS");
		g.GPSDirectionUpdated += (string dir, Vector2 vec) => Set_Direction(vec);
		g.GPSLocationUpdated += (string loc, int pre, Vector2 loc_vec) => Set_Location(loc_vec, pre);
	}
	// end Subscribe_To_Events()
	
	private void Set_Direction(Vector2 direction)
	{
		Rotation = direction.Angle();
	}
	// end Set_Direction()

	private void Set_Location(Vector2 location, int precision)
	{
		Position = location;

		switch (precision)
		{
			case 1:
				flt_Precision_Radius = (float)Enum_Radii.LEVEL_1;
				break;

			case 10:
				flt_Precision_Radius = (float)Enum_Radii.LEVEL_2;
				break;

			case 100:
				flt_Precision_Radius = (float)Enum_Radii.LEVEL_3;
				break;

			default:
				break;
		}
		
		QueueRedraw();	// redraw to change radius
	}
	// end Set_Location
}
