using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed partial class DependOnlyOn
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_WhenEnoughItemsDependOnlyOnTheAssemblies_ShouldSucceed()
			{
				IEnumerable<Assembly?> subject = [typeof(In).Assembly, typeof(ThatAssemblies).Assembly,];

				async Task Act()
				{
					await That(subject).AtLeast(1).DependOnlyOn("aweXpect.Core");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Exactly_WhenTheMatchTypeIsSetAfterwards_ShouldUseTheMatchType()
			{
				IEnumerable<Assembly?> subject = [typeof(In).Assembly, typeof(ThatAssemblies).Assembly,];

				async Task Act()
				{
					await That(subject).Exactly(1).DependOnlyOn("AWEXPECT.CORE").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenAnItemDependsOnlyOnTheAssemblies_ShouldFail()
			{
				IEnumerable<Assembly?> subject = [typeof(In).Assembly, typeof(ThatAssemblies).Assembly,];

				async Task Act()
				{
					await That(subject).None().DependOnlyOn("aweXpect.Core");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has dependencies only on assemblies equal to "aweXpect.Core" for no items,
					             but 1 of 2 did

					             Matching items:
					             [
					               aweXpect.Reflection, Version=*
					             ]

					             Collection:
					             [
					               aweXpect.Reflection, Version=*,
					               aweXpect.Reflection.Tests, Version=* depends on [*]
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Assembly?> subject = [null, typeof(In).Assembly,];

				async Task Act()
				{
					await That(subject).AtLeast(1).DependOnlyOn("aweXpect.Core");
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.AtLeast(1).DependOnlyOn("aweXpect.Core"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has dependencies only on assemblies equal to "aweXpect.Core" for at least one item,
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
					             has dependencies only on assemblies equal to "aweXpect.Core" for no items,
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
			public async Task AtLeast_WhenTooFewItemsDependOnlyOnTheAssemblies_ShouldFail()
			{
				ImmutableArray<Assembly?> subject = [typeof(In).Assembly, typeof(ThatAssemblies).Assembly,];

				async Task Act()
				{
					await That(subject).AtLeast(2).DependOnlyOn("aweXpect.Core");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has dependencies only on assemblies equal to "aweXpect.Core" for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=* depends on [*]
					             ]

					             Collection:
					             [
					               aweXpect.Reflection, Version=*,
					               aweXpect.Reflection.Tests, Version=* depends on [*]
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task Exactly_WhenOneItemDependsOnlyOnTheAssemblies_ShouldSucceed()
			{
				ImmutableArray<Assembly?> subject = [typeof(In).Assembly, typeof(ThatAssemblies).Assembly,];

				async Task Act()
				{
					await That(subject).Exactly(1).DependOnlyOn("aweXpect.*").AsWildcard();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class AsyncEnumerableQuantifierTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsDependOnlyOnTheAssemblies_ShouldFail()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(ThatAssemblies).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();

				async Task Act()
				{
					await That(subject).AtLeast(2).DependOnlyOn("aweXpect.Core");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has dependencies only on assemblies equal to "aweXpect.Core" for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=* depends on [*]
					             ]

					             Collection:
					             [
					               aweXpect.Reflection, Version=*,
					               aweXpect.Reflection.Tests, Version=* depends on [*]
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task Exactly_WhenOneItemDependsOnlyOnTheAssemblies_ShouldSucceed()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(ThatAssemblies).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();

				List<string> expected = ["aweXpect.Core",];

				async Task Act()
				{
					await That(subject).Exactly(1).DependOnlyOn(expected);
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
