using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotRefStructs
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEnumerableContainsNoRefStructTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicStruct), typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreNotRefStructs();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsRefStructType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicStruct), typeof(PublicRefStruct),
				};

				async Task Act()
				{
					await That(subject).AreNotRefStructs();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are ref structs,
					             but 1 of 2 were

					             Matching items:
					             [
					               PublicRefStruct
					             ]

					             Collection:
					             [
					               PublicStruct,
					               PublicRefStruct
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenAsyncEnumerableContainsRefStructType_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(PublicStruct), typeof(PublicRefStruct),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).AreNotRefStructs();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are ref structs,
					             but 1 of 2 were

					             Matching items:
					             [
					               PublicRefStruct
					             ]

					             Collection:
					             [
					               PublicStruct,
					               PublicRefStruct
					             ]
					             """);
			}

			[Test]
			public async Task WhenAsyncEnumerableContainsNoRefStructTypes_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(PublicStruct), typeof(PublicClass),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).AreNotRefStructs();
				}

				await That(Act).DoesNotThrow();
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEnumerableContainsNoRefStructTypes_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicStruct),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotRefStructs());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one is a ref struct,
					             but none of 1 were

					             Collection:
					             [
					               PublicStruct
					             ]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsRefStructType_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicStruct), typeof(PublicRefStruct),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotRefStructs());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
