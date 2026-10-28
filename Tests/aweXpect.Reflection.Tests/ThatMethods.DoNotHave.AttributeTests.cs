using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class DoNotHave
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
					             all have no ThatMethods.DoNotHave.AttributeTests.FooAttribute,
					             but it contained not matching methods

					             Not matching items:
					             [
					               void ThatMethods.DoNotHave.AttributeTests.TestClass.TestMethod()
					             ]
					             """);
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
					             not all have no ThatMethods.DoNotHave.AttributeTests.FooAttribute,
					             but it only contained matching methods

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
					             all have no ThatMethods.DoNotHave.AttributeTests.FooAttribute,
					             but it contained not matching methods

					             Not matching items:
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
					             all have no ThatMethods.DoNotHave.AttributeTests.FooAttribute,
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
					             not all have no ThatMethods.DoNotHave.AttributeTests.FooAttribute,
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
