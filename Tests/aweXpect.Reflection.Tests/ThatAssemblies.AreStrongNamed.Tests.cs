using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed class AreStrongNamed
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllAssembliesAreStrongNamed_ShouldSucceed()
			{
				IEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(object).Assembly,
				};

				async Task Act()
				{
					await That(subject).AreStrongNamed();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssembliesContainNotStrongNamedAssemblies_ShouldFail()
			{
				IEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(PublicAbstractClass).Assembly,
				};

				async Task Act()
				{
					await That(subject).AreStrongNamed();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all strong named,
					             but it contained not strong named assemblies

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllAssembliesAreStrongNamed_ShouldFail()
			{
				IEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(object).Assembly,
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreStrongNamed());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are not all strong named,
					             but it only contained strong named assemblies

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenAssembliesContainNotStrongNamedAssemblies_ShouldSucceed()
			{
				IEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(PublicAbstractClass).Assembly,
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreStrongNamed());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenAllAssembliesAreStrongNamed_ShouldSucceed()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(object).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();

				async Task Act()
				{
					await That(subject).AreStrongNamed();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssembliesContainNotStrongNamedAssemblies_ShouldFail()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(PublicAbstractClass).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();

				async Task Act()
				{
					await That(subject).AreStrongNamed();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all strong named,
					             but it contained not strong named assemblies

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
