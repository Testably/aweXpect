using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class ContainsMethods
	{
		public sealed class Tests
		{
			[Fact]
			public async Task Exactly_WhenCountDiffers_ShouldFail()
			{
				Type subject = typeof(ClassWithMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>()).Exactly(2);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute exactly twice,
					             but it contained 1 matching member in ThatType.ContainsMethods.ClassWithMarkedMethod
					             """);
			}

			[Fact]
			public async Task Exactly_WhenCountMatches_ShouldSucceed()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>()).Exactly(2);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Never_WhenTypeContainsNoMatchingMethod_ShouldSucceed()
			{
				Type subject = typeof(ClassWithoutMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>()).Never();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeContainsMatchingMethod_ShouldSucceed()
			{
				Type subject = typeof(ClassWithMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeContainsNoMatchingMethod_ShouldFail()
			{
				Type subject = typeof(ClassWithoutMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute at least once,
					             but it contained 0 matching members in ThatType.ContainsMethods.ClassWithoutMarkedMethod
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute at least once,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeContainsMatchingMethod_ShouldFail()
			{
				Type subject = typeof(ClassWithMarkedMethod);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> it.ContainsMethods(methods => methods.With<MarkerAttribute>()));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain methods with ThatType.ContainsMethods.MarkerAttribute at least once,
					             but it contained 1 matching member in ThatType.ContainsMethods.ClassWithMarkedMethod
					             """);
			}

			[Fact]
			public async Task WhenTypeContainsNoMatchingMethod_ShouldSucceed()
			{
				Type subject = typeof(ClassWithoutMarkedMethod);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> it.ContainsMethods(methods => methods.With<MarkerAttribute>()));
				}

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
