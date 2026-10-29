using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class Has
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenFieldDoesNotHaveAttribute_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField("NoAttributeField")!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatField.Has.AttributeTests.MarkerAttribute,
					             but it did not in string ThatField.Has.AttributeTests.TestClass.NoAttributeField
					             """);
			}

			[Test]
			public async Task WhenFieldHasAttribute_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField("TestField")!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldHasMatchingAttribute_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField("TestFieldWithValue")!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>(attr => attr.Value == 42);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldHasNotMatchingAttribute_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField("TestFieldWithValue")!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>(attr => attr.Value == 99);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatField.Has.AttributeTests.MarkerAttribute matching attr => attr.Value == 99,
					             but it did not in string ThatField.Has.AttributeTests.TestClass.TestFieldWithValue
					             """);
			}

			[Test]
			public async Task WhenFieldIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             has ThatField.Has.AttributeTests.MarkerAttribute,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WithType_WhenFieldHasAttribute_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField("TestField")!;

				Type attributeType = typeof(MarkerAttribute);

				async Task Act()
				{
					await That(subject).Has(attributeType);
				}

				await That(Act).DoesNotThrow();
			}

			[AttributeUsage(AttributeTargets.Field)]
			private class MarkerAttribute : Attribute
			{
				public int Value { get; set; }
			}

#pragma warning disable CS0414 // Field is assigned but its value is never used
			private class TestClass
			{
				public string NoAttributeField = "";

				[Marker] public string TestField = "";

				[Marker(Value = 42)] public string TestFieldWithValue = "";
			}
#pragma warning restore CS0414
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFieldDoesNotHaveAttribute_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField("NoAttributeField")!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Has<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldDoesNotHaveMatchingAttribute_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField("TestFieldWithValue")!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Has<MarkerAttribute>(attr => attr.Value == 999));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldHasAttribute_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField("TestField")!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Has<MarkerAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatField.Has.NegatedTests.MarkerAttribute,
					             but it did in string ThatField.Has.NegatedTests.TestClass.TestField
					             """);
			}

			[AttributeUsage(AttributeTargets.Field)]
			private class MarkerAttribute : Attribute
			{
				public int Value { get; set; }
			}

#pragma warning disable CS0414 // Field is assigned but its value is never used
			private class TestClass
			{
				public string NoAttributeField = "";

				[Marker] public string TestField = "";

				[Marker(Value = 42)] public string TestFieldWithValue = "";
			}
#pragma warning restore CS0414
		}
	}
}
