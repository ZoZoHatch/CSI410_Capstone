using Godot;
using System;
using System.Diagnostics;

public partial class Rover_View : Control
{
	private Output_Terminal tml;
	private Rover rvr;	

	private string str_Help_Filename = "System_Help.txt";
	private string str_Welcome_Filename = "Welcome.txt";
	private string str_Map_Filename = "Map.txt";
	private string str_Obj_Filename = "Objectives.txt";
	private string str_Credits_Filename = "Credits.txt";

	private string str_GameOver_Filename = "Game_Over.txt";
	private string str_Win_Filename = "Congrats.txt";

	private string str_Sender = "System";

	private bool bl_Rover_Powered = false;
	private bool bl_Rock_Sampled = false;

	public override void _Ready()
	{	
		// Initialize tml
		tml = GetOwner<Node>().GetNode<Output_Terminal>("%Output_Terminal");

		// Initialize rvr
		rvr = GetNode("%World").GetNode<Rover>("%Rover");	

		Subscribe_To_Events();

		Update_View();
	}
	// end _Ready()

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);		
	}

	public Rover Get_Rover()
	{
		return GetNode("%World").GetNode<Rover>("%Rover");
	}
	// end Get_Rover()

	private void Subscribe_To_Events()
	{		
		GetOwner<Node>().GetNode<Key_Pad>("%Key_Pad").KeyPadReturnedFormated += (string adr, string cmd)
			=> Process_Key_Pad_Input(adr,cmd);
		GetOwner<Node2D>().Ready += () => Print_File(str_Welcome_Filename);

		rvr.GetNode<Scanner>("Scanner").RoverScanned += () => Update_View();

		// subscribe to objecive related events
		rvr.GetNode<Processor>("Processor").nrg_Power.EnergyAdjusted += 
			(int _, int nrg) => Check_Power_On(nrg);

		rvr.GetNode<Collector>("Collector").SampleCollected += 
			(Rock_Sample_Struct.Enum_Rock_Types type) => Check_Sampled_Correct_Rock(type);
	}
	// end Subscribe_To_Events()

	private void Process_Key_Pad_Input(string adr, string cmd)
	{
		if(!int.TryParse(adr, out _))
		{
			Process_System_Command(cmd);			
			return;
		}

		if(!rvr.bl_Is_Functional)
		{
			Print_Message(new Message_Struct(
				Message_Struct.Enum_Message_Types.ERROR,
				str_Sender,
				"The Rover has become non-functional and cannot continue."));
			Print_Objective_Status();
			Print_File(str_GameOver_Filename);

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
	// end Print_Help_File()
	private void Print_File(string filename)
	{
		tml.Print_File(filename, File_Reader.Enum_File_Types.GENERIC);
	}
	// end Print_File()

	private void Check_Power_On(int nrg)
	{
		bl_Rover_Powered = nrg > 0;
	}
	// end Check_Power_On_Object()

	private void Check_Sampled_Correct_Rock(Rock_Sample_Struct.Enum_Rock_Types type)
	{
		bl_Rock_Sampled = type == Rock_Sample_Struct.Enum_Rock_Types.UNKOWN_TYPE;
	}
	// end Check_Sampled_Correct_Rock()

	private void Print_Objective_Status()
	{
		Print_Message( new Message_Struct(Message_Struct.Enum_Message_Types.MESSAGE,
		str_Sender,
		$"The current Objective status:\n" + 
		$" 1. Power Processor: {bl_Rover_Powered}\n" +
		$" 2. Collect star stone in NW: {bl_Rock_Sampled}\n" +
		$" 3. Avoid breaking the rover: {rvr.bl_Is_Functional}"));
	}
	// end Print_Objective_Status()

	private void Process_System_Command(string cmd)
	{
		if(cmd == "")
		{
			Print_Help_File(str_Help_Filename);
			return;
		}

		switch(cmd[0])
		{
			case '0':
				Print_File(str_Welcome_Filename);
				break;
			
			case '1':
				Print_File(str_Map_Filename);
				break;
			
			case '2':
				Print_File(str_Obj_Filename);
				break;
			
			case '3':
				Print_Objective_Status();
				break;
			
			case '4':
				Print_File(str_Credits_Filename);
				break;

			default:
				Print_Help_File(str_Help_Filename);
				break;
		}
	}
	// end Process_System_Command()
	
	private void Update_View()
	{			
		SubViewport svp_World = GetNode<SubViewport>("%SubViewport");
		TextureRect tr_Snapshot = GetNode<TextureRect>("%Snapshot");
				
		tr_Snapshot.Texture = ImageTexture.CreateFromImage(
			svp_World.GetTexture().GetImage());		
	}
	// end Update_View()


}
