using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreReadOnly
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllTypesAreReadOnly_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicReadOnlyStruct),
				};

				async Task Act()
				{
					await That(subject).AreReadOnly();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeTypesAreNotReadOnly_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicReadOnlyStruct), typeof(PublicStruct),
				};

				async Task Act()
				{
					await That(subject).AreReadOnly();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are read-only,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               PublicStruct
					             ]

					             Collection:
					             [
					               PublicReadOnlyStruct,
					               PublicStruct
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenAsyncEnumerableAllTypesAreReadOnly_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(PublicReadOnlyStruct),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).AreReadOnly();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAsyncEnumerableSomeTypesAreNotReadOnly_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(PublicReadOnlyStruct), typeof(PublicStruct),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).AreReadOnly();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are read-only,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               PublicStruct
					             ]

					             Collection:
					             [
					               PublicReadOnlyStruct,
					               PublicStruct
					             ]
					             """);
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllTypesAreReadOnly_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicReadOnlyStruct),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreReadOnly());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all are read-only,
					             but all 1 were

					             Collection:
					             [
					               PublicReadOnlyStruct
					             ]
					             """);
			}

			[Test]
			public async Task WhenSomeTypesAreNotReadOnly_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicReadOnlyStruct), typeof(PublicStruct),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreReadOnly());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
