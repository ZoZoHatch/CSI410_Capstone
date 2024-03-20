using Godot;
using System;
using System.Dynamic;
using System.Security.Principal;
using System.Collections.Generic;

public partial class Scanner : Component
{
	private Timer tmr;	

	private float flt_Scan_Angle = 359.9f;	// in degrees
	private float flt_Scan_Time = 5.0f;		// in seconds
	private float flt_Time_To_Scan = 0.0f;
	private int int_Scanner_Turn_Dir = 0;	// -1 || 0 || 1
	int int_Scan_Length = 1000;

	private int int_Num_Scans_Per_Energy = 30;

	private bool bl_Run_Auto_Scan_On_Move = false;	
	private bool bl_Auto_Scan_Rover_On_Arm = false;
	
	[Signal]
	public delegate void ScanStartedEventHandler(float duration, int direction, int range);	
		// emitted when the scanner start it's scanning process

	[Signal]
	public delegate void ScanMadeEventHandler(Vector2 position);		
		// emitted when an individual scan is made
		// Contains the position of any collision that may happen from the collision
		// of there is no collision postion = Vector2.Zero

	[Signal]
	public delegate void RoverScannedEventHandler();	
		// emitted when the scanner scans the Rover

	[Signal]
	public delegate void ScanCompletedEventHandler();	
		// emitted when the scanning process is finished

	public override void _Ready()
	{
		base._Ready();

		tmr = GetNode<Timer>("Timer");

		// Intialize nrg_Power.int_Max_Energy
		nrg_Power.Init_Max_Energy(3); 

		// Set nrg_Power.int_Min_Energy
		nrg_Power.Set_Min_Energy(1);

		Subscribe_To_Events();
	}
	// end _Ready()

	protected override void Subscribe_To_Events()
	{
		base.Subscribe_To_Events();			
		
		tmr.Timeout += () => Stop_Scanning();

		Motor mtr = GetParent<Rover>().GetNode<Motor>("Motor");
		mtr.MovementFinished += () => Auto_Scan_Environment(); 
	}
	// end Subscribe_To_Events()

	// Physics process used for timeing each scan
	public override void _PhysicsProcess(double delta)	
	{
		base._PhysicsProcess(delta);

		if(!tmr.IsStopped())
		{				
			float flt_Scanner_Turn_Spd = flt_Scan_Angle / flt_Scan_Time;			
			float flt_Scan_Interval = flt_Scan_Time / (int_Num_Scans_Per_Energy * nrg_Power.int_Allocated_Energy);

			flt_Time_To_Scan += (float)delta;
			if(flt_Time_To_Scan >= flt_Scan_Interval)
			{
				flt_Time_To_Scan = 0.0f;
				Make_Scan();
			}

			RotationDegrees += flt_Scanner_Turn_Spd * int_Scanner_Turn_Dir * (float)delta;
		}
	}
	// end _PhysicsProcess

	public override bool Process_Command(string command)
	{
		/*
		// Scanner takes commands one(1) character at a time and discards any other numbers.
		//
		// 0 -> Print the current configuration. 
		// 1 -> Scan the environment around the Rover.
		// 2 -> Scan the Rover itself
		// 	This updates the Rover preview to show the orientation of the Rover 
		// 	at the time of the scan
		//
		// 3 -> Toggle automatically scanning when the Rover moves. 
		// 4 -> Toggle automatically scanning the Rover when the arm moves	
		//
		// Each scan environment scan takes 5 seconds to make.
		// The Scanner scans in the counter clockwise direction.
		//
		// Increasing the Scanner's energy increases the detail of each scan.
		*/

		if(base.Process_Command(command))
		{
			return true;	// Return true if command was handled by bas component
		}		
		
		switch (command[0])
		{
			case '0':
				Print_Settings();
				break;

			case '1':
				Start_Scanning();
				break;

			case '2':
				Scan_Rover();
				break;

			case '3':
				bl_Run_Auto_Scan_On_Move = !bl_Run_Auto_Scan_On_Move;
				break;

			case '4':
				bl_Auto_Scan_Rover_On_Arm = !bl_Auto_Scan_Rover_On_Arm;
				break;			

			default:
				EmitSignal(SignalName.CommandNotRecognized, str_Sender, command);
				return false;	// command isn't recognized
		}

		EmitSignal(SignalName.CommandProcessed, str_Sender, command);
		return true;	// Command processed
	}	
	// end Process_Command()		
	
	private void Scan_Rover()
	{
		EmitSignal(SignalName.RoverScanned);
	}
	// end Scan_Rover()

	private void Start_Scanning()
	{
		if(tmr.IsStopped() && nrg_Power.Has_Enough_Power() && bl_Is_Functional)
		{						
			int_Scanner_Turn_Dir = -1;
			tmr.Start(flt_Scan_Time);
			EmitSignal(SignalName.ScanStarted, flt_Scan_Time, int_Scanner_Turn_Dir, int_Scan_Length);
		}
	}
	// end Start_Scanning()
		
	private void Make_Scan()
	{		
		var spaceState = GetWorld2D().DirectSpaceState;	// the space state of the game world

		Rover rvr = GetParent<Rover>();
		
		// The vector from the Scanner's current rotation times int_Scan_Length
		Vector2 to = new Vector2(MathF.Cos(GlobalRotation), MathF.Sin(GlobalRotation)) * int_Scan_Length;

		// Create a query from the Global position and use the Rover's colison mask and Rid
		var query = PhysicsRayQueryParameters2D.Create(GlobalPosition, GlobalPosition + to,		
		rvr.CollisionMask, new Godot.Collections.Array<Rid> {rvr.GetRid()});

		// get the result of the query in the game world
		var result = spaceState.IntersectRay(query);

		Vector2 collision = Vector2.Zero;

		if(result.Count > 0)
		{	
			// extract the collision position from result, if there is one
			collision = (Vector2)result["position"];
		}
		
		// need to convert the global cordinates to local, relative to the World the Rover is in
		if(collision != Vector2.Zero)
		{	// only if there was a collision
			Node2D world = rvr.GetParent<Node2D>();
			collision = world.ToLocal(collision);
		}		

		EmitSignal(SignalName.ScanMade, collision);
	}
	// end Make_Scan()

	private void Stop_Scanning()
	{
		Rotation = 0.0f;
		int_Scanner_Turn_Dir = 0;		
		EmitSignal(SignalName.ScanCompleted);
	}
	//end Stop_Scanning()

	private void Print_Settings()
	{
		Rover rvr = GetParent<Rover>();
		rvr.Send_Message(str_Sender, $"Automatically Scan when Motor stops moving: {bl_Run_Auto_Scan_On_Move}");		
		rvr.Send_Message(str_Sender, $"Automatically Scan when Arm moves: {bl_Auto_Scan_Rover_On_Arm}");		
	}
	// end Print_Settings()

	private void Auto_Scan_Environment()
	{
		if(bl_Run_Auto_Scan_On_Move) { Start_Scanning(); }
	}
	// end Auto_Scan_Environment()

	private void Auto_Scan_Rover()
	{
		if(bl_Auto_Scan_Rover_On_Arm) { Scan_Rover(); }
	}
	// end Auto_Scan_Rover()
}

