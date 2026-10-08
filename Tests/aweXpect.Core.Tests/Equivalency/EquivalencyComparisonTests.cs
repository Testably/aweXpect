using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using aweXpect.Core.Constraints;
using aweXpect.Core.Metadata;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Customization;
using aweXpect.Equivalency;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

namespace aweXpect.Core.Tests.Equivalency;

public sealed partial class EquivalencyComparisonTests
{
	[Test]
	public async Task WhenActualFieldIsInternal_WithInternalFields_ShouldCompareIt()
	{
		WithInternalValue actual = new(2);
		var expected = new
		{
			Value = 1,
		};
		StringBuilder failureBuilder = new();
		EquivalencyOptions options = new()
		{
			Fields = IncludeMembers.Public | IncludeMembers.Internal,
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                      Actual: 2
		                                                    Expected: 1
		                                                """).IgnoringNewlineStyle()
			.Because("the expected property falls back to the internal field of the same name on the actual object");
	}

	[Test]
	public async Task WhenActualFieldIsPrivate_WithInternalFields_ShouldTreatItAsMissing()
	{
		WithPrivateField actual = new(2);
		var expected = new
		{
			Value = 1,
		};
		StringBuilder failureBuilder = new();
		EquivalencyOptions options = new()
		{
			Fields = IncludeMembers.Public | IncludeMembers.Internal,
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value was missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("private members are never compared, also not on the actual object");
	}

	[Test]
	public async Task WhenActualImplementsAByRefLikePropertyExplicitly_ShouldTreatItAsMissing()
	{
		ExplicitSpan actual = new(1);
		var expected = new
		{
			Values = new[]
			{
				1,
			},
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Values was missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("a span cannot be boxed, so reflection cannot read the explicit implementation");
	}

	[Test]
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
		                                                      Actual: 1
		                                                    Expected: 5
		                                                """).IgnoringNewlineStyle()
			.Because("the explicit implementation is only a fallback for a property the actual type does not have");
	}

	[Test]
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
		                                                      Actual: 5
		                                                    Expected: 6
		                                                """).IgnoringNewlineStyle()
			.Because("the explicit implementation is reported under the short name the expectation uses");
	}

	[Test]
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
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("the registered explicit implementation is not one reflection could find, and a type with only explicit implementations still counts as registered");
	}

	[Test]
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

	[Test]
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

		                                                  Property Phantom was ambiguous on the actual object, which implements it explicitly for more than one interface
		                                                """).IgnoringNewlineStyle()
			.Because("the registry has to decide the ambiguity the same way reflection does");
	}

	[Test]
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

		                                                  Property Value was ambiguous on the actual object, which implements it explicitly for more than one interface
		                                                """).IgnoringNewlineStyle()
			.Because("picking one of the implementations would make the result depend on the order reflection returns them in");
	}

	[Test]
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
		                                                      Actual: 5
		                                                    Expected: 6
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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

	[Test]
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
		                                                      Actual: 5
		                                                    Expected: 6
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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

	[Test]
	public async Task WhenActualImplementsAPropertyExplicitly_WithAnExpectedField_ShouldCompareIt()
	{
		ExplicitValue actual = new(5, 1);
		WithPublicValue expected = new(6);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Field Value differed:
		                                                      Actual: 5
		                                                    Expected: 6
		                                                """).IgnoringNewlineStyle()
			.Because("an expected field falls back to a property of the same name, which includes an explicit implementation");
	}

	[Test]
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

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that actual
			             is not equivalent to unexpected,
			             but it was EquivalencyComparisonTests.ExplicitValue {
			                 Other = 1
			               }, which is considered equivalent

			             Equivalency options:
			              - include public fields and properties
			             """).Because("the explicit implementation makes the actual object equivalent, even though the formatter does not list it");
	}

	[Test]
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
			Properties = IncludeMembers.Public | IncludeMembers.Internal,
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, options, new StringBuilder());

		await That(result).IsTrue()
			.Because("requesting non-public members still finds the explicit implementation by its short name");
	}

	[Test]
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

		                                                  Field Value was missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("an explicit implementation is a property, so excluding properties excludes it as well");
	}

	[Test]
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
		                                                      Actual: EquivalencyComparisonTests.WithLength { Length = 2 }
		                                                    Expected: "ab"
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenActualIsComparedByMembers_AndItsEqualsAcceptsTheExpectedValue_ShouldLetTheExpectedValueDecide()
	{
		EqualToAnything actual = new();
		string expected = "ab";

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), new StringBuilder());

		await That(result).IsFalse()
			.Because("a type compared by members has its Equals ignored, so only the Equals of the value compared by value may decide");
	}

	[Test]
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

	[Test]
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
		                                                      Actual: <null>
		                                                    Expected: "Foo"
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenActualPropertyHasNoPublicGetter_ShouldTreatItAsMissing()
	{
		WithPrivateGetter actual = new(1);
		WithPublicGetter expected = new(1);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value was missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("a registration cannot call a non-public getter, so reflection must not read one either");
	}

	[Test]
	public async Task WhenActualPropertyIsPrivate_WithInternalProperties_ShouldTreatItAsMissing()
	{
		WithPrivateProperty actual = new(2);
		var expected = new
		{
			Value = 1,
		};
		StringBuilder failureBuilder = new();
		EquivalencyOptions options = new()
		{
			Properties = IncludeMembers.Public | IncludeMembers.Internal,
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value was missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("private members are never compared, also not on the actual object");
	}

	[Test]
	public async Task WhenActualTypeHasAFieldAndAPropertyOfTheSameName_WithAnExpectedField_ShouldUseTheField()
	{
		FieldHidingProperty actual = new(99, 1);
		WithPublicValue expected = new(2);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Field Value differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("the member of the same kind takes precedence, so the fallback to the property never applies");
	}

	[Test]
	public async Task WhenActualTypeHasAFieldAndAPropertyOfTheSameName_WithAnExpectedProperty_ShouldUseTheProperty()
	{
		FieldHidingProperty actual = new(1, 99);
		WithProperty expected = new(2);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("the member of the same kind takes precedence, so the fallback to the field never applies");
	}

	[Test]
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

	[Test]
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

	[Test]
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
		                                                       Actual: {actual.Value}
		                                                     Expected: {expected.Value}
		                                                 """).IgnoringNewlineStyle()
			.Because("an assembly only describes what it loaded, so walking it reaches getters that throw instead of state that could be compared");
	}

	[Test]
	public async Task WhenBigIntegerMemberDiffers_ShouldReportTheDifference()
	{
		var actual = new
		{
			Value = new BigInteger(3),
		};
		var expected = new
		{
			Value = new BigInteger(5),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                      Actual: 3
		                                                    Expected: 5
		                                                """).IgnoringNewlineStyle()
			.Because("the members of a BigInteger (IsZero, IsEven, Sign, ...) cannot tell 3 and 5 apart");
	}

	[Test]
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

	[Test]
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
		                                                      Actual: ['a']
		                                                    Expected: "a"
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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
		                                                      Actual: 2
		                                                    Expected: 5
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenCollectionElementMatchesAScopedIgnoreRule_ShouldStillCompareIt()
	{
		int[] actual = [1,];
		int[] expected = [2,];
		EquivalencyOptions options = new()
		{
			MembersToIgnore =
			[
				new MemberToIgnore.ByFieldPredicate((_, _) => true, "all fields"),
				new MemberToIgnore.ByPropertyPredicate((_, _) => true, "all properties"),
			],
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse()
			.Because("a collection element is neither a field nor a property, so a rule scoped to either never applies to it");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [0] differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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
		                                                      Actual: 1 (int)
		                                                    Expected: 1 (long)
		                                                """).IgnoringNewlineStyle()
			.Because("two elements that format identically are only told apart by their type");
	}

	[Test]
	public async Task WhenCollectionHasFewerElements_AndTheMissingElementIsIgnored_ShouldSucceed()
	{
		object?[] actual = [1,];
		object?[] expected = [1, null,];
		EquivalencyOptions options = new()
		{
			MembersToIgnore = [new MemberToIgnore.ByName("[1]"),],
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue()
			.Because("an ignored element is not reported as missing");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenCollectionHasFewerElements_ShouldReportTheMissingElements()
	{
		int[] actual = [1,];
		int[] expected = [1, 2, 3,];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [1] was missing 2 and
		                                                  Element [2] was missing 3
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenCollectionHasMoreElements_AndTheSuperfluousElementIsIgnored_ShouldSucceed()
	{
		object?[] actual = [null, null,];
		object?[] expected = [null,];
		EquivalencyOptions options = new()
		{
			MembersToIgnore = [new MemberToIgnore.ByName("[1]"),],
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue()
			.Because("an ignored element is not reported as superfluous");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenCollectionHasMoreElements_ShouldReportTheSuperfluousElements()
	{
		int[] actual = [1, 2, 3,];
		int[] expected = [1,];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [1] had superfluous 2 and
		                                                  Element [2] had superfluous 3
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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

	[Test]
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

	[Test]
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

		                                                  Element [0] was missing 1 and
		                                                  Element [1] was missing 2
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenCollectionOrderIsIgnored_AndADictionaryElementHasAnUnmatchedKey_ShouldReportIt()
	{
		object[] actual =
		[
			new Hashtable(StringComparer.OrdinalIgnoreCase)
			{
				["A"] = 1,
			},
		];
		object[] expected =
		[
			new Dictionary<string, int>
			{
				["a"] = 1,
			},
		];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [0][A] matched no expected key
		                                                """).IgnoringNewlineStyle()
			.Because("the key is only written for the leftover pair, not while the elements are matched");
	}

	[Test]
	public async Task WhenCollectionOrderIsIgnored_AndADictionaryElementLacksADistinctKey_ShouldReportIt()
	{
		object[] actual =
		[
			new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
			{
				["a"] = 1,
			},
		];
		object[] expected =
		[
			new Dictionary<string, int>
			{
				["a"] = 1,
				["A"] = 1,
			},
		];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [0][A] lacked a distinct key
		                                                """).IgnoringNewlineStyle()
			.Because("the key is only written for the leftover pair, not while the elements are matched");
	}

	[Test]
	public async Task WhenCollectionOrderIsIgnored_AndAnArrayElementHasOtherDimensions_ShouldReportThem()
	{
		int[][,] actual = [new int[1, 2],];
		int[][,] expected = [new int[2, 1],];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [0] had dimensions [1,2] instead of [2,1]
		                                                """).IgnoringNewlineStyle()
			.Because("the dimensions are only written for the leftover pair, not while the elements are matched");
	}

	[Test]
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

	[Test]
	public async Task WhenCollectionOrderIsIgnored_AndAnElementIsMissingAMember_ShouldReportIt()
	{
		object[] actual = [new WithPublicValue(1),];
		object[] expected =
		[
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

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property [0].Other was missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("the missing member is only written for the leftover pair, not while the elements are matched");
	}

	[Test]
	public async Task WhenCollectionOrderIsIgnored_AndAnElementIsNull_WithAnIgnoreRule_ShouldMatchIt()
	{
		object?[] actual = [1, null,];
		object?[] expected = [null, 1,];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
			MembersToIgnore = [new MemberToIgnore.ByName("Unused"),],
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenCollectionOrderIsIgnored_AndANestedCollectionHasASuperfluousElement_ShouldReportIt()
	{
		int[][] actual = [[1, 2,],];
		int[][] expected = [[1,],];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [0][1] had superfluous 2
		                                                """).IgnoringNewlineStyle()
			.Because("the superfluous element is only written for the leftover pair, not while the elements are matched or ranked");
	}

	[Test]
	public async Task WhenCollectionOrderIsIgnored_AndANestedCollectionMissesAnElement_ShouldReportIt()
	{
		int[][] actual = [[1,],];
		int[][] expected = [[1, 2,],];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [0][1] was missing 2
		                                                """).IgnoringNewlineStyle()
			.Because("the missing element is only written for the leftover pair, not while the elements are matched or ranked");
	}

	[Test]
	public async Task WhenCollectionOrderIsIgnored_AndAnItIsMemberDoesNotMatch_ShouldReportIt()
	{
		object[] actual =
		[
			new
			{
				Value = 1,
			},
		];
		object[] expected =
		[
			new
			{
				Value = It.Is<int>().That.IsGreaterThan(2),
			},
		];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property [0].Value differed:
		                                                      Actual: 1
		                                                    Expected: is int that is greater than 2
		                                                """).IgnoringNewlineStyle()
			.Because("the expectation is only written for the leftover pair, not while the elements are matched");
	}

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
	public async Task WhenCollectionOrderIsIgnored_AndElementsAreReversedWithDuplicates_ShouldReportTheDifference()
	{
		WithTwoPublicValues[] actual = [new(1, 10), new(2, 20), new(2, 20), new(3, 30),];
		WithTwoPublicValues[] expected = [new(3, 30), new(2, 20), new(2, 21), new(1, 10),];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Field [1].Other differed:
		                                                      Actual: 20
		                                                    Expected: 21
		                                                """).IgnoringNewlineStyle()
			.Because("the neighbour of the previous match is tried first, so the later of the two equal actual elements is matched and the earlier one is left over");
	}

	[Test]
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

	[Test]
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

		                                                  Element [0] had superfluous 1 and
		                                                  Element [1] had superfluous 2
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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
		                                                      Actual: 10
		                                                    Expected: 88
		                                                and
		                                                  Field [1].Other differed:
		                                                      Actual: 20
		                                                    Expected: 99
		                                                """).IgnoringNewlineStyle()
			.Because("pairing each element with the one that shares its Value reports the one member that differs, while pairing them by position would report both members of both elements");
	}

	[Test]
	public async Task WhenCollectionOrderIsIgnored_AndManyPairsAreCompared_ShouldMatchAllElements()
	{
		int[] actual = Enumerable.Range(1, 40).Select(i => i * 17 % 41).ToArray();
		int[] expected = Enumerable.Range(1, 40).ToArray();
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue()
			.Because("a shuffled collection compares enough pairs that their results are moved into a table of all pairs, which must keep them");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
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
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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

	[Test]
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
		                                                      Actual: 1
		                                                    Expected: 3
		                                                and
		                                                  Element [1] differed:
		                                                      Actual: 2
		                                                    Expected: 4
		                                                and
		                                                  Element [2] was missing 5
		                                                """).IgnoringNewlineStyle()
			.Because("the expected elements that a leftover could be paired with are reported as differences in the order of the actual elements, and only the surplus that no actual element is left for is reported as missing");
	}

	[Test]
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
		                                                      Actual: EquivalencyComparisonTests.WithLength { Length = 2 }
		                                                    Expected: "ab"
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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

	[Test]
	public async Task WhenCollectionOrderIsIgnored_AndRegisteredValueMembersDiffer_ShouldReportTheLeftoverPair()
	{
		RegisterValues();
		RegisteredValuesProbe[] actual = [new(1, 1.5, DayOfWeek.Monday), new(2, 2.5, DayOfWeek.Friday),];
		RegisteredValuesProbe[] expected = [new(2, 2.5, DayOfWeek.Friday), new(1, 1.5, DayOfWeek.Sunday),];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property [0].Day differed:
		                                                      Actual: Monday
		                                                    Expected: Sunday
		                                                """).IgnoringNewlineStyle()
			.Because("the registered value members decide the pairs without being read as objects, but are reported like any other member");
	}

	[Test]
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

	[Test]
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
				1,
				2,
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
				2,
				1,
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
		                                                      Actual: 1
		                                                    Expected: 2
		                                                and
		                                                  Element Ordered[1] differed:
		                                                      Actual: 2
		                                                    Expected: 1
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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

	[Test]
	public async Task WhenComparedRepeatedlyWithTheSameOptions_ShouldResolveTheRegistrationOnce()
	{
		int resolutions = 0;
		EquivalencyOptions options = new EquivalencyOptions().For<WithProperty>(typeOptions =>
		{
			resolutions++;
			return typeOptions;
		});

		bool first = await EquivalencyComparison.Compare(new WithProperty(1), new WithProperty(1), options,
			new StringBuilder());
		bool second = await EquivalencyComparison.Compare(new WithProperty(2), new WithProperty(3), options,
			new StringBuilder());

		await That(first).IsTrue();
		await That(second).IsFalse();
		await That(resolutions).IsEqualTo(1)
			.Because("the registered options are cached per options instance, e.g. for the items of a collection");
	}

	[Test]
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
		                                                      Actual: de-DE
		                                                    Expected: en-US
		                                                """).IgnoringNewlineStyle()
			.Because("the culture name is the identity, while its members expand into every format pattern the operating system knows");
	}

#if NET8_0_OR_GREATER
	[Test]
	public async Task WhenDateOnlyMemberDiffers_ShouldReportTheDifference()
	{
		var actual = new
		{
			Born = new DateOnly(2020, 1, 1),
		};
		var expected = new
		{
			Born = new DateOnly(2020, 1, 2),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Born differed:
		                                                      Actual: 2020-01-01
		                                                    Expected: 2020-01-02
		                                                """).IgnoringNewlineStyle()
			.Because("the members of a date only repeat the same difference in several forms");
	}
