using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed partial class HaveVersion
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_WhenEnoughItemsHaveTheMajorVersion_ShouldSucceed()
			{
				Assembly assembly = typeof(ThatAssemblies).Assembly;
				IEnumerable<Assembly?> subject = [typeof(In).Assembly, assembly,];
				int major = assembly.GetName().Version!.Major;

				async Task Act()
				{
					await That(subject).AtLeast(1).HaveVersion().WithMajor.EqualTo(major);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_WhenTheComparisonIsOrderedAgainstNull_ShouldFailBothWays()
			{
				Assembly assembly = typeof(ThatAssemblies).Assembly;
				IEnumerable<Assembly?> subject = [assembly,];
				int major = assembly.GetName().Version!.Major;

				async Task Act()
				{
					await That(subject).AtLeast(1).HaveVersion().WithMajor.GreaterThan(null);
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.AtLeast(1).HaveVersion().WithMajor.GreaterThan(null));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has major version greater than <null> for at least one item,
					              but for the item at index 0, it had major version {major}

					              Collection:
					              [
					                aweXpect.Reflection.Tests, Version=*
					              ]
					              """).AsWildcard();
				await That(ActNegated).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has major version greater than <null> for no items,
					              but for the item at index 0, it had major version {major}

					              Collection:
					              [
					                aweXpect.Reflection.Tests, Version=*
					              ]
					              """).AsWildcard()
					.Because("an ordering against null answers neither the expectation nor its negation");
			}

			[Test]
			public async Task Exactly_WhenTooFewItemsHaveTheComponents_ShouldFail()
			{
				IEnumerable<Assembly?> subject = [typeof(In).Assembly, typeof(ThatAssemblies).Assembly,];

				async Task Act()
				{
					await That(subject).Exactly(2).HaveVersion().WithMajor.GreaterThanOrEqualTo(0)
						.WithMinor.LessThan(0);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has major version greater than or equal to 0 and minor version less than 0 for exactly 2 items,
					             but none of 2 did

					             Collection:
					             [
					               aweXpect.Reflection, Version=*,
					               aweXpect.Reflection.Tests, Version=*
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task None_WhenAnItemHasAMatchingVersion_ShouldFail()
			{
				IEnumerable<Assembly?> subject = [typeof(ThatAssemblies).Assembly,];

				async Task Act()
				{
					await That(subject).None().HaveVersion(version => version.Major >= 0);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has version matching version => version.Major >= 0 for no items,
					             but 1 of 1 did

					             Matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*
					             ]

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Assembly?> subject = [null, typeof(ThatAssemblies).Assembly,];

				async Task Act()
				{
					await That(subject).AtLeast(1).HaveVersion(version => version.Major >= 0);
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.AtLeast(1).HaveVersion(version => version.Major >= 0));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has version matching version => version.Major >= 0 for at least one item,
					             but for the item at index 0, it was <null>

					             Collection:
					             [
					               <null>,
					               aweXpect.Reflection.Tests, Version=*
					             ]
					             """).AsWildcard();
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has version matching version => version.Major >= 0 for no items,
					             but for the item at index 0, it was <null>

					             Collection:
					             [
					               <null>,
					               aweXpect.Reflection.Tests, Version=*
					             ]
					             """).AsWildcard().Because("a null item cannot be inspected, like a null subject");
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveAMatchingVersion_ShouldFail()
			{
				ImmutableArray<Assembly?> subject = [typeof(ThatAssemblies).Assembly,];

				async Task Act()
				{
					await That(subject).AtLeast(1).HaveVersion(version => version.Major < 0);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has version matching version => version.Major < 0 for at least one item,
					             but none of 1 did

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task Exactly_WhenOneItemHasTheMajorVersion_ShouldSucceed()
			{
				Assembly assembly = typeof(ThatAssemblies).Assembly;
				ImmutableArray<Assembly?> subject = [assembly,];

				async Task Act()
				{
					await That(subject).Exactly(1).HaveVersion().WithMajor.EqualTo(assembly.GetName().Version!.Major);
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveTheMajorVersion_ShouldFail()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(ThatAssemblies).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();

				async Task Act()
				{
					await That(subject).AtLeast(1).HaveVersion().WithMajor.LessThan(0);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has major version less than 0 for at least one item,
					             but none of 1 did

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task Exactly_WhenOneItemHasAMatchingVersion_ShouldSucceed()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(ThatAssemblies).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();

				async Task Act()
				{
					await That(subject).Exactly(1).HaveVersion(version => version.Major >= 0);
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
