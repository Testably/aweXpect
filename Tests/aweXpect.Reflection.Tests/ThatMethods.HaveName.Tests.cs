using System.Collections.Generic;
using System.Reflection;
using aweXpect.Customization;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed partial class HaveName
	{
		public sealed class EnumerableTests
		{
			[Test]
			public async Task WhenAllMethodsHaveName_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1))!,
				];

				async Task Act()
				{
					await That(subject).HaveName("PublicMethod1");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMoreMethodsDoNotMatchSelectorThanTheMaximumNumberOfCollectionItems_ShouldLimitTheList()
			{
				using IDisposable _ = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Set(1);
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1))!,
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod2))!,
				];

				async Task Act()
				{
					await That(subject).HaveName(method => method.Name + "X");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have name matching method => method.Name + "X",
					             but none of 2 did

					             Not matching items:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1() with name "PublicMethod1" instead of "PublicMethod1X",
					               (… and 1 more)
					             ]

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1() with name "PublicMethod1" instead of "PublicMethod1X",
					               (… and 1 more)
					             ]
					             """)
					.Because("the not matching members are limited like any other collection in a failure message");
			}

			[Test]
			public async Task WhenMultipleMethodsDoNotMatchSelector_ShouldListAllSeparatedByComma()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1))!,
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod2))!,
				];

				async Task Act()
				{
					await That(subject).HaveName(method => method.Name + "X");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have name matching method => method.Name + "X",
					             but none of 2 did

					             Not matching items:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1() with name "PublicMethod1" instead of "PublicMethod1X",
					               int ThatMethods.ClassWithMethods.PublicMethod2() with name "PublicMethod2" instead of "PublicMethod2X"
					             ]

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1() with name "PublicMethod1" instead of "PublicMethod1X",
					               int ThatMethods.ClassWithMethods.PublicMethod2() with name "PublicMethod2" instead of "PublicMethod2X"
					             ]
					             """);
			}

			[Test]
			public async Task WhenSelectorMatchesEachName_Negated_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName(method => method.Name));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all have name matching method => method.Name,
					             but all 1 did

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1()
					             ]
					             """);
			}

			[Test]
			public async Task WhenSelectorMatchesEachName_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1))!,
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod2))!,
				];

				async Task Act()
				{
					await That(subject).HaveName(method => method.Name);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSelectorIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1))!,
				];

				async Task Act()
				{
					await That(subject).HaveName((Func<MethodInfo, string>)null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectedNameSelector").And
					.WithMessage("The 'expectedNameSelector' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenSelectorThrows_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1))!,
				];

				async Task Act()
				{
					await That(subject).HaveName(_ => throw new InvalidOperationException("boom"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have name matching _ => throw new InvalidOperationException("boom"),
					             but the name selector did throw an InvalidOperationException:
					               boom

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1()
					             ]
					             """)
					.Because("an exception of the name selector must fail the expectation instead of aborting the evaluation");
			}

			[Test]
			public async Task WhenSelectorThrows_Negated_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.HaveName(_ => throw new InvalidOperationException("boom")));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all have name matching _ => throw new InvalidOperationException("boom"),
					             but the name selector did throw an InvalidOperationException:
					               boom

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1()
					             ]
					             """)
					.Because("a name selector that throws answers neither the expectation nor its negation");
			}
		}

		public sealed class Tests
		{
			[Test]
			public async Task WhenMethodInfosContainMethodInfoWithDifferentName_ShouldFail()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithMethods>().Methods();

				async Task Act()
				{
					await That(subject).HaveName("PublicMethod");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods in types matching t => t == typeof(T) in assembly containing type ThatMethod.ClassWithMethods
					             all have name equal to "PublicMethod",
					             but only 1 of 12 did

					             Not matching items:
					             [
					               int ThatMethod.ClassWithMethods.InternalMethod(),
					               int ThatMethod.ClassWithMethods.ProtectedMethod(),
					               int ThatMethod.ClassWithMethods.PrivateMethod(),
					               int ThatMethod.ClassWithMethods.ProtectedInternalMethod(),
					               int ThatMethod.ClassWithMethods.PrivateProtectedMethod(),
					               T ThatMethod.ClassWithMethods.GenericMethod<T>(T value),
					               void ThatMethod.ClassWithMethods.AnotherGenericMethod<T, U>(T first, U second),
					               int ThatMethod.ClassWithMethods.NonGenericMethod(),
					               void ThatMethod.ClassWithMethods.VoidMethod(),
					               void ThatMethod.ClassWithMethods.GenericWithUnrestrictedArgumentMethod<TFoo>(),
					               (… and 1 more)
					             ]

					             Collection:
					             [
					               int ThatMethod.ClassWithMethods.PublicMethod(),
					               int ThatMethod.ClassWithMethods.InternalMethod(),
					               int ThatMethod.ClassWithMethods.ProtectedMethod(),
					               int ThatMethod.ClassWithMethods.PrivateMethod(),
					               int ThatMethod.ClassWithMethods.ProtectedInternalMethod(),
					               int ThatMethod.ClassWithMethods.PrivateProtectedMethod(),
					               T ThatMethod.ClassWithMethods.GenericMethod<T>(T value),
					               void ThatMethod.ClassWithMethods.AnotherGenericMethod<T, U>(T first, U second),
					               int ThatMethod.ClassWithMethods.NonGenericMethod(),
					               void ThatMethod.ClassWithMethods.VoidMethod(),
					               (… and 2 more)
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenMethodInfosHaveName_ShouldSucceed()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithSingleMethod>().Methods();

				async Task Act()
				{
					await That(subject).HaveName("MyMethod");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodInfosMatchIgnoringCase_ShouldSucceed()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithSingleMethod>().Methods();

				async Task Act()
				{
					await That(subject).HaveName("mYmethod").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodInfosMatchSuffix_ShouldSucceed()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithMethods>().Methods();

				async Task Act()
				{
					await That(subject).HaveName("Method").AsSuffix();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSelectorMatchesAsPrefix_ShouldSucceed()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithSingleMethod>().Methods();

				async Task Act()
				{
					await That(subject).HaveName(_ => "My").AsPrefix();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSelectorMatchesIgnoringCase_ShouldSucceed()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithSingleMethod>().Methods();

				async Task Act()
				{
					await That(subject).HaveName(method => method.Name.ToUpperInvariant()).IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSelectorReturnsDifferentName_ShouldFail()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithSingleMethod>().Methods();

				async Task Act()
				{
					await That(subject).HaveName(method => method.Name + "X");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods in types matching t => t == typeof(T) in assembly containing type ThatMethod.ClassWithSingleMethod
					             all have name matching method => method.Name + "X",
					             but none of 1 did

					             Not matching items:
					             [
					               int ThatMethod.ClassWithSingleMethod.MyMethod() with name "MyMethod" instead of "MyMethodX"
					             ]

					             Collection:
					             [
					               int ThatMethod.ClassWithSingleMethod.MyMethod() with name "MyMethod" instead of "MyMethodX"
					             ]
					             """);
			}

			[Test]
			public async Task WhenSelectorReturnsEachMethodsOwnName_ShouldSucceed()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithMethods>().Methods();

				async Task Act()
				{
					await That(subject).HaveName(method => method.Name);
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenMethodsDoNotHaveName_ShouldSucceed()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithMethods>().Methods();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName("NonExistentMethod"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsHaveName_ShouldFail()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithSingleMethod>().Methods();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName("MyMethod"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods in types matching t => t == typeof(T) in assembly containing type ThatMethod.ClassWithSingleMethod
					             not all have name equal to "MyMethod",
					             but all 1 did

					             Collection:
					             [
					               int ThatMethod.ClassWithSingleMethod.MyMethod()
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenSelectorMatchesEveryName_ShouldFail()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithSingleMethod>().Methods();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName(method => method.Name));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods in types matching t => t == typeof(T) in assembly containing type ThatMethod.ClassWithSingleMethod
					             not all have name matching method => method.Name,
					             but all 1 did*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenSelectorMatchesNoName_ShouldSucceed()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithSingleMethod>().Methods();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName(method => method.Name + "X"));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
