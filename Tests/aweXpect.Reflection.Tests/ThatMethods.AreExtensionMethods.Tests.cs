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
	public sealed class AreExtensionMethods
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFilteringOnlyExtensionMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(StaticClassWithExtensionMethods)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.Where(m => m.IsReallyExtensionMethod());

				async Task Act()
				{
					await That(subject).AreExtensionMethods();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsContainNonExtensionMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(StaticClassWithExtensionMethods)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreExtensionMethods();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all extension methods,
					             but it contained non-extension methods

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
			public async Task WhenFilteringOnlyExtensionMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(StaticClassWithExtensionMethods)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.Where(m => m.IsReallyExtensionMethod());

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreExtensionMethods());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are not all extension methods,
					             but it only contained extension methods

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenMethodsContainNonExtensionMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(StaticClassWithExtensionMethods)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreExtensionMethods());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenFilteringOnlyExtensionMethods_Negated_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(StaticClassWithExtensionMethods)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.Where(m => m.IsReallyExtensionMethod())
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreExtensionMethods());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are not all extension methods,
					             but it only contained extension methods

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyExtensionMethods_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(StaticClassWithExtensionMethods)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.Where(m => m.IsReallyExtensionMethod())
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreExtensionMethods();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsContainNonExtensionMethods_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(StaticClassWithExtensionMethods)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreExtensionMethods();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all extension methods,
					             but it contained non-extension methods

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
			public async Task WhenAllAreNewSyntaxExtensionMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(StaticClassWithNewExtensionMethods).GetMethod(
						nameof(StaticClassWithNewExtensionMethods.IsLongText))!,
					typeof(StaticClassWithNewExtensionMethods).GetMethod(
						nameof(StaticClassWithNewExtensionMethods.Create), Type.EmptyTypes)!,
					typeof(StaticClassWithNewExtensionMethods).GetMethod(
						nameof(StaticClassWithNewExtensionMethods.Combine), [typeof(int),])!,
				];

				async Task Act()
				{
					await That(subject).AreExtensionMethods();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenContainingARegularStaticMethod_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(StaticClassWithNewExtensionMethods).GetMethod(
						nameof(StaticClassWithNewExtensionMethods.Create), Type.EmptyTypes)!,
					typeof(StaticClassWithNewExtensionMethods).GetMethod(
						nameof(StaticClassWithNewExtensionMethods.RegularStaticMethod))!,
				];

				async Task Act()
				{
					await That(subject).AreExtensionMethods();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all extension methods,
					             but it contained non-extension methods

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
