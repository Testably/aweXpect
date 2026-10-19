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

	private abstract class AbstractPropertyClass
	{
		public abstract string AbstractProperty { get; set; }
		public virtual string VirtualProperty { get; set; } = "";
	}

	private class ConcretePropertyClass
	{
		public virtual string VirtualProperty { get; set; } = "";
		public string ConcreteProperty { get; set; } = "";
	}

	private class SealedPropertyClass : AbstractPropertyClass
	{
		public sealed override string AbstractProperty { get; set; } = "";
		public sealed override string VirtualProperty { get; set; } = "";
	}
}
