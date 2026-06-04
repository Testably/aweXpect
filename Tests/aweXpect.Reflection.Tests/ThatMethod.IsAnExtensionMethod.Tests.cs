using System.Reflection;
using System.Reflection.Emit;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsAnExtensionMethod
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodHasNoDeclaringType_ShouldFail()
			{
				MethodInfo subject = new DynamicMethod("Dynamic", typeof(void), Type.EmptyTypes);

				async Task Act()
				{
					await That(subject).IsAnExtensionMethod();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is an extension method,
					              but it was not an extension method {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenMethodIsAnExtensionMethod_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(StaticClassWithExtensionMethods).GetMethod(
						nameof(StaticClassWithExtensionMethods.IsPositive))!;

				async Task Act()
				{
					await That(subject).IsAnExtensionMethod();
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
					await That(subject).IsAnExtensionMethod();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is an extension method,
					              but it was not an extension method {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsAnExtensionMethod();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is an extension method,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenMethodIsAnExtensionMethod_ShouldFail()
			{
				MethodInfo subject =
					typeof(StaticClassWithExtensionMethods).GetMethod(
						nameof(StaticClassWithExtensionMethods.IsPositive))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAnExtensionMethod());
				}

				await That(Act).Throws<XunitException>()
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
					await That(subject).DoesNotComplyWith(it => it.IsAnExtensionMethod());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET10_0_OR_GREATER
		public sealed class NewSyntaxTests
		{
			[Fact]
			public async Task WhenMethodIsANewSyntaxInstanceExtensionMethod_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(StaticClassWithNewExtensionMethods).GetMethod(
						nameof(StaticClassWithNewExtensionMethods.IsLongText))!;

				async Task Act()
				{
					await That(subject).IsAnExtensionMethod();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodIsAStaticExtensionMethod_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(StaticClassWithNewExtensionMethods).GetMethod(
						nameof(StaticClassWithNewExtensionMethods.Create), Type.EmptyTypes)!;

				async Task Act()
				{
					await That(subject).IsAnExtensionMethod();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodIsAStaticExtensionMethodWithParameters_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(StaticClassWithNewExtensionMethods).GetMethod(
						nameof(StaticClassWithNewExtensionMethods.Combine), [typeof(int),])!;

				async Task Act()
				{
					await That(subject).IsAnExtensionMethod();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodIsARegularStaticMethodInExtensionClass_ShouldFail()
			{
				MethodInfo subject =
					typeof(StaticClassWithNewExtensionMethods).GetMethod(
						nameof(StaticClassWithNewExtensionMethods.RegularStaticMethod))!;

				async Task Act()
				{
					await That(subject).IsAnExtensionMethod();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is an extension method,
					              but it was not an extension method {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenMethodMatchesStaticExtensionByNameWithDifferentParameterCount_ShouldFail()
			{
				MethodInfo subject =
					typeof(StaticClassWithNewExtensionMethods).GetMethod(
						nameof(StaticClassWithNewExtensionMethods.Create), [typeof(int),])!;

				async Task Act()
				{
					await That(subject).IsAnExtensionMethod();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is an extension method,
					              but it was not an extension method {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenMethodMatchesStaticExtensionByNameWithDifferentParameterType_ShouldFail()
			{
				MethodInfo subject =
					typeof(StaticClassWithNewExtensionMethods).GetMethod(
						nameof(StaticClassWithNewExtensionMethods.Combine), [typeof(string),])!;

				async Task Act()
				{
					await That(subject).IsAnExtensionMethod();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is an extension method,
					              but it was not an extension method {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenMethodIsAGenericInstanceExtensionMethod_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(GenericClassWithNewExtensionMethods).GetMethod(
						nameof(GenericClassWithNewExtensionMethods.IsNotNullValue))!;

				async Task Act()
				{
					await That(subject).IsAnExtensionMethod();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodIsAGenericStaticExtensionMethod_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(GenericClassWithNewExtensionMethods).GetMethod(
						nameof(GenericClassWithNewExtensionMethods.Empty))!;

				async Task Act()
				{
					await That(subject).IsAnExtensionMethod();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodIsAGenericStaticExtensionMethodWithGenericParameter_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(GenericClassWithNewExtensionMethods).GetMethod(
						nameof(GenericClassWithNewExtensionMethods.Identity))!;

				async Task Act()
				{
					await That(subject).IsAnExtensionMethod();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodIsAGenericStaticExtensionMethodWithConstructedGenericParameter_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(GenericClassWithNewExtensionMethods).GetMethod(
						nameof(GenericClassWithNewExtensionMethods.First))!;

				async Task Act()
				{
					await That(subject).IsAnExtensionMethod();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodIsAGenericStaticExtensionMethodWithOwnTypeParameter_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(GenericClassWithNewExtensionMethods).GetMethod(
						nameof(GenericClassWithNewExtensionMethods.Convert))!;

				async Task Act()
				{
					await That(subject).IsAnExtensionMethod();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
