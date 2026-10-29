using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed partial class DoNotHave
	{
		public sealed class TypeTests
		{
#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WhenNoItemHasAttribute_ShouldSucceed()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.WithoutFoo)), typeof(TestClass).GetField(nameof(TestClass.WithBar)),
				}.ToTestAsyncEnumerable<FieldInfo?>();

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
				IEnumerable<FieldInfo?> subject = [typeof(TestClass).GetField(nameof(TestClass.WithoutFoo)), typeof(TestClass).GetField(nameof(TestClass.WithFoo1)),];

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).DoNotHave(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatFields.DoNotHave.TypeTests.FooAttribute,
					             but 1 of 2 did

					             Matching items:
					             [
					               int ThatFields.DoNotHave.TypeTests.TestClass.WithFoo1
					             ]

					             Collection:
					             [
					               int ThatFields.DoNotHave.TypeTests.TestClass.WithoutFoo,
					               int ThatFields.DoNotHave.TypeTests.TestClass.WithFoo1
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

#pragma warning disable CS0649
			private class TestClass
			{
				[Foo(Value = 1)]
				public int WithFoo1;

				[Foo(Value = 2)]
				public int WithFoo2;

				[Bar]
				public int WithBar;

				public int WithoutFoo;
			}
#pragma warning restore CS0649
		}
	}
}
