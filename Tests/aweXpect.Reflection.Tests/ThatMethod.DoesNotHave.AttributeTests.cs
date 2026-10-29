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
			}

			private class TestClass
			{
				[Foo]
				public static void TestMethod()
				{
				}

				public static void NoAttributeMethod()
				{
				}
			}
		}
	}
}
