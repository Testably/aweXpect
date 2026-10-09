using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed class NonNullableResultTests
{
	private const string Reason = "the awaited result is the verified subject, which cannot be null";

	[Fact]
	public async Task AssemblyHas_ShouldReturnNonNullableAssembly()
	{
		Assembly? subject = typeof(NonNullableResultTests).Assembly;

		Assembly result = await That(subject).Has<AssemblyTitleAttribute>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task ConstructorHas_ShouldReturnNonNullableConstructor()
	{
		ConstructorInfo? subject = typeof(TestClass).GetConstructor(Type.EmptyTypes);

		ConstructorInfo result = await That(subject).Has<TestAttribute>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task EventHas_ShouldReturnNonNullableEvent()
	{
		EventInfo? subject = typeof(TestClass).GetEvent(nameof(TestClass.Event));

		EventInfo result = await That(subject).Has<TestAttribute>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task EventIsOfExactType_ShouldReturnNonNullableEvent()
	{
		EventInfo? subject = typeof(TestClass).GetEvent(nameof(TestClass.Event));

		EventInfo result = await That(subject).IsOfExactType<EventHandler>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task EventIsOfType_ShouldReturnNonNullableEvent()
	{
		EventInfo? subject = typeof(TestClass).GetEvent(nameof(TestClass.Event));

		EventInfo result = await That(subject).IsOfType<EventHandler>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task FieldHas_ShouldReturnNonNullableField()
	{
		FieldInfo? subject = typeof(TestClass).GetField(nameof(TestClass.Field));

		FieldInfo result = await That(subject).Has<TestAttribute>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task FieldIsOfExactType_ShouldReturnNonNullableField()
	{
		FieldInfo? subject = typeof(TestClass).GetField(nameof(TestClass.Field));

		FieldInfo result = await That(subject).IsOfExactType<Uri>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task FieldIsOfType_ShouldReturnNonNullableField()
	{
		FieldInfo? subject = typeof(TestClass).GetField(nameof(TestClass.Field));

		FieldInfo result = await That(subject).IsOfType<Uri>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MemberDoesNotHaveName_ShouldReturnNonNullableMember()
	{
		PropertyInfo? subject = typeof(TestClass).GetProperty(nameof(TestClass.Property));

		PropertyInfo result = await That(subject).DoesNotHaveName("Foo");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MemberHasName_ShouldReturnNonNullableMember()
	{
		PropertyInfo? subject = typeof(TestClass).GetProperty(nameof(TestClass.Property));

		PropertyInfo result = await That(subject).HasName(nameof(TestClass.Property));

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MemberIsNotObsolete_ShouldReturnNonNullableMember()
	{
		PropertyInfo? subject = typeof(TestClass).GetProperty(nameof(TestClass.Property));

		PropertyInfo result = await That(subject).IsNotObsolete();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MemberIsPublic_ShouldReturnNonNullableMember()
	{
		PropertyInfo? subject = typeof(TestClass).GetProperty(nameof(TestClass.Property));

		PropertyInfo result = await That(subject).IsPublic();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MethodHas_ShouldReturnNonNullableMethod()
	{
		MethodInfo? subject = typeof(TestClass).GetMethod(nameof(TestClass.Method));

		MethodInfo result = await That(subject).Has<TestAttribute>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MethodReturns_ShouldReturnNonNullableMethod()
	{
		MethodInfo? subject = typeof(TestClass).GetMethod(nameof(TestClass.Method));

		MethodInfo result = await That(subject).Returns<Uri>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MethodReturnsExactly_ShouldReturnNonNullableMethod()
	{
		MethodInfo? subject = typeof(TestClass).GetMethod(nameof(TestClass.Method));

		MethodInfo result = await That(subject).ReturnsExactly<Uri>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MethodReturnsVoid_ShouldReturnNonNullableMethod()
	{
		MethodInfo? subject = typeof(TestClass).GetMethod(nameof(TestClass.VoidMethod));

		MethodInfo result = await That(subject).ReturnsVoid();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task PropertyHas_ShouldReturnNonNullableProperty()
	{
		PropertyInfo? subject = typeof(TestClass).GetProperty(nameof(TestClass.Property));

		PropertyInfo result = await That(subject).Has<TestAttribute>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task PropertyIsOfExactType_ShouldReturnNonNullableProperty()
	{
		PropertyInfo? subject = typeof(TestClass).GetProperty(nameof(TestClass.Property));

		PropertyInfo result = await That(subject).IsOfExactType<Uri>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task PropertyIsOfType_ShouldReturnNonNullableProperty()
	{
		PropertyInfo? subject = typeof(TestClass).GetProperty(nameof(TestClass.Property));

		PropertyInfo result = await That(subject).IsOfType<Uri>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeDependsOn_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(TestClass);

		Type result = await That(subject).DependsOn<Uri>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeDoesNotDependOn_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(TestClass);

		Type result = await That(subject).DoesNotDependOn<NonNullableResultTests>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeHas_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(TestClass);

		Type result = await That(subject).Has<TestAttribute>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[AttributeUsage(AttributeTargets.All)]
	private sealed class TestAttribute : Attribute;

	[Test]
	private sealed class TestClass
	{
		[Test] public Uri Field = new("https://awexpect.com");

		[Test]
		public TestClass()
		{
		}

		[Test] public Uri? Property { get; set; }

		[Test] public event EventHandler? Event;

		[Test]
		public Uri? Method() => Field;

		public void VoidMethod() => Event?.Invoke(this, EventArgs.Empty);
	}
}
