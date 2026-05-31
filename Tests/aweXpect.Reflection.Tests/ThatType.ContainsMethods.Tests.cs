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

		public sealed class QuantifierTests
		{
			[Fact]
			public async Task AtLeast_Once_WhenCountIsSufficient_ShouldSucceed()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.AtLeast().Once();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AtLeast_Twice_WhenCountIsSufficient_ShouldSucceed()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.AtLeast().Twice();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AtLeast_Times_WhenCountIsSufficient_ShouldSucceed()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.AtLeast(2);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AtMost_Twice_WhenCountIsWithinLimit_ShouldSucceed()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.AtMost().Twice();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AtMost_Times_WhenCountIsWithinLimit_ShouldSucceed()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.AtMost(2);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Between_And_WhenCountIsWithinRange_ShouldSucceed()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.Between(1).And(2);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task LessThan_Twice_WhenCountIsBelowLimit_ShouldSucceed()
			{
				Type subject = typeof(ClassWithMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.LessThan().Twice();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task LessThan_Times_WhenCountIsBelowLimit_ShouldSucceed()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.LessThan(3);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task MoreThan_Once_WhenCountIsAboveLimit_ShouldSucceed()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.MoreThan().Once();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task MoreThan_Times_WhenCountIsAboveLimit_ShouldSucceed()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.MoreThan(1);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Once_WhenCountIsOne_ShouldSucceed()
			{
				Type subject = typeof(ClassWithMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.Once();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Twice_WhenCountIsTwo_ShouldSucceed()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.Twice();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AtMost_WhenCountExceedsLimit_ShouldFail()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.AtMost().Once();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute at most once,
					             but it contained 2 matching members in ThatType.ContainsMethods.ClassWithTwoMarkedMethods
					             """);
			}

			[Fact]
			public async Task AtLeast_Fluent_WhenCountIsInsufficient_ShouldFail()
			{
				Type subject = typeof(ClassWithMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.AtLeast().Twice();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute at least twice,
					             but it contained 1 matching member in ThatType.ContainsMethods.ClassWithMarkedMethod
					             """);
			}

			[Fact]
			public async Task AtLeast_Times_WhenCountIsInsufficient_ShouldFail()
			{
				Type subject = typeof(ClassWithMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.AtLeast(2);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute at least twice,
					             but it contained 1 matching member in ThatType.ContainsMethods.ClassWithMarkedMethod
					             """);
			}

			[Fact]
			public async Task AtMost_Times_WhenCountExceedsLimit_ShouldFail()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.AtMost(1);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute at most once,
					             but it contained 2 matching members in ThatType.ContainsMethods.ClassWithTwoMarkedMethods
					             """);
			}

			[Fact]
			public async Task Between_And_WhenCountIsBelowRange_ShouldFail()
			{
				Type subject = typeof(ClassWithMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.Between(2).And(3);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute between 2 and 3 times,
					             but it contained 1 matching member in ThatType.ContainsMethods.ClassWithMarkedMethod
					             """);
			}

			[Fact]
			public async Task LessThan_Fluent_WhenCountReachesLimit_ShouldFail()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.LessThan().Twice();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute less than twice,
					             but it contained 2 matching members in ThatType.ContainsMethods.ClassWithTwoMarkedMethods
					             """);
			}

			[Fact]
			public async Task LessThan_Times_WhenCountReachesLimit_ShouldFail()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.LessThan(2);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute less than twice,
					             but it contained 2 matching members in ThatType.ContainsMethods.ClassWithTwoMarkedMethods
					             """);
			}

			[Fact]
			public async Task MoreThan_Fluent_WhenCountReachesLimit_ShouldFail()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.MoreThan().Twice();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute more than twice,
					             but it contained 2 matching members in ThatType.ContainsMethods.ClassWithTwoMarkedMethods
					             """);
			}

			[Fact]
			public async Task MoreThan_Times_WhenCountReachesLimit_ShouldFail()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.MoreThan(2);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute more than twice,
					             but it contained 2 matching members in ThatType.ContainsMethods.ClassWithTwoMarkedMethods
					             """);
			}

			[Fact]
			public async Task Once_WhenCountIsTwo_ShouldFail()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.Once();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute exactly once,
					             but it contained 2 matching members in ThatType.ContainsMethods.ClassWithTwoMarkedMethods
					             """);
			}

			[Fact]
			public async Task Twice_WhenCountIsOne_ShouldFail()
			{
				Type subject = typeof(ClassWithMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.Twice();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute exactly twice,
					             but it contained 1 matching member in ThatType.ContainsMethods.ClassWithMarkedMethod
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
