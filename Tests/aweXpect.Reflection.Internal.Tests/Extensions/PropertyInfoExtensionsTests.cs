using System.Reflection;
using aweXpect.Reflection.Extensions;

namespace aweXpect.Reflection.Internal.Tests.Extensions;

public sealed class PropertyInfoExtensionsTests
{
	[Fact]
	public async Task HasAttribute_WithAttribute_ShouldReturnTrue()
	{
		PropertyInfo type =
			typeof(TestClass).GetProperty(nameof(TestClass.Property1WithAttribute))!;

		bool result = type.HasAttribute<DummyAttribute>();

		await That(result).IsTrue();
	}

	[Fact]
	public async Task HasAttribute_WithInheritedAttribute_ShouldReturnTrue()
	{
		PropertyInfo type =
			typeof(TestClass).GetProperty(nameof(TestClass.PropertyWithAttributeInBaseClass))!;

		bool result = type.HasAttribute<DummyAttribute>();

		await That(result).IsTrue();
	}

	[Fact]
	public async Task HasAttribute_WithoutAttribute_ShouldReturnFalse()
	{
		PropertyInfo type =
			typeof(TestClass).GetProperty(nameof(TestClass.Property2WithoutAttribute))!;

		bool result = type.HasAttribute<DummyAttribute>();

		await That(result).IsFalse();
	}

	[Fact]
	public async Task HasAttribute_WithPredicate_ShouldReturnPredicateResult()
	{
		PropertyInfo type =
			typeof(TestClass).GetProperty(nameof(TestClass.Property1WithAttribute))!;

		bool result1 = type.HasAttribute<DummyAttribute>(d => d.Value == 1);
		bool result2 = type.HasAttribute<DummyAttribute>(d => d.Value == 2);

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
}
