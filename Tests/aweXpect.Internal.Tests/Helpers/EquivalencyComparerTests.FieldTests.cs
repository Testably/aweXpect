using aweXpect.Core;
using aweXpect.Equivalency;

namespace aweXpect.Internal.Tests.Helpers;

public sealed partial class EquivalencyComparerTests
{
	public sealed class FieldTests
	{
		[Fact]
		public async Task ShouldBeEquivalentToClassWithSameFields()
		{
			MyClassWithField actual = new()
			{
				MyValue = "foo",
			};
			MyClassWithField expected = new()
			{
				MyValue = "foo",
			};
			EquivalencyComparer sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsTrue();
		}

		[Fact]
		public async Task ShouldConsiderPublicFieldsOnly()
		{
			MyClassWithFields actual = new(1, 2, 3);
			MyClassWithFields expected = new(1, 3, 4);
			EquivalencyComparer sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsTrue();
		}

		[Theory]
		[InlineData("foo", null)]
		[InlineData(null, "bar")]
		public async Task ShouldNotBeEquivalentToClassWhenOneFieldIsNull(
			string? actualValue, string? expectedValue)
		{
			MyClassWithField actual = new()
			{
				MyValue = actualValue,
			};
			MyClassWithField expected = new()
			{
				MyValue = expectedValue,
			};
			EquivalencyComparer sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, expected);
			string failure = sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsFalse();
			await That(failure).IsEqualTo($"""
			                               it was not:
			                                 Field MyValue was {Formatter.Format(actualValue)} instead of {Formatter.Format(expectedValue)}
			                               """);
		}

		[Fact]
		public async Task ShouldNotBeEquivalentToClassWithDifferentFields()
		{
			MyClassWithField actual = new()
			{
				MyValue = "foo",
			};
			MyClassWithField expected = new()
			{
				MyValue = "bar",
			};
			EquivalencyComparer sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, expected);
			string failure = sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsFalse();
			await That(failure).IsEqualTo("""
			                              it was not:
			                                Field MyValue differed:
			                                     Found: "foo"
			                                  Expected: "bar"
			                              """);
		}

		[Theory]
		[InlineData(5, 5, true)]
		[InlineData(5, 6, false)]
		public async Task WhenIncludingInternalMembers_ShouldConsiderPublicAndInternalFields(
			int actualInternalValue, int expectedInternalValue, bool expectedResult)
		{
			MyClassWithFields actual = new(1, actualInternalValue, 3);
			MyClassWithFields expected = new(2, expectedInternalValue, 4);
			EquivalencyComparer sut = new(new EquivalencyOptions
			{
				Fields = IncludeMembers.Internal,
			});

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsEqualTo(expectedResult);
			if (!expectedResult)
			{
				await That(sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected))
					.IsEqualTo($"""
					            it was not:
					              Field MyInternalField differed:
					                   Found: {actualInternalValue}
					                Expected: {expectedInternalValue}
					            """);
			}
		}

		[Theory]
		[InlineData(5, 5, true)]
		[InlineData(5, 6, false)]
		public async Task WhenIncludingPrivateMembers_ShouldConsiderPublicAndInternalFields(
			int actualPrivateValue, int expectedPrivateValue, bool expectedResult)
		{
			MyClassWithFields actual = new(1, 3, actualPrivateValue);
			MyClassWithFields expected = new(2, 4, expectedPrivateValue);
			EquivalencyComparer sut = new(new EquivalencyOptions
			{
				Fields = IncludeMembers.Private,
			});

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsEqualTo(expectedResult);
			if (!expectedResult)
			{
				await That(sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected))
					.IsEqualTo($"""
					            it was not:
					              Field MyPrivateField differed:
					                   Found: {actualPrivateValue}
					                Expected: {expectedPrivateValue}
					            """);
			}
		}

		[Theory]
		[InlineData(1, 2, 1, 2, true)]
		[InlineData(1, 2, 1, 3, false)]
		[InlineData(1, 2, 3, 2, false)]
		public async Task WhenIncludingPublicAndInternalMembers_ShouldConsiderPublicAndInternalFields(
			int actualPublicValue, int actualInternalValue, int expectedPublicValue, int expectedInternalValue,
			bool expectedResult)
		{
			MyClassWithFields actual = new(actualPublicValue, actualInternalValue, 3);
			MyClassWithFields expected = new(expectedPublicValue, expectedInternalValue, 4);
			EquivalencyComparer sut = new(new EquivalencyOptions
			{
				Fields = IncludeMembers.Public | IncludeMembers.Internal,
			});

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsEqualTo(expectedResult)
				.Because("a field has to satisfy one of the requested visibilities, not all of them at once");
		}

		[Theory]
		[InlineData(5, 5, true)]
		[InlineData(5, 6, false)]
		public async Task WhenIncludingPublicMembers_ShouldConsiderPublicAndInternalFields(
			int actualPublicValue, int expectedPublicValue, bool expectedResult)
		{
			MyClassWithFields actual = new(actualPublicValue, 1, 3);
			MyClassWithFields expected = new(expectedPublicValue, 2, 4);
			EquivalencyComparer sut = new(new EquivalencyOptions
			{
				Fields = IncludeMembers.Public,
			});

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsEqualTo(expectedResult);
			if (!expectedResult)
			{
				await That(sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected))
					.IsEqualTo($"""
					            it was not:
					              Field MyPublicField differed:
					                   Found: {actualPublicValue}
					                Expected: {expectedPublicValue}
					            """);
			}
		}

		private sealed class MyClassWithFields(int publicField, int internalField, int privateField)
		{
			internal int MyInternalField = internalField;
			private int MyPrivateField = privateField;
			public int MyPublicField = publicField;
		}
	}
}
