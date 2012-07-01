using System;
using Gtk;

class Checkov
{
	public static void Main (string[] args)
	{
		Application.Init ();
		CheckovUI win = new CheckovUI ();
		win.Show ();
		win.Start ();
		Application.Run ();
	}
}
