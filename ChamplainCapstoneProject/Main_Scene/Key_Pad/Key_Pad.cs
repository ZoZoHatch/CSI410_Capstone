using Godot;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class Key_Pad : Control
{	
	// Constant variables used for formating Key_Pad output and preview 
	private const int MAX_INPUT_LENGTH = 7;
	private const int SEPERATOR_INDEX = 1;
	private const string SEPERATOR_CHAR = "|";

	// Signal for returning the raw Key_Pad output (w/o seperating)
	[Signal]
	public delegate void KeyPadReturnedRawEventHandler(string output);

	// Signal for returning the formated Key_Pad output (w/ seperating)
	[Signal]
	public delegate void KeyPadReturnedFormatedEventHandler(string preSeperator, string postSeperator);

	// Input_Preview is used to show the current input into the Key_Pad
	private Label lbl_Input_Preview;
	// Current input into the Key_Pad 
	private string str_Input = "";
	
	public override void _Ready() 
	{	
		// Instantiate lbl_Input_Preview
		NodePath nodePath = "Key_Pad_Panel/Label_PanelContainer/Label_MarginContainer/Input_Preview";
		lbl_Input_Preview = GetNode<Label>(nodePath);

		// Set up Key_Pad event listeners
		Subscribe_To_Events();
	}
	// end _Ready()
	
	private void Subscribe_To_Events()
	{
		// Subscribe to events for the number buttons' presses
		NodePath nodePath = "Key_Pad_Panel/0_MarginContainer/Button_0";		
		GetNode<Button>(nodePath).Pressed += () => Button_Pressed('0');
		nodePath = "Key_Pad_Panel/1-9_MarginContainer/1-9_AspectRatioContainer/1-9_GridContainer/Button_1";
		GetNode<Button>(nodePath).Pressed += () => Button_Pressed('1');	
		nodePath = "Key_Pad_Panel/1-9_MarginContainer/1-9_AspectRatioContainer/1-9_GridContainer/Button_2";
		GetNode<Button>(nodePath).Pressed += () => Button_Pressed('2');
		nodePath = "Key_Pad_Panel/1-9_MarginContainer/1-9_AspectRatioContainer/1-9_GridContainer/Button_3";
		GetNode<Button>(nodePath).Pressed += () => Button_Pressed('3');
		nodePath = "Key_Pad_Panel/1-9_MarginContainer/1-9_AspectRatioContainer/1-9_GridContainer/Button_4";
		GetNode<Button>(nodePath).Pressed += () => Button_Pressed('4');
		nodePath = "Key_Pad_Panel/1-9_MarginContainer/1-9_AspectRatioContainer/1-9_GridContainer/Button_5";
		GetNode<Button>(nodePath).Pressed += () => Button_Pressed('5');
		nodePath = "Key_Pad_Panel/1-9_MarginContainer/1-9_AspectRatioContainer/1-9_GridContainer/Button_6";
		GetNode<Button>(nodePath).Pressed += () => Button_Pressed('6');
		nodePath = "Key_Pad_Panel/1-9_MarginContainer/1-9_AspectRatioContainer/1-9_GridContainer/Button_7";
		GetNode<Button>(nodePath).Pressed += () => Button_Pressed('7');
		nodePath = "Key_Pad_Panel/1-9_MarginContainer/1-9_AspectRatioContainer/1-9_GridContainer/Button_8";
		GetNode<Button>(nodePath).Pressed += () => Button_Pressed('8');	
		nodePath = "Key_Pad_Panel/1-9_MarginContainer/1-9_AspectRatioContainer/1-9_GridContainer/Button_9";
		GetNode<Button>(nodePath).Pressed += () => Button_Pressed('9');

		// Subscribe to event for the enter botton's press
		nodePath = "Key_Pad_Panel/Enter_MarginContainer/Button_Enter";		
		GetNode<Button>(nodePath).Pressed += () => Enter_Button_Pressed();

		// Subscribe to event for the period button's press
		nodePath = "Key_Pad_Panel/Period_MarginContainer/Button_Period";
		GetNode<Button>(nodePath).Pressed += () => Button_Pressed('.');
	}
	// end Subscribe_To_Events()

	private void Button_Pressed(char character)
	{
		if(str_Input.Length < MAX_INPUT_LENGTH)
		{
			str_Input = $"{str_Input}{character}";
			Update_Preview();
		}
	}
	// end Number_Button_Pressed()

	private void Enter_Button_Pressed()
	{
		Return_Input();
		Clear_Input();
		Update_Preview();
	}
	// end Enter_Button_Pressed()	

	private void Update_Preview()
	{		
		string str_Preview = str_Input;

		// Fill str_Preview with spaces so that it's long enough for formating		
		str_Preview = Fill_String(str_Preview);				

		// String fromating to deliniate the address part of the input from the command part
		str_Preview = str_Preview.Insert(SEPERATOR_INDEX, SEPERATOR_CHAR);

		lbl_Input_Preview.Text = str_Preview;
	}
	// end Update_Preview()

	private void Clear_Input()
	{
		str_Input = "";
	}
	// end Clear_Input()

	private void Return_Input()
	{
		if(str_Input == "")	{ return; }	// If str_Input is empty, don't send any signals 

		str_Input = Fill_String(str_Input);	// Fill str_Input so that it's long enough for accessors
		string str_Pre = str_Input.Substring(0, SEPERATOR_INDEX);
		string str_Post = str_Input.Substring(SEPERATOR_INDEX);

		EmitSignal(SignalName.KeyPadReturnedRaw, str_Input);
		EmitSignal(SignalName.KeyPadReturnedFormated, str_Pre, str_Post);
	}
	// end Return_Input()	

	// A helper function to fill a string with white space so that 
	// it's long enough for formating index accessors
	private string Fill_String(string str)
	{
		while(str.Length <= SEPERATOR_INDEX)
		{
			str = $"{str} ";
		}
		return str;
	}	
}


