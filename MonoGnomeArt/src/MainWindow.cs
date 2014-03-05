using System;
using System.Collections;
using System.Threading;
using Gtk;
using Glade;
using GLib;

namespace MonoGnomeArt
{
	public class MainWindow
	{	
		 private Glade.XML gui;
	
		private Hashtable guiTypes; // associate TreeIter to Enum ArtType
		
		[Widget] protected Window mainWindow;
		[Widget] protected Menu downloadsMenu;
		[Widget] protected TreeView previewTreeview;
		protected TreeStore _previewStore;
		[Widget] protected TreeView themeTypeTreeview;
		protected TreeStore _themeTypeStore;
		[Widget] protected Statusbar mainStatusbar;
		protected ProgressBar loadArtProgressBar;
		protected Label statusLabel;
		
		private Theme _currentTheme;
		private ArtType _currentArtId;
		private uint _currentIdleFetch;
		private IdleHandler _idleHandlerFetchGuiArt;
		
		private string _stFilename;
		
		public MainWindow ()
		{
			// Glade settings
			gui = new Glade.XML ("monognomeart.glade", "mainWindow", "");
            gui.Autoconnect (this);
			
			guiTypes = new Hashtable ();
			_idleHandlerFetchGuiArt = new IdleHandler (OnIdleFetchGuiArt);
			_currentTheme = new Theme ();
			
			// Add elements into statusbar
			statusLabel = new Label ("MonoGnomeArt");
			statusLabel.Xalign = 0F;
			loadArtProgressBar = new Gtk.ProgressBar ();
			mainStatusbar.Add (statusLabel);
			Gtk.Box.BoxChild bc0 = ((Gtk.Box.BoxChild)(mainStatusbar[statusLabel]));
            bc0.Position = 0;
			mainStatusbar.Add (loadArtProgressBar);
			Gtk.Box.BoxChild bc3 = ((Gtk.Box.BoxChild)(mainStatusbar[loadArtProgressBar]));
            bc3.Position = 3;
			
			// ThemeType Treeview definition
			themeTypeTreeview.Selection.Changed += new EventHandler (ThemeTypeSelectionChanged);
			
			// Store Model
			_themeTypeStore = new TreeStore (typeof (string));
			themeTypeTreeview.Model = _themeTypeStore;
			
			// Add all Type and associate to ArtType
			TreeIter allTypeIter = AddThemeTypeElement ("<b>All</b>");
			guiTypes.Add (allTypeIter, ArtType.All);
			guiTypes.Add (AddThemeTypeElement ("Background All"), ArtType.BackgroundAll);
			guiTypes.Add (AddThemeTypeElement ("Background Gnome"), ArtType.BackgroundGnome);
			guiTypes.Add (AddThemeTypeElement ("Background Other"), ArtType.BackgroundOther);
			guiTypes.Add (AddThemeTypeElement ("Icons"), ArtType.ThemesIcons); 
			guiTypes.Add (AddThemeTypeElement ("Thème Applications"), ArtType.ThemesApplications);
			guiTypes.Add (AddThemeTypeElement ("Window Border"), ArtType.ThemesWindowsBorders);
			guiTypes.Add (AddThemeTypeElement ("Login Manager"), ArtType.ThemesGDM);
			guiTypes.Add (AddThemeTypeElement ("Splash Screen"), ArtType.ThemesSplashs);
			guiTypes.Add (AddThemeTypeElement ("Gtk+ Engine"), ArtType.ThemesGTKEngines);
			
			
			// Column instantiation
			TreeViewColumn themeTypeColumn = new TreeViewColumn ();

			// ThemeType Column definition
			CellRendererText typecr = new CellRendererText ();
			
			themeTypeColumn.Sizing = TreeViewColumnSizing.Fixed;
			themeTypeColumn.FixedWidth = 20;
			themeTypeColumn.Resizable = false;
			themeTypeColumn.PackStart (typecr,  true);
			themeTypeColumn.AddAttribute (typecr,  "markup", 0);
			themeTypeTreeview.AppendColumn (themeTypeColumn);
			
			themeTypeTreeview.Selection.SelectIter (allTypeIter);
			
			// Preview TreeView definition
			// Store Model
			_previewStore = new TreeStore (typeof (Gdk.Pixbuf), typeof (string));

			
			previewTreeview.Model = _previewStore;
			 
			// Columns instantiation
			TreeViewColumn imageColumnPreview = new TreeViewColumn ();
			TreeViewColumn descColumnPreview = new TreeViewColumn ();
			
			// Image Column definition
			CellRendererPixbuf imagecr = new CellRendererPixbuf ();
			imageColumnPreview.Title = "Image";
			imageColumnPreview.MinWidth = 200;
			imageColumnPreview.Sizing = TreeViewColumnSizing.Autosize;
			imageColumnPreview.PackStart (imagecr, true);
			imageColumnPreview.AddAttribute (imagecr, "pixbuf", 0);
			previewTreeview.AppendColumn (imageColumnPreview);
			
			// Description Column definition
			CellRendererText desccr = new CellRendererText ();
			descColumnPreview.Title = "Description";
			descColumnPreview.MinWidth = 300;
			descColumnPreview.Sizing = TreeViewColumnSizing.Autosize;
			descColumnPreview.PackStart (desccr, true);
			descColumnPreview.AddAttribute (desccr, "markup", 1);
			previewTreeview.AppendColumn (descColumnPreview);
			
			gui["mainWindow"].ShowAll();
		}
		
