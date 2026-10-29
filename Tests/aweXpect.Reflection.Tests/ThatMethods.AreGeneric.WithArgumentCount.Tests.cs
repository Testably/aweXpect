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
				[Test]
				[Arguments(1, true)]
				[Arguments(2, false)]
				public async Task ForMethodWithOneGenericParameter_ShouldSucceedWhenArgumentCountMatches(
					int argumentCount, bool expectSuccess)
				{
					Filtered.Methods subject = GetMethods("GenericMethod1");

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgumentCount(argumentCount);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that methods matching methodInfo => methodInfo.Name.StartsWith(methodPrefix) in types matching t => t == typeof(ClassWithMethods) in assembly containing type ThatMethods.ClassWithMethods
						             all are generic with 2 generic arguments,
						             but none of 1 were

						             Not matching items:
						             [
						               T ThatMethods.ClassWithMethods.GenericMethod1<T>(T value)
						             ]

						             Collection:
						             [
						               T ThatMethods.ClassWithMethods.GenericMethod1<T>(T value)
						             ]
						             """);
				}

				[Test]
				[Arguments(1, false)]
				[Arguments(2, true)]
				public async Task ForMethodWithTwoGenericParameters_ShouldSucceedWhenArgumentCountMatches(
					int argumentCount, bool expectSuccess)
				{
					Filtered.Methods subject = GetMethods("GenericMethod2");

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgumentCount(argumentCount);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that methods matching methodInfo => methodInfo.Name.StartsWith(methodPrefix) in types matching t => t == typeof(ClassWithMethods) in assembly containing type ThatMethods.ClassWithMethods
						             all are generic with 1 generic argument,
						             but none of 1 were

						             Not matching items:
						             [
						               U ThatMethods.ClassWithMethods.GenericMethod2<T, U>(T first, U second)
						             ]

						             Collection:
						             [
						               U ThatMethods.ClassWithMethods.GenericMethod2<T, U>(T first, U second)
						             ]
						             """);
				}


				[Test]
				public async Task WhenMethodIsNotGeneric_ShouldFail()
				{
					Filtered.Methods subject = GetMethods("NonGenericMethod");

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgumentCount(1);
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that methods matching methodInfo => methodInfo.Name.StartsWith(methodPrefix) in types matching t => t == typeof(ClassWithMethods) in assembly containing type ThatMethods.ClassWithMethods
						             all are generic with 1 generic argument,
						             but none of 2 were

						             Not matching items:
						             [
						               int ThatMethods.ClassWithMethods.NonGenericMethod1(),
						               int ThatMethods.ClassWithMethods.NonGenericMethod2()
						             ]

						             Collection:
						             [
						               int ThatMethods.ClassWithMethods.NonGenericMethod1(),
						               int ThatMethods.ClassWithMethods.NonGenericMethod2()
						             ]
						             """);
				}
			}
		}
	}
}
