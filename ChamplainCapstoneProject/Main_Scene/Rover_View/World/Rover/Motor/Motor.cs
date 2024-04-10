using Godot;
using System;
using System.Collections.Generic;
using System.Security.Principal;


public partial class Motor : Component
{
	// Character body 2D for the Rover
	private CharacterBody2D cb_Rover;
	
	// Vars for movement
	private Timer tmr;	// A timer for regulating dequeueing commands
	private Queue<char> que_Commands = new Queue<char>();

	private float flt_Move_Speed = 0.0f;
	private float flt_Turn_Speed = 0.0f;

	// Signals
	[Signal]	// This signal is emitted whenever que_Commands is emptied
	public delegate void MovementFinishedEventHandler();

	[Signal]	// This signal is emitted whenever the rover rotates
	public delegate void ChangedDirectionEventHandler();

	[Signal]	// This signal is emitted whenever a que_Commands is dequeued
	public delegate void MovementCommandStartedEventHandler(int move_dir, int turn_dir);
	
	[Signal]	// This signal is emitted whenever tmr.Timeout is recieved
	public delegate void MovementCommandFinishedEventHandler();

	[Signal]	// This signal is emitted if there's a collision
	public delegate void RoverCollidedEventHandler(float speed);

	public override void _Ready()
	{
		base._Ready();

		// Instantiate cb_Rover
		cb_Rover = GetParent<CharacterBody2D>();

		// Instantiate tmr
		tmr = GetNode<Timer>("Timer");

		// Intialize nrg_Power.int_Max_Energy
		nrg_Power.Init_Max_Energy(5); 

		// Set nrg_Power.int_Min_Energy
		nrg_Power.Set_Min_Energy(1);		

		Subscribe_To_Events();
	}
	// end _Ready()

	public override void _PhysicsProcess(double delta)
	{		
		base._PhysicsProcess(delta);
		
		cb_Rover.Rotation += flt_Turn_Speed * (float)delta;
		cb_Rover.Velocity = cb_Rover.Transform.X * flt_Move_Speed;
		var col = cb_Rover.MoveAndCollide(cb_Rover.Velocity * (float)delta);
		
		// handle any collisions that may have occured by 
		// halting the rover
		// emptying the command que
		// and stopping tmr
		if(col != null)
		{
			EmitSignal(SignalName.RoverCollided, cb_Rover.Velocity.Length());
			que_Commands.Clear();
			tmr.Stop();
			Halt_Movement();
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
			Update_Movement_Settings();	// Starts dequeueing commands
		}
		
		EmitSignal(SignalName.CommandProcessed, str_Sender, command);
		return true;	// Command processed
	}	
	// end Process_Command()

	protected override void Subscribe_To_Events()
	{
		base.Subscribe_To_Events();	
		
		tmr.Timeout += () => Update_Movement_Settings();
		tmr.Timeout += () => Clamp_Rotation();
		tmr.Timeout += () => EmitSignal(SignalName.MovementCommandFinished);
	}
	// end Subscribe_To_Events()

	protected override void Load_Config_File()
	{

	}
	// end Load_Config_File()

