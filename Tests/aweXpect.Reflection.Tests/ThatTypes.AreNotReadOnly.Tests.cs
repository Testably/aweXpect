using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;
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
			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not read-only,
					             but it contained read-only types [
					               PublicReadOnlyStruct
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not read-only,
					             but it contained read-only types [
					               PublicReadOnlyStruct
					             ]
					             """);
			}

			[Fact]
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
			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             also contain a read-only type,
					             but it only contained not read-only types [
					               PublicStruct
					             ]
					             """);
			}

			[Fact]
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
