#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using System.Linq;
using aweXpect.Core;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotContain
	{
		public sealed class ImmutableItemTests
		{
			[Theory]
			[InlineData(2, false)]
			[InlineData(3, true)]
			public async Task ShouldSupportAtLeast(int minimum, bool expectSuccess)
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).DoesNotContain(1).AtLeast(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain 1 at least {minimum.ToTimesString()},
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
			public async Task ShouldSupportAtMost(int maximum, bool expectSuccess)
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).DoesNotContain(1).AtMost(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             does not contain 1 at most twice,
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
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).DoesNotContain(1).Between(minimum).And(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain 1 between {minimum} and {maximum} times,
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
				ImmutableArray<MyClass> subject = [..Factory.GetFibonacciNumbers(20).Select(x => new MyClass(x)),];
				MyClass unexpected = new(5);

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Equivalent();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain MyClass {
					               StringValue = "",
					               Value = 5
					             } equivalent,
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
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).DoesNotContain(1).Exactly(times);

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain 1 exactly {times.ToTimesString()},
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
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).DoesNotContain(1).LessThan(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             does not contain 1 less than 3 times,
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
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).DoesNotContain(1).MoreThan(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             does not contain 1 more than once,
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

			[Fact]
			public async Task WhenEnumerableContainsUnexpectedValue_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				int unexpected = 3;

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not contain {Formatter.Format(unexpected)},
					              but it contained it at least once

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenEnumerableDoesNotContainsUnexpectedValue_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				int unexpected = 4;

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableStringItemTests
		{
			[Theory]
			[InlineData(2, false)]
			[InlineData(3, true)]
			public async Task ShouldSupportAtLeast(int minimum, bool expectSuccess)
			{
				ImmutableArray<string?> subject = ["green", "blue", "blue", "yellow",];

				async Task Act()
					=> await That(subject).DoesNotContain("blue").AtLeast(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain "blue" at least {minimum.ToTimesString()},
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
			public async Task ShouldSupportAtMost(int maximum, bool expectSuccess)
			{
				ImmutableArray<string?> subject = ["green", "blue", "blue", "yellow",];

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
				ImmutableArray<string?> subject = ["green", "blue", "blue", "yellow",];

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

			[Fact]
			public async Task ShouldSupportCasing()
			{
				ImmutableArray<string?> subject = ["FOO",];

				async Task Act()
					=> await That(subject).DoesNotContain("foo").IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "foo" ignoring case,
					             but it contained it at least once

					             Collection:
					             [
					               "FOO"
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			[InlineData(3, true)]
			public async Task ShouldSupportExactly(int times, bool expectSuccess)
			{
				ImmutableArray<string?> subject = ["green", "blue", "blue", "yellow",];

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
				ImmutableArray<string?> subject = ["green", "blue", "blue", "yellow",];

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
				ImmutableArray<string?> subject = ["green", "blue", "blue", "yellow",];

				async Task Act()
					=> await That(subject).DoesNotContain("blue").MoreThan(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             does not contain "blue" more than once,
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

			[Fact]
			public async Task WhenEnumerableContainsUnexpectedValue_ShouldFail()
			{
				ImmutableArray<string?> subject = ["FOO", "foo",];

				async Task Act()
					=> await That(subject).DoesNotContain("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "foo",
					             but it contained it at least once

					             Collection:
					             [
					               "FOO",
					               "foo"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableDoesNotContainUnexpectedValue_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["FOO",];

				async Task Act()
					=> await That(subject).DoesNotContain("foo");

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutablePredicateTests
		{
			[Theory]
			[InlineData(2, false)]
			[InlineData(3, true)]
			public async Task ShouldSupportAtLeast(int minimum, bool expectSuccess)
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).DoesNotContain(x => x == 1).AtLeast(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain item matching x => x == 1 at least {minimum.ToTimesString()},
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
			public async Task ShouldSupportAtMost(int maximum, bool expectSuccess)
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).DoesNotContain(x => x == 1).AtMost(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             does not contain item matching x => x == 1 at most twice,
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
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).DoesNotContain(x => x == 1).Between(minimum).And(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain item matching x => x == 1 between {minimum} and {maximum} times,
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
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).DoesNotContain(x => x == 1).Exactly(times);

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain item matching x => x == 1 exactly {times.ToTimesString()},
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
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).DoesNotContain(x => x == 1).LessThan(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             does not contain item matching x => x == 1 less than 3 times,
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
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).DoesNotContain(x => x == 1).MoreThan(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             does not contain item matching x => x == 1 more than once,
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

			[Fact]
			public async Task WhenEnumerableContainsUnexpectedValue_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				int unexpected = 3;

				async Task Act()
					=> await That(subject).DoesNotContain(x => x == unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not contain item matching x => x == unexpected,
					              but it contained it at least once

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenEnumerableDoesNotContainsUnexpectedValue_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				int unexpected = 4;

				async Task Act()
					=> await That(subject).DoesNotContain(x => x == unexpected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
