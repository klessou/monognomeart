using System;
using System.Net;
using System.Text.RegularExpressions;

namespace MonoGnomeArt
{
	
	public class ArtLoginManager : ArtItem
	{
		private string _smallThumbnailUrl;
		
		public ArtLoginManager 
		(
			string downloadStartTimestamp, 
			string downloadCount, 
			string name, 
			string description,
			string author,
			string url,
			string thumbnailUrl,
			string smallThumbnailUrl
		) : base (downloadStartTimestamp, downloadCount, name, description, author, url, thumbnailUrl)
		{
			_smallThumbnailUrl = smallThumbnailUrl;
		}
		
		public override string ThumbnailFile { 
			get {
				WebClient web = new WebClient();
				
				if (_downloadStartTimestamp == null) {
					Console.Error.WriteLine ("Mistake : No Download Start Timastamp");
					return null;
				}
				
				Regex reg = new Regex ("/");
				string name = reg.Replace (_name, "_");
				
				string imageURI = Conf.Homedir + "Background/" + _downloadStartTimestamp + name;
				
				Console.WriteLine ("Small Thumbnail into : " + imageURI);

				if (!System.IO.File.Exists(imageURI) && _smallThumbnailUrl != null)
					try {
						web.DownloadFile (_smallThumbnailUrl, imageURI);
					} 
					catch (System.Net.WebException e)
					{
						Console.WriteLine(e.Message);
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
