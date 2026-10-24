using System.Threading;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using TUnit.Core.Exceptions;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class ContainsMethods
	{
		public sealed class Tests
		{
			[Test]
			public async Task Exactly_AfterBecause_ShouldApplyBoth()
			{
				Type subject = typeof(ClassWithMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.Because("we want two of them").Exactly(2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute exactly twice, because we want two of them,
					             but it contained 1 matching member in ThatType.ContainsMethods.ClassWithMarkedMethod
					             """)
					.Because("Because(…) must return the result, so that the quantifier can still follow it");
			}

			[Test]
			public async Task Exactly_WhenCountDiffers_ShouldFail()
			{
				Type subject = typeof(ClassWithMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>()).Exactly(2);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute exactly twice,
					             but it contained 1 matching member in ThatType.ContainsMethods.ClassWithMarkedMethod
					             """);
			}

			[Test]
			public async Task Exactly_WhenCountMatches_ShouldSucceed()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>()).Exactly(2);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Never_WhenTypeContainsNoMatchingMethod_ShouldSucceed()
			{
				Type subject = typeof(ClassWithoutMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>()).Never();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenCanceled_ShouldNotBeVerified()
			{
				using CancellationTokenSource cts = new();
				cts.Cancel();
				Type subject = typeof(ClassWithMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.WithCancellation(cts.Token);
				}

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute at least once,
					             but it could not be verified, because the evaluation was already canceled
					             """)
					.Because("the cancellation must reach the counting of the members");
			}

			[Test]
			public async Task WhenDirectlyIsSetAfterTheQuantifier_ShouldApplyIt()
			{
				Type subject = typeof(DerivedClassWithInheritedMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>()).Never().Directly();
				}

				await That(Act).DoesNotThrow()
					.Because("the options chain in any order");
			}

			[Test]
			public async Task WhenFilterIsNull_ShouldThrowArgumentNullException()
			{
				Type subject = typeof(ClassWithMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("filter").And
					.WithMessage("The 'filter' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenFilterThrows_ShouldFail()
			{
				Type subject = typeof(ClassWithMarkedMethod);
				int calls = 0;

				async Task Act()
				{
					await That(subject).ContainsMethods(methods
						=> ++calls > 1 ? throw new InvalidOperationException("boom") : methods.With<MarkerAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute at least once,
					             but the filter did throw an InvalidOperationException:
					               boom
					             """)
					.Because("the filter is first called to describe the members and throws only while the expectation is evaluated");
			}

			[Test]
			public async Task WhenInheritedMethodMatches_Directly_ShouldFail()
			{
				Type subject = typeof(DerivedClassWithInheritedMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>()).Directly();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             declares methods with ThatType.ContainsMethods.MarkerAttribute at least once,
					             but it contained 0 matching members in ThatType.ContainsMethods.DerivedClassWithInheritedMarkedMethod
					             """);
			}

			[Test]
			public async Task WhenInheritedMethodMatches_ShouldSucceed()
			{
				Type subject = typeof(DerivedClassWithInheritedMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenInterfaceInheritsMatchingMethod_ShouldSucceed()
			{
				Type subject = typeof(IDerivedInterfaceWithMembers);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods
						=> methods.WithName(nameof(IBaseInterfaceWithMembers.BaseInterfaceMethod)));
				}

				await That(Act).DoesNotThrow()
					.Because("an interface inherits the members of its base interfaces");
			}

			[Test]
			public async Task WhenObjectMethodDoesNotMatch_IncludingObjectMembers_ShouldFail()
			{
				Type subject = typeof(StructWithMembers);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.WithName("Missing"))
						.IncludingObjectMembers();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains methods with name equal to "Missing" including object members at least once,
					             but it contained 0 matching members in StructWithMembers
					             """);
			}

			[Test]
			public async Task WhenObjectMethodMatches_IncludingObjectMembers_ShouldSucceed()
			{
				Type subject = typeof(StructWithMembers);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.WithName(nameof(Equals)))
						.IncludingObjectMembers();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenObjectMethodMatches_ShouldFail()
			{
				Type subject = typeof(StructWithMembers);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.WithName(nameof(Equals)));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains methods with name equal to "Equals" at least once,
					             but it contained 0 matching members in StructWithMembers
					             """)
					.Because("the members declared on System.Object and System.ValueType are excluded by default");
			}

			[Test]
			public async Task WhenTypeContainsMatchingMethod_ShouldSucceed()
			{
				Type subject = typeof(ClassWithMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeContainsNoMatchingMethod_ShouldFail()
			{
				Type subject = typeof(ClassWithoutMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute at least once,
					             but it contained 0 matching members in ThatType.ContainsMethods.ClassWithoutMarkedMethod
					             """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute at least once,
					             but it was <null>
					             """);
			}
		}

		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_Fluent_WhenCountIsInsufficient_ShouldFail()
			{
				Type subject = typeof(ClassWithMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.AtLeast().Twice();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute at least twice,
					             but it contained 1 matching member in ThatType.ContainsMethods.ClassWithMarkedMethod
					             """);
			}

			[Test]
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

			[Test]
			public async Task AtLeast_Times_WhenCountIsInsufficient_ShouldFail()
			{
				Type subject = typeof(ClassWithMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.AtLeast(2);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute at least twice,
					             but it contained 1 matching member in ThatType.ContainsMethods.ClassWithMarkedMethod
					             """);
			}

			[Test]
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

			[Test]
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

			[Test]
			public async Task AtMost_Times_WhenCountExceedsLimit_ShouldFail()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.AtMost(1);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute at most once,
					             but it contained 2 matching members in ThatType.ContainsMethods.ClassWithTwoMarkedMethods
					             """);
			}

			[Test]
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

			[Test]
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

			[Test]
			public async Task AtMost_WhenCountExceedsLimit_ShouldFail()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.AtMost().Once();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute at most once,
					             but it contained 2 matching members in ThatType.ContainsMethods.ClassWithTwoMarkedMethods
					             """);
			}

			[Test]
			public async Task Between_And_WhenCountIsBelowRange_ShouldFail()
			{
				Type subject = typeof(ClassWithMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.Between(2).And(3);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute between 2 and 3 times,
					             but it contained 1 matching member in ThatType.ContainsMethods.ClassWithMarkedMethod
					             """);
			}

			[Test]
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

			[Test]
			public async Task LessThan_Fluent_WhenCountReachesLimit_ShouldFail()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.LessThan().Twice();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute fewer than twice,
					             but it contained 2 matching members in ThatType.ContainsMethods.ClassWithTwoMarkedMethods
					             """);
			}

			[Test]
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

			[Test]
			public async Task LessThan_Times_WhenCountReachesLimit_ShouldFail()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.LessThan(2);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute fewer than twice,
					             but it contained 2 matching members in ThatType.ContainsMethods.ClassWithTwoMarkedMethods
					             """);
			}

			[Test]
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

			[Test]
			public async Task MoreThan_Fluent_WhenCountReachesLimit_ShouldFail()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.MoreThan().Twice();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute more than twice,
					             but it contained 2 matching members in ThatType.ContainsMethods.ClassWithTwoMarkedMethods
					             """);
			}

			[Test]
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

			[Test]
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

			[Test]
			public async Task MoreThan_Times_WhenCountReachesLimit_ShouldFail()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.MoreThan(2);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute more than twice,
					             but it contained 2 matching members in ThatType.ContainsMethods.ClassWithTwoMarkedMethods
					             """);
			}

			[Test]
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

			[Test]
			public async Task Once_WhenCountIsTwo_ShouldFail()
			{
				Type subject = typeof(ClassWithTwoMarkedMethods);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.Once();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute exactly once,
					             but it contained 2 matching members in ThatType.ContainsMethods.ClassWithTwoMarkedMethods
					             """);
			}

			[Test]
			public async Task Twice_WhenCountIsOne_ShouldFail()
			{
				Type subject = typeof(ClassWithMarkedMethod);

				async Task Act()
				{
					await That(subject).ContainsMethods(methods => methods.With<MarkerAttribute>())
						.Twice();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatType.ContainsMethods.MarkerAttribute exactly twice,
					             but it contained 1 matching member in ThatType.ContainsMethods.ClassWithMarkedMethod
					             """);
			}

			[Test]
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
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeContainsMatchingMethod_ShouldFail()
			{
				Type subject = typeof(ClassWithMarkedMethod);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> it.ContainsMethods(methods => methods.With<MarkerAttribute>()));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain methods with ThatType.ContainsMethods.MarkerAttribute at least once,
					             but it contained 1 matching member in ThatType.ContainsMethods.ClassWithMarkedMethod
					             """);
			}

			[Test]
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

			[Test]
			public async Task WhenTypeDeclaresMatchingMethod_Directly_ShouldFail()
			{
				Type subject = typeof(ClassWithMarkedMethod);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> it.ContainsMethods(methods => methods.With<MarkerAttribute>()).Directly());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not declare methods with ThatType.ContainsMethods.MarkerAttribute at least once,
					             but it contained 1 matching member in ThatType.ContainsMethods.ClassWithMarkedMethod
					             """);
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

		private class BaseClassWithMarkedMethod
		{
#pragma warning disable CA1822 // Mark members as static
			[Marker]
			public void Inherited()
			{
			}
#pragma warning restore CA1822
		}

		private class DerivedClassWithInheritedMarkedMethod : BaseClassWithMarkedMethod
		{
		}
	}
}
