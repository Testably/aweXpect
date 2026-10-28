using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class Have
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenAllEventsHaveAttribute_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = new[]
				{
					typeof(TestClass).GetEvent("TestEvent1")!, typeof(TestClass).GetEvent("TestEvent2")!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllEventsHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = new[]
				{
					typeof(TestClass).GetEvent("TestEvent1")!, typeof(TestClass).GetEvent("TestEvent2")!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>(attr => attr.Value.StartsWith("Event"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllEventsInheritAttributeFromOverriddenEvents_Directly_ShouldFail()
			{
				IEnumerable<EventInfo> subject = new[]
				{
					typeof(DerivedClass).GetEvent(nameof(DerivedClass.InheritedEvent))!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>().Directly();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have direct ThatEvents.Have.AttributeTests.MarkerAttribute,
					             but it contained not matching events

					             Not matching items:
					             [
					               event Action ThatEvents.Have.AttributeTests.DerivedClass.InheritedEvent
					             ]
					             """)
					.Because("with Directly only the attributes declared on the overriding event count");
			}

			[Test]
			public async Task WhenAllEventsInheritAttributeFromOverriddenEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = new[]
				{
					typeof(DerivedClass).GetEvent(nameof(DerivedClass.InheritedEvent))!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>();
				}

				await That(Act).DoesNotThrow()
					.Because("the attribute on the overridden base event is inherited, as in the Events().With<TAttribute>() filter");
			}

			[Test]
			public async Task WhenEventsAreFilteredByInheritedAttribute_ShouldSucceed()
			{
				async Task Act()
				{
					await That(In.Type<DerivedClass>().Events().With<MarkerAttribute>())
						.IsNotEmpty().And.Have<MarkerAttribute>();
				}

				await That(Act).DoesNotThrow()
					.Because("the expectation must agree with the filter on the inherited attribute");
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<EventInfo?> subject = [typeof(TestClass).GetEvent("TestEvent1"), null,];

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.Have<MarkerAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatEvents.Have.AttributeTests.MarkerAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               event Action ThatEvents.Have.AttributeTests.TestClass.TestEvent1,
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all have ThatEvents.Have.AttributeTests.MarkerAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               event Action ThatEvents.Have.AttributeTests.TestClass.TestEvent1,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNotAllEventsHaveAttribute_ShouldFail()
			{
				IEnumerable<EventInfo> subject = new[]
				{
					typeof(TestClass).GetEvent("TestEvent1")!, typeof(TestClass).GetEvent("NoAttributeEvent")!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatEvents.Have.AttributeTests.MarkerAttribute,
					             but it contained not matching events

					             Not matching items:
					             [
					               event Action ThatEvents.Have.AttributeTests.TestClass.NoAttributeEvent
					             ]
					             """);
			}

			[Test]
			public async Task WhenNotAllEventsHaveMatchingAttribute_ShouldFail()
			{
				IEnumerable<EventInfo> subject = new[]
				{
					typeof(TestClass).GetEvent("TestEvent1")!, typeof(TestClass).GetEvent("TestEvent2")!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>(attr => attr.Value == "WrongValue");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatEvents.Have.AttributeTests.MarkerAttribute matching attr => attr.Value == "WrongValue",
					             but it contained not matching events

					             Not matching items:
					             [
					               event Action ThatEvents.Have.AttributeTests.TestClass.TestEvent1,
					               event Action ThatEvents.Have.AttributeTests.TestClass.TestEvent2
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			public sealed class AsyncEnumerableTests
			{
				[Test]
				public async Task WhenAllEventsHaveAttribute_ShouldSucceed()
				{
					IAsyncEnumerable<EventInfo?> subject = new[]
					{
						typeof(TestClass).GetEvent("TestEvent1")!, typeof(TestClass).GetEvent("TestEvent2")!,
					}.ToTestAsyncEnumerable<EventInfo?>();

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAllEventsHaveMatchingAttribute_ShouldSucceed()
				{
					IAsyncEnumerable<EventInfo?> subject = new[]
					{
						typeof(TestClass).GetEvent("TestEvent1")!, typeof(TestClass).GetEvent("TestEvent2")!,
					}.ToTestAsyncEnumerable<EventInfo?>();

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value.StartsWith("Event"));
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenNotAllEventsHaveAttribute_ShouldFail()
				{
					IAsyncEnumerable<EventInfo?> subject = new[]
					{
						typeof(TestClass).GetEvent("TestEvent1")!, typeof(TestClass).GetEvent("NoAttributeEvent")!,
					}.ToTestAsyncEnumerable<EventInfo?>();

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatEvents.Have.AttributeTests.MarkerAttribute,
						             but it contained not matching events

						             Not matching items:
						             [
						               event Action ThatEvents.Have.AttributeTests.TestClass.NoAttributeEvent
						             ]
						             """);
				}

				[Test]
				public async Task WhenNotAllEventsHaveMatchingAttribute_ShouldFail()
				{
					IAsyncEnumerable<EventInfo?> subject = new[]
					{
						typeof(TestClass).GetEvent("TestEvent1")!, typeof(TestClass).GetEvent("TestEvent2")!,
					}.ToTestAsyncEnumerable<EventInfo?>();

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value == "WrongValue");
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatEvents.Have.AttributeTests.MarkerAttribute matching attr => attr.Value == "WrongValue",
						             but it contained not matching events

						             Not matching items:
						             [
						               event Action ThatEvents.Have.AttributeTests.TestClass.TestEvent1,
						               event Action ThatEvents.Have.AttributeTests.TestClass.TestEvent2
						             ]
						             """);
				}
			}
#endif

			[AttributeUsage(AttributeTargets.Event)]
			private class MarkerAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

#pragma warning disable CS0067 // Event is never used
			private class TestClass
			{
				[Marker(Value = "Event1Value")] public event Action? TestEvent1;

				[Marker(Value = "Event2Value")] public event Action? TestEvent2;

				public event Action? NoAttributeEvent;
			}

			private class BaseClass
			{
				[Marker(Value = "BaseValue")] public virtual event Action? InheritedEvent;
			}

			private class DerivedClass : BaseClass
			{
				public override event Action? InheritedEvent;
			}
#pragma warning restore CS0067
		}

		public sealed class OrHave
		{
			public sealed class AttributeTests
			{
				[Test]
				public async Task WhenEventsHaveBothAttributes_ShouldSucceed()
				{
					IEnumerable<EventInfo> subject = new[]
					{
						typeof(TestClass).GetEvent("BothEvent")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEventsHaveFirstAttribute_ShouldSucceed()
				{
					IEnumerable<EventInfo> subject = new[]
					{
						typeof(TestClass).GetEvent("TestEvent1")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEventsHaveMatchingFirstAttribute_ShouldSucceed()
				{
					IEnumerable<EventInfo> subject = new[]
					{
						typeof(TestClass).GetEvent("TestEvent1")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value == "Event1Value")
							.OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEventsHaveMatchingSecondAttribute_ShouldSucceed()
				{
					IEnumerable<EventInfo> subject = new[]
					{
						typeof(TestClass).GetEvent("BarEvent")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value == "NonExistent")
							.OrHave<BarAttribute>(attr => attr.Name == "bar");
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEventsHaveNeitherAttribute_ShouldFail()
				{
					IEnumerable<EventInfo> subject = new[]
					{
						typeof(TestClass).GetEvent("NoAttributeEvent")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatEvents.Have.OrHave.AttributeTests.MarkerAttribute or ThatEvents.Have.OrHave.AttributeTests.BarAttribute,
						             but it contained not matching events

						             Not matching items:
						             [
						               event Action ThatEvents.Have.OrHave.AttributeTests.TestClass.NoAttributeEvent
						             ]
						             """);
				}

				[Test]
				public async Task WhenEventsHaveSecondAttribute_ShouldSucceed()
				{
					IEnumerable<EventInfo> subject = new[]
					{
						typeof(TestClass).GetEvent("BarEvent")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WithPredicate_WhenEventsHaveNotMatchingAttribute_ShouldFail()
				{
					IEnumerable<EventInfo> subject = new[]
					{
						typeof(TestClass).GetEvent("TestEvent1")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value == "WrongValue")
							.OrHave<BarAttribute>(attr => attr.Name == "wrong");
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatEvents.Have.OrHave.AttributeTests.MarkerAttribute matching attr => attr.Value == "WrongValue" or ThatEvents.Have.OrHave.AttributeTests.BarAttribute matching attr => attr.Name == "wrong",
						             but it contained not matching events

						             Not matching items:
						             [
						               event Action ThatEvents.Have.OrHave.AttributeTests.TestClass.TestEvent1
						             ]
						             """);
				}

				[AttributeUsage(AttributeTargets.Event)]
				private class MarkerAttribute : Attribute
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
					[Marker(Value = "Event1Value")] public event Action? TestEvent1;

					[Bar(Name = "bar")] public event Action? BarEvent;

					[Marker(Value = "BothValue")]
					[Bar(Name = "both")]
					public event Action? BothEvent;

					public event Action? NoAttributeEvent;
				}
#pragma warning restore CS0067
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEventsDoNotHaveAttribute_ShouldSucceed()
			{
				IEnumerable<EventInfo> subjects = new[]
				{
					typeof(TestClass).GetEvent("NoAttributeEvent")!,
				};

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they => they.Have<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventsDoNotHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<EventInfo> subjects = new[]
				{
					typeof(TestClass).GetEvent("TestEvent1")!,
				};

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they
						=> they.Have<MarkerAttribute>(attr => attr.Value == "NonExistent"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventsHaveAttribute_ShouldFail()
			{
				IEnumerable<EventInfo> subjects = new[]
				{
					typeof(TestClass).GetEvent("TestEvent1")!,
				};

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they
						=> they.Have<MarkerAttribute>().OrHave<MarkerAttribute>(x => x.Value == "foo"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subjects
					             not all have ThatEvents.Have.NegatedTests.MarkerAttribute or ThatEvents.Have.NegatedTests.MarkerAttribute matching x => x.Value == "foo",
					             but it only contained matching events

					             Collection:
					             [
					               event Action ThatEvents.Have.NegatedTests.TestClass.TestEvent1
					             ]
					             """);
			}

			[AttributeUsage(AttributeTargets.Event)]
			private class MarkerAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

#pragma warning disable CS0067 // Event is never used
			private class TestClass
			{
				[Marker(Value = "Event1Value")] public event Action? TestEvent1;

				public event Action? NoAttributeEvent;
			}
#pragma warning restore CS0067
		}
	}
}
