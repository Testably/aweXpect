using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WithExactParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForConstructorsWithParameterOfExactType()
			{
				Filtered.Constructors constructors = In.Type<TestClass>()
					.Constructors().WithExactParameter<DummyBase>();

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
					.Constructors().WithExactParameter<DummyBase>();
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
					.Constructors().WithExactParameter<DummyBase>("parameter");

				await That(constructors).IsEqualTo([
					typeof(TestClass).GetConstructor([typeof(DummyBase),])!,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter of exact type ").AsPrefix();
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
