using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class DoNotHaveADefaultConstructor
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEnumerableContainsTypeWithADefaultConstructor_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithoutDefaultConstructor), typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).DoNotHaveADefaultConstructor();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all do not have a default constructor,
					             but it contained types with a default constructor [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNoTypeHasADefaultConstructor_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithoutDefaultConstructor), typeof(IPublicInterface),
				};

				async Task Act()
				{
					await That(subject).DoNotHaveADefaultConstructor();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEnumerableContainsTypeWithADefaultConstructor_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithoutDefaultConstructor), typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHaveADefaultConstructor());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoTypeHasADefaultConstructor_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithoutDefaultConstructor), typeof(IPublicInterface),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHaveADefaultConstructor());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a type with a default constructor,
					             but it only contained types without a default constructor [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenEnumerableContainsTypeWithADefaultConstructor_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithoutDefaultConstructor), typeof(PublicClass),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoNotHaveADefaultConstructor();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all do not have a default constructor,
					             but it contained types with a default constructor [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNoTypeHasADefaultConstructor_Negated_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithoutDefaultConstructor), typeof(IPublicInterface),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHaveADefaultConstructor());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a type with a default constructor,
					             but it only contained types without a default constructor [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNoTypeHasADefaultConstructor_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithoutDefaultConstructor), typeof(IPublicInterface),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoNotHaveADefaultConstructor();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
