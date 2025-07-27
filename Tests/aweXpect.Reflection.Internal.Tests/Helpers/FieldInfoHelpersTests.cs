using System.Reflection;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Internal.Tests.Helpers;

public sealed class FieldInfoHelpersTests
{
	[Fact]
	public async Task HasAttribute_WithAttribute_ShouldReturnTrue()
	{
		FieldInfo type = typeof(TestClass).GetField(nameof(TestClass.Field1WithAttribute))!;

		bool result = type.HasAttribute<DummyAttribute>();

		await That(result).IsTrue();
	}

	[Fact]
	public async Task HasAttribute_WithoutAttribute_ShouldReturnFalse()
	{
		FieldInfo type = typeof(TestClass).GetField(nameof(TestClass.Field2WithoutAttribute))!;

		bool result = type.HasAttribute<DummyAttribute>();

		await That(result).IsFalse();
	}

	[Fact]
	public async Task HasAttribute_WithPredicate_ShouldReturnPredicateResult()
	{
		FieldInfo type = typeof(TestClass).GetField(nameof(TestClass.Field1WithAttribute))!;

		bool result1 = type.HasAttribute<DummyAttribute>(d => d.Value == 1);
		bool result2 = type.HasAttribute<DummyAttribute>(d => d.Value == 2);

		await That(result1).IsTrue();
		await That(result2).IsFalse();
	}

	[AttributeUsage(AttributeTargets.Field)]
	private class DummyAttribute : Attribute
	{
		public DummyAttribute(int value)
		{
			Value = value;
		}

		public int Value { get; }
	}

#pragma warning disable CS0649
	private class TestClass
	{
		[Dummy(1)] public int Field1WithAttribute = 1;

		public int Field2WithoutAttribute = 1;
	}
#pragma warning restore CS0649
}
