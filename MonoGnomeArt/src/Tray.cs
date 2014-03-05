using Gtk;
using System;
using System.Runtime.InteropServices;

namespace MonoGnomeArt
{
	
	public class Tray
	{
		private MainWindow _mainwin;
		
		public Tray(MainWindow win)
		{
			_mainwin = win;
			// we need a eventbox, because Gtk.Image doesn't receive signals
			EventBox eb = new EventBox ();
			eb.Add (new Image (Stock.Network, IconSize.Menu)); // using stock icon
			eb.ButtonPressEvent += new ButtonPressEventHandler (this.OnImageClick);
			TrayIcon icon = new TrayIcon ("MonoGnomeArt");
			icon.Add (eb);
			// showing the trayicon
			icon.ShowAll ();
		}
		
		// handler for mouse click
		private void OnImageClick (object o, ButtonPressEventArgs args) 
		{
	   		if (args.Event.Button == 3) //right click
	   		{
				Menu popupMenu = new Menu (); // creates the menu  
	      		ImageMenuItem menuPopup1 = new ImageMenuItem ("Quit");
	      		Image appimg = new Image (Stock.Quit, IconSize.Menu);
	      		menuPopup1.Image = appimg; // sets the menu item's image
	      		popupMenu.Add (menuPopup1); // adds the menu item to the menu
	                
	      		menuPopup1.Activated += new EventHandler (this.OnPopupClick); // event when the user clicks the icon
				popupMenu.ShowAll (); // shows everything
	            // pops up the actual menu when the user right clicks
	      		popupMenu.Popup (null, null, null, IntPtr.Zero, args.Event.Button, args.Event.Time);
	   		}
	   		else 
	   		{
	   			Menu mainPopupMenu = new Menu ();
	   			
	   			ImageMenuItem menuPopupReload = new ImageMenuItem ("Reload");
	   			ImageMenuItem menuPopupAbout = new ImageMenuItem ("About");
	   			
	   			Image reloadimg = new Image (Stock.Refresh, IconSize.Menu);
	   			Image aboutimg = new Image (Stock.About, IconSize.Menu);
	   			
	   			menuPopupReload.Image = reloadimg;
	   			menuPopupAbout.Image = aboutimg;
	   			
	   			mainPopupMenu.Add (menuPopupReload);
	   			mainPopupMenu.Add (menuPopupAbout);
	   			
	   			menuPopupReload.Activated += new EventHandler (this.OnPopupReloadClick);
	   			menuPopupAbout.Activated += new EventHandler (this.OnPopupAboutClick);
	   			mainPopupMenu.ShowAll ();
	   			mainPopupMenu.Popup (null, null, null, IntPtr.Zero, args.Event.Button, args.Event.Time);
	   		}
		}
		
		private void OnPopupClick(object o, EventArgs args)
		{
			Application.Quit (); // quits the application when the users clicks the popup menu
		}
		
		private void OnPopupReloadClick(object o, EventArgs args)
		{
			//_mainwin.FetchGuiArt (ArtType.All);
		}
		
		private void OnPopupAboutClick(object o, EventArgs args)
		{
			//_mainwin.
		}
	}
	
}
