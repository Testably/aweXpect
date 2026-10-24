using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreNotPrivateProtected
	{
		public sealed class EnumerableTests
		{
			[Test]
			public async Task WhenAllMethodsAreNotPrivateProtected_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod("PublicMethod1",
						BindingFlags.Public | BindingFlags.Instance)!,
				];

				async Task Act()
				{
					await That(subject).AreNotPrivateProtected();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodIsPrivateProtected_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod("PrivateProtectedMethod1",
						BindingFlags.NonPublic | BindingFlags.Instance)!,
				];

				async Task Act()
				{
					await That(subject).AreNotPrivateProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are private protected,
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

		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedMethod")]
			[Arguments("PublicMethod")]
			[Arguments("InternalMethod")]
			public async Task WhenMethodInfoIsNotPrivateProtected_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
				{
					await That(subject).AreNotPrivateProtected();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodInfoIsPrivateProtected_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("PrivateProtectedMethod");

				async Task Act()
				{
					await That(subject).AreNotPrivateProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             none are private protected,
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
			[Arguments("ProtectedMethod")]
			[Arguments("PublicMethod")]
			[Arguments("InternalMethod")]
			public async Task WhenMethodInfoIsNotPrivateProtected_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotPrivateProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             at least one is private protected,
					             but none of * were

					             Collection:
					             *
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenMethodInfoIsPrivateProtected_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("PrivateProtectedMethod");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotPrivateProtected());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
