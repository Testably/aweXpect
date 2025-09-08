using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed partial class AreGeneric
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
					Filtered.Methods subject = GetMethods("GenericMethod1");

					async Task Act()
						=> await That(subject).AreGeneric().WithArgumentCount(argumentCount);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that methods matching methodInfo => methodInfo.Name.StartsWith(methodPrefix) in types matching t => t == typeof(ClassWithMethods) in assembly containing type ThatMethods.ClassWithMethods
						             are all generic with 2 generic arguments,
						             but it contained not matching methods [
						               T ThatMethods.ClassWithMethods.GenericMethod1<T>(T value)
						             ]
						             """);
				}

				[Theory]
				[InlineData(1, false)]
				[InlineData(2, true)]
				public async Task ForMethodWithTwoGenericParameters_ShouldSucceedWhenArgumentCountMatches(
					int argumentCount, bool expectSuccess)
				{
					Filtered.Methods subject = GetMethods("GenericMethod2");

					async Task Act()
						=> await That(subject).AreGeneric().WithArgumentCount(argumentCount);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that methods matching methodInfo => methodInfo.Name.StartsWith(methodPrefix) in types matching t => t == typeof(ClassWithMethods) in assembly containing type ThatMethods.ClassWithMethods
						             are all generic with 1 generic argument,
						             but it contained not matching methods [
						               U ThatMethods.ClassWithMethods.GenericMethod2<T, U>(T first, U second)
						             ]
						             """);
				}


				[Fact]
				public async Task WhenMethodIsNotGeneric_ShouldFail()
				{
					Filtered.Methods subject = GetMethods("NonGenericMethod");

					async Task Act()
						=> await That(subject).AreGeneric().WithArgumentCount(1);

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that methods matching methodInfo => methodInfo.Name.StartsWith(methodPrefix) in types matching t => t == typeof(ClassWithMethods) in assembly containing type ThatMethods.ClassWithMethods
						             are all generic with 1 generic argument,
						             but it contained not matching methods [
						               int ThatMethods.ClassWithMethods.NonGenericMethod1(),
						               int ThatMethods.ClassWithMethods.NonGenericMethod2()
						             ]
						             """);
				}
			}
		}
	}
}
