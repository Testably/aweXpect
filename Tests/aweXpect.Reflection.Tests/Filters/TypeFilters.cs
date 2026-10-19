namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	private abstract class AbstractTestClass
	{
		// ReSharper disable once UnusedMember.Global
		public abstract void AbstractMethod();
	}

	private class ConcreteTestClass
	{
		// ReSharper disable once UnusedMember.Global
		public virtual void VirtualMethod() { }
	}

	private sealed class SealedTestClass
	{
#pragma warning disable CA1822
		// ReSharper disable once UnusedMember.Local
		public void Method() { }
#pragma warning restore CA1822
	}
}
