using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql
{
	public class QueryReturnInfo
	{
		private List<GDAParameter> _parameters;
		private string _commandText;
		private List<Mapper> _recoverProperties;
		public List<GDAParameter> Parameters {
			get {
				return _parameters;
			}
		}
		public string CommandText {
			get {
				return _commandText;
			}
		}
		internal List<Mapper> RecoverProperties {
			get {
				return _recoverProperties;
			}
		}
		internal QueryReturnInfo (string a, IList<GDAParameter> b, List<Mapper> c)
		{
			_commandText = a;
			_parameters = new List<GDAParameter> (b);
			_recoverProperties = c;
		}
	}
}
