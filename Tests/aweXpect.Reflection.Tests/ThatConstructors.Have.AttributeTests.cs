using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class Have
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task WhenAllConstructorsHaveAttribute_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(int),])!,
				};

				async Task Act()
					=> await That(subject).Have<TestAttribute>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllConstructorsHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(int),])!,
				};

				async Task Act()
					=> await That(subject).Have<TestAttribute>(attr => attr.Value.StartsWith("Constructor"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllConstructorsHaveAttribute_ShouldFail()
			{
				IEnumerable<ConstructorInfo> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(string),])!,
					typeof(TestClass).GetConstructor(Type.EmptyTypes)!,
				};

				async Task Act()
					=> await That(subject).Have<TestAttribute>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatConstructors.Have.AttributeTests.TestAttribute,
					             but it contained not matching constructors [
					               Void .ctor()
					             ]
					             """);
			}

			[Fact]
			public async Task WhenNotAllConstructorsHaveMatchingAttribute_ShouldFail()
			{
				IEnumerable<ConstructorInfo> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(int),])!,
				};

				async Task Act()
					=> await That(subject).Have<TestAttribute>(attr => attr.Value == "WrongValue");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatConstructors.Have.AttributeTests.TestAttribute matching attr => attr.Value == "WrongValue",
					             but it contained not matching constructors [
					               Void .ctor(System.String),
					               Void .ctor(Int32)
					             ]
					             """);
			}

			[AttributeUsage(AttributeTargets.Constructor)]
			private class TestAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

			// ReSharper disable UnusedMember.Local
			// ReSharper disable UnusedParameter.Local
			private class TestClass
			{
				public TestClass() { }

				[Test(Value = "ConstructorValue1")]
				public TestClass(string value) { }

				[Test(Value = "ConstructorValue2")]
				public TestClass(int value) { }
			}
			// ReSharper restore UnusedParameter.Local
			// ReSharper restore UnusedMember.Local
		}

		public sealed class OrHave
		{
			public sealed class AttributeTests
			{
				[Fact]
				public async Task WhenConstructorsHaveBothAttributes_ShouldSucceed()
				{
					IEnumerable<ConstructorInfo> subject = new[]
					{
						typeof(TestClass).GetConstructor([typeof(long),])!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenConstructorsHaveFirstAttribute_ShouldSucceed()
				{
					IEnumerable<ConstructorInfo> subject = new[]
					{
						typeof(TestClass).GetConstructor([typeof(string),])!,
						typeof(TestClass).GetConstructor([typeof(int),])!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenConstructorsHaveMatchingFirstAttribute_ShouldSucceed()
				{
					IEnumerable<ConstructorInfo> subject = new[]
					{
						typeof(TestClass).GetConstructor([typeof(string),])!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>(attr => attr.Value == "Constructor1Value")
							.OrHave<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenConstructorsHaveMatchingSecondAttribute_ShouldSucceed()
				{
					IEnumerable<ConstructorInfo> subject = new[]
					{
						typeof(TestClass).GetConstructor([typeof(double),])!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>()
							.OrHave<BarAttribute>(attr => attr.Name == "bar");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenConstructorsHaveNeitherAttribute_ShouldFail()
				{
					IEnumerable<ConstructorInfo> subject = new[]
					{
						typeof(TestClass).GetConstructor(Type.EmptyTypes)!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatConstructors.Have.OrHave.AttributeTests.TestAttribute or ThatConstructors.Have.OrHave.AttributeTests.BarAttribute,
						             but it contained not matching constructors [
						               Void .ctor()
						             ]
						             """);
				}

				[Fact]
				public async Task WhenConstructorsHaveSecondAttribute_ShouldSucceed()
				{
					IEnumerable<ConstructorInfo> subject = new[]
					{
						typeof(TestClass).GetConstructor([typeof(double),])!,
						typeof(TestClass).GetConstructor([typeof(float),])!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithInheritance_ShouldWorkCorrectly()
				{
					IEnumerable<ConstructorInfo> subject = new[]
					{
						typeof(TestClass).GetConstructor([typeof(string),])!,
					};

					async Task Act()
						=> await That(subject).Have<TestAttribute>(false);

					await That(Act).DoesNotThrow();
				}

				[AttributeUsage(AttributeTargets.Constructor)]
				private class TestAttribute : Attribute
				{
					public string Value { get; set; } = "";
				}

				[AttributeUsage(AttributeTargets.Constructor)]
				private class BarAttribute : Attribute
				{
					public string Name { get; set; } = "";
				}

				// ReSharper disable UnusedMember.Local
				// ReSharper disable UnusedParameter.Local
				private class TestClass
				{
					public TestClass() { }

					[Test(Value = "Constructor1Value")]
					public TestClass(string value) { }

					[Test(Value = "Constructor2Value")]
					public TestClass(int value) { }

					[Bar(Name = "bar")]
					public TestClass(double value) { }

					[Bar(Name = "bar2")]
					public TestClass(float value) { }

					[Test(Value = "BothValue")]
					[Bar(Name = "both")]
					public TestClass(long value) { }
				}
				// ReSharper restore UnusedParameter.Local
				// ReSharper restore UnusedMember.Local
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllConstructorsHaveAttribute_ShouldFail()
			{
				IEnumerable<ConstructorInfo> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(int),])!,
				};

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Have<TestAttribute>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             not all have ThatConstructors.Have.NegatedTests.TestAttribute,
					             but it only contained matching constructors [
					               Void .ctor(System.String),
					               Void .ctor(Int32)
					             ]
					             """);
			}

			[Fact]
			public async Task WhenNotAllConstructorsHaveAttribute_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(string),])!,
					typeof(TestClass).GetConstructor(Type.EmptyTypes)!,
				};

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Have<TestAttribute>());

				await That(Act).DoesNotThrow();
			}

			[AttributeUsage(AttributeTargets.Constructor)]
			private class TestAttribute : Attribute
			{
				// ReSharper disable once UnusedAutoPropertyAccessor.Local
				public string Value { get; set; } = "";
			}

			// ReSharper disable UnusedMember.Local
			// ReSharper disable UnusedParameter.Local
			private class TestClass
			{
				public TestClass() { }

				[Test(Value = "Constructor1Value")]
				public TestClass(string value) { }

				[Test(Value = "Constructor2Value")]
				public TestClass(int value) { }
			}
			// ReSharper restore UnusedParameter.Local
			// ReSharper restore UnusedMember.Local
		}
	}
}
