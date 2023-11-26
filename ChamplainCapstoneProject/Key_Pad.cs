using Godot;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

public partial class Key_Pad : Control
{
	const int MAX_CHARS_FOR_INPUT = 10;

	RichTextLabel txt_lable;

	string str_currentInput = "";	
	
	/*
	// Signals
	*/

	[Signal]
	public delegate void CodeEnteredEventHandler(string str_Input);

	
	public override void _Process(double delta)
	{
		// make suer the preview is showing the latest code
		refesh_preview();
	}
	// end _Process()

	/*
	// Functions to handle input into Key_Pad
	*/

	// Handle key board input into Key_Pad
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event.GetType() == typeof(InputEventKey))
		{
			var keyEvent = (InputEventKey)@event;

			// Don't proccess held and released events 
			if(keyEvent.Echo || !keyEvent.Pressed)
			{
				return;
			}
			// end if()

			switch (keyEvent.Keycode)
			{
				// Number Keys
				case Key.Key0:
					add_number('0');
					GetViewport().SetInputAsHandled();
					break;
				case Key.Key1:
					add_number('1');
					GetViewport().SetInputAsHandled();
					break;
				case Key.Key2:
					add_number('2');
					GetViewport().SetInputAsHandled();
					break;
				case Key.Key3:
					add_number('3');
					GetViewport().SetInputAsHandled();
					break;
				case Key.Key4:
					add_number('4');
					GetViewport().SetInputAsHandled();
					break;
				case Key.Key5:
					add_number('5');
					GetViewport().SetInputAsHandled();
					break;
				case Key.Key6:
					add_number('6');
					GetViewport().SetInputAsHandled();
					break;
				case Key.Key7:
					add_number('7');
					GetViewport().SetInputAsHandled();
					break;
				case Key.Key8:
					add_number('8');
					GetViewport().SetInputAsHandled();
					break;
				case Key.Key9:
					add_number('9');
					GetViewport().SetInputAsHandled();
					break;

				// Keypad Number Keys
				case Key.Kp0:
					add_number('0');
					GetViewport().SetInputAsHandled();
					break;
				case Key.Kp1:
					add_number('1');
					GetViewport().SetInputAsHandled();
					break;
				case Key.Kp2:
					add_number('2');
					GetViewport().SetInputAsHandled();
					break;
				case Key.Kp3:
					add_number('3');
					GetViewport().SetInputAsHandled();
					break;
				case Key.Kp4:
					add_number('4');
					GetViewport().SetInputAsHandled();
					break;
				case Key.Kp5:
					add_number('5');
					GetViewport().SetInputAsHandled();
					break;
				case Key.Kp6:
					add_number('6');
					GetViewport().SetInputAsHandled();
					break;
				case Key.Kp7:
					add_number('7');
					GetViewport().SetInputAsHandled();
					break;
				case Key.Kp8:
					add_number('8');
					GetViewport().SetInputAsHandled();
					break;
				case Key.Kp9:
					add_number('9');
					GetViewport().SetInputAsHandled();
					break;

				// Non-Number Keys
				case Key.Enter:
					enter_code();
					GetViewport().SetInputAsHandled();
					break;
				case Key.KpEnter:
					enter_code();
					GetViewport().SetInputAsHandled();
					break;
				default:
					break;				
			}
			// end switch (keyEvent.Keycode)
		}
		// end if()
	}
	// end _Input()

	// Handle GUI button presses
	private void _on_btn_0_pressed()
	{
		add_number('0');
	}	
	private void _on_btn_1_pressed()
	{
		add_number('1');
	}
	private void _on_btn_2_pressed()
	{
		add_number('2');
	}
	private void _on_btn_3_pressed()
	{
		add_number('3');
	}
	private void _on_btn_4_pressed()
	{
		add_number('4');
	}
	private void _on_btn_5_pressed()
	{
		add_number('5');
	}
	private void _on_btn_6_pressed()
	{
		add_number('6');
	}
	private void _on_btn_7_pressed()
	{
		add_number('7');
	}
	private void _on_btn_8_pressed()
	{
		add_number('8');
	}
	private void _on_btn_9_pressed()
	{
		add_number('9');
	}
	private void _on_btn_enter_pressed()
	{
		enter_code();
	}
	// end GUI button presses

	private void _on_txt_preview_ready()
	{
		// Set txt_label to the most recent child node		
		txt_lable = GetChild(0).GetChild<RichTextLabel>(-1);
	}	
	// end _on_txt_preview_ready()

	/*
	// Functions for changing the Key_Pad input string str_currentInput
	*/
	protected void add_number(char num)
	{
		if(str_currentInput.Length < MAX_CHARS_FOR_INPUT)
		{
			str_currentInput += num;
		}				
	}
	// end add_number()

	protected void enter_code()
	{
		EmitSignal(SignalName.CodeEntered, str_currentInput);
		clear_input();
	}
	// end enter_code()

	protected void clear_input()
	{
		str_currentInput = "";
	}
	// end clear_input

	private void refesh_preview()
	{
		txt_lable.Text = str_currentInput;
	}
	// end refresh_preview()	

}
