using System;
using Mono.Unix;

namespace MonoGnomeArt
{
	/*
	 * List of ArtItem types
	 * the number corresponds to the xml id
	 */
	public enum ArtType {
		All = 0,
		BackgroundGnome = 10,
		BackgroundOther = 11,
		BackgroundAll = 12,
		BackgroundNature = 13,
		BackgroundAbstract = 14,
						
		ThemesApplications = 20,
		ThemesWindowsBorders = 21,
		ThemesIcons = 22,
						
		ThemesGDM = 30,
		ThemesSplashs = 31,
		ThemesGTKEngines = 32
	};


	/// <summary>
	/// Manage configuration
	/// </summary>
	public class Conf
	{
		private static string _homedir;
		
		public static string Homedir {
			get {
				return _homedir;
			}
		}
		
		public static string Tempdir {
			get {
				return _homedir + "tmp/";
			}
		}
		
		public Conf()
		{
			// initialises variables
			// _homedir = Environment.GetEnvironmentVariable ("HOME") + "/.MonoGnomeArt/";
			// Recovers path of the personal repertory of the user
			long CurrentUserID = UnixEnvironment.RealUserId;
			UnixUserInfo CurrentUser = new UnixUserInfo (CurrentUserID);
			_homedir = CurrentUser.HomeDirectory + "/.MonoGnomeArt/";
			
			Console.WriteLine(_homedir);
			// create home dirs if they not exists
			create_home_dirs ();
		}
		
		
		// <summary>
		/// Create setings directory
		/// </summary>
		private void create_home_dirs ()
		{
			try {
				if (!System.IO.Directory.Exists (_homedir)) {
					System.IO.Directory.CreateDirectory (_homedir);
				}
				if (!System.IO.Directory.Exists (_homedir + "Background")) {
					System.IO.Directory.CreateDirectory (_homedir + "Background");
				}
				if (!System.IO.Directory.Exists (_homedir  + "Desktop")) {
					System.IO.Directory.CreateDirectory (_homedir + "Desktop" );
				}
				if (!System.IO.Directory.Exists (_homedir + "GtkEngine" )) {
					System.IO.Directory.CreateDirectory (_homedir + "GtkEngine");
				}
				if (!System.IO.Directory.Exists (_homedir + "Icons")) {
					System.IO.Directory.CreateDirectory (_homedir + "Icons");
				}
				if (!System.IO.Directory.Exists (_homedir + "LoginManager")) {
					System.IO.Directory.CreateDirectory (_homedir + "LoginManager");
				}
				if (!System.IO.Directory.Exists (_homedir + "SplashScreen")) {
					System.IO.Directory.CreateDirectory (_homedir + "SplashScreen");
				}
				if (!System.IO.Directory.Exists (_homedir + "tmp")) {
					System.IO.Directory.CreateDirectory (_homedir + "tmp");
				}
			} catch (System.Exception ex) {
				Console.Error.WriteLine ("The process failed: {0}", ex.ToString ());
			}
		}
	}
	
}
