using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WithParameterExactly
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForConstructorsWithParameterOfExactType()
			{
				Filtered.Constructors constructors = In.Type<TestClass>()
					.Constructors().WithParameterExactly<DummyBase>();

				await That(constructors).IsEqualTo([
					typeof(TestClass).GetConstructor([typeof(DummyBase),])!,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter of exact type ").AsPrefix();
			}

			[Fact]
			public async Task ShouldNotIncludeConstructorsWithParameterOfDerivedType()
			{
				Filtered.Constructors exactConstructors = In.Type<TestClass>()
					.Constructors().WithParameterExactly<DummyBase>();
				Filtered.Constructors assignableConstructors = In.Type<TestClass>()
					.Constructors().WithParameter<DummyBase>();

				await That(exactConstructors).IsEqualTo([
					typeof(TestClass).GetConstructor([typeof(DummyBase),])!,
				]).InAnyOrder();
				await That(assignableConstructors).IsEqualTo([
					typeof(TestClass).GetConstructor([typeof(DummyBase),])!,
					typeof(TestClass).GetConstructor([typeof(Dummy),])!,
				]).InAnyOrder();
			}

			[Fact]
			public async Task WithName_ShouldFilterForConstructorsWithParameterOfExactTypeAndName()
			{
				Filtered.Constructors constructors = In.Type<TestClass>()
					.Constructors().WithParameterExactly<DummyBase>("parameter");

				await That(constructors).IsEqualTo([
					typeof(TestClass).GetConstructor([typeof(DummyBase),])!,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.Contains("and name equal to \"parameter\"");
			}

			[Fact]
			public async Task WithType_ShouldFilterForConstructorsWithParameterOfExactType()
			{
				Filtered.Constructors constructors = In.Type<TestClass>()
					.Constructors().WithParameterExactly(typeof(DummyBase));

				await That(constructors).IsEqualTo([
					typeof(TestClass).GetConstructor([typeof(DummyBase),])!,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter of exact type ").AsPrefix();
			}

			[Fact]
			public async Task WithType_ShouldNotIncludeConstructorsWithParameterOfDerivedType()
			{
				Filtered.Constructors exactConstructors = In.Type<TestClass>()
					.Constructors().WithParameterExactly(typeof(DummyBase));
				Filtered.Constructors assignableConstructors = In.Type<TestClass>()
					.Constructors().WithParameter(typeof(DummyBase));

				await That(exactConstructors).IsEqualTo([
					typeof(TestClass).GetConstructor([typeof(DummyBase),])!,
				]).InAnyOrder();
				await That(assignableConstructors).IsEqualTo([
					typeof(TestClass).GetConstructor([typeof(DummyBase),])!,
					typeof(TestClass).GetConstructor([typeof(Dummy),])!,
				]).InAnyOrder();
			}

			[Fact]
			public async Task WithType_WithName_ShouldFilterForConstructorsWithParameterOfExactTypeAndName()
			{
				Filtered.Constructors constructors = In.Type<TestClass>()
					.Constructors().WithParameterExactly(typeof(DummyBase), "parameter");

				await That(constructors).IsEqualTo([
					typeof(TestClass).GetConstructor([typeof(DummyBase),])!,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.Contains("and name equal to \"parameter\"");
			}
		}

		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class TestClass
		{
			public TestClass(DummyBase parameter) { }
			public TestClass(Dummy parameter) { }
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local

		private class DummyBase
		{
		}

		private class Dummy : DummyBase
		{
		}
	}
}
