using Godot;
using System;

public partial class Draw_Chassis : Node2D
{
	public float flt_Height;
	public float flt_Width;
	[Export]
	public float flt_Boarder_Thickness = 2f;
	[Export]
	public Color col_Fill = new Color(0.102f, 0.102f, 0.102f);
	[Export]
	public Color col_Boarder = new Color(0, 0.686f, 0);

	public override void _Ready()
	{
		base._Ready();
		RectangleShape2D rect = (RectangleShape2D)GetParent()
			.GetParent().GetChild<CollisionShape2D>(-1).Shape;

		flt_Height = rect.Size.Y;
		flt_Width = rect.Size.X;
	}
	// end _Ready()

	public override void _Draw()
	{
		base._Draw();

		Vector2 pos = ToLocal(GetParent<Node2D>().GlobalPosition);		
		
		Rect2 arm = new Rect2(pos.X - (flt_Width/2f), pos.Y - (flt_Height/2f), flt_Width, flt_Height);

		// draw rectangle
		DrawRect(arm, col_Boarder, false, flt_Boarder_Thickness);	
	}
	// end _Draw()

	public override void _Process(double delta)
	{
		base._Process(delta);
		QueueRedraw();
	}
	// end _Process()
}
