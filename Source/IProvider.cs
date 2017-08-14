using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace GDA.Interfaces
{
	public interface IProvider
	{
		bool ExecuteCommandsOneAtATime {
			get;
		}
		string SqlQueryReturnIdentity {
			get;
		}
		bool GenerateIdentity {
			get;
		}
		[Obsolete ("Use GetIdentitySelect with GDA.Sql.TableName")]
		string GetIdentitySelect (string a, string b);
		string GetIdentitySelect (GDA.Sql.TableName a, string b);
		string ParameterPrefix {
			get;
		}
		string ParameterSuffix {
			get;
		}
		string Name {
			get;
		}
		bool SupportSQLCommandLimit {
			get;
		}
		long GetDbType (Type a);
		long GetDbType (string a, bool b);
		Type GetSystemType (long a);
		DateTime MinimumSupportedDateTime {
			get;
		}
		DateTime MaximumSupportedDateTime {
			get;
		}
		string StatementTerminator {
			get;
		}
		bool IsReservedWord (string a);
		char QuoteCharacter {
			get;
		}
		GDA.Provider.Capability Capabilities {
			get;
		}
		System.Collections.Generic.List<string> ReservedsWords {
			get;
		}
		string QuoteExpression (string a);
		string QuoteExpressionBegin {
			get;
		}
		string QuoteExpressionEnd {
			get;
		}
		string BuildTableName (GDA.Sql.TableName a);
		IDbConnection CreateConnection ();
		IDbCommand CreateCommand ();
		IDbDataAdapter CreateDataAdapter ();
		System.Data.Common.DbParameter CreateParameter ();
		string SQLCommandLimit (List<Mapper> a, string b, int c, int d);
		void SetParameterValue (IDbDataParameter a, object b);
	}
}
