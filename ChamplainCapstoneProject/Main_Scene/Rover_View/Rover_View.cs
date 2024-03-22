using Godot;
using System;
using System.Diagnostics;

public partial class Rover_View : Control
{
	private Output_Terminal tml;
	private Rover rvr;

	public override void _Ready()
	{	
		// Initialize tml
		tml = GetOwner<Node>().GetNode<Output_Terminal>("%Output_Terminal");

		// Initialize rvr
		rvr = GetNode("%World").GetNode<Rover>("%Rover");

		Subscribe_To_Events();
	}
	// end _Ready()

	public Rover Get_Rover()
	{
		return GetNode("%World").GetNode<Rover>("%Rover");
	}
	// end Get_Rover()

	private void Subscribe_To_Events()
	{		
		GetOwner<Node>().GetNode<Key_Pad>("%Key_Pad").KeyPadReturnedFormated += (string adr, string cmd)
			=> Process_Key_Pad_Input(adr,cmd);
		GetOwner<Node2D>().Ready += () => Print_Help_File("System");
	}
	// end Subscribe_To_Events()

	private void Process_Key_Pad_Input(string adr, string cmd)
	{
		if(!int.TryParse(adr, out _))
		{
			Process_System_Command(cmd);
			Print_Message(new Message_Struct(
				Message_Struct.Enum_Message_Types.ERROR,
				Name,
				"Please start your input with a number."));
			return;
		}

		if(!rvr.bl_Is_Functional)
		{
			Print_Message(new Message_Struct(
				Message_Struct.Enum_Message_Types.ERROR,
				Name,
				"The Rover has become non-functional and cannot continue."));
			return;
		}

		try
		{
			rvr.GetNode<Processor>("%Processor").Process_Command(adr, cmd);
		}
		catch(Component.Rover_ComponentException e)
		{
			Print_Message(e.msg_Message);
		}		
	}
	// end Send_Command_To_Rover()

	public void Print_Message(Message_Struct message)
	{
		tml.Print_Message(message);
	}
	// end Print_Message()

	public void Print_Help_File(string filename)
	{
		tml.Print_File(filename, File_Reader.Enum_File_Types.HELP);
	}
	// end Print_File()

	private void Process_System_Command(string cmd)
	{
		if(cmd == "")
		{
			Print_Help_File("System");
		}
	}
	// end Process_System_Command()
}
