using Godot;
using System;

public partial class Output_Terminal : Control
{
	// Array of strings corrolating Enum_Message_Types to strings
	public string[] Arr_Message_Type_Strings =
	{
		"MESSAGE",
		"#INFO#",
		"*ERROR*",
		"!WARNING!",
		"*!EMRGNCY!*"
	};

	// The RichTextLable used for printing messages
	private RichTextLabel rtl_Output;

	// Text animation controls
	private float flt_Char_Print_Speed = 1.0f/24.0f;		// The number of characters that get printed every second	
	private float flt_New_Line_Pause_Duration = .5f;	// How long the animation pauses on a new line character in seconds
	private Timer tmr_New_Char;	
	
	private int int_Max_Lines = 127;		// Maximum number of lines in rtl_Output.txt

	private File_Reader fr_Reader;
	public override void _Ready()
	{
		// Instantiate rtl_Output
		NodePath nodePath = (NodePath)"%RichTextLabel";
		rtl_Output = GetNode<RichTextLabel>(nodePath);
		rtl_Output.VisibleCharacters = 0;

		// Instantiate fr_Reader
		fr_Reader = GetNode<File_Reader>("File_Reader");

		// Instantiate Timers
		tmr_New_Char = GetNode<Timer>("NewCharTimer");				

		// Set up Output_Termial event listeners
		Subscribe_To_Events();
	}
	// end _Ready()
	
	private void Subscribe_To_Events()
	{
		// Subscribing to Key_Pad returned event in order to test Output_Terminal( using the Key_Pad
		//GetNode<Key_Pad>("%Key_Pad").KeyPadReturnedFormated += (string first, string second) 
			//=> Print_File("Test_File.txt", File_Reader.Enum_File_Types.CONFIG);
			//=> Print_Message(new Message_Struct((Message_Struct.Enum_Message_Types)(first.ToInt() % 5), first, second));
		
		// Subscribing to File_Reader signals
		GetNode<File_Reader>("File_Reader").FileOpenedSucessfully += (Message_Struct msg) => Print_Message(msg);

		// Subscribing to Timer signals
		tmr_New_Char.Timeout += () => Increase_Character_Count();		
	}
	// end Subscribe_To_Events()	

	public void Print_Message(Message_Struct msg)
	{
		//string str_Current_Time = Time.GetTimeStringFromSystem();	// Currently System Time, might switch it out with a fictional in-game time

		// Message format: [  Time  ][      Type      ][     Sender     ]: Message body
		// Message format v02: [Type][Sender]: Message body	// Cut time field because it looked cluttered
		string str_Formated_Message = String.Format 	
			("[{0}][{1}]: {2}", 
			Get_Type_As_String(msg.enm_Type), msg.str_Sender, msg.str_Message);
		Print_Line(str_Formated_Message);
	}
	// end Print_Message()

	private void Print_Line(string line)
	{
		rtl_Output.Text += $"{line}\n";
		if(tmr_New_Char.IsStopped())	
		{
			Animate_Text_Printing();
		}
	}
	// end Print_Line()

	public void Print_File(string filepath, File_Reader.Enum_File_Types type)
	{
		// try to open file
		try
		{
			fr_Reader.Open_File(filepath, type);

			do 	// Read each line of file until EOF
			{
				// Print the line
				Print_Line(fr_Reader.Read_Next_Line());				
				
			} while (!fr_Reader.At_End_Of_File());			
			
			// close file
			fr_Reader.Close_File();
		}
		// catch exceptions
		catch(File_Reader.File_ReaderException e)
		{
			Print_Message(e.msg_Message);
		}	
	}
	// end Print_File()

	// Helper function to increase the character count of rtl_Output
	private void Increase_Character_Count()
	{
		rtl_Output.VisibleCharacters++;
		Animate_Text_Printing();			
	}
	// end Increase_Character_Count()

	// Function for animating text using tmr_New_Char to control when a new 
	// character is made visible
	private void Animate_Text_Printing()
	{
		if(rtl_Output.VisibleCharacters < rtl_Output.Text.Length)
		{
			switch (rtl_Output.Text[rtl_Output.VisibleCharacters])	// Get the last visible character
			{
				case '\n':	// in case of \n wait longer to print the next char
					tmr_New_Char.Start(flt_New_Line_Pause_Duration);					
					break;

				default:	// default case uses flt_Char_Print_Speed
					tmr_New_Char.Start(flt_Char_Print_Speed);
					break;
			}			
		}
	}
	// end Animate_Text_Printing()

	// Helper functions to convert Mesage_Types enum to a human-readable string
	private string Get_Type_As_String(Message_Struct.Enum_Message_Types type)
	{
		return Arr_Message_Type_Strings[(int)type];
	}  	
	// end Get_Type_As_String()
}
