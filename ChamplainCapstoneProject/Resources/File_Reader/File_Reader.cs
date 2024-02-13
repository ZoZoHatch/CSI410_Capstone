using Godot;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

public partial class File_Reader : Node
{
	private StreamReader sr_StreamReader = null;	

	public enum Enum_Reader_Exceptions
	{
		INVALID_EXCEPTION = -1,
		FILE_DNE,
		BUSY, 
		INVALID_FILE_TYPE,      
		FINAL_EXCEPTION
	}

	public enum Enum_File_Types
	{
		INVALID_TYPE = -1,
		GENERIC,
		HELP,
		CONFIG,
		FINAL_TYPE
	}

	// File directory name variables
	private string str_Text_File_Directory = "Resources/Text_Files/";	
	private string str_Help_File_Subdirectoty = "Help_Files/";
	private string str_Config_File_Subdirectory = "Config_Files/";

	// Sender field for messages		
	string str_Sender;

	
	[Signal]
	public delegate void FileOpenedSucessfullyEventHandler(Message_Struct message);
	public override void _Ready()
	{
		// Initialize Message_Structs
		str_Sender = GetParent().Name;					
		
		

		
	}
	// end _Ready()

	// Open StreamReader
	private void Open_File(string filename)	// Privated to prevent confusion w/ overload // Type needs to be specified for every file
	{
		// Format File_Path based on given filename
		string str_File_Path = $"{str_Text_File_Directory}{filename}";
		//string str_File_Path = filename; // was used to help with debugging

		// Check if the file doesn't exist
		if(!File.Exists(str_File_Path))
		{            
			Message_Struct msg_File_DNE = new Message_Struct(
				Message_Struct.Enum_Message_Types.ERROR, str_Sender, $"The file {filename} does not exist");
			throw new File_ReaderException(msg_File_DNE, Enum_Reader_Exceptions.FILE_DNE);           
		}

		// Check if sr_StreamReader is already open
		if(sr_StreamReader != null)
		{
			Message_Struct msg_Busy = new Message_Struct(
				Message_Struct.Enum_Message_Types.ERROR, str_Sender, "File_Reader is busy with another file");
			throw new File_ReaderException(msg_Busy, Enum_Reader_Exceptions.BUSY);
		}

		// Open file
		sr_StreamReader = new StreamReader(str_File_Path);

		Message_Struct msg_File_Opened = new Message_Struct(
			Message_Struct.Enum_Message_Types.INFORAMTION, str_Sender, $"{filename} Opened Successfully");
		EmitSignal(SignalName.FileOpenedSucessfully, msg_File_Opened);
	}
	// end Open_File()

	// an overload of Open_File so that different file types can
	// be kept in sub directories without every call needing to
	// know what the sub directory is called
	public void Open_File(string filename, Enum_File_Types type)
	{
		switch (type)
		{
			case Enum_File_Types.GENERIC:
				Open_File(filename);
				break;

			case Enum_File_Types.HELP:
				Open_File($"{str_Help_File_Subdirectoty}{filename}");
				break;

			case Enum_File_Types.CONFIG:
				Open_File($"{str_Config_File_Subdirectory}{filename}");
				break;

			default:
				Message_Struct msg_Invalid_Type = new Message_Struct(
					Message_Struct.Enum_Message_Types.ERROR, str_Sender, "The file type provided is not valid");
				throw new File_ReaderException(msg_Invalid_Type, Enum_Reader_Exceptions.INVALID_FILE_TYPE);
		}

	}
	// end Open_File()

	// Close StreamReader
	public void Close_File()
	{
		// Check that sr_StreamReader is actually open
		if(sr_StreamReader != null)
		{   
			// Close the file
			sr_StreamReader.Close();     
			sr_StreamReader = null;       
		}        
	}
	// end Close_File()

	// Get next line from file
	public string Read_Next_Line()
	{
		if(!At_End_Of_File())
		{
			return sr_StreamReader.ReadLine();
		}

		return "EOF";
	}
	// end Read_Next_Line()

	public bool At_End_Of_File()
	{
		return sr_StreamReader.EndOfStream;
	}
	// end At_End_Of_File()
	
	
	// Exception class for any errors with file reading
	public class File_ReaderException : System.Exception
	{		
		// Send the exception type when an error is thrown
		public File_ReaderException(string message, Enum_Reader_Exceptions type) : base(message) { enm_Type = type; }
		// Send the exception using the Message_Struct class and exception type
		public File_ReaderException(Message_Struct message, Enum_Reader_Exceptions type) { msg_Message = message; enm_Type = type; }		
		
		public Enum_Reader_Exceptions enm_Type = Enum_Reader_Exceptions.INVALID_EXCEPTION;
		public Message_Struct msg_Message;
	}
}
