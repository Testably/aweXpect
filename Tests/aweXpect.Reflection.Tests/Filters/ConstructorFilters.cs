using System.Linq;
using System.Reflection;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	private static ConstructorInfo? ExpectedConstructorInfo()
		=> typeof(SomeClassToVerifyTheConstructorNameOfIt)
			.GetConstructors(
				BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
			.Single();

	private class SomeClassToVerifyTheConstructorNameOfIt
	{
		private SomeClassToVerifyTheConstructorNameOfIt()
		{
		}
	}
}
