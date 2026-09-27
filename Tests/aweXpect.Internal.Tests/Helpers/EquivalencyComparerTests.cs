using aweXpect.Core;
using aweXpect.Equivalency;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable NotAccessedField.Local
namespace aweXpect.Internal.Tests.Helpers;

public sealed partial class EquivalencyComparerTests
{
	public sealed class Tests
	{
		[Fact]
		public async Task ShouldBeEquivalentToSelf()
		{
			MyClass actual = new()
			{
				MyValue = "foo",
			};
			EquivalencyComparer sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, actual);

			await That(result).IsTrue();
		}

		[Fact]
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
			EquivalencyComparer sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, expected);
			string failure = sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

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

		[Fact]
		public async Task WhenActualAndExpectedAreNull_ShouldBeConsideredEqual()
		{
			MyClass? actual = null;
			MyClass? expected = null;
			EquivalencyComparer sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsTrue();
		}

		[Fact]
		public async Task WhenOnlyActualIsNull_ShouldNotBeConsideredEqual()
		{
			MyClass? actual = null;
			MyClass expected = new();
			EquivalencyComparer sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, expected);
			string failure = sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsFalse();
			await That(failure).IsEqualTo("""
			                              it was <null> instead of EquivalencyComparerTests.MyClass { MyValue = <null>, Nested = <null> }
			                              """);
		}

		[Fact]
		public async Task WhenOnlyExpectedIsNull_ShouldNotBeConsideredEqual()
		{
			MyClass actual = new();
			MyClass? expected = null;
			EquivalencyComparer sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, expected);
			string failure = sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsFalse();
			await That(failure).IsEqualTo("""
			                              it was EquivalencyComparerTests.MyClass { MyValue = <null>, Nested = <null> } instead of <null>
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
