#if NET10_0_OR_GREATER
using System;
#endif
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreNotExtensionMethods
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFilteringOnlyNonExtensionMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(StaticClassWithExtensionMethods)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsReallyExtensionMethod());

				async Task Act()
				{
					await That(subject).AreNotExtensionMethods();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsContainExtensionMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(StaticClassWithExtensionMethods)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotExtensionMethods();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all not extension methods,
					             but it contained extension methods

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFilteringOnlyNonExtensionMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(StaticClassWithExtensionMethods)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsReallyExtensionMethod());

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotExtensionMethods());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             also contain an extension method,
					             but it only contained non-extension methods

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenMethodsContainExtensionMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(StaticClassWithExtensionMethods)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotExtensionMethods());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenFilteringOnlyNonExtensionMethods_Negated_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(StaticClassWithExtensionMethods)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsReallyExtensionMethod())
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotExtensionMethods());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             also contain an extension method,
					             but it only contained non-extension methods

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyNonExtensionMethods_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(StaticClassWithExtensionMethods)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsReallyExtensionMethod())
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreNotExtensionMethods();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsContainExtensionMethods_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(StaticClassWithExtensionMethods)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreNotExtensionMethods();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all not extension methods,
					             but it contained extension methods

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif

#if NET10_0_OR_GREATER
		public sealed class NewSyntaxTests
		{
			[Test]
			public async Task WhenAllAreRegularMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(StaticClassWithNewExtensionMethods).GetMethod(
						nameof(StaticClassWithNewExtensionMethods.RegularStaticMethod))!,
					typeof(StaticClassWithNewExtensionMethods).GetMethod(
						nameof(StaticClassWithNewExtensionMethods.Create), [typeof(int),])!,
				];

				async Task Act()
				{
					await That(subject).AreNotExtensionMethods();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenContainingAStaticExtensionMethod_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(StaticClassWithNewExtensionMethods).GetMethod(
						nameof(StaticClassWithNewExtensionMethods.RegularStaticMethod))!,
					typeof(StaticClassWithNewExtensionMethods).GetMethod(
						nameof(StaticClassWithNewExtensionMethods.Create), Type.EmptyTypes)!,
				];

				async Task Act()
				{
					await That(subject).AreNotExtensionMethods();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all not extension methods,
					             but it contained extension methods

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
