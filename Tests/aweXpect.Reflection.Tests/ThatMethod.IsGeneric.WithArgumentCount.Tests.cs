using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed partial class IsGeneric
	{
		public sealed class WithArgumentCount
		{
			public sealed class Tests
			{
				[Theory]
				[InlineData(1, true)]
				[InlineData(2, false)]
				public async Task ForMethodWithOneGenericParameter_ShouldSucceedWhenArgumentCountMatches(
					int argumentCount, bool expectSuccess)
				{
					MethodInfo? subject = GetMethod("GenericMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgumentCount(argumentCount);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that subject
						             is generic with 2 generic arguments,
						             but it was generic T ThatMethod.ClassWithMethods.GenericMethod<T>(T value)
						             """);
				}

				[Theory]
				[InlineData(1, false)]
				[InlineData(2, true)]
				public async Task ForMethodWithTwoGenericParameters_ShouldSucceedWhenArgumentCountMatches(
					int argumentCount, bool expectSuccess)
				{
					MethodInfo? subject = GetMethod("AnotherGenericMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgumentCount(argumentCount);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that subject
						             is generic with 1 generic argument,
						             but it was generic void ThatMethod.ClassWithMethods.AnotherGenericMethod<T, U>(T first, U second)
						             """);
				}


				[Fact]
				public async Task WhenMethodIsNotGeneric_ShouldFail()
				{
					MethodInfo? subject = GetMethod("NonGenericMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgumentCount(1);

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that subject
						             is generic with 1 generic argument,
						             but it was non-generic int ThatMethod.ClassWithMethods.NonGenericMethod()
						             """);
				}
			}
		}
	}
}
