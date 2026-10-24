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
			[Test]
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

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none have a default constructor,
					             but * of * did

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
			[Test]
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

			[Test]
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

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one has a default constructor,
					             but none of * did

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

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none have a default constructor,
					             but * of * did

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

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one has a default constructor,
					             but none of * did

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
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
