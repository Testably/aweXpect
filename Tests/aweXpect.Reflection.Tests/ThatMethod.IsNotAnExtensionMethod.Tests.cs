#if NET10_0_OR_GREATER
using System;
#endif
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotAnExtensionMethod
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodIsAnExtensionMethod_ShouldFail()
			{
				MethodInfo subject =
					typeof(StaticClassWithExtensionMethods).GetMethod(
						nameof(StaticClassWithExtensionMethods.IsPositive))!;

				async Task Act()
				{
					await That(subject).IsNotAnExtensionMethod();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not an extension method,
					              but it was an extension method {Formatter.Format(subject)}
					              """);
			}

			[Fact]
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

			[Fact]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotAnExtensionMethod();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not an extension method,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
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

			[Fact]
			public async Task WhenMethodIsNotAnExtensionMethod_ShouldFail()
			{
				MethodInfo subject =
					typeof(StaticClassWithExtensionMethods).GetMethod(
						nameof(StaticClassWithExtensionMethods.RegularStaticMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnExtensionMethod());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is an extension method,
					              but it was not an extension method {Formatter.Format(subject)}
					              """);
			}
		}

#if NET10_0_OR_GREATER
		public sealed class NewSyntaxTests
		{
			[Fact]
			public async Task WhenMethodIsAStaticExtensionMethod_ShouldFail()
			{
				MethodInfo subject =
					typeof(StaticClassWithNewExtensionMethods).GetMethod(
						nameof(StaticClassWithNewExtensionMethods.Create), Type.EmptyTypes)!;

				async Task Act()
				{
					await That(subject).IsNotAnExtensionMethod();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not an extension method,
					              but it was an extension method {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenMethodIsANewSyntaxInstanceExtensionMethod_ShouldFail()
			{
				MethodInfo subject =
					typeof(StaticClassWithNewExtensionMethods).GetMethod(
						nameof(StaticClassWithNewExtensionMethods.IsLongText))!;

				async Task Act()
				{
					await That(subject).IsNotAnExtensionMethod();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not an extension method,
					              but it was an extension method {Formatter.Format(subject)}
					              """);
			}

			[Fact]
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
