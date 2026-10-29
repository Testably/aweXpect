using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class Has
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenPropertyDoesNotHaveAttribute_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty("NoAttributeProperty")!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatProperty.Has.AttributeTests.MarkerAttribute,
					             but it did not in public string ThatProperty.Has.AttributeTests.TestClass.NoAttributeProperty { get; set; }
					             """);
			}

			[Test]
			public async Task WhenPropertyHasAttribute_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty("TestProperty")!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyHasMatchingAttribute_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty("TestProperty")!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>(attr => attr.Value == "PropertyValue");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyHasNotMatchingAttribute_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty("TestProperty")!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>(attr => attr.Value == "WrongValue");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatProperty.Has.AttributeTests.MarkerAttribute matching attr => attr.Value == "WrongValue",
					             but it did not in public string ThatProperty.Has.AttributeTests.TestClass.TestProperty { get; set; }
					             """);
			}

			[Test]
			public async Task WhenPropertyInheritsAttributeFromOverriddenProperty_Directly_ShouldFail()
			{
				PropertyInfo subject = typeof(DerivedClass).GetProperty(nameof(DerivedClass.InheritedProperty))!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>().Directly();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has direct ThatProperty.Has.AttributeTests.MarkerAttribute,
					             but it did not in public string ThatProperty.Has.AttributeTests.DerivedClass.InheritedProperty { get; set; }
					             """)
					.Because("with Directly only the attributes declared on the overriding property count");
			}

			[Test]
			public async Task WhenPropertyInheritsAttributeFromOverriddenProperty_ShouldSucceed()
			{
				PropertyInfo subject = typeof(DerivedClass).GetProperty(nameof(DerivedClass.InheritedProperty))!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>();
				}

				await That(Act).DoesNotThrow()
					.Because("the attribute on the overridden base property is inherited, as in the Properties().With<TAttribute>() filter");
			}

			[Test]
			public async Task WhenPropertyInheritsMatchingAttributeFromOverriddenProperty_ShouldSucceed()
			{
				PropertyInfo subject = typeof(DerivedClass).GetProperty(nameof(DerivedClass.InheritedProperty))!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>(attr => attr.Value == "BaseValue");
				}

				await That(Act).DoesNotThrow()
					.Because("the predicate is evaluated on the attribute inherited from the overridden base property");
			}

			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             has ThatProperty.Has.AttributeTests.MarkerAttribute,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenPropertyOverridesPropertyWithNonInheritableAttribute_ShouldFail()
			{
				PropertyInfo subject = typeof(DerivedClass).GetProperty(nameof(DerivedClass.NonInheritableProperty))!;

				async Task Act()
				{
					await That(subject).Has<NonInheritableAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatProperty.Has.AttributeTests.NonInheritableAttribute,
					             but it did not in public string ThatProperty.Has.AttributeTests.DerivedClass.NonInheritableProperty { get; set; }
					             """)
					.Because("an attribute with Inherited = false is not inherited from the overridden base property");
			}

			[Test]
			public async Task WithType_WhenPropertyHasAttribute_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty("TestProperty")!;

				Type attributeType = typeof(MarkerAttribute);

				async Task Act()
				{
					await That(subject).Has(attributeType);
				}

				await That(Act).DoesNotThrow();
			}

			[AttributeUsage(AttributeTargets.Property)]
			private class MarkerAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

			[AttributeUsage(AttributeTargets.Property, Inherited = false)]
			private class NonInheritableAttribute : Attribute
			{
			}

			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				[Marker(Value = "PropertyValue")] public string TestProperty { get; set; } = "";

				public string NoAttributeProperty { get; set; } = "";
			}

			private class BaseClass
			{
				[Marker(Value = "BaseValue")] public virtual string InheritedProperty { get; set; } = "";

				[NonInheritable] public virtual string NonInheritableProperty { get; set; } = "";
			}

			private class DerivedClass : BaseClass
			{
				public override string InheritedProperty { get; set; } = "";

				public override string NonInheritableProperty { get; set; } = "";
			}
			// ReSharper restore UnusedMember.Local
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertyDoesNotHaveAttribute_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty("NoAttributeProperty")!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Has<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyDoesNotHaveMatchingAttribute_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty("TestProperty")!;

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(it => it.Has<MarkerAttribute>(attr => attr.Value == "NonExistent"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyHasAttribute_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty("TestProperty")!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Has<MarkerAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatProperty.Has.NegatedTests.MarkerAttribute,
					             but it did in public string ThatProperty.Has.NegatedTests.TestClass.TestProperty { get; set; }
					             """);
			}

			[AttributeUsage(AttributeTargets.Property)]
			private class MarkerAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				[Marker(Value = "PropertyValue")] public string TestProperty { get; set; } = "";

				public string NoAttributeProperty { get; set; } = "";
			}
			// ReSharper restore UnusedMember.Local
		}
	}
}
