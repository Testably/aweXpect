using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed class AreNotStrongNamed
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllAssembliesAreNotStrongNamed_ShouldSucceed()
			{
				IEnumerable<Assembly?> subject = new[]
				{
					typeof(PublicAbstractClass).Assembly,
				};

				async Task Act()
				{
					await That(subject).AreNotStrongNamed();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssembliesContainStrongNamedAssemblies_ShouldFail()
			{
				IEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(PublicAbstractClass).Assembly,
				};

				async Task Act()
				{
					await That(subject).AreNotStrongNamed();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are strong named,
					             but * of * were

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllAssembliesAreNotStrongNamed_ShouldFail()
			{
				IEnumerable<Assembly?> subject = new[]
				{
					typeof(PublicAbstractClass).Assembly,
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotStrongNamed());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one is strong named,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenAssembliesContainStrongNamedAssemblies_ShouldSucceed()
			{
				IEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(PublicAbstractClass).Assembly,
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotStrongNamed());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenAllAssembliesAreNotStrongNamed_ShouldSucceed()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(PublicAbstractClass).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();

				async Task Act()
				{
					await That(subject).AreNotStrongNamed();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssembliesContainStrongNamedAssemblies_ShouldFail()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(PublicAbstractClass).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();

				async Task Act()
				{
					await That(subject).AreNotStrongNamed();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are strong named,
					             but * of * were

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
