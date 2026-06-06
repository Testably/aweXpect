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
			[Fact]
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

			[Fact]
			public async Task WhenMethodsContainOperators_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithOperators)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotOperators();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not operators,
					             but it contained operators [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonOperators_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithOperators)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsReallyOperator());

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotOperators());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain an operator,
					             but it only contained non-operators [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain an operator,
					             but it only contained non-operators [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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

			[Fact]
			public async Task WhenMethodsContainOperators_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(ClassWithOperators)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreNotOperators();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not operators,
					             but it contained operators [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
