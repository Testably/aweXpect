using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;
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
			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all ref structs,
					             but it contained other types [
					               PublicStruct
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all ref structs,
					             but it contained other types [
					               PublicStruct
					             ]
					             """);
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             are not all ref structs,
					             but it only contained ref structs [
					               PublicRefStruct
					             ]
					             """);
			}

			[Fact]
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
