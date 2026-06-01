using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class DoesNotHave
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task WhenFieldDoesNotHaveAttribute_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.NoAttributeField))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldHasAttribute_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.TestField))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).Throws<XunitException>()
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
				[Foo] public string TestField = "";

				public string NoAttributeField = "";
			}
#pragma warning restore CS0414
		}
	}
}
