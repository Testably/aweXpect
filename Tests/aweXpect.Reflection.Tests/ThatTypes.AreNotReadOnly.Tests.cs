using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotReadOnly
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEnumerableContainsNoReadOnlyTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicStruct), typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreNotReadOnly();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsReadOnlyType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicStruct), typeof(PublicReadOnlyStruct),
				};

				async Task Act()
				{
					await That(subject).AreNotReadOnly();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are read-only,
					             but 1 of 2 were

					             Matching items:
					             [
					               PublicReadOnlyStruct
					             ]

					             Collection:
					             [
					               PublicStruct,
					               PublicReadOnlyStruct
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenAsyncEnumerableContainsReadOnlyType_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(PublicStruct), typeof(PublicReadOnlyStruct),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).AreNotReadOnly();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are read-only,
					             but 1 of 2 were

					             Matching items:
					             [
					               PublicReadOnlyStruct
					             ]

					             Collection:
					             [
					               PublicStruct,
					               PublicReadOnlyStruct
					             ]
					             """);
			}

			[Test]
			public async Task WhenAsyncEnumerableContainsNoReadOnlyTypes_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(PublicStruct), typeof(PublicClass),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).AreNotReadOnly();
				}

				await That(Act).DoesNotThrow();
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEnumerableContainsNoReadOnlyTypes_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicStruct),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotReadOnly());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one is read-only,
					             but none of 1 were

					             Collection:
					             [
					               PublicStruct
					             ]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsReadOnlyType_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicStruct), typeof(PublicReadOnlyStruct),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotReadOnly());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
