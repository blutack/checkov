using System;
using Gtk;

namespace checklist5
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();
			win.Start ();
			Application.Run ();
		}
	}
}
