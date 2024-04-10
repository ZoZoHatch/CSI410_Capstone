using Godot;
using System;
using System.Security.Principal;


public partial class Draw_Circle : Node2D
{
	[Export]
	public float flt_Radius = 5f;
	[Export]
	public float flt_Boarder_Thickness = 1f;
	[Export]
	public Color col_Fill = new Color(0.102f, 0.102f, 0.102f);
	[Export]
	public Color col_Boarder = new Color(0, 0.686f, 0);	

	public override void _Draw()
	{
		base._Draw();

		Vector2 pos = Vector2.Zero;		
		
		// draw larger outside circle with the boarder color
		DrawCircle(pos, 
			flt_Radius,		 
			col_Boarder);

		// draw a smaller inside circle with the fill color
		DrawCircle(pos, 
			flt_Radius - flt_Boarder_Thickness,		 
			col_Fill);		
	}
	// end _Draw()

	public override void _Process(double delta)
	{
		base._Process(delta);
		QueueRedraw();
	}
	// end _Process()
}