#endif

	[Test]
	public async Task WhenDateTimeMemberHasAnIncompatibleKind_ShouldReportTheDifference()
	{
		var actual = new
		{
			Value = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc),
		};
		var expected = new
		{
			Value = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Local),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse()
			.Because("a UTC and a local time with the same ticks describe different instants, although their Equals ignores the kind");
		await That(failureBuilder.ToString()).IsEqualTo($$"""

		                                                  Property Value differed:
		                                                      Actual: 2024-01-02T03:04:05.0000000Z
		                                                    Expected: {{expected.Value:o}}
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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
		                                                      Actual: Func<string, bool> { Method = Boolean IsNullOrEmpty(System.String), Target = <null> }
		                                                    Expected: Func<string, bool> { Method = Boolean IsNullOrWhiteSpace(System.String), Target = <null> }
		                                                """).IgnoringNewlineStyle()
			.Because("a delegate is its target and method, so walking it would drag a captured closure into the comparison");
	}

	[Test]
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

	[Test]
	public async Task WhenDictionaryHasAnAdditionalAndAMissingKeyWithNullValues_ShouldReportBoth()
	{
		Dictionary<string, object?> actual = new()
		{
			["a"] = null,
		};
		Dictionary<string, object?> expected = new()
		{
			["b"] = null,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [a] had superfluous <null> and
		                                                  Element [b] was missing <null>
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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

		                                                  Element [b] had superfluous 1 and
		                                                  Element [A] lacked a distinct key
		                                                """).IgnoringNewlineStyle()
			.Because("a read-only dictionary looks its keys up through the dictionary it wraps");
	}

	[Test]
	public async Task WhenDictionaryIsASortedDictionary_AndExpectedKeysHaveMixedTypes_ShouldReportTheMissingKey()
	{
		SortedDictionary<int, int> actual = new()
		{
			[1] = 1,
		};
		Dictionary<object, int> expected = new()
		{
			[1] = 1,
			["a"] = 2,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [a] was missing 2
		                                                """).IgnoringNewlineStyle()
			.Because("an expected key of another type than the keys of the sorted dictionary can never be among its matched keys");
	}

	[Test]
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

		                                                  Element [b] had superfluous 1 and
		                                                  Element [A] lacked a distinct key
		                                                """).IgnoringNewlineStyle()
			.Because("a sorted dictionary considers two keys the same when its comparer orders neither before the other");
	}

	[Test]
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

	[Test]
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
		                                                      Actual: 1
		                                                    Expected: 11
		                                                """).IgnoringNewlineStyle()
			.Because("the bracket inside the key does not open the path segment that the ignored name refers to");
	}

	[Test]
	public async Task WhenDictionaryKeyIsCultureDependent_ShouldFormatItInvariantly()
	{
		using CultureOverride _ = new("de-DE");
		Dictionary<double, int> actual = new()
		{
			[1.5] = 1,
			[2.5] = 1,
		};
		Dictionary<double, int> expected = new()
		{
			[1.5] = 2,
			[2.5] = 2,
		};
		StringBuilder failureBuilder = new();
		EquivalencyOptions options = new()
		{
			MembersToIgnore = [new MemberToIgnore.ByName("[2.5]"),],
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [1.5] differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("a member path to ignore has to match on every machine, whatever its culture");
	}

	[Test]
	public async Task WhenDictionaryOnlyImplementsTheGenericInterface_AndAnEntryHasANullKey_ShouldCompareItAsASequence()
	{
		ReadOnlyDictionaryWithEntries actual = new(new KeyValuePair<string, int>("a", 1),
			new KeyValuePair<string, int>(null!, 2));
		object[] expected = [new KeyValuePair<string, int>("a", 1), new KeyValuePair<string, int>(null!, 2),];

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), new StringBuilder());

		await That(result).IsTrue()
			.Because("a dictionary cannot hold a null key, so the entries are compared as the items of a sequence");
	}

	[Test]
	public async Task WhenDictionaryOnlyImplementsTheGenericInterface_AndAnEntryHasNoKey_ShouldCompareItAsASequence()
	{
		ReadOnlyDictionaryWithEntries actual = new(1, 2);
		int[] expected = [1, 2,];

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), new StringBuilder());

		await That(result).IsTrue()
			.Because("an entry without a key cannot be copied into a dictionary, so the entries are compared as the items of a sequence");
	}

	[Test]
	public async Task WhenDictionaryOnlyImplementsTheGenericInterface_AndAnEntryHasNoValue_ShouldCompareItAsASequence()
	{
		ReadOnlyDictionaryWithEntries actual = new(new KeyOnly("a"));
		object[] expected = [new KeyOnly("a"),];

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), new StringBuilder());

		await That(result).IsTrue()
			.Because("an entry without a value cannot be copied into a dictionary, so the entries are compared as the items of a sequence");
	}

	[Test]
	public async Task WhenDictionaryOnlyImplementsTheGenericInterface_AndAnEntryIsNull_ShouldCompareItAsASequence()
	{
		ReadOnlyDictionaryWithEntries actual = new([null,]);
		object?[] expected = [null,];

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), new StringBuilder());

		await That(result).IsTrue()
			.Because("a null entry cannot be copied into a dictionary, so the entries are compared as the items of a sequence");
	}

	[Test]
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

	[Test]
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

	[Test]
	public async Task WhenDictionaryOnlyImplementsTheGenericInterface_AndSubjectExposesAComparerProperty_ShouldCompareTheKeysByTheirEquality()
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

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [a] had superfluous 1 and
		                                                  Element [A] was missing 1
		                                                """).IgnoringNewlineStyle()
			.Because("the comparer is only read from known dictionary types, as for IsEqualTo and under Native AOT");
	}

	[Test]
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

		                                                  Element [a] had superfluous 1 and
		                                                  Element [A] was missing 1
		                                                """).IgnoringNewlineStyle()
			.Because("a comparer that cannot be read cannot be honoured by the copy of the entries");
	}

	[Test]
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
		                                                      Actual: 2
		                                                    Expected: 3
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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

	[Test]
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
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

		                                                  Element [b] had superfluous 1 and
		                                                  Element [A] lacked a distinct key
		                                                """).IgnoringNewlineStyle()
			.Because("the collapsed expected key counts only once, so the entry count does not hide the leftover key");
	}

	[Test]
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

		                                                  Element [a] had superfluous 1 and
		                                                  Element [A] was missing 1
		                                                """).IgnoringNewlineStyle()
			.Because("the key comparer of the expected dictionary does not decide which keys the subject has");
	}

	[Test]
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

	[Test]
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

		                                                  Element [b] had superfluous 1 and
		                                                  Element [A] lacked a distinct key
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenDictionarySubjectUsesAnUnreadableComparer_WithADifferentlyCasedKey_ShouldReportItAsUnmatched()
	{
		Hashtable actual = new(StringComparer.OrdinalIgnoreCase)
		{
			["A"] = 1,
		};
		Dictionary<string, int> expected = new()
		{
			["a"] = 1,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [A] matched no expected key
		                                                """).IgnoringNewlineStyle()
			.Because("without the comparer of the hashtable, the key \"A\" cannot be told apart from a superfluous key");
	}

	[Test]
	public async Task WhenDictionarySubjectUsesAnUnreadableComparer_WithOneMatchedKeyOfSeveral_ShouldReportTheKeyCounts()
	{
		Hashtable actual = new(StringComparer.OrdinalIgnoreCase)
		{
			["a"] = 1,
			["b"] = 1,
			["c"] = 1,
		};
		Dictionary<string, int> expected = new()
		{
			["A"] = 1,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  It contained 3 keys and matched 1 expected key
		                                                """).IgnoringNewlineStyle()
			.Because("without the comparer of the hashtable, the matched key \"a\" cannot be told apart from the superfluous ones");
	}

	[Test]
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

	[Test]
	public async Task WhenDictionarySubjectUsesAnUnreadableComparer_WithTwoKeysThatOnlyItUnifiesAndAnAdditionalKey_ShouldReportTheUnmatchedKey()
	{
		Hashtable actual = new(StringComparer.OrdinalIgnoreCase)
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

		                                                  Element [b] matched no expected key
		                                                """).IgnoringNewlineStyle()
			.Because("without the comparer of the hashtable, the two expected keys must not hide the key \"b\"");
	}

	[Test]
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
		                                                      Actual: 2
		                                                    Expected: 3
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenDictionaryValuesAreNull_ShouldSucceed()
	{
		Dictionary<string, object?> actual = new()
		{
			["a"] = null,
		};
		Dictionary<string, object?> expected = new()
		{
			["a"] = null,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenEncodingMemberDiffers_ShouldReportTheDifference()
	{
		var actual = new
		{
			Value = new UTF8Encoding(true),
		};
		var expected = new
		{
			Value = new UTF8Encoding(false),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse()
			.Because("the encodings differ in whether they emit a byte order mark, which their Equals compares");
		await That(failureBuilder.ToString()).StartsWith("""

		                                                   Property Value differed:
		                                                 """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenEncodingMembersAreEqual_ShouldSucceed()
	{
		var actual = new
		{
			Value = new UTF8Encoding(false),
		};
		var expected = new
		{
			Value = new UTF8Encoding(false),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("an encoding is compared by its Equals, as its preamble is a span that reflection cannot read");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenEnumeratingAMemberThrows_ShouldFailWithTheException()
	{
		var actual = new
		{
			Items = new[]
			{
				1,
			}.Select<int, int>(_ => throw new InvalidOperationException("enumeration failed")),
		};
		var expected = new
		{
			Items = new[]
			{
				1,
			},
		};

		async Task Act()
			=> await That(actual).IsEquivalentTo(expected);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that actual
			             is equivalent to expected,
			             but Items did throw an InvalidOperationException:
			               enumeration failed

			             Equivalency options:
			              - include public fields and properties
			             """)
			.Because("enumerating a member runs code of the caller, just like its getter");
	}

	[Test]
	public async Task WhenEnumeratingAMemberThrows_WhenNegated_ShouldFailWithTheException()
	{
		var actual = new
		{
			Items = new[]
			{
				1,
			}.Select<int, int>(_ => throw new InvalidOperationException("enumeration failed")),
		};
		var unexpected = new
		{
			Items = new[]
			{
				1,
			},
		};

		async Task Act()
			=> await That(actual).IsNotEquivalentTo(unexpected);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that actual
			             is not equivalent to unexpected,
			             but Items did throw an InvalidOperationException:
			               enumeration failed

			             Equivalency options:
			              - include public fields and properties
			             """)
			.Because("an enumeration that threw answered nothing, so the negation fails as well");
	}

	[Test]
	public async Task WhenEnumeratingTheSubjectThrows_ShouldFailWithTheException()
	{
		object subject = new[]
		{
			1,
		}.Select<int, int>(_ => throw new InvalidOperationException("enumeration failed"));
		int[] expected = [1,];

		async Task Act()
			=> await That(subject).IsEquivalentTo(expected);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is equivalent to expected,
			             but it did throw an InvalidOperationException:
			               enumeration failed

			             Equivalency options:
			              - include public fields and properties
			             """);
	}

	[Test]
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
		                                                      Actual: Foo (EquivalencyComparisonTests.EnumWithFoo)
		                                                    Expected: Foo (EquivalencyComparisonTests.OtherEnumWithFoo)
		                                                """).IgnoringNewlineStyle()
			.Because("the rule is about values that cannot be told apart, not about numeric types");
	}

	[Test]
	public async Task WhenExpectedFieldIsMissingOnTheActualType_ShouldReportItAsMissing()
	{
		WithPublicValue actual = new(1);
		WithTwoPublicValues expected = new(1, 2);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Field Other was missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("a field is reported as missing just like a property");
	}

	[Test]
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

	[Test]
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

		                                                  Field Value was missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("the fallback may only reach a kind that the caller included");
	}

	[Test]
	public async Task WhenExpectedFieldMatchesAnActualProperty_WhenTheyDiffer_ShouldReportItAsAField()
	{
		WithProperty actual = new(1);
		WithPublicValue expected = new(2);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Field Value differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("the compared members come from the expected object, so its kind names the difference and agrees with the kind a scoped ignore rule applies to");
	}

	[Test]
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
		                                                      Actual: EquivalencyComparisonTests.WithLength { Length = 2 }
		                                                    Expected: "ab"
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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

	[Test]
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
		                                                      Actual: 1
		                                                    Expected: <null>
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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

		                                                  Property B was missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("the actual object never had the member, so claiming that it was found as <null> would be wrong");
	}

	[Test]
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

		                                                  Property B was missing on the actual object
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("the fallback applies at every level of the graph, and the path stays the one of the expected member");
	}

	[Test]
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

	[Test]
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

		                                                  Property Value was missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("the fallback may only reach a kind that the caller included");
	}

	[Test]
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

	[Test]
	public async Task WhenExpectedPropertyMatchesAnActualField_WhenTheyDiffer_ShouldReportItAsAProperty()
	{
		WithPublicValue actual = new(1);
		WithProperty expected = new(2);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("the compared members come from the expected object, so its kind names the difference and agrees with the kind a scoped ignore rule applies to");
	}

	[Test]
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
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("a registration keeps the kind of the member, so the fallback has to cross it in the registry as well");
	}

	[Test]
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

	[Test]
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

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that actual
			             is equivalent to expected,
			             but Equals of EquivalencyComparisonTests.WithThrowingEquals did throw a NotSupportedException:
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

	[Test]
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

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that actual
			             is not equivalent to unexpected,
			             but Equals of EquivalencyComparisonTests.WithThrowingEquals did throw a NotSupportedException:
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

	[Test]
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

	[Test]
	public async Task WhenExpectedTypeIsDerivedFromTheActualType_ShouldReportTheAdditionalMemberAsMissing()
	{
		WithProperty actual = new(1);
		WithProperty expected = new DerivedWithAdditionalProperty(1, 5);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Additional was missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("the members are taken from the runtime type of the expected object, which the actual type does not have");
	}

	[Test]
	public async Task WhenFailureBuilderIsNotEmpty_ShouldSeparateOnlyTheDifferences()
	{
		var actual = new
		{
			A = 1,
			B = 1,
		};
		var expected = new
		{
			A = 2,
			B = 2,
		};
		StringBuilder failureBuilder = new("Differences:");

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""
		                                                Differences:
		                                                  Property A differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                and
		                                                  Property B differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("the content of the failure builder is no difference that the first one is separated from");
	}

	[Test]
	public async Task WhenFailureBuilderIsNotEmpty_WithSingleLineDifferences_ShouldSeparateOnlyTheDifferences()
	{
		var actual = new
		{
			A = 1,
		};
		var expected = new
		{
			B = 1,
			C = 1,
		};
		StringBuilder failureBuilder = new("Differences:");

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""
		                                                Differences:
		                                                  Property B was missing on the actual object and
		                                                  Property C was missing on the actual object
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	[Arguments(1, 3, 2, 3, "Property Value differed")]
	[Arguments(1, 3, 1, 4, "Field Value differed")]
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

	[Test]
	public async Task WhenGetterThrows_ShouldFailWithTheGetterException()
	{
		WithThrowingGetter actual = new("getter failed");
		WithThrowingGetter expected = new("getter failed");

		async Task Act()
			=> await That(actual).IsEquivalentTo(expected);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that actual
			             is equivalent to expected,
			             but Value did throw an InvalidOperationException:
			               getter failed

			             Equivalency options:
			              - include public fields and properties
			             """).And
			.Whose(e => e.InnerException, i => i.Is<InvalidOperationException>())
			.Because("reflection wraps the exception, while a registered accessor lets it through, so both paths have to agree");
	}

	[Test]
	public async Task WhenGetterThrows_WhenNegated_ShouldFailWithTheGetterException()
	{
		WithThrowingGetter actual = new("getter failed");
		WithThrowingGetter unexpected = new("getter failed");

		async Task Act()
			=> await That(actual).IsNotEquivalentTo(unexpected);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that actual
			             is not equivalent to unexpected,
			             but Value did throw an InvalidOperationException:
			               getter failed

			             Equivalency options:
			              - include public fields and properties
			             """).And
			.Whose(e => e.InnerException, i => i.Is<InvalidOperationException>())
			.Because("a getter that threw answered nothing, so the negation fails as well");
	}

	[Test]
	public async Task WhenGetterThrows_WhenReflected_ShouldThrowTheGetterException()
	{
		WithThrowingGetter actual = new("getter failed");
		WithThrowingGetter expected = new("getter failed");
		EquivalencyOptions options = new()
		{
			Properties = IncludeMembers.Public | IncludeMembers.Internal,
		};

		async Task Act()
			=> await EquivalencyComparison.Compare(actual, expected, options, new StringBuilder());

		await That(Act).Throws<Exception>()
			.WithMessage("The code of the caller threw an exception while the expectation was evaluated.").And
			.WithInner<InvalidOperationException>(inner => inner.HasMessage("getter failed"))
			.Because("non-public members are read by reflection, which wraps the exception that the cached accessor has to unwrap");
	}

	[Test]
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

	[Test]
	public async Task WhenHandleMembersAreEqual_ShouldSucceed()
	{
		var actual = new
		{
			Type = typeof(int),
			Method = typeof(string).GetMethod(nameof(string.Trim), Type.EmptyTypes),
			typeof(EquivalencyComparisonTests).Assembly,
			typeof(EquivalencyComparisonTests).Module,
			Delegate = (Func<string?, bool>)string.IsNullOrEmpty,
			Uri = new Uri("a/b", UriKind.Relative),
			Culture = new CultureInfo("de-DE"),
		};
		var expected = new
		{
			Type = typeof(int),
			Method = typeof(string).GetMethod(nameof(string.Trim), Type.EmptyTypes),
			typeof(EquivalencyComparisonTests).Assembly,
			typeof(EquivalencyComparisonTests).Module,
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

	[Test]
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
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("a native integer is a primitive, so it needs no entry of its own among the by-value defaults");
	}

	[Test]
	public async Task WhenIPAddressMemberDiffers_ShouldReportTheDifference()
	{
		var actual = new
		{
			Value = IPAddress.Parse("10.0.0.1"),
		};
		var expected = new
		{
			Value = IPAddress.Parse("10.0.0.2"),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                      Actual: 10.0.0.1
		                                                    Expected: 10.0.0.2
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenIPAddressMemberDiffers_WhenNegated_ShouldSucceed()
	{
		var actual = new
		{
			Value = IPAddress.Parse("10.0.0.1"),
		};
		var unexpected = new
		{
			Value = IPAddress.Loopback,
		};

		async Task Act()
			=> await That(actual).IsNotEquivalentTo(unexpected);

		await That(Act).DoesNotThrow();
	}

	[Test]
	[Arguments("10.0.0.1")]
	[Arguments("fe80::1%3")]
	public async Task WhenIPAddressMembersAreEqual_ShouldSucceed(string address)
	{
		var actual = new
		{
			Value = IPAddress.Parse(address),
		};
		var expected = new
		{
			Value = IPAddress.Parse(address),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("an IP address is compared by its Equals, as ScopeId throws for an IPv4 address and Address for an IPv6 address");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
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
		                                                      Actual: 1
		                                                    Expected: is int that is greater than 2
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenItIsMemberExpectationSpansMultipleLines_ShouldIndentTheContinuationLines()
	{
		var actual = new
		{
			Value = new WithPublicValue(1),
		};
		var expected = new
		{
			Value = It.Is<WithPublicValue>().That.IsEqualTo(new WithPublicValue(2)),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                      Actual: EquivalencyComparisonTests.WithPublicValue { Value = 1 }
		                                                    Expected: is EquivalencyComparisonTests.WithPublicValue that is equal to EquivalencyComparisonTests.WithPublicValue {
		                                                        Value = 2
		                                                      }
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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
		                                                      Actual: "abc" (string)
		                                                    Expected: is DateTime
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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
		                                                      Actual: <null>
		                                                    Expected: is string that is empty
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenItIsMemberIsUndecided_AndActualIsComparedByMembers_ShouldCompareTheMembers()
	{
		var actual = new
		{
			Value = new WithPublicValue(1),
		};
		var expected = new
		{
			Value = IsUndecided<WithPublicValue>(),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value.That was missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("an expectation that cannot decide leaves the comparison to the members of the expected value");
	}

	[Test]
	public async Task WhenItIsMemberIsUndecided_ShouldCompareTheValues()
	{
		var actual = new
		{
			Value = 1,
		};
		var expected = new
		{
			Value = IsUndecided<int>(),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse()
			.Because("an expectation that cannot decide leaves the comparison to the values");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                      Actual: 1
		                                                    Expected: is int that decides nothing
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenItIsMemberMatches_ShouldSucceed()
	{
		var actual = new
		{
			Value = 3,
		};
		var expected = new
		{
			Value = It.Is<int>().That.IsGreaterThan(2),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenJsonElementMemberDiffers_ShouldReportTheJson()
	{
		var actual = new
		{
			Value = JsonDocument.Parse("""{ "a": 1 }""").RootElement,
		};
		var expected = new
		{
			Value = JsonDocument.Parse("""{ "a": 2 }""").RootElement,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse()
			.Because("the only public member of a JsonElement is its ValueKind, so its JSON is compared instead");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                      Actual: "{\"a\":1}"
		                                                    Expected: "{\"a\":2}"
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenJsonElementMemberIsFromADisposedDocument_ShouldFailWithTheException()
	{
		JsonDocument document = JsonDocument.Parse("""{ "a": 1 }""");
		var actual = new
		{
			Value = document.RootElement,
		};
		var expected = new
		{
			Value = JsonDocument.Parse("""{ "a": 1 }""").RootElement,
		};
		document.Dispose();

		async Task Act()
			=> await That(actual).IsEquivalentTo(expected);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that actual
			             is equivalent to expected,
			             but Value did throw an ObjectDisposedException:
			               *
			             """).AsWildcard()
			.Because("reading the JSON of a disposed document runs into the document, just like reading its ValueKind did");
	}

	[Test]
	[Arguments("""{"a":[1,2]}""", """ { "a" : [ 1, 2 ] } """, true)]
	[Arguments("{\n\t\"a\": 1\r\n}", """{"a":1}""", true)]
	[Arguments("""{"a":"x y"}""", """{"a":"xy"}""", false)]
	[Arguments("""{"a":"x\" y"}""", """{"a":"x\"y"}""", false)]
	[Arguments("\"1\"", "1", false)]
	[Arguments("""{"a":1,"b":2}""", """{"b":2,"a":1}""", false)]
	public async Task WhenJsonElementMembersAreCompared_ShouldIgnoreOnlyTheWhitespaceBetweenTokens(
		string actualJson, string expectedJson, bool isEquivalent)
	{
		var actual = new
		{
			Value = JsonDocument.Parse(actualJson).RootElement,
		};
		var expected = new
		{
			Value = JsonDocument.Parse(expectedJson).RootElement,
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(),
			new StringBuilder());

		await That(result).IsEqualTo(isEquivalent);
	}

	[Test]
	public async Task WhenJsonElementMembersAreDefault_ShouldSucceed()
	{
		var actual = new
		{
			Value = default(JsonElement),
		};
		var expected = new
		{
			Value = default(JsonElement),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("a default JsonElement has no JSON, but is the same undefined value on both sides");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenJsonNodeMemberDiffers_ShouldReportTheJson()
	{
		var actual = new
		{
			Value = JsonNode.Parse("""{ "a": [1, 2] }"""),
		};
		var expected = new
		{
			Value = JsonNode.Parse("""{ "a": [1, 3] }"""),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse()
			.Because("the public members of a JsonNode only lead to its parent and root, so its JSON is compared instead");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                      Actual: "{\"a\":[1,2]}"
		                                                    Expected: "{\"a\":[1,3]}"
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenJsonNodeOptionsMemberDiffers_ShouldCompareItsMembers()
	{
		var actual = new
		{
			Value = new JsonNodeOptions
			{
				PropertyNameCaseInsensitive = true,
			},
		};
		var expected = new
		{
			Value = new JsonNodeOptions(),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse()
			.Because("only a JsonNode stands for its JSON, not every type of its namespace");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value.PropertyNameCaseInsensitive differed:
		                                                      Actual: True
		                                                    Expected: False
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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
		                                                      Actual: 1
		                                                    Expected: 3
		                                                and
		                                                  Element [2] differed:
		                                                      Actual: 3
		                                                    Expected: 1
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenLongStringMembersDifferAfterACustomMaximumStringLength_ShouldKeepTheDifferenceWithinIt()
	{
		var actual = new
		{
			Text = "aaaaaaaaaax",
		};
		var expected = new
		{
			Text = "aaaaaaaaaay",
		};
		StringBuilder failureBuilder = new();

		bool result;
		using (IDisposable _ = Customize.aweXpect.Formatting().MaximumStringLength.Set(6))
		{
			result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);
		}

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Text differed:
		                                                      Actual: "…aaaax"
		                                                    Expected: "…aaaay"
		                                                """).IgnoringNewlineStyle()
			.Because("the leading ellipsis and the difference must both fit into the maximum string length");
	}

	[Test]
	public async Task WhenLongStringMembersDifferAfterTheMaximumStringLength_ShouldShowTheDifference()
	{
		string common = new('a', 120);
		var actual = new
		{
			Text = common + "x",
		};
		var expected = new
		{
			Text = common + "y",
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Text differed:
		                                                      Actual: "…aaaaaaaaaax"
		                                                    Expected: "…aaaaaaaaaay"
		                                                """).IgnoringNewlineStyle()
			.Because("both truncated texts would be identical, so the strings are shown from shortly before their first difference");
	}

	[Test]
	[Arguments("foo", "foo", true)]
	[Arguments("foo", "bar", false)]
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

	[Test]
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
		                                                      Actual: 1 (int)
		                                                    Expected: 1 (long)
		                                                """).IgnoringNewlineStyle()
			.Because("an int member and a long member are a real difference that the formatted values do not show");
	}

	[Test]
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
		                                                      Actual: ValueLikeWithConstantText
		                                                    Expected: ValueLikeWithConstantText
		                                                """).IgnoringNewlineStyle()
			.Because("one and the same type on both sides tells the two values apart just as little as the values do");
	}

	[Test]
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
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("the type would be noise where the values already differ");
	}

	[Test]
	[Arguments("ame", false)]
	[Arguments("d.Name", false)]
	[Arguments("Name", true)]
	[Arguments("Child.Name", true)]
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

	[Test]
	public async Task WhenMemoryMemberIsComparedWithAnArray_ShouldCompareTheItems()
	{
		var actual = new
		{
			Value = new Memory<int>([1, 2, 3,], 1, 2),
		};
		var expected = new
		{
			Value = new[]
			{
				2, 3,
			},
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("a memory is the sequence of its items, like an array");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
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
		                                                      Actual: System.String Trim()
		                                                    Expected: System.String ToUpperInvariant()
		                                                """).IgnoringNewlineStyle()
			.Because("walking a member descriptor reports metadata tokens and raw runtime handle addresses instead of the method it stands for");
	}

	[Test]
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
		await That(failureBuilder.ToString()).IsEqualTo($"""

		                                                   Property Value differed:
		                                                       Actual: {typeof(EquivalencyComparisonTests).Module.Name}
		                                                     Expected: aweXpect.Core.dll
		                                                 """).IgnoringNewlineStyle()
			.Because("a module describes an emitted file, so walking it reaches getters that throw instead of state that could be compared");
	}

	[Test]
	public async Task WhenMultipleMembersAreMissing_ShouldJoinThemWithATrailingAnd()
	{
		var actual = new
		{
			Bee = 1,
		};
		var expected = new
		{
			Ant = 3,
			Bee = 1,
			Cow = 4,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Ant was missing on the actual object and
		                                                  Property Cow was missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("single-line findings only put the \"and\" on its own line when another finding spans several lines");
	}

	[Test]
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

		                                                  Property Ant was missing on the actual object
		                                                and
		                                                  Property Bee differed:
		                                                      Actual: 1
		                                                    Expected: 9
		                                                and
		                                                  Property Cow was missing on the actual object
		                                                """).IgnoringNewlineStyle()
			.Because("a missing member has to be separated from the other findings, in either direction");
	}

	[Test]
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
		                                                      Actual: 1
		                                                    Expected: 2
		                                                and
		                                                  Property Second differed:
		                                                      Actual: <null>
		                                                    Expected: "Foo"
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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

		                                                  Property Inner.B was missing on the actual object
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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
		                                                      Actual: <null>
		                                                    Expected: "Foo"
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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

	[Test]
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
		                                                      Actual: 1 (int)
		                                                    Expected: 1 (long)
		                                                """).IgnoringNewlineStyle()
			.Because("the member path does not tell the values apart either");
	}

	[Test]
	[Explicit]
	[Category(TestCategories.Slow)]
	public async Task WhenNestingExceedsTheStack_ShouldContinueOnAFreshStack()
	{
		NestedNode actual = new(5000);
		NestedNode expected = new(5000);
		EquivalencyOptions options = new()
		{
			MaxRecursionDepth = 5000,
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, options, new StringBuilder());

		await That(result).IsTrue()
			.Because("5000 levels need more stack than a thread has, so the comparison continues on a fresh one");
	}

	[Test]
	public async Task WhenNoMembersCanBeCompared_ShouldThrowInvalidOperationException()
	{
		ClassWithOnlyPrivateState actual = new(1);
		ClassWithOnlyPrivateState expected = new(2);

		async Task Act()
			=> await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), new StringBuilder());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage(
				"It has no members that could be compared on EquivalencyComparisonTests.ClassWithOnlyPrivateState, which would make the equivalency comparison succeed without verifying anything. Adjust the equivalency options to include the relevant members or to compare this type by value, or, when publishing with trimming or Native AOT enabled, ensure that the type is rooted, so that its members are preserved.");
	}

	[Test]
	public async Task WhenNullableMemberIsIgnoredByType_ShouldMatchTheUnderlyingType()
	{
		var actual = new
		{
			At = (DateTime?)new DateTime(2020, 1, 1),
		};
		var expected = new
		{
			At = (DateTime?)new DateTime(2020, 1, 2),
		};
		StringBuilder failureBuilder = new();
		EquivalencyOptions options = new()
		{
			MembersToIgnore = [new MemberToIgnore.ByPredicate((_, type) => type == typeof(DateTime), "DateTime"),],
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue()
			.Because("a nullable member holds a value of the underlying type");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	[MethodDataSource(nameof(DifferentNumbers))]
	public async Task WhenNumberMemberDiffers_ShouldFail(object actualValue, object expectedValue)
	{
		var actual = new
		{
			Value = actualValue,
		};
		var expected = new
		{
			Value = expectedValue,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).Contains("Property Value differed:");
	}

	[Test]
	[MethodDataSource(nameof(EqualNumbers))]
	public async Task WhenNumberMembersAreEqual_ShouldSucceed(object actualValue, object expectedValue)
	{
		var actual = new
		{
			Value = actualValue,
		};
		var expected = new
		{
			Value = expectedValue,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue();
	}

	[Test]
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
		                                                      Actual: <null>
		                                                    Expected: 1
		                                                """).IgnoringNewlineStyle()
			.Because("a missing value has no runtime type, and it is already distinguishable without one");
	}

	[Test]
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

	[Test]
	public async Task WhenPropertyHasAByRefLikeType_ShouldIgnoreIt()
	{
		WithSpan actual = new(1);
		WithSpan expected = new(2);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("a span cannot be boxed, so reflection cannot read it, and the source generator skips it as well");
	}

	[Test]
	public async Task WhenPropertyIsHiddenByAWriteOnlyProperty_ShouldNotCompareTheHiddenOne()
	{
		WriteOnlyHidingProperty actual = new(1, 3);
		WriteOnlyHidingProperty expected = new(2, 3);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("the write-only declaration hides the base property by name, like a declaration with a non-public getter does");
	}

	[Test]
	public async Task WhenPropertyIsOverriddenWithOnlyASetter_ShouldCompareItThroughTheInheritedGetter()
	{
		SetterOnlyOverride actual = new()
		{
			Value = 1,
		};
		SetterOnlyOverride expected = new()
		{
			Value = 2,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("an override of the setter alone still inherits the getter, so the property stays readable");
	}

	[Test]
	public async Task WhenPropertyReturnsByReference_ShouldIgnoreItByTheReferencedType()
	{
		WithRefValue actual = new(1, "foo");
		WithRefValue expected = new(2, "foo");
		EquivalencyOptions options = new()
		{
			MembersToIgnore = [new MemberToIgnore.ByPredicate((_, type) => type == typeof(int), "int"),],
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue()
			.Because("a ref-returning property is declared with the type it refers to, as a registration declares it");
	}

	[Test]
	public async Task WhenReadingADictionaryEntryThrows_ShouldFailWithTheException()
	{
		var actual = new
		{
			Values = new ThrowingHashtable
			{
				["a"] = 1,
			},
		};
		var expected = new
		{
			Values = new Dictionary<string, int>
			{
				["a"] = 1,
			},
		};

		async Task Act()
			=> await That(actual).IsEquivalentTo(expected);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that actual
			             is equivalent to expected,
			             but Values[a] did throw an InvalidOperationException:
			               indexer failed

			             Equivalency options:
			              - include public fields and properties
			             """)
			.Because("reading an entry of a dictionary runs code of the caller, just like a getter");
	}

	[Test]
	public async Task WhenReadOnlyMemoryMemberDiffers_ShouldReportTheElement()
	{
		var actual = new
		{
			Data = new ReadOnlyMemory<byte>([1, 2,]),
		};
		var expected = new
		{
			Data = new ReadOnlyMemory<byte>([1, 3,]),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element Data[1] differed:
		                                                      Actual: 2
		                                                    Expected: 3
		                                                """).IgnoringNewlineStyle()
			.Because("without its span, only the length of a memory would be left to compare");
	}

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
	public async Task WhenRegexDiffersInItsOptions_ShouldReportTheOptions()
	{
		Regex actual = PatternAIgnoringCase();
		Regex expected = PatternA();
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse()
			.Because("the options change which texts a pattern matches");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  It differed:
		                                                      Actual: ("^a$", IgnoreCase)
		                                                    Expected: ("^a$", None)
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenRegexDiffersInItsPattern_ShouldReportThePattern()
	{
		Regex actual = PatternA();
		Regex expected = PatternB();
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse()
			.Because("the public members of a regex are only its options and its timeout, while it keeps the pattern to itself");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  It differed:
		                                                      Actual: ("^a$", None)
		                                                    Expected: ("^b$", None)
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenRegexHasTheSamePatternAndOptions_ShouldSucceed()
	{
		Regex actual = PatternAIgnoringCase();
		Regex expected = CopyOfPatternAIgnoringCase();
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("two separate instances with the same pattern and options match the same texts");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenSameInstanceHasNoComparableMembers_AsCollectionElement_ShouldSucceed()
	{
		object shared = new();
		object[] actual = [1, shared,];
		object[] expected = [1, shared,];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenSameInstanceHasNoComparableMembers_AsDictionaryValue_ShouldSucceed()
	{
		object shared = new();
		Dictionary<string, object> actual = new()
		{
			["a"] = shared,
		};
		Dictionary<string, object> expected = new()
		{
			["a"] = shared,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenSameInstanceHasNoComparableMembers_AsMember_ShouldSucceed()
	{
		var actual = new
		{
			Args = EventArgs.Empty,
		};
		var expected = new
		{
			Args = EventArgs.Empty,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("an instance is equivalent to itself, so there is nothing left to verify");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenSameInstanceHasNoComparableMembers_AsRoot_ShouldSucceed()
	{
		object subject = new();

		async Task Act()
			=> await That(subject).IsEquivalentTo(subject);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WhenSameInstanceHasNoComparableMembers_AsRoot_WhenNegated_ShouldFail()
	{
		object subject = new();

		async Task Act()
			=> await That(subject).IsNotEquivalentTo(subject);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not equivalent to subject,
			             but it was*
			             """).AsWildcard();
	}

	[Test]
	public async Task WhenSameInstanceIsAnEnumerable_ShouldNotEnumerateIt()
	{
		CountingEnumerable shared = new();
		var actual = new
		{
			Items = shared,
		};
		var expected = new
		{
			Items = shared,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue();
		await That(shared.Enumerations).IsEqualTo(0)
			.Because("an enumerable that can only be enumerated once could not be compared against itself");
	}

	[Test]
	public async Task WhenSameInstanceIsComparedByValue_ShouldStillCallItsEquals()
	{
		WithThrowingEquals shared = new();

		async Task Act()
			=> await That(shared).IsEquivalentTo(shared, o => o
				.For<WithThrowingEquals>(t => t with
				{
					ComparisonType = EquivalencyComparisonType.ByValue,
				}));

		await That(Act).Throws<FailException>()
			.WithMessage("*Equals of EquivalencyComparisonTests.WithThrowingEquals did throw a NotSupportedException*")
			.AsWildcard().And
			.Whose(e => e.InnerException, i => i.Is<NotSupportedException>())
			.Because("how a type that is compared by value treats the same instance is up to its Equals");
	}

	[Test]
	public async Task WhenSetElementsAreInDifferentOrder_ShouldSucceed()
	{
		var actual = new
		{
			Values = new HashSet<int>
			{
				1,
				2,
				3,
			},
		};
		var expected = new
		{
			Values = new HashSet<int>
			{
				3,
				2,
				1,
			},
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("a set has no order, so comparing two of them by position would only report how they happen to be stored");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
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

	[Test]
	public async Task WhenSetElementsDiffer_ShouldReportTheDifference()
	{
		HashSet<int> actual = [1, 2,];
		HashSet<int> expected = [1, 3,];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [1] differed:
		                                                      Actual: 2
		                                                    Expected: 3
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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
		                                                      Actual: 2
		                                                    Expected: 1
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
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

	[Test]
	public async Task WhenSetIsNested_AndAnElementDiffers_ShouldReportTheMemberPath()
	{
		var actual = new
		{
			Values = new HashSet<int>
			{
				1,
				2,
			},
		};
		var expected = new
		{
			Values = new HashSet<int>
			{
				1,
				3,
			},
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element Values[1] differed:
		                                                      Actual: 2
		                                                    Expected: 3
		                                                """).IgnoringNewlineStyle();
	}

#if NET8_0_OR_GREATER
	[Test]
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

#if NET8_0_OR_GREATER
	[Test]
	public async Task WhenSetSubjectIsAFrozenSet_AndUsesACaseInsensitiveComparer_ShouldMatchTheExpectedItemsThroughIt()
	{
		FrozenSet<string> actual = new[]
		{
			"a", "b",
		}.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
		HashSet<string> expected = ["B", "A",];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenSetSubjectIsAnImmutableHashSet_AndUsesACaseInsensitiveComparer_ShouldMatchTheExpectedItemsThroughIt()
	{
		ImmutableHashSet<string> actual = ImmutableHashSet.Create<string>(StringComparer.OrdinalIgnoreCase, "a", "b");
		HashSet<string> expected = ["B", "A",];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}
#endif

	[Test]
	public async Task WhenSetSubjectIsNestedInAMember_AndUsesACaseInsensitiveComparer_ShouldMatchTheExpectedItemsThroughIt()
	{
		var actual = new
		{
			Value = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"a",
			},
		};
		var expected = new
		{
			Value = new HashSet<string>
			{
				"A",
			},
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenSetSubjectUsesACaseInsensitiveComparer_AndContainsNull_ShouldMatchItByEquivalency()
	{
		HashSet<string?> actual = new(StringComparer.OrdinalIgnoreCase)
		{
			"a",
			null,
		};
		object?[] expected = [null, "A",];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("a null item is never handed to the comparer of the set, but still equivalent to another null");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenSetSubjectUsesACaseInsensitiveComparer_AndExpectedIsAnArray_ShouldMatchTheExpectedItemsThroughIt()
	{
		HashSet<string> actual = new(StringComparer.OrdinalIgnoreCase)
		{
			"a",
			"b",
		};
		string[] expected = ["B", "A",];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("the comparer of the subject decides, whatever collection is expected");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenSetSubjectUsesACaseInsensitiveComparer_ShouldMatchTheExpectedItemsThroughIt()
	{
		HashSet<string> actual = new(StringComparer.OrdinalIgnoreCase)
		{
			"a",
			"b",
		};
		HashSet<string> expected = ["B", "A",];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("the comparer of the subject decides which items are the same");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenSetSubjectUsesACaseInsensitiveComparer_WithAnItemThatItDoesNotFind_ShouldReportTheDifference()
	{
		HashSet<string> actual = new(StringComparer.OrdinalIgnoreCase)
		{
			"a",
			"b",
		};
		HashSet<string> expected = ["A", "c",];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [1] differed:
		                                                      Actual: "b"
		                                                    Expected: "c"
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenSetSubjectUsesACaseSensitiveComparer_AndExpectedACaseInsensitiveOne_ShouldReportTheDifference()
	{
		HashSet<string> actual = ["a",];
		HashSet<string> expected = new(StringComparer.OrdinalIgnoreCase)
		{
			"A",
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse()
			.Because("only the comparer of the subject decides which items are the same");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [0] differed:
		                                                      Actual: "a"
		                                                    Expected: "A"
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenSetSubjectUsesACustomComparer_WithAnItemThatOnlyItsMembersMatch_ShouldSucceed()
	{
		HashSet<WithProperty> actual = new(new NeverEqualComparer())
		{
			new WithProperty(1),
		};
		HashSet<WithProperty> expected = [new(1),];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("an item that the comparer does not find is still matched by the equivalency comparison");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenSetSubjectUsesACustomComparer_WithObjectsThatOnlyItConsidersTheSame_ShouldSucceed()
	{
		HashSet<WithProperty> actual = new(new SameParityComparer())
		{
			new WithProperty(1),
			new WithProperty(2),
		};
		HashSet<WithProperty> expected = [new(4), new(3),];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("an item that the comparer of the subject finds is contained, whatever its members are");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenSetSubjectUsesTheDefaultComparer_WithObjectsThatOnlyTheirEqualsConsidersTheSame_ShouldReportTheDifference()
	{
		HashSet<AlwaysEqual> actual = [new(1),];
		HashSet<AlwaysEqual> expected = [new(2),];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse()
			.Because("a set with the default comparer leaves its items to the equivalency comparison, which ignores their Equals");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property [0].Value differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle();
	}

#if NET8_0_OR_GREATER
	[Test]
	public async Task WhenSortedSetSubjectIsAnImmutableSortedSet_AndUsesACaseInsensitiveComparer_ShouldMatchTheExpectedItemsThroughIt()
	{
		ImmutableSortedSet<string> actual = ImmutableSortedSet.Create<string>(StringComparer.OrdinalIgnoreCase, "a", "b");
		HashSet<string> expected = ["B", "A",];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenSortedSetSubjectIsAnImmutableSortedSet_AndUsesTheDefaultComparer_ShouldReportTheDifference()
	{
		ImmutableSortedSet<AlwaysSameOrder> actual = [new(1),];
		AlwaysSameOrder[] expected = [new(2),];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property [0].Value differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle();
	}
#endif

	[Test]
	public async Task WhenSortedSetSubjectIsNestedInAMember_AndUsesACaseInsensitiveComparer_ShouldMatchTheExpectedItemsThroughIt()
	{
		var actual = new
		{
			Value = new SortedSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"a",
			},
		};
		var expected = new
		{
			Value = new[]
			{
				"A",
			},
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenSortedSetSubjectUsesACaseInsensitiveComparer_AndContainsNull_ShouldMatchItByEquivalency()
	{
		SortedSet<string?> actual = new(StringComparer.OrdinalIgnoreCase)
		{
			"a",
			null,
		};
		object?[] expected = [null, "A",];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("a null item is never handed to the comparer of the set, but still equivalent to another null");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenSortedSetSubjectUsesACaseInsensitiveComparer_ShouldMatchTheExpectedItemsThroughIt()
	{
		SortedSet<string> actual = new(StringComparer.OrdinalIgnoreCase)
		{
			"a",
			"b",
		};
		string[] expected = ["B", "A",];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("two items are the same for a sorted set when its comparer orders neither before the other");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenSortedSetSubjectUsesACaseInsensitiveComparer_WithAnItemThatItDoesNotFind_ShouldReportTheDifference()
	{
		SortedSet<string> actual = new(StringComparer.OrdinalIgnoreCase)
		{
			"a",
			"b",
		};
		string[] expected = ["A", "c",];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [1] differed:
		                                                      Actual: "b"
		                                                    Expected: "c"
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenSortedSetSubjectUsesACustomComparer_WithObjectsThatOnlyItConsidersTheSame_ShouldSucceed()
	{
		SortedSet<WithProperty> actual = new(new ParityOrder())
		{
			new WithProperty(1),
			new WithProperty(2),
		};
		HashSet<WithProperty> expected = [new(4), new(3),];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("an item that the comparer of the subject finds is contained, whatever its members are");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenSortedSetSubjectUsesTheDefaultComparer_WithObjectsThatOnlyTheirCompareToConsidersTheSame_ShouldReportTheDifference()
	{
		SortedSet<AlwaysSameOrder> actual = [new(1),];
		AlwaysSameOrder[] expected = [new(2),];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse()
			.Because("a set with the default comparer leaves its items to the equivalency comparison, which ignores their CompareTo");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property [0].Value differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenStringBuilderMemberDiffers_ShouldReportTheText()
	{
		var actual = new
		{
			Value = new StringBuilder("abc"),
		};
		var expected = new
		{
			Value = new StringBuilder("xyz"),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                      Actual: "abc"
		                                                    Expected: "xyz"
		                                                """).IgnoringNewlineStyle()
			.Because("the members of a StringBuilder (Capacity, Length, ...) do not contain its text");
	}

	[Test]
	[Arguments(true)]
	[Arguments(false)]
	public async Task WhenStringBuilderMemberIsComparedWithADifferentString_ShouldReportTheText(
		bool isStringBuilderActual)
	{
		var actual = new
		{
			Value = isStringBuilderActual ? new StringBuilder("abc") : (object)"abc",
		};
		var expected = new
		{
			Value = isStringBuilderActual ? "xyz" : (object)new StringBuilder("xyz"),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                      Actual: "abc"
		                                                    Expected: "xyz"
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	[Arguments(true)]
	[Arguments(false)]
	public async Task WhenStringBuilderMemberIsComparedWithAString_ShouldCompareTheText(bool isStringBuilderActual)
	{
		object stringBuilder = new StringBuilder("abc");
		var actual = new
		{
			Value = isStringBuilderActual ? stringBuilder : "abc",
		};
		var expected = new
		{
			Value = isStringBuilderActual ? "abc" : stringBuilder,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue();
	}

	[Test]
	public async Task WhenStringBuilderMembersContainTheSameText_ShouldSucceed()
	{
		var actual = new
		{
			Value = new StringBuilder("abc"),
		};
		var expected = new
		{
			Value = new StringBuilder().Append("ab").Append('c'),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue();
	}

	[Test]
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
		                                                       Actual: "{new string('a', 100)}…"
		                                                     Expected: "{new string('b', 100)}…"
		                                                 """).IgnoringNewlineStyle();
	}

	[Test]
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
		                                                      Actual: "foo\nbar"
		                                                    Expected: "foo\nbaz"
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenTaskMemberIsTheSameInstance_ShouldSucceedWithoutWaiting()
	{
		TaskCompletionSource<int> tcs = new();
		var actual = new
		{
			Value = tcs.Task,
		};
		var expected = new
		{
			Value = tcs.Task,
		};
		StringBuilder failureBuilder = new();

		Task<bool> comparison = Task.Run(async ()
			=> await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder));
		bool isCompleted = await Task.WhenAny(comparison, Task.Delay(TimeSpan.FromSeconds(30))) == comparison;
		// Releases a comparison that waits for the task, so that it does not hang the test run.
		tcs.SetResult(1);

		await That(isCompleted).IsTrue().Because("a pending task must not be waited for");
		await That(await comparison).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenTaskMembersAreDifferentInstances_ShouldReportTheDifferenceWithANote()
	{
		var actual = new
		{
			Value = Task.FromResult("foo"),
		};
		var expected = new
		{
			Value = Task.FromResult("foo"),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                      Actual: Task<string> (RanToCompletion, "foo")
		                                                    Expected: Task<string> (RanToCompletion, "foo")
		                                                    (tasks are compared by reference)
		                                                """).IgnoringNewlineStyle()
			.Because("the state of a task changes over time, so only the same instance is equivalent");
	}

	[Test]
	public async Task WhenTaskMembersArePendingAndDifferentInstances_ShouldReportTheDifferenceWithoutWaiting()
	{
		TaskCompletionSource<int> actualSource = new();
		TaskCompletionSource<int> expectedSource = new();
		var actual = new
		{
			Value = actualSource.Task,
		};
		var expected = new
		{
			Value = expectedSource.Task,
		};
		StringBuilder failureBuilder = new();

		Task<bool> comparison = Task.Run(async ()
			=> await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder));
		bool isCompleted = await Task.WhenAny(comparison, Task.Delay(TimeSpan.FromSeconds(30))) == comparison;
		// Releases a comparison that waits for the tasks, so that it does not hang the test run.
		actualSource.SetResult(1);
		expectedSource.SetResult(1);

		await That(isCompleted).IsTrue().Because("a pending task must not be waited for");
		await That(await comparison).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                      Actual: Task<int> (WaitingForActivation)
		                                                    Expected: Task<int> (WaitingForActivation)
		                                                    (tasks are compared by reference)
		                                                """).IgnoringNewlineStyle();
	}

#if NET8_0_OR_GREATER
	[Test]
	public async Task WhenTimeOnlyMemberDiffers_ShouldReportTheDifference()
	{
		var actual = new
		{
			At = new TimeOnly(10, 30),
		};
		var expected = new
		{
			At = new TimeOnly(10, 31),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property At differed:
		                                                      Actual: 10:30:00.0000000
		                                                    Expected: 10:31:00.0000000
		                                                """).IgnoringNewlineStyle()
			.Because("the members of a time only repeat the same difference in several forms");
	}
#endif

	[Test]
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

	[Test]
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
		                                                      Actual: "ab"
		                                                    Expected: "cd"
		                                                """).IgnoringNewlineStyle()
			.Because("the comparison type registered for the element type describes the element only, not its members");
	}

	[Test]
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

	[Test]
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
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("an int has no members, so comparing it by members only because its owner is would throw");
	}

	[Test]
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
		                                                      Actual: "ab"
		                                                    Expected: "cd"
		                                                """).IgnoringNewlineStyle()
			.Because("comparing a string by members only compares its length, which would hide the difference");
	}

	[Test]
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
		                                                      Actual: "ab"
		                                                    Expected: "cd"
		                                                """).IgnoringNewlineStyle()
			.Because("the comparison type must not reach the members of a member without a registration of its own either");
	}

	[Test]
	public async Task WhenTypeIsRegistered_AndItsValueMemberGetterThrows_ShouldFailWithTheGetterException()
	{
		TypeMetadataRegistry.RegisterProperty<RegisteredThrowingProbe, int>("Phantom", x => x.PhantomValue());
		RegisteredThrowingProbe actual = new("phantom failed");
		RegisteredThrowingProbe expected = new("phantom failed");

		async Task Act()
			=> await That(actual).IsEquivalentTo(expected);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that actual
			             is equivalent to expected,
			             but Phantom did throw an InvalidOperationException:
			               phantom failed

			             Equivalency options:
			              - include public fields and properties
			             """)
			.Because("a registered value member that is compared without being read as an object still names itself as the thrower");
	}

	[Test]
	public async Task WhenTypeIsRegistered_AndItsValueMemberIsComparedByMembers_ShouldThrowInvalidOperationException()
	{
		RegisterValues();
		RegisteredValuesProbe actual = new(1, 1.5, DayOfWeek.Monday);
		RegisteredValuesProbe expected = new(1, 1.5, DayOfWeek.Monday);
		EquivalencyOptions options = new EquivalencyOptions().For<int>(o => o with
		{
			ComparisonType = EquivalencyComparisonType.ByMembers,
		});

		async Task Act()
			=> await EquivalencyComparison.Compare(actual, expected, options, new StringBuilder());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage(
				"Property Count has no members that could be compared on int, which would make the equivalency comparison succeed without verifying anything. Adjust the equivalency options to include the relevant members or to compare this type by value, or, when publishing with trimming or Native AOT enabled, ensure that the type is rooted, so that its members are preserved.")
			.Because("the comparison type registered for the member type also applies to a registered value member");
	}

	[Test]
	public async Task WhenTypeIsRegistered_AndItsValueMembersAreNaN_ShouldSucceed()
	{
		RegisterValues();
		RegisteredValuesProbe actual = new(1, double.NaN, DayOfWeek.Monday);
		RegisteredValuesProbe expected = new(1, double.NaN, DayOfWeek.Monday);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("a registered value member is equal exactly when its Equals says so, which considers NaN equal to itself");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenTypeIsRegistered_AndItsValueMembersDiffer_ShouldReportTheDifferences()
	{
		RegisterValues();
		RegisteredValuesProbe actual = new(1, 1.5, DayOfWeek.Monday);
		RegisteredValuesProbe expected = new(2, 1.5, DayOfWeek.Tuesday);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Count differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                and
		                                                  Property Day differed:
		                                                      Actual: Monday
		                                                    Expected: Tuesday
		                                                """).IgnoringNewlineStyle()
			.Because("a registered value member is reported like a member that is read as an object");
	}

	[Test]
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

	[Test]
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

	[Test]
	public async Task WhenTypeIsRegisteredAfterAComparison_ShouldCompareTheRegisteredMembers()
	{
		LateRegisteredProbe actual = new(1);
		LateRegisteredProbe expected = new(2);
		StringBuilder failureBuilder = new();

		bool resultBeforeRegistration =
			await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);
		TypeMetadataRegistry.RegisterProperty<LateRegisteredProbe, int>("Phantom", x => x.PhantomValue());
		bool resultAfterRegistration =
			await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(resultBeforeRegistration).IsTrue();
		await That(resultAfterRegistration).IsFalse()
			.Because("the members that were resolved for the first comparison must not hide a later registration");
		await That(failureBuilder.ToString()).Contains("Property Phantom differed");
	}

	[Test]
	public async Task WhenTypeIsRegisteredAsNullable_ShouldApplyTheOptionsToTheMember()
	{
		var actual = new
		{
			At = (Position?)new Position
			{
				X = 1,
				Y = 2,
			},
		};
		var expected = new
		{
			At = (Position?)new Position
			{
				X = 1,
				Y = 3,
			},
		};
		StringBuilder failureBuilder = new();
		EquivalencyOptions options = new EquivalencyOptions().For<Position?>(x => x with
		{
			MembersToIgnore = [new MemberToIgnore.ByName("Y"),],
		});

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue()
			.Because("the runtime type of a boxed nullable value is its underlying type");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
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
		                                                      Actual: int
		                                                    Expected: long
		                                                """).IgnoringNewlineStyle()
			.Because("GenericParameterPosition throws on a type that is not a generic parameter, so the walk cannot reach a difference at all");
	}

	[Test]
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

	[Test]
	[Arguments("a/b", "a/c", UriKind.Relative)]
	[Arguments("https://a/b", "https://a/c", UriKind.Absolute)]
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
		                                                       Actual: {actualUri}
		                                                     Expected: {expectedUri}
		                                                 """).IgnoringNewlineStyle()
			.Because("every component of a relative URI throws, and the components of an absolute one repeat the same difference many times over");
	}

	[Test]
	public async Task WhenValueTaskMembersHaveDifferentResults_ShouldReportTheDifference()
	{
		var actual = new
		{
			Value = new ValueTask<int>(1),
		};
		var expected = new
		{
			Value = new ValueTask<int>(2),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Value differed:
		                                                      Actual: ValueTask<int> (RanToCompletion, 1)
		                                                    Expected: ValueTask<int> (RanToCompletion, 2)
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenValueTaskMembersHaveTheSameResult_ShouldSucceed()
	{
		var actual = new
		{
			Value = new ValueTask<int>(1),
			Untyped = new ValueTask(),
		};
		var expected = new
		{
			Value = new ValueTask<int>(1),
			Untyped = new ValueTask(),
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenValueTaskMembersWrapTheSamePendingTask_ShouldSucceedWithoutWaiting()
	{
		TaskCompletionSource<int> tcs = new();
		var actual = new
		{
			Value = new ValueTask<int>(tcs.Task),
			Untyped = new ValueTask(tcs.Task),
		};
		var expected = new
		{
			Value = new ValueTask<int>(tcs.Task),
			Untyped = new ValueTask(tcs.Task),
		};
		StringBuilder failureBuilder = new();

		Task<bool> comparison = Task.Run(async ()
			=> await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder));
		bool isCompleted = await Task.WhenAny(comparison, Task.Delay(TimeSpan.FromSeconds(30))) == comparison;
		// Releases a comparison that waits for the task, so that it does not hang the test run.
		tcs.SetResult(1);

		await That(isCompleted).IsTrue().Because("a pending task must not be waited for");
		await That(await comparison).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
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
		                                                      Actual: 2
		                                                    Expected: 3
		                                                """).IgnoringNewlineStyle()
			.Because("an ordinary class carries its state in its members, so naming the differing component stays the better message");
	}

	public static IEnumerable<(object, object)> DifferentNumbers() =>
	[
		(new BigInteger(3), new BigInteger(5)),
		(new Complex(1, 2), new Complex(1, 3)),
		((Half)1, (Half)2),
		((NFloat)1, (NFloat)2),
		((Int128)1, (Int128)2),
		((UInt128)1, (UInt128)2),
	];

	public static IEnumerable<(object, object)> EqualNumbers() =>
	[
		(new BigInteger(3), new BigInteger(3)),
		(new Complex(1, 2), new Complex(1, 2)),
		((Half)1, (Half)1),
		((NFloat)1, (NFloat)1),
		((Int128)1, (Int128)1),
		((UInt128)1, (UInt128)1),
	];

	/// <remarks>
	///     Each call captures the <paramref name="value" /> in a closure of its own, so two delegates over the same
	///     method get separate targets.
	/// </remarks>
	private static Func<int> Capture(int value) => () => value;

#if NET8_0_OR_GREATER
	[GeneratedRegex("^a$", RegexOptions.IgnoreCase)]
	private static partial Regex CopyOfPatternAIgnoringCase();

	[GeneratedRegex("^a$")]
	private static partial Regex PatternA();

	[GeneratedRegex("^a$", RegexOptions.IgnoreCase)]
	private static partial Regex PatternAIgnoringCase();

	[GeneratedRegex("^b$")]
	private static partial Regex PatternB();
#else
	private static readonly Regex CopyOfPatternAIgnoringCaseRegex = new("^a$", RegexOptions.IgnoreCase);
	private static readonly Regex PatternARegex = new("^a$");
	private static readonly Regex PatternAIgnoringCaseRegex = new("^a$", RegexOptions.IgnoreCase);
	private static readonly Regex PatternBRegex = new("^b$");

	private static Regex CopyOfPatternAIgnoringCase() => CopyOfPatternAIgnoringCaseRegex;

	private static Regex PatternA() => PatternARegex;

	private static Regex PatternAIgnoringCase() => PatternAIgnoringCaseRegex;

	private static Regex PatternB() => PatternBRegex;
#endif

	private static It.IsEquivalent<T> IsUndecided<T>()
	{
		It.IsEquivalent<T> isEquivalent = It.Is<T>();
		((IExpectThat<T>)isEquivalent).ExpectationBuilder.AddConstraint((_, _) => new UndecidedConstraint<T>());
		return isEquivalent;
	}

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

	private static void RegisterValues()
		=> TypeMetadataRegistry.RegisterBatch(() =>
		{
			TypeMetadataRegistry.RegisterProperty<RegisteredValuesProbe, int>("Count", x => x.CountValue());
			TypeMetadataRegistry.RegisterProperty<RegisteredValuesProbe, double>("Ratio", x => x.RatioValue());
			TypeMetadataRegistry.RegisterProperty<RegisteredValuesProbe, DayOfWeek>("Day", x => x.DayValue());
		});

	private sealed class AlwaysEqual(int value)
	{
		public int Value => value;

		public override bool Equals(object? obj) => obj is AlwaysEqual;

		public override int GetHashCode() => 0;
	}

	private sealed class AlwaysSameOrder(int value) : IComparable<AlwaysSameOrder>
	{
		public int Value => value;

		public int CompareTo(AlwaysSameOrder? other) => 0;
	}

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

	private sealed class CountingEnumerable : IEnumerable<int>
	{
		public int Enumerations { get; private set; }

		public IEnumerator<int> GetEnumerator()
		{
			Enumerations++;
			yield return 1;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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

	private sealed class ExplicitSpan(int value) : IHasSpan
	{
		ReadOnlySpan<int> IHasSpan.Values => new[]
		{
			value,
		};
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

	private interface IHasSpan
	{
		ReadOnlySpan<int> Values { get; }
	}

	private interface IHasValue
	{
		int Value { get; }
	}

	private sealed class KeyOnly(string key)
	{
		public string Key => key;
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

	private struct Position
	{
		public int X { get; set; }
		public int Y { get; set; }
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

	/// <remarks>
	///     Has no entries of its own, but enumerates the given <paramref name="entries" /> when it is enumerated as a
	///     sequence, so that the comparison finds entries that it cannot copy into a dictionary.
	/// </remarks>
	private sealed class ReadOnlyDictionaryWithEntries(params object?[] entries) : IReadOnlyDictionary<string, int>
	{
		public int Count => 0;
		public IEnumerable<string> Keys => [];
		public IEnumerable<int> Values => [];
		public int this[string key] => throw new KeyNotFoundException();
		public bool ContainsKey(string key) => false;

		public bool TryGetValue(string key, out int value)
		{
			value = 0;
			return false;
		}

		public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
			=> Enumerable.Empty<KeyValuePair<string, int>>().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => entries.GetEnumerator();
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

	private sealed class LateRegisteredProbe(int phantom)
	{
		public int Visible { get; set; }

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

	private sealed class RegisteredThrowingProbe(string message)
	{
		public int PhantomValue() => throw new InvalidOperationException(message);
	}

	private sealed class RegisteredValuesProbe(int count, double ratio, DayOfWeek day)
	{
		public int CountValue() => count;
		public DayOfWeek DayValue() => day;
		public double RatioValue() => ratio;
	}

	private sealed class SetterOnlyOverride : VirtualValue
	{
		public override int Value
		{
			set => base.Value = value;
		}
	}

	private sealed class ThrowingHashtable : Hashtable
	{
		public override object? this[object key]
		{
			get => throw new InvalidOperationException("indexer failed");
			set => base[key] = value;
		}
	}

	private sealed class UndecidedConstraint<T> : IValueConstraint<T>
	{
		public ConstraintResult IsMetBy(T actual) => new DummyConstraintResult(Outcome.Undecided, "decides nothing");

		public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("decides nothing");
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

	private class VirtualValue
	{
		public virtual int Value { get; set; }
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

	private sealed class WithPrivateField(int value)
	{
		private readonly int Value = value;

		public override string ToString() => $"{Value}";
	}

	private sealed class WithPrivateGetter(int value)
	{
		public int Value { private get; set; } = value;
		public int Other { get; set; }

		public override string ToString() => $"{Value}";
	}

	private sealed class WithPrivateProperty(int value)
	{
		private int Value { get; } = value;

		public override string ToString() => $"{Value}";
	}

	private sealed class NeverEqualComparer : IEqualityComparer<WithProperty>
	{
		public bool Equals(WithProperty? x, WithProperty? y) => false;

		public int GetHashCode(WithProperty obj) => obj.Value;
	}

	private sealed class SameParityComparer : IEqualityComparer<WithProperty>
	{
		public bool Equals(WithProperty? x, WithProperty? y) => x!.Value % 2 == y!.Value % 2;

		public int GetHashCode(WithProperty obj) => obj.Value % 2;
	}

	private sealed class ParityOrder : IComparer<WithProperty>
	{
		public int Compare(WithProperty? x, WithProperty? y) => (x!.Value % 2).CompareTo(y!.Value % 2);
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

	private sealed class WithRefValue(int value, string own)
	{
		private int _value = value;
		public string Own { get; } = own;
		public ref int Value => ref _value;
	}

	private sealed class WithSpan(int value)
	{
		public ReadOnlySpan<int> Span => new[]
		{
			value,
		};

		public int Value => value;
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

	private sealed class WriteOnlyHidingProperty(int property, int own) : WithProperty(property)
	{
		private string _value = "";
		public int Own { get; } = own;

		public new string Value
		{
			set => _value = value;
		}
	}
}
