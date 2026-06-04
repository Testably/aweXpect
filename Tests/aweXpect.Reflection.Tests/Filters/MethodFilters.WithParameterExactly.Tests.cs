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
			public async Task UsingType_ShouldFilterForMethodsWithParameterOfExactType()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WithParameterExactly(typeof(DummyBase));

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithDummyBase))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of exact type ").AsPrefix();
			}

			[Fact]
			public async Task UsingType_ShouldNotIncludeMethodsWithParameterOfDerivedType()
			{
				Filtered.Methods exactMethods = In.Type<TestClass>()
					.Methods().WithParameterExactly(typeof(DummyBase));
				Filtered.Methods assignableMethods = In.Type<TestClass>()
					.Methods().WithParameter(typeof(DummyBase));

				await That(exactMethods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithDummyBase))!,
				]).InAnyOrder();
				await That(assignableMethods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithDummyBase))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithDummy))!,
				]).InAnyOrder();
			}

			[Fact]
			public async Task UsingType_WithName_ShouldFilterForMethodsWithParameterOfExactTypeAndName()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WithParameterExactly(typeof(DummyBase), "parameter");

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithDummyBase))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of exact type ").AsPrefix();
			}

#pragma warning disable CA2263
			[Fact]
			public async Task UsingType_WithName_ShouldIncludeNameInDescription()
			{
				Filtered.Methods methods = In.Type<TestClassWithIntParameters>()
					.Methods().WithParameterExactly(typeof(int), "value");

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of exact type int and name equal to \"value\" in").AsPrefix();
			}
#pragma warning restore CA2263

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

			[Fact]
			public async Task WithName_ShouldIncludeNameInDescription()
			{
				Filtered.Methods methods = In.Type<TestClassWithIntParameters>()
					.Methods().WithParameterExactly<int>("value");

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of exact type int and name equal to \"value\" in").AsPrefix();
			}
		}

		private class DummyBase
		{
		}

		private class Dummy : DummyBase
		{
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		private class TestClass
		{
			public void MethodWithDummyBase(DummyBase parameter) { }
			public void MethodWithDummy(Dummy parameter) { }
			public void MethodWithString(string parameter) { }
		}

		private class TestClassWithIntParameters
		{
			public void MethodWithIntValue(int value) { }
		}
		// ReSharper restore UnusedParameter.Local
#pragma warning restore CA1822
	}
}
