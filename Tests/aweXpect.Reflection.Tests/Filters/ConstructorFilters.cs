using System.Linq;
using System.Reflection;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	private static ConstructorInfo? ExpectedConstructorInfo()
		=> typeof(SomeClassToVerifyTheConstructorNameOfIt)
			.GetConstructors().Single();

	private class SomeClassToVerifyTheConstructorNameOfIt
	{
		public SomeClassToVerifyTheConstructorNameOfIt()
		{
			
		}
	}
}
