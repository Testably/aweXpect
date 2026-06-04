using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class DoNotHave
	{
		public sealed class AttributeTests
		{
#if NET8_0_OR_GREATER
			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have no ThatMethods.DoNotHave.AttributeTests.FooAttribute,
					             but it contained not matching methods [
					               void ThatMethods.DoNotHave.AttributeTests.TestClass.TestMethod()
					             ]
					             """);
			}
#endif

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             not all have no ThatMethods.DoNotHave.AttributeTests.FooAttribute,
					             but it only contained matching methods [
					               void ThatMethods.DoNotHave.AttributeTests.TestClass.NoAttributeMethod()
					             ]
					             """);
			}

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have no ThatMethods.DoNotHave.AttributeTests.FooAttribute,
					             but it contained not matching methods [
					               void ThatMethods.DoNotHave.AttributeTests.TestClass.TestMethod()
					             ]
					             """);
			}

			[Fact]
			public async Task WhenNoMethodHasAttribute_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.NoAttributeMethod)), null,
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
