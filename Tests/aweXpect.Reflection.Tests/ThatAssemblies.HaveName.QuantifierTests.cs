using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed partial class HaveName
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsMatchTheSelector_ShouldFail()
			{
				IEnumerable<Assembly?> subject = [typeof(In).Assembly, typeof(ThatAssemblies).Assembly,];

				async Task Act()
				{
					await That(subject).AtLeast(2).HaveName(_ => "aweXpect.Reflection");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name matching _ => "aweXpect.Reflection" for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null with name "aweXpect.Reflection.Tests" instead of "aweXpect.Reflection"
					             ]

					             Collection:
					             [
					               aweXpect.Reflection, Version=*,
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null with name "aweXpect.Reflection.Tests" instead of "aweXpect.Reflection"
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task Exactly_WhenTheMatchTypeIsSetAfterwards_ShouldUseTheMatchType()
			{
				IEnumerable<Assembly?> subject = [typeof(In).Assembly, typeof(ThatAssemblies).Assembly,];

				async Task Act()
				{
					await That(subject).Exactly(1).HaveName(_ => "AWEXPECT.REFLECTION").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenAnItemMatchesTheSelector_ShouldFail()
			{
				IEnumerable<Assembly?> subject = [typeof(In).Assembly, typeof(ThatAssemblies).Assembly,];

				async Task Act()
				{
					await That(subject).None().HaveName(_ => "aweXpect.Reflection");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name matching _ => "aweXpect.Reflection" for no items,
					             but 1 of 2 did

					             Matching items:
					             [
					               aweXpect.Reflection, Version=*
					             ]

					             Collection:
					             [
					               aweXpect.Reflection, Version=*,
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null with name "aweXpect.Reflection.Tests" instead of "aweXpect.Reflection"
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Assembly?> subject = [null, typeof(In).Assembly,];

				async Task Act()
				{
					await That(subject).AtLeast(1).HaveName(assembly => assembly!.GetName().Name!);
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.AtLeast(1).HaveName(assembly => assembly!.GetName().Name!));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name matching assembly => assembly!.GetName().Name! for at least one item,
					             but for the item at index 0, it was <null>

					             Collection:
					             [
					               <null>,
					               aweXpect.Reflection, Version=*
					             ]
					             """).AsWildcard();
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name matching assembly => assembly!.GetName().Name! for no items,
					             but for the item at index 0, it was <null>

					             Collection:
					             [
					               <null>,
					               aweXpect.Reflection, Version=*
					             ]
					             """).AsWildcard().Because("a null item cannot be inspected, like a null subject");
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsMatchTheSelector_ShouldFail()
			{
				ImmutableArray<Assembly?> subject = [typeof(In).Assembly, typeof(ThatAssemblies).Assembly,];

				async Task Act()
				{
					await That(subject).AtLeast(2).HaveName(_ => "aweXpect.Reflection");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name matching _ => "aweXpect.Reflection" for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null with name "aweXpect.Reflection.Tests" instead of "aweXpect.Reflection"
					             ]

					             Collection:
					             [
					               aweXpect.Reflection, Version=*,
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null with name "aweXpect.Reflection.Tests" instead of "aweXpect.Reflection"
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task Exactly_WhenOneItemMatchesTheSelector_ShouldSucceed()
			{
				ImmutableArray<Assembly?> subject = [typeof(In).Assembly, typeof(ThatAssemblies).Assembly,];

				async Task Act()
				{
					await That(subject).Exactly(1).HaveName(_ => "Tests").AsSuffix();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsMatchTheSelector_ShouldFail()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(ThatAssemblies).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();

				async Task Act()
				{
					await That(subject).AtLeast(2).HaveName(_ => "aweXpect.Reflection");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name matching _ => "aweXpect.Reflection" for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null with name "aweXpect.Reflection.Tests" instead of "aweXpect.Reflection"
					             ]

					             Collection:
					             [
					               aweXpect.Reflection, Version=*,
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null with name "aweXpect.Reflection.Tests" instead of "aweXpect.Reflection"
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenAllItemsMatchTheSelector_ShouldSucceed()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(ThatAssemblies).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();

				async Task Act()
				{
					await That(subject).HaveName(assembly => assembly!.GetName().Name!.ToUpperInvariant()).IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAnItemDoesNotMatchTheSelector_ShouldFail()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(ThatAssemblies).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();

				async Task Act()
				{
					await That(subject).HaveName(_ => "aweXpect.Reflection");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have name matching _ => "aweXpect.Reflection",
					             but only 1 of 2 did

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null with name "aweXpect.Reflection.Tests" instead of "aweXpect.Reflection"
					             ]

					             Collection:
					             [
					               aweXpect.Reflection, Version=*,
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null with name "aweXpect.Reflection.Tests" instead of "aweXpect.Reflection"
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
