using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotImmutable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllTypesAreMutable_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithMutableField), typeof(ClassWithSettableProperty),
				};

				async Task Act()
				{
					await That(subject).AreNotImmutable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomeTypesAreImmutable_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithMutableField), typeof(ImmutableClass),
				};

				async Task Act()
				{
					await That(subject).AreNotImmutable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not immutable,
					             but it contained immutable types [
					               ImmutableClass
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task WhenAsyncEnumerableAllTypesAreMutable_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithMutableField), typeof(ClassWithSettableProperty),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).AreNotImmutable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAsyncEnumerableSomeTypesAreImmutable_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithMutableField), typeof(ImmutableClass),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).AreNotImmutable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not immutable,
					             but it contained immutable types [
					               ImmutableClass
					             ]
					             """);
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllTypesAreMutable_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithMutableField),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotImmutable());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             also contain an immutable type,
					             but it only contained mutable types [
					               ClassWithMutableField
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSomeTypesAreImmutable_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithMutableField), typeof(ImmutableClass),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotImmutable());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
