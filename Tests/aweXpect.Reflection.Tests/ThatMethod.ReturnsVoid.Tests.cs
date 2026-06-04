using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class ReturnsVoid
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodDoesNotReturnVoid_ShouldFail()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
				{
					await That(subject).ReturnsVoid();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             returns void,
					             but it returned int
					             """);
			}

			[Fact]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).ReturnsVoid();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             returns void,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenMethodReturnsVoid_ShouldSucceed()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.VoidMethod))!;

				async Task Act()
				{
					await That(subject).ReturnsVoid();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OrReturnsTests
		{
			[Fact]
			public async Task WhenMethodReturnsNoneOfTheTypes_ShouldFail()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
				{
					await That(subject).ReturnsVoid().OrReturns<bool>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             returns void or bool,
					             but it returned int
					             """);
			}

			[Fact]
			public async Task WhenMethodReturnsOneOfTheTypes_ShouldSucceed()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
				{
					await That(subject).ReturnsVoid().OrReturns<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodReturnsVoid_ShouldSucceed()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.VoidMethod))!;

				async Task Act()
				{
					await That(subject).ReturnsVoid().OrReturns<int>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenMethodDoesNotReturnVoid_ShouldSucceed()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.ReturnsVoid());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.ReturnsVoid());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             does not return void,
					             but it was <null>
					             """)
					.AsWildcard();
			}

			[Fact]
			public async Task WhenMethodReturnsVoid_ShouldFail()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.VoidMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.ReturnsVoid());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not return void,
					             but it did
					             """)
					.AsWildcard();
			}
		}
	}
}
