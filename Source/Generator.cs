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
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace GDA.Analysis
{
	/// <summary>
	/// Classe responsável pela geração do código da classe com base no esquema
	/// das tabelas do banco de dados.
	/// </summary>
	public class Generator
	{
		private string _namespaceName;

		private string _codeLanguage = "C#";

		private List<CodeAttributeDeclaration> _propertyCustomAttributes = new List<CodeAttributeDeclaration>();

		private List<CodeAttributeDeclaration> _classCustomAttributes = new List<CodeAttributeDeclaration>();

		private List<CodeNamespaceImport> _imports = new List<CodeNamespaceImport>();

		private bool _usingWCFPattern;

		/// <summary>
		/// Namespace usado na geracao do codigo.
		/// </summary>
		public string NamespaceName
		{
			get
			{
				return _namespaceName;
			}
			set
			{
				_namespaceName = value;
			}
		}

		/// <summary>
		/// Linguagem que sera usada para gerar o codigo.
		/// </summary>
		public string CodeLanguage
		{
			get
			{
				return _codeLanguage;
			}
			set
			{
				_codeLanguage = value;
			}
		}

		/// <summary>
		/// Lista dos atributos que sao adicionados a todas as propriedade geradas.
		/// </summary>
		public List<CodeAttributeDeclaration> PropertyCustomAttributes
		{
			get
			{
				return _propertyCustomAttributes;
			}
		}

		/// <summary>
		/// Lista dos atributos que sao adicionados as classes geradas.
		/// </summary>
		public List<CodeAttributeDeclaration> ClassCustomAttributes
		{
			get
			{
				return _classCustomAttributes;
			}
		}

		/// <summary>
		/// Namespaces que sao importados no codigo.
		/// </summary>
		public List<CodeNamespaceImport> Imports
		{
			get
			{
				return _imports;
			}
		}

		/// <summary>
		/// Define se sera usada o padrao do WCF para gerar as classes.
		/// </summary>
		public bool UsingWCFPattern
		{
			get
			{
				return _usingWCFPattern;
			}
			set
			{
				_usingWCFPattern = value;
			}
		}

		/// <summary>
		/// Padroniza o nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="keepInvalidsChars">Identifica se é para pular os caracteres inválidos.</param>
		/// <returns></returns>
		public static string StandartName(string name, bool keepInvalidsChars)
		{
			bool beginWord = false;
			bool capt = false;
			string result = "";
			for(int i = 0; i < name.Length; i++)
			{
				if(!beginWord && ((name[i] >= 'a' && name[i] <= 'z') || (name[i] >= 'A' && name[i] <= 'Z')))
				{
					result += char.ToUpper(name[i]);
					beginWord = true;
				}
				else if(capt && ((name[i] >= 'a' && name[i] <= 'z') || (name[i] >= 'A' && name[i] <= 'Z') || (name[i] >= '0' && name[i] <= '9')))
				{
					result += char.ToUpper(name[i]);
					capt = false;
				}
				else if(!((name[i] >= 'a' && name[i] <= 'z') || (name[i] >= 'A' && name[i] <= 'Z') || (name[i] >= '0' && name[i] <= '9')))
				{
					if(!keepInvalidsChars)
						result += name[i];
					capt = true;
				}
				else if(i > 0 && (name[i - 1] >= 'a' && name[i - 1] <= 'z') && (name[i] >= 'A' && name[i] <= 'Z'))
					result += name[i];
				else
					result += char.ToLower(name[i]);
			}
			return result;
		}

		/// <summary>
		/// Gera o código do mapeamento.
		/// </summary>
		/// <param name="map">Dados do mapeamento.</param>
		/// <param name="baseTypeName">Tipo base da model a ser criada.</param>
		/// <param name="filename">Caminho onde será salvo o código gerado</param>
		public void Generate(TableMap map, string baseTypeName, string filename)
		{
			using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
			{
				Generate(map, baseTypeName, fs);
			}
		}

		/// <summary>
		/// Gera o código do mapeamento.
		/// </summary>
		/// <param name="map">Dados do mapeamento.</param>
		/// <param name="baseTypeName">Tipo base da model a ser criada.</param>
		/// <param name="outStream">Sream onde sera salvo o codigo gerado.</param>
		public void Generate(TableMap map, string baseTypeName, Stream outStream)
		{
			if(map == null)
				throw new ArgumentNullException("map");
			CodeCompileUnit codeCU = new CodeCompileUnit();
			codeCU.StartDirectives.Clear();
			CodeNamespace codeNsp = new CodeNamespace(_namespaceName);
			codeNsp.Imports.Add(new CodeNamespaceImport("System"));
			codeNsp.Imports.Add(new CodeNamespaceImport("GDA"));
			if(UsingWCFPattern)
				codeNsp.Imports.Add(new CodeNamespaceImport("System.Runtime.Serialization"));
			foreach (CodeNamespaceImport cni in _imports)
				codeNsp.Imports.Add(cni);
			codeCU.Namespaces.Add(codeNsp);
			CodeTypeDeclaration codeType = new CodeTypeDeclaration(StandartName(map.TableName, true));
			codeType.Comments.Add(new CodeCommentStatement("<summary>", true));
			codeType.Comments.Add(new CodeCommentStatement(string.Format("This class represent the {0} {1}.", (map.IsView ? "view" : "table"), map.TableName), true));
			if(!string.IsNullOrEmpty(map.Comment))
				codeType.Comments.Add(new CodeCommentStatement(map.Comment, true));
			codeType.Comments.Add(new CodeCommentStatement("</summary>", true));
			if(!string.IsNullOrEmpty(baseTypeName))
				codeType.BaseTypes.Add(new CodeTypeReference(baseTypeName));
			codeType.CustomAttributes.Add(new CodeAttributeDeclaration("PersistenceClass", new CodeAttributeArgument(new CodeTypeReferenceExpression("\"" + StandartName(map.TableName, false) + "\""))));
			if(UsingWCFPattern)
			{
				CodeAttributeDeclaration codeAD = new CodeAttributeDeclaration("DataContract");
				codeAD.Arguments.Add(new CodeAttributeArgument("Name", new CodeTypeReferenceExpression("\"" + StandartName(map.TableName, false) + "\"")));
				codeAD.Arguments.Add(new CodeAttributeArgument("Namespace", new CodeTypeReferenceExpression("\"\"")));
				codeType.CustomAttributes.Add(codeAD);
			}
			foreach (CodeAttributeDeclaration attr in _classCustomAttributes)
				codeType.CustomAttributes.Add(attr);
			for(int i = 0; i < map.Fields.Count; i++)
			{
				FieldMap fm = map.Fields[i];
				if(string.IsNullOrEmpty(fm.ColumnName))
					continue;
				string fieldName = StandartName(fm.ColumnName, true);
				if(fieldName.Length > 1)
					fieldName = "_" + char.ToLower(fieldName[0]) + fieldName.Substring(1);
				else
					fieldName = "_" + fieldName.ToUpper();
				CodeMemberField field = new CodeMemberField(fm.MemberType, fieldName);
				codeType.Members.Add(field);
				if(i == 0)
				{
					CodeRegionDirective startlocalVariablesRegion = new CodeRegionDirective(CodeRegionMode.Start, "Local Variables\r\n");
					field.StartDirectives.Add(startlocalVariablesRegion);
				}
				if(i + 1 == map.Fields.Count || map.Fields.Count == 1)
				{
					CodeRegionDirective endlocalVariablesRegion = new CodeRegionDirective(CodeRegionMode.End, "Local Variables");
					field.EndDirectives.Add(endlocalVariablesRegion);
				}
			}
			for(int i = 0; i < map.Fields.Count; i++)
			{
				FieldMap fm = map.Fields[i];
				if(string.IsNullOrEmpty(fm.ColumnName))
					continue;
				string fieldName = StandartName(fm.ColumnName, true);
				if(fieldName.Length > 1)
					fieldName = "_" + char.ToLower(fieldName[0]) + fieldName.Substring(1);
				else
					fieldName = "_" + fieldName.ToUpper();
				string propertyName = StandartName(fm.ColumnName, true);
				Type pType = fm.MemberType;
				CodeMemberProperty property = new CodeMemberProperty();
				property.Name = propertyName;
				property.Type = new CodeTypeReference(pType);
				property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
				if(!string.IsNullOrEmpty(fm.Comment))
				{
					property.Comments.Add(new CodeCommentStatement("<summary>", true));
					property.Comments.Add(new CodeCommentStatement(fm.Comment, true));
					property.Comments.Add(new CodeCommentStatement("</summary>", true));
				}
				CodeAttributeDeclaration cadPP = new CodeAttributeDeclaration("PersistenceProperty");
				cadPP.Arguments.Add(new CodeAttributeArgument(new CodeTypeReferenceExpression("\"" + fm.ColumnName + "\"")));
				if(fm.IsAutoGenerated)
					cadPP.Arguments.Add(new CodeAttributeArgument(new CodeTypeReferenceExpression("PersistenceParameterType.IdentityKey")));
				else if(fm.IsPrimaryKey)
					cadPP.Arguments.Add(new CodeAttributeArgument(new CodeTypeReferenceExpression("PersistenceParameterType.Key")));
				if(!pType.IsValueType && fm.Size > 0)
				{
					cadPP.Arguments.Add(new CodeAttributeArgument(new CodeTypeReferenceExpression(fm.Size.ToString())));
				}
				property.CustomAttributes.Add(cadPP);
				if(!pType.IsValueType && !fm.IsNullable)
				{
					CodeAttributeDeclaration cadReq = new CodeAttributeDeclaration("RequiredValidator");
					property.CustomAttributes.Add(cadReq);
				}
				if(!string.IsNullOrEmpty(fm.ForeignKeyTableName))
				{
					property.CustomAttributes.Add(new CodeAttributeDeclaration("PersistenceForeignKey", new CodeAttributeArgument(new CodeTypeReferenceExpression("typeof(" + StandartName(fm.ForeignKeyTableName, true) + ")")), new CodeAttributeArgument(new CodeTypeReferenceExpression("\"" + StandartName(fm.ForeignKeyColumnName, true) + "\""))));
				}
				if(UsingWCFPattern)
				{
					CodeAttributeDeclaration codeAD = new CodeAttributeDeclaration("DataMember");
					codeAD.Arguments.Add(new CodeAttributeArgument("Name", new CodeTypeReferenceExpression("\"" + property.Name + "\"")));
					if(!fm.IsNullable)
						codeAD.Arguments.Add(new CodeAttributeArgument("IsRequired", new CodePrimitiveExpression(true)));
					property.CustomAttributes.Add(codeAD);
				}
				foreach (CodeAttributeDeclaration attr in _propertyCustomAttributes)
					property.CustomAttributes.Add(attr);
				property.GetStatements.Add(new CodeMethodReturnStatement(new CodePropertyReferenceExpression(null, fieldName)));
				property.SetStatements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(null, fieldName), new CodePropertySetValueReferenceExpression()));
				codeType.Members.Add(property);
				if(i == 0)
				{
					CodeRegionDirective startlocalVariablesRegion = new CodeRegionDirective(CodeRegionMode.Start, "Properties\r\n");
					property.StartDirectives.Add(startlocalVariablesRegion);
				}
				if(i + 1 == map.Fields.Count || map.Fields.Count == 1)
				{
					CodeRegionDirective endlocalVariablesRegion = new CodeRegionDirective(CodeRegionMode.End, "Properties");
					property.EndDirectives.Add(endlocalVariablesRegion);
				}
			}
			codeNsp.Types.Add(codeType);
			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);
			CodeGeneratorOptions options = new CodeGeneratorOptions();
			options.BracingStyle = "C";
			options.ElseOnClosing = true;
			CodeDomProvider provider = CodeDomProvider.CreateProvider(_codeLanguage);
			provider.GenerateCodeFromNamespace(codeNsp, sw, options);
			string code = sb.ToString();
			code = code.Replace("System.DateTime", "DateTime");
			int pos1 = 0, pos2 = 0, pos3 = 0;
			do
			{
				pos1 = code.IndexOf("System.Nullable<", pos1);
				if(pos1 >= 0)
				{
					pos2 = code.IndexOf('>', pos1);
					pos3 = pos1 + "System.Nullable<".Length;
					code = code.Substring(0, pos1) + code.Substring(pos3, pos2 - pos3) + "?" + code.Substring(pos2 + 1);
				}
			}
			while (pos1 >= 0);
			outStream.Write(System.Text.Encoding.Default.GetBytes(code), 0, code.Length);
		}
	}
}
