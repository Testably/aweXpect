using System.Reflection;
using aweXpect.Reflection.Extensions;

namespace aweXpect.Reflection.Internal.Tests.Extensions;

public sealed class EventInfoExtensionsTests
{
	[Fact]
	public async Task HasAttribute_WithAttribute_ShouldReturnTrue()
	{
		EventInfo type = typeof(TestClass).GetEvent(nameof(TestClass.Event1WithAttribute))!;

		bool result = type.HasAttribute<DummyAttribute>();

		await That(result).IsTrue();
	}

	[Fact]
	public async Task HasAttribute_WithInheritedAttribute_ShouldReturnTrue()
	{
		EventInfo type =
			typeof(TestClass).GetEvent(nameof(TestClass.EventWithAttributeInBaseClass))!;

		bool result = type.HasAttribute<DummyAttribute>();

		await That(result).IsTrue();
	}

	[Fact]
	public async Task HasAttribute_WithoutAttribute_ShouldReturnFalse()
	{
		EventInfo type = typeof(TestClass).GetEvent(nameof(TestClass.Event2WithoutAttribute))!;

		bool result = type.HasAttribute<DummyAttribute>();

		await That(result).IsFalse();
	}

	[Fact]
	public async Task HasAttribute_WithPredicate_ShouldReturnPredicateResult()
	{
		EventInfo type = typeof(TestClass).GetEvent(nameof(TestClass.Event1WithAttribute))!;

		bool result1 = type.HasAttribute<DummyAttribute>(d => d.Value == 1);
		bool result2 = type.HasAttribute<DummyAttribute>(d => d.Value == 2);

		await That(result1).IsTrue();
		await That(result2).IsFalse();
	}

	[AttributeUsage(AttributeTargets.Event)]
	private class DummyAttribute : Attribute
	{
		public DummyAttribute(int value)
		{
			Value = value;
		}

		public int Value { get; }
	}
#pragma warning disable CS8618
#pragma warning disable CS0067
	public delegate void Dummy();

	private class TestClass : TestClassBase
	{
		[Dummy(1)] public event Dummy Event1WithAttribute;

		public event Dummy Event2WithoutAttribute;

		public override event Dummy EventWithAttributeInBaseClass;
	}

	private class TestClassBase
	{
		[Dummy(1)] public virtual event Dummy EventWithAttributeInBaseClass;
	}
#pragma warning restore CS0067
#pragma warning restore CS8618
}
