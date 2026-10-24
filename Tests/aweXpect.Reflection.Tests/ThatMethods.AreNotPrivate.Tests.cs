using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreNotPrivate
	{
		public sealed class EnumerableTests
		{
			[Test]
			public async Task WhenAllMethodsAreNotPrivate_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod("PublicMethod1",
						BindingFlags.Public | BindingFlags.Instance)!,
				];

				async Task Act()
				{
					await That(subject).AreNotPrivate();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodIsPrivate_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod("PrivateMethod1",
						BindingFlags.NonPublic | BindingFlags.Instance)!,
				];

				async Task Act()
				{
					await That(subject).AreNotPrivate();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are private,
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
			public async Task WhenMethodInfoIsNotPrivate_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
				{
					await That(subject).AreNotPrivate();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodInfoIsPrivate_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("PrivateMethod");

				async Task Act()
				{
					await That(subject).AreNotPrivate();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             none are private,
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
			public async Task WhenMethodInfoIsNotPrivate_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotPrivate());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             at least one is private,
					             but none of * were

					             Collection:
					             *
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenMethodInfoIsPrivate_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("PrivateMethod");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotPrivate());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
