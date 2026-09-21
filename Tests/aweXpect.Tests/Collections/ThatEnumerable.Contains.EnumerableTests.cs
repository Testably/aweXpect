using System.Collections;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Contains
	{
		public sealed class EnumerableItemTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

				async Task Act()
					=> await That(subject).Contains(1)
						.And.Contains(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers();

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
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

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
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

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
					               (… and maybe more)
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, 2, true)]
			[InlineData(2, 3, true)]
			[InlineData(3, 4, false)]
			public async Task ShouldSupportBetween(int minimum, int maximum, bool expectSuccess)
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

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
					                (… and 10 more)
					              ]
					              """);
			}

			[Fact]
			public async Task ShouldSupportEquivalent()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(20).Select(x => new MyClass(x));
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
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

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
					                {(times == 1 ? "(… and maybe more)" : "(… and 10 more)")}
					              ]
					              """);
			}

			[Theory]
			[InlineData(2, false)]
			[InlineData(3, true)]
			public async Task ShouldSupportLessThan(int maximum, bool expectSuccess)
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

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
					               (… and maybe more)
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			[InlineData(3, false)]
			public async Task ShouldSupportMoreThan(int minimum, bool expectSuccess)
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

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
					                (… and 10 more)
					              ]
					              """);
			}

			[Fact]
			public async Task ShouldSupportNever()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

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
					               (… and maybe more)
					             ]
					             """);
			}

			[Fact]
			public async Task Using_WithAllDifferentComparer_ShouldFail()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

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
					               (… and 10 more)
					             ]
					             """);
			}

			[Fact]
			public async Task Using_WithAllEqualComparer_ShouldSucceed()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(1).AtLeast(4).Using(new AllEqualComparer());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsExpectedValue_ShouldSucceed()
			{
				IEnumerable subject = Enumerable.Range(1, 3);
				int expected = 3;

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableDoesNotContainsExpectedValue_ShouldFail()
			{
				IEnumerable subject = Enumerable.Range(1, 3);
				int expected = 4;


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
			public async Task WhenExpectedIsNullLiteral_ShouldMatchTheNullItem()
			{
				IEnumerable subject = new ArrayList { 1, "a", null, };

				async Task Act()
					=> await That(subject).Contains(null).Once();

				await That(Act).DoesNotThrow()
					.Because("a `null` literal is a value, not a predicate");
			}

			[Fact]
			public async Task WhenExpectedIsNullLiteral_WithoutANullItem_ShouldFail()
			{
				ArrayList subject = new() { 1, "a", };

				async Task Act()
					=> await That(subject).Contains(null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains <null> at least once,
					             but it did not contain it

					             Collection:
					             [1, "a"]
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
				IEnumerable subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).Contains(4).And.Contains(5);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains 4 at least once and contains 5 at least once,
					             but it did not contain it

					             Collection:
					             [1, 2, 3]
					             """);
			}
		}

		public sealed class EnumerablePredicateTests
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
				IEnumerable subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).Contains(x => 5.Equals(x));

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, true)]
			[InlineData(3, false)]
			public async Task ShouldSupportAtLeast(int minimum, bool expectSuccess)
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => 1.Equals(x)).AtLeast(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains item matching x => 1.Equals(x) at least {minimum} times,
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
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => 1.Equals(x)).AtMost(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains item matching x => 1.Equals(x) at most once,
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
			[InlineData(1, 2, true)]
			[InlineData(2, 3, true)]
			[InlineData(3, 4, false)]
			public async Task ShouldSupportBetween(int minimum, int maximum, bool expectSuccess)
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => 1.Equals(x)).Between(minimum).And(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains item matching x => 1.Equals(x) between {minimum} and {maximum} times,
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
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => 1.Equals(x)).Exactly(times);

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains item matching x => 1.Equals(x) exactly {(times == 1 ? "once" : $"{times} times")},
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
					                {(times == 1 ? "(… and maybe more)" : "(… and 10 more)")}
					              ]
					              """);
			}

			[Theory]
			[InlineData(2, false)]
			[InlineData(3, true)]
			public async Task ShouldSupportLessThan(int maximum, bool expectSuccess)
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => 1.Equals(x)).LessThan(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains item matching x => 1.Equals(x) less than twice,
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
			[InlineData(3, false)]
			public async Task ShouldSupportMoreThan(int minimum, bool expectSuccess)
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => 1.Equals(x)).MoreThan(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              contains item matching x => 1.Equals(x) more than {minimum.ToTimesString()},
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
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).Contains(x => 1.Equals(x)).Never();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain item matching x => 1.Equals(x),
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

			[Fact]
			public async Task WhenEnumerableContainsExpectedValue_ShouldSucceed()
			{
				IEnumerable subject = Enumerable.Range(1, 3);
				int expected = 3;

				async Task Act()
					=> await That(subject).Contains(x => expected.Equals(x));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableDoesNotContainsExpectedValue_ShouldFail()
			{
				IEnumerable subject = Enumerable.Range(1, 3);
				int expected = 4;

				async Task Act()
					=> await That(subject).Contains(x => expected.Equals(x));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              contains item matching x => expected.Equals(x) at least once,
					              but it did not contain it

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers();

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
