using System.Collections.Generic;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class DoNotHave
	{
		public sealed class TypeTests
		{
#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WhenNoItemHasAttribute_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(WithoutFoo), typeof(WithBar),
				}.ToTestAsyncEnumerable<Type?>();

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
				IEnumerable<Type?> subject = [typeof(WithoutFoo), typeof(WithFoo1),];

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).DoNotHave(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatTypes.DoNotHave.TypeTests.FooAttribute,
					             but 1 of 2 did

					             Matching items:
					             [
					               ThatTypes.DoNotHave.TypeTests.WithFoo1
					             ]

					             Collection:
					             [
					               ThatTypes.DoNotHave.TypeTests.WithoutFoo,
					               ThatTypes.DoNotHave.TypeTests.WithFoo1
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

			[Foo(Value = 1)]
			private class WithFoo1
			{
			}

			[Foo(Value = 2)]
			private class WithFoo2
			{
			}

			[Bar]
			private class WithBar
			{
			}

			private class WithoutFoo
			{
			}
		}
	}
}
