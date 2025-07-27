using System.Reflection;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	private static PropertyInfo? ExpectedPropertyInfo()
		=> typeof(SomeClassToVerifyThePropertyNameOfIt)
			.GetProperty(nameof(SomeClassToVerifyThePropertyNameOfIt.SomePropertyToVerifyTheNameOfIt));

	private class SomeClassToVerifyThePropertyNameOfIt
	{
		public int SomePropertyToVerifyTheNameOfIt { get; set; }
	}
}
