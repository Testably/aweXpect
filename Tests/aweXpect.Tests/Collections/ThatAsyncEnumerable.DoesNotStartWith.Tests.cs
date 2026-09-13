#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed class DoesNotStartWith
	{
		public sealed class Tests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

				async Task Act()
					=> await That(subject).DoesNotStartWith(0)
						.And.DoesNotStartWith(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

				async Task Act()
					=> await That(subject).DoesNotStartWith(1, 1, 2, 3, 4);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldSupportCaseInsensitiveComparison()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["FOO", "BAR",]);

				async Task Act()
					=> await That(subject).DoesNotStartWith("foo").IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with ["foo"] ignoring case,
					             but it did start with [
					               "FOO"
					             ]
					             """);
			}

			[Fact]
			public async Task ShouldSupportEquivalent()
			{
				IAsyncEnumerable<MyClass> subject = Factory.GetAsyncFibonacciNumbers(x => new MyClass(x), 20);
				IEnumerable<MyClass> unexpected = [new(1), new(1), new(2),];

				async Task Act()
					=> await That(subject).DoesNotStartWith(unexpected).Equivalent();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with unexpected equivalent,
					             but it did start with [
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
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenCollectionsAreIdentical_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).DoesNotStartWith(1, 2, 3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with [1, 2, 3],
					             but it did start with [
					               1,
					               2,
					               3
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableHasDifferentStartingElements_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);
				IEnumerable<int> unexpected = [1, 3,];

				async Task Act()
					=> await That(subject).DoesNotStartWith(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotStartWith(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with [1],
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectStartsWithUnexpectedValues_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);
				IEnumerable<string> unexpected = ["foo", "bar",];

				async Task Act()
					=> await That(subject).DoesNotStartWith(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with unexpected,
					             but it did start with [
					               "foo",
					               "bar"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenUnexpectedContainsAdditionalElements_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).DoesNotStartWith(1, 2, 3, 4);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUnexpectedIsEmpty_ShouldThrowArgumentException()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2);

				async Task Act()
					=> await That(subject).DoesNotStartWith();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix();
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1);

				async Task Act()
					=> await That(subject).DoesNotStartWith(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected cannot be null.").AsPrefix();
			}
		}
	}
}
#endif
