namespace aweXpect.Core.Tests.Core;

public class TimesTests
{
	[Test]
	[AutoArguments]
	public async Task ExplicitConstructor_ShouldSetValueProperty(int value)
	{
		Times times = new(value);

		await That(times.Value).IsEqualTo(value);
	}

	[Test]
	[AutoArguments]
	public async Task ExtensionMethod_ShouldSetValueProperty(int value)
	{
		Times times = value.Times();

		await That(times.Value).IsEqualTo(value);
	}

	[Test]
	public async Task ImplicitConversion_ShouldWorkAsExpected()
	{
		int expectedValue = 5;

		Times times = expectedValue;
		int actualValue = times;

		await That(times.Value).IsEqualTo(expectedValue);
		await That(actualValue).IsEqualTo(expectedValue);
	}

	[Test]
	[AutoArguments]
	public async Task ImplicitOperator_ShouldSetValueProperty(int value)
	{
		Times times = value;

		await That(times.Value).IsEqualTo(value);
	}
}
