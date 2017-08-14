namespace GDA.Common
{
	using System.IO;
	public class FileSystemUtil
	{
		public static bool IsValidFilePath (string a)
		{
			if (a == null || a.Length == 0)
				return false;
			try {
				FileInfo b = new FileInfo (a);
				return b.Exists;
			}
			catch {
				return false;
			}
		}
		public static bool IsFileName (string a)
		{
			bool b = a != null && a.Length > 0;
			b |= a.IndexOf (Path.DirectorySeparatorChar) != -1;
			b |= a.IndexOf (Path.AltDirectorySeparatorChar) != -1;
			return b;
		}
		public static bool IsFolder (string a)
		{
			if (a == null || a.Length == 0)
				return false;
			try {
				DirectoryInfo b = new DirectoryInfo (a);
				return b.Exists;
			}
			catch {
				return false;
			}
		}
		public static bool IsRelativePath (string a)
		{
			return !Path.IsPathRooted (a);
		}
		public static string CombinePathAndFileName (string a, string b)
		{
			a = Path.GetFullPath (a);
			return Path.Combine (a, b);
		}
		public static string DetermineFileLocation (string a, string[] b)
		{
			foreach (string folder in b) {
				if (folder != null) {
					string c = CombinePathAndFileName (folder, a);
					if (IsValidFilePath (c))
						return c;
				}
			}
			return null;
		}
	}
}
