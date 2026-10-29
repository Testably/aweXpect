using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed partial class Return
	{
		private static MethodInfo? ReturningMethod(string name)
			=> typeof(ReturningMethods).GetMethod(name);

		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_OrReturn_ShouldCountItemsReturningAnyOfTheTypes()
			{
				IEnumerable<MethodInfo?> subject =
				[
					ReturningMethod(nameof(ReturningMethods.ReturnsInt)),
					ReturningMethod(nameof(ReturningMethods.ReturnsString)),
				];

				async Task Act()
				{
					await That(subject).AtLeast(2).Return<int>().OrReturn<string>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_WhenTooFewItemsReturnTheType_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					ReturningMethod(nameof(ReturningMethods.ReturnsInt)),
					ReturningMethod(nameof(ReturningMethods.ReturnsString)),
				];

				Type returnType = typeof(int);

				async Task Act()
				{
					await That(subject).AtLeast(2).Return(returnType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             returns int for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               string ThatMethods.Return.ReturningMethods.ReturnsString()
					             ]

					             Collection:
					             [
					               int ThatMethods.Return.ReturningMethods.ReturnsInt(),
					               string ThatMethods.Return.ReturningMethods.ReturnsString()
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_ReturnExactly_ShouldNotCountSubtypes()
			{
				IEnumerable<MethodInfo?> subject =
				[
					ReturningMethod(nameof(ReturningMethods.ReturnsObject)),
					ReturningMethod(nameof(ReturningMethods.ReturnsString)),
				];

				async Task Act()
				{
					await That(subject).Exactly(2).ReturnExactly<object>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             returns exactly object for exactly 2 items,
					             but only 1 of 2 did

					             Collection:
					             [
					               object ThatMethods.Return.ReturningMethods.ReturnsObject(),
					               string ThatMethods.Return.ReturningMethods.ReturnsString()
					             ]
					             """);
			}

			[Test]
			public async Task None_ReturnVoid_WhenAnItemReturnsVoid_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					ReturningMethod(nameof(ReturningMethods.ReturnsInt)),
					ReturningMethod(nameof(ReturningMethods.ReturnsVoid)),
				];

				async Task Act()
				{
					await That(subject).None().ReturnVoid();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             returns void for no items,
					             but 1 of 2 did

					             Matching items:
					             [
					               void ThatMethods.Return.ReturningMethods.ReturnsVoid()
					             ]

					             Collection:
					             [
					               int ThatMethods.Return.ReturningMethods.ReturnsInt(),
					               void ThatMethods.Return.ReturningMethods.ReturnsVoid()
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<MethodInfo?> subject = [ReturningMethod(nameof(ReturningMethods.ReturnsInt)), null,];

				async Task Act()
				{
					await That(subject).AtMost(1).Return<int>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.AtMost(1).Return<int>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             returns int for at most one item,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               int ThatMethods.Return.ReturningMethods.ReturnsInt(),
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             returns int for more than one item,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               int ThatMethods.Return.ReturningMethods.ReturnsInt(),
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_OrReturnExactly_WhenTooFewItemsMatch_ShouldFail()
			{
				ImmutableArray<MethodInfo?> subject =
				[
					ReturningMethod(nameof(ReturningMethods.ReturnsInt)),
					ReturningMethod(nameof(ReturningMethods.ReturnsString)),
				];

				Type returnType = typeof(long);

				async Task Act()
				{
					await That(subject).AtLeast(2).Return(returnType).OrReturnExactly<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             returns long or exactly int for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               string ThatMethods.Return.ReturningMethods.ReturnsString()
					             ]

					             Collection:
					             [
					               int ThatMethods.Return.ReturningMethods.ReturnsInt(),
					               string ThatMethods.Return.ReturningMethods.ReturnsString()
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WhenTheNumberOfMatchingItemsMatches_ShouldSucceed()
			{
				ImmutableArray<MethodInfo?> subject =
				[
					ReturningMethod(nameof(ReturningMethods.ReturnsObject)),
					ReturningMethod(nameof(ReturningMethods.ReturnsString)),
					ReturningMethod(nameof(ReturningMethods.ReturnsVoid)),
				];

				Type objectType = typeof(object);
				Type stringType = typeof(string);

				async Task Act()
				{
					await That(subject).Exactly(1).Return(stringType);
					await That(subject).Exactly(1).ReturnExactly(objectType);
					await That(subject).Exactly(1).ReturnVoid();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task AtLeast_ReturnExactly_WhenTooFewItemsMatch_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					ReturningMethod(nameof(ReturningMethods.ReturnsObject)),
					ReturningMethod(nameof(ReturningMethods.ReturnsString)),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).AtLeast(2).ReturnExactly<object>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             returns exactly object for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               string ThatMethods.Return.ReturningMethods.ReturnsString()
					             ]

					             Collection:
					             [
					               object ThatMethods.Return.ReturningMethods.ReturnsObject(),
					               string ThatMethods.Return.ReturningMethods.ReturnsString()
					             ]
					             """);
			}

			[Test]
			public async Task DoNotReturn_WhenAnItemReturnsTheType_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					ReturningMethod(nameof(ReturningMethods.ReturnsInt)),
					ReturningMethod(nameof(ReturningMethods.ReturnsString)),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).DoNotReturn<string>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none return string,
					             but 1 of 2 did

					             Matching items:
					             [
					               string ThatMethods.Return.ReturningMethods.ReturnsString()
					             ]

					             Collection:
					             [
					               int ThatMethods.Return.ReturningMethods.ReturnsInt(),
					               string ThatMethods.Return.ReturningMethods.ReturnsString()
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WhenTheNumberOfMatchingItemsMatches_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					ReturningMethod(nameof(ReturningMethods.ReturnsObject)),
					ReturningMethod(nameof(ReturningMethods.ReturnsString)),
					ReturningMethod(nameof(ReturningMethods.ReturnsVoid)),
				}.ToTestAsyncEnumerable();

				Type objectType = typeof(object);
				Type stringType = typeof(string);

				async Task Act()
				{
					await That(subject).Exactly(1).Return<string>();
					await That(subject).Exactly(1).Return(stringType);
					await That(subject).Exactly(1).ReturnExactly(objectType);
					await That(subject).Exactly(1).ReturnVoid();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif

#pragma warning disable CA1822
		// ReSharper disable UnusedMember.Local
		private sealed class ReturningMethods
		{
			public int ReturnsInt() => 0;

			public object ReturnsObject() => new();

			public string ReturnsString() => "";

			public void ReturnsVoid()
			{
			}
		}
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
