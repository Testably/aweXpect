using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class Have
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task WhenAllFieldsHaveAttribute_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject = new[]
				{
					typeof(TestClass).GetField("TestField1")!, typeof(TestClass).GetField("TestField2")!,
				};

				async Task Act()
					=> await That(subject).Have<TestAttribute>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllFieldsHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject = new[]
				{
					typeof(TestClass).GetField("TestField1")!, typeof(TestClass).GetField("TestField2")!,
				};

				async Task Act()
					=> await That(subject).Have<TestAttribute>(attr => attr.Value.StartsWith("Field"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllFieldsHaveAttribute_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = new[]
				{
					typeof(TestClass).GetField("TestField1")!, typeof(TestClass).GetField("NoAttributeField")!,
				};

				async Task Act()
					=> await That(subject).Have<TestAttribute>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatFields.Have.AttributeTests.TestAttribute,
					             but it contained not matching fields [
					               System.String NoAttributeField
					             ]
					             """);
			}

			[Fact]
			public async Task WhenNotAllFieldsHaveMatchingAttribute_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = new[]
				{
					typeof(TestClass).GetField("TestField1")!, typeof(TestClass).GetField("TestField2")!,
				};

				async Task Act()
					=> await That(subject).Have<TestAttribute>(attr => attr.Value == "WrongValue");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatFields.Have.AttributeTests.TestAttribute matching attr => attr.Value == "WrongValue",
					             but it contained not matching fields [
					               System.String TestField1,
					               System.String TestField2
					             ]
					             """);
			}

			[AttributeUsage(AttributeTargets.Field)]
			private class TestAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

#pragma warning disable CS0414 // Field is assigned but its value is never used
			private class TestClass
			{
				public string NoAttributeField = "";

				[Test(Value = "Field1Value")] public string TestField1 = "";

				[Test(Value = "Field2Value")] public string TestField2 = "";
			}
#pragma warning restore CS0414
		}

		public sealed class OrHave
		{
			public sealed class AttributeTests
			{
				[Fact]
				public async Task WhenFieldsHaveBothAttributes_ShouldSucceed()
				{
					IEnumerable<FieldInfo> subject = new[]
					{
						typeof(TestClass).GetField("BothField")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenFieldsHaveFirstAttribute_ShouldSucceed()
				{
					IEnumerable<FieldInfo> subject = new[]
					{
						typeof(TestClass).GetField("TestField1")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenFieldsHaveMatchingFirstAttribute_ShouldSucceed()
				{
					IEnumerable<FieldInfo> subject = new[]
					{
						typeof(TestClass).GetField("TestField1")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>(attr => attr.Value == "Field1Value")
							.OrHave<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenFieldsHaveMatchingSecondAttribute_ShouldSucceed()
				{
					IEnumerable<FieldInfo> subject = new[]
					{
						typeof(TestClass).GetField("BarField")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>(attr => attr.Value == "NonExistent")
							.OrHave<BarAttribute>(attr => attr.Name == "bar");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenFieldsHaveNeitherAttribute_ShouldFail()
				{
					IEnumerable<FieldInfo> subject = new[]
					{
						typeof(TestClass).GetField("NoAttributeField")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatFields.Have.OrHave.AttributeTests.TestAttribute or ThatFields.Have.OrHave.AttributeTests.BarAttribute,
						             but it contained not matching fields [
						               System.String NoAttributeField
						             ]
						             """);
				}

				[Fact]
				public async Task WhenFieldsHaveSecondAttribute_ShouldSucceed()
				{
					IEnumerable<FieldInfo> subject = new[]
					{
						typeof(TestClass).GetField("BarField")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPredicate_WhenFieldsHaveNotMatchingAttribute_ShouldFail()
				{
					IEnumerable<FieldInfo> subject = new[]
					{
						typeof(TestClass).GetField("TestField1")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>(attr => attr.Value == "WrongValue")
							.OrHave<BarAttribute>(attr => attr.Name == "wrong");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatFields.Have.OrHave.AttributeTests.TestAttribute matching attr => attr.Value == "WrongValue" or ThatFields.Have.OrHave.AttributeTests.BarAttribute matching attr => attr.Name == "wrong",
						             but it contained not matching fields [
						               System.String TestField1
						             ]
						             """);
				}

				[AttributeUsage(AttributeTargets.Field)]
				private class TestAttribute : Attribute
				{
					public string Value { get; set; } = "";
				}

				[AttributeUsage(AttributeTargets.Field)]
				private class BarAttribute : Attribute
				{
					public string Name { get; set; } = "";
				}

#pragma warning disable CS0414 // Field is assigned but its value is never used
				private class TestClass
				{
					[Bar(Name = "bar")] public string BarField = "";

					[Test(Value = "BothValue")] [Bar(Name = "both")]
					public string BothField = "";

					public string NoAttributeField = "";

					[Test(Value = "Field1Value")] public string TestField1 = "";
				}
#pragma warning restore CS0414
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFieldsDoNotHaveAttribute_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subjects = new[]
				{
					typeof(TestClass).GetField("NoAttributeField")!,
				};

				async Task Act()
					=> await That(subjects).DoesNotComplyWith(they => they.Have<TestAttribute>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldsDoNotHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subjects = new[]
				{
					typeof(TestClass).GetField("TestField1")!,
				};

				async Task Act()
					=> await That(subjects).DoesNotComplyWith(they
						=> they.Have<TestAttribute>(attr => attr.Value == "NonExistent"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldsHaveAttribute_ShouldFail()
			{
				IEnumerable<FieldInfo> subjects = new[]
				{
					typeof(TestClass).GetField("TestField1")!,
				};

				async Task Act()
					=> await That(subjects).DoesNotComplyWith(they
						=> they.Have<TestAttribute>().OrHave<TestAttribute>(x => x.Value == "foo"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subjects
					             not all have ThatFields.Have.NegatedTests.TestAttribute or ThatFields.Have.NegatedTests.TestAttribute matching x => x.Value == "foo",
					             but it only contained matching fields [
					               System.String TestField1
					             ]
					             """);
			}

			[AttributeUsage(AttributeTargets.Field)]
			private class TestAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

#pragma warning disable CS0414 // Field is assigned but its value is never used
			private class TestClass
			{
				public string NoAttributeField = "";
				[Test(Value = "Field1Value")] public string TestField1 = "";
			}
#pragma warning restore CS0414
		}
	}
}
