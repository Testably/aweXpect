using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed partial class HaveName
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsMatchTheSelector_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1)),
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod2)),
				];

				async Task Act()
				{
					await That(subject).AtLeast(2).HaveName(_ => "PublicMethod1");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name matching _ => "PublicMethod1" for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod2() with name "PublicMethod2" instead of "PublicMethod1"
					             ]

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1(),
					               int ThatMethods.ClassWithMethods.PublicMethod2() with name "PublicMethod2" instead of "PublicMethod1"
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WhenTheMatchTypeIsSetAfterwards_ShouldUseTheMatchType()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1)),
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod2)),
				];

				async Task Act()
				{
					await That(subject).Exactly(1).HaveName(_ => "PUBLICMETHOD1").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenAnItemMatchesTheSelector_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1)),
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod2)),
				];

				async Task Act()
				{
					await That(subject).None().HaveName(_ => "PublicMethod1");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name matching _ => "PublicMethod1" for no items,
					             but 1 of 2 did

					             Matching items:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1()
					             ]

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1(),
					               int ThatMethods.ClassWithMethods.PublicMethod2() with name "PublicMethod2" instead of "PublicMethod1"
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<MethodInfo?> subject =
				[
					null,
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1)),
				];

				async Task Act()
				{
					await That(subject).AtLeast(1).HaveName(method => method.Name);
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.AtLeast(1).HaveName(method => method.Name));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name matching method => method.Name for at least one item,
					             but for the item at index 0, it was <null>

					             Collection:
					             [
					               <null>,
					               int ThatMethods.ClassWithMethods.PublicMethod1()
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name matching method => method.Name for no items,
					             but for the item at index 0, it was <null>

					             Collection:
					             [
					               <null>,
					               int ThatMethods.ClassWithMethods.PublicMethod1()
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsMatchTheSelector_ShouldFail()
			{
				ImmutableArray<MethodInfo?> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1)),
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod2)),
				];

				async Task Act()
				{
					await That(subject).AtLeast(2).HaveName(_ => "PublicMethod1");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name matching _ => "PublicMethod1" for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod2() with name "PublicMethod2" instead of "PublicMethod1"
					             ]

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1(),
					               int ThatMethods.ClassWithMethods.PublicMethod2() with name "PublicMethod2" instead of "PublicMethod1"
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WhenOneItemMatchesTheSelector_ShouldSucceed()
			{
				ImmutableArray<MethodInfo?> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1)),
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod2)),
				];

				async Task Act()
				{
					await That(subject).Exactly(1).HaveName(_ => "Method1").AsSuffix();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsMatchTheSelector_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1)),
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod2)),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).AtLeast(2).HaveName(_ => "PublicMethod1");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name matching _ => "PublicMethod1" for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod2() with name "PublicMethod2" instead of "PublicMethod1"
					             ]

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1(),
					               int ThatMethods.ClassWithMethods.PublicMethod2() with name "PublicMethod2" instead of "PublicMethod1"
					             ]
					             """);
			}

			[Test]
			public async Task WhenAllItemsMatchTheSelector_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1)),
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod2)),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).HaveName(method => method.Name.ToUpperInvariant()).IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAnItemDoesNotMatchTheSelector_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1)),
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod2)),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).HaveName(_ => "PublicMethod1");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have name matching _ => "PublicMethod1",
					             but only 1 of 2 did

					             Not matching items:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod2() with name "PublicMethod2" instead of "PublicMethod1"
					             ]

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1(),
					               int ThatMethods.ClassWithMethods.PublicMethod2() with name "PublicMethod2" instead of "PublicMethod1"
					             ]
					             """);
			}
		}
#endif
	}
}
