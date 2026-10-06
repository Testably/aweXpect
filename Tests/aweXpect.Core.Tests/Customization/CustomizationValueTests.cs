using aweXpect.Customization;

namespace aweXpect.Core.Tests.Customization;

public sealed class CustomizationValueTests
{
	private static readonly string Key = $"aweXpect.Core.Tests.{Guid.NewGuid()}";

	[Test]
	public async Task Constructor_WhenCustomizationIsNull_ShouldThrowArgumentNullException()
	{
		void Act() => _ = new CustomizationValue<int>(null!, Key, 42);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("customization").And
			.WithMessage("The 'customization' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task Constructor_WhenKeyIsNull_ShouldThrowArgumentNullException()
	{
		void Act() => _ = new CustomizationValue<int>(new AwexpectCustomization(), null!, 42);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("key").And
			.WithMessage("The 'key' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task Get_WhenNotSet_ShouldReturnDefaultValue()
	{
		CustomizationValue<int> sut = new(new AwexpectCustomization(), Key, 42);

		int result = sut.Get();

		await That(result).IsEqualTo(42);
	}

	[Test]
	public async Task Set_OnGlobal_ShouldApplyToTheCurrentAsyncFlow()
	{
		AwexpectCustomization customization = new();
		CustomizationValue<int> sut = new(customization.Global, Key, 42);
		CustomizationValue<int> other = new(customization, Key, 42);

		using (sut.Set(43))
		{
			await That(other.Get()).IsEqualTo(43)
				.Because("a value set on the global customization applies to all async flows");
		}

		await That(other.Get()).IsEqualTo(42);
	}

	[Test]
	public async Task Set_ShouldStoreValueUntilLifetimeIsDisposed()
	{
		CustomizationValue<int> sut = new(new AwexpectCustomization(), Key, 42);

		using (sut.Set(43))
		{
			await That(sut.Get()).IsEqualTo(43);
		}

		await That(sut.Get()).IsEqualTo(42)
			.Because("disposing the lifetime restores the previous value");
	}

	[Test]
	public async Task Set_WhenValidationFails_ShouldThrowAndKeepValue()
	{
		CustomizationValue<int> sut = new(new AwexpectCustomization(), Key, 42, value =>
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(value), "The value must not be negative.");
			}
		});

		void Act() => sut.Set(-1);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("The value must not be negative.").AsPrefix();
		await That(sut.Get()).IsEqualTo(42)
			.Because("an invalid value must not be stored");
	}

	[Test]
	public async Task Set_WhenValidationPasses_ShouldStoreValue()
	{
		int? validatedValue = null;
		CustomizationValue<int> sut = new(new AwexpectCustomization(), Key, 42, value => validatedValue = value);

		using (sut.Set(43))
		{
			await That(sut.Get()).IsEqualTo(43);
		}

		await That(validatedValue).IsEqualTo(43)
			.Because("the value must be validated before it is stored");
	}
}
