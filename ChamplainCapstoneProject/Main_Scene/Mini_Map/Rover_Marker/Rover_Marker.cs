using Godot;
using System;
using System.Dynamic;

public partial class Rover_Marker : Polygon2D
{	
	private enum Enum_Radii
	{
		LEVEL_1 = 10,
		LEVEL_2 = 125,
		LEVEL_3 = 200
	}

	private Timer tmr;

	// vars for animating a scan	
	private int int_Rotation_Direction = 0;
	private float flt_Scan_Angle = 0;
	private Vector2 vec2_Draw_Dir;
	private float flt_Anim_Speed = 0;
	private int int_Anim_Size = 0;


	// vars for gps precision
	private float flt_Precision_Radius = (float)Enum_Radii.LEVEL_3;
	private int int_Num_Points = 16;
	private int int_Line_Width = 8;


	public override void _Ready()
	{
		tmr = GetNode<Timer>("Timer");
		Subscribe_To_Events();
	}
	// end _Ready()

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		
		if (!tmr.IsStopped())
		{			
			flt_Scan_Angle += flt_Anim_Speed * int_Rotation_Direction * (float)delta;
			vec2_Draw_Dir = new Vector2(MathF.Cos(flt_Scan_Angle), MathF.Sin(flt_Scan_Angle)) * int_Anim_Size;
			QueueRedraw();
		}
	}

	public override void _Draw()
	{
		base._Draw();	    

		DrawArc(Vector2.Zero, 
			flt_Precision_Radius, 
			0.0f, MathF.Tau, 
			int_Num_Points, 
			Color,
			int_Line_Width);		

		if (!tmr.IsStopped())
		{
			DrawLine(Vector2.Zero, vec2_Draw_Dir, Color, int_Line_Width);
		}		
	}
	// end _Draw()

	private void Subscribe_To_Events()
	{
		GPS g = GetOwner<Mini_Map>().GetParent().GetNode<Rover_View>("%Rover_View").Get_Rover().GetNode<GPS>("GPS");
		g.GPSDirectionUpdated += (string dir, Vector2 vec) => Set_Direction(vec);
		g.GPSLocationUpdated += (string loc, int pre, Vector2 loc_vec) => Set_Location(loc_vec, pre);

		Scanner s = GetOwner<Mini_Map>().GetParent().GetNode<Rover_View>("%Rover_View").Get_Rover().GetNode<Scanner>("Scanner");
		s.ScanStarted += (float dur, int dir, int rang) => Animate_Scan(dur, dir, rang);

		tmr.Timeout += () => QueueRedraw();
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
		
		QueueRedraw();	// redraw to change radius and locatation
	}
	// end Set_Location

	private void Animate_Scan(float dur, int dir, int range)
	{
		int_Anim_Size = range;
		flt_Anim_Speed = Mathf.Tau / dur;
		int_Rotation_Direction = dir;
		flt_Scan_Angle = Rotation;

		tmr.Start(dur);
	}
	// end Animate_Scan()
}
