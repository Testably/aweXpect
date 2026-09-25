using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using aweXpect.Core.Metadata;
using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Equivalency;

public sealed class EquivalencyComparisonTests
{
	[Fact]
	public async Task WhenActualImplementsAPropertyExplicitly_AndHasAPublicPropertyOfTheSameName_ShouldCompareThePublicOne()
	{
		ExplicitAndPublicValue actual = new(5, 1);
		var expected = new
		{
			Value = 5,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: 1
		                                                    Expected: 5
		                                                """).IgnoringNewlineStyle()
			.Because("the explicit implementation is only a fallback for a property the actual type does not have");
	}

	[Fact]
	public async Task WhenActualImplementsAPropertyExplicitly_AndItDiffers_ShouldReportTheProperty()
	{
		ExplicitValue actual = new(5, 1);
		var expected = new
		{
			Other = 1,
			Value = 6,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: 5
		                                                    Expected: 6
		                                                """).IgnoringNewlineStyle()
			.Because("the explicit implementation is reported under the short name the expectation uses");
	}

	[Fact]
	public async Task WhenActualImplementsAPropertyExplicitly_AndTheTypeIsRegistered_ShouldCompareTheRegisteredOne()
	{
		RegisterExplicitPhantom();
		RegisteredExplicitProbe actual = new(1);
		var expected = new
		{
			Phantom = 2,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Phantom differed:
		                                                       Found: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("the registered explicit implementation is not one reflection could find, and a type with only explicit implementations still counts as registered");
	}

	[Fact]
	public async Task WhenActualImplementsAPropertyExplicitly_ForAGenericInterface_ShouldCompareIt()
	{
		ExplicitGenericValue actual = new("foo");
		var expected = new
		{
			Value = "foo",
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), new StringBuilder());

		await That(result).IsTrue()
			.Because("the name of the implementation contains the type arguments of the interface, but still ends with the short name");
	}

	[Fact]
	public async Task WhenActualImplementsAPropertyExplicitly_ForTwoInterfaces_AndTheTypeIsRegistered_ShouldReportItAsAmbiguous()
	{
		RegisterAmbiguousExplicitPhantom();
		RegisteredAmbiguousExplicitProbe actual = new(1);
		var expected = new
		{
			Phantom = 1,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Phantom is ambiguous on the actual object, which implements it explicitly for more than one interface
		                                                """).IgnoringNewlineStyle()
			.Because("the registry has to decide the ambiguity the same way reflection does");
	}

	[Fact]
	public async Task WhenActualImplementsAPropertyExplicitly_ForTwoInterfaces_ShouldReportItAsAmbiguous()
	{
		ExplicitValueForTwoInterfaces actual = new(1, 2);
		var expected = new
		{
			Value = 1,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value is ambiguous on the actual object, which implements it explicitly for more than one interface
		                                                """).IgnoringNewlineStyle()
			.Because("picking one of the implementations would make the result depend on the order reflection returns them in");
	}

	[Fact]
	public async Task WhenActualImplementsAPropertyExplicitly_InACollection_ShouldCompareIt()
	{
		ExplicitValue[] actual = [new(5, 1),];
		object[] expected =
		[
			new
			{
				Other = 1,
				Value = 6,
			},
		];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property [0].Value differed:
		                                                       Found: 5
		                                                    Expected: 6
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenActualImplementsAPropertyExplicitly_OnABaseType_ShouldCompareIt()
	{
		DerivedFromExplicitValue actual = new(5, 1);
		var expected = new
		{
			Other = 1,
			Value = 5,
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), new StringBuilder());

		await That(result).IsTrue()
			.Because("reflection does not return the private members of a base type, so the hierarchy has to be walked");
	}

	[Fact]
	public async Task WhenActualImplementsAPropertyExplicitly_OnANestedMember_ShouldCompareIt()
	{
		var actual = new
		{
			Inner = new ExplicitValue(5, 1),
		};
		var expected = new
		{
			Inner = new
			{
				Other = 1,
				Value = 6,
			},
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Inner.Value differed:
		                                                       Found: 5
		                                                    Expected: 6
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenActualImplementsAPropertyExplicitly_ShouldCompareIt()
	{
		ExplicitValue actual = new(5, 1);
		var expected = new
		{
			Other = 1,
			Value = 5,
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), new StringBuilder());

		await That(result).IsTrue()
			.Because("an explicitly implemented property is matched by the short name of the interface property");
	}

	[Fact]
	public async Task WhenActualImplementsAPropertyExplicitly_WithAnExpectedField_ShouldCompareIt()
	{
		ExplicitValue actual = new(5, 1);
		WithPublicValue expected = new(6);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Field Value differed:
		                                                       Found: 5
		                                                    Expected: 6
		                                                """).IgnoringNewlineStyle()
			.Because("an expected field falls back to a property of the same name, which includes an explicit implementation");
	}

	[Fact]
	public async Task WhenActualImplementsAPropertyExplicitly_WithIsNotEquivalentTo_ShouldFail()
	{
		ExplicitValue actual = new(5, 1);
		var unexpected = new
		{
			Other = 1,
			Value = 5,
		};

		async Task Act()
			=> await That(actual).IsNotEquivalentTo(unexpected);

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that actual
			             is not equivalent to {
			                 Other = 1,
			                 Value = 5
			               },
			             but it was EquivalencyComparisonTests.ExplicitValue {
			                 Other = 1
			               }, which is considered equivalent

			             Equivalency options:
			              - include public fields and properties
			             """).Because("the explicit implementation makes the actual object equivalent, even though the formatter does not list it");
	}

	[Fact]
	public async Task WhenActualImplementsAPropertyExplicitly_WithNonPublicMembers_ShouldCompareIt()
	{
		ExplicitValue actual = new(5, 1);
		var expected = new
		{
			Other = 1,
			Value = 5,
		};
		EquivalencyOptions options = new()
		{
			Properties = IncludeMembers.Public | IncludeMembers.Private,
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, options, new StringBuilder());

		await That(result).IsTrue()
			.Because("including private members makes the implementation visible under its qualified name, which still does not match the short one");
	}

	[Fact]
	public async Task WhenActualImplementsAPropertyExplicitly_WithoutProperties_ShouldTreatItAsMissing()
	{
		ExplicitValue actual = new(5, 1);
		WithPublicValue expected = new(5);
		StringBuilder failureBuilder = new();
		EquivalencyOptions options = new()
		{
			Properties = IncludeMembers.None,
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Field Value is missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("an explicit implementation is a property, so excluding properties excludes it as well");
	}

	[Fact]
	public async Task WhenActualIsComparedByMembers_AndExpectedIsAString_ShouldCompareByValue()
	{
		WithLength actual = new(2);
		string expected = "ab";
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse()
			.Because("a string is compared by value, so it must not be reduced to its Length just because the subject is compared by members");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  It differed:
		                                                       Found: EquivalencyComparisonTests.WithLength { Length = 2 }
		                                                    Expected: "ab"
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenActualIsComparedByMembers_AndItsEqualsAcceptsTheExpectedValue_ShouldLetTheExpectedValueDecide()
	{
		EqualToAnything actual = new();
		string expected = "ab";

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), new StringBuilder());

		await That(result).IsFalse()
			.Because("a type compared by members has its Equals ignored, so only the Equals of the value compared by value may decide");
	}

