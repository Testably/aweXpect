using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreNotOperators
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFilteringOnlyNonOperators_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithOperators)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsReallyOperator());

				async Task Act()
				{
					await That(subject).AreNotOperators();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsContainOperators_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithOperators)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotOperators();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are operators,
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
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFilteringOnlyNonOperators_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithOperators)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsReallyOperator());

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotOperators());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one is an operator,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenMethodsContainOperators_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithOperators)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotOperators());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenFilteringOnlyNonOperators_Negated_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(ClassWithOperators)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsReallyOperator())
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotOperators());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one is an operator,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyNonOperators_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(ClassWithOperators)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsReallyOperator())
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreNotOperators();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsContainOperators_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(ClassWithOperators)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreNotOperators();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are operators,
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
		}
#endif
	}
}
