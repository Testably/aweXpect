using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class DoesNotHave
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenPropertyDoesNotHaveAttribute_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.NoAttributeProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyHasAttribute_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.TestProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatProperty.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in public string ThatProperty.DoesNotHave.AttributeTests.TestClass.TestProperty { get; set; }
					             """);
			}

			[Test]
			public async Task WhenPropertyInheritsAttributeFromOverriddenProperty_Directly_ShouldSucceed()
			{
				PropertyInfo subject = typeof(DerivedClass).GetProperty(nameof(DerivedClass.InheritedProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>().Directly();
				}

				await That(Act).DoesNotThrow()
					.Because("with Directly only the attributes declared on the overriding property count");
			}

			[Test]
			public async Task WhenPropertyInheritsAttributeFromOverriddenProperty_ShouldFail()
			{
				PropertyInfo subject = typeof(DerivedClass).GetProperty(nameof(DerivedClass.InheritedProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatProperty.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in public string ThatProperty.DoesNotHave.AttributeTests.DerivedClass.InheritedProperty { get; set; }
					             """)
					.Because("the attribute on the overridden base property is inherited, as in the Properties().With<TAttribute>() filter");
			}

			[Test]
			public async Task WithType_WhenPropertyHasAttribute_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.TestProperty))!;

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).DoesNotHave(attributeType);
				}

				await That(Act).Throws<FailException>()
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

			private class BaseClass
			{
				[Foo] public virtual string InheritedProperty { get; set; } = "";
			}

			private class DerivedClass : BaseClass
			{
				public override string InheritedProperty { get; set; } = "";
			}
		}
	}
}
