using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;
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
			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all immutable,
					             but it contained mutable types [
					               ClassWithMutableField
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all immutable,
					             but it contained mutable types [
					               ClassWithMutableField
					             ]
					             """);
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             are not all immutable,
					             but it only contained immutable types [
					               ImmutableClass
					             ]
					             """);
			}

			[Fact]
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
