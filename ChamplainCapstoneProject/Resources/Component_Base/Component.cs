using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Threading;


public partial class Component : Node2D
{
	// A class that all other components derive from
	protected File_Reader fr_Reader;	// For reading config files
	public Energy_Resource nrg_Power {get; private set;} 	// Energy_Resouce for Power_Suply component
	protected string str_Sender;	// string for the Sender field of any messages that a component would need to send the Output_Terminal
	protected char chr_Deliniator;	
		// This char is used to deliniate between the base Process_Command() and overidden Process_Command()
			// if command string starts with chr_Deliniator, use the base Process_Command()
	

	[Signal]
	public delegate void CommandProcessedEventHandler(string sender, string command);

	[Signal]
	public delegate void CommandNotRecognizedEventHandler(string sender, string command);

	[Signal]
	public delegate void FileEncounteredErrorEventHandler(string sender);
	
	[Signal]
	public delegate void ComponentEncounteredProblemEventHandler(string sender, string problem);
	
	public override void _Ready()
	{
		fr_Reader = GetNode<File_Reader>("File_Reader");
		nrg_Power = GetNode<Energy_Resource>("Energy_Resource");
		str_Sender = $"{GetIndex()}:{Name}";

		// These are intended to be controlled by the config file
		chr_Deliniator = '.';					
	}
	// end _Ready()
	
	public virtual void Step(double deltaT)
	{

	}
	// end Step()

	public virtual bool Process_Command(string command)	// returns True if the command is processed, otherwise False
	{
		if(command == " ")	// 	No command => print help file
		{
			Print_Help_File();
			EmitSignal(SignalName.CommandProcessed, str_Sender, command);			
			return true;
		}

		if(!nrg_Power.Has_Enough_Power())
		{
			throw new Rover_ComponentException(new Message_Struct(
				Message_Struct.Enum_Message_Types.WARNING,
				str_Sender,
				$"{Name} doesn't have enough power to process command: {command}"));							
		}

		if(command[0] != chr_Deliniator)
		{
			return false;
		}
		
		EmitSignal(SignalName.CommandProcessed, str_Sender, command);
		return true;		
	}
	// end Process_Command()	

	protected virtual void Subscribe_To_Events()
	{
		
	}
	// end Subscribe_To_Events()	

	protected virtual void Load_Config_File()
	{

	}	
	// end Load_Config_File()

	protected void Print_Help_File()	// Help_File naming format: [Component Name]_Help.txt
	{
		GetParent<Rover>().Print_Help_File($"{Name}_Help.txt");	
	}
	// end Print_Help_File()

	public class Rover_ComponentException : System.Exception
			{
				public Rover_ComponentException() { }
				public Rover_ComponentException(Message_Struct message) { msg_Message = message; }
				public Message_Struct msg_Message;	
			}	
}
