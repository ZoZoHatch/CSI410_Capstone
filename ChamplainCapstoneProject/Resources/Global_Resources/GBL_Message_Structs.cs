using Godot;
using System;
using System.Reflection.Metadata;



[GlobalClass]
public partial class Message_Struct : Resource
{
	public enum Enum_Message_Types
	{   
		INVALID_TYPE = -1,
		MESSAGE,
		INFORAMTION,
		ERROR,
		WARNING,
		EMERGENCY,
		FINAL_TYPE
	}
	

	[Export(PropertyHint.Enum, "Message,Info,Error,Warning,Emergency")]	
	public Enum_Message_Types enm_Type { get; set; } = Enum_Message_Types.INVALID_TYPE;
	[Export]
	public string str_Sender { get; set; }
	[Export]
	public string str_Message { get; set; }

	public Message_Struct(Enum_Message_Types type, string sender, string message)
	{
		enm_Type = type;
		str_Sender = sender;
		str_Message = message;
	}
}
	
