using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Internal.Tests.Helpers;

public sealed class PropertyInfoHelpersTests
{
	[Fact]
	public async Task HasAccessModifier_Internal_ShouldMatchForExpectedProperty()
	{
		PropertyInfo propertyInfo = typeof(AccessModifierTestClass).GetDeclaredProperties()
			.Single(c => c.Name == nameof(AccessModifierTestClass.InternalProperty));

		await That(propertyInfo.HasAccessModifier(AccessModifiers.Internal)).IsTrue();
		await That(propertyInfo.HasAccessModifier(AccessModifiers.Private)).IsFalse();
		await That(propertyInfo.HasAccessModifier(AccessModifiers.Protected)).IsFalse();
		await That(propertyInfo.HasAccessModifier(AccessModifiers.Public)).IsFalse();
	}

	[Theory]
	[InlineData(nameof(AccessModifierTestClass.PublicPropertyWithPrivateGetter))]
	[InlineData(nameof(AccessModifierTestClass.PublicPropertyWithPrivateSetter))]
	public async Task HasAccessModifier_MixedAccessConditionsForGetterAndSetter_ShouldMatchBoth(string propertyName)
	{
		PropertyInfo propertyInfo = typeof(AccessModifierTestClass).GetDeclaredProperties()
			.Single(c => c.Name == propertyName);

		await That(propertyInfo).IsNotNull();
		await That(propertyInfo.HasAccessModifier(AccessModifiers.Internal)).IsFalse();
		await That(propertyInfo.HasAccessModifier(AccessModifiers.Private)).IsTrue();
		await That(propertyInfo.HasAccessModifier(AccessModifiers.Protected)).IsFalse();
		await That(propertyInfo.HasAccessModifier(AccessModifiers.Public)).IsTrue();
	}

	[Fact]
	public async Task HasAccessModifier_Private_ShouldMatchForExpectedProperty()
	{
		PropertyInfo propertyInfo = typeof(AccessModifierTestClass).GetDeclaredProperties()
			.Single(c => c.Name == "PrivateProperty");

		await That(propertyInfo.HasAccessModifier(AccessModifiers.Internal)).IsFalse();
		await That(propertyInfo.HasAccessModifier(AccessModifiers.Private)).IsTrue();
		await That(propertyInfo.HasAccessModifier(AccessModifiers.Protected)).IsFalse();
		await That(propertyInfo.HasAccessModifier(AccessModifiers.Public)).IsFalse();
	}

	[Fact]
	public async Task HasAccessModifier_Protected_ShouldMatchForExpectedProperty()
	{
		PropertyInfo propertyInfo = typeof(AccessModifierTestClass).GetDeclaredProperties()
			.Single(c => c.Name == "ProtectedProperty");

		await That(propertyInfo.HasAccessModifier(AccessModifiers.Internal)).IsFalse();
		await That(propertyInfo.HasAccessModifier(AccessModifiers.Private)).IsFalse();
		await That(propertyInfo.HasAccessModifier(AccessModifiers.Protected)).IsTrue();
		await That(propertyInfo.HasAccessModifier(AccessModifiers.Public)).IsFalse();
	}

	[Fact]
	public async Task HasAccessModifier_Public_ShouldMatchForExpectedProperty()
	{
		PropertyInfo propertyInfo = typeof(AccessModifierTestClass).GetDeclaredProperties()
			.Single(c => c.Name == nameof(AccessModifierTestClass.PublicProperty));

		await That(propertyInfo.HasAccessModifier(AccessModifiers.Internal)).IsFalse();
		await That(propertyInfo.HasAccessModifier(AccessModifiers.Private)).IsFalse();
		await That(propertyInfo.HasAccessModifier(AccessModifiers.Protected)).IsFalse();
		await That(propertyInfo.HasAccessModifier(AccessModifiers.Public)).IsTrue();
	}

	[Fact]
	public async Task HasAccessModifier_WhenNull_ShouldReturnFalse()
	{
		PropertyInfo? propertyInfo = null;

		await That(propertyInfo.HasAccessModifier(AccessModifiers.Internal)).IsFalse();
		await That(propertyInfo.HasAccessModifier(AccessModifiers.Private)).IsFalse();
		await That(propertyInfo.HasAccessModifier(AccessModifiers.Protected)).IsFalse();
		await That(propertyInfo.HasAccessModifier(AccessModifiers.Public)).IsFalse();
	}

	[Fact]
	public async Task HasAttribute_WithAttribute_ShouldReturnTrue()
	{
		PropertyInfo propertyInfo =
			typeof(TestClass).GetProperty(nameof(TestClass.Property1WithAttribute))!;

		bool result = propertyInfo.HasAttribute<DummyAttribute>();

		await That(result).IsTrue();
	}

	[Fact]
	public async Task HasAttribute_WithInheritedAttribute_ShouldReturnTrue()
	{
		PropertyInfo propertyInfo =
			typeof(TestClass).GetProperty(nameof(TestClass.PropertyWithAttributeInBaseClass))!;

		bool result = propertyInfo.HasAttribute<DummyAttribute>();

		await That(result).IsTrue();
	}

	[Fact]
	public async Task HasAttribute_WithoutAttribute_ShouldReturnFalse()
	{
		PropertyInfo propertyInfo =
			typeof(TestClass).GetProperty(nameof(TestClass.Property2WithoutAttribute))!;

		bool result = propertyInfo.HasAttribute<DummyAttribute>();

		await That(result).IsFalse();
	}

	[Fact]
	public async Task HasAttribute_WithPredicate_ShouldReturnPredicateResult()
	{
		PropertyInfo propertyInfo =
			typeof(TestClass).GetProperty(nameof(TestClass.Property1WithAttribute))!;

		bool result1 = propertyInfo.HasAttribute<DummyAttribute>(d => d.Value == 1);
		bool result2 = propertyInfo.HasAttribute<DummyAttribute>(d => d.Value == 2);

		await That(result1).IsTrue();
		await That(result2).IsFalse();
	}

	[AttributeUsage(AttributeTargets.Property)]
	private class DummyAttribute : Attribute
	{
		public DummyAttribute(int value)
		{
			Value = value;
		}

		public int Value { get; }
	}

	private class TestClass : TestClassBase
	{
		[Dummy(1)] public int Property1WithAttribute { get; set; } = 1;

		public int Property2WithoutAttribute { get; set; } = 1;

		public override int PropertyWithAttributeInBaseClass { get; set; } = 1;
	}

	private class TestClassBase
	{
		[Dummy(1)] public virtual int PropertyWithAttributeInBaseClass { get; set; } = 1;
	}

	private class AccessModifierTestClass
	{
		internal int InternalProperty { get; set; }

		private int PrivateProperty { get; set; }

		protected int ProtectedProperty { get; set; }
		public int PublicProperty { get; set; }
		public int PublicPropertyWithPrivateSetter { get; private set; }
		public int PublicPropertyWithPrivateGetter { private get; set; }
	}
}
