using Godot;
using System;

public partial class Rock_Data : Node
{
	public Rock_Sample_Struct Sample_Data;

	[Export(PropertyHint.Enum, "Unknown,Limestone,Marble,Calcarenite,Travertine")]
	public int type = 1;
	
	public override void _Ready()
	{
		Sample_Data = new Rock_Sample_Struct((Rock_Sample_Struct.Enum_Rock_Types)type);
	}
	// end _Ready()
}
