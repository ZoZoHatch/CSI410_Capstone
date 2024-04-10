using Godot;
using System;
using System.Security.Principal;


public partial class Collector : Component
{	
	private Node2D n2d_Anchor;	// the Transform fror the Collector_Pos on the Arm compoent
	private Arm arm;
	
	private bool bl_Auto_Print_On_Angle = false;
	private bool bl_Auto_Print_On_Distance = false;
	private bool bl_Do_Auto_Print_Next_Tick = false;

	private bool bl_Take_Sample_Next_Tick = false;
	private bool bl_Check_Angle_Next_Tick = false;
	private bool bl_Check_Distance_Next_Tick = false;

	[Signal]	// Emitted when a sample is succesfully collected
	public delegate void SampleCollectedEventHandler(Rock_Sample_Struct.Enum_Rock_Types type);

	[Signal]	// Emitted when the collection fails 
	public delegate void CollectionFailedEventHandler(string reason);

	public override void _Ready()
	{
		base._Ready();

		arm = GetParent().GetNode<Arm>("Arm");

		// instantiate t2d_Anchor
		n2d_Anchor = arm.GetNode<Node2D>("%Collector_Anchor");		

		// Intialize nrg_Power.int_Max_Energy
		nrg_Power.Init_Max_Energy(2); 

		// Set nrg_Power.int_Min_Energy
		nrg_Power.Set_Min_Energy(2);

		Subscribe_To_Events();
	}
	// end _Ready()

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		
		GlobalTransform = n2d_Anchor.GlobalTransform;
		
