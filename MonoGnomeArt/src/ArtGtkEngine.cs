using System;
using System.Net;
using System.Text.RegularExpressions;

namespace MonoGnomeArt
{
	
	public class ArtGtkEngine : ArtItem
	{
		
		public ArtGtkEngine 
		(
			string downloadStartTimestamp,
			string downloadCount,
			string name,
			string description,
			string author,
			string url,
			string thumbnailUrl
		) : base (downloadStartTimestamp, downloadCount, name, description, author, url, thumbnailUrl)
		{
		}
		
		public override string ThumbnailFile {
			get {
				WebClient web = new WebClient();
				
				if (_downloadStartTimestamp == null) {
					Console.Error.WriteLine ("Mistake : No Download Start Timestamp");
					return null;
				}
				
				Regex reg = new Regex ("/");
				string name = reg.Replace (_name, "_");
				
				string imageURI = Conf.Homedir + "Background/" + _downloadStartTimestamp + name;
				
				Console.WriteLine ("Thumbnail into : " + imageURI);

				if (!System.IO.File.Exists(imageURI) && _thumbnailUrl != null)
					try {
						web.DownloadFile (_thumbnailUrl, imageURI);
					}
					catch (System.Net.WebException e)
					{
						Console.Error.WriteLine ("Unable to download :" + _thumbnailUrl + e.Message);
						return null;
					}
					
				return imageURI;
			}
		}
		
		public override string Description 
		{ 
			get {
				int descLength = 80;
				
				if (_description.Length < 80)
					descLength = _description.Length;
				
				string desc = "<b>"+_name+"'</b>\n"+_description.Substring (0, descLength)+"...\n<i>Author :"+_author+"</i>";
				return desc; 
			}
		}
		
		public override void Install()
		{
		}
	}
	
}
