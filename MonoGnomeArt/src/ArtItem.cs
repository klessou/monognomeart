using System;
using System.Net;
using System.Text.RegularExpressions;

namespace MonoGnomeArt
{

	public class ArtItem
	{
		protected string _author;
		protected string _downloadStartTimestamp;
		protected string _downloadCount;
		protected string _name;
		protected string _description;
		protected string _category;
		protected string _license;
		protected string _url;
		protected string _thumbnailUrl;
		protected string _thumbnailFile;
		
		public ArtItem (
			string downloadStartTimestamp, 
			string downloadCount, 
			string name, 
			string description,
			string author,
			string url,
			string thumbnailUrl
		)
		{
			_downloadStartTimestamp = downloadStartTimestamp;
			_downloadCount = downloadCount;
			_name = name;
			_description = description;
			_author = author;
			_url = url;
			_thumbnailUrl = thumbnailUrl;
		}
		
		public string DowloadStartTimestamp { get { return _downloadStartTimestamp; } }	
		public string DownloadCount { get { return _downloadCount; } }
		public string Name { get { return _name; } }
		public virtual string Description 
		{ 
			get { return _description; } 
		}
		public string Url { get { return _url; } }
		private int i;
		public string ThumbnailUrl { get { return _thumbnailUrl; } }
		
		public virtual string ThumbnailFile {
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
						Console.Error.WriteLine("Unable to download :" + _thumbnailUrl + e.Message);
						return null;
					}
					
				return imageURI;
			}
		}
		
		public virtual void Install() {}
		
		public void Download (string filename) 
		{
			WebClient web = new WebClient();
			
			try {
					web.DownloadFile (_url, filename);
			}
			catch (System.Net.WebException e)
			{
				Console.Error.WriteLine ("Unable to download :" + _url + e.Message);
			}
		}
	}	
}
