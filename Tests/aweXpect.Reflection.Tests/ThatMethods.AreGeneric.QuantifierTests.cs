using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed partial class AreGeneric
	{
		private static MethodInfo? GenericTestMethod(string name)
			=> typeof(ClassWithMethods).GetMethod(name);

		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_WithArgumentCount_ShouldOnlyCountMatchingItems()
			{
				IEnumerable<MethodInfo?> subject =
				[
					GenericTestMethod(nameof(ClassWithMethods.GenericMethod1)),
					GenericTestMethod(nameof(ClassWithMethods.GenericMethod2)),
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
					               T ThatMethods.ClassWithMethods.GenericMethod1<T>(T value)
					             ]

					             Collection:
					             [
					               T ThatMethods.ClassWithMethods.GenericMethod1<T>(T value),
					               U ThatMethods.ClassWithMethods.GenericMethod2<T, U>(T first, U second)
					             ]
					             """);
			}

			[Test]
			public async Task None_WhenNoItemIsGeneric_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject =
				[
					GenericTestMethod(nameof(ClassWithMethods.NonGenericMethod1)),
					GenericTestMethod(nameof(ClassWithMethods.NonGenericMethod2)),
				];

				async Task Act()
				{
					await That(subject).None().AreGeneric();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<MethodInfo?> subject = [GenericTestMethod(nameof(ClassWithMethods.GenericMethod1)), null,];

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
					               T ThatMethods.ClassWithMethods.GenericMethod1<T>(T value),
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
					               T ThatMethods.ClassWithMethods.GenericMethod1<T>(T value),
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task Exactly_WhenTheNumberOfGenericItemsDiffers_ShouldFail()
			{
				ImmutableArray<MethodInfo?> subject =
				[
					GenericTestMethod(nameof(ClassWithMethods.GenericMethod1)),
					GenericTestMethod(nameof(ClassWithMethods.NonGenericMethod1)),
				];

				async Task Act()
				{
					await That(subject).Exactly(2).AreGeneric();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is generic for exactly 2 items,
					             but only 1 of 2 were

					             Collection:
					             [
					               T ThatMethods.ClassWithMethods.GenericMethod1<T>(T value),
					               int ThatMethods.ClassWithMethods.NonGenericMethod1()
					             ]
					             """);
			}
		}

		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task AreNotGeneric_WhenAnItemIsGeneric_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					GenericTestMethod(nameof(ClassWithMethods.GenericMethod1)),
					GenericTestMethod(nameof(ClassWithMethods.NonGenericMethod1)),
				}.ToTestAsyncEnumerable();

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
					               T ThatMethods.ClassWithMethods.GenericMethod1<T>(T value)
					             ]

					             Collection:
					             [
					               T ThatMethods.ClassWithMethods.GenericMethod1<T>(T value),
					               int ThatMethods.ClassWithMethods.NonGenericMethod1()
					             ]
					             """);
			}

			[Test]
			public async Task AtLeast_WhenEnoughItemsAreGeneric_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					GenericTestMethod(nameof(ClassWithMethods.GenericMethod1)),
					GenericTestMethod(nameof(ClassWithMethods.NonGenericMethod1)),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).AtLeast(1).AreGeneric();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
