using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using GDA.Sql.InterpreterExpression;
using GDA.Provider;
using GDA.Interfaces;
using GDA.Sql.InterpreterExpression.Nodes;
using GDA.Caching;
using GDA.Collections;
namespace GDA.Sql
{
	public class Query : BaseQuery, IEquatable<Query>, IGDAParameterContainer
	{
		private GDAParameterCollection _parameters = new GDAParameterCollection (3);
		private ConditionalContainer _conditional;
		private string orderClause;
		private string groupByClause;
		private bool _useDistinct = false;
		private string _mainAlias;
		private List<JoinInfo> _joins = new List<JoinInfo> ();
		private string _selectProperties;
		public string MainAlias {
			get {
				return _mainAlias;
			}
			set {
				_mainAlias = value;
			}
		}
		public string SelectProperties {
			get {
				return _selectProperties;
			}
		}
		public List<GDAParameter> Parameters {
			get {
				return _parameters;
			}
		}
		public List<JoinInfo> Join {
			get {
				return _joins;
			}
		}
		protected ConditionalContainer WhereConditional {
			get {
				if (_conditional == null)
					_conditional = new ConditionalContainer ("");
				return _conditional;
			}
			set {
				if (value == null)
					_conditional = new ConditionalContainer ("");
				else
					_conditional = value;
			}
		}
		public Query () : this (null)
		{
		}
		public Query (string a) : this (a, null, null)
		{
		}
		public Query (string a, string b) : this (a, b, null)
		{
		}
		public Query (string a, IEnumerable<GDAParameter> b) : this (a, null, b)
		{
		}
		public Query (string a, string b, IEnumerable<GDAParameter> c)
		{
			Where = a;
			this.orderClause = b;
			if (c != null)
				foreach (var i in c) {
					var d = this._parameters.FindIndex (e => e.ParameterName == i.ParameterName);
					if (d >= 0)
						this._parameters.RemoveAt (d);
					this._parameters.Add (i);
				}
		}
		public Query Alias (string a)
		{
			_mainAlias = a;
			return this;
		}
		public Query Distinct ()
		{
			_useDistinct = true;
			return this;
		}
		public Query RemoveDistinct ()
		{
			_useDistinct = false;
			return this;
		}
		public Query Skip (int a)
		{
			_skipCount = a;
			return this;
		}
		public Query Take (int a)
		{
			_takeCount = a;
			return this;
		}
		public string Where {
			get {
				return WhereConditional.ToString ();
			}
			set {
				WhereConditional = new ConditionalContainer (value) {
					ParameterContainer = this
				};
			}
		}
		public QueryWhereClause WhereClause {
			get {
				return new QueryWhereClause (this, _conditional);
			}
		}
		public ConditionalContainer CreateWhereClause (string a)
		{
			Where = a;
			return _conditional;
		}
		public Query SetWhereClause (ConditionalContainer a)
		{
			if (a == null)
				Where = "";
			else {
				_conditional = a;
				_conditional.ParameterContainer = this;
			}
			return this;
		}
		public string Order {
			get {
				return orderClause;
			}
			set {
				orderClause = value;
			}
		}
		public string GroupByClause {
			get {
				return groupByClause;
			}
			set {
				groupByClause = value;
			}
		}
		public Query Select (string a)
		{
			_selectProperties = a;
			return this;
		}
		#if CLS_3_5
        /// <summary>
        /// Recupera o seletor de propriedade.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertiesSelector"></param>
        /// <returns></returns>
        public QueryPropertySelector<T> PropertySelector<T>(params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : new()
        {
            return new QueryPropertySelector<T>(this).Add(propertiesSelector);
        }
        /// <summary>
        /// Seleciona o primeiro registro para a consulta e filtra as colunas 
        /// do tipo que irão para o resultado.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session">Sessão que será usada na execução do comando.</param>
        /// <param name="propertiesSelector">Seletor das propriedades que deverão ser retornadas.</param>
        /// <returns></returns>
        public GDADataRecord SelectFirst<T>(GDASession session, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector)
        {
            if (propertiesSelector != null)
            {
                var properties = new List<string>();
                foreach (var i in propertiesSelector)
                {
                    if (i == null) continue;
                    var property = i.GetMember() as System.Reflection.PropertyInfo;
                    if (property != null)
                        properties.Add(property.Name);
                }
                if (properties.Count > 0)
                    Select(string.Join(", ", properties.ToArray()));
            }
            using (var enumerator = ToDataRecords<T>(session).GetEnumerator())
            {
                if (enumerator.MoveNext())
                    return enumerator.Current;
            }
            return null;
        }
        /// <summary>
        /// Seleciona o primeiro registro para a consulta e filtra as colunas 
        /// do tipo que irão para o resultado.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertiesSelector">Seletor das propriedades que deverão ser retornadas.</param>
        /// <returns></returns>
        public GDADataRecord SelectFirst<T>(params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector)
        {
            GDASession session = null;
            return SelectFirst<T>(session, propertiesSelector);
        }
        /// <summary>
        /// Executa a consulta selecionar o valor da coluna
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session">Sessão que será usada na execução do comando.</param>
        /// <param name="selector">Propriedade que será recuperada.</param>
        /// <returns></returns>
        public GDAPropertyValue SelectFirstValue<T>(GDASession session, System.Linq.Expressions.Expression<Func<T, object>> selector)
        {
            if (selector == null)
                throw new ArgumentException("selector");
            var property = selector.GetMember() as System.Reflection.PropertyInfo;
            Select(property.Name);
            using (var enumerator = ToDataRecords<T>(session).GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    var record = enumerator.Current;
                    if (record[0].ValueExists)
                        return record[0];
                }
            }
            return new GDAPropertyValue(null, false);
        }
        /// <summary>
        /// Executa a consulta selecionar o valor da coluna
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector">Propriedade que será recuperada.</param>
        /// <returns></returns>
        public GDAPropertyValue SelectFirstValue<T>(System.Linq.Expressions.Expression<Func<T, object>> selector)
        {
            return SelectFirstValue<T>(null, selector);
        }
#endif
		public Query InnerJoin<ClassJoin> ()
		{
			return InnerJoin<ClassJoin> (null);
		}
		public Query InnerJoin<ClassJoin> (string a)
		{
			return InnerJoin<ClassJoin> (a, null);
		}
		public Query InnerJoin<ClassJoin> (string a, string b)
		{
			_joins.Add (new JoinInfo (JoinType.InnerJoin, null, typeof(ClassJoin), null, a, b));
			return this;
		}
		public Query InnerJoin<ClassMain, ClassJoin> ()
		{
			return InnerJoin<ClassMain, ClassJoin> (null);
		}
		public Query InnerJoin<ClassMain, ClassJoin> (string groupRelationship)
		{
			return InnerJoin<ClassMain, ClassJoin> (null, null, groupRelationship);
		}
		public Query InnerJoin<ClassMain, ClassJoin> (string a, string b, string c)
		{
			_joins.Add (new JoinInfo (JoinType.InnerJoin, typeof(ClassMain), typeof(ClassJoin), a, b, c));
			return this;
		}
		public Query LeftJoin<ClassJoin> ()
		{
			return LeftJoin<ClassJoin> (null);
		}
		public Query LeftJoin<ClassJoin> (string a)
		{
			return LeftJoin<ClassJoin> (a, null);
		}
		public Query LeftJoin<ClassJoin> (string a, string b)
		{
			_joins.Add (new JoinInfo (JoinType.LeftJoin, null, typeof(ClassJoin), null, a, b));
			return this;
		}
		public Query LeftJoin<ClassMain, ClassJoin> ()
		{
			return LeftJoin<ClassMain, ClassJoin> (null);
		}
		public Query LeftJoin<ClassMain, ClassJoin> (string groupRelationship)
		{
			return LeftJoin<ClassMain, ClassJoin> (null, null, groupRelationship);
		}
		public Query LeftJoin<ClassMain, ClassJoin> (string a, string b, string c)
		{
			_joins.Add (new JoinInfo (JoinType.LeftJoin, typeof(ClassMain), typeof(ClassJoin), a, b, c));
			return this;
		}
		public Query RightJoin<ClassJoin> ()
		{
			return RightJoin<ClassJoin> (null);
		}
		public Query RightJoin<ClassJoin> (string a)
		{
			return RightJoin<ClassJoin> (a, null);
		}
		public Query RightJoin<ClassJoin> (string a, string b)
		{
			_joins.Add (new JoinInfo (JoinType.RightJoin, null, typeof(ClassJoin), null, a, b));
			return this;
		}
		public Query RightJoin<ClassMain, ClassJoin> ()
		{
			return RightJoin<ClassMain, ClassJoin> (null);
		}
		public Query RightJoin<ClassMain, ClassJoin> (string groupRelationship)
		{
			return RightJoin<ClassMain, ClassJoin> (null, null, groupRelationship);
		}
		public Query RightJoin<ClassMain, ClassJoin> (string a, string b, string c)
		{
			_joins.Add (new JoinInfo (JoinType.RightJoin, typeof(ClassMain), typeof(ClassJoin), a, b, c));
			return this;
		}
		public Query Add (GDAParameter a)
		{
			if (a != null) {
				var b = this._parameters.FindIndex (c => c.ParameterName == a.ParameterName);
				if (b >= 0)
					this._parameters.RemoveAt (b);
				this._parameters.Add (a);
			}
			return this;
		}
		public Query Add (params GDAParameter[] a)
		{
			foreach (var i in a) {
				var b = this._parameters.FindIndex (c => c.ParameterName == i.ParameterName);
				if (b >= 0)
					this._parameters.RemoveAt (b);
				this._parameters.Add (i);
			}
			return this;
		}
		public Query Add (IEnumerable<GDAParameter> a)
		{
			foreach (var i in a) {
				var b = this._parameters.FindIndex (c => c.ParameterName == i.ParameterName);
				if (b >= 0)
					this._parameters.RemoveAt (b);
				this._parameters.Add (i);
			}
			return this;
		}
		public Query SetWhere (string a)
		{
			Where = a;
			return this;
		}
		public Query SetOrder (string a)
		{
			this.orderClause = a;
			return this;
		}
		public Query SetGroupBy (string a)
		{
			this.groupByClause = a;
			return this;
		}
		public ResultList<T> ToResultList<T> (int a) where T : new()
		{
			return new ResultList<T> (this, a);
		}
		public ResultList<T> ToResultList<T> (GDASession a, int b) where T : new()
		{
			return new ResultList<T> (this, a, b);
		}
		public override IEnumerable<Result> ToCursor<T, Result> (GDASession a)
		{
			if (string.IsNullOrEmpty (_selectProperties)) {
				var b = new List<string> ();
				var c = typeof(Result).GetProperties (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.SetField);
				foreach (var i in MappingManager.GetMappers<T> (null, new DirectionParameter[] {
					DirectionParameter.Input,
					DirectionParameter.InputOutput
				})) {
					if (Helper.GDAHelper.Exists (c, d => d.Name == i.PropertyMapperName))
						b.Add (i.PropertyMapperName);
				}
				_selectProperties = string.Join (", ", b.ToArray ());
			}
			return base.ToCursor<T, Result> (a);
		}
		public override GDAPropertyValue GetValue<T> (GDASession a, string b)
		{
			var c = _selectProperties;
			_selectProperties = b;
			try {
				foreach (var i in ToDataRecords<T> (a))
					return i [b];
				return new GDAPropertyValue (null, false);
			}
			finally {
				_selectProperties = c;
			}
		}
		public override IEnumerable<GDAPropertyValue> GetValues<T> (GDASession a, string b)
		{
			var c = _selectProperties;
			_selectProperties = b;
			try {
				foreach (var i in ToDataRecords<T> (a))
					yield return i [b];
			}
			finally {
				_selectProperties = c;
			}
		}
		public int Delete<T> ()
		{
			return Delete<T> (null);
		}
		public int Delete<T> (GDASession a)
		{
			_returnTypeQuery = typeof(T);
			IProviderConfiguration b = GDAOperations.GetDAO (_returnTypeQuery).Configuration;
			List<Mapper> c = MappingManager.GetMappers (_returnTypeQuery, null, null);
			var d = MappingManager.GetTableName (_returnTypeQuery);
			StringBuilder e = new StringBuilder (string.Format ("DELETE FROM {0}", b.Provider.BuildTableName (d)));
			if (!string.IsNullOrEmpty (this.Where)) {
				Parser f = new Parser (new Lexer (this.Where));
				WherePart g = f.ExecuteWherePart ();
				SelectStatement h = new SelectStatement (g);
				foreach (ColumnInfo ci in h.ColumnsInfo) {
					if (string.IsNullOrEmpty (ci.TableNameOrTableAlias) || ci.TableNameOrTableAlias == "main") {
						ci.TableNameOrTableAlias = b.Provider.BuildTableName (d);
					}
					Mapper j = c.Find (delegate (Mapper k) {
						return (string.Compare (k.PropertyMapperName, ci.ColumnName, true) == 0);
					});
					if (j == null) {
						j = MappingManager.GetPropertyMapper (_returnTypeQuery, ci.ColumnName);
						if (j == null)
							throw new GDAException ("Property {0} not exists in {1} or not mapped.", ci.ColumnName, _returnTypeQuery.FullName);
					}
					ci.DBColumnName = j.Name;
					ci.RenameToMapper (b.Provider);
				}
				ParserToSqlCommand l = new ParserToSqlCommand (g, b.Provider.QuoteExpressionBegin, b.Provider.QuoteExpressionEnd);
				e.Append (" WHERE " + l.SqlCommand);
			}
			DataAccess m = new DataAccess (b);
			return m.ExecuteCommand (a, e.ToString (), this.Parameters.ToArray ());
		}
		public static Query From<T> (string a) where T : new()
		{
			Query b = new Query (a);
			b.ReturnTypeQuery = typeof(T);
			return b;
		}
		public static Query From<T> () where T : new()
		{
			Query a = new Query ();
			a.ReturnTypeQuery = typeof(T);
			return a;
		}
		public override QueryReturnInfo BuildResultInfo<T> (GDA.Interfaces.IProviderConfiguration a)
		{
			this._returnTypeQuery = typeof(T);
			if (a != null)
				a = GDA.GDASettings.DefaultProviderConfiguration;
			return this.BuildResultInfo2 (a.Provider, null);
		}
		public QueryReturnInfo BuildResultInfo<T> (string a)
		{
			this._returnTypeQuery = typeof(T);
			return this.BuildResultInfo (a);
		}
		public override QueryReturnInfo BuildResultInfo (string aggregationFunction)
		{
			if (_returnTypeQuery == null)
				throw new QueryException ("Type return query not found.");
			IProvider provider = GDAOperations.GetDAO (_returnTypeQuery).Configuration.Provider;
			return BuildResultInfo2 (provider, aggregationFunction);
		}
		public override QueryReturnInfo BuildResultInfo2 (IProvider a, string b)
		{
			return BuildResultInfo2 (a, b, new Dictionary<string, Type> ());
		}
		public override QueryReturnInfo BuildResultInfo2 (GDA.Interfaces.IProvider a, string b, Dictionary<string, Type> c)
		{
			if (_returnTypeQuery == null)
				throw new QueryException ("Type return query not found.");
			if (c == null)
				c = new Dictionary<string, Type> ();
			var d = "main";
			if (string.IsNullOrEmpty (MainAlias)) {
				for (var e = 1; c.ContainsKey (d); e++)
					d = string.Format ("main{0}", e);
			}
			else
				d = MainAlias;
			c.Add (d, _returnTypeQuery);
			foreach (JoinInfo ji in _joins) {
				if (ji.ClassTypeMain == null) {
					ji.ClassTypeMain = _returnTypeQuery;
					ji.ClassTypeMainAlias = d;
				}
				else {
					if (string.IsNullOrEmpty (ji.ClassTypeMainAlias))
						ji.ClassTypeMainAlias = ji.ClassTypeJoin.Name.ToLower ();
					if (!c.ContainsKey (ji.ClassTypeMainAlias))
						c.Add (ji.ClassTypeMainAlias, ji.ClassTypeMain);
				}
				if (string.IsNullOrEmpty (ji.ClassTypeJoinAlias))
					ji.ClassTypeJoinAlias = ji.ClassTypeJoin.Name.ToLower ();
				if (!c.ContainsKey (ji.ClassTypeJoinAlias))
					c.Add (ji.ClassTypeJoinAlias, ji.ClassTypeJoin);
			}
			List<Mapper> f = new List<Mapper> ();
			List<Mapper> g = new List<Mapper> ();
			List<Mapper> h = MappingManager.GetMappers (_returnTypeQuery, null, null);
			List<GroupOfRelationshipInfo> j = MappingManager.GetForeignKeyAttributes (_returnTypeQuery);
			if (h.Count == 0) {
			}
			StringBuilder k = new StringBuilder (128).Append ("SELECT ");
			if (_useDistinct)
				k.Append ("DISTINCT ");
			bool l = false;
			if (!string.IsNullOrEmpty (_selectProperties)) {
				var m = new List<string> ();
				Parser n = new Parser (new Lexer (_selectProperties));
				SelectPart o = n.ExecuteSelectPart ();
				var p = new List<string> (o.SelectionExpressions.Count);
				var q = new List<SelectExpression> ();
				foreach (SelectExpression se in o.SelectionExpressions) {
					if (se.ColumnName.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Column) {
						Column r = se.Column;
						if (string.IsNullOrEmpty (r.TableName)) {
							se.ColumnName.Value.Text = string.Format ("{0}.{1}", d, se.ColumnName.Value.Text);
							r = se.Column;
						}
						if (r.TableName == d && r.Name == "*") {
							foreach (Mapper mp in h) {
								if (mp.Direction == DirectionParameter.Input || mp.Direction == DirectionParameter.InputOutput || mp.Direction == DirectionParameter.OutputOnlyInsert) {
									var s = new SelectExpression (new SqlExpression (new Expression (0, 0) {
										Text = string.Format ("{0}.{1}", d, mp.PropertyMapperName)
									}), new SqlExpression (new Expression (0, 0) {
										Text = mp.PropertyMapperName
									}));
									q.Add (s);
								}
							}
							continue;
						}
						else if (r.Name == "*")
							throw new GDAException ("Invalid expression {0}", se.ColumnName.Value.Text);
						else {
							q.Add (se);
						}
					}
					else if (se.ColumnName.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Function) {
						SqlFunction t = (SqlFunction)se.ColumnName;
						if (t.Parameters.Count > 0) {
							foreach (List<SqlExpression> listSqlEx in t.Parameters) {
								foreach (SqlExpression ss1 in listSqlEx) {
									if (ss1.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Column) {
										ColumnInfo r = new ColumnInfo (ss1.Value);
										if (string.IsNullOrEmpty (r.TableNameOrTableAlias))
											r.TableNameOrTableAlias = d;
										if (r.ColumnName == "*")
											break;
										if (c.ContainsKey (r.TableNameOrTableAlias)) {
											var u = MappingManager.GetMappers (c [r.TableNameOrTableAlias], null, null);
											bool v = false;
											foreach (Mapper mapper in u)
												if (string.Compare (r.ColumnName, mapper.PropertyMapperName, true) == 0) {
													r.DBColumnName = mapper.Name;
													r.RenameToMapper (a);
													v = true;
													break;
												}
											if (!v)
												throw new GDAException ("Property {0} not found for type {1}.", r.ColumnName, c [r.TableNameOrTableAlias].FullName);
										}
										else {
											throw new GDAException ("Classe alias {0} not found.", r.TableNameOrTableAlias);
										}
									}
								}
							}
						}
						q.Add (se);
					}
				}
				l = false;
				foreach (var se in q) {
					if (se.ColumnName.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Column) {
						var w = se.Column;
						Type x = null;
						if (!c.TryGetValue (w.TableName, out x))
							throw new GDAException ("Not found class for alias {0}", w.TableName);
						var y = MappingManager.GetMappers (x, null, null);
						if (y == null)
							continue;
						var z = y.Find (A => StringComparer.InvariantCultureIgnoreCase.Equals (w.Name, A.PropertyMapperName));
						if (z == null)
							throw new GDAException ("Not fount mapping {0} for type {1}", w.Name, x.FullName);
						if (l)
							k.Append (", ");
						else
							l = true;
						k.Append (a.QuoteExpression (w.TableName)).Append (".").Append (a.QuoteExpression (z.Name));
						if (!string.IsNullOrEmpty (w.Alias)) {
							var B = h.Find (A => StringComparer.InvariantCultureIgnoreCase.Equals (w.Alias, A.PropertyMapperName));
							if (B != null) {
								f.Add (B);
								k.Append (" AS ").Append (a.QuoteExpression (B.Name));
							}
							else
								k.Append (" AS ").Append (a.QuoteExpression (w.Alias));
						}
						else
							f.Add (z);
					}
					else if (se.ColumnName.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Function) {
						if (l)
							k.Append (", ");
						else
							l = true;
						k.Append (se.ColumnName.ToString ());
						if (se.ColumnAlias != null) {
							var C = se.ColumnAlias.ToString ();
							var B = h.Find (A => StringComparer.InvariantCultureIgnoreCase.Equals (C, A.PropertyMapperName));
							if (B != null) {
								f.Add (B);
								k.Append (" AS ").Append (a.QuoteExpression (B.Name));
							}
							else
								k.Append (" AS ").Append (a.QuoteExpression (C));
						}
					}
				}
				l = !string.IsNullOrEmpty (_selectProperties);
			}
			l = false;
			foreach (Mapper column in h) {
				switch (column.Direction) {
				case DirectionParameter.Input:
				case DirectionParameter.InputOutput:
				case DirectionParameter.OutputOnlyInsert:
					if (string.IsNullOrEmpty (_selectProperties)) {
						if (l)
							k.Append (", ");
						else
							l = true;
						k.Append (a.QuoteExpression (d)).Append (".").Append (a.QuoteExpression (column.Name));
						f.Add (column);
					}
					g.Add (column);
					break;
				}
			}
			IList<ForeignMemberMapper> D = MappingManager.GetForeignMemberMapper (_returnTypeQuery);
			foreach (ForeignMemberMapper fmm in D) {
				int E = _joins.FindIndex (delegate (JoinInfo F) {
					return F.ClassTypeJoin == fmm.TypeOfClassRelated && F.GroupOfRelationship == fmm.GroupOfRelationship;
				});
				if (E >= 0) {
					if (l)
						k.Append (", ");
					else
						l = true;
					PersistencePropertyAttribute G = MappingManager.GetPersistenceProperty (fmm.PropertyOfClassRelated);
					if (G == null)
						throw new GDAException ("Fail on ForeignKeyMember, property {0} in {1} isn't mapped.", fmm.PropertyOfClassRelated.Name, fmm.TypeOfClassRelated.FullName);
					k.Append (a.QuoteExpression (_joins [E].ClassTypeJoinAlias)).Append (".").Append (a.QuoteExpression (G.Name)).Append (" AS ").Append (a.QuoteExpression (fmm.PropertyModel.Name));
					f.Add (new Mapper (_returnTypeQuery, new PersistencePropertyAttribute (fmm.PropertyModel.Name, DirectionParameter.InputOptional), fmm.PropertyModel));
				}
			}
			if (!string.IsNullOrEmpty (b)) {
				Parser H = new Parser (new Lexer (b));
				SelectPart I = H.ExecuteSelectPart ();
				if (I.SelectionExpressions.Count == 0)
					throw new GDAException ("Not found aggregation function.");
				foreach (SelectExpression sqlEx in I.SelectionExpressions)
					if (sqlEx.ColumnName is SqlFunction) {
						SqlFunction t = (SqlFunction)sqlEx.ColumnName;
						if (t.Parameters.Count > 0) {
							foreach (List<SqlExpression> listSqlEx in t.Parameters) {
								foreach (SqlExpression ss1 in listSqlEx) {
									if (ss1.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Column) {
										Column r = new Column (ss1, null);
										if (string.IsNullOrEmpty (r.TableName))
											r.TableName = d;
										if (r.Name == "*")
											break;
										if (c.ContainsKey (r.TableName)) {
											List<Mapper> J = MappingManager.GetMappers (c [r.TableName], null, null);
											bool v = false;
											foreach (Mapper mapper in J)
												if (string.Compare (r.Name, mapper.PropertyMapperName, true) == 0) {
													b = b.Substring (0, ss1.Value.BeginPoint) + a.QuoteExpression (r.TableName) + "." + a.QuoteExpression (mapper.Name) + b.Substring (ss1.Value.BeginPoint + ss1.Value.Length);
													v = true;
													break;
												}
											if (!v)
												throw new GDAException ("Property {0} not found.", r.Name);
										}
										else {
											throw new GDAException ("Classe alias {0} not found.", r.TableName);
										}
									}
								}
							}
						}
					}
				k = new StringBuilder ("SELECT ").Append (b);
			}
			k.Append (" FROM ").Append (a.BuildTableName (MappingManager.GetTableName (_returnTypeQuery))).Append (" ").Append (a.QuoteExpression (d)).Append (" ");
			StringBuilder K = new StringBuilder ();
			if (_joins.Count > 0) {
				int L = 1;
				for (int e = 0; e < _joins.Count; e++) {
					JoinInfo F = _joins [e];
					GroupOfRelationshipInfo M = j.Find (delegate (GroupOfRelationshipInfo N) {
						return N.TypeOfClassRelated == F.ClassTypeJoin && N.GroupOfRelationship == F.GroupOfRelationship;
					});
					if (M == null) {
						M = MappingManager.GetForeignKeyAttributes (F.ClassTypeJoin).Find (delegate (GroupOfRelationshipInfo N) {
							return N.TypeOfClassRelated == _returnTypeQuery && N.GroupOfRelationship == F.GroupOfRelationship;
						});
						if (M == null) {
							foreach (GroupOfRelationshipInfo gri2 in j) {
								MappingManager.LoadClassMapper (gri2.TypeOfClassRelated);
								M = MappingManager.GetForeignKeyAttributes (gri2.TypeOfClassRelated).Find (delegate (GroupOfRelationshipInfo N) {
									return N.TypeOfClassRelated == F.ClassTypeJoin && N.GroupOfRelationship == F.GroupOfRelationship;
								});
								if (M != null) {
									F.ClassTypeMain = M.TypeOfClass;
									int O = 0;
									for (O = 0; O < _joins.Count; O++)
										if (_joins [O].ClassTypeJoin == M.TypeOfClass) {
											F.ClassTypeMainAlias = _joins [O].ClassTypeJoinAlias;
											break;
										}
									if (O >= _joins.Count) {
										F.ClassTypeMainAlias = F.ClassTypeMain.Name;
										_joins.Add (new JoinInfo (F.Type, ReturnTypeQuery, M.TypeOfClass, F.ClassTypeMain.Name, F.ClassTypeJoinAlias, M.GroupOfRelationship));
									}
									break;
								}
							}
							if (M == null) {
								foreach (KeyValuePair<string, Type> it in c) {
									if (it.Key == d)
										continue;
									MappingManager.LoadClassMapper (it.Value);
									M = MappingManager.GetForeignKeyAttributes (it.Value).Find (delegate (GroupOfRelationshipInfo N) {
										return N.TypeOfClassRelated == F.ClassTypeJoin && N.GroupOfRelationship == F.GroupOfRelationship;
									});
									if (M != null) {
										F.ClassTypeMain = M.TypeOfClass;
										int O = 0;
										for (O = 0; O < _joins.Count; O++)
											if (_joins [O].ClassTypeJoin == M.TypeOfClass) {
												F.ClassTypeMainAlias = _joins [O].ClassTypeJoinAlias;
												break;
											}
										if (O >= _joins.Count) {
											F.ClassTypeMainAlias = F.ClassTypeMain.Name;
											_joins.Add (new JoinInfo (F.Type, ReturnTypeQuery, M.TypeOfClass, F.ClassTypeMain.Name, F.ClassTypeJoinAlias, M.GroupOfRelationship));
										}
										break;
									}
								}
							}
							if (M == null)
								throw new QueryException ("Not found foreign key with {0}", F.ClassTypeJoin.FullName);
						}
					}
					switch (F.Type) {
					case JoinType.InnerJoin:
						K.Append ("INNER JOIN ");
						break;
					case JoinType.LeftJoin:
						K.Append ("LEFT JOIN ");
						break;
					case JoinType.RightJoin:
						K.Append ("RIGHT JOIN ");
						break;
					case JoinType.CrossJoin:
						K.Append ("CROSS JOIN ");
						break;
					}
					K.Append (a.BuildTableName (MappingManager.GetTableName (F.ClassTypeJoin))).Append (" AS ").Append (a.QuoteExpression (F.ClassTypeJoinAlias)).Append (" ON(");
					for (int P = 0; P < M.ForeignKeys.Count; P++) {
						ForeignKeyMapper Q = M.ForeignKeys [P];
						var R = MappingManager.GetPersistenceProperty (Q.PropertyOfClassRelated);
						if (R == null)
							throw new GDAException ("Not found mapper for property '{0}' of type '{1}'.", Q.PropertyOfClassRelated.Name, Q.PropertyOfClassRelated.DeclaringType.FullName);
						K.Append (a.QuoteExpression (F.ClassTypeMainAlias)).Append (".").Append (a.QuoteExpression (MappingManager.GetPersistenceProperty (Q.PropertyModel).Name)).Append ("=").Append (a.QuoteExpression (F.ClassTypeJoinAlias)).Append (".").Append (a.QuoteExpression (R.Name));
						if ((P + 1) < M.ForeignKeys.Count)
							K.Append (" AND ");
					}
					K.Append (") ");
					L++;
				}
			}
			k.Append (K.ToString ());
			if (this._conditional != null && this._conditional.Count > 0) {
				Parser S = new Parser (new Lexer (this.Where));
				WherePart U = S.ExecuteWherePart ();
				SelectStatement V = new SelectStatement (U);
				foreach (ColumnInfo ci in V.ColumnsInfo) {
					if (string.IsNullOrEmpty (ci.TableNameOrTableAlias) || ci.TableNameOrTableAlias == d) {
						ci.TableNameOrTableAlias = a.QuoteExpression (d);
					}
					else {
						foreach (JoinInfo ji in this._joins) {
							if (ji.ClassTypeJoin.Name == ci.TableNameOrTableAlias || ji.ClassTypeJoin.FullName == ci.TableNameOrTableAlias || ji.ClassTypeJoinAlias == ci.TableNameOrTableAlias) {
								List<Mapper> W = MappingManager.GetMappers (ji.ClassTypeJoin, null, null);
								Mapper X = W.Find (delegate (Mapper Y) {
									return (string.Compare (Y.PropertyMapperName, ci.ColumnName, true) == 0);
								});
								if (X == null) {
									X = MappingManager.GetPropertyMapper (ji.ClassTypeJoin, ci.ColumnName);
									if (X == null)
										throw new GDAException ("Property {0} not exists in {1} or not mapped.", ci.ColumnName, ji.ClassTypeJoin.FullName);
								}
								ci.TableNameOrTableAlias = ji.ClassTypeJoinAlias;
								ci.DBColumnName = X.Name;
								ci.RenameToMapper (a);
								break;
							}
						}
						if (string.IsNullOrEmpty (ci.DBColumnName)) {
							Type Z = null;
							if (c.TryGetValue (ci.TableNameOrTableAlias, out Z)) {
								List<Mapper> W = MappingManager.GetMappers (Z, null, null);
								Mapper X = W.Find (delegate (Mapper Y) {
									return (string.Compare (Y.PropertyMapperName, ci.ColumnName, true) == 0);
								});
								if (X == null) {
									X = MappingManager.GetPropertyMapper (Z, ci.ColumnName);
									if (X == null)
										throw new GDAException ("Property {0} not exists in {1} or not mapped.", ci.ColumnName, Z.FullName);
								}
								ci.DBColumnName = X.Name;
								ci.RenameToMapper (a);
							}
							if (string.IsNullOrEmpty (ci.DBColumnName))
								throw new QueryException ("Field {0} not found in mapping.", ci.ToString ());
						}
						continue;
					}
					Mapper _ = g.Find (delegate (Mapper Y) {
						return (string.Compare (Y.PropertyMapperName, ci.ColumnName, true) == 0);
					});
					if (_ == null) {
						_ = MappingManager.GetPropertyMapper (_returnTypeQuery, ci.ColumnName);
						if (_ == null)
							throw new GDAException ("Property {0} not exists in {1} or not mapped.", ci.ColumnName, _returnTypeQuery.FullName);
					}
					ci.DBColumnName = _.Name;
					ci.RenameToMapper (a);
				}
				foreach (var variableInfo in V.VariablesInfo)
					variableInfo.Replace (a, this, c);
				ParserToSqlCommand a0 = new ParserToSqlCommand (U, a.QuoteExpressionBegin, a.QuoteExpressionEnd);
				k.Append (" WHERE ").Append (a0.SqlCommand);
			}
			if (!string.IsNullOrEmpty (this.GroupByClause)) {
				Parser S = new Parser (new Lexer (this.GroupByClause));
				GroupByPart a1 = S.ExecuteGroupByPart ();
				SelectStatement V = new SelectStatement (a1);
				foreach (ColumnInfo ci in V.ColumnsInfo) {
					if (string.IsNullOrEmpty (ci.TableNameOrTableAlias) || ci.TableNameOrTableAlias == d) {
						ci.TableNameOrTableAlias = d;
					}
					else {
						foreach (JoinInfo ji in this._joins) {
							if (ji.ClassTypeJoin.Name == ci.TableNameOrTableAlias || ji.ClassTypeJoin.FullName == ci.TableNameOrTableAlias || ji.ClassTypeJoinAlias == ci.TableNameOrTableAlias) {
								List<Mapper> W = MappingManager.GetMappers (ji.ClassTypeJoin, null, null);
								Mapper X = W.Find (delegate (Mapper Y) {
									return (string.Compare (Y.PropertyMapperName, ci.ColumnName, true) == 0);
								});
								if (X == null) {
									X = MappingManager.GetPropertyMapper (ji.ClassTypeJoin, ci.ColumnName);
									if (X == null)
										throw new GDAException ("Property {0} not exists in {1} or not mapped.", ci.ColumnName, ji.ClassTypeJoin.FullName);
								}
								ci.TableNameOrTableAlias = ji.ClassTypeJoinAlias;
								ci.DBColumnName = X.Name;
								ci.RenameToMapper (a);
								break;
							}
						}
						if (string.IsNullOrEmpty (ci.DBColumnName))
							throw new QueryException ("Field {0} not found in mapping.", ci.ToString ());
						continue;
					}
					Mapper _ = g.Find (delegate (Mapper Y) {
						return (string.Compare (Y.PropertyMapperName, ci.ColumnName, true) == 0);
					});
					if (_ == null) {
						_ = MappingManager.GetPropertyMapper (_returnTypeQuery, ci.ColumnName);
						if (_ == null)
							throw new GDAException ("Property {0} not exists in {1} or not mapped.", ci.ColumnName, _returnTypeQuery.FullName);
					}
					ci.DBColumnName = _.Name;
					ci.RenameToMapper (a);
				}
				ParserToSqlCommand a0 = new ParserToSqlCommand (a1, a.QuoteExpressionBegin, a.QuoteExpressionEnd);
				k.Append (" GROUP BY ").Append (a0.SqlCommand);
			}
			if (!string.IsNullOrEmpty (this.Order) && string.IsNullOrEmpty (b)) {
				Parser S = new Parser (new Lexer (this.Order));
				OrderByPart a2 = S.ExecuteOrderByPart ();
				SelectStatement V = new SelectStatement (a2);
				foreach (ColumnInfo ci in V.ColumnsInfo) {
					if (string.IsNullOrEmpty (ci.TableNameOrTableAlias) || ci.TableNameOrTableAlias == d) {
						Mapper _ = g.Find (delegate (Mapper Y) {
							return string.Compare (Y.PropertyMapperName, ci.ColumnName, true) == 0;
						});
						if (_ == null) {
							GDAOperations.CallDebugTrace (this, string.Format ("Property {0} not exists in {1} or not mapped.", ci.ColumnName, _returnTypeQuery.FullName));
							continue;
						}
						ci.DBColumnName = _.Name;
						ci.RenameToMapper (a);
					}
					else {
						foreach (JoinInfo ji in this._joins) {
							if (ji.ClassTypeJoin.Name == ci.TableNameOrTableAlias || ji.ClassTypeJoin.FullName == ci.TableNameOrTableAlias || ji.ClassTypeJoinAlias == ci.TableNameOrTableAlias) {
								List<Mapper> W = MappingManager.GetMappers (ji.ClassTypeJoin, null, null);
								Mapper X = W.Find (delegate (Mapper Y) {
									return (string.Compare (Y.PropertyMapperName, ci.ColumnName, true) == 0);
								});
								if (X == null) {
									X = MappingManager.GetPropertyMapper (ji.ClassTypeJoin, ci.ColumnName);
									if (X == null) {
										GDAOperations.CallDebugTrace (this, string.Format ("Property {0} not exists in {1} or not mapped.", ci.ColumnName, ji.ClassTypeJoin.FullName));
										continue;
									}
								}
								ci.TableNameOrTableAlias = ji.ClassTypeJoinAlias;
								ci.DBColumnName = X.Name;
								ci.RenameToMapper (a);
								break;
							}
						}
					}
					if (string.IsNullOrEmpty (ci.DBColumnName))
						GDAOperations.CallDebugTrace (this, string.Format ("Field {0} not found in mapping.", ci.ToString ()));
				}
				ParserToSqlCommand a0 = new ParserToSqlCommand (a2, a.QuoteExpressionBegin, a.QuoteExpressionEnd);
				k.Append (" ORDER BY ").Append (a0.SqlCommand);
			}
			return new QueryReturnInfo (k.ToString (), this.Parameters, f);
		}
		public override bool Equals (object a)
		{
			if (a is Query)
				return Equals ((Query)a);
			else
				return false;
		}
		public bool Equals (Query a)
		{
			return Where == null ? a.Where == null : Where.Equals (a.Where) && orderClause == null ? a.orderClause == null : orderClause.Equals (a.orderClause);
		}
		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}
		public Query Add (DbType a, object b)
		{
			return Add ("", a, b);
		}
		public Query Add (string a, DbType b, object c)
		{
			return Add (a, b, 0, c);
		}
		public Query Add (string a, object b)
		{
			return Add (new GDAParameter (a, b));
		}
		public Query Add (DbType a, int b, object c)
		{
			return Add ("", a, b, c);
		}
		public Query Add (string a, DbType b, int c, object d)
		{
			GDAParameter e = new GDAParameter ();
			e.ParameterName = a;
			e.DbType = b;
			e.Size = c;
			e.Value = d;
			var f = this._parameters.FindIndex (g => g.ParameterName == e.ParameterName);
			if (f >= 0)
				this._parameters.RemoveAt (f);
			this._parameters.Add (e);
			return this;
		}
		public long Count<T> () where T : new()
		{
			_returnTypeQuery = typeof(T);
			return GDAOperations.GetDAO<T> ().Count (this);
		}
		public long Count<T> (GDASession a) where T : new()
		{
			_returnTypeQuery = typeof(T);
			return GDAOperations.GetDAO<T> ().Count (a, this);
		}
		public double Sum<T> (string a) where T : new()
		{
			_aggregationFunctionProperty = a;
			_returnTypeQuery = typeof(T);
			return GDAOperations.GetDAO<T> ().Sum (null, this);
		}
		public double Sum<T> (GDASession a, string b) where T : new()
		{
			_aggregationFunctionProperty = b;
			_returnTypeQuery = typeof(T);
			return GDAOperations.GetDAO<T> ().Sum (a, this);
		}
		public double Max<T> (string a) where T : new()
		{
			_aggregationFunctionProperty = a;
			_returnTypeQuery = typeof(T);
			return GDAOperations.GetDAO<T> ().Max (null, this);
		}
		public double Max<T> (GDASession a, string b) where T : new()
		{
			_aggregationFunctionProperty = b;
			_returnTypeQuery = typeof(T);
			return GDAOperations.GetDAO<T> ().Max (a, this);
		}
		public double Min<T> (string a) where T : new()
		{
			_aggregationFunctionProperty = a;
			_returnTypeQuery = typeof(T);
			return GDAOperations.GetDAO<T> ().Min (null, this);
		}
		public double Min<T> (GDASession a, string b) where T : new()
		{
			_aggregationFunctionProperty = b;
			_returnTypeQuery = typeof(T);
			return GDAOperations.GetDAO<T> ().Min (a, this);
		}
		public double Avg<T> (string a) where T : new()
		{
			_aggregationFunctionProperty = a;
			_returnTypeQuery = typeof(T);
			return GDAOperations.GetDAO<T> ().Avg (null, this);
		}
		public double Avg<T> (GDASession a, string b) where T : new()
		{
			_aggregationFunctionProperty = b;
			_returnTypeQuery = typeof(T);
			return GDAOperations.GetDAO<T> ().Avg (a, this);
		}
		void IGDAParameterContainer.Add (GDAParameter parameter)
		{
			var index = this._parameters.FindIndex (f => f.ParameterName == parameter.ParameterName);
			if (index >= 0)
				this._parameters.RemoveAt (index);
			this._parameters.Add (parameter);
		}
		bool IGDAParameterContainer.TryGet (string a, out GDAParameter b)
		{
			return _parameters.TryGet (a, out b);
		}
		bool IGDAParameterContainer.ContainsKey (string a)
		{
			return _parameters.ContainsKey (a);
		}
		bool IGDAParameterContainer.Remove (string a)
		{
			return _parameters.Remove (a);
		}
		IEnumerator<GDAParameter> IEnumerable<GDAParameter>.GetEnumerator ()
		{
			return _parameters.GetEnumerator ();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return _parameters.GetEnumerator ();
		}
	}
}
