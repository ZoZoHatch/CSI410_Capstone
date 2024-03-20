using Godot;
using System;

public partial class Match_CollisionPolygon2D : Polygon2D
{	
	public override void _Ready()
	{
		Polygon = GetParent<CollisionPolygon2D>().Polygon;
	}
	// end _Ready()	
}
