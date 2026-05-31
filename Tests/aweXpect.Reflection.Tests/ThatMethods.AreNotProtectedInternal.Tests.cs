using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreNotProtectedInternal
	{
		public sealed class EnumerableTests
		{
			[Fact]
			public async Task WhenAllMethodsAreNotProtectedInternal_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod("PublicMethod1",
						BindingFlags.Public | BindingFlags.Instance)!,
				];

				async Task Act()
				{
					await That(subject).AreNotProtectedInternal();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodIsProtectedInternal_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod("ProtectedInternalMethod1",
						BindingFlags.NonPublic | BindingFlags.Instance)!,
				];

				async Task Act()
				{
					await That(subject).AreNotProtectedInternal();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all are not protected internal,
					             but it contained protected internal items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedMethod")]
			[InlineData("PublicMethod")]
			[InlineData("PrivateMethod")]
			public async Task WhenMethodInfoIsNotProtectedInternal_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
				{
					await That(subject).AreNotProtectedInternal();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodInfoIsProtectedInternal_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("ProtectedInternalMethod");

				async Task Act()
				{
					await That(subject).AreNotProtectedInternal();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are not protected internal,
					             but it contained protected internal items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedMethod")]
			[InlineData("PublicMethod")]
			[InlineData("PrivateMethod")]
			public async Task WhenMethodInfoIsNotProtectedInternal_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotProtectedInternal());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             at least one is protected internal,
					             but none were
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodInfoIsProtectedInternal_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("ProtectedInternalMethod");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotProtectedInternal());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
