using aweXpect.Core;
using aweXpect.Equivalency;

namespace aweXpect.Internal.Tests.Helpers;

public sealed partial class EquivalencyComparerTests
{
	public sealed class PropertyTests
	{
		[Fact]
		public async Task ShouldBeEquivalentToClassWithSameProperties()
		{
			MyClass actual = new()
			{
				MyValue = "foo",
			};
			MyClass expected = new()
			{
				MyValue = "foo",
			};
			EquivalencyComparer sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsTrue();
		}

		[Fact]
		public async Task ShouldBeEquivalentToDynamicWithProperties()
		{
			MyClass actual = new()
			{
				MyValue = "foo",
			};
			EquivalencyComparer sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, new
			{
				MyValue = "foo",
			});

			await That(result).IsTrue();
		}

		[Theory]
		[InlineData("foo", null)]
		[InlineData(null, "bar")]
		public async Task ShouldNotBeEquivalentToClassWhenOnePropertyIsNull(
			string? actualValue, string? expectedValue)
		{
			MyClass actual = new()
			{
				MyValue = actualValue,
			};
			MyClass expected = new()
			{
				MyValue = expectedValue,
			};
			EquivalencyComparer sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, expected);
			string failure = sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsFalse();
			await That(failure).IsEqualTo($"""
			                               it was not:
			                                 Property MyValue was {Formatter.Format(actualValue)} instead of {Formatter.Format(expectedValue)}
			                               """);
		}

		[Fact]
		public async Task ShouldNotBeEquivalentToClassWithDifferentProperties()
		{
			MyClass actual = new()
			{
				MyValue = "foo",
			};
			MyClass expected = new()
			{
				MyValue = "bar",
			};
			EquivalencyComparer sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, expected);
			string failure = sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsFalse();
			await That(failure).IsEqualTo("""
			                              it was not:
			                                Property MyValue differed:
			                                     Found: "foo"
			                                  Expected: "bar"
			                              """);
		}

		[Theory]
		[InlineData(5, 5, true)]
		[InlineData(5, 6, false)]
		public async Task WhenIncludingInternalMembers_ShouldConsiderPublicAndInternalProperties(
			int actualInternalValue, int expectedInternalValue, bool expectedResult)
		{
			MyClassWithProperties actual = new(1, actualInternalValue, 3);
			MyClassWithProperties expected = new(2, expectedInternalValue, 4);
			EquivalencyComparer sut = new(new EquivalencyOptions
			{
				Properties = IncludeMembers.Internal,
			});

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsEqualTo(expectedResult);
			if (!expectedResult)
			{
				await That(sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected))
					.IsEqualTo($"""
					            it was not:
					              Property MyInternalProperty differed:
					                   Found: {actualInternalValue}
					                Expected: {expectedInternalValue}
					            """);
			}
		}

		[Theory]
		[InlineData(5, 5, true)]
		[InlineData(5, 6, false)]
		public async Task WhenIncludingPrivateMembers_ShouldConsiderPublicAndInternalProperties(
			int actualPrivateValue, int expectedPrivateValue, bool expectedResult)
		{
			MyClassWithProperties actual = new(1, 3, actualPrivateValue);
			MyClassWithProperties expected = new(2, 4, expectedPrivateValue);
			EquivalencyComparer sut = new(new EquivalencyOptions
			{
				Properties = IncludeMembers.Private,
			});

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsEqualTo(expectedResult);
			if (!expectedResult)
			{
				await That(sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected))
					.IsEqualTo($"""
					            it was not:
					              Property MyPrivateProperty differed:
					                   Found: {actualPrivateValue}
					                Expected: {expectedPrivateValue}
					            """);
			}
		}

		[Theory]
		[InlineData(1, 2, 1, 2, true)]
		[InlineData(1, 2, 1, 3, false)]
		[InlineData(1, 2, 3, 2, false)]
		public async Task WhenIncludingPublicAndInternalMembers_ShouldConsiderPublicAndInternalProperties(
			int actualPublicValue, int actualInternalValue, int expectedPublicValue, int expectedInternalValue,
			bool expectedResult)
		{
			MyClassWithProperties actual = new(actualPublicValue, actualInternalValue, 3);
			MyClassWithProperties expected = new(expectedPublicValue, expectedInternalValue, 4);
			EquivalencyComparer sut = new(new EquivalencyOptions
			{
				Properties = IncludeMembers.Public | IncludeMembers.Internal,
			});

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsEqualTo(expectedResult)
				.Because("a property has to satisfy one of the requested visibilities, not all of them at once");
		}

		[Theory]
		[InlineData(5, 5, true)]
		[InlineData(5, 6, false)]
		public async Task WhenIncludingPublicMembers_ShouldConsiderPublicAndInternalProperties(
			int actualPublicValue, int expectedPublicValue, bool expectedResult)
		{
			MyClassWithProperties actual = new(actualPublicValue, 1, 3);
			MyClassWithProperties expected = new(expectedPublicValue, 2, 4);
			EquivalencyComparer sut = new(new EquivalencyOptions
			{
				Properties = IncludeMembers.Public,
			});

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsEqualTo(expectedResult);
			if (!expectedResult)
			{
				await That(sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected))
					.IsEqualTo($"""
					            it was not:
					              Property MyPublicProperty differed:
					                   Found: {actualPublicValue}
					                Expected: {expectedPublicValue}
					            """);
			}
		}

		private sealed class MyClassWithProperties(int publicProperty, int internalProperty, int privateProperty)
		{
			internal int MyInternalProperty { get; } = internalProperty;
			private int MyPrivateProperty { get; } = privateProperty;
			public int MyPublicProperty { get; } = publicProperty;
		}
	}
}
