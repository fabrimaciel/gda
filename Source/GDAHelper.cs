/* 
 * GDA - Generics Data Access, is framework to object-relational mapping 
 * (a programming technique for converting data between incompatible 
 * type systems in databases and Object-oriented programming languages) using c#.
 * 
 * Copyright (C) 2010  <http://www.colosoft.com.br/gda> - support@colosoft.com.br
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;

namespace GDA.Helper
{
	internal class GDAHelper
	{
		/// <summary>
		/// Verifica se a expressão de ordenação possui um item que represente reversão na ordenação.
		/// </summary>
		/// <param name="sortExpression">Expressão de ordenação.</param>
		/// <returns>True se existir reversão.</returns>
		public static bool SortExpression(string sortExpression)
		{
			return sortExpression.ToLower().EndsWith(" desc");
		}

		public static bool MatchString(string str, string regexstr)
		{
			str = str.Trim();
			System.Text.RegularExpressions.Regex pattern = new System.Text.RegularExpressions.Regex(regexstr);
			return pattern.IsMatch(str);
		}

		public static bool IsValidUserName(string strUsername)
		{
			string regExPattern = @"^[\w-'\.]{2,128}$";
			bool allowEmailUsernames = true;
			if(allowEmailUsernames)
			{
				return (MatchString(strUsername, regExPattern) || IsValidEmailAddress(strUsername));
			}
			else
			{
				return MatchString(strUsername, regExPattern);
			}
		}

		public static bool IsValidPassword(string strPassword)
		{
			bool passwordComplexity = true;
			int minPasswordLen = 6;
			int strongPasswordLen = 8;
			if(passwordComplexity)
			{
				string regExPattern = @"^.*(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[`~!@#\$%\^\&\*\(\)-_\=\+\[\{\]\}\\\|;:',<\.>/?]).*$";
				return (strPassword.Length >= strongPasswordLen && MatchString(strPassword, regExPattern));
			}
			else
			{
				return (strPassword.Length >= minPasswordLen);
			}
		}

		public static bool IsValidName(string strName)
		{
			string regExPattern = @"^[a-zA-Z-'\.\s]{2,128}$";
			return MatchString(strName, regExPattern);
		}

		public static bool IsValidStreetAddress(string strAddress)
		{
			string regExPattern = @"\d{1,3}.?\d{0,3}\s[a-zA-Z]{2,30}(\s[a-zA-Z]{2,15})?([#\.0-9a-zA-Z]*)?";
			return MatchString(strAddress, regExPattern);
		}

		public static bool IsValidCity(string strCity)
		{
			return IsValidName(strCity);
		}

		public static bool IsValidUSState(string strState)
		{
			string[] stateNames =  {
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
			string[] stateCodes =  {
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
			strState = strState.ToUpper();
			ArrayList stateCodesArray = new ArrayList(stateCodes);
			ArrayList stateNamesArray = new ArrayList(stateNames);
			return (stateCodesArray.Contains(strState) || stateNamesArray.Contains(strState));
		}

		public static bool IsValidZIPCode(string strZIP)
		{
			string regExPattern = @"^(\d{5}-\d{4}|\d{5}|\d{9})$";
			return MatchString(strZIP, regExPattern);
		}

		public static bool IsValidUSPhoneNumber(string strPhone)
		{
			string regExPattern = @"^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$";
			return MatchString(strPhone, regExPattern);
		}

		public static bool IsValidCCNumber(string strCCNumber)
		{
			string regExPattern = @"^((4\d{3})|(5[1-5]\d{2})|(6011))-?\d{4}-?\d{4}-?\d{4}|3[4,7][\d\s-]{15}$";
			return MatchString(strCCNumber, regExPattern);
		}

		public static bool IsValidSSN(string strSSN)
		{
			string regExPattern = @"^\d{3}[-]?\d{2}[-]?\d{4}$";
			return MatchString(strSSN, regExPattern);
		}

		public static bool IsValidEmailAddress(string strEmail)
		{
			string regExPattern = @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";
			return MatchString(strEmail, regExPattern);
		}

		public static bool IsValidURL(string strURL)
		{
			string regExPattern = @"^^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_=]*)?$";
			return MatchString(strURL, regExPattern);
		}

		public static bool IsValidIPAddress(string strIP)
		{
			string regExPattern = @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
			return MatchString(strIP, regExPattern);
		}

		public static bool IsValidAlphaText(string strAlpha)
		{
			string regExPattern = @"^[A-Za-z]+$";
			return MatchString(strAlpha, regExPattern);
		}

		public static bool IsValidAlphaNumericText(string strAlphaNum)
		{
			string regExPattern = @"^[A-Za-z0-9]+$";
			return MatchString(strAlphaNum, regExPattern);
		}

		public static bool IsValidNumericText(string strNumeric)
		{
			string regExPattern = @"/[+-]?\d+(\.\d+)?$";
			return MatchString(strNumeric, regExPattern);
		}

		/// <summary>
		/// Converte um parametro do GDA para um parametro do ADO.NET.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="parameter"></param>
		/// <param name="provider">Provider que será usado na conversão.</param>
		/// <returns></returns>
		internal static IDbDataParameter ConvertGDAParameter(IDbCommand cmd, GDAParameter parameter, GDA.Interfaces.IProvider provider)
		{
			if(provider is Interfaces.IParameterConverter2)
				return ((Interfaces.IParameterConverter2)provider).Converter(cmd, parameter);
			else if(provider is Interfaces.IParameterConverter)
				return ((Interfaces.IParameterConverter)provider).Convert(parameter);
			else
			{
				IDbDataParameter p = cmd.CreateParameter();
				if(p.Direction != parameter.Direction)
					p.Direction = parameter.Direction;
				p.Size = parameter.Size;
				try
				{
					if(parameter.ParameterName[0] == '?')
						p.ParameterName = provider.ParameterPrefix + parameter.ParameterName.Substring(1) + provider.ParameterSuffix;
					else
						p.ParameterName = parameter.ParameterName;
				}
				catch(Exception ex)
				{
					throw new GDAException("Error on convert parameter name '" + parameter.ParameterName + "'.", ex);
				}
				if(parameter.DbTypeIsDefined)
					p.DbType = parameter.DbType;
				provider.SetParameterValue(p, parameter.Value == null ? DBNull.Value : parameter.Value);
				return p;
			}
		}

		/// <summary>
		/// Verifica se o tipo e um Nullable.
		/// </summary>
		/// <param name="theType"></param>
		/// <returns></returns>
		public static bool IsNullableType(Type theType)
		{
			return (theType.IsGenericType && theType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
		}

		/// <summary>
		/// Tenta recupera o valor da string.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public static bool TryParse(string value, out int result)
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
			return int.TryParse(value, out result);
			#endif
		}

		/// <summary>
		/// Tenta recupera o valor da string.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public static bool TryParse(string value, out bool result)
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
			return bool.TryParse(value, out result);
			#endif
		}

		public static bool Exists<T>(T[] array, Predicate<T> match)
		{
			if(match == null)
				throw new ArgumentNullException("match");
			#if PocketPC
			            foreach (var i in array)
                if (match(i))
                    return true;

            return false;
#else
			return Array.Exists(array, match);
			#endif
		}
	}
}