	private void Update_Movement_Settings()
	{
		/*
		//	Motor processes commands in one(1) character chunks.		//
		//	 
		//				 forward
		//		fwd left    |    fwd right
		//				\   |   /
		//				 7  8  9
		//  turn left--- 4     6 ---turn right
		//				 1  2  3
		//				/   |   \
		//	   bck left     |     bck right
		//				 backward
		//
		//	./5 -> break/wait
		*/		

		Halt_Movement();	// reset movement vars before moving

		if(que_Commands.Count == 0)
		{	
			EmitSignal(SignalName.MovementFinished);			
			return;	// No Commands left to process, so halt and return						
		}

		// the distance that the rover moves for each forward/backward command // in world units
		float flt_Move_Dist = 10.0f * nrg_Power.int_Allocated_Energy;	

		float flt_Turn_Dist = 45.0f * (MathF.PI / 180.0f);	// the amount that the rover turns for each right/left command 
		// first num is in degrees, is than converted to radians

		
		float flt_Move_And_Turn_Time = 1.0f;
		tmr.Start(flt_Move_And_Turn_Time);	

		int int_Move_Dir;
		int int_Turn_Dir;			

		char cmd = que_Commands.Dequeue();		

		// Switch statement sets Dir vars
		// 1 	=> Forward/Clockwise
		// 0 	=> Stopped
		// -1	=> Backwards/CounterClockwise
		switch (cmd)
		{
			case '8':	// Forwards						
				int_Move_Dir = 1;
				int_Turn_Dir = 0;
				break;
				
			case '7':	// Forward Left (counter clockwise)
				int_Move_Dir = 1;
				int_Turn_Dir = -1;
				break;			

			case '9':	// Forward Right (clockwise)
				int_Move_Dir = 1;
				int_Turn_Dir = 1;
				break;

			case '2':	// Backwards
				int_Move_Dir = -1;
				int_Turn_Dir = 0;
				break;	

			case '1':	// Backward Left (clockwise)
				int_Move_Dir = -1;
				int_Turn_Dir = 1;
				break;

			case '3':	// Backward Right (counter clockwise)
				int_Move_Dir = -1;
				int_Turn_Dir = -1;
				break;

			case '4':	// Turn Left (counter clockwise)
				int_Move_Dir = 0;
				int_Turn_Dir = -1;
				break;	

			case '6': 	// Turn Right (clockwise)
				int_Move_Dir = 0;
				int_Turn_Dir = 1;
				break;

			default: // If case is deault halt and return
				Halt_Movement();
				return;			
		}		

		EmitSignal(SignalName.MovementCommandStarted, int_Move_Dir, int_Turn_Dir);

		// Set Speed vars
		if(int_Turn_Dir != 0)	// Moving while turing	// need to calc arc length using formula: radius * theta where theta is in radians
		{
			flt_Move_Speed = flt_Move_Dist*flt_Turn_Dist / flt_Move_And_Turn_Time * int_Move_Dir;
		}
		else	// Moving w/o turning
		{
			flt_Move_Speed = flt_Move_Dist / flt_Move_And_Turn_Time * int_Move_Dir;
		}
		
		flt_Turn_Speed = flt_Turn_Dist / flt_Move_And_Turn_Time * int_Turn_Dir;		
	}	
	// end Get_Movement_Settings()

	// Helper function to set the velocity movement vars to 0
	private void Halt_Movement()
	{
		flt_Move_Speed = 0.0f;
		flt_Turn_Speed = 0.0f;				
	}
	// end Halt_Movement()	

	// Helper function to prevent over/underturning that can occure from 
	// floating point rounding errors
	// Clamps the rover's rotation to the nearest 45 degree angle, should be inperceptable 
	// to the user
	private void Clamp_Rotation()
	{
		float flt_Angle = cb_Rover.Transform.X.Angle() * 180 / MathF.PI;
		int int_Clamped = (int)MathF.Round(flt_Angle / 45.0f) * 45;
		if(int_Clamped == 360)
		{
			int_Clamped = 0;
		}

		cb_Rover.RotationDegrees = int_Clamped;
	}
	// end Clamp_Rotation

	// Helper function to calculate the time each movement command takes
	// Depricated in favor of simpler system that scales the move distance
	// and keeps the duration constant.
	private float Get_Move_Duration()
	{
		int int_Power_Difference = nrg_Power.int_Max_Energy - nrg_Power.int_Allocated_Energy; 
		return (0.047f * (int_Power_Difference^2)) + (0.1f * int_Power_Difference) + 1;
		// The amount of time each movement operation takes in seconds
		// More Power allocated == Lower Time
		// graph of powe curve: https://www.desmos.com/calculator/fxfecilncv
		// x = 0 is max power, x = 7 is min power
	}
}

