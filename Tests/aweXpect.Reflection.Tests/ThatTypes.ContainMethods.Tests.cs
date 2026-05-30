using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class ContainMethods
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllTypesContainMatchingMethod_ShouldSucceed()
			{
				Filtered.Types subject = In.Types<ClassWithMarkedMethod, ClassWithTwoMarkedMethods>();

				async Task Act()
					=> await That(subject).ContainMethods(methods => methods.With<MarkerAttribute>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomeTypeContainsNoMatchingMethod_ShouldFail()
			{
				Filtered.Types subject = In.Types<ClassWithMarkedMethod, ClassWithoutMarkedMethod>();

				async Task Act()
					=> await That(subject).ContainMethods(methods => methods.With<MarkerAttribute>());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that in types ThatTypes.ContainMethods.ClassWithMarkedMethod and ThatTypes.ContainMethods.ClassWithoutMarkedMethod
					             all contain methods with ThatTypes.ContainMethods.MarkerAttribute at least once,
					             but it contained not matching types [
					               ThatTypes.ContainMethods.ClassWithoutMarkedMethod
					             ]
					             """);
			}

			[Fact]
			public async Task Exactly_WhenAllCountsMatch_ShouldSucceed()
			{
				Filtered.Types subject = In.Type<ClassWithTwoMarkedMethods>();

				async Task Act()
					=> await That(subject).ContainMethods(methods => methods.With<MarkerAttribute>()).Exactly(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Never_WhenNoTypeContainsMatchingMethod_ShouldSucceed()
			{
				Filtered.Types subject = In.Type<ClassWithoutMarkedMethod>();

				async Task Act()
					=> await That(subject).ContainMethods(methods => methods.With<MarkerAttribute>()).Never();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllTypesContainMatchingMethod_ShouldFail()
			{
				Filtered.Types subject = In.Types<ClassWithMarkedMethod, ClassWithTwoMarkedMethods>();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they
						=> they.ContainMethods(methods => methods.With<MarkerAttribute>()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in types ThatTypes.ContainMethods.ClassWithMarkedMethod and ThatTypes.ContainMethods.ClassWithTwoMarkedMethods
					             do not all contain methods with ThatTypes.ContainMethods.MarkerAttribute at least once,
					             but it only contained matching types [
					               ThatTypes.ContainMethods.ClassWithMarkedMethod,
					               ThatTypes.ContainMethods.ClassWithTwoMarkedMethods
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSomeTypeContainsNoMatchingMethod_ShouldSucceed()
			{
				Filtered.Types subject = In.Types<ClassWithMarkedMethod, ClassWithoutMarkedMethod>();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they
						=> they.ContainMethods(methods => methods.With<MarkerAttribute>()));

				await That(Act).DoesNotThrow();
			}
		}

		[AttributeUsage(AttributeTargets.Method)]
		private class MarkerAttribute : Attribute
		{
		}

		private class ClassWithMarkedMethod
		{
			[Marker]
			public static void Tagged()
			{
			}
		}

		private class ClassWithTwoMarkedMethods
		{
			[Marker]
			public static void TaggedOne()
			{
			}

			[Marker]
			public static void TaggedTwo()
			{
			}
		}

		private class ClassWithoutMarkedMethod
		{
			public static void Untagged()
			{
			}
		}
	}
}
