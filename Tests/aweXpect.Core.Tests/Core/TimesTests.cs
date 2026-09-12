namespace aweXpect.Core.Tests.Core;

public class TimesTests
{
	[Theory]
	[AutoData]
	public async Task ExplicitConstructor_ShouldSetValueProperty(int value)
	{
		Times times = new(value);

		await That(times.Value).IsEqualTo(value);
	}

	[Theory]
	[AutoData]
	public async Task ExtensionMethod_ShouldSetValueProperty(int value)
	{
		Times times = value.Times();

		await That(times.Value).IsEqualTo(value);
	}

	[Fact]
	public async Task ImplicitConversion_ShouldWorkAsExpected()
	{
		int expectedValue = 5;

		Times times = expectedValue;
		int actualValue = times;

		await That(times.Value).IsEqualTo(expectedValue);
		await That(actualValue).IsEqualTo(expectedValue);
	}

	[Theory]
	[AutoData]
	public async Task ImplicitOperator_ShouldSetValueProperty(int value)
	{
		Times times = value;

		await That(times.Value).IsEqualTo(value);
	}
}
