using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class Have
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task WhenAllEventsHaveAttribute_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = new[]
				{
					typeof(TestClass).GetEvent("TestEvent1")!, typeof(TestClass).GetEvent("TestEvent2")!,
				};

				async Task Act()
					=> await That(subject).Have<TestAttribute>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllEventsHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = new[]
				{
					typeof(TestClass).GetEvent("TestEvent1")!, typeof(TestClass).GetEvent("TestEvent2")!,
				};

				async Task Act()
					=> await That(subject).Have<TestAttribute>(attr => attr.Value.StartsWith("Event"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllEventsHaveAttribute_ShouldFail()
			{
				IEnumerable<EventInfo> subject = new[]
				{
					typeof(TestClass).GetEvent("TestEvent1")!, typeof(TestClass).GetEvent("NoAttributeEvent")!,
				};

				async Task Act()
					=> await That(subject).Have<TestAttribute>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatEvents.Have.AttributeTests.TestAttribute,
					             but it contained not matching events [
					               System.Action NoAttributeEvent
					             ]
					             """);
			}

			[Fact]
			public async Task WhenNotAllEventsHaveMatchingAttribute_ShouldFail()
			{
				IEnumerable<EventInfo> subject = new[]
				{
					typeof(TestClass).GetEvent("TestEvent1")!, typeof(TestClass).GetEvent("TestEvent2")!,
				};

				async Task Act()
					=> await That(subject).Have<TestAttribute>(attr => attr.Value == "WrongValue");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatEvents.Have.AttributeTests.TestAttribute matching attr => attr.Value == "WrongValue",
					             but it contained not matching events [
					               System.Action TestEvent1,
					               System.Action TestEvent2
					             ]
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
				[Test(Value = "Event1Value")] public event Action? TestEvent1;

				[Test(Value = "Event2Value")] public event Action? TestEvent2;

				public event Action? NoAttributeEvent;
			}
#pragma warning restore CS0067
		}

		public sealed class OrHave
		{
			public sealed class AttributeTests
			{
				[Fact]
				public async Task WhenEventsHaveBothAttributes_ShouldSucceed()
				{
					IEnumerable<EventInfo> subject = new[]
					{
						typeof(TestClass).GetEvent("BothEvent")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEventsHaveFirstAttribute_ShouldSucceed()
				{
					IEnumerable<EventInfo> subject = new[]
					{
						typeof(TestClass).GetEvent("TestEvent1")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEventsHaveMatchingFirstAttribute_ShouldSucceed()
				{
					IEnumerable<EventInfo> subject = new[]
					{
						typeof(TestClass).GetEvent("TestEvent1")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>(attr => attr.Value == "Event1Value")
							.OrHave<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEventsHaveMatchingSecondAttribute_ShouldSucceed()
				{
					IEnumerable<EventInfo> subject = new[]
					{
						typeof(TestClass).GetEvent("BarEvent")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>(attr => attr.Value == "NonExistent")
							.OrHave<BarAttribute>(attr => attr.Name == "bar");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEventsHaveNeitherAttribute_ShouldFail()
				{
					IEnumerable<EventInfo> subject = new[]
					{
						typeof(TestClass).GetEvent("NoAttributeEvent")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatEvents.Have.OrHave.AttributeTests.TestAttribute or ThatEvents.Have.OrHave.AttributeTests.BarAttribute,
						             but it contained not matching events [
						               System.Action NoAttributeEvent
						             ]
						             """);
				}

				[Fact]
				public async Task WhenEventsHaveSecondAttribute_ShouldSucceed()
				{
					IEnumerable<EventInfo> subject = new[]
					{
						typeof(TestClass).GetEvent("BarEvent")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPredicate_WhenEventsHaveNotMatchingAttribute_ShouldFail()
				{
					IEnumerable<EventInfo> subject = new[]
					{
						typeof(TestClass).GetEvent("TestEvent1")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>(attr => attr.Value == "WrongValue")
							.OrHave<BarAttribute>(attr => attr.Name == "wrong");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatEvents.Have.OrHave.AttributeTests.TestAttribute matching attr => attr.Value == "WrongValue" or ThatEvents.Have.OrHave.AttributeTests.BarAttribute matching attr => attr.Name == "wrong",
						             but it contained not matching events [
						               System.Action TestEvent1
						             ]
						             """);
				}

				[AttributeUsage(AttributeTargets.Event)]
				private class TestAttribute : Attribute
				{
					public string Value { get; set; } = "";
				}

				[AttributeUsage(AttributeTargets.Event)]
				private class BarAttribute : Attribute
				{
					public string Name { get; set; } = "";
				}

#pragma warning disable CS0067 // Event is never used
				private class TestClass
				{
					[Test(Value = "Event1Value")] public event Action? TestEvent1;

					[Bar(Name = "bar")] public event Action? BarEvent;

					[Test(Value = "BothValue")]
					[Bar(Name = "both")]
					public event Action? BothEvent;

					public event Action? NoAttributeEvent;
				}
#pragma warning restore CS0067
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventsDoNotHaveAttribute_ShouldSucceed()
			{
				IEnumerable<EventInfo> subjects = new[]
				{
					typeof(TestClass).GetEvent("NoAttributeEvent")!,
				};

				async Task Act()
					=> await That(subjects).DoesNotComplyWith(they => they.Have<TestAttribute>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventsDoNotHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<EventInfo> subjects = new[]
				{
					typeof(TestClass).GetEvent("TestEvent1")!,
				};

				async Task Act()
					=> await That(subjects).DoesNotComplyWith(they
						=> they.Have<TestAttribute>(attr => attr.Value == "NonExistent"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventsHaveAttribute_ShouldFail()
			{
				IEnumerable<EventInfo> subjects = new[]
				{
					typeof(TestClass).GetEvent("TestEvent1")!,
				};

				async Task Act()
					=> await That(subjects).DoesNotComplyWith(they
						=> they.Have<TestAttribute>().OrHave<TestAttribute>(x => x.Value == "foo"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subjects
					             not all have ThatEvents.Have.NegatedTests.TestAttribute or ThatEvents.Have.NegatedTests.TestAttribute matching x => x.Value == "foo",
					             but it only contained matching events [
					               System.Action TestEvent1
					             ]
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
				[Test(Value = "Event1Value")] public event Action? TestEvent1;

				public event Action? NoAttributeEvent;
			}
#pragma warning restore CS0067
		}
	}
}
