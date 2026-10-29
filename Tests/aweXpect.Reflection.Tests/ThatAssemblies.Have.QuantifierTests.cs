using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed partial class Have
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_WithPredicate_WhenTooFewItemsMatch_ShouldFail()
			{
				IEnumerable<Assembly?> subject = [TestAssembly, typeof(In).Assembly,];

				async Task Act()
				{
					await That(subject).AtLeast(2)
						.Have<AssemblyTitleAttribute>(title => title.Title == "aweXpect.Reflection.Tests");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has AssemblyTitleAttribute matching title => title.Title == "aweXpect.Reflection.Tests" for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               aweXpect.Reflection, Version=*, Culture=neutral, PublicKeyToken=c5072279b64ef837
					             ]

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null,
					               aweXpect.Reflection, Version=*, Culture=neutral, PublicKeyToken=c5072279b64ef837
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task Exactly_WhenTheNumberOfMatchingItemsMatches_ShouldSucceed()
			{
				IEnumerable<Assembly?> subject = [TestAssembly, typeof(In).Assembly,];

				async Task Act()
				{
					await That(subject).Exactly(2).Have<AssemblyTitleAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenAnItemHasAttribute_ShouldFail()
			{
				IEnumerable<Assembly?> subject = [TestAssembly,];

				Type attributeType = typeof(AssemblyTitleAttribute);

				async Task Act()
				{
					await That(subject).None().Have(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has AssemblyTitleAttribute for no items,
					             but 1 of 1 did

					             Matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WithType_WhenAllItemsHaveAttribute_ShouldSucceed()
			{
				IEnumerable<Assembly?> subject = [TestAssembly, typeof(In).Assembly,];

				Type attributeType = typeof(AssemblyTitleAttribute);

				async Task Act()
				{
					await That(subject).Have(attributeType);
				}

				await That(Act).DoesNotThrow();
			}

			private static Assembly TestAssembly => typeof(QuantifierTests).Assembly;
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveAttribute_ShouldFail()
			{
				ImmutableArray<Assembly?> subject = [typeof(StructCollectionTests).Assembly,];

				async Task Act()
				{
					await That(subject).AtLeast(1).Have(typeof(ObsoleteAttribute));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ObsoleteAttribute for at least one item,
					             but none of 1 did

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenTheItemsMatch_ShouldSucceed()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(AsyncEnumerableTests).Assembly, typeof(In).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();

				Type attributeType = typeof(AssemblyTitleAttribute);

				async Task Act()
				{
					await That(subject).Have(attributeType);
					await That(subject).AtLeast(2).Have<AssemblyTitleAttribute>();
					await That(subject).Exactly(1)
						.Have<AssemblyTitleAttribute>(title => title.Title == "aweXpect.Reflection.Tests");
					await That(subject).AtLeast(2).Have(attributeType);
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
