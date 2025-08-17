using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class Have
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task WhenAllPropertiesHaveAttribute_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = new[]
				{
					typeof(TestClass).GetProperty("TestProperty1")!, typeof(TestClass).GetProperty("TestProperty2")!,
				};

				async Task Act()
					=> await That(subject).Have<TestAttribute>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllPropertiesHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = new[]
				{
					typeof(TestClass).GetProperty("TestProperty1")!, typeof(TestClass).GetProperty("TestProperty2")!,
				};

				async Task Act()
					=> await That(subject).Have<TestAttribute>(attr => attr.Value.StartsWith("Property"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllPropertiesHaveAttribute_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = new[]
				{
					typeof(TestClass).GetProperty("TestProperty1")!,
					typeof(TestClass).GetProperty("NoAttributeProperty")!,
				};

				async Task Act()
					=> await That(subject).Have<TestAttribute>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatProperties.Have.AttributeTests.TestAttribute,
					             but it contained not matching properties [
					               System.String NoAttributeProperty
					             ]
					             """);
			}

			[Fact]
			public async Task WhenNotAllPropertiesHaveMatchingAttribute_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = new[]
				{
					typeof(TestClass).GetProperty("TestProperty1")!, typeof(TestClass).GetProperty("TestProperty2")!,
				};

				async Task Act()
					=> await That(subject).Have<TestAttribute>(attr => attr.Value == "WrongValue");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatProperties.Have.AttributeTests.TestAttribute matching attr => attr.Value == "WrongValue",
					             but it contained not matching properties [
					               System.String TestProperty1,
					               System.String TestProperty2
					             ]
					             """);
			}

			[AttributeUsage(AttributeTargets.Property)]
			private class TestAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				[Test(Value = "Property1Value")] public string TestProperty1 { get; set; } = "";

				[Test(Value = "Property2Value")] public string TestProperty2 { get; set; } = "";

				public string NoAttributeProperty { get; set; } = "";
			}
			// ReSharper restore UnusedMember.Local
		}

		public sealed class OrHave
		{
			public sealed class AttributeTests
			{
				[Fact]
				public async Task WhenPropertiesHaveBothAttributes_ShouldSucceed()
				{
					IEnumerable<PropertyInfo> subject = new[]
					{
						typeof(TestClass).GetProperty("BothProperty")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenPropertiesHaveFirstAttribute_ShouldSucceed()
				{
					IEnumerable<PropertyInfo> subject = new[]
					{
						typeof(TestClass).GetProperty("TestProperty1")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenPropertiesHaveMatchingFirstAttribute_ShouldSucceed()
				{
					IEnumerable<PropertyInfo> subject = new[]
					{
						typeof(TestClass).GetProperty("TestProperty1")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>(attr => attr.Value == "Property1Value")
							.OrHave<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenPropertiesHaveMatchingSecondAttribute_ShouldSucceed()
				{
					IEnumerable<PropertyInfo> subject = new[]
					{
						typeof(TestClass).GetProperty("BarProperty")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>(attr => attr.Value == "NonExistent")
							.OrHave<BarAttribute>(attr => attr.Name == "bar");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenPropertiesHaveNeitherAttribute_ShouldFail()
				{
					IEnumerable<PropertyInfo> subject = new[]
					{
						typeof(TestClass).GetProperty("NoAttributeProperty")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatProperties.Have.OrHave.AttributeTests.TestAttribute or ThatProperties.Have.OrHave.AttributeTests.BarAttribute,
						             but it contained not matching properties [
						               System.String NoAttributeProperty
						             ]
						             """);
				}

				[Fact]
				public async Task WhenPropertiesHaveSecondAttribute_ShouldSucceed()
				{
					IEnumerable<PropertyInfo> subject = new[]
					{
						typeof(TestClass).GetProperty("BarProperty")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPredicate_WhenPropertiesHaveNotMatchingAttribute_ShouldFail()
				{
					IEnumerable<PropertyInfo> subject = new[]
					{
						typeof(TestClass).GetProperty("TestProperty1")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>(attr => attr.Value == "WrongValue")
							.OrHave<BarAttribute>(attr => attr.Name == "wrong");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatProperties.Have.OrHave.AttributeTests.TestAttribute matching attr => attr.Value == "WrongValue" or ThatProperties.Have.OrHave.AttributeTests.BarAttribute matching attr => attr.Name == "wrong",
						             but it contained not matching properties [
						               System.String TestProperty1
						             ]
						             """);
				}

				[AttributeUsage(AttributeTargets.Property)]
				private class TestAttribute : Attribute
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
					[Test(Value = "Property1Value")] public string TestProperty1 { get; set; } = "";

					[Bar(Name = "bar")] public string BarProperty { get; set; } = "";

					[Test(Value = "BothValue")]
					[Bar(Name = "both")]
					public string BothProperty { get; set; } = "";

					public string NoAttributeProperty { get; set; } = "";
				}
				// ReSharper restore UnusedMember.Local
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertiesDoNotHaveAttribute_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subjects = new[]
				{
					typeof(TestClass).GetProperty("NoAttributeProperty")!,
				};

				async Task Act()
					=> await That(subjects).DoesNotComplyWith(they => they.Have<TestAttribute>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesDoNotHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subjects = new[]
				{
					typeof(TestClass).GetProperty("TestProperty1")!,
				};

				async Task Act()
					=> await That(subjects).DoesNotComplyWith(they
						=> they.Have<TestAttribute>(attr => attr.Value == "NonExistent"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesHaveAttribute_ShouldFail()
			{
				IEnumerable<PropertyInfo> subjects = new[]
				{
					typeof(TestClass).GetProperty("TestProperty1")!,
				};

				async Task Act()
					=> await That(subjects).DoesNotComplyWith(they
						=> they.Have<TestAttribute>().OrHave<TestAttribute>(x => x.Value == "foo"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subjects
					             not all have ThatProperties.Have.NegatedTests.TestAttribute or ThatProperties.Have.NegatedTests.TestAttribute matching x => x.Value == "foo",
					             but it only contained matching properties [
					               System.String TestProperty1
					             ]
					             """);
			}

			[AttributeUsage(AttributeTargets.Property)]
			private class TestAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				[Test(Value = "Property1Value")] public string TestProperty1 { get; set; } = "";

				public string NoAttributeProperty { get; set; } = "";
			}
			// ReSharper restore UnusedMember.Local
		}
	}
}