		/// <summary>
		/// Add a Theme type Element
		/// </summary>
		/// <param name="type">Theme type name</param>
		/// <returns>TreeIter</returns>
		private TreeIter AddThemeTypeElement (String type)
		{
			TreeIter iter = _themeTypeStore.AppendValues (type);
			return iter;
		}
		
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}

		protected virtual void OnQuitMenuitemActivate (object sender, System.EventArgs e)
		{
			Application.Quit ();
		}

		protected virtual void OnRefreshMenuitemActivate (object sender, System.EventArgs e)
		{
			OnRefreshToolbuttonClicked (sender, e);
		}

		protected virtual void OnInstallMenuitemActivate (object sender, System.EventArgs e)
		{
			OnInstallToolbuttonClicked (sender, e);
		}

		protected virtual void OnSaveMenuitemActivate (object sender, System.EventArgs e)
		{
			OnSaveToolbuttonClicked (sender, e);
		}

		protected virtual void OnPreviewMenuitemActivate (object sender, System.EventArgs e)
		{
			OnPreviewToolbuttonClicked (sender, e);
		}
		
		protected virtual void OnNoDownloadsMenuitemActivate (object sender, System.EventArgs e)
		{
		}

		protected virtual void OnAboutMenuitemActivate (object sender, System.EventArgs e)
		{
			AboutDialog ();
		}

		protected virtual void OnSaveToolbuttonClicked (object sender, System.EventArgs e)
		{
			SaveTheme ();
		}

		protected virtual void OnInstallToolbuttonClicked (object sender, System.EventArgs e)
		{
			/*TreeIter iter;
			previewTreeview.Selection.GetSelected (out iter);
			_currentTheme.Install (iter);*/
		}

		protected virtual void OnPreviewToolbuttonClicked (object sender, System.EventArgs e)
		{            
			System.Threading.Thread thrDownload = new System.Threading.Thread (new ThreadStart (ThreadDownloadPreview));
    		thrDownload.Start ();
		}

		protected virtual void OnRefreshToolbuttonClicked (object sender, System.EventArgs e)
		{
			ReloadThemeList ();
		}
		
		
		/*
		 * Gui Interactions
		 */
		 
		 /// <summary>
		/// About dialog
		/// </summary>
		private void AboutDialog () 
		{
			AboutDialog _AboutDialog = new AboutDialog ();
			_AboutDialog.Name = "MonoGnomeArt";
			_AboutDialog.Authors = new string[]{"David Piry <klessou@gmail.com>", "Florian Coulmier <couloum@gmail.com>"};

			_AboutDialog.License = @"MonoGnomeArt is free software; you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation; either version 2 of the License, or
	(at your option) any later version.

	MonoGnomeArt is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	in a file called COPYING along with MonoGnomeArt; if not, write to
	the Free Software Foundation, Inc., 59 Temple Place, Suite 330,
	Boston, MA  02111-1307  USA";

			_AboutDialog.Copyright = "(c) 2006 Florian Coulmier <couloum@gmail.com> & David Piry <klessou@gmail.com";
			_AboutDialog.Website = "http://klessou.homelinux.org";
			_AboutDialog.WebsiteLabel = "MonoGnomeArt homepage";
			Version _Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			_AboutDialog.Version = string.Format("{0}.{1}", _Version.Major, _Version.Minor);
			_AboutDialog.Run();
		}
		
		/// <summary>
		/// Save a theme
		/// </summary>
		private void SaveTheme () 
		{
			TreeIter iter;
			
			if (previewTreeview.Selection.GetSelected (out iter)) // Launch FileChooserDialog if there is a selection
			{
				FileChooserDialog fileChooserSaveTheme;
				
				// New Save FileChooser
				fileChooserSaveTheme = new FileChooserDialog ("Save ArtItem", mainWindow, FileChooserAction.Save,  
											"Cancel",ResponseType.Cancel,
	                                    	  "Save",ResponseType.Accept);
				
				fileChooserSaveTheme.CurrentName = _currentTheme.GetFilename (iter);
				
				 if (fileChooserSaveTheme.Run() == (int)ResponseType.Accept) 
	          	{
	          		_stFilename = fileChooserSaveTheme.Filename;
	          		System.Threading.Thread thrDownload = new System.Threading.Thread (new ThreadStart (ThreadSaveTheme));
    				thrDownload.Start ();
	          	}
	          	fileChooserSaveTheme.Destroy ();
	          }
		}
		
