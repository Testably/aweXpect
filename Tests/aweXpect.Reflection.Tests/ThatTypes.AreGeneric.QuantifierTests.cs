using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class AreGeneric
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_WithArgumentCount_ShouldOnlyCountMatchingItems()
			{
				IEnumerable<Type?> subject =
				[
					typeof(GenericClassWithOneArgument<int>), typeof(GenericClassWithTwoArguments<int, BaseClass>),
				];

				async Task Act()
				{
					await That(subject).AtLeast(2).AreGeneric().WithArgumentCount(2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is generic with 2 generic arguments for at least 2 items,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               ThatTypes.GenericClassWithOneArgument<int>
					             ]

					             Collection:
					             [
					               ThatTypes.GenericClassWithOneArgument<int>,
					               ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
					             ]
					             """);
			}

			[Test]
			public async Task None_WhenNoItemIsGeneric_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(BaseClass), typeof(UnrelatedClass),];

				async Task Act()
				{
					await That(subject).None().AreGeneric();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(GenericClassWithOneArgument<int>), null,];

				async Task Act()
				{
					await That(subject).AtMost(1).AreGeneric();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.AtMost(1).AreGeneric());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is generic for at most one item,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.GenericClassWithOneArgument<int>,
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is generic for more than one item,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.GenericClassWithOneArgument<int>,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task Exactly_WithArgument_WhenTheNumberOfMatchingItemsDiffers_ShouldFail()
			{
				ImmutableArray<Type?> subject =
				[
					typeof(GenericClassWithOneArgument<int>), typeof(GenericClassWithTwoArguments<int, BaseClass>),
				];

				async Task Act()
				{
					await That(subject).Exactly(2).AreGeneric().WithArgument<BaseClass>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is generic with argument of type ThatTypes.BaseClass for exactly 2 items,
					             but only 1 of 2 were

					             Collection:
					             [
					               ThatTypes.GenericClassWithOneArgument<int>,
					               ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
					             ]
					             """);
			}
		}

		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task AreNotGeneric_WhenAnItemIsGeneric_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(BaseClass), typeof(GenericClassWithOneArgument<int>),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).AreNotGeneric();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are generic,
					             but 1 of 2 were

					             Matching items:
					             [
					               ThatTypes.GenericClassWithOneArgument<int>
					             ]

					             Collection:
					             [
					               ThatTypes.BaseClass,
					               ThatTypes.GenericClassWithOneArgument<int>
					             ]
					             """);
			}

			[Test]
			public async Task AtLeast_WhenTooFewItemsAreGeneric_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(GenericClassWithOneArgument<int>), typeof(BaseClass),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).AtLeast(2).AreGeneric();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is generic for at least 2 items,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               ThatTypes.BaseClass
					             ]

					             Collection:
					             [
					               ThatTypes.GenericClassWithOneArgument<int>,
					               ThatTypes.BaseClass
					             ]
					             """);
			}
		}
#endif
	}
}
