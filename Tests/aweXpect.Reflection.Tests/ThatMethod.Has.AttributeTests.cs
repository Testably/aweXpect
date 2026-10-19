using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class Has
	{
		public sealed class AttributeTests
		{
			[TUnit.Core.Test]
			public async Task WhenMethodDoesNotHaveAttribute_ShouldFail()
			{
				MethodInfo subject = typeof(TestClass).GetMethod("NoAttributeMethod")!;

				async Task Act()
				{
					await That(subject).Has<TestAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatMethod.Has.AttributeTests.TestAttribute,
					             but it did not in void ThatMethod.Has.AttributeTests.TestClass.NoAttributeMethod()
					             """);
			}

			[TUnit.Core.Test]
			public async Task WhenMethodHasAttribute_ShouldSucceed()
			{
				MethodInfo subject = typeof(TestClass).GetMethod("TestMethod")!;

				async Task Act()
				{
					await That(subject).Has<TestAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[TUnit.Core.Test]
			public async Task WhenMethodHasMatchingAttribute_ShouldSucceed()
			{
				MethodInfo subject = typeof(TestClass).GetMethod("TestMethod")!;

				async Task Act()
				{
					await That(subject).Has<TestAttribute>(attr => attr.Value == "MethodValue");
				}

				await That(Act).DoesNotThrow();
			}

			[TUnit.Core.Test]
			public async Task WhenMethodHasNotMatchingAttribute_ShouldFail()
			{
				MethodInfo subject = typeof(TestClass).GetMethod("TestMethod")!;

				async Task Act()
				{
					await That(subject).Has<TestAttribute>(attr => attr.Value == "WrongValue");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatMethod.Has.AttributeTests.TestAttribute matching attr => attr.Value == "WrongValue",
					             but it did not in void ThatMethod.Has.AttributeTests.TestClass.TestMethod()
					             """);
			}

			[TUnit.Core.Test]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).Has<TestAttribute>();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             has ThatMethod.Has.AttributeTests.TestAttribute,
					             but it was <null>
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
				[Test(Value = "MethodValue")]
				public void TestMethod() { }

				public void NoAttributeMethod() { }
			}
			// ReSharper restore UnusedMember.Local
		}

		public sealed class NegatedTests
		{
			[TUnit.Core.Test]
			public async Task WhenMethodDoesNotHaveAttribute_ShouldSucceed()
			{
				MethodInfo subject = typeof(TestClass).GetMethod("NoAttributeMethod")!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Has<TestAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[TUnit.Core.Test]
			public async Task WhenMethodDoesNotHaveMatchingAttribute_ShouldSucceed()
			{
				MethodInfo subject = typeof(TestClass).GetMethod("TestMethod")!;

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(it => it.Has<TestAttribute>(attr => attr.Value == "NonExistent"));
				}

				await That(Act).DoesNotThrow();
			}

			[TUnit.Core.Test]
			public async Task WhenMethodHasAttribute_ShouldFail()
			{
				MethodInfo subject = typeof(TestClass).GetMethod("TestMethod")!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Has<TestAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatMethod.Has.NegatedTests.TestAttribute,
					             but it did in void ThatMethod.Has.NegatedTests.TestClass.TestMethod()
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
				[Test(Value = "MethodValue")]
				public void TestMethod() { }

				public void NoAttributeMethod() { }
			}
			// ReSharper restore UnusedMember.Local
		}
	}
}
