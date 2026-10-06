using aweXpect.Core;
using aweXpect.Equivalency;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable NotAccessedField.Local
namespace aweXpect.Internal.Tests.Helpers;

public sealed partial class EquivalencyMatchTypeTests
{
	public sealed class Tests
	{
		[Test]
		public async Task ShouldBeEquivalentToSelf()
		{
			MyClass actual = new()
			{
				MyValue = "foo",
			};
			EquivalencyMatchType sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, actual);

			await That(result).IsTrue();
		}

		[Test]
		public async Task ShouldPreventCyclicReferences()
		{
			MyClass actual = new()
			{
				MyValue = "foo",
			};
			MyClass expected = new()
			{
				MyValue = "bar",
			};
			actual.Nested = expected;
			expected.Nested = actual;
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
			                              and
			                                Property Nested.MyValue differed:
			                                    Actual: "bar"
			                                  Expected: "foo"
			                              """);
		}

		[Test]
		public async Task WhenActualAndExpectedAreNull_ShouldBeConsideredEqual()
		{
			MyClass? actual = null;
			MyClass? expected = null;
			EquivalencyMatchType sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsTrue();
		}

		[Test]
		public async Task WhenOnlyActualIsNull_ShouldNotBeConsideredEqual()
		{
			MyClass? actual = null;
			MyClass expected = new();
			EquivalencyMatchType sut = new(new EquivalencyOptions());

			IObjectMatchResult explanation = await sut.AreConsideredEqualWithExplanation(actual, expected);
			bool result = explanation.IsMatch;
			string failure = explanation.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsFalse();
			await That(failure).IsEqualTo("""
			                              it was <null> instead of EquivalencyMatchTypeTests.MyClass { MyValue = <null>, Nested = <null> }
			                              """);
		}

		[Test]
		public async Task WhenOnlyExpectedIsNull_ShouldNotBeConsideredEqual()
		{
			MyClass actual = new();
			MyClass? expected = null;
			EquivalencyMatchType sut = new(new EquivalencyOptions());

			IObjectMatchResult explanation = await sut.AreConsideredEqualWithExplanation(actual, expected);
			bool result = explanation.IsMatch;
			string failure = explanation.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsFalse();
			await That(failure).IsEqualTo("""
			                              it was EquivalencyMatchTypeTests.MyClass { MyValue = <null>, Nested = <null> } instead of <null>
			                              """);
		}
	}

	private sealed class MyClass
	{
		public string? MyValue { get; set; }
		public MyClass? Nested { get; set; }
	}

	private sealed class MyClassWithField
	{
		public string? MyValue;
	}
}
