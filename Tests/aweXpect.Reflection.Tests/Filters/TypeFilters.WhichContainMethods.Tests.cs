using aweXpect.Reflection.Collections;

using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichContainMethods
	{
		public sealed class Tests
		{
			[Test]
			public async Task AtLeastFluent_ShouldFilterForTypesWithAtLeastTheCount()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).AtLeast().Twice();

				await That(types).IsEqualTo([typeof(TwiceTaggedClass),]).InAnyOrder();
			}

			[Test]
			public async Task AtLeastTimes_ShouldFilterForTypesWithAtLeastTheCount()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).AtLeast(2);

				await That(types).IsEqualTo([typeof(TwiceTaggedClass),]).InAnyOrder();
			}

			[Test]
			public async Task AtMost_ShouldFilterForTypesWithAtMostTheCount()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).AtMost(1);

				await That(types).IsEqualTo([typeof(TaggedClass), typeof(UntaggedClass),]).InAnyOrder();
			}

			[Test]
			public async Task AtMostFluent_ShouldFilterForTypesWithAtMostTheCount()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).AtMost().Once();

				await That(types).IsEqualTo([typeof(TaggedClass), typeof(UntaggedClass),]).InAnyOrder();
			}

			[Test]
			public async Task Between_ShouldExcludeTypesOutsideTheRange()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).Between(2).And(2);

				await That(types).IsEqualTo([typeof(TwiceTaggedClass),]).InAnyOrder();
			}

			[Test]
			public async Task Between_ShouldFilterForTypesWithinTheRange()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).Between(1).And(2);

				await That(types).IsEqualTo([typeof(TaggedClass), typeof(TwiceTaggedClass),]).InAnyOrder();
			}

			[Test]
			public async Task ByDefault_ShouldFilterForTypesContainingAtLeastOneMatchingMethod()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>());

				await That(types).IsEqualTo([typeof(TaggedClass), typeof(TwiceTaggedClass),]).InAnyOrder();
			}

			[Test]
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

			[Test]
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

			[Test]
			public async Task CompetingQuantifiers_ShouldThrowInvalidOperationException()
			{
				void Act()
				{
					In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
						.WhichContainMethods(methods => methods.With<MarkerAttribute>()).Once().Twice();
				}

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Twice cannot be combined with Once.");
			}

			[Test]
			public async Task Exactly_ShouldFilterForTypesWithTheExactCount()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).Exactly(2);

				await That(types).IsEqualTo([typeof(TwiceTaggedClass),]).InAnyOrder();
			}

			[Test]
			public async Task LessThanFluent_ShouldFilterForTypesWithLessThanTheCount()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).LessThan().Twice();

				await That(types).IsEqualTo([typeof(TaggedClass), typeof(UntaggedClass),]).InAnyOrder();
			}

			[Test]
			public async Task LessThanTimes_ShouldFilterForTypesWithLessThanTheCount()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).LessThan(2);

				await That(types).IsEqualTo([typeof(TaggedClass), typeof(UntaggedClass),]).InAnyOrder();
			}

			[Test]
			public async Task MoreThanFluent_ShouldFilterForTypesWithMoreThanTheCount()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).MoreThan().Once();

				await That(types).IsEqualTo([typeof(TwiceTaggedClass),]).InAnyOrder();
			}

			[Test]
			public async Task MoreThanTimes_ShouldFilterForTypesWithMoreThanTheCount()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).MoreThan(1);

				await That(types).IsEqualTo([typeof(TwiceTaggedClass),]).InAnyOrder();
			}

			[Test]
			public async Task Never_ShouldFilterForTypesWithoutAMatchingMethod()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).Never();

				await That(types).IsEqualTo([typeof(UntaggedClass),]).InAnyOrder();
			}

			[Test]
			public async Task Once_ShouldFilterForTypesWithExactlyOneMatch()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).Once();

				await That(types).IsEqualTo([typeof(TaggedClass),]).InAnyOrder();
			}

			[Test]
			public async Task Quantifier_ShouldBeReflectedInTheDescription()
			{
				Filtered.Types types = In.AssemblyContaining<MarkerAttribute>().Types()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).Exactly(3);

				await That(types.GetDescription())
					.IsEqualTo(
						"types which contain methods with TypeFilters.WhichContainMethods.MarkerAttribute exactly 3 times ")
					.AsPrefix();
			}

			[Test]
			public async Task Quantifiers_WhenAStoredFilterIsContinuedTwice_ShouldKeepThemApart()
			{
				Reflection.TypeFilters.TypesContainingMembersScopeResult stored =
					In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
						.WhichContainMethods(methods => methods.With<MarkerAttribute>());

				Reflection.TypeFilters.TypesContainingMembersScopeResult once = stored.Once();
				Reflection.TypeFilters.TypesContainingMembersScopeResult never = stored.Never();

				await That(stored).IsEqualTo([typeof(TaggedClass), typeof(TwiceTaggedClass),]).InAnyOrder()
					.Because("a quantifier must return a new filter instead of changing the one it was called on");
				await That(once).IsEqualTo([typeof(TaggedClass),]);
				await That(once.GetDescription())
					.Contains("which contain methods with TypeFilters.WhichContainMethods.MarkerAttribute exactly once");
				await That(never).IsEqualTo([typeof(UntaggedClass),]);
				await That(never.GetDescription())
					.Contains("which contain methods with TypeFilters.WhichContainMethods.MarkerAttribute never");
			}

			[Test]
			public async Task RepeatedQuantifier_ShouldReplaceTheEarlierOne()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).AtLeast(1).AtLeast(2);

				await That(types).IsEqualTo([typeof(TwiceTaggedClass),]);
				await That(types.GetDescription())
					.Contains("which contain methods with TypeFilters.WhichContainMethods.MarkerAttribute at least twice");
			}

			[Test]
			public async Task ShouldIncludeTypesWithInheritedMatchingMethod()
			{
				Filtered.Types types = In.Type<DerivedClassWithInheritedMarkedMethod>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>());

				await That(types).IsEqualTo([typeof(DerivedClassWithInheritedMarkedMethod),]).InAnyOrder();
			}

			[Test]
			public async Task ShouldReadNaturallyInAFailingExpectation()
			{
				async Task Act()
				{
					await That(In.Types<TaggedClass, UntaggedClass>()
							.WhichContainMethods(methods => methods.With<MarkerAttribute>()))
						.HaveName("Tests").AsSuffix();
				}

				await That(Act).Throws<FailException>()
					.WithMessage(
						"Expected that in types * which contain methods with TypeFilters.WhichContainMethods.MarkerAttribute at least once*all have name ending with \"Tests\"*")
					.AsWildcard();
			}

			[Test]
			public async Task ShouldStripTrailingInFromTheInnerMembersDescription()
			{
				Filtered.Types types = In.AssemblyContaining<MarkerAttribute>().Types()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>());

				// The inner probe description ("methods with …MarkerAttribute in ") has its
				// trailing "in " removed before being embedded into the type description, so the
				// members fragment reads exactly "…MarkerAttribute at least once " with no chopped
				// characters around "MarkerAttribute".
				await That(types.GetDescription())
					.IsEqualTo(
						"*which contain methods with TypeFilters.WhichContainMethods.MarkerAttribute at least once *")
					.AsWildcard();
			}

			[Test]
			public async Task ShouldUseTheInnerFilterDescriptionWithTheDefaultQuantifier()
			{
				Filtered.Types types = In.AssemblyContaining<MarkerAttribute>().Types()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>().OrWith<OtherAttribute>());

				await That(types.GetDescription())
					.IsEqualTo(
						"types which contain methods with TypeFilters.WhichContainMethods.MarkerAttribute or with TypeFilters.WhichContainMethods.OtherAttribute at least once ")
					.AsPrefix();
			}

			[Test]
			public async Task Twice_ShouldFilterForTypesWithExactlyTwoMatches()
			{
				Filtered.Types types = In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).Twice();

				await That(types).IsEqualTo([typeof(TwiceTaggedClass),]).InAnyOrder();
			}

			[Test]
			public async Task Twice_ShouldNotChangeTheFilterItWasCalledOn()
			{
				Reflection.TypeFilters.TypesContainingMembers containing =
					In.Types<TaggedClass, TwiceTaggedClass, UntaggedClass>()
						.WhichContainMethods(methods => methods.With<MarkerAttribute>());

				Filtered.Types twice = containing.Twice();

				await That(containing).IsEqualTo([typeof(TaggedClass), typeof(TwiceTaggedClass),]).InAnyOrder()
					.Because("Twice must return a new filter instead of changing the one it was called on");
				await That(containing.GetDescription())
					.Contains("which contain methods with TypeFilters.WhichContainMethods.MarkerAttribute at least once");
				await That(twice).IsEqualTo([typeof(TwiceTaggedClass),]).InAnyOrder();
			}

			[Test]
			public async Task WhenDirectly_AfterAQuantifier_ShouldKeepTheQuantifier()
			{
				Reflection.TypeFilters.TypesContainingMembers types = In.Type<DerivedClassWithInheritedMarkedMethod>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).Never().Directly();

				await That(types).IsEqualTo([typeof(DerivedClassWithInheritedMarkedMethod),]);
				await That(types.GetDescription())
					.Contains("which declare methods with TypeFilters.WhichContainMethods.MarkerAttribute never");
			}

			[Test]
			public async Task WhenDirectly_ShouldApplyToTheQuantifier()
			{
				Filtered.Types types = In.Type<DerivedClassWithInheritedMarkedMethod>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).Directly().Never();

				await That(types).IsEqualTo([typeof(DerivedClassWithInheritedMarkedMethod),]);
			}

			[Test]
			public async Task WhenDirectly_ShouldDescribeTheScope()
			{
				Filtered.Types types = In.AssemblyContaining<MarkerAttribute>().Types()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).Directly();

				await That(types.GetDescription())
					.IsEqualTo(
						"types which declare methods with TypeFilters.WhichContainMethods.MarkerAttribute at least once ")
					.AsPrefix();
			}

			[Test]
			public async Task WhenDirectly_ShouldNotIncludeTypesWithOnlyInheritedMatchingMethod()
			{
				Filtered.Types types = In.Type<DerivedClassWithInheritedMarkedMethod>()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).Directly();

				await That(types).IsEmpty();
			}

			[Test]
			public async Task WhenIncludingObjectMembers_AfterAQuantifier_ShouldKeepTheQuantifier()
			{
				Reflection.TypeFilters.TypesContainingMembers types = In.Type<StructWithMembers>()
					.WhichContainMethods(methods => methods.WithName(nameof(Equals))).Never().IncludingObjectMembers();

				await That(types).IsEmpty();
				await That(types.GetDescription())
					.Contains("which contain methods with name equal to \"Equals\" including object members never");
			}

			[Test]
			public async Task WhenIncludingObjectMembers_ShouldDescribeTheScope()
			{
				Filtered.Types types = In.AssemblyContaining<MarkerAttribute>().Types()
					.WhichContainMethods(methods => methods.With<MarkerAttribute>()).IncludingObjectMembers();

				await That(types.GetDescription())
					.IsEqualTo(
						"types which contain methods with TypeFilters.WhichContainMethods.MarkerAttribute including object members at least once ")
					.AsPrefix();
			}

			[Test]
			public async Task WhenIncludingObjectMembers_ShouldIncludeTypesWithMatchingObjectMethod()
			{
				Filtered.Types withoutObjectMembers = In.Type<StructWithMembers>()
					.WhichContainMethods(methods => methods.WithName(nameof(Equals)));
				Filtered.Types withObjectMembers = In.Type<StructWithMembers>()
					.WhichContainMethods(methods => methods.WithName(nameof(Equals))).IncludingObjectMembers();

				await That(withoutObjectMembers).IsEmpty()
					.Because("the members declared on System.Object and System.ValueType are excluded by default");
				await That(withObjectMembers).IsEqualTo([typeof(StructWithMembers),]);
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

		private class BaseClassWithMarkedMethod
		{
#pragma warning disable CA1822 // Mark members as static
			[Marker]
			public void Inherited()
			{
			}
#pragma warning restore CA1822
		}

		private class DerivedClassWithInheritedMarkedMethod : BaseClassWithMarkedMethod
		{
		}
	}
}
