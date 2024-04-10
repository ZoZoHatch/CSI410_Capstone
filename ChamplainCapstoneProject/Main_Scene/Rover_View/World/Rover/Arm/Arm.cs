using Godot;
using System;
using System.Collections.Generic;
using System.Security.Principal;


public partial class Arm : Component
{
	// Vars for articulating the Arm
	private Timer tmr;	// A timer for regulating dequeueing commands
	
	private RigidBody2D rb_Shoulder;
	private RigidBody2D rb_Elbow;
	private RigidBody2D rb_Wrist;

	// enum for controlling which joint is currently being rotated
	public enum Enum_Joints
	{
		INVALID_JOINT = -1,
		SHOULDER,
		ELBOW,
		WRIST
	}

	private Enum_Joints enm_Current_Joint = Enum_Joints.INVALID_JOINT;

	private Queue<char> que_Commands = new Queue<char>();
	
	float flt_Turn_Time = 2.0f;	// in seconds
	float flt_Turn_Dist = 10f; // in radians			
	

	private int int_Turn_Dir = 0;
	// int_Turn_Dir is 	1 for clockwise
	//				   -1 for counter clockwise
	

	[Signal]	// emitted whenever the arm finishes a movement command
	public delegate void ArmMovedEventHandler();	

	[Signal]	// emitted when the command que is emptied
	public delegate void ArmFinishedMovingEventHandler();

	[Signal]	// emitted if a joint is at the min or max angle
	public delegate void JointAtContraintBoundsEventHandler(string joint, bool is_min);

	public override void _Ready()
	{
		base._Ready();

		// instantiate joints
		rb_Shoulder =GetNode<RigidBody2D>("%Rover_Joint");
		rb_Elbow = GetNode<RigidBody2D>("%Center_Joint");
		rb_Wrist = GetNode<RigidBody2D>("%Collector_Joint");		

		// Instantiate tmr
		tmr = GetNode<Timer>("Timer");

		// Intialize nrg_Power.int_Max_Energy
		nrg_Power.Init_Max_Energy(3); 

		// Set nrg_Power.int_Min_Energy
		nrg_Power.Set_Min_Energy(3);

		Subscribe_To_Events();
	}
	// end _Ready()	

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		
		if(!tmr.IsStopped())
		{
			float flt_Turn_Speed = flt_Turn_Dist / flt_Turn_Time * int_Turn_Dir;	// in degs/sec

			RigidBody2D rb;

			switch(enm_Current_Joint)
			{				
				case Enum_Joints.SHOULDER:
					rb = rb_Shoulder;
					break;

				case Enum_Joints.ELBOW:
					rb = rb_Elbow;
					break;

				case Enum_Joints.WRIST:
					rb = rb_Wrist;
					break;

				default:
					return;
			}

			rb.RotationDegrees += flt_Turn_Speed * (float)delta;
		}
	}
	// end _PhysicsProcess()

	public override bool Process_Command(string command)
	{
		if(base.Process_Command(command))
		{
			return true;	// Return true if command was handled by bas component
		}

		/*		
		//	Commands are added to a queue and processed sequentially.
		*/

		foreach(char c in command)
		{
			que_Commands.Enqueue(c);
		}
		
		if(tmr.IsStopped())
		{
			Update_Articulation_Settings();	// Starts dequeueing commands
		}
		
		EmitSignal(SignalName.CommandProcessed, str_Sender, command);
		return true;	// Command processed		
	}	
	// end Process_Command()

	protected override void Subscribe_To_Events()
	{
		base.Subscribe_To_Events();
		
		tmr.Timeout += () => EmitSignal(SignalName.ArmMoved);
		tmr.Timeout += () => Update_Articulation_Settings();
	}
	// end Subscribe_To_Events()

	private void Update_Articulation_Settings()
	{
		/*
		// Arm processes commands in one(1) character chunks.
		//
		// Controls for Collector joint:
		// 7 -> rotate counter-clockwise
		// 8 -> get joints current angle
		// 9 -> rotate clockwise
		//
		// Controls for center joint:
		// 4 -> rotate counter-clockwise
		// 5 -> get joints current angle
		// 6 -> rotate clockwise
		//
		// Controls for Rover joint:
		// 1 -> rotate counter-clockwise
		// 2 -> get joints current angle
		// 3 -> rotate clockwise
		//
		// Commands are added to a queue and processed sequentially.
		//
		// Arm has a high minimum power requirement, but
		// once that requirement is met it works optimally.
		//
		// Arm doesn't benifit from increasing the power
		// beyond the minimum.
		*/

		enm_Current_Joint = Enum_Joints.INVALID_JOINT;	// reset the current active joint

		if(que_Commands.Count == 0)
		{	
			EmitSignal(SignalName.ArmFinishedMoving);
			return;	// No Commands left to process, so return
		}

		char cmd = que_Commands.Dequeue();		

		switch (cmd)
		{
			case '7':	// Rotate Collector (counter clockwise)			
				int_Turn_Dir = -1;
				enm_Current_Joint = Enum_Joints.WRIST;
				break;	

			case '8':	// print Collector joint's current angle
				int_Turn_Dir = 0;							
				break;

			case '9':	// Rotate Collector (clockwise)
				int_Turn_Dir = 1;	
				enm_Current_Joint = Enum_Joints.WRIST;						
				break;

			case '4':	// Rotate fore arm (counter clockwise)	
				int_Turn_Dir = -1;
				enm_Current_Joint = Enum_Joints.ELBOW;				
				break;	

			case '5':	// Print fore arm's current angle
				int_Turn_Dir = 0;				
				break;

			case '6': 	// Rotate fore arm (clockwise)
				int_Turn_Dir = 1;
				enm_Current_Joint = Enum_Joints.ELBOW;		
				break;

			case '1':	// Rotate upper arm (counter clockwise)	
				int_Turn_Dir = -1;
				enm_Current_Joint = Enum_Joints.SHOULDER;			
				break;	

			case '2':	// Print upper arm's current angle
				int_Turn_Dir = 0;				
				break;

			case '3':	// Rotate upper arm (clockwise)
				int_Turn_Dir = 1;
				enm_Current_Joint = Enum_Joints.SHOULDER;			
				break;

			default: // If case is deault command isn't recongnized, 
					// call Update_Articulation_Settings() again
				EmitSignal(SignalName.CommandNotRecognized, str_Sender, cmd);
				Update_Articulation_Settings();	
				return;		
		}

		// start the time, also starting the rotation in _PhysicsProcess()
		tmr.Start(flt_Turn_Time);
	}
	// end Update_Articulation_Settings()

	// Depricated
	private void Print_Joint_Angle(PinJoint2D joint)
	{
		GetParent<Rover>().Send_Message(str_Sender, 
		$"The angle of {joint.Name} is: {(int)joint.GetChild<Node2D>(0).RotationDegrees}");
	}
	// end Print_Joint_Angle()

	private void Print_Joint_Angle(RigidBody2D joint)
	{
		GetParent<Rover>().Send_Message(str_Sender, 
		$"The angle of {joint.Name} is: {(int)joint.RotationDegrees}");
	}
}

