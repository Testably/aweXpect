using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WithParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task GetDescription_ShouldReturnCorrectDescription()
			{
				Filtered.Constructors constructors = In.Type<TestClassWithConstructorParameters>()
					.Constructors().WithParameter<string>();

				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter of type string in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterAtIndex_ShouldFilterForConstructorsWithParameterAtSpecificIndex()
			{
				Filtered.Constructors constructors = In.Type<TestClassWithConstructorParameters>()
					.Constructors().WithParameter<int>().AtIndex(0);

				await That(constructors).IsEqualTo([
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(int),])!,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter of type int at index 0 in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterAtIndex_WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				void Act() => In.Type<TestClassWithConstructorParameters>()
					.Constructors().WithParameter<int>().AtIndex(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("index").And
					.WithMessage("The index must be greater than or equal to 0.").AsPrefix();
			}

			[Fact]
			public async Task WithParameterAtIndexFromEnd_ShouldFilterForConstructorsWithParameterAtSpecificIndex()
			{
				Filtered.Constructors constructors = In.Type<TestClassWithConstructorParameters>()
					.Constructors().WithParameter<int>().AtIndex(0).FromEnd();

				await That(constructors).IsEqualTo([
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(int),])!,
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(string), typeof(int),])!,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter of type int at index 0 from end in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterByName_ShouldFilterForConstructorsWithParameterOfSpecificName()
			{
				Filtered.Constructors constructors = In.Type<TestClassWithConstructorParameters>()
					.Constructors().WithParameter("id");

				await That(constructors).IsEqualTo([
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(int),])!,
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(string), typeof(int),])!,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter with name equal to \"id\" in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterName_ShouldSupportAsPrefix()
			{
				Filtered.Constructors constructors = In.Type<TestClassWithConstructorParameters>()
					.Constructors().WithParameter("nam").AsPrefix();

				await That(constructors).IsEqualTo([
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(string),])!,
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(int), typeof(string),])!,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter with name starting with \"nam\" in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterName_ShouldSupportAsSuffix()
			{
				Filtered.Constructors constructors = In.Type<TestClassWithConstructorParameters>()
					.Constructors().WithParameter("ame").AsSuffix();

				await That(constructors).IsEqualTo([
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(string),])!,
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(int), typeof(string),])!,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter with name ending with \"ame\" in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterName_ShouldSupportCustomComparer()
			{
				Filtered.Constructors constructors = In.Type<TestClassWithConstructorParameters>()
					.Constructors().WithParameter("nAmE").Using(new IgnoreCaseForVocalsComparer());

				await That(constructors).IsEqualTo([
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(string),])!,
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(int), typeof(string),])!,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo(
						"constructors with parameter with name equal to \"nAmE\" using IgnoreCaseForVocalsComparer in")
					.AsPrefix();
			}

			[Fact]
			public async Task WithParameterName_ShouldSupportIgnoringCase()
			{
				Filtered.Constructors constructors = In.Type<TestClassWithConstructorParameters>()
					.Constructors().WithParameter("NAME").IgnoringCase();

				await That(constructors).IsEqualTo([
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(string),])!,
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(int), typeof(string),])!,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter with name equal to \"NAME\" ignoring case in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterName_ShouldSupportRegex()
			{
				Filtered.Constructors constructors = In.Type<TestClassWithConstructorParameters>()
					.Constructors().WithParameter("n[a-z]*e").AsRegex();

				await That(constructors).IsEqualTo([
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(string),])!,
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(int), typeof(string),])!,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter with name matching regex \"n[a-z]*e\" in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterName_ShouldSupportWildcard()
			{
				Filtered.Constructors constructors = In.Type<TestClassWithConstructorParameters>()
					.Constructors().WithParameter("n??e").AsWildcard();

				await That(constructors).IsEqualTo([
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(string),])!,
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(int), typeof(string),])!,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter with name matching \"n??e\" in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterOfType_ShouldFilterForConstructorsWithParameterOfSpecificType()
			{
				Filtered.Constructors constructors = In.Type<TestClassWithConstructorParameters>()
					.Constructors().WithParameter<string>();

				await That(constructors).IsEqualTo([
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(string),])!,
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(string), typeof(int),])!,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter of type string in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterOfTypeAndName_ShouldFilterForConstructorsWithParameterOfSpecificTypeAndName()
			{
				Filtered.Constructors constructors = In.Type<TestClassWithConstructorParameters>()
					.Constructors().WithParameter<string>("name");

				await That(constructors).IsEqualTo([
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(string),])!,
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(int), typeof(string),])!,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter of type string and name equal to \"name\" in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterWithDefaultValue_ShouldFilterForConstructorsWithParameterWithDefaultValue()
			{
				Filtered.Constructors constructors = In.Type<TestClassWithConstructorParameters>()
					.Constructors().WithParameter<int>().WithDefaultValue();

				await That(constructors).IsEqualTo([
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(int),])!,
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(string), typeof(int),])!,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter of type int and with a default value in").AsPrefix();
			}

			[Fact]
			public async Task
				WithParameterWithSpecificDefaultValue_ShouldFilterForConstructorsWithParameterWithSpecificDefaultValue()
			{
				Filtered.Constructors constructors = In.Type<TestClassWithConstructorParameters>()
					.Constructors().WithParameter<int>().WithDefaultValue(42);

				await That(constructors).IsEqualTo([
					typeof(TestClassWithConstructorParameters).GetConstructor([typeof(int),])!,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter of type int and with default value 42 in").AsPrefix();
			}
		}

		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class TestClassWithConstructorParameters
		{
			public TestClassWithConstructorParameters() { }
			public TestClassWithConstructorParameters(string name) { }
			public TestClassWithConstructorParameters(int id, string name) { }
			public TestClassWithConstructorParameters(int id = 42) { }
			public TestClassWithConstructorParameters(string id, int value = 100) { }
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
