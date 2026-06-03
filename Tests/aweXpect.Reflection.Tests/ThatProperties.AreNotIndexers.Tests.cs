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
			[Fact]
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

			[Fact]
			public async Task WhenPropertiesContainIndexers_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithOnlyIndexers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotIndexers();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not indexers,
					             but it contained indexer properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertiesContainNoIndexers_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotIndexers());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain an indexer property,
					             but it only contained non-indexer properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
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

			[Fact]
			public async Task WhenPropertiesContainIndexers_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(TestClassWithOnlyIndexers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreNotIndexers();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not indexers,
					             but it contained indexer properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
