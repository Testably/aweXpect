using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WithParameterExactly
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForMethodsWithParameterOfExactType()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WithParameterExactly<DummyBase>();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithDummyBase))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of exact type ").AsPrefix();
			}

			[Fact]
			public async Task ShouldNotIncludeMethodsWithParameterOfDerivedType()
			{
				Filtered.Methods exactMethods = In.Type<TestClass>()
					.Methods().WithParameterExactly<DummyBase>();
				Filtered.Methods assignableMethods = In.Type<TestClass>()
					.Methods().WithParameter<DummyBase>();

				await That(exactMethods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithDummyBase))!,
				]).InAnyOrder();
				await That(assignableMethods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithDummyBase))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithDummy))!,
				]).InAnyOrder();
			}

			[Fact]
			public async Task WithName_ShouldFilterForMethodsWithParameterOfExactTypeAndName()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WithParameterExactly<DummyBase>("parameter");

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithDummyBase))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of exact type ").AsPrefix();
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		private class TestClass
		{
			public void MethodWithDummyBase(DummyBase parameter) { }
			public void MethodWithDummy(Dummy parameter) { }
			public void MethodWithString(string parameter) { }
		}
		// ReSharper restore UnusedParameter.Local
#pragma warning restore CA1822

		private class DummyBase
		{
		}

		private class Dummy : DummyBase
		{
		}
	}
}
