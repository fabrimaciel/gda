using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Reflection;
using GDA.Interfaces;
using GDA.Collections;
namespace GDA
{
	[Serializable]
	public abstract class Persistent
	{
		public Persistent ()
		{
		}
		public uint Insert (string a, DirectionPropertiesName b)
		{
			return Insert (null, a, b);
		}
		public uint Insert (GDASession a, string b, DirectionPropertiesName c)
		{
			return GDA.GDAOperations.Insert (a, this, b, c);
		}
		public uint Insert (string a)
		{
			return Insert (a, DirectionPropertiesName.Inclusion);
		}
		public uint Insert (GDASession a, string b)
		{
			return Insert (a, b, DirectionPropertiesName.Inclusion);
		}
		public uint Insert ()
		{
			return Insert (null, null);
		}
		public uint Insert (GDASession a)
		{
			return Insert (a, null);
		}
		public virtual int Update (GDASession a, string b, DirectionPropertiesName c)
		{
			return GDAOperations.Update (a, this, b, c);
		}
		public virtual int Update (string a, DirectionPropertiesName b)
		{
			return GDAOperations.Update (null, this, a, b);
		}
		public int Update (GDASession a, string b)
		{
			return GDAOperations.Update (a, this, b, DirectionPropertiesName.Inclusion);
		}
		public int Update (string a)
		{
			return GDAOperations.Update (this, a, DirectionPropertiesName.Inclusion);
		}
		public int Update (GDASession a)
		{
			return GDAOperations.Update (a, this, null);
		}
		public int Update ()
		{
			return GDAOperations.Update (this);
		}
		public int Delete (GDASession a)
		{
			return GDAOperations.Delete (a, this);
		}
		public int Delete ()
		{
			return GDAOperations.Delete (this);
		}
		public uint Save ()
		{
			return GDAOperations.Save (this);
		}
		public uint Save (GDASession a)
		{
			return GDAOperations.Save (a, this);
		}
		public void RecoverData ()
		{
			GDAOperations.RecoverData (this);
		}
		public void RecoverData (GDASession a)
		{
			GDAOperations.RecoverData (a, this);
		}
		public ISimpleBaseDAO GetDAO ()
		{
			return GDAOperations.GetDAO (this);
		}
		public GDAList<ClassRelated> LoadRelationship1toN<ClassRelated> (string a, InfoSortExpression b, InfoPaging c) where ClassRelated : new()
		{
			return GDAOperations.LoadRelationship1toN<ClassRelated> (this, a, b, c);
		}
		public GDAList<ClassRelated> LoadRelationship1toN<ClassRelated> (InfoSortExpression a, InfoPaging b) where ClassRelated : new()
		{
			return GDAOperations.LoadRelationship1toN<ClassRelated> (this, null, a, b);
		}
		public GDAList<ClassRelated> LoadRelationship1toN<ClassRelated> (string a, InfoSortExpression b) where ClassRelated : new()
		{
			return GDAOperations.LoadRelationship1toN<ClassRelated> (this, a, b, null);
		}
		public GDAList<ClassRelated> LoadRelationship1toN<ClassRelated> (InfoSortExpression a) where ClassRelated : new()
		{
			return GDAOperations.LoadRelationship1toN<ClassRelated> (this, null, a, null);
		}
		public GDAList<ClassRelated> LoadRelationship1toN<ClassRelated> (string a, InfoPaging b) where ClassRelated : new()
		{
			return GDAOperations.LoadRelationship1toN<ClassRelated> (this, a, null, b);
		}
		public GDAList<ClassRelated> LoadRelationship1toN<ClassRelated> (InfoPaging a) where ClassRelated : new()
		{
			return GDAOperations.LoadRelationship1toN<ClassRelated> (this, null, null, a);
		}
		public GDAList<ClassRelated> LoadRelationship1toN<ClassRelated> (string a) where ClassRelated : new()
		{
			return GDAOperations.LoadRelationship1toN<ClassRelated> (this, a, null, null);
		}
		public GDAList<ClassRelated> LoadRelationship1toN<ClassRelated> () where ClassRelated : new()
		{
			return GDAOperations.LoadRelationship1toN<ClassRelated> (this, null, null, null);
		}
		public ClassRelated LoadRelationship1to1<ClassRelated> (string a) where ClassRelated : new()
		{
			GDAList<ClassRelated> b = GDAOperations.LoadRelationship1toN<ClassRelated> (this, a);
			if (b.Count > 1)
				throw new GDAException ("There is more one row found for this relationship.");
			else if (b.Count == 1)
				return b [0];
			else
				return default(ClassRelated);
		}
		public ClassRelated LoadRelationship1to1<ClassRelated> (Type a) where ClassRelated : new()
		{
			return GDAOperations.LoadRelationship1to1<ClassRelated> (this, (string)null);
		}
		public int CountRowRelationship1toN<ClassRelated> (string a) where ClassRelated : new()
		{
			return GDAOperations.CountRowRelationship1toN<ClassRelated> (this, a);
		}
		public int CountRowRelationship1toN<ClassRelated> () where ClassRelated : new()
		{
			return GDAOperations.CountRowRelationship1toN<ClassRelated> (this, null);
		}
		public string GetTableName ()
		{
			return GDAOperations.GetTableName (this.GetType ());
		}
	}
}
