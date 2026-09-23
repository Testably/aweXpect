using System.Collections;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotContain
	{
		public sealed class EnumerableItemTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

				async Task Act()
					=> await That(subject).DoesNotContain(0)
						.And.DoesNotContain(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers();

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
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

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
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

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
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

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
				IEnumerable subject = Factory.GetFibonacciNumbers(20).Select(x => new MyClass(x));
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
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

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
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

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
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

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

			[Fact]
			public async Task WhenEnumerableContainsUnexpectedValue_ShouldFail()
			{
				IEnumerable subject = Enumerable.Range(1, 3);
				int unexpected = 3;

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain an item equal to 3,
					             but it contained it at least once

					             Collection:
					             [
					               1,
					               2,
					               3
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableDoesNotContainsUnexpectedValue_ShouldSucceed()
			{
				IEnumerable subject = Enumerable.Range(1, 3);
				int unexpected = 4;

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				int expected = 42;
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotContain(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain an item equal to 42,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenUnexpectedIsNullLiteral_ShouldMatchTheNullItem()
			{
				ArrayList subject = new() { 1, "a", null, };

				async Task Act()
					=> await That(subject).DoesNotContain(null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain an item equal to <null>,
					             but it contained it at least once

					             Collection:
					             [1, "a", <null>]
					             """)
					.Because("a `null` literal is a value, not a predicate");
			}
		}

		public sealed class EnumerablePredicateTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceEnumerable subject = new();

				async Task Act()
					=> await That(subject).DoesNotContain(x => x == 0)
						.And.DoesNotContain(x => x == 0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).DoesNotContain(x => 5.Equals(x));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain an item matching x => 5.Equals(x),
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
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).DoesNotContain(x => 1.Equals(x)).AtLeast(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain an item matching x => 1.Equals(x) at least {minimum.ToTimesString()},
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
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).DoesNotContain(x => 1.Equals(x)).AtMost(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             does not contain an item matching x => 1.Equals(x) at most twice,
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
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).DoesNotContain(x => 1.Equals(x)).Between(minimum).And(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain an item matching x => 1.Equals(x) between {minimum} and {maximum} times,
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
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).DoesNotContain(x => 1.Equals(x)).Exactly(times);

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain an item matching x => 1.Equals(x) exactly {times.ToTimesString()},
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
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).DoesNotContain(x => 1.Equals(x)).LessThan(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             does not contain an item matching x => 1.Equals(x) less than 3 times,
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
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).DoesNotContain(x => 1.Equals(x)).MoreThan(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             does not contain an item matching x => 1.Equals(x) more than once,
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

			[Fact]
			public async Task WhenEnumerableContainsUnexpectedValue_ShouldFail()
			{
				IEnumerable subject = Enumerable.Range(1, 3);
				int unexpected = 3;

				async Task Act()
					=> await That(subject).DoesNotContain(x => unexpected.Equals(x));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain an item matching x => unexpected.Equals(x),
					             but it contained it at least once

					             Collection:
					             [
					               1,
					               2,
					               3
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableDoesNotContainsUnexpectedValue_ShouldSucceed()
			{
				IEnumerable subject = Enumerable.Range(1, 3);
				int unexpected = 4;

				async Task Act()
					=> await That(subject).DoesNotContain(x => unexpected.Equals(x));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string>? subject = null;

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
	}
}
