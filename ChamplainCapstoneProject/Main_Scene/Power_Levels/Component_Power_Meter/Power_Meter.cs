using Godot;
using System;
using System.Data.Common;
using System.Dynamic;

public partial class Power_Meter : VBoxContainer
{
	
	private Label lbl;
	private ProgressBar pgb;
	private Timer tmr;
	
	private int int_Val_Size = 16; // the number of pixels that each value takes up
	private int int_Target_Val;
	private float flt_Anim_Duration = 0.5f;	// the total duration of the animation, per integer value
	private float flt_Step_Size = 0.25f; // How much the value of the progress bar changes 
		//each time Animate_Value_Change() is called


	public override void _Ready()
	{
		lbl = GetNode<Label>("Label_PanelContainer/MarginContainer/Label");
		pgb = GetNode<ProgressBar>("ProgressBar_PanelContainter/MarginContainer/ProgressBar");
		tmr = GetNode<Timer>("Animation_Timer");

		Subscribe_To_Events();
	}
	// end _Ready()

	private void Subscribe_To_Events()
	{
		tmr.Timeout += () => Animate_Value_Change(MathF.Sign(int_Target_Val - (float)pgb.Value));
	}
	// end Subscribe_To_Events()	

	public void Set_Lable_Text(string text)
	{
		lbl.Text = text;
	}
	// end Set_Label_Text()

	public void Set_Progress_Bar_Max(int max)
	{
		pgb.MaxValue = max;
		pgb.CustomMinimumSize = new Vector2(0, max * int_Val_Size);
	}
	// end Set_Progress_Bar_Max()

	public void Set_Progress_Bar_Min(int min)
	{
		ProgressBar minBar = GetNode<ProgressBar>("ProgressBar_PanelContainter/MarginContainer/ProgressBar_Min");
		minBar.CustomMinimumSize = new Vector2(0, min * int_Val_Size);
	}
	// end Set_Progress_Bar_Min()

	public void Set_Progress_Bar_Val(int val)
	{
		int_Target_Val = val;

		if (tmr.IsStopped())
		{
			Animate_Value_Change(MathF.Sign(int_Target_Val - (float)pgb.Value));
		}
	}
	// end Set_Progress_Bar_Val()

	private void Animate_Value_Change(float dir)
	{
		float flt_Close_Enough = .0001f;

		if(pgb.Value > int_Target_Val + flt_Close_Enough || pgb.Value < int_Target_Val - flt_Close_Enough)
		{	
			pgb.Value += flt_Step_Size * dir;		
			tmr.Start(flt_Anim_Duration * flt_Step_Size);			
		}
	}
	// end Animate_Value_Change()


}
