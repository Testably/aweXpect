using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class DoesNotHave
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task WhenPropertyDoesNotHaveAttribute_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.NoAttributeProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyHasAttribute_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.TestProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatProperty.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in public string ThatProperty.DoesNotHave.AttributeTests.TestClass.TestProperty { get; set; }
					             """);
			}

			[AttributeUsage(AttributeTargets.Property)]
			private class FooAttribute : Attribute
			{
			}

			private class TestClass
			{
				[Foo] public string TestProperty { get; set; } = "";

				public string NoAttributeProperty { get; set; } = "";
			}
		}
	}
}
