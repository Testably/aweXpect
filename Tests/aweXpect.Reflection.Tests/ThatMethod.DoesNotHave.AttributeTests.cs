using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class DoesNotHave
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenMethodDoesNotHaveAttribute_ShouldSucceed()
			{
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.NoAttributeMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodHasAttribute_ShouldFail()
			{
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatMethod.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in void ThatMethod.DoesNotHave.AttributeTests.TestClass.TestMethod()
					             """);
			}

			[Test]
			public async Task WithPredicate_WhenMethodDoesNotHaveAttribute_ShouldSucceed()
			{
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.NoAttributeMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(_ => true);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithPredicate_WhenMethodHasMatchingAttribute_ShouldFail()
			{
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatMethod.DoesNotHave.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but it did in void ThatMethod.DoesNotHave.AttributeTests.TestClass.TestMethod()
					             """);
			}

			[Test]
			public async Task WithPredicate_WhenMethodHasNotMatchingAttribute_ShouldSucceed()
			{
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(foo => foo.Value == 3);
				}

				await That(Act).DoesNotThrow()
					.Because("an attribute that does not match the predicate does not count");
			}

			[Test]
			public async Task WithPredicate_WhenMethodInheritsMatchingAttribute_Directly_ShouldSucceed()
			{
				MethodInfo subject = typeof(DerivedClass).GetMethod(nameof(DerivedClass.InheritedMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(foo => foo.Value == 2).Directly();
				}

				await That(Act).DoesNotThrow()
					.Because("with Directly only the attributes declared on the overriding method count");
			}

			[Test]
			public async Task WithPredicate_WhenMethodInheritsMatchingAttribute_ShouldFail()
			{
				MethodInfo subject = typeof(DerivedClass).GetMethod(nameof(DerivedClass.InheritedMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatMethod.DoesNotHave.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but it did in void ThatMethod.DoesNotHave.AttributeTests.DerivedClass.InheritedMethod()
					             """);
			}

			[Test]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(_ => throw new InvalidOperationException("boom"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatMethod.DoesNotHave.AttributeTests.FooAttribute matching _ => throw new InvalidOperationException("boom"),
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the negated expectation as well instead of passing");
			}

			[Test]
			public async Task WithType_WhenMethodHasAttribute_ShouldFail()
			{
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod))!;

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).DoesNotHave(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatMethod.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in void ThatMethod.DoesNotHave.AttributeTests.TestClass.TestMethod()
					             """);
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
