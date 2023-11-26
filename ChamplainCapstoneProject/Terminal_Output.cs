using Godot;
using System;

public partial class Terminal_Output : Control
{

	RichTextLabel txt_Terminal;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	// end _Ready()

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	// end _Process()

	public void print_message(string str_message)
	{
		txt_Terminal.Text += str_message + "\n";
	}
	// end print_message()

	private void _on_txt_terminal_ready()
	{
		txt_Terminal = GetChild(0).GetChild<RichTextLabel>(-1);
	}
	// end _on_txt_terminal_ready()
}



