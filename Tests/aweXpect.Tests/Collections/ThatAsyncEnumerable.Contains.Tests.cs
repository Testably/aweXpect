#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class Contains
	{
		public sealed class ItemTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

				async Task Act()
					=> await That(subject).Contains(1)
						.And.Contains(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

				async Task Act()
					=> await That(subject).Contains(5);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, true)]
			[InlineData(3, false)]
			public async Task ShouldSupportAtLeast(int minimum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(1).AtLeast(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains 1 at least {minimum} times,
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
					                …
					              ]
					              """);
			}

			[Theory]
			[InlineData(1, false)]
			[InlineData(2, true)]
			[InlineData(3, true)]
			public async Task ShouldSupportAtMost(int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(1).AtMost(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains 1 at most once,
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
					               (… and maybe others)
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, 2, true)]
			[InlineData(2, 3, true)]
			[InlineData(3, 4, false)]
			public async Task ShouldSupportBetween(int minimum, int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(1).Between(minimum).And(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains 1 between {minimum} and {maximum} times,
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
					                …
					              ]
					              """);
			}

			[Fact]
			public async Task ShouldSupportEquivalent()
			{
				IAsyncEnumerable<MyClass> subject = Factory.GetAsyncFibonacciNumbers(x => new MyClass(x), 20);
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
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(1).Exactly(times);

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains 1 exactly {(times == 1 ? "once" : $"{times} times")},
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
					                {(times == 1 ? "(… and maybe others)" : "…")}
					              ]
					              """);
			}

			[Theory]
			[InlineData(2, false)]
			[InlineData(3, true)]
			public async Task ShouldSupportLessThan(int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(1).LessThan(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains 1 less than twice,
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
					               (… and maybe others)
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			[InlineData(3, false)]
			public async Task ShouldSupportMoreThan(int minimum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(1).MoreThan(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains 1 more than {minimum.ToTimesString()},
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
					                …
					              ]
					              """);
			}

			[Fact]
			public async Task ShouldSupportNever()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(2).Never();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain 2,
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
					               (… and maybe others)
					             ]
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsExpectedValue_ShouldSucceed(
				List<int> values, int expected)
			{
				values.Add(expected);
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(values.ToArray());

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableDoesNotContainsExpectedValue_ShouldFail(
				int[] values, int expected)
			{
				while (values.Contains(expected))
				{
					expected++;
				}

				IAsyncEnumerable<int> subject = ToAsyncEnumerable(values);

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              contains {Formatter.Format(expected)} at least once,
					              but it did not contain it

					              Collection:
					              {Formatter.Format(values)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				int expected = 42;
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains 42 at least once,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithMultipleFailures_ShouldIncludeCollectionOnlyOnce()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);

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

		public sealed class StringItemTests
		{
			[Theory]
			[InlineData("[a-f]{1}[o]*", true)]
			[InlineData("[g-h]{1}[o]*", false)]
			public async Task AsRegex_ShouldUseRegex(string regex, bool expectSuccess)
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).Contains(regex).AsRegex();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains "{regex}" as regex at least once,
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).Contains(wildcard).AsWildcard();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains "{wildcard}" as wildcard at least once,
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
				IAsyncEnumerable<string> sut = ToAsyncEnumerable(["green", "blue", "yellow",]);

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["green", "blue", "blue", "yellow",]);

				async Task Act()
					=> await That(subject).Contains("blue").AtLeast(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains "blue" at least {minimum} times,
					              but it contained it twice

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["green", "blue", "blue", "yellow",]);

				async Task Act()
					=> await That(subject).Contains("blue").AtMost(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains "blue" at most once,
					             but it contained it twice

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["green", "blue", "blue", "yellow",]);

				async Task Act()
					=> await That(subject).Contains("blue").Between(minimum).And(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains "blue" between {minimum} and {maximum} times,
					              but it contained it twice

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["green", "blue", "blue", "yellow",]);

				async Task Act()
					=> await That(subject).Contains("yellow").Exactly(times);

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains "yellow" exactly {times.ToTimesString()},
					              but it contained it once

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["green", "blue", "blue", "yellow",]);

				async Task Act()
					=> await That(subject).Contains("blue").LessThan(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains "blue" less than twice,
					             but it contained it twice

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["green", "blue", "blue", "yellow",]);

				async Task Act()
					=> await That(subject).Contains("blue").MoreThan(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains "blue" more than {minimum.ToTimesString()},
					              but it contained it twice

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["green", "blue", "blue", "yellow",]);

				async Task Act()
					=> await That(subject).Contains("yellow").Never();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "yellow",
					             but it contained it once

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
				IAsyncEnumerable<string> sut = ToAsyncEnumerable(["green", "blue", "yellow",]);

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
			public async Task WhenExpectedIsPartOfStringEnumerable_ShouldSucceed()
			{
				IAsyncEnumerable<string> sut = ToAsyncEnumerable(["green", "blue", "yellow",]);

				async Task Act()
					=> await That(sut).Contains("blue");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenIgnoringCase_ShouldSucceedForCaseSensitiveDifference()
			{
				IAsyncEnumerable<string> sut = ToAsyncEnumerable(["green", "blue", "yellow",]);

				async Task Act()
					=> await That(sut).Contains("GREEN").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("fo\ro", true)]
			[InlineData("go\ro", false)]
			public async Task WhenIgnoringNewlineStyle_ShouldIgnoreNewlineStyle(string match, bool expectSuccess)
			{
				string nl = Environment.NewLine;
				IAsyncEnumerable<string> sut = ToAsyncEnumerable([$"fo{nl}o", $"ba{nl}r", $"ba{nl}z",]);

				async Task Act()
					=> await That(sut).Contains(match).IgnoringNewlineStyle();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that sut
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

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string expected = "foo";
				IAsyncEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "foo" at least once,
					             but it was <null>
					             """);
			}
		}

		public sealed class PredicateTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

				async Task Act()
					=> await That(subject).Contains(_ => true)
						.And.Contains(_ => true);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

				async Task Act()
					=> await That(subject).Contains(x => x == 5);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, true)]
			[InlineData(3, false)]
			public async Task ShouldSupportAtLeast(int minimum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => x == 1).AtLeast(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains item matching x => x == 1 at least {minimum} times,
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
					                …
					              ]
					              """);
			}

			[Theory]
			[InlineData(1, false)]
			[InlineData(2, true)]
			[InlineData(3, true)]
			public async Task ShouldSupportAtMost(int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => x == 1).AtMost(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains item matching x => x == 1 at most once,
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
					               (… and maybe others)
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, 2, true)]
			[InlineData(2, 3, true)]
			[InlineData(3, 4, false)]
			public async Task ShouldSupportBetween(int minimum, int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => x == 1).Between(minimum).And(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains item matching x => x == 1 between {minimum} and {maximum} times,
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
					                …
					              ]
					              """);
			}

			[Theory]
			[InlineData(1, false)]
			[InlineData(2, true)]
			[InlineData(3, false)]
			public async Task ShouldSupportExactly(int times, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => x == 1).Exactly(times);

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains item matching x => x == 1 exactly {(times == 1 ? "once" : $"{times} times")},
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
					                {(times == 1 ? "(… and maybe others)" : "…")}
					              ]
					              """);
			}

			[Theory]
			[InlineData(2, false)]
			[InlineData(3, true)]
			public async Task ShouldSupportLessThan(int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => x == 1).LessThan(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains item matching x => x == 1 less than twice,
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
					               (… and maybe others)
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			[InlineData(3, false)]
			public async Task ShouldSupportMoreThan(int minimum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => x == 1).MoreThan(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains item matching x => x == 1 more than {minimum.ToTimesString()},
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
					                …
					              ]
					              """);
			}

			[Fact]
			public async Task ShouldSupportNever()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => x == 2).Never();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain item matching x => x == 2,
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
					               (… and maybe others)
					             ]
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsExpectedValue_ShouldSucceed(
				List<int> values, int expected)
			{
				values.Add(expected);
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(values.ToArray());

				async Task Act()
					=> await That(subject).Contains(x => x == expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableDoesNotContainsExpectedValue_ShouldFail(
				int[] values, int expected)
			{
				while (values.Contains(expected))
				{
					expected++;
				}

				IAsyncEnumerable<int> subject = ToAsyncEnumerable(values.ToArray());

				async Task Act()
					=> await That(subject).Contains(x => x == expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              contains item matching x => x == expected at least once,
					              but it did not contain it

					              Collection:
					              {Formatter.Format(values)}
					              """);
			}

			[Fact]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

				async Task Act()
					=> await That(subject).Contains(predicate: null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The predicate cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).Contains(_ => true);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains item matching _ => true at least once,
					             but it was <null>
					             """);
			}
		}
	}
}
#endif
