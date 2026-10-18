using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreNotProtectedInternal
	{
		public sealed class EnumerableTests
		{
			[Test]
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not protected internal,
					             but it contained protected internal items

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedMethod")]
			[Arguments("PublicMethod")]
			[Arguments("PrivateMethod")]
			public async Task WhenMethodInfoIsNotProtectedInternal_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
				{
					await That(subject).AreNotProtectedInternal();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodInfoIsProtectedInternal_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("ProtectedInternalMethod");

				async Task Act()
				{
					await That(subject).AreNotProtectedInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             are all not protected internal,
					             but it contained protected internal items

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
			[Arguments("ProtectedMethod")]
			[Arguments("PublicMethod")]
			[Arguments("PrivateMethod")]
			public async Task WhenMethodInfoIsNotProtectedInternal_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotProtectedInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             also contain a protected internal item,
					             but it contained no protected internal items

					             Collection:
					             *
					             """).AsWildcard();
			}

			[Test]
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
