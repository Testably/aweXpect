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
			[Test]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

				async Task Act()
					=> await That(subject).Contains(1)
						.And.Contains(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task DoesNotMaterializeEnumerable()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

				async Task Act()
					=> await That(subject).Contains(5);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(1, true)]
			[Arguments(2, true)]
			[Arguments(3, false)]
			public async Task ShouldSupportAtLeast(int minimum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(1).AtLeast(minimum.Times());

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
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

			[Test]
			[Arguments(1, false)]
			[Arguments(2, true)]
			[Arguments(3, true)]
			public async Task ShouldSupportAtMost(int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(1).AtMost(maximum.Times());

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains an item equal to 1 at most once,
					             but it contained 1 at least twice

					             Collection:
					             [1, 1, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task ShouldSupportAtMost_WhenTheFailureIsDecidedBeforeTheEnd_ShouldStopCounting()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 1);

				async Task Act()
					=> await That(subject).Contains(1).AtMost(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains an item equal to 1 at most once,
					             but it contained 1 at least twice

					             Collection:
					             [1, 1, (… and maybe more)]
					             """)
					.Because("the sync version reports the same input this way");
			}

			[Test]
			public async Task ShouldSupportAtMost_WhenTheSourceThrowsAfterTheFailure_ShouldKeepTheFailure()
			{
				async IAsyncEnumerable<int> ThrowingAfterTwoItems()
				{
					await Task.Yield();
					yield return 1;
					yield return 1;
					throw new InvalidOperationException("enumerated too far");
				}

				async Task Act()
					=> await That(ThrowingAfterTwoItems()).Contains(1).AtMost(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that ThrowingAfterTwoItems()
					             contains an item equal to 1 at most once,
					             but it contained 1 at least twice

					             Collection:
					             [1, 1, (… and maybe more)]
					             """);
			}

			[Test]
			[Arguments(1, 2, true)]
			[Arguments(2, 3, true)]
			[Arguments(3, 4, false)]
			public async Task ShouldSupportBetween(int minimum, int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(1).Between(minimum).And(maximum.Times());

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
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

			[Test]
			public async Task ShouldSupportEquivalent()
			{
				IAsyncEnumerable<MyClass> subject = Factory.GetAsyncFibonacciNumbers(x => new MyClass(x), 20);
				MyClass expected = new(1);

				async Task Act()
					=> await That(subject).Contains(expected).AtLeast(1).Equivalent();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(1, false)]
			[Arguments(2, true)]
			[Arguments(3, false)]
			public async Task ShouldSupportExactly(int times, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(1).Exactly(times);

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains an item equal to 1 exactly {(times == 1 ? "once" : $"{times} times")},
					              but it contained 1 {(times == 1 ? "at least " : "")}twice

					              Collection:
					              {(times == 1
						              ? "[1, 1, (… and maybe more)]"
						              : Formatter.Format(Factory.GetFibonacciNumbers(20).ToArray(), FormattingOptions.MultipleLines))}
					              """);
			}

			[Test]
			[Arguments(2, false)]
			[Arguments(3, true)]
			public async Task ShouldSupportLessThan(int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(1).LessThan(maximum.Times());

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains an item equal to 1 fewer than twice,
					             but it contained 1 at least twice

					             Collection:
					             [1, 1, (… and maybe more)]
					             """);
			}

			[Test]
			[Arguments(1, true)]
			[Arguments(2, false)]
			[Arguments(3, false)]
			public async Task ShouldSupportMoreThan(int minimum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(1).MoreThan(minimum.Times());

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
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

			[Test]
			public async Task ShouldSupportNever()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(2).Never();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain an item equal to 2,
					             but it contained 2 at least once

					             Collection:
					             [1, 1, 2, (… and maybe more)]
					             """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenEnumerableContainsExpectedValue_ShouldSucceed(
				List<int> values, int expected)
			{
				values.Add(expected);
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(values.ToArray());

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
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

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              contains an item equal to {Formatter.Format(expected)} at least once,
					              but it did not contain it

					              Collection:
					              {Formatter.Format(values)}
					              """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				int expected = 42;
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains an item equal to 42 at least once,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WithMultipleFailures_ShouldIncludeCollectionOnlyOnce()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);

				async Task Act()
					=> await That(subject).Contains("d").And.Contains("e");

				await That(Act).Throws<FailException>()
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
			[Test]
			[Arguments("[a-f]{1}[o]*", true)]
			[Arguments("[g-h]{1}[o]*", false)]
			public async Task AsRegex_ShouldUseRegex(string regex, bool expectSuccess)
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).Contains(regex).AsRegex();

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
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

			[Test]
			[Arguments("?oo", true)]
			[Arguments("f??o", false)]
			public async Task AsWildcard_ShouldUseWildcard(string wildcard, bool expectSuccess)
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).Contains(wildcard).AsWildcard();

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
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

			[Test]
			public async Task ShouldCompareCaseSensitive()
			{
				IAsyncEnumerable<string> sut = ToAsyncEnumerable(["green", "blue", "yellow",]);

				async Task Act()
					=> await That(sut).Contains("GREEN");

				await That(Act).Throws<FailException>()
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

			[Test]
			[Arguments(1, true)]
			[Arguments(2, true)]
			[Arguments(3, false)]
			public async Task ShouldSupportAtLeast(int minimum, bool expectSuccess)
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["green", "blue", "blue", "yellow",]);

				async Task Act()
					=> await That(subject).Contains("blue").AtLeast(minimum.Times());

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
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

			[Test]
			[Arguments(1, false)]
			[Arguments(2, true)]
			[Arguments(3, true)]
			public async Task ShouldSupportAtMost(int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["green", "blue", "blue", "yellow",]);

				async Task Act()
					=> await That(subject).Contains("blue").AtMost(maximum.Times());

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains "blue" at most once,
					             but it contained "blue" at least twice

					             Collection:
					             [
					               "green",
					               "blue",
					               "blue",
					               (… and maybe more)
					             ]
					             """);
			}

			[Test]
			[Arguments(1, 2, true)]
			[Arguments(2, 3, true)]
			[Arguments(3, 4, false)]
			public async Task ShouldSupportBetween(int minimum, int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["green", "blue", "blue", "yellow",]);

				async Task Act()
					=> await That(subject).Contains("blue").Between(minimum).And(maximum.Times());

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
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

			[Test]
			[Arguments(1, true)]
			[Arguments(2, false)]
			[Arguments(3, false)]
			public async Task ShouldSupportExactly(int times, bool expectSuccess)
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["green", "blue", "blue", "yellow",]);

				async Task Act()
					=> await That(subject).Contains("yellow").Exactly(times);

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
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

			[Test]
			[Arguments(2, false)]
			[Arguments(3, true)]
			public async Task ShouldSupportLessThan(int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["green", "blue", "blue", "yellow",]);

				async Task Act()
					=> await That(subject).Contains("blue").LessThan(maximum.Times());

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains "blue" fewer than twice,
					             but it contained "blue" at least twice

					             Collection:
					             [
					               "green",
					               "blue",
					               "blue",
					               (… and maybe more)
					             ]
					             """);
			}

			[Test]
			[Arguments(1, true)]
			[Arguments(2, false)]
			[Arguments(3, false)]
			public async Task ShouldSupportMoreThan(int minimum, bool expectSuccess)
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["green", "blue", "blue", "yellow",]);

				async Task Act()
					=> await That(subject).Contains("blue").MoreThan(minimum.Times());

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
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

			[Test]
			public async Task ShouldSupportNever()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["green", "blue", "blue", "yellow",]);

				async Task Act()
					=> await That(subject).Contains("yellow").Never();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "yellow",
					             but it contained "yellow" at least once

					             Collection:
					             [
					               "green",
					               "blue",
					               "blue",
					               "yellow",
					               (… and maybe more)
					             ]
					             """);
			}

			[Test]
			public async Task WhenExpectedIsNotPartOfStringEnumerable_ShouldFail()
			{
				IAsyncEnumerable<string> sut = ToAsyncEnumerable(["green", "blue", "yellow",]);

				async Task Act()
					=> await That(sut).Contains("red");

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenExpectedIsPartOfStringEnumerable_ShouldSucceed()
			{
				IAsyncEnumerable<string> sut = ToAsyncEnumerable(["green", "blue", "yellow",]);

				async Task Act()
					=> await That(sut).Contains("blue");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenIgnoringCase_ShouldSucceedForCaseSensitiveDifference()
			{
				IAsyncEnumerable<string> sut = ToAsyncEnumerable(["green", "blue", "yellow",]);

				async Task Act()
					=> await That(sut).Contains("GREEN").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(" foo", true)]
			[Arguments("goo", false)]
			public async Task WhenIgnoringLeadingWhiteSpace_ShouldIgnoreLeadingWhiteSpace(string match,
				bool expectSuccess)
			{
				IAsyncEnumerable<string> sut = ToAsyncEnumerable(["  foo", "bar", "baz",]);

				async Task Act()
					=> await That(sut).Contains(match).IgnoringLeadingWhiteSpace();

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that sut
					              contains {Formatter.Format(match)} ignoring leading whitespace at least once,
					              but it did not contain it

					              Collection:
					              [
					                "  foo",
					                "bar",
					                "baz"
					              ]
					              """);
			}

			[Test]
			[Arguments("fo\ro", true)]
			[Arguments("go\ro", false)]
			public async Task WhenIgnoringNewlineStyle_ShouldIgnoreNewlineStyle(string match, bool expectSuccess)
			{
				string nl = Environment.NewLine;
				IAsyncEnumerable<string> sut = ToAsyncEnumerable([$"fo{nl}o", $"ba{nl}r", $"ba{nl}z",]);

				async Task Act()
					=> await That(sut).Contains(match).IgnoringNewlineStyle();

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
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

			[Test]
			[Arguments("foo ", true)]
			[Arguments("goo", false)]
			public async Task WhenIgnoringTrailingWhiteSpace_ShouldIgnoreTrailingWhiteSpace(string match,
				bool expectSuccess)
			{
				IAsyncEnumerable<string> sut = ToAsyncEnumerable(["foo  ", "bar", "baz",]);

				async Task Act()
					=> await That(sut).Contains(match).IgnoringTrailingWhiteSpace();

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that sut
					              contains {Formatter.Format(match)} ignoring trailing whitespace at least once,
					              but it did not contain it

					              Collection:
					              [
					                "foo  ",
					                "bar",
					                "baz"
					              ]
					              """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string expected = "foo";
				IAsyncEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains "foo" at least once,
					             but it was <null>
					             """);
			}
		}

		public sealed class PredicateTests
		{
			[Test]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

				async Task Act()
					=> await That(subject).Contains(_ => true)
						.And.Contains(_ => true);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task DoesNotMaterializeEnumerable()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

				async Task Act()
					=> await That(subject).Contains(x => x == 5);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(1, true)]
			[Arguments(2, true)]
			[Arguments(3, false)]
			public async Task ShouldSupportAtLeast(int minimum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => x == 1).AtLeast(minimum.Times());

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
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

			[Test]
			[Arguments(1, false)]
			[Arguments(2, true)]
			[Arguments(3, true)]
			public async Task ShouldSupportAtMost(int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => x == 1).AtMost(maximum.Times());

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains an item matching x => x == 1 at most once,
					             but it contained it at least twice

					             Collection:
					             [1, 1, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task ShouldSupportAtMost_WhenTheFailureIsDecidedBeforeTheEnd_ShouldStopCounting()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 1);

				async Task Act()
					=> await That(subject).Contains(x => x == 1).AtMost(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains an item matching x => x == 1 at most once,
					             but it contained it at least twice

					             Collection:
					             [1, 1, (… and maybe more)]
					             """)
					.Because("the sync version reports the same input this way");
			}

			[Test]
			[Arguments(1, 2, true)]
			[Arguments(2, 3, true)]
			[Arguments(3, 4, false)]
			public async Task ShouldSupportBetween(int minimum, int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => x == 1).Between(minimum).And(maximum.Times());

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
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

			[Test]
			[Arguments(1, false)]
			[Arguments(2, true)]
			[Arguments(3, false)]
			public async Task ShouldSupportExactly(int times, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => x == 1).Exactly(times);

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains an item matching x => x == 1 exactly {(times == 1 ? "once" : $"{times} times")},
					              but it contained it {(times == 1 ? "at least " : "")}twice

					              Collection:
					              {(times == 1
						              ? "[1, 1, (… and maybe more)]"
						              : Formatter.Format(Factory.GetFibonacciNumbers(20).ToArray(), FormattingOptions.MultipleLines))}
					              """);
			}

			[Test]
			[Arguments(2, false)]
			[Arguments(3, true)]
			public async Task ShouldSupportLessThan(int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => x == 1).LessThan(maximum.Times());

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains an item matching x => x == 1 fewer than twice,
					             but it contained it at least twice

					             Collection:
					             [1, 1, (… and maybe more)]
					             """);
			}

			[Test]
			[Arguments(1, true)]
			[Arguments(2, false)]
			[Arguments(3, false)]
			public async Task ShouldSupportMoreThan(int minimum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => x == 1).MoreThan(minimum.Times());

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
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

			[Test]
			public async Task ShouldSupportNever()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => x == 2).Never();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain an item matching x => x == 2,
					             but it contained it at least once

					             Collection:
					             [1, 1, 2, (… and maybe more)]
					             """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenEnumerableContainsExpectedValue_ShouldSucceed(
				List<int> values, int expected)
			{
				values.Add(expected);
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(values.ToArray());

				async Task Act()
					=> await That(subject).Contains(x => x == expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
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

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              contains an item matching x => x == expected at least once,
					              but it did not contain it

					              Collection:
					              {Formatter.Format(values)}
					              """);
			}

			[Test]
			public async Task WhenEnumeratingTheSubjectThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("enumeration failed");
				IAsyncEnumerable<int> subject = ThrowAfter(exception, 1, 2);

				async Task Act()
					=> await That(subject).Contains(x => x == 3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains an item matching x => x == 3 at least once,
					             but it did throw an InvalidOperationException:
					               enumeration failed
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a subject that cannot be enumerated fails the expectation instead of aborting its evaluation");
			}

			[Test]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

				async Task Act()
					=> await That(subject).Contains(predicate: null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenPredicateThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("predicate failed");
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).Contains(x => x == 2 ? throw exception : false);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains an item matching x => x == 2 ? throw exception : false at least once,
					             but the predicate did throw an InvalidOperationException:
					               predicate failed
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a predicate that throws fails the expectation instead of aborting its evaluation");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).Contains(_ => true);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains an item matching _ => true at least once,
					             but it was <null>
					             """);
			}
		}
	}
}
#endif
