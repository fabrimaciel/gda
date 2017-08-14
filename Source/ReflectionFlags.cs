using System.Reflection;
namespace GDA.Common.Helper
{
	class ReflectionFlags
	{
		public static readonly BindingFlags DefaultCriteria = BindingFlags.Public | BindingFlags.NonPublic;
		public static readonly BindingFlags InstanceCriteria = DefaultCriteria | BindingFlags.Instance;
		public static readonly BindingFlags StaticCriteria = DefaultCriteria | BindingFlags.Static | BindingFlags.FlattenHierarchy;
		public static readonly BindingFlags AllCriteria = InstanceCriteria | StaticCriteria;
	}
}
