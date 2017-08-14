using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	public interface IGeneratorKey
	{
		void GenerateKey (object a, GenerateKeyArgs b);
	}
}
