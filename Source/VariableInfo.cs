using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace GDA.Sql
{
	public class VariableInfo
	{
		private GDA.Sql.InterpreterExpression.Expression _expression;
		private string _name;
		public string Name {
			get {
				return _name;
			}
		}
		internal VariableInfo (GDA.Sql.InterpreterExpression.Expression a)
		{
			_expression = a;
			_name = a.Text;
		}
		public bool Replace (GDA.Interfaces.IProvider a, IGDAParameterContainer b, Dictionary<string, Type> c)
		{
			if (b == null)
				return false;
			GDAParameter d = null;
			if (!b.TryGet (Name, out d))
				return false;
			QueryReturnInfo e = null;
			if (d.Value is IQuery) {
				var f = (IQuery)d.Value;
				e = f.BuildResultInfo2 (a, f.AggregationFunctionProperty, c);
			}
			else
				e = d.Value as QueryReturnInfo;
			if (e != null) {
				_expression.Text = e.CommandText;
				foreach (var p in e.Parameters)
					if (!b.ContainsKey (p.ParameterName))
						b.Add (p);
				return true;
			}
			return false;
		}
		public class VariableInfoComparer : IComparer<VariableInfo>
		{
			public static readonly IComparer<VariableInfo> Instance;
			static VariableInfoComparer ()
			{
				Instance = new VariableInfoComparer ();
			}
			public int Compare (VariableInfo a, VariableInfo b)
			{
				if (a == null && b != null)
					return -1;
				else if (a != null && b == null)
					return 1;
				else if (a == null && b == null)
					return 0;
				return StringComparer.Ordinal.Compare (a.Name, b.Name);
			}
		}
	}
}
