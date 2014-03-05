using System;
using System.IO;
using System.Text;
using System.Net;
using System.Xml;
using MonoGnomeArt;

namespace MonoGnomeArt
{
	/// <summary>
	/// Makes treatments of xml files
	/// </summary>		
	public class XmlTreat
	{
		private String _xmlContent;
		private string _artGnomeURL;
		private XmlTextReader _xmlRd;
		private int _totalElmt;
		private int _remaingElmt;
		private ArtType _curArtType;
		
		public string XmlContent
		{
			get { return _xmlContent; }
		}
							
		/// <summary>
		/// Constructor. Initialise somme member variables
		/// </summary>
		public XmlTreat()
		{
			_xmlContent = "";
			_artGnomeURL = "http://art.gnome.org/xml.php?art=";
			_xmlRd = null;
			_totalElmt = 0;
			_remaingElmt = 0;
		}
		
		/*
		public void GetURL(ArtType artTypeId, int ArtId)
		{
			
		}
		*/
		
		/// <summary>
		/// Initialise the class by opening the xml corresponding to the ArtType and counting elements in it.
		/// </summary>
		/// <param name="artTypeId">Select which xml to parse depending on the ArtType</param>
		/// <returns>Total number of elements in the xml</returns>
		public int InitGetArt (ArtType artTypeId)
		{
			// No initialisation if there is a ArtItem remaining
			if (_remaingElmt > 0 && _curArtType == artTypeId)
					return _totalElmt;
					
			/* Hack to handle the ArtType "All" => do nothing */
			if (artTypeId == ArtType.All)
				return 0;
				
			string cacheXmlName = "art_"+artTypeId+".xml";
			
			_curArtType = artTypeId;
			
			// Close any handle of the xml in case there's one 
			if (_xmlRd != null)
			{
				_xmlRd.Close();
			}
			
			//Check that the xml corresponding to the ArtType exists
			if (File.Exists(cacheXmlName))
			{
				Console.WriteLine("File exists");
				// Open the xml
				_xmlRd = new XmlTextReader(cacheXmlName);
				// Call a private method to count numbers of ArtItems in the xml
				this.ParseCacheXml(artTypeId);
			}
			else
			{
				// If the xml doesn't exists, ask the user to download it !
				Console.WriteLine ("You have to refresh first !");
				return 0;
			}
			
			Console.WriteLine ("Nombre d'éléments : " + _totalElmt);
			
			return _totalElmt;
		}
		
		public void EndGetArt ()
		{
			_xmlRd.Close ();
			_xmlRd = null;
		}
		
		/// <summary>
		/// Parse one element of the xml and return the ArtItem generated.
		/// </summary>
		/// <param name="Remaining">Number of ArtItem remaining in the xml. Decresed by 1 each time this method is called.</param>
		/// <returns>an ArtItem corresponding to the current ArtType.</returns>
		public ArtItem GetArtItem (out int remaining)
		{
			remaining = _remaingElmt;
			
			if (remaining == 0)
				return null;
			
			// Read the xml until the begining of the next ArtItem declaration
			/*Console.WriteLine("Depth 1 :" + _xmlRd.Depth );
			_xmlRd.Read();
			Console.WriteLine("Depth 1.5 :" + _xmlRd.Depth );
			while (_xmlRd.NodeType != XmlNodeType.Element && _xmlRd.Depth != 1)
			{
				Console.WriteLine("Depth :" + _xmlRd.Depth );
				if (!_xmlRd.Read())
				{
					Console.WriteLine("No more nodes to read");
					remaining = -1;
					return null;
				}
			}
			Console.WriteLine("Depth 2 :" + _xmlRd.Depth );*/
			
			/*
			 * At this point what we read in the xml concern the ArtItem we want to fill. 
			 * We just have to call a private method to do this
			 */
			ArtItem Item = this.FillArtItem ();
			
			// Decrease the number of remaining Elements in the xml.
			if (_xmlRd.EOF)
				remaining = 0;
			else
			{
				_remaingElmt--;
				remaining = _remaingElmt;
			}
			
			return Item;
		}
		
