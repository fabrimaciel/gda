using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
namespace GDA
{
	[Serializable]
	#if !PocketPC
	public class GDAException : Exception, System.Runtime.Serialization.ISerializable
	#else
		    public class GDAException : Exception
#endif
	{
		public GDAException (string a) : base (a)
		{
		}
		public GDAException (Exception a) : base (a.Message, a)
		{
		}
		public GDAException (string a, Exception b) : base (a, b)
		{
		}
		public GDAException (string a, params object[] b) : this (String.Format (a, b))
		{
		}
		#if !PocketPC
		protected GDAException (System.Runtime.Serialization.SerializationInfo a, System.Runtime.Serialization.StreamingContext b) : base (a, b)
		{
		}
		public override void GetObjectData (System.Runtime.Serialization.SerializationInfo a, System.Runtime.Serialization.StreamingContext b)
		{
			base.GetObjectData (a, b);
		}
	#endif
	}
	[Serializable]
	public class GDAColumnNotFoundException : Exception
	{
		public GDAColumnNotFoundException (string a, string b) : base ("Column " + a + " not found in result. " + b)
		{
		}
	}
	[Serializable]
	public class GDAReferenceDAONotFoundException : GDAException
	{
		public GDAReferenceDAONotFoundException (string a) : base (a)
		{
		}
	}
	[Serializable]
	public class ItemNotFoundException : GDAException
	{
		public ItemNotFoundException (string a) : base (a)
		{
		}
	}
}