		private void ThreadSaveTheme ()
		{
			TreeIter iter;
			
			if (previewTreeview.Selection.GetSelected (out iter))
			{
				MenuItem mi = new MenuItem (_stFilename);
            	downloadsMenu.Append (mi); // Add "a download" to the menu
            	downloadsMenu.ShowAll ();
            	
				_currentTheme.Download (iter, _stFilename);
				
				downloadsMenu.Remove (mi);
			}
		}
		
		/// <summary>
		/// Download and open the preview
		/// </summary>
		private void ThreadDownloadPreview () 
		{
			TreeIter iter;
			
			if (previewTreeview.Selection.GetSelected (out iter)) // Preview if there is a selection
			{
				MenuItem mi = new MenuItem (_currentTheme.GetFilename (iter));
            	downloadsMenu.Append (mi); // Add "a download" to the menu
            	downloadsMenu.ShowAll ();
			
				DateTime now = DateTime.Now;
				string timestamp = "" + now.Millisecond + now.Second + now.Minute + now.Hour +now.DayOfYear + now.Year;
				_currentTheme.Download (iter, Conf.Tempdir + timestamp);
				
				System.Diagnostics.Process.Start ("gnome-open " + Conf.Tempdir + timestamp);
				
				downloadsMenu.Remove (mi);
			}
			
		}
		
		/// <summary>
		/// Download the xml to reload the theme list
		/// </summary>				
		private void ReloadThemeList ()
		{
			TreeIter iter;
			
			// get selected iter
			if (themeTypeTreeview.Selection.GetSelected (out iter))
			{
				if (guiTypes.ContainsKey( iter ))
				{
					_currentTheme.GetArtItemList( (ArtType) guiTypes [(TreeIter) iter] );
					_currentArtId = (ArtType) guiTypes[(TreeIter) iter];
				}
				else 
					Console.Error.WriteLine ( "Error : Unknown Iter" );
			}
			
			// get selected iter
			if (themeTypeTreeview.Selection.GetSelected (out iter))
				FetchPreviews (iter); // Prints all previews
			else
				Console.Error.WriteLine ("Error : Selection not found");
		}

		/// ...
		public void ThemeTypeSelectionChanged (object o, EventArgs args)
		{
			TreeSelection selection = (TreeSelection) o;
			TreeIter iter;
			
			// reset loadProgressBar
			if (loadArtProgressBar.Adjustment != null)
			{
				loadArtProgressBar.Adjustment.Lower = 0;
				loadArtProgressBar.Adjustment.Upper = 0;
				loadArtProgressBar.Adjustment.Value = 0;
			}
			
			// get selected iter
			if (selection.GetSelected (out iter))
				FetchPreviews (iter);
			else
				Console.Error.WriteLine ("Error : Selection not found");
		}
		
		protected void FetchPreviews (TreeIter iter)
		{
			if (guiTypes.ContainsKey (iter))
			{
				// Removes the current "Fetch - Thread"
				if (_currentArtId > 0)
				{
					GLib.Source.Remove (_currentIdleFetch);
					Console.WriteLine ("Idle Removed " + _currentIdleFetch);
				}
				
				// Clears All TreeIter 
				try {
						_previewStore.Clear ();
				} catch (System.NullReferenceException e) {}
				
				// Creates and Run "Fetch - Thread"
				_currentIdleFetch = GLib.Idle.Add (_idleHandlerFetchGuiArt);
				_currentArtId = (ArtType) guiTypes [(TreeIter) iter];
			}
			else 
				Console.Error.WriteLine ("Error : Unknown Iter");
		}
		
		/// ...
		public bool OnIdleFetchGuiArt ()
		{
					int remainingTheme;
					
					if ((remainingTheme = _currentTheme.AddArt (_currentArtId, ref _previewStore)) == 0)
					{
					 	Console.WriteLine ("No more Item to fetch");
					 	previewTreeview.Model = _previewStore;
					 	
					 	statusLabel.LabelProp = "MonoGnomeArt";
					 	
						return false;
					}
					
					if (loadArtProgressBar.Adjustment.Upper == 0) {
						loadArtProgressBar.Adjustment.Upper = remainingTheme;
					}
					
					loadArtProgressBar.Adjustment.Value += 1;
					
					statusLabel.LabelProp = "Downloading Thumbnails : " 
						+ loadArtProgressBar.Adjustment.Value 
						+ "/" + loadArtProgressBar.Adjustment.Upper;
						
					previewTreeview.Model = null;
						
					return true;
		}
		
	}
}
