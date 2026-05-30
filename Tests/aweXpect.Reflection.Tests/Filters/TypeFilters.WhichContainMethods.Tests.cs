using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichContainMethods
	{
		public sealed class Tests
		{
			[Fact]
			public async Task AtMost_ShouldFilterForTypesWithAtMostTheCount()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).AtMost(1);

				await That(types).IsEqualTo([typeof(TaggedClass), typeof(UntaggedClass),]).InAnyOrder();
			}

			[Fact]
			public async Task Between_ShouldFilterForTypesWithinTheRange()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).Between(1).And(2);

				await That(types).IsEqualTo([typeof(TaggedClass), typeof(TwiceTaggedClass),]).InAnyOrder();
			}

			[Fact]
			public async Task ByDefault_ShouldFilterForTypesContainingAtLeastOneMatchingMethod()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>());

				await That(types).IsEqualTo([typeof(TaggedClass), typeof(TwiceTaggedClass),]).InAnyOrder();
			}

			[Fact]
			public async Task ChainedConditions_ShouldApplyTheQuantifierOnlyToTheLastCondition()
			{
				// Methods: implicitly "at least once"; Constructors: "exactly twice".
				Filtered.Types types =
					In.Types<ClassWith1Method2Constructors, ClassWith1Method1Constructor,
							ClassWith0Methods2Constructors>()
						.WhichContainMethods(methods => methods.With<MarkerAttribute>())
						.WhichContainConstructors(constructors => constructors.With<MarkerAttribute>()).Exactly(2);

				await That(types).IsEqualTo([typeof(ClassWith1Method2Constructors),]).InAnyOrder();
			}

			[Fact]
			public async Task ChainedConditions_ShouldDescribeEachConditionWithItsOwnQuantifier()
			{
				Filtered.Types types = In.AssemblyContaining<MarkerAttribute>().Types()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>())
					.WhichContainConstructors(constructors => constructors.With<MarkerAttribute>()).Exactly(2);

				await That(types.GetDescription())
					.IsEqualTo(
						"types which contain methods with TypeFilters.WhichContainMethods.MarkerAttribute at least once which contain constructors with TypeFilters.WhichContainMethods.MarkerAttribute exactly twice ")
					.AsPrefix();
			}

			[Fact]
			public async Task Exactly_ShouldFilterForTypesWithTheExactCount()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).Exactly(2);

				await That(types).IsEqualTo([typeof(TwiceTaggedClass),]).InAnyOrder();
			}

			[Fact]
			public async Task Never_ShouldFilterForTypesWithoutAMatchingMethod()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).Never();

				await That(types).IsEqualTo([typeof(UntaggedClass),]).InAnyOrder();
			}

			[Fact]
			public async Task Quantifier_ShouldBeReflectedInTheDescription()
			{
				Filtered.Types types = In.AssemblyContaining<MarkerAttribute>().Types()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).Exactly(3);

				await That(types.GetDescription())
					.IsEqualTo(
						"types which contain methods with TypeFilters.WhichContainMethods.MarkerAttribute exactly 3 times ")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldReadNaturallyInAFailingExpectation()
			{
				async Task Act()
				{
					await That(In.Types<TaggedClass, UntaggedClass>()
							.WhichContainMethods(methods => methods.With<MarkerAttribute>()))
						.HaveName("Tests").AsSuffix();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage(
						"Expected that in types * which contain methods with TypeFilters.WhichContainMethods.MarkerAttribute at least once *all have name ending with \"Tests\"*")
					.AsWildcard();
			}

			[Fact]
			public async Task ShouldUseTheInnerFilterDescriptionWithTheDefaultQuantifier()
			{
				Filtered.Types types = In.AssemblyContaining<MarkerAttribute>().Types()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>().OrWith<OtherAttribute>());

				await That(types.GetDescription())
					.IsEqualTo(
						"types which contain methods with TypeFilters.WhichContainMethods.MarkerAttribute or with TypeFilters.WhichContainMethods.OtherAttribute at least once ")
					.AsPrefix();
			}
		}

		[AttributeUsage(AttributeTargets.All)]
		private class MarkerAttribute : Attribute
		{
		}

		[AttributeUsage(AttributeTargets.Method)]
		private class OtherAttribute : Attribute
		{
		}

		private class TaggedClass
		{
			[Marker]
			public static void Tagged()
			{
			}
		}

		private class TwiceTaggedClass
		{
			[Marker]
			public static void TaggedOne()
			{
			}

			[Marker]
			public static void TaggedTwo()
			{
			}
		}

		private class UntaggedClass
		{
			public static void Untagged()
			{
			}
		}

		private class ClassWith1Method2Constructors
		{
			[Marker]
			public ClassWith1Method2Constructors()
			{
			}

			[Marker]
			public ClassWith1Method2Constructors(int value)
			{
			}

			[Marker]
			public static void Method()
			{
			}
		}

		private class ClassWith1Method1Constructor
		{
			[Marker]
			public ClassWith1Method1Constructor()
			{
			}

			[Marker]
			public static void Method()
			{
			}
		}

		private class ClassWith0Methods2Constructors
		{
			[Marker]
			public ClassWith0Methods2Constructors()
			{
			}

			[Marker]
			public ClassWith0Methods2Constructors(int value)
			{
			}

			public static void Method()
			{
			}
		}
	}
}