	[Fact]
	public async Task WhenActualMemberIsNull_ShouldReportFoundAndExpected()
	{
		var actual = new
		{
			Value = (string?)null,
		};
		var expected = new
		{
			Value = (string?)"Foo",
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: <null>
		                                                    Expected: "Foo"
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenActualMemberIsMoreVisibleThanRequested_ShouldStillCompareIt()
	{
		WithPublicValue actual = new(1);
		WithInternalValue expected = new(1);
		EquivalencyOptions options = new()
		{
			Fields = IncludeMembers.Internal,
			Properties = IncludeMembers.None,
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, options, new StringBuilder());

		await That(result).IsTrue()
			.Because("the visibility selects the members of the expected object, while the actual side only has to have a member of that name");
	}

	[Fact]
	public async Task WhenActualPropertyHasNoPublicGetter_ShouldTreatItAsMissing()
	{
		WithPrivateGetter actual = new(1);
		WithPublicGetter expected = new(1);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value is missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("a registration cannot call a non-public getter, so reflection must not read one either");
	}

	[Fact]
	public async Task WhenActualTypeHasAFieldAndAPropertyOfTheSameName_WithAnExpectedField_ShouldUseTheField()
	{
		FieldHidingProperty actual = new(99, 1);
		WithPublicValue expected = new(2);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Field Value differed:
		                                                       Found: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("the member of the same kind takes precedence, so the fallback to the property never applies");
	}

	[Fact]
	public async Task WhenActualTypeHasAFieldAndAPropertyOfTheSameName_WithAnExpectedProperty_ShouldUseTheProperty()
	{
		FieldHidingProperty actual = new(1, 99);
		WithProperty expected = new(2);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("the member of the same kind takes precedence, so the fallback to the field never applies");
	}

	[Fact]
	public async Task WhenActualTypeIsComparedByMembersExplicitly_AndExpectedIsAString_ShouldCompareByValue()
	{
		WithLength actual = new(2);
		string expected = "ab";
		EquivalencyOptions options = new EquivalencyOptions()
			.For<WithLength>(o => o with
			{
				ComparisonType = EquivalencyComparisonType.ByMembers,
			});

		bool result = await EquivalencyComparison.Compare(actual, expected, options, new StringBuilder());

		await That(result).IsFalse()
			.Because("comparing the subject by members cannot make a string equivalent to anything other than an equal string");
	}

	[Fact]
	public async Task WhenAllMembersAreExcludedExplicitly_ShouldNotThrow()
	{
		ClassWithOnlyPrivateState actual = new(1);
		ClassWithOnlyPrivateState expected = new(2);
		EquivalencyOptions options = new()
		{
			Fields = IncludeMembers.None,
			Properties = IncludeMembers.None,
		};

		async Task Act()
			=> await EquivalencyComparison.Compare(actual, expected, options, new StringBuilder());

		await That(Act).DoesNotThrow()
			.Because("excluding every member is an explicit choice by the caller");
	}

	[Fact]
	public async Task WhenAssemblyMemberDiffers_ShouldReportTheDifference()
	{
		var actual = new
		{
			Value = typeof(EquivalencyComparisonTests).Assembly,
		};
		var expected = new
		{
			Value = typeof(EquivalencyComparison).Assembly,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo($"""

		                                                   Property Value differed:
		                                                        Found: {actual.Value}
		                                                     Expected: {expected.Value}
		                                                 """).IgnoringNewlineStyle()
			.Because("an assembly only describes what it loaded, so walking it reaches getters that throw instead of state that could be compared");
	}

	[Fact]
	public async Task WhenBothMembersAreNull_ShouldSucceed()
	{
		WithNullableValue actual = new(null);
		WithNullableValue expected = new(null);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("a member that exists on both sides and is null on both sides is equivalent");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenCharArrayMemberIsComparedWithAString_ShouldReportTheDifference()
	{
		var actual = new
		{
			Value = new[]
			{
				'a',
			},
		};
		var expected = new
		{
			Value = "a",
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse()
			.Because("a string is compared by value, which a collection of its characters is not equal to");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: ['a']
		                                                    Expected: "a"
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenCollectionElementDiffers_ShouldReportTheElementIndex()
	{
		var actual = new
		{
			Values = new[]
			{
				1, 2, 3,
			},
		};
		var expected = new
		{
			Values = new[]
			{
				1, 5, 3,
			},
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element Values[1] differed:
		                                                       Found: 2
		                                                    Expected: 5
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenCollectionElementsFormatIdentically_ShouldAppendTheRuntimeType()
	{
		var actual = new
		{
			Values = new object[]
			{
				1,
			},
		};
		var expected = new
		{
			Values = new object[]
			{
				1L,
			},
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element Values[0] differed:
		                                                       Found: 1 (int)
		                                                    Expected: 1 (long)
		                                                """).IgnoringNewlineStyle()
			.Because("two elements that format identically are only told apart by their type");
	}

	[Fact]
	public async Task WhenCollectionOrderIsIgnored_AndActualHasFewerElements_ShouldReportTheMissingElement()
	{
		int[] actual = [1, 2,];
		int[] expected = [1, 2, 3,];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [2] was missing 3
		                                                """).IgnoringNewlineStyle()
			.Because("the index of a missing element is its position in the expected collection");
	}

	[Fact]
	public async Task WhenCollectionOrderIsIgnored_AndActualHasMoreElements_ShouldReportTheSuperfluousElement()
	{
		int[] actual = [1, 2, 3,];
		int[] expected = [1, 2,];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [2] had superfluous 3
		                                                """).IgnoringNewlineStyle()
			.Because("the index of a superfluous element is its position in the actual collection");
	}

	[Fact]
	public async Task WhenCollectionOrderIsIgnored_AndActualIsEmpty_ShouldReportEveryMissingElement()
	{
		int[] actual = [];
		int[] expected = [1, 2,];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [0] was missing 1
		                                                and
		                                                  Element [1] was missing 2
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenCollectionOrderIsIgnored_AndAnElementIsIgnored_ShouldIgnoreItInBothCollections()
	{
		int[] actual = [1, 2, 3,];
		int[] expected = [3, 99, 1,];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
			MembersToIgnore = [new MemberToIgnore.ByPredicate((path, _) => path == "[1]", "index 1"),],
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue()
			.Because("an ignored element has no counterpart it could be skipped in once the order is ignored, so neither the actual 2 has to be matched nor the expected 99 has to be found");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenCollectionOrderIsIgnored_AndBothCollectionsAreEmpty_ShouldSucceed()
	{
		int[] actual = [];
		int[] expected = [];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task
		WhenCollectionOrderIsIgnored_AndElementIsEquivalentToMultipleExpectedElements_ShouldStillMatchEveryElement()
	{
		object[] actual = [new WithTwoPublicValues(1, 2), new WithTwoPublicValues(1, 3),];
		object[] expected =
		[
			new
			{
				Value = 1,
			},
			new
			{
				Value = 1,
				Other = 2,
			},
		];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue()
			.Because("the first actual element is equivalent to both expected ones, so a greedy match would claim it for the first expected element and then report the second one as missing, although the second actual element is a counterpart for it");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenCollectionOrderIsIgnored_AndElementReferencesItself_ShouldNotExceedTheRecursionLimit()
	{
		NestedNode actualNode = new(1);
		actualNode.Inner = actualNode;
		NestedNode expectedNode = new(1);
		expectedNode.Inner = expectedNode;
		NestedNode[] actual = [actualNode,];
		NestedNode[] expected = [expectedNode,];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
			MaxRecursionDepth = 2,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue()
			.Because("the cycle detection stops the walk before the depth limit can be reached, also while elements are matched");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenCollectionOrderIsIgnored_AndElementsAreCollections_ShouldMatchThemInAnyOrder()
	{
		int[][] actual = [[1, 2,], [3, 4,],];
		int[][] expected = [[3, 4,], [1, 2,],];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenCollectionOrderIsIgnored_AndElementsAreNotComparable_ShouldMatchThemInAnyOrder()
	{
		WithProperty[] actual = [new(1), new(2),];
		WithProperty[] expected = [new(2), new(1),];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue()
			.Because("the elements are matched by the equivalency comparison, which a type that is not comparable also supports");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenCollectionOrderIsIgnored_AndElementsAreNotComparable_WhenInTheSameOrder_ShouldSucceed()
	{
		WithProperty[] actual = [new(1), new(2),];
		WithProperty[] expected = [new(1), new(2),];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue()
			.Because("ignoring the order must not make a collection that is already in order fail");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenCollectionOrderIsIgnored_AndElementsHaveDifferentTypes_ShouldMatchThemInAnyOrder()
	{
		object[] actual = [1, "a",];
		object[] expected = ["a", 1,];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue()
			.Because("elements of unrelated types cannot be sorted against each other, but they can be compared");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenCollectionOrderIsIgnored_AndExpectedIsEmpty_ShouldReportEverySuperfluousElement()
	{
		int[] actual = [1, 2,];
		int[] expected = [];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [0] had superfluous 1
		                                                and
		                                                  Element [1] had superfluous 2
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenCollectionOrderIsIgnored_AndLeftoversDiffer_ShouldPairThemByTheFewestDifferences()
	{
		WithTwoPublicValues[] actual = [new(1, 10), new(2, 20),];
		WithTwoPublicValues[] expected = [new(2, 99), new(1, 88),];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Field [0].Other differed:
		                                                       Found: 10
		                                                    Expected: 88
		                                                and
		                                                  Field [1].Other differed:
		                                                       Found: 20
		                                                    Expected: 99
		                                                """).IgnoringNewlineStyle()
			.Because("pairing each element with the one that shares its Value reports the one member that differs, while pairing them by position would report both members of both elements");
	}

	[Fact]
	public async Task WhenCollectionOrderIsIgnored_AndMultiplicityDiffers_ShouldReportTheDifference()
	{
		int[] actual = [1, 1, 2,];
		int[] expected = [1, 2, 2,];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse()
			.Because("each expected element needs an element of its own, so the same value cannot be matched twice");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [1] differed:
		                                                       Found: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenCollectionOrderIsIgnored_AndNestedCollectionsAreInAnyOrder_ShouldSucceed()
	{
		int[][] actual = [[1, 2,], [3, 4,],];
		int[][] expected = [[4, 3,], [2, 1,],];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue()
			.Because("the option applies to the nested collections as well");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenCollectionOrderIsIgnored_AndOnlySomeLeftoversCanBePaired_ShouldReportThePairsBeforeTheSurplus()
	{
		int[] actual = [1, 2,];
		int[] expected = [3, 4, 5,];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [0] differed:
		                                                       Found: 1
		                                                    Expected: 3
		                                                and
		                                                  Element [1] differed:
		                                                       Found: 2
		                                                    Expected: 4
		                                                and
		                                                  Element [2] was missing 5
		                                                """).IgnoringNewlineStyle()
			.Because("the expected elements that a leftover could be paired with are reported as differences in the order of the actual elements, and only the surplus that no actual element is left for is reported as missing");
	}

	[Fact]
	public async Task WhenCollectionOrderIsIgnored_AndOnlyTheExpectedElementIsComparedByValue_ShouldReportTheDifference()
	{
		object[] actual = [new WithLength(2),];
		object[] expected = ["ab",];
		StringBuilder failureBuilder = new();
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse()
			.Because("the matching of the elements has to decide the same way as the comparison of a single value");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [0] differed:
		                                                       Found: EquivalencyComparisonTests.WithLength { Length = 2 }
		                                                    Expected: "ab"
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenCollectionOrderIsIgnored_AndRecursionDepthExceedsTheLimit_ShouldReportTheMemberPath()
	{
		NestedNode[] actual = [new(4),];
		NestedNode[] expected = [new(4),];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
			MaxRecursionDepth = 3,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property [0].Inner.Inner exceeded the maximum recursion depth of 3
		                                                """).IgnoringNewlineStyle()
			.Because("an element that could not be matched is reported with the reason it could not be matched for");
	}

	[Fact]
	public async Task WhenCollectionOrderIsIgnored_AndTheCollectionContainsItself_ShouldSucceed()
	{
		List<object> actual = [1,];
		actual.Add(actual);
		List<object> expected = [1,];
		expected.Add(expected);
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue()
			.Because("the cycle detection also terminates the matching of a collection that is one of its own elements");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenCollectionOrderIsIgnored_AndTheOptionIsScopedToAType_ShouldOnlyApplyToThatType()
	{
		var actual = new
		{
			Ordered = new[]
			{
				1, 2,
			},
			Unordered = new List<int>
			{
				1, 2,
			},
		};
		var expected = new
		{
			Ordered = new[]
			{
				2, 1,
			},
			Unordered = new List<int>
			{
				2, 1,
			},
		};
		EquivalencyOptions options = new EquivalencyOptions().For<List<int>>(x => x with
		{
			IgnoreCollectionOrder = true,
		});
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse()
			.Because("only the members of the scoped type may ignore their order");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element Ordered[0] differed:
		                                                       Found: 1
		                                                    Expected: 2
		                                                and
		                                                  Element Ordered[1] differed:
		                                                       Found: 2
		                                                    Expected: 1
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenComparedByValue_ShouldReportTheDifferenceInsteadOfThrowing()
	{
		ValueLikeWithoutMembers actual = new(1);
		ValueLikeWithoutMembers expected = new(2);
		StringBuilder failureBuilder = new();
		EquivalencyOptions options = new()
		{
			ComparisonType = EquivalencyComparisonType.ByValue,
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).Contains("It differed:")
			.Because("comparing by value is the documented remedy for types without comparable members");
	}

	[Fact]
	public async Task WhenCultureInfoMemberDiffers_ShouldReportTheDifference()
	{
		var actual = new
		{
			Value = new CultureInfo("de-DE"),
		};
		var expected = new
		{
			Value = new CultureInfo("en-US"),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: de-DE
		                                                    Expected: en-US
		                                                """).IgnoringNewlineStyle()
			.Because("the culture name is the identity, while its members expand into every format pattern the operating system knows");
	}

	[Fact]
	public async Task WhenDelegateMemberDiffers_ShouldReportTheDifference()
	{
		var actual = new
		{
			Value = (Func<string?, bool>)string.IsNullOrEmpty,
		};
		var expected = new
		{
			Value = (Func<string?, bool>)string.IsNullOrWhiteSpace,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: Func<string, bool> { Method = Boolean IsNullOrEmpty(System.String), Target = <null> }
		                                                    Expected: Func<string, bool> { Method = Boolean IsNullOrWhiteSpace(System.String), Target = <null> }
		                                                """).IgnoringNewlineStyle()
			.Because("a delegate is its target and method, so walking it would drag a captured closure into the comparison");
	}

	[Fact]
	public async Task WhenDelegatesCaptureEqualValues_ShouldReportTheDifference()
	{
		var actual = new
		{
			Value = Capture(1),
		};
		var expected = new
		{
			Value = Capture(1),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse()
			.Because("two closures over the same value are separate targets, and walking them would compare whatever the lambda captured - up to the whole enclosing object");
		await That(failureBuilder.ToString()).Contains("Property Value differed:");
	}

	[Fact]
	public async Task WhenDictionaryIsAReadOnlyDictionary_WithTwoKeysThatOnlyTheWrappedComparerUnifies_ShouldReportTheKeyWithoutADistinctKey()
	{
		ReadOnlyDictionary<string, int> actual = new(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
		{
			["a"] = 1,
			["b"] = 1,
		});
		Dictionary<string, int> expected = new()
		{
			["a"] = 1,
			["A"] = 1,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [b] had superfluous 1
		                                                and
		                                                  Element [A] lacked a distinct key
		                                                """).IgnoringNewlineStyle()
			.Because("a read-only dictionary looks its keys up through the dictionary it wraps");
	}

	[Fact]
	public async Task WhenDictionaryIsASortedDictionary_WithTwoKeysThatOnlyItsComparerUnifies_ShouldReportTheKeyWithoutADistinctKey()
	{
		SortedDictionary<string, int> actual = new(StringComparer.OrdinalIgnoreCase)
		{
			["a"] = 1,
			["b"] = 1,
		};
		Dictionary<string, int> expected = new()
		{
			["a"] = 1,
			["A"] = 1,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [b] had superfluous 1
		                                                and
		                                                  Element [A] lacked a distinct key
		                                                """).IgnoringNewlineStyle()
			.Because("a sorted dictionary considers two keys the same when its comparer orders neither before the other");
	}

	[Fact]
	public async Task WhenDictionaryIsNestedInAMember_AndSubjectUsesACaseInsensitiveComparer_ShouldLookTheExpectedKeysUpThroughIt()
	{
		var actual = new
		{
			Value = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
			{
				["a"] = 1,
			},
		};
		var expected = new
		{
			Value = new Dictionary<string, int>
			{
				["A"] = 1,
			},
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("the key comparer of the dictionary at that point of the subject decides which keys are the same");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenDictionaryKeyContainsABracket_ShouldNotIgnoreTheEntryForAnotherKey()
	{
		Dictionary<string, int> actual = new()
		{
			["a[b"] = 1,
			["b"] = 2,
		};
		Dictionary<string, int> expected = new()
		{
			["a[b"] = 11,
			["b"] = 22,
		};
		StringBuilder failureBuilder = new();
		EquivalencyOptions options = new()
		{
			MembersToIgnore = [new MemberToIgnore.ByName("[b]"),],
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [a[b] differed:
		                                                       Found: 1
		                                                    Expected: 11
		                                                """).IgnoringNewlineStyle()
			.Because("the bracket inside the key does not open the path segment that the ignored name refers to");
	}

	[Fact]
	public async Task WhenDictionaryOnlyImplementsTheGenericInterface_AndEntriesAreInDifferentOrder_ShouldSucceed()
	{
		ReadOnlyDictionaryOnly<string, int> actual = new(new Dictionary<string, int>
		{
			["A"] = 1,
			["B"] = 2,
		});
		ReadOnlyDictionaryOnly<string, int> expected = new(new Dictionary<string, int>
		{
			["B"] = 2,
			["A"] = 1,
		});
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("a dictionary is a keyed lookup, so the order in which it enumerates its entries is not part of its content");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenDictionaryOnlyImplementsTheGenericInterface_AndExpectedIsANonGenericOne_ShouldCompareByKey()
	{
		ReadOnlyDictionaryOnly<string, int> actual = new(new Dictionary<string, int>
		{
			["A"] = 1,
			["B"] = 2,
		});
		Dictionary<string, int> expected = new()
		{
			["B"] = 2,
			["A"] = 1,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("both sides offer a lookup by key, whichever dictionary interface they implement");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenDictionaryOnlyImplementsTheGenericInterface_AndSubjectExposesACaseInsensitiveComparer_ShouldLookTheExpectedKeysUpThroughIt()
	{
		ReadOnlyDictionaryOnlyWithComparer<string, int> actual = new(
			new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
			{
				["a"] = 1,
			});
		Dictionary<string, int> expected = new()
		{
			["A"] = 1,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("the entries are copied into a dictionary that uses the comparer the subject exposes");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenDictionaryOnlyImplementsTheGenericInterface_AndSubjectHidesItsComparer_ShouldCompareTheKeysByTheirEquality()
	{
		ReadOnlyDictionaryOnly<string, int> actual = new(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
		{
			["a"] = 1,
		});
		Dictionary<string, int> expected = new()
		{
			["A"] = 1,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [a] had superfluous 1
		                                                and
		                                                  Element [A] was missing 1
		                                                """).IgnoringNewlineStyle()
			.Because("a comparer that cannot be read cannot be honoured by the copy of the entries");
	}

	[Fact]
	public async Task WhenDictionaryOnlyImplementsTheGenericInterface_AndValueDiffers_ShouldReportTheKey()
	{
		ReadOnlyDictionaryOnly<string, int> actual = new(new Dictionary<string, int>
		{
			["A"] = 1,
			["B"] = 2,
		});
		ReadOnlyDictionaryOnly<string, int> expected = new(new Dictionary<string, int>
		{
			["A"] = 1,
			["B"] = 3,
		});
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [B] differed:
		                                                       Found: 2
		                                                    Expected: 3
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenDictionarySubjectUsesACaseInsensitiveComparer_ShouldLookTheExpectedKeysUpThroughIt()
	{
		Dictionary<string, int> actual = new(StringComparer.OrdinalIgnoreCase)
		{
			["a"] = 1,
		};
		Dictionary<string, int> expected = new()
		{
			["A"] = 1,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("the key comparer of the subject decides which keys are the same");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenDictionarySubjectUsesACaseInsensitiveComparer_WithADifferentValue_ShouldReportTheExpectedKey()
	{
		Dictionary<string, int> actual = new(StringComparer.OrdinalIgnoreCase)
		{
			["a"] = 1,
		};
		Dictionary<string, int> expected = new()
		{
			["A"] = 2,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [A] differed:
		                                                       Found: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenDictionarySubjectUsesACaseInsensitiveComparer_WithAnAdditionalKey_ShouldReportItAsSuperfluous()
	{
		Dictionary<string, int> actual = new(StringComparer.OrdinalIgnoreCase)
		{
			["a"] = 1,
			["b"] = 2,
		};
		Dictionary<string, int> expected = new()
		{
			["A"] = 1,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [b] had superfluous 2
		                                                """).IgnoringNewlineStyle()
			.Because("the matched keys are collected with the comparer of the subject, so the remaining keys are exact");
	}

	[Fact]
	public async Task WhenDictionarySubjectUsesACaseInsensitiveComparer_WithTwoKeysThatOnlyItUnifies_AndTheSecondIsIgnored_ShouldSucceed()
	{
		Dictionary<string, int> actual = new(StringComparer.OrdinalIgnoreCase)
		{
			["a"] = 1,
		};
		Dictionary<string, int> expected = new()
		{
			["a"] = 1,
			["A"] = 1,
		};
		StringBuilder failureBuilder = new();
		EquivalencyOptions options = new()
		{
			MembersToIgnore = [new MemberToIgnore.ByName("[A]"),],
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenDictionarySubjectUsesACaseInsensitiveComparer_WithTwoKeysThatOnlyItUnifies_ShouldReportTheKeyWithoutADistinctKey()
	{
		Dictionary<string, int> actual = new(StringComparer.OrdinalIgnoreCase)
		{
			["a"] = 1,
		};
		Dictionary<string, int> expected = new()
		{
			["a"] = 1,
			["A"] = 1,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [A] lacked a distinct key
		                                                """).IgnoringNewlineStyle()
			.Because("one entry of the subject cannot stand in for two entries of the expected dictionary");
	}

	[Fact]
	public async Task WhenDictionarySubjectUsesACaseInsensitiveComparer_WithTwoKeysThatOnlyItUnifiesAndAnAdditionalKey_ShouldReportBoth()
	{
		Dictionary<string, int> actual = new(StringComparer.OrdinalIgnoreCase)
		{
			["a"] = 1,
			["b"] = 1,
		};
		Dictionary<string, int> expected = new()
		{
			["a"] = 1,
			["A"] = 1,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [b] had superfluous 1
		                                                and
		                                                  Element [A] lacked a distinct key
		                                                """).IgnoringNewlineStyle()
			.Because("the collapsed expected key counts only once, so the entry count does not hide the leftover key");
	}

	[Fact]
	public async Task WhenDictionarySubjectUsesACaseSensitiveComparer_AndExpectedACaseInsensitiveOne_ShouldReportMissingAndSuperfluousKeys()
	{
		Dictionary<string, int> actual = new()
		{
			["a"] = 1,
		};
		Dictionary<string, int> expected = new(StringComparer.OrdinalIgnoreCase)
		{
			["A"] = 1,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [a] had superfluous 1
		                                                and
		                                                  Element [A] was missing 1
		                                                """).IgnoringNewlineStyle()
			.Because("the key comparer of the expected dictionary does not decide which keys the subject has");
	}

	[Fact]
	public async Task WhenDictionarySubjectUsesACaseSensitiveComparer_WithAKeyThatOnlyTheExpectedOneUnifies_ShouldReportItAsSuperfluous()
	{
		Dictionary<string, int> actual = new()
		{
			["a"] = 1,
			["A"] = 1,
		};
		Dictionary<string, int> expected = new(StringComparer.OrdinalIgnoreCase)
		{
			["a"] = 1,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [A] had superfluous 1
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenDictionarySubjectUsesACustomComparer_WithTwoKeysThatOnlyItUnifies_ShouldReportTheKeyWithoutADistinctKey()
	{
		Dictionary<string, int> actual = new(new CaseInsensitiveComparer())
		{
			["a"] = 1,
			["b"] = 1,
		};
		Dictionary<string, int> expected = new()
		{
			["a"] = 1,
			["A"] = 1,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [b] had superfluous 1
		                                                and
		                                                  Element [A] lacked a distinct key
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenDictionarySubjectUsesAnUnreadableComparer_WithTwoKeysThatOnlyItUnifies_ShouldReportTheKeyCounts()
	{
		Hashtable actual = new(StringComparer.OrdinalIgnoreCase)
		{
			["a"] = 1,
		};
		Dictionary<string, int> expected = new()
		{
			["a"] = 1,
			["A"] = 1,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  It contained 1 key and matched 2 expected keys
		                                                """).IgnoringNewlineStyle()
			.Because("a hashtable does not expose its comparer, so naming the keys could overshoot");
	}

	[Fact]
	public async Task WhenDictionaryValueDiffers_ShouldReportTheKey()
	{
		Dictionary<string, int> actual = new()
		{
			["A"] = 1,
			["B"] = 2,
		};
		Dictionary<string, int> expected = new()
		{
			["A"] = 1,
			["B"] = 3,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [B] differed:
		                                                       Found: 2
		                                                    Expected: 3
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenEnumMembersOfDifferentTypesFormatIdentically_ShouldAppendTheRuntimeType()
	{
		var actual = new
		{
			Value = (object)EnumWithFoo.Foo,
		};
		var expected = new
		{
			Value = (object)OtherEnumWithFoo.Foo,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: Foo (EquivalencyComparisonTests.EnumWithFoo)
		                                                    Expected: Foo (EquivalencyComparisonTests.OtherEnumWithFoo)
		                                                """).IgnoringNewlineStyle()
			.Because("the rule is about values that cannot be told apart, not about numeric types");
	}

	[Fact]
	public async Task WhenExpectedFieldIsMissingOnTheActualType_ShouldReportItAsMissing()
	{
		WithPublicValue actual = new(1);
		WithTwoPublicValues expected = new(1, 2);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Field Other is missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("a field is reported as missing just like a property");
	}

	[Fact]
	public async Task WhenExpectedFieldMatchesAnActualProperty_ShouldCompareThem()
	{
		WithProperty actual = new(1);
		WithPublicValue expected = new(1);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("whether the actual type stores the member as a field or as a property is an implementation detail of that type");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenExpectedFieldMatchesAnActualProperty_WhenPropertiesAreExcluded_ShouldReportItAsMissing()
	{
		WithProperty actual = new(1);
		WithPublicValue expected = new(1);
		EquivalencyOptions options = new()
		{
			Properties = IncludeMembers.None,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Field Value is missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("the fallback may only reach a kind that the caller included");
	}

	[Fact]
	public async Task WhenExpectedFieldMatchesAnActualProperty_WhenTheyDiffer_ShouldReportItAsAField()
	{
		WithProperty actual = new(1);
		WithPublicValue expected = new(2);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Field Value differed:
		                                                       Found: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("the compared members come from the expected object, so its kind names the difference and agrees with the kind a scoped ignore rule applies to");
	}

	[Fact]
	public async Task WhenExpectedMemberIsAString_AndActualMemberIsComparedByMembers_ShouldReportTheDifference()
	{
		var actual = new
		{
			Value = new WithLength(2),
		};
		var expected = new
		{
			Value = "ab",
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse()
			.Because("swapping subject and expectation must not change the result, and a string is never equivalent to a non-string");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: EquivalencyComparisonTests.WithLength { Length = 2 }
		                                                    Expected: "ab"
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenExpectedMemberIsMissingOnTheActualType_WhenIgnored_ShouldSucceed()
	{
		var actual = new
		{
			A = 1,
		};
		var expected = new
		{
			A = 1,
			B = 5,
		};
		EquivalencyOptions options = new()
		{
			MembersToIgnore = [new MemberToIgnore.ByName("B"),],
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue()
			.Because("an ignored member is never looked up on the actual object, so it cannot be missing");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenExpectedMemberIsNull_ShouldReportFoundAndExpected()
	{
		var actual = new
		{
			Value = (int?)1,
		};
		var expected = new
		{
			Value = (int?)null,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: 1
		                                                    Expected: <null>
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenExpectedPropertyIsMissingOnTheActualType_ShouldReportItAsMissing()
	{
		var actual = new
		{
			A = 1,
		};
		var expected = new
		{
			A = 1,
			B = 5,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property B is missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("the actual object never had the member, so claiming that it was found as <null> would be wrong");
	}

	[Fact]
	public async Task WhenExpectedPropertyIsMissingOnTheActualType_WithNullValue_ShouldStillFail()
	{
		var actual = new
		{
			A = 1,
		};
		var expected = new
		{
			A = 1,
			B = (string?)null,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse()
			.Because("a member the actual object does not have has to fail whatever the expected value is");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property B is missing on the actual object
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenExpectedPropertyMatchesAnActualField_AtANestedPath_ShouldReportTheFullMemberPath()
	{
		var actual = new
		{
			Inner = new WithPublicValue(1),
		};
		var expected = new
		{
			Inner = new WithProperty(2),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Inner.Value differed:
		                                                       Found: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("the fallback applies at every level of the graph, and the path stays the one of the expected member");
	}

	[Fact]
	public async Task WhenExpectedPropertyMatchesAnActualField_ShouldCompareThem()
	{
		WithPublicValue actual = new(1);
		WithProperty expected = new(1);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("a DTO with public fields is routinely compared against an anonymous object, which can only have properties");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenExpectedPropertyMatchesAnActualField_WhenFieldsAreExcluded_ShouldReportItAsMissing()
	{
		WithPublicValue actual = new(1);
		WithProperty expected = new(1);
		EquivalencyOptions options = new()
		{
			Fields = IncludeMembers.None,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value is missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("the fallback may only reach a kind that the caller included");
	}

	[Fact]
	public async Task WhenExpectedPropertyMatchesAnActualField_WhenIgnored_ShouldSucceed()
	{
		WithPublicValue actual = new(1);
		WithProperty expected = new(2);
		EquivalencyOptions options = new()
		{
			MembersToIgnore = [new MemberToIgnore.ByName("Value"),],
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue()
			.Because("an ignored member is never looked up on the actual object, so the fallback cannot revive it");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenExpectedPropertyMatchesAnActualField_WhenTheyDiffer_ShouldReportItAsAProperty()
	{
		WithPublicValue actual = new(1);
		WithProperty expected = new(2);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("the compared members come from the expected object, so its kind names the difference and agrees with the kind a scoped ignore rule applies to");
	}

	[Fact]
	public async Task WhenExpectedPropertyMatchesARegisteredField_ShouldCompareThem()
	{
		RegisterPhantomField();
		RegisteredFieldProbe actual = new(1);
		var expected = new
		{
			Phantom = 2,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Phantom differed:
		                                                       Found: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("a registration keeps the kind of the member, so the fallback has to cross it in the registry as well");
	}

	[Fact]
	public async Task WhenExpectedTypeIsComparedByValueByTheSelector_ShouldCompareByValue()
	{
		WithProperty actual = new(1);
		ValueObject expected = new(1);
		EquivalencyOptions options = new()
		{
			DefaultComparisonTypeSelector = type => type == typeof(ValueObject)
				? EquivalencyComparisonType.ByValue
				: EquivalencyDefaults.DefaultComparisonType(type),
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, options, new StringBuilder());

		await That(result).IsFalse()
			.Because("the expected type asks for its own equality, which the subject does not satisfy although it has the same members");
	}

	[Fact]
	public async Task WhenExpectedTypeIsComparedByValueForTheType_AndItsEqualsThrows_ShouldFailWithTheException()
	{
		WithProperty actual = new(1);
		WithThrowingEquals expected = new();

		async Task Act()
			=> await That(actual).IsEquivalentTo(expected, o => o
				.For<WithThrowingEquals>(t => t with
				{
					ComparisonType = EquivalencyComparisonType.ByValue,
				}));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that actual
			             is equivalent to EquivalencyComparisonTests.WithThrowingEquals {
			                 Value = 1
			               },
			             but it did throw a NotSupportedException:
			               equals

			             Equivalency options:
			              - include public fields and properties
			              - for EquivalencyComparisonTests.WithThrowingEquals:
			                - include public fields and properties
			                - compare types by value
			             """).And
			.Whose(e => e.InnerException, i => i.Is<NotSupportedException>())
			.Because("the Equals that is called belongs to the expected value, which is the only side compared by value");
	}

	[Fact]
	public async Task WhenExpectedTypeIsComparedByValueForTheType_AndItsEqualsThrows_WhenNegated_ShouldFail()
	{
		WithProperty actual = new(1);
		WithThrowingEquals unexpected = new();

		async Task Act()
			=> await That(actual).IsNotEquivalentTo(unexpected, o => o
				.For<WithThrowingEquals>(t => t with
				{
					ComparisonType = EquivalencyComparisonType.ByValue,
				}));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that actual
			             is not equivalent to EquivalencyComparisonTests.WithThrowingEquals {
			                 Value = 1
			               },
			             but it did throw a NotSupportedException:
			               equals

			             Equivalency options:
			              - include public fields and properties
			              - for EquivalencyComparisonTests.WithThrowingEquals:
			                - include public fields and properties
			                - compare types by value
			             """).And
			.Whose(e => e.InnerException, i => i.Is<NotSupportedException>())
			.Because("an Equals that threw answered nothing, so the negation fails as well");
	}

	[Fact]
	public async Task WhenExpectedTypeIsComparedByValueForTheType_ShouldCompareByValue()
	{
		WithProperty actual = new(1);
		ValueObject expected = new(1);
		EquivalencyOptions options = new EquivalencyOptions()
			.For<ValueObject>(o => o with
			{
				ComparisonType = EquivalencyComparisonType.ByValue,
			});

		bool result = await EquivalencyComparison.Compare(actual, expected, options, new StringBuilder());

		await That(result).IsFalse()
			.Because("the expected type asks for its own equality, which the subject does not satisfy although it has the same members");
	}

	[Fact]
	public async Task WhenExpectedTypeIsDerivedFromTheActualType_ShouldReportTheAdditionalMemberAsMissing()
	{
		WithProperty actual = new(1);
		WithProperty expected = new DerivedWithAdditionalProperty(1, 5);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Additional is missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("the members are taken from the runtime type of the expected object, which the actual type does not have");
	}

	[Theory]
	[InlineData(1, 3, 2, 3, "Property Value differed")]
	[InlineData(1, 3, 1, 4, "Field Value differed")]
	public async Task WhenFieldHidesAProperty_ShouldCompareBoth(int actualProperty, int actualField,
		int expectedProperty, int expectedField, string expectedDifference)
	{
		FieldHidingProperty actual = new(actualProperty, actualField);
		FieldHidingProperty expected = new(expectedProperty, expectedField);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).Contains(expectedDifference)
			.Because("a field and a property of the same name are both members of the type");
	}

	[Fact]
	public async Task WhenGetterThrows_ShouldFailWithTheGetterException()
	{
		WithThrowingGetter actual = new("getter failed");
		WithThrowingGetter expected = new("getter failed");

		async Task Act()
			=> await That(actual).IsEquivalentTo(expected);

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that actual
			             is equivalent to EquivalencyComparisonTests.WithThrowingGetter {
			                 Value = [Member 'Value' threw an exception: 'getter failed']
			               },
			             but it did throw an InvalidOperationException:
			               getter failed

			             Equivalency options:
			              - include public fields and properties
			             """).And
			.Whose(e => e.InnerException, i => i.Is<InvalidOperationException>())
			.Because("reflection wraps the exception, while a registered accessor lets it through, so both paths have to agree");
	}

	[Fact]
	public async Task WhenGetterThrows_WhenNegated_ShouldFailWithTheGetterException()
	{
		WithThrowingGetter actual = new("getter failed");
		WithThrowingGetter unexpected = new("getter failed");

		async Task Act()
			=> await That(actual).IsNotEquivalentTo(unexpected);

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that actual
			             is not equivalent to EquivalencyComparisonTests.WithThrowingGetter {
			                 Value = [Member 'Value' threw an exception: 'getter failed']
			               },
			             but it did throw an InvalidOperationException:
			               getter failed

			             Equivalency options:
			              - include public fields and properties
			             """).And
			.Whose(e => e.InnerException, i => i.Is<InvalidOperationException>())
			.Because("a getter that threw answered nothing, so the negation fails as well");
	}

	[Fact]
	public async Task WhenGraphReferencesItself_ShouldNotExceedTheRecursionLimit()
	{
		NestedNode actual = new(1);
		actual.Inner = actual;
		NestedNode expected = new(1);
		expected.Inner = expected;
		EquivalencyOptions options = new()
		{
			MaxRecursionDepth = 1,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue()
			.Because("the cycle detection stops the walk before the depth limit can be reached");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenHandleMembersAreEqual_ShouldSucceed()
	{
		var actual = new
		{
			Type = typeof(int),
			Method = typeof(string).GetMethod(nameof(string.Trim), Type.EmptyTypes),
			Assembly = typeof(EquivalencyComparisonTests).Assembly,
			Module = typeof(EquivalencyComparisonTests).Module,
			Delegate = (Func<string?, bool>)string.IsNullOrEmpty,
			Uri = new Uri("a/b", UriKind.Relative),
			Culture = new CultureInfo("de-DE"),
		};
		var expected = new
		{
			Type = typeof(int),
			Method = typeof(string).GetMethod(nameof(string.Trim), Type.EmptyTypes),
			Assembly = typeof(EquivalencyComparisonTests).Assembly,
			Module = typeof(EquivalencyComparisonTests).Module,
			Delegate = (Func<string?, bool>)string.IsNullOrEmpty,
			Uri = new Uri("a/b", UriKind.Relative),
			Culture = new CultureInfo("de-DE"),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("the separate instances are equal handles, and the by-value comparison asks Equals instead of the reference");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenIntPtrMemberDiffers_ShouldReportTheDifference()
	{
		var actual = new
		{
			Value = (IntPtr)1,
		};
		var expected = new
		{
			Value = (IntPtr)2,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("a native integer is a primitive, so it needs no entry of its own among the by-value defaults");
	}

	[Fact]
	public async Task WhenItIsMemberDoesNotMatch_ShouldReportTheExpectationAsExpected()
	{
		var actual = new
		{
			Value = 1,
		};
		var expected = new
		{
			Value = It.Is<int>().That.IsGreaterThan(2),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: 1
		                                                    Expected: is int that is greater than 2
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenItIsMemberExpectationSpansMultipleLines_ShouldIndentTheContinuationLines()
	{
		var actual = new
		{
			Value = new WithPublicValue(1),
		};
		var expected = new
		{
			Value = It.Is<WithPublicValue>().That.IsEquivalentTo(new WithPublicValue(2)),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: EquivalencyComparisonTests.WithPublicValue { Value = 1 }
		                                                    Expected: is EquivalencyComparisonTests.WithPublicValue that is equivalent to EquivalencyComparisonTests.WithPublicValue {
		                                                        Value = 2
		                                                      }
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenItIsMemberHasADifferentType_ShouldIncludeTheFoundType()
	{
		var actual = new
		{
			Value = "abc",
		};
		var expected = new
		{
			Value = It.Is<DateTime>(),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: "abc" (string)
		                                                    Expected: is DateTime
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenItIsMemberIsNull_ShouldNotIncludeAType()
	{
		var actual = new
		{
			Value = (string?)null,
		};
		var expected = new
		{
			Value = It.Is<string>().That.IsEmpty(),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: <null>
		                                                    Expected: is string that is empty
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenListElementsAreInDifferentOrder_ShouldReportTheDifference()
	{
		List<int> actual = [1, 2, 3,];
		List<int> expected = [3, 2, 1,];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse()
			.Because("a list has an order that is part of its content, so only a set or a dictionary is compared without it");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [0] differed:
		                                                       Found: 1
		                                                    Expected: 3
		                                                and
		                                                  Element [2] differed:
		                                                       Found: 3
		                                                    Expected: 1
		                                                """).IgnoringNewlineStyle();
	}

	[Theory]
	[InlineData("foo", "foo", true)]
	[InlineData("foo", "bar", false)]
	public async Task WhenMemberIsHidden_ShouldCompareTheMostDerivedDeclarationOnly(string actualText,
		string expectedText, bool expectedResult)
	{
		PropertyHidingProperty actual = new(1, actualText);
		PropertyHidingProperty expected = new(2, expectedText);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsEqualTo(expectedResult)
			.Because("reflection returns both declarations, but only the one on the most derived type is visible");
	}

	[Fact]
	public async Task WhenMembersFormatIdentically_ShouldAppendTheRuntimeType()
	{
		var actual = new
		{
			Value = 1,
		};
		var expected = new
		{
			Value = 1L,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: 1 (int)
		                                                    Expected: 1 (long)
		                                                """).IgnoringNewlineStyle()
			.Because("an int member and a long member are a real difference that the formatted values do not show");
	}

	[Fact]
	public async Task WhenMembersFormatIdenticallyWithTheSameType_ShouldNotAppendTheRuntimeType()
	{
		ValueLikeWithConstantText actual = new(1);
		ValueLikeWithConstantText expected = new(2);
		StringBuilder failureBuilder = new();
		EquivalencyOptions options = new()
		{
			ComparisonType = EquivalencyComparisonType.ByValue,
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  It differed:
		                                                       Found: ValueLikeWithConstantText
		                                                    Expected: ValueLikeWithConstantText
		                                                """).IgnoringNewlineStyle()
			.Because("one and the same type on both sides tells the two values apart just as little as the values do");
	}

	[Fact]
	public async Task WhenMembersOfDifferentTypesFormatDifferently_ShouldNotAppendTheRuntimeType()
	{
		var actual = new
		{
			Value = 1,
		};
		var expected = new
		{
			Value = 2L,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("the type would be noise where the values already differ");
	}

	[Theory]
	[InlineData("ame", false)]
	[InlineData("d.Name", false)]
	[InlineData("Name", true)]
	[InlineData("Child.Name", true)]
	public async Task WhenMemberToIgnoreIsGiven_ShouldOnlyIgnoreWholePathSegments(string memberToIgnore,
		bool expectedResult)
	{
		var actual = new
		{
			Child = new
			{
				Name = "cc",
			},
		};
		var expected = new
		{
			Child = new
			{
				Name = "c",
			},
		};
		EquivalencyOptions options = new()
		{
			MembersToIgnore = [new MemberToIgnore.ByName(memberToIgnore),],
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, options, new StringBuilder());

		await That(result).IsEqualTo(expectedResult)
			.Because("only a name that covers whole segments of the member path may exclude Child.Name");
	}

	[Fact]
	public async Task WhenMethodInfoMemberDiffers_ShouldReportTheDifference()
	{
		var actual = new
		{
			Value = typeof(string).GetMethod(nameof(string.Trim), Type.EmptyTypes),
		};
		var expected = new
		{
			Value = typeof(string).GetMethod(nameof(string.ToUpperInvariant), Type.EmptyTypes),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: System.String Trim()
		                                                    Expected: System.String ToUpperInvariant()
		                                                """).IgnoringNewlineStyle()
			.Because("walking a member descriptor reports metadata tokens and raw runtime handle addresses instead of the method it stands for");
	}

	[Fact]
	public async Task WhenModuleMemberDiffers_ShouldReportTheDifference()
	{
		var actual = new
		{
			Value = typeof(EquivalencyComparisonTests).Module,
		};
		var expected = new
		{
			Value = typeof(EquivalencyComparison).Module,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: aweXpect.Core.Tests.dll
		                                                    Expected: aweXpect.Core.dll
		                                                """).IgnoringNewlineStyle()
			.Because("a module describes an emitted file, so walking it reaches getters that throw instead of state that could be compared");
	}

	[Fact]
	public async Task WhenMultipleMembersAreMissingOrDiffer_ShouldSeparateThemWithAnd()
	{
		var actual = new
		{
			Bee = 1,
			Dog = 2,
		};
		var expected = new
		{
			Ant = 3,
			Bee = 9,
			Cow = 4,
			Dog = 2,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Ant is missing on the actual object
		                                                and
		                                                  Property Bee differed:
		                                                       Found: 1
		                                                    Expected: 9
		                                                and
		                                                  Property Cow is missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("a missing member has to be separated from the other findings, in either direction");
	}

	[Fact]
	public async Task WhenMultipleMembersDiffer_ShouldSeparateThemWithAnd()
	{
		var actual = new
		{
			First = 1,
			Second = (string?)null,
		};
		var expected = new
		{
			First = 2,
			Second = (string?)"Foo",
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property First differed:
		                                                       Found: 1
		                                                    Expected: 2
		                                                and
		                                                  Property Second differed:
		                                                       Found: <null>
		                                                    Expected: "Foo"
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenNestedExpectedMemberIsMissing_ShouldReportTheFullMemberPath()
	{
		var actual = new
		{
			Inner = new
			{
				A = 1,
			},
		};
		var expected = new
		{
			Inner = new
			{
				A = 1,
				B = 5,
			},
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Inner.B is missing on the actual object
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenNestedMemberDiffers_ShouldReportTheFullMemberPath()
	{
		var actual = new
		{
			Inner = new
			{
				Value = (string?)null,
			},
		};
		var expected = new
		{
			Inner = new
			{
				Value = (string?)"Foo",
			},
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Inner.Value differed:
		                                                       Found: <null>
		                                                    Expected: "Foo"
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenNestedMemberHasNoComparableMembers_ShouldIncludeTheMemberPath()
	{
		ClassWithPrivateStateMember actual = new(new ClassWithOnlyPrivateState(1));
		ClassWithPrivateStateMember expected = new(new ClassWithOnlyPrivateState(2));

		async Task Act()
			=> await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), new StringBuilder());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Property Inner has no members that could be compared on *")
			.AsWildcard();
	}

	[Fact]
	public async Task WhenNestedMembersFormatIdentically_ShouldAppendTheRuntimeType()
	{
		var actual = new
		{
			Inner = new
			{
				Value = 1,
			},
		};
		var expected = new
		{
			Inner = new
			{
				Value = 1L,
			},
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Inner.Value differed:
		                                                       Found: 1 (int)
		                                                    Expected: 1 (long)
		                                                """).IgnoringNewlineStyle()
			.Because("the member path does not tell the values apart either");
	}

	[Fact]
	public async Task WhenNoMembersCanBeCompared_ShouldThrowInvalidOperationException()
	{
		ClassWithOnlyPrivateState actual = new(1);
		ClassWithOnlyPrivateState expected = new(2);

		async Task Act()
			=> await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), new StringBuilder());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage(
				"It has no members that could be compared on EquivalencyComparisonTests.ClassWithOnlyPrivateState, which would make the equivalency comparison succeed without verifying anything.*")
			.AsWildcard();
	}

	[Fact]
	public async Task WhenOneMemberIsNull_ShouldNotAppendTheRuntimeType()
	{
		var actual = new
		{
			Value = (object?)null,
		};
		var expected = new
		{
			Value = (object?)1L,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: <null>
		                                                    Expected: 1
		                                                """).IgnoringNewlineStyle()
			.Because("a missing value has no runtime type, and it is already distinguishable without one");
	}

	[Fact]
	public async Task WhenOptionsAreScopedToAnExpectedType_ShouldKeepTheRecursionLimit()
	{
		NestedNode actual = new(4);
		NestedNode expected = new(4);
		EquivalencyOptions<NestedNode> options = new(new EquivalencyOptions
		{
			MaxRecursionDepth = 3,
		});
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse()
			.Because("the type-scoped options have to carry over the limit of the options they were created from");
		await That(failureBuilder.ToString()).Contains("exceeded the maximum recursion depth of 3");
	}

	[Fact]
	public async Task WhenRecursionDepthExceedsTheLimit_ShouldReportTheMemberPath()
	{
		NestedNode actual = new(4);
		NestedNode expected = new(4);
		EquivalencyOptions options = new()
		{
			MaxRecursionDepth = 3,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse()
			.Because("a graph that is deeper than the limit has to fail instead of overflowing the stack");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Inner.Inner.Inner exceeded the maximum recursion depth of 3
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenRecursionDepthIsWithinTheLimit_ShouldCompareTheWholeGraph()
	{
		NestedNode actual = new(50);
		NestedNode expected = new(50);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("a graph below the default limit of 100 levels is compared as before");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenRecursionLimitIsRaised_ShouldCompareTheDeeperLevels()
	{
		NestedNode actual = new(150);
		NestedNode expected = new(150);
		EquivalencyOptions options = new()
		{
			MaxRecursionDepth = 200,
		};

		bool withDefaultLimit =
			await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), new StringBuilder());
		bool withRaisedLimit = await EquivalencyComparison.Compare(actual, expected, options, new StringBuilder());

		await That(withDefaultLimit).IsFalse()
			.Because("150 levels exceed the default limit of 100");
		await That(withRaisedLimit).IsTrue()
			.Because("the configured limit replaces the default");
	}

	[Fact]
	public async Task WhenSetElementsAreInDifferentOrder_ShouldSucceed()
	{
		var actual = new
		{
			Values = new HashSet<int>
			{
				1, 2, 3,
			},
		};
		var expected = new
		{
			Values = new HashSet<int>
			{
				3, 2, 1,
			},
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("a set has no order, so comparing two of them by position would only report how they happen to be stored");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenSetElementsAreObjects_AndAreInDifferentOrder_ShouldSucceed()
	{
		HashSet<WithProperty> actual = [new(1), new(2),];
		HashSet<WithProperty> expected = [new(2), new(1),];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("the elements of a set are matched by the equivalency comparison, which does not need them to be equal");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenSetElementsDiffer_ShouldReportTheDifference()
	{
		HashSet<int> actual = [1, 2,];
		HashSet<int> expected = [1, 3,];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [1] differed:
		                                                       Found: 2
		                                                    Expected: 3
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenSetIsComparedAgainstACollectionWithDuplicates_ShouldReportTheDifference()
	{
		HashSet<int> actual = [1, 2,];
		int[] expected = [1, 1,];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse()
			.Because("every element can be matched only once, so a set of two distinct values cannot cover the same value twice");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [1] differed:
		                                                       Found: 2
		                                                    Expected: 1
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenSetIsComparedAgainstAnOrderedCollection_ShouldIgnoreTheOrder()
	{
		HashSet<int> actual = [1, 2, 3,];
		int[] expected = [3, 2, 1,];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("one side being a set leaves no order the other side could be compared against");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Fact]
	public async Task WhenSetIsNested_AndAnElementDiffers_ShouldReportTheMemberPath()
	{
		var actual = new
		{
			Values = new HashSet<int>
			{
				1, 2,
			},
		};
		var expected = new
		{
			Values = new HashSet<int>
			{
				1, 3,
			},
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element Values[1] differed:
		                                                       Found: 2
		                                                    Expected: 3
		                                                """).IgnoringNewlineStyle();
	}

#if NET8_0_OR_GREATER
	[Fact]
	public async Task WhenSetOnlyImplementsTheReadOnlyInterface_AndElementsAreInDifferentOrder_ShouldSucceed()
	{
		ReadOnlySetOnly<int> actual = new([1, 2, 3,]);
		ReadOnlySetOnly<int> expected = new([3, 2, 1,]);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("a set that can only be read has no order either");
		await That(failureBuilder.ToString()).IsEmpty();
	}
#endif

	[Fact]
	public async Task WhenStringMemberIsLong_ShouldTruncateIt()
	{
		var actual = new
		{
			Value = new string('a', 120),
		};
		var expected = new
		{
			Value = new string('b', 120),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo($"""

		                                                   Property Value differed:
		                                                        Found: "{new string('a', 100)}…"
		                                                     Expected: "{new string('b', 100)}…"
		                                                 """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenStringMemberIsMultiLine_ShouldRenderItOnASingleLine()
	{
		var actual = new
		{
			Value = "foo\nbar",
		};
		var expected = new
		{
			Value = "foo\nbaz",
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: "foo\nbar"
		                                                    Expected: "foo\nbaz"
		                                                """).IgnoringNewlineStyle();
	}

	[Fact]
	public async Task WhenTypeHasAnIndexer_ShouldIgnoreTheIndexer()
	{
		WithIndexer actual = new()
		{
			Count = 1,
		};
		WithIndexer expected = new()
		{
			Count = 2,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).Contains("Property Count differed")
			.Because("an indexer cannot be read without an argument, so it is not a comparable member");
	}

	[Fact]
	public async Task WhenTypeIsComparedByMembersForTheCollectionElementType_ShouldCompareTheirStringMembersByValue()
	{
		List<WithNullableValue> actual = [new("ab"),];
		List<WithNullableValue> expected = [new("cd"),];
		StringBuilder failureBuilder = new();
		EquivalencyOptions options = new EquivalencyOptions()
			.For<WithNullableValue>(o => o with
			{
				ComparisonType = EquivalencyComparisonType.ByMembers,
			});

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property [0].Value differed:
		                                                       Found: "ab"
		                                                    Expected: "cd"
		                                                """).IgnoringNewlineStyle()
			.Because("the comparison type registered for the element type describes the element only, not its members");
	}

	[Fact]
	public async Task WhenTypeIsComparedByMembersForTheType_AndTheOptionsCompareByValue_ShouldCompareItsMembersByValue()
	{
		WithNestedNullableValue actual = new(new WithNullableValue("ab"));
		WithNestedNullableValue expected = new(new WithNullableValue("ab"));
		EquivalencyOptions options = new EquivalencyOptions
			{
				ComparisonType = EquivalencyComparisonType.ByValue,
			}
			.For<WithNestedNullableValue>(o => o with
			{
				ComparisonType = EquivalencyComparisonType.ByMembers,
			});

		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).StartsWith("""

		                                                   Property Inner differed:
		                                                 """).IgnoringNewlineStyle()
			.Because("the top-level comparison type applies to every member without a registration, and two separate instances are not equal by reference");
	}

	[Fact]
	public async Task WhenTypeIsComparedByMembersForTheType_ShouldCompareItsIntMemberByValue()
	{
		WithProperty actual = new(1);
		WithProperty expected = new(2);
		StringBuilder failureBuilder = new();
		EquivalencyOptions options = new EquivalencyOptions()
			.For<WithProperty>(o => o with
			{
				ComparisonType = EquivalencyComparisonType.ByMembers,
			});

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("an int has no members, so comparing it by members only because its owner is would throw");
	}

	[Fact]
	public async Task WhenTypeIsComparedByMembersForTheType_ShouldCompareItsStringMemberByValue()
	{
		WithNullableValue actual = new("ab");
		WithNullableValue expected = new("cd");
		StringBuilder failureBuilder = new();
		EquivalencyOptions options = new EquivalencyOptions()
			.For<WithNullableValue>(o => o with
			{
				ComparisonType = EquivalencyComparisonType.ByMembers,
			});

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: "ab"
		                                                    Expected: "cd"
		                                                """).IgnoringNewlineStyle()
			.Because("comparing a string by members only compares its length, which would hide the difference");
	}

	[Fact]
	public async Task WhenTypeIsComparedByMembersForTheType_ShouldCompareNestedStringMembersByValue()
	{
		WithNestedNullableValue actual = new(new WithNullableValue("ab"));
		WithNestedNullableValue expected = new(new WithNullableValue("cd"));
		StringBuilder failureBuilder = new();
		EquivalencyOptions options = new EquivalencyOptions()
			.For<WithNestedNullableValue>(o => o with
			{
				ComparisonType = EquivalencyComparisonType.ByMembers,
			});

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Inner.Value differed:
		                                                       Found: "ab"
		                                                    Expected: "cd"
		                                                """).IgnoringNewlineStyle()
			.Because("the comparison type must not reach the members of a member without a registration of its own either");
	}

	[Fact]
	public async Task WhenTypeIsRegistered_ShouldCompareTheRegisteredMembers()
	{
		RegisterPhantom();
		RegisteredProbe actual = new(1);
		RegisteredProbe expected = new(2);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).Contains("Property Phantom differed")
			.Because("the registered member is not a member reflection could find, so only the registry can report it");
	}

	[Fact]
	public async Task WhenTypeIsRegistered_WithNonPublicMembers_ShouldReflectOverTheWholeType()
	{
		RegisterPhantom();
		RegisteredProbe actual = new(1);
		RegisteredProbe expected = new(2);
		StringBuilder failureBuilder = new();
		EquivalencyOptions options = new()
		{
			Properties = IncludeMembers.Public | IncludeMembers.Internal,
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue()
			.Because("a request for non-public members bypasses the registry, and reflection does not know the phantom");
	}

	[Fact]
	public async Task WhenTypeMemberDiffers_ShouldReportTheDifference()
	{
		var actual = new
		{
			Value = typeof(int),
		};
		var expected = new
		{
			Value = typeof(long),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                       Found: int
		                                                    Expected: long
		                                                """).IgnoringNewlineStyle()
			.Because("GenericParameterPosition throws on a type that is not a generic parameter, so the walk cannot reach a difference at all");
	}

	[Fact]
	public async Task WhenTypesDifferWithoutComparableMembers_ShouldReportTheDifferenceInsteadOfThrowing()
	{
		ClassWithOnlyPrivateState actual = new(1);
		OtherClassWithOnlyPrivateState expected = new(2);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).Contains("It differed:")
			.Because("a mismatching type is a difference that can be reported without inspecting members");
	}

	[Theory]
	[InlineData("a/b", "a/c", UriKind.Relative)]
	[InlineData("https://a/b", "https://a/c", UriKind.Absolute)]
	public async Task WhenUriMemberDiffers_ShouldReportTheDifference(string actualUri, string expectedUri,
		UriKind uriKind)
	{
		var actual = new
		{
			Value = new Uri(actualUri, uriKind),
		};
		var expected = new
		{
			Value = new Uri(expectedUri, uriKind),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo($"""

		                                                   Property Value differed:
		                                                        Found: {actualUri}
		                                                     Expected: {expectedUri}
		                                                 """).IgnoringNewlineStyle()
			.Because("every component of a relative URI throws, and the components of an absolute one repeat the same difference many times over");
	}

	[Fact]
	public async Task WhenVersionMemberDiffers_ShouldStillCompareItByMembers()
	{
		var actual = new
		{
			Value = new Version(1, 2),
		};
		var expected = new
		{
			Value = new Version(1, 3),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value.Minor differed:
		                                                       Found: 2
		                                                    Expected: 3
		                                                """).IgnoringNewlineStyle()
			.Because("an ordinary class carries its state in its members, so naming the differing component stays the better message");
	}

	/// <remarks>
	///     Each call captures the <paramref name="value" /> in a closure of its own, so two delegates over the same
	///     method get separate targets.
	/// </remarks>
	private static Func<int> Capture(int value) => () => value;

	private static void RegisterAmbiguousExplicitPhantom()
	{
		TypeMetadataRegistry.RegisterExplicitProperty<RegisteredAmbiguousExplicitProbe, int>("Lib.IFirst.Phantom",
			x => x.PhantomValue());
		TypeMetadataRegistry.RegisterExplicitProperty<RegisteredAmbiguousExplicitProbe, int>("Lib.ISecond.Phantom",
			x => x.PhantomValue());
	}

	private static void RegisterExplicitPhantom()
		=> TypeMetadataRegistry.RegisterExplicitProperty<RegisteredExplicitProbe, int>("Lib.IPhantom.Phantom",
			x => x.PhantomValue());

	private static void RegisterPhantom()
		=> TypeMetadataRegistry.RegisterProperty<RegisteredProbe, int>("Phantom", x => x.PhantomValue());

	private static void RegisterPhantomField()
		=> TypeMetadataRegistry.RegisterField<RegisteredFieldProbe, int>("Phantom", x => x.PhantomValue());

	/// <remarks>
	///     Implements only the generic <see cref="IEqualityComparer{T}" />, unlike <see cref="StringComparer" />.
	/// </remarks>
	private sealed class CaseInsensitiveComparer : IEqualityComparer<string>
	{
		public bool Equals(string? x, string? y) => string.Equals(x, y, StringComparison.OrdinalIgnoreCase);

		public int GetHashCode(string obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj);
	}

	private sealed class ClassWithOnlyPrivateState(int value)
	{
		private readonly int _value = value;

		public override string ToString() => $"{nameof(ClassWithOnlyPrivateState)}({_value})";
	}

	private sealed class ClassWithPrivateStateMember(ClassWithOnlyPrivateState inner)
	{
		public ClassWithOnlyPrivateState Inner { get; } = inner;
	}

	private sealed class DerivedFromExplicitValue(int value, int other) : ExplicitValue(value, other);

	private sealed class DerivedWithAdditionalProperty(int value, int additional) : WithProperty(value)
	{
		public int Additional { get; } = additional;
	}

	private enum EnumWithFoo
	{
		Foo,
	}

	private sealed class EqualToAnything
	{
#pragma warning disable CA1822 // the comparison only reads instance members
		public int Length => 2;
#pragma warning restore CA1822

		public override bool Equals(object? obj) => true;

		public override int GetHashCode() => 0;
	}

	private sealed class ExplicitAndPublicValue(int explicitValue, int publicValue) : IHasValue
	{
		public int Value => publicValue;
		int IHasValue.Value => explicitValue;
	}

	private sealed class ExplicitGenericValue(string value) : IHasGenericValue<string>
	{
		string IHasGenericValue<string>.Value => value;
	}

	private class ExplicitValue(int value, int other) : IHasValue
	{
		public int Other => other;
		int IHasValue.Value => value;
	}

	private sealed class ExplicitValueForTwoInterfaces(int value, int otherValue) : IHasValue, IHasOtherValue
	{
		int IHasOtherValue.Value => otherValue;
		int IHasValue.Value => value;
	}

	private sealed class FieldHidingProperty(int property, int field) : WithProperty(property)
	{
		public new int Value = field;
	}

	private interface IHasGenericValue<out T>
	{
		T Value { get; }
	}

	private interface IHasOtherValue
	{
		int Value { get; }
	}

	private interface IHasValue
	{
		int Value { get; }
	}

	/// <remarks>
	///     Builds a chain of <paramref name="depth" /> nodes, so a comparison recurses exactly that many levels.
	/// </remarks>
	private sealed class NestedNode(int depth)
	{
		public NestedNode? Inner { get; set; } = depth > 1 ? new NestedNode(depth - 1) : null;
	}

	private sealed class OtherClassWithOnlyPrivateState(int value)
	{
		private readonly int _value = value;

		public override string ToString() => $"{nameof(OtherClassWithOnlyPrivateState)}({_value})";
	}

	private enum OtherEnumWithFoo
	{
		Foo,
	}

	private sealed class PropertyHidingProperty(int property, string text) : WithProperty(property)
	{
		public new string Value { get; } = text;
	}

	/// <remarks>
	///     Implements no dictionary interface besides <see cref="IReadOnlyDictionary{TKey,TValue}" />, so that the
	///     comparison cannot reach the non-generic <see cref="IDictionary" /> instead.
	/// </remarks>
	private sealed class ReadOnlyDictionaryOnly<TKey, TValue>(Dictionary<TKey, TValue> entries)
		: IReadOnlyDictionary<TKey, TValue>
		where TKey : notnull
	{
		public int Count => entries.Count;
		public IEnumerable<TKey> Keys => entries.Keys;
		public IEnumerable<TValue> Values => entries.Values;
		public TValue this[TKey key] => entries[key];
		public bool ContainsKey(TKey key) => entries.ContainsKey(key);
		public bool TryGetValue(TKey key, out TValue value) => entries.TryGetValue(key, out value!);
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => entries.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	/// <remarks>
	///     Like <see cref="ReadOnlyDictionaryOnly{TKey,TValue}" />, but exposes the comparer of its entries.
	/// </remarks>
	private sealed class ReadOnlyDictionaryOnlyWithComparer<TKey, TValue>(Dictionary<TKey, TValue> entries)
		: IReadOnlyDictionary<TKey, TValue>
		where TKey : notnull
	{
		public IEqualityComparer<TKey> Comparer => entries.Comparer;
		public int Count => entries.Count;
		public IEnumerable<TKey> Keys => entries.Keys;
		public IEnumerable<TValue> Values => entries.Values;
		public TValue this[TKey key] => entries[key];
		public bool ContainsKey(TKey key) => entries.ContainsKey(key);
		public bool TryGetValue(TKey key, out TValue value) => entries.TryGetValue(key, out value!);
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => entries.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

#if NET8_0_OR_GREATER
	/// <remarks>
	///     Implements no set interface besides <see cref="IReadOnlySet{T}" />, so that the comparison cannot reach
	///     <see cref="ISet{T}" /> instead.
	/// </remarks>
	private sealed class ReadOnlySetOnly<T>(HashSet<T> items) : IReadOnlySet<T>
	{
		public int Count => items.Count;
		public bool Contains(T item) => items.Contains(item);
		public bool IsProperSubsetOf(IEnumerable<T> other) => items.IsProperSubsetOf(other);
		public bool IsProperSupersetOf(IEnumerable<T> other) => items.IsProperSupersetOf(other);
		public bool IsSubsetOf(IEnumerable<T> other) => items.IsSubsetOf(other);
		public bool IsSupersetOf(IEnumerable<T> other) => items.IsSupersetOf(other);
		public bool Overlaps(IEnumerable<T> other) => items.Overlaps(other);
		public bool SetEquals(IEnumerable<T> other) => items.SetEquals(other);
		public IEnumerator<T> GetEnumerator() => items.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
#endif

	private sealed class RegisteredAmbiguousExplicitProbe(int phantom)
	{
		public int PhantomValue() => phantom;
	}

	private sealed class RegisteredExplicitProbe(int phantom)
	{
		public int PhantomValue() => phantom;
	}

	private sealed class RegisteredFieldProbe(int phantom)
	{
		public int PhantomValue() => phantom;
	}

	private sealed class RegisteredProbe(int phantom)
	{
		public int Visible { get; set; }

		public int PhantomValue() => phantom;
	}

	private sealed class ValueLikeWithConstantText(int value)
	{
		private readonly int _value = value;

		public override bool Equals(object? obj)
			=> obj is ValueLikeWithConstantText other && other._value == _value;

		public override int GetHashCode() => _value;

		public override string ToString() => nameof(ValueLikeWithConstantText);
	}

	private sealed class ValueLikeWithoutMembers(int value)
	{
		private readonly int _value = value;

		public override bool Equals(object? obj)
			=> obj is ValueLikeWithoutMembers other && other._value == _value;

		public override int GetHashCode() => _value;

		public override string ToString() => $"{nameof(ValueLikeWithoutMembers)}({_value})";
	}

	private sealed class ValueObject(int value)
	{
		public int Value => value;
	}

	private sealed class WithIndexer
	{
		public int Count { get; set; }
		public int this[int index] => index;
	}

	private sealed class WithInternalValue(int value)
	{
		internal int Value = value;
	}

	private sealed class WithLength(int length)
	{
		public int Length => length;
	}

	private sealed class WithNestedNullableValue(WithNullableValue inner)
	{
		public WithNullableValue Inner { get; } = inner;
	}

	private sealed class WithNullableValue(string? value)
	{
		public string? Value { get; } = value;
	}

	private sealed class WithPrivateGetter(int value)
	{
		public int Value { private get; set; } = value;
		public int Other { get; set; }

		public override string ToString() => $"{Value}";
	}

	private class WithProperty(int value)
	{
		public int Value => value;
	}

	private sealed class WithPublicGetter(int value)
	{
		public int Value { get; set; } = value;
		public int Other { get; set; }
	}

	private sealed class WithPublicValue(int value)
	{
		public int Value = value;
	}

	private sealed class WithThrowingEquals
	{
#pragma warning disable CA1822 // the comparison only reads instance members
		public int Value => 1;
#pragma warning restore CA1822

		public override bool Equals(object? obj) => throw new NotSupportedException("equals");

		public override int GetHashCode() => 0;
	}

	private sealed class WithThrowingGetter(string message)
	{
		public int Value => throw new InvalidOperationException(message);
	}

	private sealed class WithTwoPublicValues(int value, int other)
	{
		public int Other = other;
		public int Value = value;
	}
}
