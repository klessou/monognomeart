// project created on 31/10/2006 at 21:01
using System;
using Gtk;

namespace MonoGnomeArt
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow ();
			// creates the tray icon
			Tray test = new Tray (win);
			Application.Run ();
		}
	}
}