		{	// raycast for getting distance/angle
			// and sample taking

			int int_Scan_Length = 10;
			var spaceState = GetWorld2D().DirectSpaceState;	// the space state of the game world
			Rover rvr = GetParent<Rover>();
			
			// The vector from the Scanner's current rotation times int_Scan_Length
			Vector2 to = new Vector2(MathF.Cos(GlobalRotation), MathF.Sin(GlobalRotation)) * int_Scan_Length;
			to += GlobalPosition;

			// Create a query from the Global position and use the Rover's colison mask and Rid
			var query = PhysicsRayQueryParameters2D.Create(GlobalPosition, to,
			rvr.CollisionMask, new Godot.Collections.Array<Rid> {rvr.GetRid()});

			// get the result of the query in the game world
			var result = spaceState.IntersectRay(query);

			if(result.Count > 0)	// the ray cast hit something
			{						// this is where the bulk of the 
									// collector's logic is

				// Logic for checking distance
				bool within_dist = Check_Distance(GlobalPosition, result["position"].As<Vector2>());

				// Logic for checking angle
				bool within_angle = Check_Angle(to, result["normal"].As<Vector2>());
				
				// Do any auto checks/prints
				if(bl_Do_Auto_Print_Next_Tick)
				{
					bl_Do_Auto_Print_Next_Tick = false;

					if(bl_Auto_Print_On_Angle && within_angle)
					{
						rvr.Send_Message(str_Sender, $"{Name} is properly alligned");
					}
					
					if(bl_Auto_Print_On_Distance && within_dist)
					{
						rvr.Send_Message(str_Sender, $"{Name} is within the proper distance");
					}
				}

				// Logic for taking a sample
				if(bl_Take_Sample_Next_Tick)
				{
					bl_Take_Sample_Next_Tick = false;
					if(within_angle && within_dist)
					{
						GodotObject obj_Rock = result["collider"].As<GodotObject>();
						if (obj_Rock.GetType() == typeof(StaticBody2D))
						{
							Rock_Data rd =((Node)obj_Rock).GetNode<Rock_Data>("Rock_Data");
							rvr.Send_Message(str_Sender, $"Collection Successful");
							rvr.Send_Message(str_Sender, $"Sampled rock was: {rd.Sample_Data.str_Type}");
							EmitSignal(SignalName.SampleCollected, (int)rd.Sample_Data.enm_Type);
						}
						else
						{
							EmitSignal(SignalName.CollectionFailed, "Incompatible object");
						}	
					}
					else if(within_angle && !within_dist)
					{
						rvr.Send_Message(str_Sender, $"{Name} is not close enough to the surface");
					}
					else if(!within_angle && within_dist)
					{
						rvr.Send_Message(str_Sender, $"{Name} is not properly alligned with the surface");
					}
					else
					{
						rvr.Send_Message(str_Sender, $"{Name} is not close enough nor properly alligned");
					}					
				}			
			}
			else if(bl_Check_Angle_Next_Tick
					|| bl_Check_Distance_Next_Tick
					|| bl_Take_Sample_Next_Tick)
			{
				rvr.Send_Message(str_Sender, $"{Name} isn't close enough to a surface to do that \n" +
					$"Position {Name} using the Arm");
			}			
		}	
	}

	public override bool Process_Command(string command)
	{
		/*
		// Collector only takes one number a time, and discards any extra numbers.
		//
		// 0 -> Print the current configuration.
		// 1 -> Check distance from surface.
		// 2 -> Check alignent with surface.
		// 3 -> Toggle automatically printing when 
		// 		the Collector is within the proper distance.
		// 4 -> Toggle automatically printing when
		// 		the Collector is properly alignent with a surface.
		// 5 -> Take a sample.
		//
		// Inorder for the Collector to sucessfully take a sample it:
		// 	*Must be within 10 degrees of being perpendicular with the surface.
		// 	*Must be no further than 5 units from the surface.	
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
				bl_Check_Distance_Next_Tick = true;
				break;

			case '2':
				bl_Check_Angle_Next_Tick = true;
				break;

			case '3':
				bl_Auto_Print_On_Distance = !bl_Auto_Print_On_Distance;
				GetParent<Rover>().Send_Message(str_Sender,
					$"Auto print when within proper distance now set to: {bl_Auto_Print_On_Distance}");
				break;

			case '4':
				bl_Auto_Print_On_Angle = !bl_Auto_Print_On_Angle;
				GetParent<Rover>().Send_Message(str_Sender,
					$"Auto print when properly alligned now set to: {bl_Auto_Print_On_Angle}");
				break;	

			case '5':
				bl_Take_Sample_Next_Tick = true;
				break;	

			default:
				EmitSignal(SignalName.CommandNotRecognized, str_Sender, command);
				return false;	// command isn't recognized
		}

		EmitSignal(SignalName.CommandProcessed, str_Sender);
		return true;	// Command processed
	}	
	// end Process_Command()

	protected override void Subscribe_To_Events()
	{
		base.Subscribe_To_Events();

		arm.ArmFinishedMoving += () => Auto_Check();
	}
	// end Subscribe_To_Events()

	private void Print_Settings()
	{
		Rover rvr = GetParent<Rover>();
		rvr.Send_Message(str_Sender, $"Automatically Print to termial when within proper distance: {bl_Auto_Print_On_Distance}");		
		rvr.Send_Message(str_Sender, $"Automatically Print to termial when properly alligned: {bl_Auto_Print_On_Angle}");
	}
	// end Print_Settings()

	// Logic for checking distance
	private bool Check_Distance(Vector2 col, Vector2 rock)
	{
		float max_distance = 5f;
		float flt_Dist = col.DistanceTo(rock);

		if(bl_Check_Distance_Next_Tick)
		{
			bl_Check_Distance_Next_Tick = false;
			GetParent<Rover>().Send_Message( str_Sender,
				$"{Name}'s distance from the surface is: {flt_Dist} units");
		}

		return flt_Dist < max_distance;
	}
	// end Check_Distance()

	// Logic for checking angle
	// returns true if the angle between dir and norm
	// is less than the angle_range
	// where norm is perpendicular to a surface
	// this means that true if within angle_range
	// of vertical
	private bool Check_Angle(Vector2 dir, Vector2 norm)
	{
		float angle_range = 10f;	// in degrees			
		float coll_angle = dir.AngleTo(norm) * 180 / MathF.PI;
		coll_angle = MathF.Abs(coll_angle);
		coll_angle -= 90f;

		if(bl_Check_Angle_Next_Tick)
		{
			bl_Check_Angle_Next_Tick = false;
			GetParent<Rover>().Send_Message( str_Sender,
				$"{Name}'s angle with the surface is: {coll_angle} degrees");			
		}

		if(coll_angle > 90f - angle_range)
		{
			return true;
		}

		return false;
	}
	// end Check_Angle()

	private void Take_Sample()
	{
		
	}
	// end Take_Sample()

	private void Auto_Check()
	{
		if (bl_Auto_Print_On_Angle || bl_Auto_Print_On_Distance)
		{
			bl_Do_Auto_Print_Next_Tick = true;
		}		
	}
	// end Auto_Chaeck()
}

