using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class Have
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task WhenAllMethodsHaveAttribute_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = new[]
				{
					typeof(TestClass).GetMethod("TestMethod1")!, typeof(TestClass).GetMethod("TestMethod2")!,
				};

				async Task Act()
					=> await That(subject).Have<TestAttribute>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllMethodsHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = new[]
				{
					typeof(TestClass).GetMethod("TestMethod1")!, typeof(TestClass).GetMethod("TestMethod2")!,
				};

				async Task Act()
					=> await That(subject).Have<TestAttribute>(attr => attr.Value.StartsWith("Method"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllMethodsHaveAttribute_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = new[]
				{
					typeof(TestClass).GetMethod("TestMethod1")!, typeof(TestClass).GetMethod("NoAttributeMethod")!,
				};

				async Task Act()
					=> await That(subject).Have<TestAttribute>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatMethods.Have.AttributeTests.TestAttribute,
					             but it contained not matching methods [
					               void ThatMethods.Have.AttributeTests.TestClass.NoAttributeMethod()
					             ]
					             """);
			}

			[Fact]
			public async Task WhenNotAllMethodsHaveMatchingAttribute_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = new[]
				{
					typeof(TestClass).GetMethod("TestMethod1")!, typeof(TestClass).GetMethod("TestMethod2")!,
				};

				async Task Act()
					=> await That(subject).Have<TestAttribute>(attr => attr.Value == "WrongValue");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatMethods.Have.AttributeTests.TestAttribute matching attr => attr.Value == "WrongValue",
					             but it contained not matching methods [
					               void ThatMethods.Have.AttributeTests.TestClass.TestMethod1(),
					               void ThatMethods.Have.AttributeTests.TestClass.TestMethod2()
					             ]
					             """);
			}

			[AttributeUsage(AttributeTargets.Method)]
			private class TestAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				[Test(Value = "Method1Value")]
				public void TestMethod1() { }

				[Test(Value = "Method2Value")]
				public void TestMethod2() { }

				public void NoAttributeMethod() { }
			}
			// ReSharper restore UnusedMember.Local
		}

		public sealed class OrHave
		{
			public sealed class AttributeTests
			{
				[Fact]
				public async Task WhenMethodsHaveBothAttributes_ShouldSucceed()
				{
					IEnumerable<MethodInfo> subject = new[]
					{
						typeof(TestClass).GetMethod("BothMethod")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenMethodsHaveFirstAttribute_ShouldSucceed()
				{
					IEnumerable<MethodInfo> subject = new[]
					{
						typeof(TestClass).GetMethod("TestMethod1")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenMethodsHaveMatchingFirstAttribute_ShouldSucceed()
				{
					IEnumerable<MethodInfo> subject = new[]
					{
						typeof(TestClass).GetMethod("TestMethod1")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>(attr => attr.Value == "Method1Value")
							.OrHave<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenMethodsHaveMatchingSecondAttribute_ShouldSucceed()
				{
					IEnumerable<MethodInfo> subject = new[]
					{
						typeof(TestClass).GetMethod("BarMethod")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>(attr => attr.Value == "NonExistent")
							.OrHave<BarAttribute>(attr => attr.Name == "bar");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenMethodsHaveNeitherAttribute_ShouldFail()
				{
					IEnumerable<MethodInfo> subject = new[]
					{
						typeof(TestClass).GetMethod("NoAttributeMethod")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatMethods.Have.OrHave.AttributeTests.TestAttribute or ThatMethods.Have.OrHave.AttributeTests.BarAttribute,
						             but it contained not matching methods [
						               void ThatMethods.Have.OrHave.AttributeTests.TestClass.NoAttributeMethod()
						             ]
						             """);
				}

				[Fact]
				public async Task WhenMethodsHaveSecondAttribute_ShouldSucceed()
				{
					IEnumerable<MethodInfo> subject = new[]
					{
						typeof(TestClass).GetMethod("BarMethod")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPredicate_WhenMethodsHaveNotMatchingAttribute_ShouldFail()
				{
					IEnumerable<MethodInfo> subject = new[]
					{
						typeof(TestClass).GetMethod("TestMethod1")!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>(attr => attr.Value == "WrongValue")
							.OrHave<BarAttribute>(attr => attr.Name == "wrong");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatMethods.Have.OrHave.AttributeTests.TestAttribute matching attr => attr.Value == "WrongValue" or ThatMethods.Have.OrHave.AttributeTests.BarAttribute matching attr => attr.Name == "wrong",
						             but it contained not matching methods [
						               void ThatMethods.Have.OrHave.AttributeTests.TestClass.TestMethod1()
						             ]
						             """);
				}

				[AttributeUsage(AttributeTargets.Method)]
				private class TestAttribute : Attribute
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
					[Test(Value = "Method1Value")]
					public void TestMethod1() { }

					[Bar(Name = "bar")]
					public void BarMethod() { }

					[Test(Value = "BothValue")]
					[Bar(Name = "both")]
					public void BothMethod() { }

					public void NoAttributeMethod() { }
				}
				// ReSharper restore UnusedMember.Local
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenMethodsDoNotHaveAttribute_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subjects = new[]
				{
					typeof(TestClass).GetMethod("NoAttributeMethod")!,
				};

				async Task Act()
					=> await That(subjects).DoesNotComplyWith(they => they.Have<TestAttribute>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsDoNotHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subjects = new[]
				{
					typeof(TestClass).GetMethod("TestMethod1")!,
				};

				async Task Act()
					=> await That(subjects).DoesNotComplyWith(they
						=> they.Have<TestAttribute>(attr => attr.Value == "NonExistent"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsHaveAttribute_ShouldFail()
			{
				IEnumerable<MethodInfo> subjects = new[]
				{
					typeof(TestClass).GetMethod("TestMethod1")!,
				};

				async Task Act()
					=> await That(subjects).DoesNotComplyWith(they
						=> they.Have<TestAttribute>().OrHave<TestAttribute>(x => x.Value == "foo"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subjects
					             not all have ThatMethods.Have.NegatedTests.TestAttribute or ThatMethods.Have.NegatedTests.TestAttribute matching x => x.Value == "foo",
					             but it only contained matching methods [
					               void ThatMethods.Have.NegatedTests.TestClass.TestMethod1()
					             ]
					             """);
			}

			[AttributeUsage(AttributeTargets.Method)]
			private class TestAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				[Test(Value = "Method1Value")]
				public void TestMethod1() { }

				public void NoAttributeMethod() { }
			}
			// ReSharper restore UnusedMember.Local
		}
	}
}
