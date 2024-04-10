using Godot;
using System;

public partial class Draw_Collector : Node2D
{
	[Export]
	public float flt_Width = 8f;
	[Export]
	public float flt_Height = 6f;
	[Export]
	public float flt_Boarder_Thickness = 1f;

	[Export]
	public float flt_Drill_Length = 5f;	
	[Export]
	public float flt_Drill_Thickness = 2f;

	[Export]
	public Color col_Fill = new Color(0.102f, 0.102f, 0.102f);
	[Export]
	public Color col_Boarder = new Color(0, 0.686f, 0);

	public override void _Ready()
	{
		base._Ready();		
	}
	// end _Ready()

	public override void _Draw()
	{
		base._Draw();

		Vector2 pos = new Vector2(
			Position.X - (flt_Width/2f) + 1, 
			Position.Y - (flt_Height/2f));
		
		Rect2 rct_Col_Body_Border = new Rect2(pos, flt_Width, flt_Height);
		Rect2 rct_Col_Body_Fill = new Rect2(			
			pos.X + flt_Boarder_Thickness, 
			pos.Y + flt_Boarder_Thickness, 
			flt_Width - (2 * flt_Boarder_Thickness), 
			flt_Height - (2 * flt_Boarder_Thickness));

		// draw rectangles for the body of the Collector
		DrawRect(rct_Col_Body_Border, col_Boarder, true);
		DrawRect(rct_Col_Body_Fill, col_Fill, true);	

		Vector2 vec_Drill_Offset = new Vector2(3, 0);

		// draw line for drill
		DrawLine(Position + vec_Drill_Offset,
			new Vector2(Position.X + flt_Drill_Length, Position.Y) + vec_Drill_Offset,
			col_Boarder,
			flt_Drill_Thickness);
	}
	// end _Draw()

	public override void _Process(double delta)
	{
		base._Process(delta);
		QueueRedraw();
	}
	// end _Process()
}
