using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed partial class DoNotHave
	{
		public sealed class TypeTests
		{
#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WhenNoItemHasAttribute_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.WithoutFoo)), typeof(TestClass).GetMethod(nameof(TestClass.WithBar)),
				}.ToTestAsyncEnumerable<MethodInfo?>();

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).DoNotHave(attributeType);
				}

				await That(Act).DoesNotThrow();
			}

#endif
			[Test]
			public async Task WhenAnItemHasAttribute_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject = [typeof(TestClass).GetMethod(nameof(TestClass.WithoutFoo)), typeof(TestClass).GetMethod(nameof(TestClass.WithFoo1)),];

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).DoNotHave(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatMethods.DoNotHave.TypeTests.FooAttribute,
					             but 1 of 2 did

					             Matching items:
					             [
					               void ThatMethods.DoNotHave.TypeTests.TestClass.WithFoo1()
					             ]

					             Collection:
					             [
					               void ThatMethods.DoNotHave.TypeTests.TestClass.WithoutFoo(),
					               void ThatMethods.DoNotHave.TypeTests.TestClass.WithFoo1()
					             ]
					             """);
			}

			[AttributeUsage(AttributeTargets.All)]
			private sealed class FooAttribute : Attribute
			{
				public int Value { get; set; }
			}

			[AttributeUsage(AttributeTargets.All)]
			private sealed class BarAttribute : Attribute
			{
			}

			private class TestClass
			{
				[Foo(Value = 1)]
				public void WithFoo1() { }

				[Foo(Value = 2)]
				public void WithFoo2() { }

				[Bar]
				public void WithBar() { }

				public void WithoutFoo() { }
			}
		}
	}
}
