using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed partial class AreOperators
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFilteringOnlyOperators_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithOperators)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.Where(m => m.IsReallyOperator());

				async Task Act()
				{
					await That(subject).AreOperators();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsContainNonOperators_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithOperators)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreOperators();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are operators,
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
			public async Task WhenFilteringOnlyOperators_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithOperators)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.Where(m => m.IsReallyOperator());

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreOperators());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all are operators,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenMethodsContainNonOperators_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithOperators)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreOperators());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenFilteringOnlyOperators_Negated_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(ClassWithOperators)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.Where(m => m.IsReallyOperator())
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreOperators());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all are operators,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyOperators_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(ClassWithOperators)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.Where(m => m.IsReallyOperator())
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreOperators();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsContainNonOperators_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(ClassWithOperators)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreOperators();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are operators,
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
