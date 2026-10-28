using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class Have
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenAllMethodsHaveAttribute_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = new[]
				{
					typeof(TestClass).GetMethod("TestMethod1")!, typeof(TestClass).GetMethod("TestMethod2")!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllMethodsHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = new[]
				{
					typeof(TestClass).GetMethod("TestMethod1")!, typeof(TestClass).GetMethod("TestMethod2")!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>(attr => attr.Value.StartsWith("Method"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<MethodInfo?> subject = [typeof(TestClass).GetMethod("TestMethod1"), null,];

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
					             all have ThatMethods.Have.AttributeTests.MarkerAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               void ThatMethods.Have.AttributeTests.TestClass.TestMethod1(),
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all have ThatMethods.Have.AttributeTests.MarkerAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               void ThatMethods.Have.AttributeTests.TestClass.TestMethod1(),
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNotAllMethodsHaveAttribute_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = new[]
				{
					typeof(TestClass).GetMethod("TestMethod1")!, typeof(TestClass).GetMethod("NoAttributeMethod")!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatMethods.Have.AttributeTests.MarkerAttribute,
					             but it contained not matching methods

					             Not matching items:
					             [
					               void ThatMethods.Have.AttributeTests.TestClass.NoAttributeMethod()
					             ]
					             """);
			}

			[Test]
			public async Task WhenNotAllMethodsHaveMatchingAttribute_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = new[]
				{
					typeof(TestClass).GetMethod("TestMethod1")!, typeof(TestClass).GetMethod("TestMethod2")!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>(attr => attr.Value == "WrongValue");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatMethods.Have.AttributeTests.MarkerAttribute matching attr => attr.Value == "WrongValue",
					             but it contained not matching methods

					             Not matching items:
					             [
					               void ThatMethods.Have.AttributeTests.TestClass.TestMethod1(),
					               void ThatMethods.Have.AttributeTests.TestClass.TestMethod2()
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			public sealed class AsyncEnumerableTests
			{
				[Test]
				public async Task WhenAllMethodsHaveAttribute_ShouldSucceed()
				{
					IAsyncEnumerable<MethodInfo?> subject = new[]
					{
						typeof(TestClass).GetMethod("TestMethod1")!, typeof(TestClass).GetMethod("TestMethod2")!,
					}.ToTestAsyncEnumerable<MethodInfo?>();

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAllMethodsHaveMatchingAttribute_ShouldSucceed()
				{
					IAsyncEnumerable<MethodInfo?> subject = new[]
					{
						typeof(TestClass).GetMethod("TestMethod1")!, typeof(TestClass).GetMethod("TestMethod2")!,
					}.ToTestAsyncEnumerable<MethodInfo?>();

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value.StartsWith("Method"));
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenNotAllMethodsHaveAttribute_ShouldFail()
				{
					IAsyncEnumerable<MethodInfo?> subject = new[]
					{
						typeof(TestClass).GetMethod("TestMethod1")!, typeof(TestClass).GetMethod("NoAttributeMethod")!,
					}.ToTestAsyncEnumerable<MethodInfo?>();

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatMethods.Have.AttributeTests.MarkerAttribute,
						             but it contained not matching methods

						             Not matching items:
						             [
						               void ThatMethods.Have.AttributeTests.TestClass.NoAttributeMethod()
						             ]
						             """);
				}

				[Test]
				public async Task WhenNotAllMethodsHaveMatchingAttribute_ShouldFail()
				{
					IAsyncEnumerable<MethodInfo?> subject = new[]
					{
						typeof(TestClass).GetMethod("TestMethod1")!, typeof(TestClass).GetMethod("TestMethod2")!,
					}.ToTestAsyncEnumerable<MethodInfo?>();

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value == "WrongValue");
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatMethods.Have.AttributeTests.MarkerAttribute matching attr => attr.Value == "WrongValue",
						             but it contained not matching methods

						             Not matching items:
						             [
						               void ThatMethods.Have.AttributeTests.TestClass.TestMethod1(),
						               void ThatMethods.Have.AttributeTests.TestClass.TestMethod2()
						             ]
						             """);
				}
			}
#endif

			[AttributeUsage(AttributeTargets.Method)]
			private class MarkerAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				[Marker(Value = "Method1Value")]
				public static void TestMethod1() { }

				[Marker(Value = "Method2Value")]
				public static void TestMethod2() { }

				public static void NoAttributeMethod() { }
			}
			// ReSharper restore UnusedMember.Local
		}

		public sealed class OrHave
		{
			public sealed class AttributeTests
			{
				[Test]
				public async Task WhenMethodsHaveBothAttributes_ShouldSucceed()
				{
					IEnumerable<MethodInfo> subject = new[]
					{
						typeof(TestClass).GetMethod("BothMethod")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenMethodsHaveFirstAttribute_ShouldSucceed()
				{
					IEnumerable<MethodInfo> subject = new[]
					{
						typeof(TestClass).GetMethod("TestMethod1")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenMethodsHaveMatchingFirstAttribute_ShouldSucceed()
				{
					IEnumerable<MethodInfo> subject = new[]
					{
						typeof(TestClass).GetMethod("TestMethod1")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value == "Method1Value")
							.OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenMethodsHaveMatchingSecondAttribute_ShouldSucceed()
				{
					IEnumerable<MethodInfo> subject = new[]
					{
						typeof(TestClass).GetMethod("BarMethod")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value == "NonExistent")
							.OrHave<BarAttribute>(attr => attr.Name == "bar");
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenMethodsHaveNeitherAttribute_ShouldFail()
				{
					IEnumerable<MethodInfo> subject = new[]
					{
						typeof(TestClass).GetMethod("NoAttributeMethod")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatMethods.Have.OrHave.AttributeTests.MarkerAttribute or ThatMethods.Have.OrHave.AttributeTests.BarAttribute,
						             but it contained not matching methods

						             Not matching items:
						             [
						               void ThatMethods.Have.OrHave.AttributeTests.TestClass.NoAttributeMethod()
						             ]
						             """);
				}

				[Test]
				public async Task WhenMethodsHaveSecondAttribute_ShouldSucceed()
				{
					IEnumerable<MethodInfo> subject = new[]
					{
						typeof(TestClass).GetMethod("BarMethod")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WithPredicate_WhenMethodsHaveNotMatchingAttribute_ShouldFail()
				{
					IEnumerable<MethodInfo> subject = new[]
					{
						typeof(TestClass).GetMethod("TestMethod1")!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value == "WrongValue")
							.OrHave<BarAttribute>(attr => attr.Name == "wrong");
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatMethods.Have.OrHave.AttributeTests.MarkerAttribute matching attr => attr.Value == "WrongValue" or ThatMethods.Have.OrHave.AttributeTests.BarAttribute matching attr => attr.Name == "wrong",
						             but it contained not matching methods

						             Not matching items:
						             [
						               void ThatMethods.Have.OrHave.AttributeTests.TestClass.TestMethod1()
						             ]
						             """);
				}

				[AttributeUsage(AttributeTargets.Method)]
				private class MarkerAttribute : Attribute
				{
					public string Value { get; set; } = "";
				}

				[AttributeUsage(AttributeTargets.Method)]
				private class BarAttribute : Attribute
				{
					public string Name { get; set; } = "";
				}

				// ReSharper disable UnusedMember.Local
				private class TestClass
				{
					[Marker(Value = "Method1Value")]
					public static void TestMethod1() { }

					[Bar(Name = "bar")]
					public static void BarMethod() { }

					[Marker(Value = "BothValue")]
					[Bar(Name = "both")]
					public static void BothMethod() { }

					public static void NoAttributeMethod() { }
				}
				// ReSharper restore UnusedMember.Local
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenMethodsDoNotHaveAttribute_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subjects = new[]
				{
					typeof(TestClass).GetMethod("NoAttributeMethod")!,
				};

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they => they.Have<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsDoNotHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subjects = new[]
				{
					typeof(TestClass).GetMethod("TestMethod1")!,
				};

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they
						=> they.Have<MarkerAttribute>(attr => attr.Value == "NonExistent"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsHaveAttribute_ShouldFail()
			{
				IEnumerable<MethodInfo> subjects = new[]
				{
					typeof(TestClass).GetMethod("TestMethod1")!,
				};

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they
						=> they.Have<MarkerAttribute>().OrHave<MarkerAttribute>(x => x.Value == "foo"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subjects
					             not all have ThatMethods.Have.NegatedTests.MarkerAttribute or ThatMethods.Have.NegatedTests.MarkerAttribute matching x => x.Value == "foo",
					             but it only contained matching methods

					             Collection:
					             [
					               void ThatMethods.Have.NegatedTests.TestClass.TestMethod1()
					             ]
					             """);
			}

			[AttributeUsage(AttributeTargets.Method)]
			private class MarkerAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				[Marker(Value = "Method1Value")]
				public static void TestMethod1() { }

				public static void NoAttributeMethod() { }
			}
			// ReSharper restore UnusedMember.Local
		}
	}
}
