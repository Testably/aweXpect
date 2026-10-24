using aweXpect.Core;
using aweXpect.Equivalency;
using aweXpect.Options;

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
			EquivalencyMatchType<object?> sut = new(new EquivalencyOptions());

			bool result = await AreConsideredEqual(sut, actual, actual);

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
			EquivalencyMatchType<object?> sut = new(new EquivalencyOptions());

			Explanation explanation = await ExplainAsync(sut, actual, expected);
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
			EquivalencyMatchType<object?> sut = new(new EquivalencyOptions());

			bool result = await AreConsideredEqual(sut, actual, expected);

			await That(result).IsTrue();
		}

		[Test]
		public async Task WhenOnlyActualIsNull_ShouldNotBeConsideredEqual()
		{
			MyClass? actual = null;
			MyClass expected = new();
			EquivalencyMatchType<object?> sut = new(new EquivalencyOptions());

			Explanation explanation = await ExplainAsync(sut, actual, expected);
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
			EquivalencyMatchType<object?> sut = new(new EquivalencyOptions());

			Explanation explanation = await ExplainAsync(sut, actual, expected);
			bool result = explanation.IsMatch;
			string failure = explanation.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsFalse();
			await That(failure).IsEqualTo("""
			                              it was EquivalencyMatchTypeTests.MyClass { MyValue = <null>, Nested = <null> } instead of <null>
			                              """);
		}
	}

	private static ValueTask<bool> AreConsideredEqual<TActual, TExpected>(EquivalencyMatchType<object?> sut, TActual actual,
		TExpected expected)
		=> new ObjectEqualityOptions<object?>().SetMatchType(sut, "Equivalent").AreConsideredEqual(actual, expected);

	private static async Task<Explanation> ExplainAsync<TActual, TExpected>(EquivalencyMatchType<object?> sut, TActual actual,
		TExpected expected)
	{
		ObjectEqualityOptions<object?> options = new ObjectEqualityOptions<object?>().SetMatchType(sut, "Equivalent");
		MatchResult result = await options.MatchWithExplanation(actual, expected);
		return new Explanation(result.IsMatch,
			(it, grammars, a, e) => options.GetExtendedFailure(it, grammars, a, e, result));
	}

	private sealed class Explanation(
		bool isMatch,
		Func<string, ExpectationGrammars, object?, object?, string> getExtendedFailure)
	{
		public bool IsMatch => isMatch;

		public string GetExtendedFailure(string it, ExpectationGrammars grammars, object? actual, object? expected)
			=> getExtendedFailure(it, grammars, actual, expected);
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
