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

	protected bool bl_Is_Functional = true;	

	private float flt_Chance_To_Fail = 0.25f;	// the chance for the component to fail when it
	// processes a command while overloaded
	// in the range (0,1)
	// represents a percent chance so 0.25f is a 25% chance to fail

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

		if(!bl_Is_Functional)
		{
			EmitSignal(SignalName.ComponentEncounteredProblem, str_Sender,
			$"{Name} is currently non-functional:\n    De-allocate all power and re-allocate to regain functionality");
			return true;			
		}

		if(!nrg_Power.Has_Enough_Power())
		{
			throw new Rover_ComponentException(new Message_Struct(
				Message_Struct.Enum_Message_Types.WARNING,
				str_Sender,
				$"{Name} doesn't have enough power to process command: {command}"));							
		}

		if(nrg_Power.Is_Overcharged())
		{	
			if(flt_Chance_To_Fail > GD.Randf())
			{	// GD.randf() returns a float between 0 and 1, if it's less than the threshold it's a fail
				bl_Is_Functional = false;
				throw new Rover_ComponentException(new Message_Struct(
					Message_Struct.Enum_Message_Types.EMERGENCY,
					str_Sender,
					$"{Name} has become non-functional from recieving too much power."));
			}			
		}

		if(command[0] != chr_Deliniator)
		{
			return false;
		}

		Print_Help_File();
		
		EmitSignal(SignalName.CommandProcessed, str_Sender, command);
		return true;		
	}
	// end Process_Command()

	public void Set_Component_As_Functional()
	{
		bl_Is_Functional = true;
	}
	// end Set_Component_As_Functional()

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
