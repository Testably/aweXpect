using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreNot
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMethodsContainTheAccessModifier_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = [Method("PublicMethod1"), Method("PrivateMethod1"),];

				async Task Act()
				{
					await That(subject).AreNot(AccessModifiers.Public);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are public,
					             but 1 of 2 were

					             Matching items:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1()
					             ]

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1(),
					               int ThatMethods.ClassWithMethods.PrivateMethod1()
					             ]
					             """);
			}

			[Test]
			public async Task WhenMethodsContainNull_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject = [Method("PrivateMethod1"), null,];

				async Task Act()
				{
					await That(subject).AreNot(AccessModifiers.Public);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are public,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PrivateMethod1(),
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenNoMethodHasTheAccessModifier_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = [Method("InternalMethod1"), Method("PrivateMethod1"),];

				async Task Act()
				{
					await That(subject).AreNot(AccessModifiers.Public);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<MethodInfo?>? subject = null;

				async Task Act()
				{
					await That(subject).AreNot(AccessModifiers.Public);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are public,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenUsingAny_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = [Method("PrivateMethod1"),];

				async Task Act()
				{
					await That(subject).AreNot(AccessModifiers.Any);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have any access modifier,
					             but 1 of 1 did

					             Matching items:
					             [
					               int ThatMethods.ClassWithMethods.PrivateMethod1()
					             ]

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PrivateMethod1()
					             ]
					             """);
			}

			[Test]
			public async Task WhenUsingCombinedFlags_ShouldFailForAnyOfThem()
			{
				IEnumerable<MethodInfo> subject = [Method("InternalMethod1"), Method("PrivateMethod1"),];

				async Task Act()
				{
					await That(subject).AreNot(AccessModifiers.Public | AccessModifiers.Internal);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are public or internal,
					             but 1 of 2 were

					             Matching items:
					             [
					               int ThatMethods.ClassWithMethods.InternalMethod1()
					             ]

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.InternalMethod1(),
					               int ThatMethods.ClassWithMethods.PrivateMethod1()
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenMethodsContainTheAccessModifier_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = [Method("PublicMethod1"), Method("PrivateMethod1"),];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNot(AccessModifiers.Public));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNoMethodHasTheAccessModifier_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = [Method("PrivateMethod1"),];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNot(AccessModifiers.Public));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one is public,
					             but none of 1 were

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PrivateMethod1()
					             ]
					             """);
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenMethodsContainTheAccessModifier_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					Method("PublicMethod1"), Method("PrivateMethod1"),
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreNot(AccessModifiers.Public);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are public,
					             but 1 of 2 were

					             Matching items:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1()
					             ]

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1(),
					               int ThatMethods.ClassWithMethods.PrivateMethod1()
					             ]
					             """);
			}

			[Test]
			public async Task WhenNoMethodHasTheAccessModifier_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					Method("InternalMethod1"), Method("PrivateMethod1"),
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreNot(AccessModifiers.Public);
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif

		private static MethodInfo Method(string name)
			=> typeof(ClassWithMethods).GetMethod(name,
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
	}
}
