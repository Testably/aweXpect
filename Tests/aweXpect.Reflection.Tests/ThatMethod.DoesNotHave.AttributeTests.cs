using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class DoesNotHave
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task WhenMethodDoesNotHaveAttribute_ShouldSucceed()
			{
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.NoAttributeMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodHasAttribute_ShouldFail()
			{
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).Throws<XunitException>()
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
