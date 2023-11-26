using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Rover : Node2D
{
	CharacterBody2D body;

	// a queue to hold the commands sent to the rover
	Queue<char>  que_chr_commands = new Queue<char>();

	// var to control how often commands get processed	
	double dbl_tick_speed = .25;	// in seconds
	
	// var to track time and compare to tick_speed
	double dbl_delta_sum = 0; // in seconds

	// vars for controling the movement of the rover
	float flt_speed = 10;
	float flt_rotation_speed_deg = 45;	// in degrees
	float flt_rotation_speed_rad;	// in degrees


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		flt_rotation_speed_rad = flt_rotation_speed_deg * (MathF.PI / 180);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		dbl_delta_sum += delta;

		if(dbl_delta_sum > dbl_tick_speed)
		{
			dbl_delta_sum = 0;
			proccess_next_command();
		}
		
		body.MoveAndSlide();
	}

	// Process packets from Mission_Control
	public void process_packet(string str_packet)
	{
		GD.Print("recieved packet: " + str_packet);

		// The first two characters of the packet are the address
		// of the component receving the command
		int int_address = (str_packet[0].ToString() + str_packet[1].ToString()).ToInt();

		// The rest of the packet is the command
		foreach (var c in str_packet.Substring(2).ToCharArray())
		{
			// Append new command chars into the queue
			que_chr_commands.Enqueue(c);
		}
		
		// Debug print statements
		//GD.Print("address: " + int_address.ToString());
		//GD.Print("command: " + que_chr_commands.ToString());
	}
	// end process_packet()

	private void proccess_next_command()
	{
		if(que_chr_commands.Count == 0)
		{			
			return;
		}

		var cmd = que_chr_commands.Dequeue();
		switch (cmd)
		{
			case '1':
			break;
			case '2':	// Reverse
			body.Velocity = body.UpDirection * -flt_speed;
			break;
			case '3':
			break;
			case '4':	// Rotate left
			body.Rotate(-flt_rotation_speed_rad);
			break;
			case '5':	// Stop
			body.Velocity = Vector2.Zero;
			break;
			case '6':	// Rotate right
			body.Rotate(flt_rotation_speed_rad);
			break;
			case '7':
			break;
			case '8':	// Drive forward
			body.Velocity = body.UpDirection * flt_speed;
			break;
			case '9':
			break;
			case '0':	
			break;

			default:
			break;
		}
		// end switch(cmd)
		
		GD.Print("velocity: " + body.Velocity);
				
	}
	// end process_next_command()

	/*
	// Process signals
	*/
	private void _on_character_body_2d_ready()
	{
		body = GetNode<CharacterBody2D>("CharacterBody2D");
	}
}



