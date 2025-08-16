using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WithParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task GetDescription_ShouldReturnCorrectDescription()
			{
				Filtered.Methods methods = In.Type<TestClassWithMethodParameters>()
					.Methods().WithParameter<string>();

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type string in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterAtIndex_ShouldFilterForMethodsWithParameterAtSpecificIndex()
			{
				Filtered.Methods methods = In.Type<TestClassWithMethodParameters>()
					.Methods().WithParameter<int>().AtIndex(0);

				await That(methods).IsEqualTo([
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method2),
						[typeof(int), typeof(string),])!,
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method3),
						[typeof(int),])!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int at index 0 in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterAtIndex_WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				void Act() => In.Type<TestClassWithMethodParameters>()
					.Methods().WithParameter<int>().AtIndex(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("index").And
					.WithMessage("The index must be greater than or equal to 0.").AsPrefix();
			}

			[Fact]
			public async Task WithParameterAtIndexFromEnd_ShouldFilterForMethodsWithParameterAtSpecificIndex()
			{
				Filtered.Methods methods = In.Type<TestClassWithMethodParameters>()
					.Methods().WithParameter<int>().AtIndex(0).FromEnd();

				await That(methods).IsEqualTo([
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method3),
						[typeof(int),])!,
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method4),
						[typeof(string), typeof(int),])!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int at index 0 from end in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterByName_ShouldFilterForMethodsWithParameterOfSpecificName()
			{
				Filtered.Methods methods = In.Type<TestClassWithMethodParameters>()
					.Methods().WithParameter("id");

				await That(methods).IsEqualTo([
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method2),
						[typeof(int), typeof(string),])!,
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method3),
						[typeof(int),])!,
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method4),
						[typeof(string), typeof(int),])!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter with name equal to \"id\" in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterName_ShouldSupportAsPrefix()
			{
				Filtered.Methods methods = In.Type<TestClassWithMethodParameters>()
					.Methods().WithParameter("nam").AsPrefix();

				await That(methods).IsEqualTo([
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method1),
						[typeof(string),])!,
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method2),
						[typeof(int), typeof(string),])!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter with name starting with \"nam\" in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterName_ShouldSupportAsSuffix()
			{
				Filtered.Methods methods = In.Type<TestClassWithMethodParameters>()
					.Methods().WithParameter("ame").AsSuffix();

				await That(methods).IsEqualTo([
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method1),
						[typeof(string),])!,
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method2),
						[typeof(int), typeof(string),])!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter with name ending with \"ame\" in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterName_ShouldSupportCustomComparer()
			{
				Filtered.Methods methods = In.Type<TestClassWithMethodParameters>()
					.Methods().WithParameter("nAmE").Using(new IgnoreCaseForVocalsComparer());

				await That(methods).IsEqualTo([
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method1),
						[typeof(string),])!,
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method2),
						[typeof(int), typeof(string),])!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter with name equal to \"nAmE\" using IgnoreCaseForVocalsComparer in")
					.AsPrefix();
			}

			[Fact]
			public async Task WithParameterName_ShouldSupportIgnoringCase()
			{
				Filtered.Methods methods = In.Type<TestClassWithMethodParameters>()
					.Methods().WithParameter("NAME").IgnoringCase();

				await That(methods).IsEqualTo([
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method1),
						[typeof(string),])!,
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method2),
						[typeof(int), typeof(string),])!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter with name equal to \"NAME\" ignoring case in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterName_ShouldSupportRegex()
			{
				Filtered.Methods methods = In.Type<TestClassWithMethodParameters>()
					.Methods().WithParameter("n[a-z]*e").AsRegex();

				await That(methods).IsEqualTo([
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method1),
						[typeof(string),])!,
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method2),
						[typeof(int), typeof(string),])!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter with name matching regex \"n[a-z]*e\" in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterName_ShouldSupportWildcard()
			{
				Filtered.Methods methods = In.Type<TestClassWithMethodParameters>()
					.Methods().WithParameter("n??e").AsWildcard();

				await That(methods).IsEqualTo([
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method1),
						[typeof(string),])!,
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method2),
						[typeof(int), typeof(string),])!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter with name matching \"n??e\" in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterOfType_ShouldFilterForMethodsWithParameterOfSpecificType()
			{
				Filtered.Methods methods = In.Type<TestClassWithMethodParameters>()
					.Methods().WithParameter<string>();

				await That(methods).IsEqualTo([
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method1),
						[typeof(string),])!,
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method2),
						[typeof(int), typeof(string),])!,
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method4),
						[typeof(string), typeof(int),])!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type string in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterOfTypeAndName_ShouldFilterForMethodsWithParameterOfSpecificTypeAndName()
			{
				Filtered.Methods methods = In.Type<TestClassWithMethodParameters>()
					.Methods().WithParameter<string>("name");

				await That(methods).IsEqualTo([
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method1),
						[typeof(string),])!,
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method2),
						[typeof(int), typeof(string),])!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type string and name equal to \"name\" in").AsPrefix();
			}

			[Fact]
			public async Task WithParameterWithDefaultValue_ShouldFilterForMethodsWithParameterWithDefaultValue()
			{
				Filtered.Methods methods = In.Type<TestClassWithMethodParameters>()
					.Methods().WithParameter<int>().WithDefaultValue();

				await That(methods).IsEqualTo([
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method3),
						[typeof(int),])!,
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method4),
						[typeof(string), typeof(int),])!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int and with a default value in").AsPrefix();
			}

			[Fact]
			public async Task
				WithParameterWithSpecificDefaultValue_ShouldFilterForMethodsWithParameterWithSpecificDefaultValue()
			{
				Filtered.Methods methods = In.Type<TestClassWithMethodParameters>()
					.Methods().WithParameter<int>().WithDefaultValue(42);

				await That(methods).IsEqualTo([
					typeof(TestClassWithMethodParameters).GetMethod(nameof(TestClassWithMethodParameters.Method3),
						[typeof(int),])!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int and with default value 42 in").AsPrefix();
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		private class TestClassWithMethodParameters
		{
			public void Method1() { }
			public void Method1(string name) { }
			public void Method2(int id, string name) { }
			public void Method3(int id = 42) { }
			public void Method4(string id, int value = 100) { }
		}
		// ReSharper restore UnusedParameter.Local
#pragma warning restore CA1822
	}
}
