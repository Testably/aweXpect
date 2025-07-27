using System.Reflection;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	private static MethodInfo? ExpectedMethodInfo()
		=> typeof(SomeClassToVerifyTheMethodNameOfIt)
			.GetMethod(nameof(SomeClassToVerifyTheMethodNameOfIt.SomeMethodToVerifyTheNameOfIt));

	private class SomeClassToVerifyTheMethodNameOfIt
	{
#pragma warning disable CA1822
		public void SomeMethodToVerifyTheNameOfIt()
		{
		}
#pragma warning restore CA1822
	}
}
