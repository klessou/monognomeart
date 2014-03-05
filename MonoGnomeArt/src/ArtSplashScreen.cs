using System;
using System.Net;
using System.Text.RegularExpressions;

namespace MonoGnomeArt
{
	
	public class ArtSplashScreen : ArtItem
	{
		private string _smallThumbnailUrl;
		
		public ArtSplashScreen 
		(
			string downloadStartTimestamp, 
			string downloadCount, 
			string name, 
			string description,
			string author,
			string url,
			string thumbnailUrl,
			string smallThumbnailUrl
		) : base
		(downloadStartTimestamp, downloadCount, name, description, author, url, thumbnailUrl)
		{
			_smallThumbnailUrl = smallThumbnailUrl;
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

				if (!System.IO.File.Exists(imageURI) && _smallThumbnailUrl != null)
					try {
						web.DownloadFile (_smallThumbnailUrl, imageURI);
					}
					catch (System.Net.WebException e)
					{
						Console.Error.WriteLine("Unable to download :" + _thumbnailUrl + e.Message);
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
			Console.WriteLine("Install current SplashScreen"); 
		}
	}
	
}
