using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;
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
			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all read-only,
					             but it contained not read-only types [
					               PublicStruct
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all read-only,
					             but it contained not read-only types [
					               PublicStruct
					             ]
					             """);
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             are not all read-only,
					             but it only contained read-only types [
					               PublicReadOnlyStruct
					             ]
					             """);
			}

			[Fact]
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
