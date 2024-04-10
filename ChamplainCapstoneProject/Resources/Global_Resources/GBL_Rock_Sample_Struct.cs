using Godot;
using System;
using System.Reflection.Metadata;



[GlobalClass]
public partial class Rock_Sample_Struct : Resource
{
	public enum Enum_Rock_Types
	{   
        UNKOWN_TYPE = 0,
        LIMESTONE,
        MARBLE,
        CALCARENITE,
        TRAVERTINE
	}
	
	private string[] arr_Rock_Types_As_Strings = 
	{
		"Unknown",
		"Limestone",
		"Marble",
		"Calcarenite",
		"Travertine"
	};

    public Enum_Rock_Types enm_Type;
	public string str_Type;

	public Rock_Sample_Struct(Enum_Rock_Types type)
	{
		enm_Type = type;
		str_Type = arr_Rock_Types_As_Strings[(int)enm_Type];
	}

	public Rock_Sample_Struct()
	{
		enm_Type = Enum_Rock_Types.LIMESTONE;
		str_Type = arr_Rock_Types_As_Strings[(int)enm_Type];
	}
}