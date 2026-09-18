namespace aweXpect.Core.Tests.Results;

public class AndOrWhoseResultTests
{
	[Fact]
	public async Task MultipleWhose_ShouldAllowChaining()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.Value1, f => f.IsTrue())
				.AndWhose(f => f.Value2, f => f.IsTrue())
				.And.IsSameAs(sut);

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is type AndOrWhoseResultTests.MyClass whose Value1 is True and whose Value2 is True and refers to AndOrWhoseResultTests.MyClass {
			                 Value1 = False,
			                 Value2 = False
			               },
			             but Value1 was False and Value2 was False
			             """);
	}

	[Fact]
	public async Task Whose_WithNestedMemberPath_ShouldOmitLeadingDot()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.Value1.ToString().Length, f => f.IsLessThan(5));

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is type AndOrWhoseResultTests.MyClass whose Value1.ToString().Length is less than 5,
			             but Value1.ToString().Length was 5
			             """);
	}

	[Fact]
	public async Task Whose_WithCast_ShouldKeepWholeSelectorBody()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(o => (bool?)o.Value1, f => f.IsEqualTo(true));

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is type AndOrWhoseResultTests.MyClass whose (bool?)o.Value1 is True,
			             but (bool?)o.Value1 was False
			             """);
	}

	[Fact]
	public async Task Whose_WithCastParameter_ShouldKeepWholeSelectorBody()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => ((MyClass)f).Value1, f => f.IsTrue());

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is type AndOrWhoseResultTests.MyClass whose ((MyClass)f).Value1 is True,
			             but ((MyClass)f).Value1 was False
			             """);
	}

	[Fact]
	public async Task Whose_WithIdentity_ShouldRenderIt()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f, f => f.IsNull());

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is type AndOrWhoseResultTests.MyClass whose it is null,
			             but it was AndOrWhoseResultTests.MyClass {
			                 Value1 = False,
			                 Value2 = False
			               }
			             """);
	}

	[Fact]
	public async Task Whose_WithNullConditional_ShouldOmitParameter()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f?.Value1, f => f.IsEqualTo(true));

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is type AndOrWhoseResultTests.MyClass whose Value1 is True,
			             but Value1 was False
			             """);
	}

	[Fact]
	public async Task Whose_WithParenthesizedParameter_ShouldOmitParameter()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose((f) => f.Value1, f => f.IsTrue());

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is type AndOrWhoseResultTests.MyClass whose Value1 is True,
			             but Value1 was False
			             """);
	}

	[Theory]
	[InlineData(true, true, true)]
	[InlineData(true, false, false)]
	[InlineData(false, true, false)]
	[InlineData(false, false, false)]
	public async Task MultipleWhose_ShouldVerifyAll(bool value1, bool value2, bool expectSuccess)
	{
		MyClass sut = new()
		{
			Value1 = value1,
			Value2 = value2,
		};

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.Value1, f => f.IsTrue())
				.AndWhose(f => f.Value2, f => f.IsTrue());

		await That(Act).Throws().OnlyIf(!expectSuccess)
			.WithMessage($"""
			              Expected that sut
			              is type AndOrWhoseResultTests.MyClass whose Value1 is True and whose Value2 is True,
			              but {(value1 ? "" : "Value1 was False")}{(!value1 && !value2 ? " and " : "")}{(value2 ? "" : "Value2 was False")}
			              """);
	}

	[Fact]
	public async Task Whose_WithAsyncMember_ShouldVerifyAwaitedValue()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.GetValue1Async(), f => f.IsTrue());

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is type AndOrWhoseResultTests.MyClass whose GetValue1Async() is True,
			             but GetValue1Async() was False
			             """);
	}

	[Fact]
	public async Task AndWhose_WithAsyncMember_ShouldVerifyAwaitedValue()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.Value2, f => f.IsFalse())
				.AndWhose(f => f.GetValue1Async(), f => f.IsTrue());

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is type AndOrWhoseResultTests.MyClass whose Value2 is False and whose GetValue1Async() is True,
			             but GetValue1Async() was False
			             """);
	}

	[Fact]
	public async Task Whose_WhenAsyncMemberFaults_ShouldPropagateException()
	{
		ThrowingClass sut = new("async member failed");

		async Task Act()
			=> await That(sut).Is<ThrowingClass>()
				.Whose(f => f.FaultedAsync(), f => f.IsTrue());

		await That(Act).ThrowsExactly<InvalidOperationException>()
			.WithMessage("async member failed");
	}

	[Fact]
	public async Task AndWhose_WhenAsyncMemberFaults_ShouldPropagateException()
	{
		ThrowingClass sut = new("async member failed");

		async Task Act()
			=> await That(sut).Is<ThrowingClass>()
				.Whose(f => f.Value, f => f.IsFalse())
				.AndWhose(f => f.FaultedAsync(), f => f.IsTrue());

		await That(Act).ThrowsExactly<InvalidOperationException>()
			.WithMessage("async member failed");
	}

	private sealed class MyClass
	{
		public bool Value1 { get; set; }
		public bool Value2 { get; set; }

		public Task<bool> GetValue1Async() => Task.FromResult(Value1);
	}

	private sealed class ThrowingClass(string message)
	{
		public bool Value { get; set; }

		public async Task<bool> FaultedAsync()
		{
			await Task.Yield();
			throw new InvalidOperationException(message);
		}
	}
}
