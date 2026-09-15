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
			             is type AndOrWhoseResultTests.MyClass whose .Value1 is True and whose .Value2 is True and refers to AndOrWhoseResultTests.MyClass {
			                 Value1 = False,
			                 Value2 = False
			               },
			             but .Value1 was False and .Value2 was False
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
			              is type AndOrWhoseResultTests.MyClass whose .Value1 is True and whose .Value2 is True,
			              but {(value1 ? "" : ".Value1 was False")}{(!value1 && !value2 ? " and " : "")}{(value2 ? "" : ".Value2 was False")}
			              """);
	}

	private sealed class MyClass
	{
		public bool Value1 { get; set; }
		public bool Value2 { get; set; }
	}
}
