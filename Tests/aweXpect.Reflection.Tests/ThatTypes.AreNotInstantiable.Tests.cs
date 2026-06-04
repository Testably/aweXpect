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
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not instantiable,
					             but it contained instantiable types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain an instantiable type,
					             but it only contained non-instantiable types [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not instantiable,
					             but it contained instantiable types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain an instantiable type,
					             but it only contained non-instantiable types [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
