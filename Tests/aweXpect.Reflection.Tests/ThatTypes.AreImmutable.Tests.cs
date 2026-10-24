using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreImmutable
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllTypesAreImmutable_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ImmutableClass), typeof(ImmutableClassWithInitProperty),
				};

				async Task Act()
				{
					await That(subject).AreImmutable();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeTypesAreMutable_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ImmutableClass), typeof(ClassWithMutableField),
				};

				async Task Act()
				{
					await That(subject).AreImmutable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are immutable,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               ClassWithMutableField
					             ]

					             Collection:
					             [
					               ImmutableClass,
					               ClassWithMutableField
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenAsyncEnumerableAllTypesAreImmutable_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ImmutableClass), typeof(ImmutableClassWithInitProperty),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).AreImmutable();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAsyncEnumerableSomeTypesAreMutable_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ImmutableClass), typeof(ClassWithMutableField),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).AreImmutable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are immutable,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               ClassWithMutableField
					             ]

					             Collection:
					             [
					               ImmutableClass,
					               ClassWithMutableField
					             ]
					             """);
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllTypesAreImmutable_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ImmutableClass),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreImmutable());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all are immutable,
					             but all 1 were

					             Collection:
					             [
					               ImmutableClass
					             ]
					             """);
			}

			[Test]
			public async Task WhenSomeTypesAreMutable_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ImmutableClass), typeof(ClassWithMutableField),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreImmutable());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
