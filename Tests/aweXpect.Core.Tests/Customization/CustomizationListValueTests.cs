using System.Collections.Generic;
using aweXpect.Customization;

namespace aweXpect.Core.Tests.Customization;

public sealed class CustomizationListValueTests
{
	private static readonly string Key = $"aweXpect.Core.Tests.{Guid.NewGuid()}";

	[Test]
	public async Task Constructor_WhenCustomizationIsNull_ShouldThrowArgumentNullException()
	{
		void Act() => _ = new CustomizationListValue<string>(null!, Key, []);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("customization").And
			.WithMessage("The 'customization' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task Constructor_WhenKeyIsNull_ShouldThrowArgumentNullException()
	{
		void Act() => _ = new CustomizationListValue<string>(new AwexpectCustomization(), null!, []);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("key").And
			.WithMessage("The 'key' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task Get_WhenNotSet_ShouldReturnDefaultValue()
	{
		IReadOnlyList<string> defaultValue = Array.AsReadOnly(["foo",]);
		CustomizationListValue<string> sut = new(new AwexpectCustomization(), Key, defaultValue);

		IReadOnlyList<string> result = sut.Get();

		await That(result).IsSameAs(defaultValue);
	}

	[Test]
	public async Task Set_ShouldStoreAReadOnlyCopy()
	{
		List<string> value = ["foo",];
		CustomizationListValue<string> sut = new(new AwexpectCustomization(), Key, []);

		using CustomizationLifetime _ = sut.Set(value);
		IReadOnlyList<string> result = sut.Get();

		await That(result).IsNotSameAs(value);
		await That(((ICollection<string>)result).IsReadOnly).IsTrue()
			.Because("a change of the returned list would bypass the scoping of the setting");
	}

	[Test]
	public async Task Set_ShouldStoreValueUntilLifetimeIsDisposed()
	{
		CustomizationListValue<string> sut = new(new AwexpectCustomization(), Key, ["foo",]);

		using (sut.Set(["bar",]))
		{
			await That(sut.Get()).IsEqualTo(["bar",]);
		}

		await That(sut.Get()).IsEqualTo(["foo",])
			.Because("disposing the lifetime restores the previous value");
	}

	[Test]
	public async Task Set_WhenNull_ShouldThrowArgumentNullExceptionAndKeepValue()
	{
		CustomizationListValue<string> sut = new(new AwexpectCustomization(), Key, ["foo",]);

		void Act() => sut.Set(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("value").And
			.WithMessage("The 'value' cannot be null.").AsPrefix();
		await That(sut.Get()).IsEqualTo(["foo",])
			.Because("a stored null would be returned as null instead of the default list");
	}

	[Test]
	public async Task Set_WhenTheListIsChangedAfterSetting_ShouldKeepTheSetting()
	{
		List<string> value = ["foo",];
		CustomizationListValue<string> sut = new(new AwexpectCustomization(), Key, []);

		using CustomizationLifetime _ = sut.Set(value);
		value.Add("bar");

		await That(sut.Get()).IsEqualTo(["foo",])
			.Because("changing the list afterwards must not change the setting");
	}

	[Test]
	public async Task Set_WhenValidationFails_ShouldThrowAndKeepValue()
	{
		CustomizationListValue<string> sut = new(new AwexpectCustomization(), Key, ["foo",], value =>
		{
			if (value.Count == 0)
			{
				throw new ArgumentException("The list must not be empty.", nameof(value));
			}
		});

		void Act() => sut.Set([]);

		await That(Act).Throws<ArgumentException>()
			.WithMessage("The list must not be empty.").AsPrefix();
		await That(sut.Get()).IsEqualTo(["foo",])
			.Because("an invalid list must not be stored");
	}
}
