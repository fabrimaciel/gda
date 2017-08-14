using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace GDA.Sql
{
	public class ConditionalContainer : Conditional, IGDAParameterContainer
	{
		private List<Conditional> _conditionals = new List<Conditional> ();
		private List<LogicalOperator> _logicalOperators = new List<LogicalOperator> ();
		private Collections.GDAParameterCollection _parameters = new Collections.GDAParameterCollection ();
		private IGDAParameterContainer _parameterContainer;
		public override ConditionalContainer Parent {
			get {
				return base.Parent;
			}
			internal set {
				base.Parent = value;
				ParameterContainer = value.ParameterContainer;
			}
		}
		public virtual IGDAParameterContainer ParameterContainer {
			get {
				if (_parameterContainer == null)
					_parameterContainer = new Collections.GDAParameterCollection ();
				return _parameterContainer;
			}
			set {
				if (_parameterContainer != null && value != null && _parameterContainer != value)
					foreach (var i in _parameterContainer) {
						var a = false;
						foreach (var j in value)
							if (j.ParameterName == i.ParameterName) {
								a = true;
								continue;
							}
						if (!a)
							value.Add (i);
					}
				_parameterContainer = value;
			}
		}
		public int Count {
			get {
				return _conditionals.Count;
			}
		}
		public ConditionalContainer ()
		{
		}
		public ConditionalContainer (string a)
		{
			if (!string.IsNullOrEmpty (a)) {
				var b = new Conditional (a);
				b.Parent = this;
				_conditionals.Add (b);
			}
		}
		public ConditionalContainer (Conditional a)
		{
			if (a != null) {
				a.Parent = this;
				_conditionals.Add (a);
				if (a is ConditionalContainer) {
					var b = (ConditionalContainer)a;
					if (b.ParameterContainer != this.ParameterContainer)
						b.ParameterContainer = this.ParameterContainer;
				}
			}
		}
		public virtual ConditionalContainer And (Conditional a)
		{
			if (a == null)
				throw new ArgumentNullException ("conditional");
			a.Parent = this;
			_conditionals.Add (a);
			if (_conditionals.Count > 1)
				_logicalOperators.Add (LogicalOperator.And);
			return this;
		}
		public virtual ConditionalContainer Or (Conditional a)
		{
			if (a == null)
				throw new ArgumentNullException ("conditional");
			a.Parent = this;
			_conditionals.Add (a);
			if (_conditionals.Count > 1)
				_logicalOperators.Add (LogicalOperator.Or);
			return this;
		}
		public virtual ConditionalContainer And (string a)
		{
			if (a == null)
				throw new ArgumentNullException ("expression");
			_conditionals.Add (new Conditional (a));
			if (_conditionals.Count > 1)
				_logicalOperators.Add (LogicalOperator.And);
			return this;
		}
		public virtual ConditionalContainer Or (string a)
		{
			if (a == null)
				throw new ArgumentNullException ("expression");
			_conditionals.Add (new Conditional (a));
			if (_conditionals.Count > 1)
				_logicalOperators.Add (LogicalOperator.Or);
			return this;
		}
		public virtual ConditionalContainer Start (Conditional a)
		{
			if (a == null)
				throw new ArgumentNullException ("conditional");
			foreach (var i in _conditionals) {
				i.Parent = null;
			}
			_conditionals.Clear ();
			_logicalOperators.Clear ();
			a.Parent = this;
			_conditionals.Add (a);
			return this;
		}
		public virtual ConditionalContainer Start (string a)
		{
			if (a == null)
				throw new ArgumentNullException ("expression");
			foreach (var i in _conditionals) {
				i.Parent = null;
			}
			_conditionals.Clear ();
			_logicalOperators.Clear ();
			_conditionals.Add (new Conditional (a));
			return this;
		}
		public override string ToString ()
		{
			var a = new StringBuilder ();
			for (int b = 0, c = -1; b < _conditionals.Count; b++, c++) {
				if (_logicalOperators.Count > c && c >= 0)
					a.Append (_logicalOperators [c] == LogicalOperator.Or ? " OR " : " AND ");
				a.Append (_conditionals [b].ToString ());
			}
			if (Parent != null)
				return "(" + a.ToString () + ")";
			else
				return a.ToString ();
		}
		public ConditionalContainer Add (string a, DbType b, object c)
		{
			ParameterContainer.Add (new GDAParameter (a, c) {
				DbType = b
			});
			return this;
		}
		public ConditionalContainer Add (string a, object b)
		{
			ParameterContainer.Add (new GDAParameter (a, b));
			return this;
		}
		public ConditionalContainer Add (DbType a, int b, object c)
		{
			ParameterContainer.Add (new GDAParameter () {
				DbType = a,
				Size = b,
				Value = c
			});
			return this;
		}
		public ConditionalContainer Add (string a, DbType b, int c, object d)
		{
			ParameterContainer.Add (new GDAParameter () {
				ParameterName = a,
				DbType = b,
				Size = c,
				Value = d
			});
			return this;
		}
		public ConditionalContainer Add (GDAParameter a)
		{
			ParameterContainer.Add (a);
			return this;
		}
		IEnumerator<GDAParameter> IEnumerable<GDAParameter>.GetEnumerator ()
		{
			return ParameterContainer.GetEnumerator ();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return ParameterContainer.GetEnumerator ();
		}
		void IGDAParameterContainer.Add (GDAParameter parameter)
		{
			ParameterContainer.Add (parameter);
		}
		bool IGDAParameterContainer.TryGet (string a, out GDAParameter b)
		{
			return ParameterContainer.TryGet (a, out b);
		}
		bool IGDAParameterContainer.ContainsKey (string a)
		{
			return ParameterContainer.ContainsKey (a);
		}
		bool IGDAParameterContainer.Remove (string a)
		{
			return ParameterContainer.Remove (a);
		}
	}
}
