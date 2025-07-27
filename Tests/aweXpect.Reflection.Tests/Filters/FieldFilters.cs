using System.Reflection;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	private static FieldInfo? ExpectedFieldInfo()
		=> typeof(SomeClassToVerifyTheFieldNameOfIt)
			.GetField(nameof(SomeClassToVerifyTheFieldNameOfIt.SomeFieldToVerifyTheNameOfIt));

	private class SomeClassToVerifyTheFieldNameOfIt
	{
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
		public int? SomeFieldToVerifyTheNameOfIt;
#pragma warning restore CS0649
	}
}
