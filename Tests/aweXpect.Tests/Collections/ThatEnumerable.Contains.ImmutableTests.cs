#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using System.Linq;
using aweXpect.Core;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Contains
	{
		public sealed class ImmutableItemTests
		{
			[Fact]
			public async Task Equivalent_InDifferentOrder_ShouldFail()
			{
				ImmutableArray<int[]> subject = [[1, 2,], [1, 3,],];

				async Task Act()
					=> await That(subject).Contains([2, 1,]).AtLeast(1).Equivalent();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains an item equivalent to [2, 1] at least once,
					             but it did not contain it

					             Collection:
					             [
					               [
					                 1,
					                 2
					               ],
					               [
					                 1,
					                 3
					               ]
					             ]
					             """);
			}

			[Fact]
			public async Task Equivalent_InDifferentOrder_WhenIgnoringCollectionOrder_ShouldSucceed()
			{
				ImmutableArray<int[]> subject = [[1, 2,], [1, 3,],];

				async Task Act()
					=> await That(subject).Contains([2, 1,]).AtLeast(1).Equivalent(o => o.IgnoringCollectionOrder());

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, true)]
			[InlineData(3, false)]
			public async Task ShouldSupportAtLeast(int minimum, bool expectSuccess)
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).Contains(1).AtLeast(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains an item equal to 1 at least {minimum} times,
					              but it contained 1 twice

					              Collection:
					              [
					                1,
					                1,
					                2,
					                3,
					                5,
					                8,
					                13,
					                21,
					                34,
					                55,
					                (… and 10 more)
					              ]
					              """);
			}

			[Theory]
			[InlineData(1, false)]
			[InlineData(2, true)]
			[InlineData(3, true)]
			public async Task ShouldSupportAtMost(int maximum, bool expectSuccess)
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).Contains(1).AtMost(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains an item equal to 1 at most once,
					             but it contained 1 at least twice

					             Collection:
					             [
					               1,
					               1,
					               2,
					               3,
					               5,
					               8,
					               13,
					               21,
					               34,
					               55,
					               (… and 10 more)
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, 2, true)]
			[InlineData(2, 3, true)]
			[InlineData(3, 4, false)]
			public async Task ShouldSupportBetween(int minimum, int maximum, bool expectSuccess)
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).Contains(1).Between(minimum).And(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains an item equal to 1 between {minimum} and {maximum} times,
					              but it contained 1 twice

					              Collection:
					              [
					                1,
					                1,
					                2,
					                3,
					                5,
					                8,
					                13,
					                21,
					                34,
					                55,
					                (… and 10 more)
					              ]
					              """);
			}

			[Fact]
			public async Task ShouldSupportEquivalent()
			{
				ImmutableArray<MyClass> subject = [..Factory.GetFibonacciNumbers(20).Select(x => new MyClass(x)),];
				MyClass expected = new(1);

				async Task Act()
					=> await That(subject).Contains(expected).AtLeast(1).Equivalent();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1, false)]
			[InlineData(2, true)]
			[InlineData(3, false)]
			public async Task ShouldSupportExactly(int times, bool expectSuccess)
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).Contains(1).Exactly(times);

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains an item equal to 1 exactly {(times == 1 ? "once" : $"{times} times")},
					              but it contained 1 {(times == 1 ? "at least " : "")}twice

					              Collection:
					              [
					                1,
					                1,
					                2,
					                3,
					                5,
					                8,
					                13,
					                21,
					                34,
					                55,
					                (… and 10 more)
					              ]
					              """);
			}

			[Theory]
			[InlineData(2, false)]
			[InlineData(3, true)]
			public async Task ShouldSupportLessThan(int maximum, bool expectSuccess)
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).Contains(1).LessThan(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains an item equal to 1 fewer than twice,
					             but it contained 1 at least twice

					             Collection:
					             [
					               1,
					               1,
					               2,
					               3,
					               5,
					               8,
					               13,
					               21,
					               34,
					               55,
					               (… and 10 more)
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			[InlineData(3, false)]
			public async Task ShouldSupportMoreThan(int minimum, bool expectSuccess)
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).Contains(1).MoreThan(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains an item equal to 1 more than {minimum.ToTimesString()},
					              but it contained 1 twice

					              Collection:
					              [
					                1,
					                1,
					                2,
					                3,
					                5,
					                8,
					                13,
					                21,
					                34,
					                55,
					                (… and 10 more)
					              ]
					              """);
			}

			[Fact]
			public async Task ShouldSupportNever()
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).Contains(1).Never();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain an item equal to 1,
					             but it contained 1 at least once

					             Collection:
					             [
					               1,
					               1,
					               2,
					               3,
					               5,
					               8,
					               13,
					               21,
					               34,
					               55,
					               (… and 10 more)
					             ]
					             """);
			}

			[Fact]
			public async Task Using_WithAllDifferentComparer_ShouldFail()
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).Contains(1).AtLeast(1).Using(new AllDifferentComparer());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains an item equal to 1 using AllDifferentComparer at least once,
					             but it did not contain it

					             Collection:
					             [
					               1,
					               1,
					               2,
					               3,
					               5,
					               8,
					               13,
					               21,
					               34,
					               55,
					               (… and 10 more)
					             ]
					             """);
			}

			[Fact]
			public async Task Using_WithAllEqualComparer_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).Contains(1).AtLeast(4).Using(new AllEqualComparer());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsExpectedValue_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				int expected = 3;

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableDoesNotContainsExpectedValue_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				int expected = 4;

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              contains an item equal to {Formatter.Format(expected)} at least once,
					              but it did not contain it

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenExpectedIsNullLiteral_ShouldMatchTheNullItem()
			{
				ImmutableArray<int?> subject = [1, null, 3,];

				async Task Act()
					=> await That(subject).Contains(null).Once();

				await That(Act).DoesNotThrow()
					.Because("a null item is an ordinary value for a nullable element type");
			}

			[Fact]
			public async Task WhenExpectedIsNullLiteral_WithoutANullItem_ShouldFail()
			{
				ImmutableArray<int?> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).Contains(null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains an item equal to <null> at least once,
					             but it did not contain it

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WithMultipleFailures_ShouldIncludeCollectionOnlyOnce()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains("d").And.Contains("e");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "d" at least once and contains "e" at least once,
					             but it did not contain it

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}
		}

		public sealed class ImmutableStringItemTests
		{
			[Theory]
			[InlineData("[a-f]{1}[o]*", true)]
			[InlineData("[g-h]{1}[o]*", false)]
			public async Task AsRegex_ShouldUseRegex(string regex, bool expectSuccess)
			{
				ImmutableArray<string?> subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject).Contains(regex).AsRegex();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains an item matching regex "{regex}" at least once,
					              but it did not contain it

					              Collection:
					              [
					                "foo",
					                "bar",
					                "baz"
					              ]
					              """);
			}

			[Theory]
			[InlineData("?oo", true)]
			[InlineData("f??o", false)]
			public async Task AsWildcard_ShouldUseWildcard(string wildcard, bool expectSuccess)
			{
				ImmutableArray<string?> subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject).Contains(wildcard).AsWildcard();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains an item matching "{wildcard}" at least once,
					              but it did not contain it

					              Collection:
					              [
					                "foo",
					                "bar",
					                "baz"
					              ]
					              """);
			}

			[Fact]
			public async Task ShouldCompareCaseSensitive()
			{
				string[] sut = ["green", "blue", "yellow",];

				async Task Act()
					=> await That(sut).Contains("GREEN");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that sut
					             contains "GREEN" at least once,
					             but it did not contain it

					             Collection:
					             [
					               "green",
					               "blue",
					               "yellow"
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, true)]
			[InlineData(3, false)]
			public async Task ShouldSupportAtLeast(int minimum, bool expectSuccess)
			{
				ImmutableArray<string?> subject = ["green", "blue", "blue", "yellow",];

				async Task Act()
					=> await That(subject).Contains("blue").AtLeast(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains "blue" at least {minimum} times,
					              but it contained "blue" twice

					              Collection:
					              [
					                "green",
					                "blue",
					                "blue",
					                "yellow"
					              ]
					              """);
			}

			[Theory]
			[InlineData(1, false)]
			[InlineData(2, true)]
			[InlineData(3, true)]
			public async Task ShouldSupportAtMost(int maximum, bool expectSuccess)
			{
				ImmutableArray<string?> subject = ["green", "blue", "blue", "yellow",];

				async Task Act()
					=> await That(subject).Contains("blue").AtMost(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains "blue" at most once,
					             but it contained "blue" at least twice

					             Collection:
					             [
					               "green",
					               "blue",
					               "blue",
					               "yellow"
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, 2, true)]
			[InlineData(2, 3, true)]
			[InlineData(3, 4, false)]
			public async Task ShouldSupportBetween(int minimum, int maximum, bool expectSuccess)
			{
				ImmutableArray<string?> subject = ["green", "blue", "blue", "yellow",];

				async Task Act()
					=> await That(subject).Contains("blue").Between(minimum).And(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains "blue" between {minimum} and {maximum} times,
					              but it contained "blue" twice

					              Collection:
					              [
					                "green",
					                "blue",
					                "blue",
					                "yellow"
					              ]
					              """);
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			[InlineData(3, false)]
			public async Task ShouldSupportExactly(int times, bool expectSuccess)
			{
				ImmutableArray<string?> subject = ["green", "blue", "blue", "yellow",];

				async Task Act()
					=> await That(subject).Contains("yellow").Exactly(times);

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains "yellow" exactly {times.ToTimesString()},
					              but it contained "yellow" once

					              Collection:
					              [
					                "green",
					                "blue",
					                "blue",
					                "yellow"
					              ]
					              """);
			}

			[Theory]
			[InlineData(2, false)]
			[InlineData(3, true)]
			public async Task ShouldSupportLessThan(int maximum, bool expectSuccess)
			{
				ImmutableArray<string?> subject = ["green", "blue", "blue", "yellow",];

				async Task Act()
					=> await That(subject).Contains("blue").LessThan(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains "blue" fewer than twice,
					             but it contained "blue" at least twice

					             Collection:
					             [
					               "green",
					               "blue",
					               "blue",
					               "yellow"
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			[InlineData(3, false)]
			public async Task ShouldSupportMoreThan(int minimum, bool expectSuccess)
			{
				ImmutableArray<string?> subject = ["green", "blue", "blue", "yellow",];

				async Task Act()
					=> await That(subject).Contains("blue").MoreThan(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains "blue" more than {minimum.ToTimesString()},
					              but it contained "blue" twice

					              Collection:
					              [
					                "green",
					                "blue",
					                "blue",
					                "yellow"
					              ]
					              """);
			}

			[Fact]
			public async Task ShouldSupportNever()
			{
				ImmutableArray<string?> subject = ["green", "blue", "blue", "yellow",];

				async Task Act()
					=> await That(subject).Contains("blue").Never();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "blue",
					             but it contained "blue" at least once

					             Collection:
					             [
					               "green",
					               "blue",
					               "blue",
					               "yellow"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsNotPartOfStringEnumerable_ShouldFail()
			{
				string[] sut = ["green", "blue", "yellow",];

				async Task Act()
					=> await That(sut).Contains("red");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that sut
					             contains "red" at least once,
					             but it did not contain it

					             Collection:
					             [
					               "green",
					               "blue",
					               "yellow"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsNullLiteral_ShouldMatchTheNullItem()
			{
				ImmutableArray<string?> subject = ["green", null,];

				async Task Act()
					=> await That(subject).Contains(null).Once().IgnoringCase();

				await That(Act).DoesNotThrow()
					.Because("a null item is an ordinary value for a nullable element type");
			}

			[Fact]
			public async Task WhenExpectedIsNullLiteral_WithoutANullItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["green", "blue",];

				async Task Act()
					=> await That(subject).Contains((string?)null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains <null> at least once,
					             but it did not contain it

					             Collection:
					             [
					               "green",
					               "blue"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsPartOfStringEnumerable_ShouldSucceed()
			{
				string[] sut = ["green", "blue", "yellow",];

				async Task Act()
					=> await That(sut).Contains("blue");

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("FOO", true)]
			[InlineData("goo", false)]
			public async Task WhenIgnoringCase_ShouldUseCaseInsensitiveMatch(string match, bool expectSuccess)
			{
				ImmutableArray<string?> subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject).Contains(match).IgnoringCase();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains {Formatter.Format(match)} ignoring case at least once,
					              but it did not contain it

					              Collection:
					              [
					                "foo",
					                "bar",
					                "baz"
					              ]
					              """);
			}

			[Theory]
			[InlineData("fo\ro", true)]
			[InlineData("go\ro", false)]
			public async Task WhenIgnoringNewlineStyle_ShouldIgnoreNewlineStyle(string match, bool expectSuccess)
			{
				string nl = Environment.NewLine;
				ImmutableArray<string?> subject = [$"fo{nl}o", $"ba{nl}r", $"ba{nl}z",];

				async Task Act()
					=> await That(subject).Contains(match).IgnoringNewlineStyle();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains {Formatter.Format(match)} ignoring newline style at least once,
					              but it did not contain it

					              Collection:
					              [
					                "fo{nl.DisplayWhitespace()}o",
					                "ba{nl.DisplayWhitespace()}r",
					                "ba{nl.DisplayWhitespace()}z"
					              ]
					              """);
			}
		}

		public sealed class ImmutablePredicateTests
		{
			[Theory]
			[InlineData(1, true)]
			[InlineData(2, true)]
			[InlineData(3, false)]
			public async Task ShouldSupportAtLeast(int minimum, bool expectSuccess)
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).Contains(x => x == 1).AtLeast(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains an item matching x => x == 1 at least {minimum} times,
					              but it contained it twice

					              Collection:
					              [
					                1,
					                1,
					                2,
					                3,
					                5,
					                8,
					                13,
					                21,
					                34,
					                55,
					                (… and 10 more)
					              ]
					              """);
			}

			[Theory]
			[InlineData(1, false)]
			[InlineData(2, true)]
			[InlineData(3, true)]
			public async Task ShouldSupportAtMost(int maximum, bool expectSuccess)
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).Contains(x => x == 1).AtMost(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains an item matching x => x == 1 at most once,
					             but it contained it at least twice

					             Collection:
					             [
					               1,
					               1,
					               2,
					               3,
					               5,
					               8,
					               13,
					               21,
					               34,
					               55,
					               (… and 10 more)
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, 2, true)]
			[InlineData(2, 3, true)]
			[InlineData(3, 4, false)]
			public async Task ShouldSupportBetween(int minimum, int maximum, bool expectSuccess)
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).Contains(x => x == 1).Between(minimum).And(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains an item matching x => x == 1 between {minimum} and {maximum} times,
					              but it contained it twice

					              Collection:
					              [
					                1,
					                1,
					                2,
					                3,
					                5,
					                8,
					                13,
					                21,
					                34,
					                55,
					                (… and 10 more)
					              ]
					              """);
			}

			[Theory]
			[InlineData(1, false)]
			[InlineData(2, true)]
			[InlineData(3, false)]
			public async Task ShouldSupportExactly(int times, bool expectSuccess)
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).Contains(x => x == 1).Exactly(times);

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains an item matching x => x == 1 exactly {(times == 1 ? "once" : $"{times} times")},
					              but it contained it {(times == 1 ? "at least " : "")}twice

					              Collection:
					              [
					                1,
					                1,
					                2,
					                3,
					                5,
					                8,
					                13,
					                21,
					                34,
					                55,
					                (… and 10 more)
					              ]
					              """);
			}

			[Theory]
			[InlineData(2, false)]
			[InlineData(3, true)]
			public async Task ShouldSupportLessThan(int maximum, bool expectSuccess)
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).Contains(x => x == 1).LessThan(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains an item matching x => x == 1 fewer than twice,
					             but it contained it at least twice

					             Collection:
					             [
					               1,
					               1,
					               2,
					               3,
					               5,
					               8,
					               13,
					               21,
					               34,
					               55,
					               (… and 10 more)
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			[InlineData(3, false)]
			public async Task ShouldSupportMoreThan(int minimum, bool expectSuccess)
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).Contains(x => x == 1).MoreThan(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains an item matching x => x == 1 more than {minimum.ToTimesString()},
					              but it contained it twice

					              Collection:
					              [
					                1,
					                1,
					                2,
					                3,
					                5,
					                8,
					                13,
					                21,
					                34,
					                55,
					                (… and 10 more)
					              ]
					              """);
			}

			[Fact]
			public async Task ShouldSupportNever()
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).Contains(x => x == 1).Never();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain an item matching x => x == 1,
					             but it contained it at least once

					             Collection:
					             [
					               1,
					               1,
					               2,
					               3,
					               5,
					               8,
					               13,
					               21,
					               34,
					               55,
					               (… and 10 more)
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsExpectedValue_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				int expected = 3;

				async Task Act()
					=> await That(subject).Contains(x => x == expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableDoesNotContainsExpectedValue_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				int expected = 4;

				async Task Act()
					=> await That(subject).Contains(x => x == expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              contains an item matching x => x == expected at least once,
					              but it did not contain it

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				ImmutableArray<int> subject = [1, 2,];

				async Task Act()
					=> await That(subject).Contains(predicate: null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenPredicateThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("predicate failed");
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).Contains(x => x == 2 ? throw exception : false);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains an item matching x => x == 2 ? throw exception : false at least once,
					             but the predicate did throw an InvalidOperationException:
					               predicate failed
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a predicate that throws fails the expectation instead of aborting its evaluation");
			}
		}
	}
}
#endif
