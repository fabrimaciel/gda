using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
namespace GDA.Helper
{
	internal class GDAHelper
	{
		public static bool SortExpression (string a)
		{
			return a.ToLower ().EndsWith (" desc");
		}
		public static bool MatchString (string a, string b)
		{
			a = a.Trim ();
			System.Text.RegularExpressions.Regex c = new System.Text.RegularExpressions.Regex (b);
			return c.IsMatch (a);
		}
		public static bool IsValidUserName (string a)
		{
			string b = @"^[\w-'\.]{2,128}$";
			bool c = true;
			if (c) {
				return (MatchString (a, b) || IsValidEmailAddress (a));
			}
			else {
				return MatchString (a, b);
			}
		}
		public static bool IsValidPassword (string a)
		{
			bool b = true;
			int c = 6;
			int d = 8;
			if (b) {
				string e = @"^.*(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[`~!@#\$%\^\&\*\(\)-_\=\+\[\{\]\}\\\|;:',<\.>/?]).*$";
				return (a.Length >= d && MatchString (a, e));
			}
			else {
				return (a.Length >= c);
			}
		}
		public static bool IsValidName (string a)
		{
			string b = @"^[a-zA-Z-'\.\s]{2,128}$";
			return MatchString (a, b);
		}
		public static bool IsValidStreetAddress (string a)
		{
			string b = @"\d{1,3}.?\d{0,3}\s[a-zA-Z]{2,30}(\s[a-zA-Z]{2,15})?([#\.0-9a-zA-Z]*)?";
			return MatchString (a, b);
		}
		public static bool IsValidCity (string a)
		{
			return IsValidName (a);
		}
		public static bool IsValidUSState (string a)
		{
			string[] b =  {
				"ALABAMA",
				"ALASKA",
				"ARIZONA",
				"ARKANSAS",
				"CALIFORNIA",
				"COLORADO",
				"CONNECTICUT",
				"DELAWARE",
				"FLORIDA",
				"GEORGIA",
				"HAWAII",
				"IDAHO",
				"ILLINOIS",
				"INDIANA",
				"IOWA",
				"KANSAS",
				"KENTUCKY",
				"LOUISIANA",
				"MAINE",
				"MARYLAND",
				"MASSACHUSETTS",
				"MICHIGAN",
				"MINNESOTA",
				"MISSISSIPPI",
				"MISSOURI",
				"MONTANA",
				"NEBRASKA",
				"NEVADA",
				"NEW HAMPSHIRE",
				"NEW JERSEY",
				"NEW MEXICO",
				"NEW YORK",
				"NORTH CAROLINA",
				"NORTH DAKOTA",
				"OHIO",
				"OKLAHOMA",
				"OREGON",
				"PENNSYLVANIA",
				"RHODE ISLAND",
				"SOUTH CAROLINA",
				"SOUTHDAKOTA",
				"TENNESSEE",
				"TEXAS",
				"UTAH",
				"VERMONT",
				"VIRGINIA",
				"WASHINGTON",
				"WEST VIRGINIA",
				"WISCONSIN",
				"WYOMING"
			};
			string[] c =  {
				"AL",
				"AK",
				"AZ",
				"AR",
				"CA",
				"CO",
				"CT",
				"DE",
				"DC",
				"FL",
				"GA",
				"HI",
				"ID",
				"IL",
				"IN",
				"IA",
				"KS",
				"KY",
				"LA",
				"ME",
				"MD",
				"MA",
				"MI",
				"MN",
				"MS",
				"MO",
				"MT",
				"NE",
				"NV",
				"NH",
				"NJ",
				"NM",
				"NY",
				"NC",
				"ND",
				"OH",
				"OK",
				"OR",
				"PA",
				"RI",
				"SC",
				"SD",
				"TN",
				"TX",
				"UT",
				"VT",
				"VA",
				"WA",
				"WV",
				"WI",
				"WY"
			};
			a = a.ToUpper ();
			ArrayList d = new ArrayList (c);
			ArrayList e = new ArrayList (b);
			return (d.Contains (a) || e.Contains (a));
		}
		public static bool IsValidZIPCode (string a)
		{
			string b = @"^(\d{5}-\d{4}|\d{5}|\d{9})$";
			return MatchString (a, b);
		}
		public static bool IsValidUSPhoneNumber (string a)
		{
			string b = @"^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$";
			return MatchString (a, b);
		}
		public static bool IsValidCCNumber (string a)
		{
			string b = @"^((4\d{3})|(5[1-5]\d{2})|(6011))-?\d{4}-?\d{4}-?\d{4}|3[4,7][\d\s-]{15}$";
			return MatchString (a, b);
		}
		public static bool IsValidSSN (string a)
		{
			string b = @"^\d{3}[-]?\d{2}[-]?\d{4}$";
			return MatchString (a, b);
		}
		public static bool IsValidEmailAddress (string a)
		{
			string b = @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";
			return MatchString (a, b);
		}
		public static bool IsValidURL (string a)
		{
			string b = @"^^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_=]*)?$";
			return MatchString (a, b);
		}
		public static bool IsValidIPAddress (string a)
		{
			string b = @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
			return MatchString (a, b);
		}
		public static bool IsValidAlphaText (string a)
		{
			string b = @"^[A-Za-z]+$";
			return MatchString (a, b);
		}
		public static bool IsValidAlphaNumericText (string a)
		{
			string b = @"^[A-Za-z0-9]+$";
			return MatchString (a, b);
		}
		public static bool IsValidNumericText (string a)
		{
			string b = @"/[+-]?\d+(\.\d+)?$";
			return MatchString (a, b);
		}
		internal static IDbDataParameter ConvertGDAParameter (IDbCommand a, GDAParameter b, GDA.Interfaces.IProvider c)
		{
			if (c is Interfaces.IParameterConverter2)
				return ((Interfaces.IParameterConverter2)c).Converter (a, b);
			else if (c is Interfaces.IParameterConverter)
				return ((Interfaces.IParameterConverter)c).Convert (b);
			else {
				IDbDataParameter d = a.CreateParameter ();
				if (d.Direction != b.Direction)
					d.Direction = b.Direction;
				d.Size = b.Size;
				try {
					if (b.ParameterName [0] == '?')
						d.ParameterName = c.ParameterPrefix + b.ParameterName.Substring (1) + c.ParameterSuffix;
					else
						d.ParameterName = b.ParameterName;
				}
				catch (Exception ex) {
					throw new GDAException ("Error on convert parameter name '" + b.ParameterName + "'.", ex);
				}
				if (b.DbTypeIsDefined)
					d.DbType = b.DbType;
				c.SetParameterValue (d, b.Value == null ? DBNull.Value : b.Value);
				return d;
			}
		}
		public static bool IsNullableType (Type a)
		{
			return (a.IsGenericType && a.GetGenericTypeDefinition ().Equals (typeof(Nullable<>)));
		}
		public static bool TryParse (string a, out int b)
		{
			#if PocketPC
						            try
            {
                result = int.Parse(value);
                return true;
            }
            catch 
            {
                result = 0;
                return false;
            }   
#else
			return int.TryParse (a, out b);
			#endif
		}
		public static bool TryParse (string a, out bool b)
		{
			#if PocketPC
						            try
            {
                result = bool.Parse(value);
                return true;
            }
            catch 
            {
                result = false;
                return false;
            }   
#else
			return bool.TryParse (a, out b);
			#endif
		}
		public static bool Exists<T> (T[] a, Predicate<T> b)
		{
			if (b == null)
				throw new ArgumentNullException ("match");
			#if PocketPC
						            foreach (var i in array)
                if (match(i))
                    return true;
            return false;
#else
			return Array.Exists (a, b);
			#endif
		}
	}
}
