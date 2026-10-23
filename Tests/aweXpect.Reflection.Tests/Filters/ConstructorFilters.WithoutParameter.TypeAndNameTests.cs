using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WithoutParameter
	{
		public sealed class Tests
		{
			[Test]
			public async Task AtIndex_ShouldOnlyFilterOutConstructorsWithParameterAtIndex()
			{
				Filtered.Constructors constructors = In.Type<TestClass>()
					.Constructors().WithoutParameter<int>().AtIndex(0);

				await That(constructors).IsEqualTo([
					ReversedConstructor,
					DefaultConstructor,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors without parameter of type int at index 0 in")
					.AsPrefix();
			}

			[Test]
			public async Task ByName_ShouldFilterOutConstructorsWithParameterName()
			{
				Filtered.Constructors constructors = In.Type<TestClass>()
					.Constructors().WithoutParameter("value");

				await That(constructors).IsEqualTo([
					RefConstructor,
					DefaultConstructor,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors without parameter with name equal to \"value\" in")
					.AsPrefix();
			}

			[Test]
			public async Task ByName_ShouldSupportAsPrefix()
			{
				Filtered.Constructors constructors = In.Type<TestClass>()
					.Constructors().WithoutParameter("coun").AsPrefix();

				await That(constructors).IsEqualTo([
					ParametersConstructor,
					ReversedConstructor,
					DefaultConstructor,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors without parameter with name starting with \"coun\" in")
					.AsPrefix();
			}

			[Test]
			public async Task ByType_ShouldFilterOutConstructorsWithParameterOfType()
			{
				Filtered.Constructors constructors = In.Type<TestClass>()
					.Constructors().WithoutParameter<string>();

				await That(constructors).IsEqualTo([
					RefConstructor,
					DefaultConstructor,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors without parameter of type string in")
					.AsPrefix();
			}

			[Test]
			public async Task ByType_WithRefModifier_ShouldOnlyFilterOutConstructorsWithRefParameter()
			{
				Filtered.Constructors constructors = In.Type<TestClass>()
					.Constructors().WithoutParameter<int>().WithRefModifier();

				await That(constructors).IsEqualTo([
					ParametersConstructor,
					ReversedConstructor,
					DefaultConstructor,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors without parameter of type int and with ref modifier in")
					.AsPrefix();
			}

			[Test]
			public async Task ByTypeAndName_ShouldFilterOutConstructorsWithMatchingParameter()
			{
				Filtered.Constructors constructors = In.Type<TestClass>()
					.Constructors().WithoutParameter<int>("count");

				await That(constructors).IsEqualTo([
					ParametersConstructor,
					ReversedConstructor,
					DefaultConstructor,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors without parameter of type int and name equal to \"count\" in")
					.AsPrefix();
			}

			[Test]
			public async Task Type_ByType_ShouldFilterOutConstructorsWithParameterOfType()
			{
				Type unexpectedType = typeof(string);
				Filtered.Constructors constructors = In.Type<TestClass>()
					.Constructors().WithoutParameter(unexpectedType);

				await That(constructors).IsEqualTo([
					RefConstructor,
					DefaultConstructor,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors without parameter of type string in")
					.AsPrefix();
			}

			[Test]
			public async Task Type_ByTypeAndName_ShouldFilterOutConstructorsWithMatchingParameter()
			{
				Type unexpectedType = typeof(int);
				Filtered.Constructors constructors = In.Type<TestClass>()
					.Constructors().WithoutParameter(unexpectedType, "count");

				await That(constructors).IsEqualTo([
					ParametersConstructor,
					ReversedConstructor,
					DefaultConstructor,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors without parameter of type int and name equal to \"count\" in")
					.AsPrefix();
			}
		}

		private static ConstructorInfo ParametersConstructor
			=> typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

		private static ConstructorInfo ReversedConstructor
			=> typeof(TestClass).GetConstructor([typeof(string), typeof(int),])!;

		private static ConstructorInfo RefConstructor
			=> typeof(TestClass).GetConstructor([typeof(int).MakeByRefType(),])!;

		private static ConstructorInfo DefaultConstructor
			=> typeof(TestClass).GetConstructor(Type.EmptyTypes)!;

		private class TestClass
		{
			public TestClass(int value, string text) { }
			public TestClass(string text, int value) { }
			public TestClass(ref int count) { }
			public TestClass() { }
		}
	}
}
