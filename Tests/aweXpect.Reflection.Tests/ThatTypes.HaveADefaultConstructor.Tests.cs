using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class HaveADefaultConstructor
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEnumerableContainsTypeWithoutADefaultConstructor_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(ClassWithoutDefaultConstructor),
				};

				async Task Act()
				{
					await That(subject).HaveADefaultConstructor();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all have a default constructor,
					             but it contained types without a default constructor [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAllTypesHaveADefaultConstructor_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicSealedClass),
				};

				async Task Act()
				{
					await That(subject).HaveADefaultConstructor();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEnumerableContainsTypeWithoutADefaultConstructor_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(ClassWithoutDefaultConstructor),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveADefaultConstructor());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllTypesHaveADefaultConstructor_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicSealedClass),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveADefaultConstructor());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             not all have a default constructor,
					             but it only contained types with a default constructor [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenEnumerableContainsTypeWithoutADefaultConstructor_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(ClassWithoutDefaultConstructor),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).HaveADefaultConstructor();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all have a default constructor,
					             but it contained types without a default constructor [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAllTypesHaveADefaultConstructor_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicSealedClass),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).HaveADefaultConstructor();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllTypesHaveADefaultConstructor_Negated_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicSealedClass),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveADefaultConstructor());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             not all have a default constructor,
					             but it only contained types with a default constructor [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
