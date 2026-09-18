#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class All
	{
		public sealed class ComplyWith
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IAsyncEnumerable<int> subject = GetCancellingAsyncEnumerable(5, cts, token);

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsLessThan(6)).WithCancellation(token);

					await That(Act).Throws<InconclusiveException>()
						.WithMessage("""
						             Expected that subject
						             is less than 6 for all items,
						             but it could not be verified, because it was already cancelled

						             Collection:
						             [0, 1, 2, 3, 4, (… and maybe others)]
						             """);
				}

				[Fact]
				public async Task ConsidersCancellationToken_WhenNested_ShouldNameTheMember()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					Container subject = new(GetCancellingAsyncEnumerable(5, cts, token));

					async Task Act()
						=> await That(subject).Whose(c => c.Items, items => items.All().ComplyWith(x => x.IsLessThan(6)))
							.WithCancellation(token);

					await That(Act).Throws<InconclusiveException>()
						.WithMessage("""
						             Expected that subject
						             whose .Items is less than 6 for all items,
						             but .Items could not be verified, because it was already cancelled
						             *
						             """).AsWildcard();
				}

				[Fact]
				public async Task DoesNotEnumerateTwice()
				{
					ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsGreaterThan(-1))
							.And.All().ComplyWith(x => x.IsGreaterThan(-1));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task DoesNotMaterializeEnumerable()
				{
					IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsEqualTo(1));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 for all items,
						             but not all were

						             Not matching items:
						             [2, 3, 5, 8, 13, 21, 34, 55, 89, (… and maybe others)]

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

				[Fact]
				public async Task WhenEnumerableContainsDifferentValues_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 1, 1, 2, 2, 3);

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsEqualTo(1));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 for all items,
						             but only 4 of 7 were

						             Not matching items:
						             [2, 2, 3]

						             Collection:
						             [1, 1, 1, 1, 2, 2, 3]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableIsEmpty_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable((int[])[]);

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsEqualTo(0));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableOnlyContainsEqualValues_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 1, 1, 1, 1, 1);

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsEqualTo(1));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsEqualTo(0));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 0 for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class StringTests
			{
				[Fact]
				public async Task WhenAllItemsMatchExpectation_ShouldSucceed()
				{
					IAsyncEnumerable<string?> subject = ToAsyncEnumerable<string?>("apple", "ant", "avocado");

					async Task Act()
						=> await That(subject).All().ComplyWith(it => it.StartsWith("a"));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenExactlyOneItemMatchesExpectation_ShouldSucceed()
				{
					IAsyncEnumerable<string?> subject = ToAsyncEnumerable<string?>("apple", "banana", "cherry");

					async Task Act()
						=> await That(subject).Exactly(1).ComplyWith(it => it.StartsWith("b"));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNotAllItemsMatch_ShouldFail()
				{
					IAsyncEnumerable<string?> subject = ToAsyncEnumerable<string?>("apple", "banana", "avocado");

					async Task Act()
						=> await That(subject).All().ComplyWith(it => it.StartsWith("a"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             starts with "a" for all items,
						             but only 2 of 3 were

						             Not matching items:
						             [
						               "banana"
						             ]

						             Collection:
						             [
						               "apple",
						               "banana",
						               "avocado"
						             ]
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<string?>? subject = null;

					async Task Act()
						=> await That(subject).All().ComplyWith(it => it.StartsWith("a"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             starts with "a" for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Fact]
				public async Task WhenEnumerableOnlyContainsEqualValues_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 1, 1, 1, 1, 1);

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.DoesNotComplyWith(it => it.IsEqualTo(1)));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to 1 for all items,
						             but none of 7 were

						             Not matching items:
						             [1, 1, 1, 1, 1, 1, 1]

						             Collection:
						             [1, 1, 1, 1, 1, 1, 1]
						             """);
				}
			}
		}
	}
}
#endif
