using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class Matching
	{
		private static MethodInfo? ParamsArrayMethod()
			=> typeof(ClassWithParameters).GetMethod(nameof(ClassWithParameters.ParamsArray));

		private static MethodInfo? ParamsCollectionMethod()
			=> typeof(ClassWithParameters).GetMethod(nameof(ClassWithParameters.ParamsCollection));

		private static MethodInfo? CountMethod()
			=> typeof(ClassWithParameters).GetMethod(nameof(ClassWithParameters.Count));

		private static MethodInfo? IndexMethod()
			=> typeof(ClassWithParameters).GetMethod(nameof(ClassWithParameters.Index));

		public sealed class Tests
		{
			[Test]
			public async Task ShouldCombineWithOtherOptions()
			{
				Filtered.Methods methods = In.Type<ClassWithParameters>()
					.Methods().WithParameter<int>().Matching(p => p.Name == "count").WithDefaultValue();

				await That(methods).IsEmpty()
					.Because("Count has a parameter named 'count', but without a default value");
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of type int and with a default value and matching p => p.Name == \"count\" in type")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldFilterForParametersMatchingThePredicate()
			{
				Filtered.Methods methods = In.Type<ClassWithParameters>()
					.Methods().WithParameter<int>().Matching(p => p.Name == "count");

				await That(methods).IsEqualTo([CountMethod(),]);
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int and matching p => p.Name == \"count\" in type")
					.AsPrefix();
			}

			[Test]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				void Act()
				{
					_ = In.Type<ClassWithParameters>().Methods().WithParameter<int>().Matching(null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WithoutParameter_ShouldExcludeMethodsWithAMatchingParameter()
			{
				Filtered.Methods methods = In.Type<ClassWithParameters>()
					.Methods().WithoutParameter<int>().Matching(p => p.Name == "count");

				await That(methods).Contains(IndexMethod());
				await That(methods).DoesNotContain(CountMethod());
				await That(methods.GetDescription())
					.IsEqualTo("methods without parameter of type int and matching p => p.Name == \"count\" in type")
					.AsPrefix();
			}

			[Test]
			public async Task WithParamsParameter_ShouldFindParamsCollections()
			{
				Filtered.Methods methods = In.Type<ClassWithParameters>()
					.Methods().WithParamsParameter().Matching(p => !p.ParameterType.IsArray);

				await That(methods).IsEqualTo([ParamsCollectionMethod(),])
					.Because("a params parameter that is not an array is a params collection");
				await That(methods).DoesNotContain(ParamsArrayMethod());
				await That(methods.GetDescription())
					.IsEqualTo("methods with params parameter matching p => !p.ParameterType.IsArray in type")
					.AsPrefix();
			}

			[Test]
			public async Task WithParamsParameter_WithoutOptions_ShouldKeepDescription()
			{
				Filtered.Methods methods = In.Type<ClassWithParameters>()
					.Methods().WithParamsParameter();

				await That(methods).Contains(ParamsArrayMethod()).And.Contains(ParamsCollectionMethod());
				await That(methods.GetDescription())
					.IsEqualTo("methods with params parameter in type")
					.AsPrefix();
			}

			[Test]
			public async Task WithRefParameter_ShouldSupportAtIndex()
			{
				Filtered.Methods methods = In.Type<ClassWithParameters>()
					.Methods().WithRefParameter().AtIndex(0);

				await That(methods).IsEmpty()
					.Because("the only ref parameter is the second parameter of Index");
				await That(methods.GetDescription())
					.IsEqualTo("methods with ref parameter at index 0 in type")
					.AsPrefix();
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class ClassWithParameters
		{
			public void ParamsArray(params int[] values)
			{
			}

			public void ParamsCollection(params List<int> values)
			{
			}

			public void Count(int count)
			{
			}

			public void Index(int index, ref int value)
			{
			}
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
