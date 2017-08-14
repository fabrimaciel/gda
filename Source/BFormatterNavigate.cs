using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace GDA.Helper.Serialization
{
	public class BFormatterNavigate
	{
		private Stream navStream;
		private int _count;
		private Type baseType;
		private int sizeCurrentItem = 0;
		private long beginStreamPosition = 0;
		private long currentStreamPosition = 0;
		private int currentPosition = -1;
		private byte[] bufferSize = new byte[sizeof(int)];
		private BFormatter.InfoCoreSupport[] coreSupports;
		private short memberAllowNullCount;
		public int Count {
			get {
				return _count;
			}
		}
		public BFormatterNavigate (Stream a, Type b)
		{
			beginStreamPosition = a.Position;
			navStream = a;
			this.baseType = b;
			Initialize ();
		}
		private void Initialize ()
		{
			navStream.Seek (beginStreamPosition, SeekOrigin.Begin);
			currentPosition = -1;
			currentStreamPosition = 0;
			sizeCurrentItem = 0;
			coreSupports = BFormatter.LoadTypeInformation (baseType, out memberAllowNullCount);
			if (navStream.Length > 0) {
				_count = BFormatter.ReadArrayLenght (navStream, int.MaxValue);
				currentStreamPosition = navStream.Position;
			}
			else
				_count = 0;
		}
		public void Reset ()
		{
			Initialize ();
		}
		public object GetItem ()
		{
			if (currentPosition < 0)
				throw new InvalidOperationException ("Item not ready.");
			navStream.Seek (currentStreamPosition, SeekOrigin.Begin);
			return BFormatter.DeserializeBase (navStream, baseType, coreSupports, memberAllowNullCount, 0, null);
		}
		public bool Read ()
		{
			if ((currentPosition + 1) >= _count || _count == 0)
				return false;
			navStream.Seek (currentStreamPosition + sizeCurrentItem, SeekOrigin.Begin);
			navStream.Read (bufferSize, 0, bufferSize.Length);
			int a = BitConverter.ToInt32 (bufferSize, 0);
			sizeCurrentItem = a;
			currentStreamPosition = navStream.Position;
			currentPosition++;
			return true;
		}
	}
}
