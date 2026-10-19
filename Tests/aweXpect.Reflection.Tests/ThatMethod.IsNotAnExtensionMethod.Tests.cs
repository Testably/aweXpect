#if NET10_0_OR_GREATER
using System;
#endif
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotAnExtensionMethod
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMethodIsAnExtensionMethod_ShouldFail()
			{
				MethodInfo subject =
					typeof(StaticClassWithExtensionMethods).GetMethod(
						nameof(StaticClassWithExtensionMethods.IsPositive))!;

				async Task Act()
				{
					await That(subject).IsNotAnExtensionMethod();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not an extension method,
					              but it was an extension method bool StaticClassWithExtensionMethods.IsPositive(int value)
					              """);
			}

			[Test]
			public async Task WhenMethodIsNotAnExtensionMethod_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(StaticClassWithExtensionMethods).GetMethod(
						nameof(StaticClassWithExtensionMethods.RegularStaticMethod))!;

				async Task Act()
				{
					await That(subject).IsNotAnExtensionMethod();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotAnExtensionMethod();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not an extension method,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenMethodIsAnExtensionMethod_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(StaticClassWithExtensionMethods).GetMethod(
						nameof(StaticClassWithExtensionMethods.IsPositive))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnExtensionMethod());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodIsNotAnExtensionMethod_ShouldFail()
			{
				MethodInfo subject =
					typeof(StaticClassWithExtensionMethods).GetMethod(
						nameof(StaticClassWithExtensionMethods.RegularStaticMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnExtensionMethod());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					              Expected that subject
					              is an extension method,
					              but it was not an extension method void StaticClassWithExtensionMethods.RegularStaticMethod()
					              """);
			}
		}

#if NET10_0_OR_GREATER
		public sealed class NewSyntaxTests
		{
			[Test]
			public async Task WhenMethodIsAStaticExtensionMethod_ShouldFail()
			{
				MethodInfo subject =
					typeof(StaticClassWithNewExtensionMethods).GetMethod(
						nameof(StaticClassWithNewExtensionMethods.Create), Type.EmptyTypes)!;

				async Task Act()
				{
					await That(subject).IsNotAnExtensionMethod();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not an extension method,
					              but it was an extension method string StaticClassWithNewExtensionMethods.Create()
					              """);
			}

			[Test]
			public async Task WhenMethodIsANewSyntaxInstanceExtensionMethod_ShouldFail()
			{
				MethodInfo subject =
					typeof(StaticClassWithNewExtensionMethods).GetMethod(
						nameof(StaticClassWithNewExtensionMethods.IsLongText))!;

				async Task Act()
				{
					await That(subject).IsNotAnExtensionMethod();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not an extension method,
					              but it was an extension method bool StaticClassWithNewExtensionMethods.IsLongText(string text)
					              """);
			}

			[Test]
			public async Task WhenMethodIsARegularStaticMethodInExtensionClass_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(StaticClassWithNewExtensionMethods).GetMethod(
						nameof(StaticClassWithNewExtensionMethods.RegularStaticMethod))!;

				async Task Act()
				{
					await That(subject).IsNotAnExtensionMethod();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
