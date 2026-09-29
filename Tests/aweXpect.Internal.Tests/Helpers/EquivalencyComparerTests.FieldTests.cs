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
			                                 Field MyValue differed:
			                                     Actual: {Formatter.Format(actualValue)}
			                                   Expected: {Formatter.Format(expectedValue)}
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
			                                    Actual: "foo"
			                                  Expected: "bar"
			                              """);
		}

		[Fact]
		public async Task WhenIncludingInternalMembers_ShouldConsiderProtectedInternalFields()
		{
			MyClassWithProtectedFields actual = new(1, 3);
			MyClassWithProtectedFields expected = new(2, 3);
			EquivalencyComparer sut = new(new EquivalencyOptions
			{
				Fields = IncludeMembers.Internal,
			});

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsFalse()
				.Because("a protected internal field is visible to the whole assembly like an internal one");
			await That(sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected))
				.IsEqualTo("""
				           it was not:
				             Field MyProtectedInternalField differed:
				                 Actual: 1
				               Expected: 2
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
					                  Actual: {actualInternalValue}
					                Expected: {expectedInternalValue}
					            """);
			}
		}

		[Fact]
		public async Task WhenIncludingInternalMembers_ShouldNotConsiderPrivateProtectedFields()
		{
			MyClassWithProtectedFields actual = new(1, 3);
			MyClassWithProtectedFields expected = new(1, 4);
			EquivalencyComparer sut = new(new EquivalencyOptions
			{
				Fields = IncludeMembers.Internal,
			});

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsTrue()
				.Because("a private protected field is only visible to derived types within the assembly");
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
					                  Actual: {actualPublicValue}
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

		private class MyClassWithProtectedFields(int protectedInternalField, int privateProtectedField)
		{
			private protected int MyPrivateProtectedField = privateProtectedField;
			protected internal int MyProtectedInternalField = protectedInternalField;
		}
	}
}
