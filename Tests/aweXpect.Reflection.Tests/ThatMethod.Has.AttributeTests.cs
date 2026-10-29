using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class Has
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenMethodDoesNotHaveAttribute_ShouldFail()
			{
				MethodInfo subject = typeof(TestClass).GetMethod("NoAttributeMethod")!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatMethod.Has.AttributeTests.MarkerAttribute,
					             but it did not in void ThatMethod.Has.AttributeTests.TestClass.NoAttributeMethod()
					             """);
			}

			[Test]
			public async Task WhenMethodHasAttribute_ShouldSucceed()
			{
				MethodInfo subject = typeof(TestClass).GetMethod("TestMethod")!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodHasMatchingAttribute_ShouldSucceed()
			{
				MethodInfo subject = typeof(TestClass).GetMethod("TestMethod")!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>(attr => attr.Value == "MethodValue");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodHasNotMatchingAttribute_ShouldFail()
			{
				MethodInfo subject = typeof(TestClass).GetMethod("TestMethod")!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>(attr => attr.Value == "WrongValue");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatMethod.Has.AttributeTests.MarkerAttribute matching attr => attr.Value == "WrongValue",
					             but it did not in void ThatMethod.Has.AttributeTests.TestClass.TestMethod()
					             """);
			}

			[Test]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             has ThatMethod.Has.AttributeTests.MarkerAttribute,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WithType_WhenMethodHasAttribute_ShouldSucceed()
			{
				MethodInfo subject = typeof(TestClass).GetMethod("TestMethod")!;

				Type attributeType = typeof(MarkerAttribute);

				async Task Act()
				{
					await That(subject).Has(attributeType);
				}

				await That(Act).DoesNotThrow();
			}

			[AttributeUsage(AttributeTargets.Method)]
			private class MarkerAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				[Marker(Value = "MethodValue")]
				public static void TestMethod() { }

				public static void NoAttributeMethod() { }
			}
			// ReSharper restore UnusedMember.Local
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenMethodDoesNotHaveAttribute_ShouldSucceed()
			{
				MethodInfo subject = typeof(TestClass).GetMethod("NoAttributeMethod")!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Has<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodDoesNotHaveMatchingAttribute_ShouldSucceed()
			{
				MethodInfo subject = typeof(TestClass).GetMethod("TestMethod")!;

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(it => it.Has<MarkerAttribute>(attr => attr.Value == "NonExistent"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodHasAttribute_ShouldFail()
			{
				MethodInfo subject = typeof(TestClass).GetMethod("TestMethod")!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Has<MarkerAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatMethod.Has.NegatedTests.MarkerAttribute,
					             but it did in void ThatMethod.Has.NegatedTests.TestClass.TestMethod()
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
				[Marker(Value = "MethodValue")]
				public static void TestMethod() { }

				public static void NoAttributeMethod() { }
			}
			// ReSharper restore UnusedMember.Local
		}
	}
}
