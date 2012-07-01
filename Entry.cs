using System;

public class Entry
{
	public enum ChecklistTypes
	{
		ITEM,
		PROGRAM
	}
	
	public readonly String data;
	public readonly ChecklistTypes type;
	public bool complete = false;
	public String notes = "";
	
	public Entry (String line)
	{
		if (line.StartsWith ("!Launch ")) {
			type = ChecklistTypes.PROGRAM;
			data = line.Remove(0, "!Launch ".Length);
		} else {
			type = ChecklistTypes.ITEM;
			data = line;
		}
	}
}

