using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreIndexers
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenPropertiesAreAllIndexers_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithOnlyIndexers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreIndexers();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesContainNonIndexerProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithIndexers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreIndexers();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all indexers,
					             but it contained non-indexer properties

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertiesAreAllIndexers_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithOnlyIndexers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreIndexers());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are not all indexers,
					             but it only contained indexer properties

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenPropertiesContainNonIndexerProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithIndexers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreIndexers());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenPropertiesAreAllIndexers_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(TestClassWithOnlyIndexers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreIndexers();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesContainNonIndexerProperties_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(TestClassWithIndexers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreIndexers();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all indexers,
					             but it contained non-indexer properties

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
