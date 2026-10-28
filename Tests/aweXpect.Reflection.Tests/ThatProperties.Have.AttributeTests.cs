using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class Have
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenAllPropertiesHaveAttribute_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = new[]
				{
					typeof(TestClass).GetProperty("TestProperty1")!, typeof(TestClass).GetProperty("TestProperty2")!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllPropertiesHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = new[]
				{
					typeof(TestClass).GetProperty("TestProperty1")!, typeof(TestClass).GetProperty("TestProperty2")!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>(attr => attr.Value.StartsWith("Property"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllPropertiesInheritAttributeFromOverriddenProperties_Directly_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = new[]
				{
					typeof(DerivedClass).GetProperty(nameof(DerivedClass.InheritedProperty))!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>().Directly();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have direct ThatProperties.Have.AttributeTests.MarkerAttribute,
					             but it contained not matching properties

					             Not matching items:
					             [
					               public string ThatProperties.Have.AttributeTests.DerivedClass.InheritedProperty { get; set; }
					             ]
					             """)
					.Because("with Directly only the attributes declared on the overriding property count");
			}

			[Test]
			public async Task WhenAllPropertiesInheritAttributeFromOverriddenProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = new[]
				{
					typeof(DerivedClass).GetProperty(nameof(DerivedClass.InheritedProperty))!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>();
				}

				await That(Act).DoesNotThrow()
					.Because("the attribute on the overridden base property is inherited, as in the Properties().With<TAttribute>() filter");
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<PropertyInfo?> subject = [typeof(TestClass).GetProperty("TestProperty1"), null,];

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
					             all have ThatProperties.Have.AttributeTests.MarkerAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               public string ThatProperties.Have.AttributeTests.TestClass.TestProperty1 { get; set; },
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all have ThatProperties.Have.AttributeTests.MarkerAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               public string ThatProperties.Have.AttributeTests.TestClass.TestProperty1 { get; set; },
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNotAllPropertiesHaveAttribute_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = new[]
				{
					typeof(TestClass).GetProperty("TestProperty1")!, typeof(TestClass).GetProperty("NoAttributeProperty")!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatProperties.Have.AttributeTests.MarkerAttribute,
					             but it contained not matching properties

					             Not matching items:
					             [
					               public string ThatProperties.Have.AttributeTests.TestClass.NoAttributeProperty { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task WhenNotAllPropertiesHaveMatchingAttribute_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = new[]
				{
					typeof(TestClass).GetProperty("TestProperty1")!, typeof(TestClass).GetProperty("TestProperty2")!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>(attr => attr.Value == "WrongValue");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatProperties.Have.AttributeTests.MarkerAttribute matching attr => attr.Value == "WrongValue",
					             but it contained not matching properties

					             Not matching items:
					             [
					               public string ThatProperties.Have.AttributeTests.TestClass.TestProperty1 { get; set; },
					               public string ThatProperties.Have.AttributeTests.TestClass.TestProperty2 { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task WhenPropertiesAreFilteredByInheritedAttribute_ShouldSucceed()
			{
				async Task Act()
				{
					await That(In.Type<DerivedClass>().Properties().With<MarkerAttribute>())
						.IsNotEmpty().And.Have<MarkerAttribute>();
				}

				await That(Act).DoesNotThrow()
					.Because("the expectation must agree with the filter on the inherited attribute");
			}

#if NET8_0_OR_GREATER
			public sealed class AsyncEnumerableTests
			{
				[Test]
				public async Task WhenAllPropertiesHaveAttribute_ShouldSucceed()
				{
					IAsyncEnumerable<PropertyInfo?> subject = new[]
					{
						typeof(TestClass).GetProperty("TestProperty1")!, typeof(TestClass).GetProperty("TestProperty2")!,
					}.ToTestAsyncEnumerable<PropertyInfo?>();

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAllPropertiesHaveMatchingAttribute_ShouldSucceed()
				{
					IAsyncEnumerable<PropertyInfo?> subject = new[]
					{
						typeof(TestClass).GetProperty("TestProperty1")!, typeof(TestClass).GetProperty("TestProperty2")!,
					}.ToTestAsyncEnumerable<PropertyInfo?>();

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value.StartsWith("Property"));
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenNotAllPropertiesHaveAttribute_ShouldFail()
				{
					IAsyncEnumerable<PropertyInfo?> subject = new[]
					{
						typeof(TestClass).GetProperty("TestProperty1")!, typeof(TestClass).GetProperty("NoAttributeProperty")!,
					}.ToTestAsyncEnumerable<PropertyInfo?>();

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatProperties.Have.AttributeTests.MarkerAttribute,
						             but it contained not matching properties

						             Not matching items:
						             [
						               public string ThatProperties.Have.AttributeTests.TestClass.NoAttributeProperty { get; set; }
						             ]
						             """);
				}

				[Test]
				public async Task WhenNotAllPropertiesHaveMatchingAttribute_ShouldFail()
				{
					IAsyncEnumerable<PropertyInfo?> subject = new[]
					{
						typeof(TestClass).GetProperty("TestProperty1")!, typeof(TestClass).GetProperty("TestProperty2")!,
					}.ToTestAsyncEnumerable<PropertyInfo?>();

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value == "WrongValue");
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatProperties.Have.AttributeTests.MarkerAttribute matching attr => attr.Value == "WrongValue",
						             but it contained not matching properties

						             Not matching items:
						             [
						               public string ThatProperties.Have.AttributeTests.TestClass.TestProperty1 { get; set; },
						               public string ThatProperties.Have.AttributeTests.TestClass.TestProperty2 { get; set; }
						             ]
						             """);
				}
			}
#endif

			[AttributeUsage(AttributeTargets.Property)]
			private class MarkerAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				[Marker(Value = "Property1Value")] public string TestProperty1 { get; set; } = "";

				[Marker(Value = "Property2Value")] public string TestProperty2 { get; set; } = "";

				public string NoAttributeProperty { get; set; } = "";
			}

			private class BaseClass
			{
				[Marker(Value = "BaseValue")] public virtual string InheritedProperty { get; set; } = "";
			}

			private class DerivedClass : BaseClass
			{
				public override string InheritedProperty { get; set; } = "";
			}
			// ReSharper restore UnusedMember.Local
		}

		public sealed class OrHave
		{
			public sealed class AttributeTests
			{
				[Test]
				public async Task WhenPropertiesHaveBothAttributes_ShouldSucceed()
				{
					IEnumerable<PropertyInfo> subject = new[]
					{
						typeof(TestClass).GetProperty("BothProperty")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenPropertiesHaveFirstAttribute_ShouldSucceed()
				{
					IEnumerable<PropertyInfo> subject = new[]
					{
						typeof(TestClass).GetProperty("TestProperty1")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenPropertiesHaveMatchingFirstAttribute_ShouldSucceed()
				{
					IEnumerable<PropertyInfo> subject = new[]
					{
						typeof(TestClass).GetProperty("TestProperty1")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value == "Property1Value")
							.OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenPropertiesHaveMatchingSecondAttribute_ShouldSucceed()
				{
					IEnumerable<PropertyInfo> subject = new[]
					{
						typeof(TestClass).GetProperty("BarProperty")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value == "NonExistent")
							.OrHave<BarAttribute>(attr => attr.Name == "bar");
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenPropertiesHaveNeitherAttribute_ShouldFail()
				{
					IEnumerable<PropertyInfo> subject = new[]
					{
						typeof(TestClass).GetProperty("NoAttributeProperty")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatProperties.Have.OrHave.AttributeTests.MarkerAttribute or ThatProperties.Have.OrHave.AttributeTests.BarAttribute,
						             but it contained not matching properties

						             Not matching items:
						             [
						               public string ThatProperties.Have.OrHave.AttributeTests.TestClass.NoAttributeProperty { get; set; }
						             ]
						             """);
				}

				[Test]
				public async Task WhenPropertiesHaveSecondAttribute_ShouldSucceed()
				{
					IEnumerable<PropertyInfo> subject = new[]
					{
						typeof(TestClass).GetProperty("BarProperty")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WithPredicate_WhenPropertiesHaveNotMatchingAttribute_ShouldFail()
				{
					IEnumerable<PropertyInfo> subject = new[]
					{
						typeof(TestClass).GetProperty("TestProperty1")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value == "WrongValue")
							.OrHave<BarAttribute>(attr => attr.Name == "wrong");
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatProperties.Have.OrHave.AttributeTests.MarkerAttribute matching attr => attr.Value == "WrongValue" or ThatProperties.Have.OrHave.AttributeTests.BarAttribute matching attr => attr.Name == "wrong",
						             but it contained not matching properties

						             Not matching items:
						             [
						               public string ThatProperties.Have.OrHave.AttributeTests.TestClass.TestProperty1 { get; set; }
						             ]
						             """);
				}

				[AttributeUsage(AttributeTargets.Property)]
				private class MarkerAttribute : Attribute
				{
					public string Value { get; set; } = "";
				}

				[AttributeUsage(AttributeTargets.Property)]
				private class BarAttribute : Attribute
				{
					public string Name { get; set; } = "";
				}

				// ReSharper disable UnusedMember.Local
				private class TestClass
				{
					[Marker(Value = "Property1Value")] public string TestProperty1 { get; set; } = "";

					[Bar(Name = "bar")] public string BarProperty { get; set; } = "";

					[Marker(Value = "BothValue")]
					[Bar(Name = "both")]
					public string BothProperty { get; set; } = "";

					public string NoAttributeProperty { get; set; } = "";
				}
				// ReSharper restore UnusedMember.Local
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertiesDoNotHaveAttribute_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subjects = new[]
				{
					typeof(TestClass).GetProperty("NoAttributeProperty")!,
				};

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they => they.Have<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesDoNotHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subjects = new[]
				{
					typeof(TestClass).GetProperty("TestProperty1")!,
				};

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they
						=> they.Have<MarkerAttribute>(attr => attr.Value == "NonExistent"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesHaveAttribute_ShouldFail()
			{
				IEnumerable<PropertyInfo> subjects = new[]
				{
					typeof(TestClass).GetProperty("TestProperty1")!,
				};

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they
						=> they.Have<MarkerAttribute>().OrHave<MarkerAttribute>(x => x.Value == "foo"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subjects
					             not all have ThatProperties.Have.NegatedTests.MarkerAttribute or ThatProperties.Have.NegatedTests.MarkerAttribute matching x => x.Value == "foo",
					             but it only contained matching properties

					             Collection:
					             [
					               public string ThatProperties.Have.NegatedTests.TestClass.TestProperty1 { get; set; }
					             ]
					             """);
			}

			[AttributeUsage(AttributeTargets.Property)]
			private class MarkerAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				[Marker(Value = "Property1Value")] public string TestProperty1 { get; set; } = "";

				public string NoAttributeProperty { get; set; } = "";
			}
			// ReSharper restore UnusedMember.Local
		}
	}
}
