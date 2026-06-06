using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreOperators
	{
		public sealed class Tests
		{
			[Fact]
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

			[Fact]
			public async Task WhenMethodsContainNonOperators_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithOperators)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreOperators();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all operators,
					             but it contained non-operators [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyOperators_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithOperators)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.Where(m => m.IsReallyOperator());

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreOperators());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all operators,
					             but it only contained operators [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all operators,
					             but it only contained operators [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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

			[Fact]
			public async Task WhenMethodsContainNonOperators_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(ClassWithOperators)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreOperators();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all operators,
					             but it contained non-operators [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
