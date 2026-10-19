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

	private abstract class AbstractMethodClass
	{
		public abstract void AbstractMethod();
		public virtual void VirtualMethod() { }
	}

	private class ConcreteMethodClass
	{
		// ReSharper disable once UnusedMember.Global
		public virtual void VirtualMethod() { }
#pragma warning disable CA1822
		// ReSharper disable once UnusedMember.Local
		public void ConcreteMethod() { }
#pragma warning restore CA1822
	}

	private class SealedMethodClass : ConcreteMethodClass
	{
		public sealed override string ToString() => "SealedMethodClass";
	}
}
