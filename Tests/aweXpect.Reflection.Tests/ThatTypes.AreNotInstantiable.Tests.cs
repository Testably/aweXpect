using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotInstantiable
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEnumerableContainsInstantiableType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicAbstractClass), typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreNotInstantiable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are instantiable,
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

			[Test]
			public async Task WhenNoTypeIsInstantiable_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicAbstractClass), typeof(IPublicInterface),
				};

				async Task Act()
				{
					await That(subject).AreNotInstantiable();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEnumerableContainsInstantiableType_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicAbstractClass), typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotInstantiable());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNoTypeIsInstantiable_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicAbstractClass), typeof(IPublicInterface),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotInstantiable());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one is instantiable,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenEnumerableContainsInstantiableType_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(PublicAbstractClass), typeof(PublicClass),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).AreNotInstantiable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are instantiable,
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

			[Test]
			public async Task WhenNoTypeIsInstantiable_Negated_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(PublicAbstractClass), typeof(IPublicInterface),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotInstantiable());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one is instantiable,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenNoTypeIsInstantiable_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(PublicAbstractClass), typeof(IPublicInterface),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).AreNotInstantiable();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
