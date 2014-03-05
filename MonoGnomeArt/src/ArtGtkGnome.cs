using System;
using System.Net;
using System.Text.RegularExpressions;

namespace MonoGnomeArt
{
	
	public class ArtGtkGnome : ArtItem
	{
		public string _smallThumbnailUrl;
		
		public ArtGtkGnome 
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
						Console.Error.WriteLine (e.Message);
						return null;
					}
					
				return imageURI;
			}
		}
		
		public override void Install()
		{
		}
	}
	
}
