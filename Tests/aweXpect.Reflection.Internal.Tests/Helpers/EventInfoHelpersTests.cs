using System.Reflection;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Internal.Tests.Helpers;

#pragma warning disable CA2263 // Prefer generic overload when type is known

public sealed class EventInfoHelpersTests
{
	[Test]
	public async Task HasAttribute_WithAttribute_ShouldReturnTrue()
	{
		EventInfo eventInfo = typeof(TestClass).GetEvent(nameof(TestClass.Event1WithAttribute))!;

		bool result = eventInfo.HasAttribute<DummyAttribute>();

		await That(result).IsTrue();
	}

	[Test]
	public async Task HasAttribute_WithAttributeType_WithoutPredicate_ShouldReturnTrue()
	{
		EventInfo eventInfo = typeof(TestClass).GetEvent(nameof(TestClass.Event1WithAttribute))!;

		bool result = eventInfo.HasAttribute(typeof(DummyAttribute));

		await That(result).IsTrue();
	}

	[Test]
	public async Task HasAttribute_WithAttributeType_WithPredicate_ShouldReturnPredicateResult()
	{
		EventInfo eventInfo = typeof(TestClass).GetEvent(nameof(TestClass.Event1WithAttribute))!;

		bool result1 = eventInfo.HasAttribute(typeof(DummyAttribute), a => ((DummyAttribute)a).Value == 1);
		bool result2 = eventInfo.HasAttribute(typeof(DummyAttribute), a => ((DummyAttribute)a).Value == 2);

		await That(result1).IsTrue();
		await That(result2).IsFalse();
	}

	[Test]
	public async Task HasAttribute_WithInheritedAttribute_ShouldReturnTrue()
	{
		EventInfo eventInfo =
			typeof(TestClass).GetEvent(nameof(TestClass.EventWithAttributeInBaseClass))!;

		bool result = eventInfo.HasAttribute<DummyAttribute>();

		await That(result).IsTrue();
	}

	[Test]
	public async Task HasAttribute_WithoutAttribute_ShouldReturnFalse()
	{
		EventInfo eventInfo = typeof(TestClass).GetEvent(nameof(TestClass.Event2WithoutAttribute))!;

		bool result = eventInfo.HasAttribute<DummyAttribute>();

		await That(result).IsFalse();
	}

	[Test]
	public async Task HasAttribute_WithPredicate_ShouldReturnPredicateResult()
	{
		EventInfo eventInfo = typeof(TestClass).GetEvent(nameof(TestClass.Event1WithAttribute))!;

		bool result1 = eventInfo.HasAttribute<DummyAttribute>(d => d.Value == 1);
		bool result2 = eventInfo.HasAttribute<DummyAttribute>(d => d.Value == 2);

		await That(result1).IsTrue();
		await That(result2).IsFalse();
	}

	[Test]
	public async Task IsOverride_WhenAddMethodIsNull_ShouldReturnFalse()
	{
		EventInfo eventInfo = new EventInfoWithoutAddMethod();

		bool result = eventInfo.IsOverride();

		await That(result).IsFalse();
	}

	[Test]
	public async Task IsOverride_WhenEventInfoIsNull_ShouldReturnFalse()
	{
		EventInfo? eventInfo = null;

		bool result = eventInfo.IsOverride();

		await That(result).IsFalse();
	}

	[Test]
	[Arguments(nameof(DerivedEventClass.AbstractEvent), true)]
	[Arguments(nameof(DerivedEventClass.VirtualEvent), true)]
	[Arguments(nameof(DerivedEventClass.DerivedPlainEvent), false)]
	public async Task IsOverride_WhenEventIsDeclaredOnDerivedClass_ShouldReturnExpectedResult(
		string eventName, bool expectedResult)
	{
		EventInfo eventInfo = typeof(DerivedEventClass).GetEvent(eventName)!;

		bool result = eventInfo.IsOverride();

		await That(result).IsEqualTo(expectedResult);
	}

	[Test]
	[Arguments(nameof(BaseEventClass.AbstractEvent))]
	[Arguments(nameof(BaseEventClass.VirtualEvent))]
	public async Task IsOverride_WhenEventIsDeclaredOnBaseClass_ShouldReturnFalse(string eventName)
	{
		EventInfo eventInfo = typeof(BaseEventClass).GetEvent(eventName)!;

		bool result = eventInfo.IsOverride();

		await That(result).IsFalse();
	}

	[Test]
	public async Task IsReallyVirtual_WhenAddMethodIsNull_ShouldReturnFalse()
	{
		EventInfo eventInfo = new EventInfoWithoutAddMethod();

		bool result = eventInfo.IsReallyVirtual();

		await That(result).IsFalse();
	}

	[Test]
	public async Task IsReallyVirtual_WhenEventInfoIsNull_ShouldReturnFalse()
	{
		EventInfo? eventInfo = null;

		bool result = eventInfo.IsReallyVirtual();

		await That(result).IsFalse();
	}

	[Test]
	[Arguments(nameof(BaseEventClass.AbstractEvent), true)]
	[Arguments(nameof(BaseEventClass.VirtualEvent), true)]
	[Arguments(nameof(BaseEventClass.PlainEvent), false)]
	[Arguments(nameof(BaseEventClass.StaticEvent), false)]
	public async Task IsReallyVirtual_WhenEventIsDeclaredOnBaseClass_ShouldReturnExpectedResult(
		string eventName, bool expectedResult)
	{
		EventInfo eventInfo = typeof(BaseEventClass).GetEvent(eventName)!;

		bool result = eventInfo.IsReallyVirtual();

		await That(result).IsEqualTo(expectedResult);
	}

	[Test]
	[Arguments(nameof(DerivedEventClass.AbstractEvent))]
	[Arguments(nameof(DerivedEventClass.VirtualEvent))]
	public async Task IsReallyVirtual_WhenEventIsDeclaredOnDerivedClass_ShouldReturnTrue(string eventName)
	{
		EventInfo eventInfo = typeof(DerivedEventClass).GetEvent(eventName)!;

		bool result = eventInfo.IsReallyVirtual();

		await That(result).IsTrue();
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

	private abstract class BaseEventClass
	{
		public abstract event Dummy AbstractEvent;
		public virtual event Dummy VirtualEvent;
		public event Dummy PlainEvent;
		public static event Dummy StaticEvent;
	}

	private class DerivedEventClass : BaseEventClass
	{
		public override event Dummy AbstractEvent;
		public sealed override event Dummy VirtualEvent;
		public event Dummy DerivedPlainEvent;
	}

	private sealed class EventInfoWithoutAddMethod : EventInfo
	{
		public override EventAttributes Attributes => EventAttributes.None;
		public override Type? DeclaringType => typeof(EventInfoHelpersTests);
		public override string Name => nameof(EventInfoWithoutAddMethod);
		public override Type? ReflectedType => typeof(EventInfoHelpersTests);

		public override MethodInfo? GetAddMethod(bool nonPublic) => null;

		public override object[] GetCustomAttributes(bool inherit) => [];

		public override object[] GetCustomAttributes(Type attributeType, bool inherit) => [];

		public override MethodInfo? GetRaiseMethod(bool nonPublic) => null;

		public override MethodInfo? GetRemoveMethod(bool nonPublic) => null;

		public override bool IsDefined(Type attributeType, bool inherit) => false;
	}
#pragma warning restore CS0067
#pragma warning restore CS8618
}

#pragma warning restore CA2263 // Prefer generic overload when type is known
