using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed partial class DoNotHave
	{
		public sealed class TypeTests
		{
#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WhenNoItemHasAttribute_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.WithoutFoo)), typeof(TestClass).GetProperty(nameof(TestClass.WithBar)),
				}.ToTestAsyncEnumerable<PropertyInfo?>();

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
				IEnumerable<PropertyInfo?> subject = [typeof(TestClass).GetProperty(nameof(TestClass.WithoutFoo)), typeof(TestClass).GetProperty(nameof(TestClass.WithFoo1)),];

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).DoNotHave(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatProperties.DoNotHave.TypeTests.FooAttribute,
					             but 1 of 2 did

					             Matching items:
					             [
					               public int ThatProperties.DoNotHave.TypeTests.TestClass.WithFoo1 { get; set; }
					             ]

					             Collection:
					             [
					               public int ThatProperties.DoNotHave.TypeTests.TestClass.WithoutFoo { get; set; },
					               public int ThatProperties.DoNotHave.TypeTests.TestClass.WithFoo1 { get; set; }
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
				public int WithFoo1 { get; set; }

				[Foo(Value = 2)]
				public int WithFoo2 { get; set; }

				[Bar]
				public int WithBar { get; set; }

				public int WithoutFoo { get; set; }
			}
		}
	}
}
