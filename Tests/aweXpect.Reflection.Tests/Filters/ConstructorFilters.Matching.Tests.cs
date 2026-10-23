using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class Matching
	{
		private static ConstructorInfo? ParamsArrayConstructor()
			=> typeof(ClassWithConstructors).GetConstructor([typeof(int[]),]);

		private static ConstructorInfo? ParamsCollectionConstructor()
			=> typeof(ClassWithConstructors).GetConstructor([typeof(List<int>),]);

		private static ConstructorInfo? CountConstructor()
			=> typeof(ClassWithConstructors).GetConstructor([typeof(int),]);

		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForParametersMatchingThePredicate()
			{
				Filtered.Constructors constructors = In.Type<ClassWithConstructors>()
					.Constructors().WithParameter<int>().Matching(p => p.Name == "count");

				await That(constructors).IsEqualTo([CountConstructor(),]);
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter of type int and matching p => p.Name == \"count\" in type")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldNotChangeTheFilterItWasCalledOn()
			{
				Reflection.ConstructorFilters.ConstructorsWithParameter<int> withInt = In.Type<ClassWithConstructors>()
					.Constructors().WithParameter<int>();

				Filtered.Constructors matching = withInt.Matching(p => p.Name == "unknown");

				await That(withInt).Contains(CountConstructor())
					.Because("Matching must return a new filter instead of changing the one it was called on");
				await That(withInt.GetDescription())
					.IsEqualTo("constructors with parameter of type int in type").AsPrefix();
				await That(matching).IsEmpty();
			}

			[Test]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				void Act()
				{
					_ = In.Type<ClassWithConstructors>().Constructors().WithParameter<int>().Matching(null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WithoutParameter_ShouldExcludeConstructorsWithAMatchingParameter()
			{
				Filtered.Constructors constructors = In.Type<ClassWithConstructors>()
					.Constructors().WithoutParameter<int>().Matching(p => p.Name == "count");

				await That(constructors).DoesNotContain(CountConstructor());
				await That(constructors).Contains(ParamsArrayConstructor());
			}

			[Test]
			public async Task WithParamsParameter_ShouldFindParamsCollections()
			{
				Filtered.Constructors constructors = In.Type<ClassWithConstructors>()
					.Constructors().WithParamsParameter().Matching(p => !p.ParameterType.IsArray);

				await That(constructors).IsEqualTo([ParamsCollectionConstructor(),])
					.Because("a params parameter that is not an array is a params collection");
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with params parameter matching p => !p.ParameterType.IsArray in type")
					.AsPrefix();
			}

			[Test]
			public async Task WithParamsParameter_WithoutOptions_ShouldKeepDescription()
			{
				Filtered.Constructors constructors = In.Type<ClassWithConstructors>()
					.Constructors().WithParamsParameter();

				await That(constructors).Contains(ParamsArrayConstructor()).And.Contains(ParamsCollectionConstructor());
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with params parameter in type")
					.AsPrefix();
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class ClassWithConstructors
		{
			public ClassWithConstructors(params int[] values)
			{
			}

			public ClassWithConstructors(params List<int> values)
			{
			}

			public ClassWithConstructors(int count)
			{
			}
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
