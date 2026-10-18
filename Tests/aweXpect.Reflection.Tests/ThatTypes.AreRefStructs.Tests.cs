using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreRefStructs
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllTypesAreRefStructs_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicRefStruct),
				};

				async Task Act()
				{
					await That(subject).AreRefStructs();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeTypesAreNotRefStructs_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicRefStruct), typeof(PublicStruct),
				};

				async Task Act()
				{
					await That(subject).AreRefStructs();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all ref structs,
					             but it contained other types

					             Not matching items:
					             [
					               PublicStruct
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenAsyncEnumerableAllTypesAreRefStructs_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(PublicRefStruct),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).AreRefStructs();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAsyncEnumerableSomeTypesAreNotRefStructs_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(PublicRefStruct), typeof(PublicStruct),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).AreRefStructs();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all ref structs,
					             but it contained other types

					             Not matching items:
					             [
					               PublicStruct
					             ]
					             """);
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllTypesAreRefStructs_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicRefStruct),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreRefStructs());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are not all ref structs,
					             but it only contained ref structs

					             Collection:
					             [
					               PublicRefStruct
					             ]
					             """);
			}

			[Test]
			public async Task WhenSomeTypesAreNotRefStructs_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicRefStruct), typeof(PublicStruct),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreRefStructs());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
