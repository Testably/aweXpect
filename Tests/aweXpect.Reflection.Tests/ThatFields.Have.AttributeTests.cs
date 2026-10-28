using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class Have
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenAllFieldsHaveAttribute_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject = new[]
				{
					typeof(TestClass).GetField("TestField1")!, typeof(TestClass).GetField("TestField2")!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllFieldsHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject = new[]
				{
					typeof(TestClass).GetField("TestField1")!, typeof(TestClass).GetField("TestField2")!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>(attr => attr.Value.StartsWith("Field"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<FieldInfo?> subject = [typeof(TestClass).GetField("TestField1"), null,];

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
					             all have ThatFields.Have.AttributeTests.MarkerAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               string ThatFields.Have.AttributeTests.TestClass.TestField1,
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all have ThatFields.Have.AttributeTests.MarkerAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               string ThatFields.Have.AttributeTests.TestClass.TestField1,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNotAllFieldsHaveAttribute_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = new[]
				{
					typeof(TestClass).GetField("TestField1")!, typeof(TestClass).GetField("NoAttributeField")!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatFields.Have.AttributeTests.MarkerAttribute,
					             but it contained not matching fields

					             Not matching items:
					             [
					               string ThatFields.Have.AttributeTests.TestClass.NoAttributeField
					             ]
					             """);
			}

			[Test]
			public async Task WhenNotAllFieldsHaveMatchingAttribute_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = new[]
				{
					typeof(TestClass).GetField("TestField1")!, typeof(TestClass).GetField("TestField2")!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>(attr => attr.Value == "WrongValue");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatFields.Have.AttributeTests.MarkerAttribute matching attr => attr.Value == "WrongValue",
					             but it contained not matching fields

					             Not matching items:
					             [
					               string ThatFields.Have.AttributeTests.TestClass.TestField1,
					               string ThatFields.Have.AttributeTests.TestClass.TestField2
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			public sealed class AsyncEnumerableTests
			{
				[Test]
				public async Task WhenAllFieldsHaveAttribute_ShouldSucceed()
				{
					IAsyncEnumerable<FieldInfo?> subject = new[]
					{
						typeof(TestClass).GetField("TestField1")!, typeof(TestClass).GetField("TestField2")!,
					}.ToTestAsyncEnumerable<FieldInfo?>();

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAllFieldsHaveMatchingAttribute_ShouldSucceed()
				{
					IAsyncEnumerable<FieldInfo?> subject = new[]
					{
						typeof(TestClass).GetField("TestField1")!, typeof(TestClass).GetField("TestField2")!,
					}.ToTestAsyncEnumerable<FieldInfo?>();

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value.StartsWith("Field"));
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenNotAllFieldsHaveAttribute_ShouldFail()
				{
					IAsyncEnumerable<FieldInfo?> subject = new[]
					{
						typeof(TestClass).GetField("TestField1")!, typeof(TestClass).GetField("NoAttributeField")!,
					}.ToTestAsyncEnumerable<FieldInfo?>();

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatFields.Have.AttributeTests.MarkerAttribute,
						             but it contained not matching fields

						             Not matching items:
						             [
						               string ThatFields.Have.AttributeTests.TestClass.NoAttributeField
						             ]
						             """);
				}

				[Test]
				public async Task WhenNotAllFieldsHaveMatchingAttribute_ShouldFail()
				{
					IAsyncEnumerable<FieldInfo?> subject = new[]
					{
						typeof(TestClass).GetField("TestField1")!, typeof(TestClass).GetField("TestField2")!,
					}.ToTestAsyncEnumerable<FieldInfo?>();

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value == "WrongValue");
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatFields.Have.AttributeTests.MarkerAttribute matching attr => attr.Value == "WrongValue",
						             but it contained not matching fields

						             Not matching items:
						             [
						               string ThatFields.Have.AttributeTests.TestClass.TestField1,
						               string ThatFields.Have.AttributeTests.TestClass.TestField2
						             ]
						             """);
				}
			}
#endif

			[AttributeUsage(AttributeTargets.Field)]
			private class MarkerAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

#pragma warning disable CS0414 // Field is assigned but its value is never used
			private class TestClass
			{
				public string NoAttributeField = "";

				[Marker(Value = "Field1Value")] public string TestField1 = "";

				[Marker(Value = "Field2Value")] public string TestField2 = "";
			}
#pragma warning restore CS0414
		}

		public sealed class OrHave
		{
			public sealed class AttributeTests
			{
				[Test]
				public async Task WhenFieldsHaveBothAttributes_ShouldSucceed()
				{
					IEnumerable<FieldInfo> subject = new[]
					{
						typeof(TestClass).GetField("BothField")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenFieldsHaveFirstAttribute_ShouldSucceed()
				{
					IEnumerable<FieldInfo> subject = new[]
					{
						typeof(TestClass).GetField("TestField1")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenFieldsHaveMatchingFirstAttribute_ShouldSucceed()
				{
					IEnumerable<FieldInfo> subject = new[]
					{
						typeof(TestClass).GetField("TestField1")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value == "Field1Value")
							.OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenFieldsHaveMatchingSecondAttribute_ShouldSucceed()
				{
					IEnumerable<FieldInfo> subject = new[]
					{
						typeof(TestClass).GetField("BarField")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value == "NonExistent")
							.OrHave<BarAttribute>(attr => attr.Name == "bar");
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenFieldsHaveNeitherAttribute_ShouldFail()
				{
					IEnumerable<FieldInfo> subject = new[]
					{
						typeof(TestClass).GetField("NoAttributeField")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatFields.Have.OrHave.AttributeTests.MarkerAttribute or ThatFields.Have.OrHave.AttributeTests.BarAttribute,
						             but it contained not matching fields

						             Not matching items:
						             [
						               string ThatFields.Have.OrHave.AttributeTests.TestClass.NoAttributeField
						             ]
						             """);
				}

				[Test]
				public async Task WhenFieldsHaveSecondAttribute_ShouldSucceed()
				{
					IEnumerable<FieldInfo> subject = new[]
					{
						typeof(TestClass).GetField("BarField")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WithPredicate_WhenFieldsHaveNotMatchingAttribute_ShouldFail()
				{
					IEnumerable<FieldInfo> subject = new[]
					{
						typeof(TestClass).GetField("TestField1")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value == "WrongValue")
							.OrHave<BarAttribute>(attr => attr.Name == "wrong");
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatFields.Have.OrHave.AttributeTests.MarkerAttribute matching attr => attr.Value == "WrongValue" or ThatFields.Have.OrHave.AttributeTests.BarAttribute matching attr => attr.Name == "wrong",
						             but it contained not matching fields

						             Not matching items:
						             [
						               string ThatFields.Have.OrHave.AttributeTests.TestClass.TestField1
						             ]
						             """);
				}

				[AttributeUsage(AttributeTargets.Field)]
				private class MarkerAttribute : Attribute
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

					[Marker(Value = "BothValue")] [Bar(Name = "both")]
					public string BothField = "";

					public string NoAttributeField = "";

					[Marker(Value = "Field1Value")] public string TestField1 = "";
				}
#pragma warning restore CS0414
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFieldsDoNotHaveAttribute_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subjects = new[]
				{
					typeof(TestClass).GetField("NoAttributeField")!,
				};

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they => they.Have<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldsDoNotHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subjects = new[]
				{
					typeof(TestClass).GetField("TestField1")!,
				};

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they
						=> they.Have<MarkerAttribute>(attr => attr.Value == "NonExistent"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldsHaveAttribute_ShouldFail()
			{
				IEnumerable<FieldInfo> subjects = new[]
				{
					typeof(TestClass).GetField("TestField1")!,
				};

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they
						=> they.Have<MarkerAttribute>().OrHave<MarkerAttribute>(x => x.Value == "foo"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subjects
					             not all have ThatFields.Have.NegatedTests.MarkerAttribute or ThatFields.Have.NegatedTests.MarkerAttribute matching x => x.Value == "foo",
					             but it only contained matching fields

					             Collection:
					             [
					               string ThatFields.Have.NegatedTests.TestClass.TestField1
					             ]
					             """);
			}

			[AttributeUsage(AttributeTargets.Field)]
			private class MarkerAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

#pragma warning disable CS0414 // Field is assigned but its value is never used
			private class TestClass
			{
				public string NoAttributeField = "";
				[Marker(Value = "Field1Value")] public string TestField1 = "";
			}
#pragma warning restore CS0414
		}
	}
}
