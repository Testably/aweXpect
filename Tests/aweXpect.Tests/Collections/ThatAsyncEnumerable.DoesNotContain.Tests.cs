#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class DoesNotContain
	{
		public sealed class ItemTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

				async Task Act()
					=> await That(subject).DoesNotContain(0)
						.And.DoesNotContain(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

				async Task Act()
					=> await That(subject).DoesNotContain(5);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain an item equal to 5,
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
					               (… and maybe more)
					             ]
					             """);
			}

			[Theory]
			[InlineData(2, false)]
			[InlineData(3, true)]
			public async Task ShouldSupportAtLeast(int minimum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).DoesNotContain(1).AtLeast(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain an item equal to 1 at least {minimum.ToTimesString()},
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
					                (… and maybe more)
					              ]
					              """);
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			public async Task ShouldSupportAtMost(int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).DoesNotContain(1).AtMost(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             does not contain an item equal to 1 at most twice,
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
			[InlineData(1, 2, false)]
			[InlineData(2, 3, false)]
			[InlineData(3, 4, true)]
			public async Task ShouldSupportBetween(int minimum, int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).DoesNotContain(1).Between(minimum).And(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain an item equal to 1 between {minimum} and {maximum} times,
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
			public async Task ShouldSupportEquivalent()
			{
				IAsyncEnumerable<MyClass> subject = Factory.GetAsyncFibonacciNumbers(x => new MyClass(x), 20);
				MyClass unexpected = new(5);

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Equivalent();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain an item equivalent to MyClass {
					               StringValue = "",
					               Value = 5
					             },
					             but it contained it at least once

					             Collection:
					             [
					               MyClass {
					                 StringValue = "",
					                 Value = 1
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 1
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 2
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 3
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 5
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 8
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 13
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 21
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 34
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 55
					               },
					               (… and maybe more)
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			[InlineData(3, true)]
			public async Task ShouldSupportExactly(int times, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).DoesNotContain(1).Exactly(times);

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain an item equal to 1 exactly {times.ToTimesString()},
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
			[InlineData(2, true)]
			[InlineData(3, false)]
			public async Task ShouldSupportLessThan(int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).DoesNotContain(1).LessThan(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             does not contain an item equal to 1 less than 3 times,
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
			public async Task ShouldSupportMoreThan(int minimum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).DoesNotContain(1).MoreThan(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             does not contain an item equal to 1 more than once,
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
					               (… and maybe more)
					             ]
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsUnexpectedValue_ShouldFail(
				List<int> values, int unexpected)
			{
				values.Add(unexpected);
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(values.ToArray());

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not contain an item equal to {Formatter.Format(unexpected)},
					              but it contained it once

					              Collection:
					              {Formatter.Format(values)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableDoesNotContainsUnexpectedValue_ShouldSucceed(
				int[] values, int unexpected)
			{
				while (values.Contains(unexpected))
				{
					unexpected++;
				}

				IAsyncEnumerable<int> subject = ToAsyncEnumerable(values);

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				int expected = 42;
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotContain(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain an item equal to 42,
					             but it was <null>
					             """);
			}
		}

		public sealed class StringItemTests
		{
			[Theory]
			[InlineData(2, false)]
			[InlineData(3, true)]
			public async Task ShouldSupportAtLeast(int minimum, bool expectSuccess)
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["green", "blue", "blue", "yellow",]);

				async Task Act()
					=> await That(subject).DoesNotContain("blue").AtLeast(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain "blue" at least {minimum.ToTimesString()},
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
			public async Task ShouldSupportAtMost(int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["green", "blue", "blue", "yellow",]);

				async Task Act()
					=> await That(subject).DoesNotContain("blue").AtMost(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             does not contain "blue" at most twice,
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
			[InlineData(1, 2, false)]
			[InlineData(2, 3, false)]
			[InlineData(3, 4, true)]
			public async Task ShouldSupportBetween(int minimum, int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["green", "blue", "blue", "yellow",]);

				async Task Act()
					=> await That(subject).DoesNotContain("blue").Between(minimum).And(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain "blue" between {minimum} and {maximum} times,
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
			[InlineData(3, true)]
			public async Task ShouldSupportExactly(int times, bool expectSuccess)
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["green", "blue", "blue", "yellow",]);

				async Task Act()
					=> await That(subject).DoesNotContain("blue").Exactly(times);

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain "blue" exactly {times.ToTimesString()},
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
			[InlineData(2, true)]
			[InlineData(3, false)]
			public async Task ShouldSupportLessThan(int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["green", "blue", "blue", "yellow",]);

				async Task Act()
					=> await That(subject).DoesNotContain("blue").LessThan(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             does not contain "blue" less than 3 times,
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
			public async Task ShouldSupportMoreThan(int minimum, bool expectSuccess)
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["green", "blue", "blue", "yellow",]);

				async Task Act()
					=> await That(subject).DoesNotContain("blue").MoreThan(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             does not contain "blue" more than once,
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
			public async Task WhenStringDiffersInCase_ShouldSucceed()
			{
				string[] values = ["a", "b", "c",];
				string unexpected = "A";

				IAsyncEnumerable<string> subject = ToAsyncEnumerable(values);

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringDiffersInCase_WhileIgnoringCase_ShouldFail()
			{
				string[] values = ["a", "b", "c",];
				string unexpected = "A";

				IAsyncEnumerable<string> subject = ToAsyncEnumerable(values);

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "A" ignoring case,
					             but it contained it once

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string expected = "foo";
				IAsyncEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotContain(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "foo",
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
					=> await That(subject).DoesNotContain(x => x == 0)
						.And.DoesNotContain(x => x == 0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

				async Task Act()
					=> await That(subject).DoesNotContain(x => x == 5);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain an item matching x => x == 5,
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
					               (… and maybe more)
					             ]
					             """);
			}

			[Theory]
			[InlineData(2, false)]
			[InlineData(3, true)]
			public async Task ShouldSupportAtLeast(int minimum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).DoesNotContain(x => x == 1).AtLeast(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain an item matching x => x == 1 at least {minimum.ToTimesString()},
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
					                (… and maybe more)
					              ]
					              """);
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			public async Task ShouldSupportAtMost(int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).DoesNotContain(x => x == 1).AtMost(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             does not contain an item matching x => x == 1 at most twice,
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
			[InlineData(1, 2, false)]
			[InlineData(2, 3, false)]
			[InlineData(3, 4, true)]
			public async Task ShouldSupportBetween(int minimum, int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).DoesNotContain(x => x == 1).Between(minimum).And(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain an item matching x => x == 1 between {minimum} and {maximum} times,
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
			[InlineData(1, true)]
			[InlineData(2, false)]
			[InlineData(3, true)]
			public async Task ShouldSupportExactly(int times, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).DoesNotContain(x => x == 1).Exactly(times);

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain an item matching x => x == 1 exactly {times.ToTimesString()},
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
			[InlineData(2, true)]
			[InlineData(3, false)]
			public async Task ShouldSupportLessThan(int maximum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).DoesNotContain(x => x == 1).LessThan(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             does not contain an item matching x => x == 1 less than 3 times,
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
			public async Task ShouldSupportMoreThan(int minimum, bool expectSuccess)
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).DoesNotContain(x => x == 1).MoreThan(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             does not contain an item matching x => x == 1 more than once,
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
					               (… and maybe more)
					             ]
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsUnexpectedValue_ShouldFail(
				List<int> values, int unexpected)
			{
				values.Add(unexpected);
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(values.ToArray());

				async Task Act()
					=> await That(subject).DoesNotContain(x => x == unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not contain an item matching x => x == unexpected,
					              but it contained it once

					              Collection:
					              {Formatter.Format(values)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableDoesNotContainsUnexpectedValue_ShouldSucceed(
				int[] values, int unexpected)
			{
				while (values.Contains(unexpected))
				{
					unexpected++;
				}

				IAsyncEnumerable<int> subject = ToAsyncEnumerable(values);

				async Task Act()
					=> await That(subject).DoesNotContain(x => x == unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotContain(_ => true);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain an item matching _ => true,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenItemIsMissing_ShouldFailWithThePositiveExpectation()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable([2, 3,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotContain(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains an item equal to 1 at least once,
					             but it did not contain it
					             *
					             """).AsWildcard();
			}
		}
	}
}
#endif
