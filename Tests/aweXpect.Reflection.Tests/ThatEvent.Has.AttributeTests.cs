using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class Has
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task WhenEventDoesNotHaveAttribute_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent("NoAttributeEvent")!;

				async Task Act()
					=> await That(subject).Has<TestAttribute>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has ThatEvent.Has.AttributeTests.TestAttribute,
					             but it did not in System.Action NoAttributeEvent
					             """);
			}

			[Fact]
			public async Task WhenEventHasAttribute_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent("TestEvent")!;

				async Task Act()
					=> await That(subject).Has<TestAttribute>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventHasMatchingAttribute_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent("TestEventWithValue")!;

				async Task Act()
					=> await That(subject).Has<TestAttribute>(attr => attr.Value == 42);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventHasNotMatchingAttribute_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent("TestEventWithValue")!;

				async Task Act()
					=> await That(subject).Has<TestAttribute>(attr => attr.Value == 99);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has ThatEvent.Has.AttributeTests.TestAttribute matching attr => attr.Value == 99,
					             but it did not in System.Action TestEventWithValue
					             """);
			}

			[Fact]
			public async Task WhenEventIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
					=> await That(subject).Has<TestAttribute>();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             has ThatEvent.Has.AttributeTests.TestAttribute,
					             but it was <null>
					             """);
			}

			[AttributeUsage(AttributeTargets.Event)]
			private class TestAttribute : Attribute
			{
				public int Value { get; set; }
			}

#pragma warning disable CS0067 // Event is never used
			private class TestClass
			{
				[Test] public event Action? TestEvent;

				[Test(Value = 42)] public event Action? TestEventWithValue;

				public event Action? NoAttributeEvent;
			}
#pragma warning restore CS0067
		}

		public sealed class OrHas
		{
			public sealed class AttributeTests
			{
				[Fact]
				public async Task WhenEventHasBothAttributes_ShouldSucceed()
				{
					EventInfo subject = typeof(FooBarClass).GetEvent("FooBarEvent")!;

					async Task Act()
						=> await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEventHasFirstAttribute_ShouldSucceed()
				{
					EventInfo subject = typeof(FooClass).GetEvent("FooEvent")!;

					async Task Act()
						=> await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEventHasMatchingAttribute_ShouldSucceed()
				{
					EventInfo subject = typeof(FooClass2).GetEvent("FooEvent2")!;

					async Task Act()
						=> await That(subject).Has<FooAttribute>(foo => foo.Value == 2).OrHas<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEventHasMatchingSecondAttribute_ShouldSucceed()
				{
					EventInfo subject = typeof(BarClass3).GetEvent("BarEvent3")!;

					async Task Act()
						=> await That(subject).Has<FooAttribute>(foo => foo.Value == 5)
							.OrHas<BarAttribute>(bar => bar.Name == "test");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEventHasNeitherAttribute_ShouldFail()
				{
					EventInfo subject = typeof(BazClass).GetEvent("BazEvent")!;

					async Task Act()
						=> await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has ThatEvent.Has.OrHas.AttributeTests.FooAttribute or ThatEvent.Has.OrHas.AttributeTests.BarAttribute,
						             but it did not in System.Action BazEvent
						             """);
				}

				[Fact]
				public async Task WhenEventHasSecondAttribute_ShouldSucceed()
				{
					EventInfo subject = typeof(BarClass).GetEvent("BarEvent")!;

					async Task Act()
						=> await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPredicate_WhenEventHasNotMatchingAttribute_ShouldFail()
				{
					EventInfo subject = typeof(FooClass2).GetEvent("FooEvent2")!;

					async Task Act()
						=> await That(subject).Has<FooAttribute>(foo => foo.Value == 5)
							.OrHas<BarAttribute>(bar => bar.Name == "test");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has ThatEvent.Has.OrHas.AttributeTests.FooAttribute matching foo => foo.Value == 5 or ThatEvent.Has.OrHas.AttributeTests.BarAttribute matching bar => bar.Name == "test",
						             but it did not in System.Action FooEvent2
						             """);
				}

				[AttributeUsage(AttributeTargets.Event)]
				private class FooAttribute : Attribute
				{
					public int Value { get; set; }
				}

				[AttributeUsage(AttributeTargets.Event)]
				private class BarAttribute : Attribute
				{
					public string? Name { get; set; }
				}

				[AttributeUsage(AttributeTargets.Event)]
				private class BazAttribute : Attribute
				{
				}

#pragma warning disable CS0067 // Event is never used
				private class FooClass
				{
					[Foo] public event Action? FooEvent;
				}

				private class FooClass2
				{
					[Foo(Value = 2)] public event Action? FooEvent2;
				}

				private class BarClass
				{
					[Bar] public event Action? BarEvent;
				}

				private class BarClass3
				{
					[Bar(Name = "test")] public event Action? BarEvent3;
				}

				private class FooBarClass
				{
					[Foo] [Bar] public event Action? FooBarEvent;
				}

				private class BazClass
				{
					[Baz] public event Action? BazEvent;
				}
#pragma warning restore CS0067
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventDoesNotHaveAttribute_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent("NoAttributeEvent")!;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Has<TestAttribute>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventDoesNotHaveMatchingAttribute_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent("TestEvent")!;

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.Has<TestAttribute>(attr => attr.Value == "NonExistent"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventHasAttribute_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent("TestEvent")!;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Has<TestAttribute>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatEvent.Has.NegatedTests.TestAttribute,
					             but it did in System.Action TestEvent
					             """);
			}

			[AttributeUsage(AttributeTargets.Event)]
			private class TestAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

#pragma warning disable CS0067 // Event is never used
			private class TestClass
			{
				[Test(Value = "EventValue")] public event Action? TestEvent;

				public event Action? NoAttributeEvent;
			}
#pragma warning restore CS0067
		}
	}
}
