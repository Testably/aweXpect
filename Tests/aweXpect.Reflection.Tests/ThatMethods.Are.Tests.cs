using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class Are
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllMethodsHaveTheAccessModifier_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = [Method("PublicMethod1"), Method("PublicMethod2"),];

				async Task Act()
				{
					await That(subject).Are(AccessModifiers.Public);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteredMethodsHaveTheAccessModifier_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("ProtectedInternalMethod");

				async Task Act()
				{
					await That(subject).Are(AccessModifiers.ProtectedInternal);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsContainADifferentAccessModifier_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = [Method("PublicMethod1"), Method("PrivateMethod1"),];

				async Task Act()
				{
					await That(subject).Are(AccessModifiers.Public);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are public,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               int ThatMethods.ClassWithMethods.PrivateMethod1()
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
				IEnumerable<MethodInfo?> subject = [Method("PublicMethod1"), null,];

				async Task Act()
				{
					await That(subject).Are(AccessModifiers.Public);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are public,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1(),
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<MethodInfo?>? subject = null;

				async Task Act()
				{
					await That(subject).Are(AccessModifiers.Public);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are public,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypesHaveTheAccessModifier_ShouldSucceed()
			{
				IEnumerable<Type> subject = [typeof(PublicClass), typeof(PublicStruct),];

				async Task Act()
				{
					await That(subject).Are(AccessModifiers.Public);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUsingAny_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					Method("PublicMethod1"), Method("PrivateMethod1"), Method("PrivateProtectedMethod1"),
				];

				async Task Act()
				{
					await That(subject).Are(AccessModifiers.Any);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUsingAQuantifier_ShouldApplyIt()
			{
				IEnumerable<MethodInfo> subject = [Method("PublicMethod1"), Method("PrivateMethod1"),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Are(AccessModifiers.Public);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is public for at least 2 items,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               int ThatMethods.ClassWithMethods.PrivateMethod1()
					             ]

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1(),
					               int ThatMethods.ClassWithMethods.PrivateMethod1()
					             ]
					             """);
			}

			[Test]
			public async Task WhenUsingCombinedFlags_ShouldFailForOtherModifiers()
			{
				IEnumerable<MethodInfo> subject = [Method("PublicMethod1"), Method("InternalMethod1"), Method("PrivateMethod1"),];

				async Task Act()
				{
					await That(subject).Are(AccessModifiers.Public | AccessModifiers.Internal);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are public or internal,
					             but only 2 of 3 were

					             Not matching items:
					             [
					               int ThatMethods.ClassWithMethods.PrivateMethod1()
					             ]

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1(),
					               int ThatMethods.ClassWithMethods.InternalMethod1(),
					               int ThatMethods.ClassWithMethods.PrivateMethod1()
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllMethodsHaveTheAccessModifier_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = [Method("PublicMethod1"), Method("PublicMethod2"),];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.Are(AccessModifiers.Public));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all are public,
					             but all 2 were

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1(),
					               int ThatMethods.ClassWithMethods.PublicMethod2()
					             ]
					             """);
			}

			[Test]
			public async Task WhenMethodsContainADifferentAccessModifier_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = [Method("PublicMethod1"), Method("PrivateMethod1"),];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.Are(AccessModifiers.Public));
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenAllMethodsHaveTheAccessModifier_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					Method("PublicMethod1"), Method("PublicMethod2"),
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).Are(AccessModifiers.Public);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsContainADifferentAccessModifier_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					Method("PublicMethod1"), Method("PrivateMethod1"),
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).Are(AccessModifiers.Public);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are public,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               int ThatMethods.ClassWithMethods.PrivateMethod1()
					             ]

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1(),
					               int ThatMethods.ClassWithMethods.PrivateMethod1()
					             ]
					             """);
			}

			[Test]
			public async Task WhenUsingAQuantifier_ShouldApplyIt()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					Method("PublicMethod1"), Method("PrivateMethod1"),
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).Exactly(1).Are(AccessModifiers.Private);
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
