using Godot;
using System;

public partial class Draw_Arm : Node2D
{
	[Export]
	public float flt_Height = -22f;
	[Export]
	public float flt_Width = 5f;

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
		
		Rect2 rct_Boarder = new Rect2(pos.X - (flt_Width/2f), pos.Y, flt_Width, flt_Height);

		Rect2 rct_Fill = new Rect2(
			pos.X - (flt_Width/2) + flt_Boarder_Thickness, 
			pos.Y + flt_Boarder_Thickness, 
			flt_Width - (2 * flt_Boarder_Thickness), 
			flt_Height - (2 * flt_Boarder_Thickness));

		// draw larger outer rectangle in boarder col
		DrawRect(rct_Boarder, col_Boarder, true);

		// draw smaller int rectangle in fill col
		DrawRect(rct_Fill, col_Fill, true);
		
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
