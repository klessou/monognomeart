using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Xml;
using System.Collections;
using Gtk;

namespace MonoGnomeArt
{
	/// <summary>
	/// Interface between the GUI and mechanism 
	/// </summary>
	public class Theme
	{
		private Hashtable  _guiArts; // associate TreeIter to GnomeArt
		private XmlTreat _xml; 
		private string _ArtGnomeURL; // Url of xmls into art.gnome.org
		private Conf _conf; // Configurations (Homedir, Cache, ...)
		
		public Theme ()
		{
			_guiArts = new Hashtable ();
			_xml = new XmlTreat ();
			_ArtGnomeURL = "http://art.gnome.org/xml.php?art=";
			_conf = new Conf ();
		}
		
		/// <summary>
		/// Download the xml of the corresponding ArtType on the site art.gnome.org
		/// </summary>
		/// <param name="artTypeId">ArtType xml list to download</param>
		/// <returns>nothing</returns>
		public void GetArtItemList (ArtType artTypeId)
		{
			WebClient web = new WebClient();
			string cacheXmlName = "art_"+artTypeId+".xml";
			string tempXmlName = "art_"+artTypeId+"_Temp.xml";
			
			// 	Remove the cache file if there already exists
			if (File.Exists (tempXmlName))
			{
				try
				{
					File.Delete (tempXmlName);
					Console.WriteLine ("Suppression du fihcier Temporaire déjà existant");
				}
				catch (System.Security.SecurityException e)
				{
					Console.Error.WriteLine ("You have not enougth permissions to delete the file");
					return;
				}
			}
			
			// Record the distant file in the local repertory 
			try
			{
				Console.WriteLine ("Download XML into art.gnome.org : " + _ArtGnomeURL + (int)artTypeId);
				web.DownloadFile (_ArtGnomeURL + (int)artTypeId, tempXmlName);
				Console.WriteLine ("Le Xml a été enregistré dans le fichier " + tempXmlName);
			}
			catch (System.IO.IOException e)
			{
				Console.Error.WriteLine( "An error occurred. Please check write permissions." );
			}
			
			// Compare the file just downloaded with the last downloaded file
			if (File.Exists (cacheXmlName))
			{
				this.UpdateCache( artTypeId );
			}
			else
			{
				// Save the temporary file as new cache file
				File.Move (tempXmlName, cacheXmlName);
				Console.WriteLine ("Xml was renamed to " + cacheXmlName);
			}
		}
		
		/// <summary>
		/// Compare the new downloaded xml with the old one in cache
		/// </summary>
		/// <param name="artTypeId">ArtType</param>
		/// <returns>nothing</returns>
		private void UpdateCache (ArtType artTypeId)
		{
			Console.WriteLine ("Start of the comparison");
			string cacheXmlName = "art_"+artTypeId+".xml";
			string tempXmlName = "art_"+artTypeId+"_Temp.xml";
			// Compare the two files
			FileStream fs1, fs2;
			fs1 = File.OpenRead (tempXmlName);
			fs2 = File.OpenRead (cacheXmlName);
			
			XmlTextReader xml1 = new XmlTextReader (fs1);
			XmlTextReader xml2 = new XmlTextReader (fs2);
			bool xmlAreDifferent = false;

			while (!xml1.EOF && !xml2.EOF)
			{
				xml1.Read ();
				while (xml1.NodeType != XmlNodeType.Element)
				{
					if (!xml1.Read ())
						break;
				}
				xml2.Read ();
				while (xml2.NodeType != XmlNodeType.Element)
				{
					if (!xml2.Read ())
						break;
				}
				if (xml1.Name != xml2.Name)
				{
					Console.WriteLine ("Nom de noeud différents : " + xml1.Name + " / " + xml2.Name);
					xmlAreDifferent = true;
				}
				else if (xml1.NodeType == XmlNodeType.Element
					&& xml1.Depth == 1
					&& xml1.GetAttribute ("download_start_timestamp") != xml2.GetAttribute ("download_start_timestamp"))
				{
					Console.WriteLine(xml1.GetAttribute ("download_start_timestamp"));
					xmlAreDifferent = true;
				}
			}
			
			xml1.Close ();
			xml2.Close ();
			fs1.Close ();
			fs2.Close ();
			
			if (xmlAreDifferent)
			{
				File.Delete( cacheXmlName );
				File.Move( tempXmlName, cacheXmlName );
				Console.WriteLine( "The old Xml was replaced by downloaded Xml" );
			}
			else
			{
				Console.WriteLine( "Le Xml téléchargé est identique à celui en cache" );
				File.Delete( tempXmlName );
			}
		}
		
		/// <summary>
		/// To add ArtItem into GUI
		/// </summary>
		/// <param name="type">ArtType to update</param>
		/// <param name="store">Store</param>
		/// <param name="iter">Iter to be defined</param>
		/// <returns>numbers remaining item</returns>
		public int AddArt (ArtType type, ref TreeStore store)
		{
			TreeIter iter; // = new TreeIter();
			Gdk.Pixbuf imagePreview; // Thumbnail
			int remainingArt = 0; // Number of remaining item
			ArtItem item = null;
			String description = "Description unvailable"; // Default description
			
			if ( _xml.InitGetArt(type) != 0 ) {
				item = _xml.GetArtItem( out remainingArt );
				Console.WriteLine( "Remaining Art to fetch : "+remainingArt );
			}				
			else {
				return 0;
			}	
			
			if (item != null) {
					try {
						imagePreview = new Gdk.Pixbuf (item.ThumbnailFile);
					}
					catch (GLib.GException e) {
						Console.WriteLine ("Error with " + item.Url + " : " + e.Message);
						imagePreview = new Gdk.Pixbuf(Conf.Homedir + "no-thumbnail.png" );
					}
					/*catch (System.NullReferenceException ex) {
						Console.WriteLine ("Error with " + item.Url + " : " + ex.Message);
						imagePreview = new Gdk.Pixbuf(Conf.Homedir + "no-thumbnail.png" );
					}*/
				
				try {
					Console.WriteLine( "Description :"+item.Description+" "+type );
					description = item.Description;
				}
				catch (System.NullReferenceException ex) {}
			}
			else {
				Console.Error.WriteLine ("No ArtItem available");
				imagePreview = new Gdk.Pixbuf (Conf.Homedir + "no-thumbnail.png");
			}

			iter = store.AppendValues( imagePreview, description );
			
			_guiArts.Add( iter, item ); // Insert the corresponding ArtItem into an Hashtable

			return remainingArt;
		}
		
		/// <summary>
		/// To Install a ArtItem
		/// </summary>
		public void Install (TreeIter iter)
		{
			Console.WriteLine( "Install" + iter.ToString() );
			Download (iter, "");
			((ArtItem)_guiArts[iter]).Install();
		}
		
		/// <summary>
		/// To Download a ArtItem
		/// </summary
		public void Download (TreeIter iter, string filename)
		{
			Console.WriteLine ("Download : " + filename );
			
			((ArtItem)_guiArts[iter]).Download(filename);
		}
		
		public void Refresh ()
		{
			Console.WriteLine ("Refresh");
		}
		
		public string GetFilename (TreeIter iter)
		{
			string url = ((ArtItem)_guiArts[iter]).Url;
			Regex reg = new Regex (".*/");
			string filename = reg.Replace (url, "");
			
			Console.WriteLine ("file : "+filename);
			
			return filename;
		}
	}
	
}
