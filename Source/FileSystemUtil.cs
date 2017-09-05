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

namespace GDA.Common
{
	using System.IO;

	public class FileSystemUtil
	{
		/// <summary>
		/// Verifica se o arquivo existe na localização passada
		/// </summary>
		public static bool IsValidFilePath(string localFilePath)
		{
			if(localFilePath == null || localFilePath.Length == 0)
				return false;
			try
			{
				FileInfo fileInfo = new FileInfo(localFilePath);
				return fileInfo.Exists;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Verifica se o argumento passado representa o nome de um arquivo, entendendo que 
		/// ele possua informação com por exemplo de diretorio.
		/// </summary>
		public static bool IsFileName(string fileName)
		{
			bool hasPath = fileName != null && fileName.Length > 0;
			hasPath |= fileName.IndexOf(Path.DirectorySeparatorChar) != -1;
			hasPath |= fileName.IndexOf(Path.AltDirectorySeparatorChar) != -1;
			return hasPath;
		}

		/// <summary>
		/// Verifica se o argumento passado representa um diretorio contendo informações sobre arquivos.
		/// </summary>
		public static bool IsFolder(string path)
		{
			if(path == null || path.Length == 0)
				return false;
			try
			{
				DirectoryInfo dirInfo = new DirectoryInfo(path);
				return dirInfo.Exists;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Verifica se o caminho informado é relativo ou absoluto
		/// </summary>
		public static bool IsRelativePath(string path)
		{
			return !Path.IsPathRooted(path);
		}

		/// <summary>
		/// Combina o nome do diretorio e do arquivo em um caminho absoluto.
		/// </summary>
		public static string CombinePathAndFileName(string folder, string fileName)
		{
			folder = Path.GetFullPath(folder);
			return Path.Combine(folder, fileName);
		}

		/// <summary>
		/// Procura por um especificado arquivo dentro das localizações passadas e retorno o primeiro arquivo encontrado.
		/// </summary>
		public static string DetermineFileLocation(string fileName, string[] searchLocations)
		{
			foreach (string folder in searchLocations)
			{
				if(folder != null)
				{
					string filePath = CombinePathAndFileName(folder, fileName);
					if(IsValidFilePath(filePath))
						return filePath;
				}
			}
			return null;
		}
	}
}
