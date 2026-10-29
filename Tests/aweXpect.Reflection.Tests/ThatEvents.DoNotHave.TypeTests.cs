using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed partial class DoNotHave
	{
		public sealed class TypeTests
		{
#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WhenNoItemHasAttribute_ShouldSucceed()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.WithoutFoo)), typeof(TestClass).GetEvent(nameof(TestClass.WithBar)),
				}.ToTestAsyncEnumerable<EventInfo?>();

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
				IEnumerable<EventInfo?> subject = [typeof(TestClass).GetEvent(nameof(TestClass.WithoutFoo)), typeof(TestClass).GetEvent(nameof(TestClass.WithFoo1)),];

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).DoNotHave(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatEvents.DoNotHave.TypeTests.FooAttribute,
					             but 1 of 2 did

					             Matching items:
					             [
					               event EventHandler ThatEvents.DoNotHave.TypeTests.TestClass.WithFoo1
					             ]

					             Collection:
					             [
					               event EventHandler ThatEvents.DoNotHave.TypeTests.TestClass.WithoutFoo,
					               event EventHandler ThatEvents.DoNotHave.TypeTests.TestClass.WithFoo1
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

#pragma warning disable CS0067
			private class TestClass
			{
				[Foo(Value = 1)]
				public event EventHandler? WithFoo1;

				[Foo(Value = 2)]
				public event EventHandler? WithFoo2;

				[Bar]
				public event EventHandler? WithBar;

				public event EventHandler? WithoutFoo;
			}
#pragma warning restore CS0067
		}
	}
}
