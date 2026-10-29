using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class DoesNotHave
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenFieldDoesNotHaveAttribute_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.NoAttributeField))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldHasAttribute_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.TestField))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatField.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in string ThatField.DoesNotHave.AttributeTests.TestClass.TestField
					             """);
			}

			[Test]
			public async Task WithType_WhenFieldHasAttribute_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.TestField))!;

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).DoesNotHave(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatField.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in string ThatField.DoesNotHave.AttributeTests.TestClass.TestField
					             """);
			}

			[AttributeUsage(AttributeTargets.Field)]
			private class FooAttribute : Attribute
			{
			}

#pragma warning disable CS0414 // Field is assigned but its value is never used
			private class TestClass
			{
				public string NoAttributeField = "";
				[Foo] public string TestField = "";
			}
#pragma warning restore CS0414
		}
	}
}
