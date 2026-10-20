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
					             are all not ref structs,
					             but it contained ref structs

					             Not matching items:
					             [
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
					             are all not ref structs,
					             but it contained ref structs

					             Not matching items:
					             [
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
					             also contain a ref struct,
					             but it only contained not ref structs

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
