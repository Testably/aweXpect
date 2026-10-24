using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotIndexers
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenPropertiesContainIndexers_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithOnlyIndexers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotIndexers();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are indexers,
					             but * of * were

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenPropertiesContainNoIndexers_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotIndexers();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertiesContainIndexers_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithOnlyIndexers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotIndexers());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesContainNoIndexers_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotIndexers());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one is an indexer,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenPropertiesContainIndexers_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(TestClassWithOnlyIndexers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreNotIndexers();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are indexers,
					             but * of * were

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenPropertiesContainNoIndexers_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreNotIndexers();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
