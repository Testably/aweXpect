using System.Collections.Generic;
using aweXpect.Reflection.Collections;
#if NET8_0_OR_GREATER
using System.Reflection;
using System.Threading;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class ContainMethods
	{
		public sealed class TypeTests
		{
			[Test]
			public async Task WhenAllTypesContainMatchingMethod_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithMarkedMethod), typeof(ClassWithTwoMarkedMethods),
				};

				async Task Act()
				{
					await That(subject).ContainMethods(methods => methods.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsNullType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithMarkedMethod), null,
				};

				async Task Act()
				{
					await That(subject).ContainMethods(methods => methods.With<MarkerAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all contain methods with ThatTypes.ContainMethods.MarkerAttribute at least once,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.ContainMethods.ClassWithMarkedMethod,
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenSomeTypeContainsNoMatchingMethod_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithMarkedMethod), typeof(ClassWithoutMarkedMethod),
				};

				async Task Act()
				{
					await That(subject).ContainMethods(methods => methods.With<MarkerAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all contain methods with ThatTypes.ContainMethods.MarkerAttribute at least once,
					             but it contained not matching types

					             Not matching items:
					             [
					               ThatTypes.ContainMethods.ClassWithoutMarkedMethod
					             ]
					             """);
			}
		}

		public sealed class Tests
		{
			[Test]
			public async Task Exactly_WhenAllCountsMatch_ShouldSucceed()
			{
				Filtered.Types subject = In.Type<ClassWithTwoMarkedMethods>();

				async Task Act()
				{
					await That(subject).ContainMethods(methods => methods.With<MarkerAttribute>()).Exactly(2);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Never_WhenNoTypeContainsMatchingMethod_ShouldSucceed()
			{
				Filtered.Types subject = In.Type<ClassWithoutMarkedMethod>();

				async Task Act()
				{
					await That(subject).ContainMethods(methods => methods.With<MarkerAttribute>()).Never();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllTypesContainMatchingMethod_ShouldSucceed()
			{
				Filtered.Types subject = In.Types<ClassWithMarkedMethod, ClassWithTwoMarkedMethods>();

				async Task Act()
				{
					await That(subject).ContainMethods(methods => methods.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOnlyObjectMethodsMatch_IncludingObjectMembers_ShouldSucceed()
			{
				Filtered.Types subject = In.Types<StructWithMembers, ClassWithMarkedMethod>();

				async Task Act()
				{
					await That(subject).ContainMethods(methods => methods.WithName(nameof(Equals)))
						.IncludingObjectMembers();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOnlyObjectMethodsMatch_ShouldFail()
			{
				Filtered.Types subject = In.Types<StructWithMembers, ClassWithMarkedMethod>();

				async Task Act()
				{
					await That(subject).ContainMethods(methods => methods.WithName(nameof(Equals)));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in types StructWithMembers and ThatTypes.ContainMethods.ClassWithMarkedMethod
					             all contain methods with name equal to "Equals" at least once,
					             but it contained not matching types*
					             """).AsWildcard()
					.Because("the members declared on System.Object and System.ValueType are excluded by default");
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenAsyncEnumerableTypesOnlyContainInheritedMethod_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClassWithInheritedMarkedMethod),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).ContainMethods(methods => methods.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenCancellationTokenIsSet_ShouldPassItToTheInnerFilter()
			{
				using CancellationTokenSource cts = new();
				TokenRecordingFilter<MethodInfo> filter = new();
				Filtered.Types subject = In.Type<ClassWithMarkedMethod>();

				await That(subject).ContainMethods(methods => methods.Which(filter)).WithCancellation(cts.Token);

				await That(filter.Tokens).IsNotEmpty();
				await That(filter.Tokens).All().AreEqualTo(cts.Token);
			}
#endif

			[Test]
			public async Task WhenSomeTypeContainsNoMatchingMethod_ShouldFail()
			{
				Filtered.Types subject = In.Types<ClassWithMarkedMethod, ClassWithoutMarkedMethod>();

				async Task Act()
				{
					await That(subject).ContainMethods(methods => methods.With<MarkerAttribute>());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that in types ThatTypes.ContainMethods.ClassWithMarkedMethod and ThatTypes.ContainMethods.ClassWithoutMarkedMethod
					             all contain methods with ThatTypes.ContainMethods.MarkerAttribute at least once,
					             but it contained not matching types

					             Not matching items:
					             [
					               ThatTypes.ContainMethods.ClassWithoutMarkedMethod
					             ]
					             """);
			}

			[Test]
			public async Task WhenTypeOnlyContainsInheritedMethod_Directly_ShouldFail()
			{
				Filtered.Types subject =
					In.Types<DerivedClassWithInheritedMarkedMethod, BaseClassWithMarkedMethod>();

				async Task Act()
				{
					await That(subject).ContainMethods(methods => methods.With<MarkerAttribute>()).Directly();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that in types ThatTypes.ContainMethods.DerivedClassWithInheritedMarkedMethod and ThatTypes.ContainMethods.BaseClassWithMarkedMethod
					             all declare methods with ThatTypes.ContainMethods.MarkerAttribute at least once,
					             but it contained not matching types

					             Not matching items:
					             [
					               ThatTypes.ContainMethods.DerivedClassWithInheritedMarkedMethod
					             ]
					             """);
			}

			[Test]
			public async Task WhenTypesOnlyContainInheritedMethod_ShouldSucceed()
			{
				Filtered.Types subject =
					In.Types<DerivedClassWithInheritedMarkedMethod, BaseClassWithMarkedMethod>();

				async Task Act()
				{
					await That(subject).ContainMethods(methods => methods.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllTypesContainMatchingMethod_ShouldFail()
			{
				Filtered.Types subject = In.Types<ClassWithMarkedMethod, ClassWithTwoMarkedMethods>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.ContainMethods(methods => methods.With<MarkerAttribute>()));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in types ThatTypes.ContainMethods.ClassWithMarkedMethod and ThatTypes.ContainMethods.ClassWithTwoMarkedMethods
					             do not all contain methods with ThatTypes.ContainMethods.MarkerAttribute at least once,
					             but it only contained matching types

					             Collection:
					             [
					               ThatTypes.ContainMethods.ClassWithMarkedMethod,
					               ThatTypes.ContainMethods.ClassWithTwoMarkedMethods
					             ]
					             """);
			}

			[Test]
			public async Task WhenSomeTypeContainsNoMatchingMethod_ShouldSucceed()
			{
				Filtered.Types subject = In.Types<ClassWithMarkedMethod, ClassWithoutMarkedMethod>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.ContainMethods(methods => methods.With<MarkerAttribute>()));
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
