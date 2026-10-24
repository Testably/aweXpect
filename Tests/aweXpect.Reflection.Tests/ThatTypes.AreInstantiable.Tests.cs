using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreInstantiable
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllTypesAreInstantiable_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicStruct),
				};

				async Task Act()
				{
					await That(subject).AreInstantiable();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsNonInstantiableType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicAbstractClass),
				};

				async Task Act()
				{
					await That(subject).AreInstantiable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are instantiable,
					             but only * of * were

					             Not matching items:
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
			public async Task WhenAllTypesAreInstantiable_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicStruct),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreInstantiable());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all are instantiable,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenEnumerableContainsNonInstantiableType_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicAbstractClass),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreInstantiable());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenAllTypesAreInstantiable_Negated_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicStruct),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreInstantiable());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all are instantiable,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenAllTypesAreInstantiable_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicStruct),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).AreInstantiable();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsNonInstantiableType_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicAbstractClass),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).AreInstantiable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are instantiable,
					             but only * of * were

					             Not matching items:
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
