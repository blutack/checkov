using System;
using System.Collections.Generic;

public class Checklist
{
	List<Entry> items;
	int currentIndex = 0;
	Gtk.ListStore listStore;
	MainWindow parent;
	
	public Checklist (MainWindow parent, Gtk.TreeView tree)
	{
		items = new List<Entry> ();
		listStore = new Gtk.ListStore (typeof(string), typeof(bool), typeof(string));
		
		this.parent = parent;
		
		tree.AppendColumn ("Item", new Gtk.CellRendererText (), "text", 0);
		tree.AppendColumn ("Complete", new Gtk.CellRendererToggle (), "active", 1);
		tree.AppendColumn ("Notes", new Gtk.CellRendererText (), "text", 2); 
		
		tree.Model = listStore;
	}
	
	public void Add (Entry entry)
	{
		items.Add (entry);
		AddToView (entry);
		Update ();
	}
	
	protected void AddToView (Entry entry)
	{
		if (entry.type == Entry.ChecklistTypes.ITEM)
			listStore.AppendValues (entry.data, entry.complete, entry.notes);
		else
			listStore.AppendValues ("Launch program " + entry.data, entry.complete, entry.notes);
	}
	
	protected void Sync ()
	{
		listStore.Clear ();
		items.ForEach (i => AddToView (i));
	}
	
	public Entry Current ()
	{
		return items [currentIndex];
	}
	
	public bool Next ()
	{
		if (currentIndex + 1 < items.Count) {
			Current ().complete = true;
			Current ().notes = parent.GetNotes ();
			parent.SetNotes ("");
			currentIndex += 1;
			Update ();
			return true;
		} else {
			Current ().complete = true;
			Current ().notes = parent.GetNotes ();
			Update ();
			return false;
		}
	}
	
	public bool Previous ()
	{
		if (currentIndex - 1 >= 0) {
			Current ().complete = false;
			currentIndex -= 1;
			parent.SetNotes (Current().notes);
			Update ();
			return true;
		}
		return false;
	}
	
	protected void Update ()
	{
		parent.SetLaunchButton (Current ());
		if (Current ().type == Entry.ChecklistTypes.ITEM)
			parent.UpdateDisplay (Current ().data);
		if (Current ().type == Entry.ChecklistTypes.PROGRAM)
			parent.UpdateDisplay ("Start " + Current ().data);
		Sync ();
	}
	
	public override String ToString ()
	{
		String output = "";
		items.ForEach (i => output += i.type + " " + i.data + " | " + i.notes + "\n");
		return output;
	}
}

