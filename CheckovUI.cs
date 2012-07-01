using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using Gtk;

public partial class CheckovUI: Gtk.Window
{	
	TextWriter log;
	Checklist checklist;
	
	public CheckovUI (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
	}
	
	public void Start ()
	{
		notebook1.CurrentPage = 0;
		checklist = new Checklist (this, tree);
		
		back.Sensitive = false;
		OnNotesFocusOutEvent(null, null);
		
		Pango.FontDescription big = new Pango.FontDescription ();
		big.Size = (int)(26 * Pango.Scale.PangoScale);
		
		currentItem.ModifyFont (big);
		LoadItems ();
	}
	
	public void UpdateDisplay (String text)
	{
		currentItem.Text = text;
	}
	
	public void SetLaunchButton (Entry item)
	{
		if (item.type == Entry.ChecklistTypes.PROGRAM) {
			launch.Label = "Launch " + item.data;
			launch.Visible = true;
		} else {
			launch.Visible = false;
		}
	}
	
	protected void OnDeleteEvent (object sender, object a)
	{
		Application.Quit ();
	}
	
	protected void LoadItems ()
	{
		try {
			StreamReader file = File.OpenText ("checklist.txt");
			List<String> entries = new List<String> ();
			
			entries.AddRange (file.ReadToEnd ().Trim ().Split ('\n'));
			entries.RemoveAll (i => i.StartsWith ("//"));
			entries.ForEach (e => checklist.Add (new Entry (e)));
			
		} catch (Exception) {	
			Error ("Missing checklist.txt\nEnsure it exists in " + Directory.GetCurrentDirectory());
		}
	}

	protected void OkPressed (object sender, System.EventArgs e)
	{
		back.Sensitive = true;
		if (!checklist.Next ())
			EndOfItems ();	
	}
	
	protected void Error (String message)
	{
		currentItem.Text = message;
		back.Hide ();
		ok.Hide ();
		notes.Hide ();
		launch.Hide ();
		
		quit.Show ();
	}

	protected void OnBackPressed (object sender, System.EventArgs e)
	{
		if(!checklist.Previous ())
			back.Sensitive = false;
	}
	
	void EndOfItems ()
	{
		back.Hide ();
		ok.Hide ();
		notes.Hide ();
		launch.Hide ();
		
		quit.Show ();
		
		checklist.Current().complete = true;
		
		currentItem.Text = "Checklist Complete";
		
		Directory.CreateDirectory ("checklist_logs");
		
		String fname = DateTime.Now.ToString ("yyyy-MM-dd HH-mm") + " checklist.txt";
		String path = System.IO.Path.Combine ("checklist_logs", fname);
		
		log = new StreamWriter (path);
		log.Write (checklist.ToString ());
		log.Close ();
	}
	
	void Launch (object o, System.EventArgs e)
	{
		Process program = new Process ();
		program.StartInfo.FileName = checklist.Current ().data;
		program.Start ();
	}
	
	public string GetNotes ()
	{
		if(notes.Text == "Notes...") return "";
		return notes.Text;
	}
	
	public void SetNotes (String note)
	{
		notes.Text = note;
		// Sorts out note hint if box was cleared
		OnNotesFocusOutEvent(null, null);
	}
	
	[GLib.ConnectBefore]
	protected void WindowKeyPressEvent (object o, Gtk.KeyPressEventArgs args)
	{
		// Move on enter, if user starts typing focus notes box
		if (args.Event.Key == Gdk.Key.Return) {
			ok.GrabFocus ();
			OkPressed (new object (), new EventArgs ());
		} else {
			if(!notes.HasFocus)
				notes.GrabFocus();
		}
	}
	
	// Handle notes hint
	protected void OnNotesFocusInEvent (object o, Gtk.FocusInEventArgs args)
	{
		notes.ModifyText(StateType.Normal);
		if (notes.Text == "Notes...") {
			notes.Text = "";
		}
	}

	protected void OnNotesFocusOutEvent (object o, Gtk.FocusOutEventArgs args)
	{
		if (notes.Text == "") {
			notes.Text = "Notes...";
			Gdk.Color grey = new Gdk.Color();
			Gdk.Color.Parse ("grey", ref grey);
			notes.ModifyText(StateType.Normal, grey);
		} else { notes.ModifyText(StateType.Normal); } 
	}
}