		/// <summary>
		/// Fill and return an ArtItem depending on the current ArtType. Read the xml to complete informations of the ArtItem. 
		/// </summary>
		/// <returns>a fully operationnal ArtItem</returns>
		private ArtItem FillArtItem ()
		{
			String downloadStartTimestamp = null,
				name = null,
				description = null,
				category = null,
				author = null,
				license = null,
				thumbnail = null,
				smallThumbnail = null,
				url = null,
				resolution = null;
			
			ArtItem item = null;
			
			_xmlRd.Read (); // move to the first node - Depth = 1
			
			//  fill variables with data from the xml
			while (
				!_xmlRd.EOF
				&& !(_xmlRd.Name == "theme" || _xmlRd.Name == "background") 
				|| _xmlRd.NodeType != XmlNodeType.EndElement
			)
			{	
				_xmlRd.Read ();
				while (_xmlRd.NodeType == XmlNodeType.Whitespace)
					_xmlRd.Read ();
				
				if ((_xmlRd.Name == "theme" || _xmlRd.Name == "background") && _xmlRd.NodeType == XmlNodeType.Element)
					downloadStartTimestamp = _xmlRd.GetAttribute ("download_start_timestamp");
				else if (_xmlRd.Name == "name")
					name =_xmlRd.ReadString ();
				else if (_xmlRd.Name == "description")
					description =_xmlRd.ReadString ();
				else if (_xmlRd.Name == "category")
					category =_xmlRd.ReadString ();
				else if (_xmlRd.Name == "author")
					author =_xmlRd.ReadString ();
				else if (_xmlRd.Name == "license")
					license =_xmlRd.ReadString ();
				else if (_xmlRd.Name == "thumbnail")
					thumbnail =_xmlRd.ReadString ();
				else if (_xmlRd.Name == "small_thumbnail")
					smallThumbnail = _xmlRd.ReadString ();
				else if (_xmlRd.Name == "url")
					url =_xmlRd.ReadString ();
				else if (_xmlRd.Name == "resolution")
					resolution = _xmlRd.ReadString ();
			}
			
			switch (_curArtType)
			{
				case ArtType.BackgroundAll :
					Console.WriteLine ("Create BackgroundAll");
					item = new ArtBackground (ArtType.BackgroundAll, resolution, downloadStartTimestamp, "", name, description, author, url, thumbnail);
					break;
				case ArtType.BackgroundAbstract :
					Console.WriteLine ("Create BackgroundAbstract");
					item = new ArtBackground (ArtType.BackgroundAbstract, resolution, downloadStartTimestamp, "", name, description, author, url, thumbnail);
					break;
				case ArtType.BackgroundGnome :
					Console.WriteLine ("Create BackgroundGnome");
					item = new ArtBackground (ArtType.BackgroundGnome, resolution, downloadStartTimestamp, "", name, description, author, url, thumbnail);
					break;
				case ArtType.BackgroundNature :
					Console.WriteLine ("Create BackgroundNature");
					item = new ArtBackground (ArtType.BackgroundNature, resolution, downloadStartTimestamp, "", name, description, author, url, thumbnail);
					break;
				case ArtType.BackgroundOther :
					Console.WriteLine ("Create BackgroundOther");
					item = new ArtBackground (ArtType.BackgroundOther, resolution, downloadStartTimestamp, "", name, description, author, url, thumbnail);
					break;
				case ArtType.ThemesGDM :
					Console.WriteLine ("Create ThemesGDM");
					item = new ArtLoginManager (downloadStartTimestamp, "", name, description, author, url, thumbnail, smallThumbnail);
					break;
				case ArtType.ThemesSplashs :
					Console.WriteLine ("Create ThemesSplash");
					item = new ArtSplashScreen (downloadStartTimestamp, "", name, description, author, url, thumbnail, smallThumbnail);
					break;
				case ArtType.ThemesGTKEngines :
					Console.WriteLine ("Create ThemesGTKEngine");
					item = new ArtGtkEngine (downloadStartTimestamp, "", name, description, author, url, thumbnail);
					break;
				case ArtType.ThemesIcons :
					Console.WriteLine ("Create ThemesIcon");
					item = new ArtIcon (downloadStartTimestamp, "", name, description, author, url, thumbnail, smallThumbnail);
					break;
				case ArtType.ThemesApplications :
					Console.WriteLine ("Create ThemeApplication");
					item = new ArtGtkGnome (downloadStartTimestamp, "", name, description, author, url, thumbnail, smallThumbnail);
					Console.WriteLine ("Thumbnail url :" + smallThumbnail);
					break;
				case ArtType.ThemesWindowsBorders :
					Console.WriteLine ("Create WindowsBorder");
					item = new ArtDesktop (downloadStartTimestamp, "", name, description, author, url, thumbnail, smallThumbnail);
					break;
				case ArtType.All :
					break;
				default :
					break;
			}
			
			return item;
		}
		
		
		/// <summary>
		/// Parse the XML and count elements in it. Save the total numbers of items in a member variable.
		/// </summary>
		/// <param name="artTypeId">Select which xml to parse depending on the ArtType</param>
		/// <returns>nothing</returns>
		private void ParseCacheXml(ArtType artTypeId)
		{
			int totalArtItem = 0;
			string cacheXmlName = "art_"+artTypeId+".xml";
			XmlTextReader tmpXmlRd = new XmlTextReader(cacheXmlName);
			
			// Read each node of the XML. Increase the total number of item when we met a new node with a depth of 1
			while (!tmpXmlRd.EOF)
			{
				tmpXmlRd.Read();
				while ((tmpXmlRd.NodeType != XmlNodeType.Element
						|| tmpXmlRd.Depth != 1)
						&& ! tmpXmlRd.EOF)
				{
					tmpXmlRd.Read();
				}
				totalArtItem ++;
			}
			
			_totalElmt = totalArtItem - 1; // Decreased by 1 because the last incrementation occures when the end of the file is reached
			_remaingElmt = _totalElmt;
		}
	}

}