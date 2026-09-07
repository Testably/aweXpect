using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Equivalency;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Contains
	{
		public sealed class ItemTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceEnumerable subject = new();

				async Task Act()
					=> await That(subject).Contains(1)
						.And.Contains(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).Contains(5);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Equivalent_InDifferentOrder_ShouldFail()
			{
				IEnumerable<int[]> subject = [[1, 2,], [1, 3,],];

				async Task Act()
					=> await That(subject).Contains([2, 1,]).AtLeast(1).Equivalent();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains [2, 1] equivalent at least once,
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
				IEnumerable<int[]> subject = [[1, 2,], [1, 3,],];

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
				IEnumerable<int> subject = Factory.GetFibonacciNumbers(20);

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
				IEnumerable<int> subject = Factory.GetFibonacciNumbers(20);

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
					               …
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, 2, true)]
			[InlineData(2, 3, true)]
			[InlineData(3, 4, false)]
			public async Task ShouldSupportBetween(int minimum, int maximum, bool expectSuccess)
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers(20);

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
				IEnumerable<MyClass> subject = Factory.GetFibonacciNumbers(20).Select(x => new MyClass(x));
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
				IEnumerable<int> subject = Factory.GetFibonacciNumbers(20);

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
					                …
					              ]
					              """);
			}

			[Theory]
			[InlineData(2, false)]
			[InlineData(3, true)]
			public async Task ShouldSupportLessThan(int maximum, bool expectSuccess)
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers(20);

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
					               …
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			[InlineData(3, false)]
			public async Task ShouldSupportMoreThan(int minimum, bool expectSuccess)
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers(20);

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
				IEnumerable<int> subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(1).Never();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain 1,
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
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task Using_WithAllDifferentComparer_ShouldFail()
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(1).AtLeast(1).Using(new AllDifferentComparer());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains 1 using AllDifferentComparer at least once,
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
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task Using_WithAllEqualComparer_ShouldSucceed()
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(1).AtLeast(4).Using(new AllEqualComparer());

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsExpectedValue_ShouldSucceed(
				List<int> subject, int expected)
			{
				subject.Add(expected);

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableDoesNotContainsExpectedValue_ShouldFail(
				int[] subject, int expected)
			{
				while (subject.Contains(expected))
				{
					expected++;
				}

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              contains {Formatter.Format(expected)} at least once,
					              but it did not contain it

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				int expected = 42;
				IEnumerable<int>? subject = null;

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
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);

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
			[InlineData("a\nb", true)]
			[InlineData("a\nb\n", true)]
			[InlineData("a\n  b", false)]
			public async Task AsBlock_ShouldMatchItemsIndentedAsAWhole(string block, bool expectSuccess)
			{
				string[] subject = ["  a\n  b", "bar", "baz",];

				async Task Act()
					=> await That(subject).Contains(block).AsBlock();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains "{block.Replace("\n", "\\n")}" as block at least once,
					              but it did not contain it

					              Collection:
					              [
					                "  a\n  b",
					                "bar",
					                "baz"
					              ]
					              """);
			}

			[Theory]
			[InlineData("fo", true)]
			[InlineData("oo", false)]
			public async Task AsPrefix_ShouldUsePrefix(string prefix, bool expectSuccess)
			{
				string[] subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject).Contains(prefix).AsPrefix();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains "{prefix}" as prefix at least once,
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
			[InlineData("[a-f]{1}[o]*", true)]
			[InlineData("[g-h]{1}[o]*", false)]
			public async Task AsRegex_ShouldUseRegex(string regex, bool expectSuccess)
			{
				string[] subject = ["foo", "bar", "baz",];

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
			[InlineData("oo", true)]
			[InlineData("fo", false)]
			public async Task AsSuffix_ShouldUsePrefix(string suffix, bool expectSuccess)
			{
				string[] subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject).Contains(suffix).AsSuffix();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains "{suffix}" as suffix at least once,
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
				string[] subject = ["foo", "bar", "baz",];

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

			[Theory]
			[InlineData("foo", true)]
			[InlineData("*oo", false)]
			public async Task Exactly_ShouldUseExactMatch(string match, bool expectSuccess)
			{
				string[] subject = ["foo", "bar", "baz",];
#pragma warning disable aweXpect0001
				StringEqualityTypeCountResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>> expectation =
					That(subject).Contains(match);
				_ = expectation.AsWildcard();

				async Task Act()
					=> await expectation.Exactly();
#pragma warning restore aweXpect0001

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains "{match}" at least once,
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
				string[] subject = ["green", "blue", "blue", "yellow",];

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
				string[] subject = ["green", "blue", "blue", "yellow",];

				async Task Act()
					=> await That(subject).Contains("blue").AtMost(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains "blue" at most once,
					             but it contained it at least twice

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
				string[] subject = ["green", "blue", "blue", "yellow",];

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
				string[] subject = ["green", "blue", "blue", "yellow",];

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
				string[] subject = ["green", "blue", "blue", "yellow",];

				async Task Act()
					=> await That(subject).Contains("blue").LessThan(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains "blue" less than twice,
					             but it contained it at least twice

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
				string[] subject = ["green", "blue", "blue", "yellow",];

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
				string[] subject = ["green", "blue", "blue", "yellow",];

				async Task Act()
					=> await That(subject).Contains("blue").Never();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "blue",
					             but it contained it at least once

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
				string[] subject = ["foo", "bar", "baz",];

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
				string[] subject = [$"fo{nl}o", $"ba{nl}r", $"ba{nl}z",];

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

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string expected = "foo";
				IEnumerable<string>? subject = null;

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
				ThrowWhenIteratingTwiceEnumerable subject = new();

				async Task Act()
					=> await That(subject).Contains(_ => true)
						.And.Contains(_ => true);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers();

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
				IEnumerable<int> subject = Factory.GetFibonacciNumbers(20);

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
				IEnumerable<int> subject = Factory.GetFibonacciNumbers(20);

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
					               …
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, 2, true)]
			[InlineData(2, 3, true)]
			[InlineData(3, 4, false)]
			public async Task ShouldSupportBetween(int minimum, int maximum, bool expectSuccess)
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers(20);

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
				IEnumerable<int> subject = Factory.GetFibonacciNumbers(20);

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
					                …
					              ]
					              """);
			}

			[Theory]
			[InlineData(2, false)]
			[InlineData(3, true)]
			public async Task ShouldSupportLessThan(int maximum, bool expectSuccess)
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers(20);

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
					               …
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			[InlineData(3, false)]
			public async Task ShouldSupportMoreThan(int minimum, bool expectSuccess)
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers(20);

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
				IEnumerable<int> subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => x == 1).Never();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain item matching x => x == 1,
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
					               …
					             ]
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsExpectedValue_ShouldSucceed(
				List<int> subject, int expected)
			{
				subject.Add(expected);

				async Task Act()
					=> await That(subject).Contains(x => x == expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableDoesNotContainsExpectedValue_ShouldFail(
				int[] subject, int expected)
			{
				while (subject.Contains(expected))
				{
					expected++;
				}

				async Task Act()
					=> await That(subject).Contains(x => x == expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              contains item matching x => x == expected at least once,
					              but it did not contain it

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).Contains(predicate: null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The predicate cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

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
