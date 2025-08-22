using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class Has
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task WhenPropertyDoesNotHaveAttribute_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty("NoAttributeProperty")!;

				async Task Act()
					=> await That(subject).Has<TestAttribute>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has ThatProperty.Has.AttributeTests.TestAttribute,
					             but it did not in public string ThatProperty.Has.AttributeTests.TestClass.NoAttributeProperty { get; set; }
					             """);
			}

			[Fact]
			public async Task WhenPropertyHasAttribute_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty("TestProperty")!;

				async Task Act()
					=> await That(subject).Has<TestAttribute>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyHasMatchingAttribute_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty("TestProperty")!;

				async Task Act()
					=> await That(subject).Has<TestAttribute>(attr => attr.Value == "PropertyValue");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyHasNotMatchingAttribute_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty("TestProperty")!;

				async Task Act()
					=> await That(subject).Has<TestAttribute>(attr => attr.Value == "WrongValue");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has ThatProperty.Has.AttributeTests.TestAttribute matching attr => attr.Value == "WrongValue",
					             but it did not in public string ThatProperty.Has.AttributeTests.TestClass.TestProperty { get; set; }
					             """);
			}

			[Fact]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
					=> await That(subject).Has<TestAttribute>();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             has ThatProperty.Has.AttributeTests.TestAttribute,
					             but it was <null>
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
				[Test(Value = "PropertyValue")] public string TestProperty { get; set; } = "";

				public string NoAttributeProperty { get; set; } = "";
			}
			// ReSharper restore UnusedMember.Local
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyDoesNotHaveAttribute_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty("NoAttributeProperty")!;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Has<TestAttribute>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyDoesNotHaveMatchingAttribute_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty("TestProperty")!;

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.Has<TestAttribute>(attr => attr.Value == "NonExistent"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyHasAttribute_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty("TestProperty")!;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Has<TestAttribute>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatProperty.Has.NegatedTests.TestAttribute,
					             but it did in public string ThatProperty.Has.NegatedTests.TestClass.TestProperty { get; set; }
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
				[Test(Value = "PropertyValue")] public string TestProperty { get; set; } = "";

				public string NoAttributeProperty { get; set; } = "";
			}
			// ReSharper restore UnusedMember.Local
		}
	}
}
