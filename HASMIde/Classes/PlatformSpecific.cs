using System;
using System.Linq;

namespace HASM.Classes
{
	public static class PlatformSpecific
	{
		public static bool UsingMonoVM { get; private set; }
		public static bool IsUNIX { get; private set; }

		public const char UnixPathSeparator = '/';
		public const char WinPathSeparator = '\\';

		public static char NameSeparator;

		public static bool CompareAsPath(this string str, string another)
        {
			return str.NormalizePath() == another.NormalizePath();
        }

		public static string NormalizePath(this string str)
		{
			if (IsUNIX) return UnixPathSeparator + str.Replace(WinPathSeparator, UnixPathSeparator).TrimStart(UnixPathSeparator);
			else return str.ToLower().Replace(UnixPathSeparator, WinPathSeparator);
		}

		public static string CombinePath(params string[] parts)
		{
			return (IsUNIX ? UnixPathSeparator.ToString() : "") + 
				string.Join(NameSeparator.ToString(), parts.Select(p => p.NormalizePath().Trim(NameSeparator)));
		}

		public static void GetPlatform()
		{
			Type t = Type.GetType("Mono.Runtime");
			UsingMonoVM = t != null;

			IsUNIX = Environment.OSVersion.Platform == PlatformID.Unix;

			if (IsUNIX)
				NameSeparator = UnixPathSeparator;
			else
				NameSeparator = WinPathSeparator;
		}
	}
}
