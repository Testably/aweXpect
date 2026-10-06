using aweXpect.Core;
using aweXpect.Equivalency;

namespace aweXpect.Internal.Tests.Helpers;

public sealed partial class EquivalencyMatchTypeTests
{
	public sealed class PropertyTests
	{
		[Test]
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
			EquivalencyMatchType sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsTrue();
		}

		[Test]
		public async Task ShouldBeEquivalentToDynamicWithProperties()
		{
			MyClass actual = new()
			{
				MyValue = "foo",
			};
			EquivalencyMatchType sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, new
			{
				MyValue = "foo",
			});

			await That(result).IsTrue();
		}

		[Test]
		[Arguments("foo", null)]
		[Arguments(null, "bar")]
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
			EquivalencyMatchType sut = new(new EquivalencyOptions());

			IObjectMatchResult explanation = await sut.AreConsideredEqualWithExplanation(actual, expected);
			bool result = explanation.IsMatch;
			string failure = explanation.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsFalse();
			await That(failure).IsEqualTo($"""
			                               it was not:
			                                 Property MyValue differed:
			                                     Actual: {Formatter.Format(actualValue)}
			                                   Expected: {Formatter.Format(expectedValue)}
			                               """);
		}

		[Test]
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
			EquivalencyMatchType sut = new(new EquivalencyOptions());

			IObjectMatchResult explanation = await sut.AreConsideredEqualWithExplanation(actual, expected);
			bool result = explanation.IsMatch;
			string failure = explanation.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsFalse();
			await That(failure).IsEqualTo("""
			                              it was not:
			                                Property MyValue differed:
			                                    Actual: "foo"
			                                  Expected: "bar"
			                              """);
		}

		[Test]
		public async Task WhenIncludingInternalMembers_ShouldConsiderProtectedInternalProperties()
		{
			MyClassWithProtectedProperties actual = new(1, 3);
			MyClassWithProtectedProperties expected = new(2, 3);
			EquivalencyMatchType sut = new(new EquivalencyOptions
			{
				Properties = IncludeMembers.Internal,
			});

			IObjectMatchResult explanation = await sut.AreConsideredEqualWithExplanation(actual, expected);
			bool result = explanation.IsMatch;

			await That(result).IsFalse()
				.Because("a protected internal property is visible to the whole assembly like an internal one");
			await That(explanation.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected))
				.IsEqualTo("""
				           it was not:
				             Property MyProtectedInternalProperty differed:
				                 Actual: 1
				               Expected: 2
				           """);
		}

		[Test]
		[Arguments(5, 5, true)]
		[Arguments(5, 6, false)]
		public async Task WhenIncludingInternalMembers_ShouldConsiderPublicAndInternalProperties(
			int actualInternalValue, int expectedInternalValue, bool expectedResult)
		{
			MyClassWithProperties actual = new(1, actualInternalValue, 3);
			MyClassWithProperties expected = new(2, expectedInternalValue, 4);
			EquivalencyMatchType sut = new(new EquivalencyOptions
			{
				Properties = IncludeMembers.Internal,
			});

			IObjectMatchResult explanation = await sut.AreConsideredEqualWithExplanation(actual, expected);
			bool result = explanation.IsMatch;

			await That(result).IsEqualTo(expectedResult);
			if (!expectedResult)
			{
				await That(explanation.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected))
					.IsEqualTo($"""
					            it was not:
					              Property MyInternalProperty differed:
					                  Actual: {actualInternalValue}
					                Expected: {expectedInternalValue}
					            """);
			}
		}

		[Test]
		public async Task WhenIncludingInternalMembers_ShouldNotConsiderPrivateProtectedProperties()
		{
			MyClassWithProtectedProperties actual = new(1, 3);
			MyClassWithProtectedProperties expected = new(1, 4);
			EquivalencyMatchType sut = new(new EquivalencyOptions
			{
				Properties = IncludeMembers.Internal,
			});

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsTrue()
				.Because("a private protected property is only visible to derived types within the assembly");
		}

		[Test]
		[Arguments(1, 2, 1, 2, true)]
		[Arguments(1, 2, 1, 3, false)]
		[Arguments(1, 2, 3, 2, false)]
		public async Task WhenIncludingPublicAndInternalMembers_ShouldConsiderPublicAndInternalProperties(
			int actualPublicValue, int actualInternalValue, int expectedPublicValue, int expectedInternalValue,
			bool expectedResult)
		{
			MyClassWithProperties actual = new(actualPublicValue, actualInternalValue, 3);
			MyClassWithProperties expected = new(expectedPublicValue, expectedInternalValue, 4);
			EquivalencyMatchType sut = new(new EquivalencyOptions
			{
				Properties = IncludeMembers.Public | IncludeMembers.Internal,
			});

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsEqualTo(expectedResult)
				.Because("a property has to satisfy one of the requested visibilities, not all of them at once");
		}

		[Test]
		[Arguments(5, 5, true)]
		[Arguments(5, 6, false)]
		public async Task WhenIncludingPublicMembers_ShouldConsiderPublicAndInternalProperties(
			int actualPublicValue, int expectedPublicValue, bool expectedResult)
		{
			MyClassWithProperties actual = new(actualPublicValue, 1, 3);
			MyClassWithProperties expected = new(expectedPublicValue, 2, 4);
			EquivalencyMatchType sut = new(new EquivalencyOptions
			{
				Properties = IncludeMembers.Public,
			});

			IObjectMatchResult explanation = await sut.AreConsideredEqualWithExplanation(actual, expected);
			bool result = explanation.IsMatch;

			await That(result).IsEqualTo(expectedResult);
			if (!expectedResult)
			{
				await That(explanation.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected))
					.IsEqualTo($"""
					            it was not:
					              Property MyPublicProperty differed:
					                  Actual: {actualPublicValue}
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

		private class MyClassWithProtectedProperties(int protectedInternalProperty, int privateProtectedProperty)
		{
			private protected int MyPrivateProtectedProperty { get; } = privateProtectedProperty;
			protected internal int MyProtectedInternalProperty { get; } = protectedInternalProperty;
		}
	}
}
