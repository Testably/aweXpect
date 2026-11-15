using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed partial class DoNotHave
	{
		public sealed class AttributeTests
		{
#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WhenAMethodHasAttribute_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.TestMethod)),
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatMethods.DoNotHave.AttributeTests.FooAttribute,
					             but 1 of 1 did

					             Matching items:
					             [
					               void ThatMethods.DoNotHave.AttributeTests.TestClass.TestMethod()
					             ]

					             Collection:
					             [
					               void ThatMethods.DoNotHave.AttributeTests.TestClass.TestMethod()
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_WithPredicate_WhenAMethodHasMatchingAttribute_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.TestMethod)),
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatMethods.DoNotHave.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but 1 of 1 did

					             Matching items:
					             [
					               void ThatMethods.DoNotHave.AttributeTests.TestClass.TestMethod()
					             ]

					             Collection:
					             [
					               void ThatMethods.DoNotHave.AttributeTests.TestClass.TestMethod()
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_WithPredicate_WhenNoMethodHasMatchingAttribute_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.NoAttributeMethod)),
					typeof(TestClass).GetMethod(nameof(TestClass.TestMethod)),
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(foo => foo.Value == 3);
				}

				await That(Act).DoesNotThrow();
			}
#endif

			[Test]
			public async Task Negated_WhenNoMethodHasAttribute_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.NoAttributeMethod)),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHave<FooAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one has ThatMethods.DoNotHave.AttributeTests.FooAttribute,
					             but none of 1 did

					             Collection:
					             [
					               void ThatMethods.DoNotHave.AttributeTests.TestClass.NoAttributeMethod()
					             ]
					             """);
			}

			[Test]
			public async Task WhenAMethodHasAttribute_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.TestMethod)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatMethods.DoNotHave.AttributeTests.FooAttribute,
					             but 1 of 1 did

					             Matching items:
					             [
					               void ThatMethods.DoNotHave.AttributeTests.TestClass.TestMethod()
					             ]

					             Collection:
					             [
					               void ThatMethods.DoNotHave.AttributeTests.TestClass.TestMethod()
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<MethodInfo?> subject = [typeof(TestClass).GetMethod(nameof(TestClass.NoAttributeMethod)), null,];

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHave<FooAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatMethods.DoNotHave.AttributeTests.FooAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               void ThatMethods.DoNotHave.AttributeTests.TestClass.NoAttributeMethod(),
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one has ThatMethods.DoNotHave.AttributeTests.FooAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               void ThatMethods.DoNotHave.AttributeTests.TestClass.NoAttributeMethod(),
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNoMethodHasAttribute_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.NoAttributeMethod)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithPredicate_WhenAMethodHasMatchingAttribute_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.NoAttributeMethod)),
					typeof(TestClass).GetMethod(nameof(TestClass.TestMethod)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatMethods.DoNotHave.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but 1 of 2 did

					             Matching items:
					             [
					               void ThatMethods.DoNotHave.AttributeTests.TestClass.TestMethod()
					             ]

					             Collection:
					             [
					               void ThatMethods.DoNotHave.AttributeTests.TestClass.NoAttributeMethod(),
					               void ThatMethods.DoNotHave.AttributeTests.TestClass.TestMethod()
					             ]
					             """);
			}

			[Test]
			public async Task WithPredicate_WhenMethodInheritsMatchingAttribute_Directly_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject = new[]
				{
					typeof(DerivedClass).GetMethod(nameof(DerivedClass.InheritedMethod)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(foo => foo.Value == 2).Directly();
				}

				await That(Act).DoesNotThrow()
					.Because("with Directly only the attributes declared on the overriding method count");
			}

			[Test]
			public async Task WithPredicate_WhenMethodInheritsMatchingAttribute_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject = new[]
				{
					typeof(DerivedClass).GetMethod(nameof(DerivedClass.InheritedMethod)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatMethods.DoNotHave.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but 1 of 1 did

					             Matching items:
					             [
					               void ThatMethods.DoNotHave.AttributeTests.DerivedClass.InheritedMethod()
					             ]

					             Collection:
					             [
					               void ThatMethods.DoNotHave.AttributeTests.DerivedClass.InheritedMethod()
					             ]
					             """);
			}

			[Test]
			public async Task WithPredicate_WhenNoMethodHasMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.NoAttributeMethod)),
					typeof(TestClass).GetMethod(nameof(TestClass.TestMethod)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(foo => foo.Value == 3);
				}

				await That(Act).DoesNotThrow()
					.Because("neither a missing attribute nor one that does not match the predicate counts");
			}

			[Test]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.TestMethod)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(_ => throw new InvalidOperationException("boom"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatMethods.DoNotHave.AttributeTests.FooAttribute matching _ => throw new InvalidOperationException("boom"),
					             but the predicate did throw an InvalidOperationException:
					               boom

					             Collection:
					             [
					               void ThatMethods.DoNotHave.AttributeTests.TestClass.TestMethod()
					             ]
					             """)
					.Because("an exception of the predicate must fail the negated expectation as well instead of passing");
			}

			[AttributeUsage(AttributeTargets.Method)]
			private class FooAttribute : Attribute
			{
				public int Value { get; set; }
			}

			private class TestClass
			{
				[Foo(Value = 2)]
				public static void TestMethod()
				{
				}

				public static void NoAttributeMethod()
				{
				}
			}

			private class BaseClass
			{
				[Foo(Value = 2)]
				public virtual void InheritedMethod()
				{
				}
			}

			private class DerivedClass : BaseClass
			{
				public override void InheritedMethod()
				{
				}
			}
		}
	}
}
