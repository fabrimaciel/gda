using System;
namespace GDA.Provider
{
	[Flags]
	public enum Capability
	{
		BatchQuery = 1,
		Paging = 2,
		NamedParameters = 4
	}
}
