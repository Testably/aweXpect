using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;
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
			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not ref structs,
					             but it contained ref structs [
					               PublicRefStruct
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not ref structs,
					             but it contained ref structs [
					               PublicRefStruct
					             ]
					             """);
			}

			[Fact]
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
			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             also contain a ref struct,
					             but it only contained not ref structs [
					               PublicStruct
					             ]
					             """);
			}

			[Fact]
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
