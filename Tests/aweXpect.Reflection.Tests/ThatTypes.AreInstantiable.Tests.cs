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
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all instantiable,
					             but it contained non-instantiable types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
		}

		public sealed class NegatedTests
		{
			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all instantiable,
					             but it only contained instantiable types [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all instantiable,
					             but it contained non-instantiable types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all instantiable,
					             but it only contained instantiable types [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
