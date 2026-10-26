using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class QuantifierSlotExtensionsTests
{
	[Test]
	[Arguments("Never", "never", "AtLeast cannot be combined with Never.")]
	[Arguments("Once", "exactly once", "AtLeast cannot be combined with Once.")]
	[Arguments("Twice", "exactly twice", "AtLeast cannot be combined with Twice.")]
	public async Task ExactOption_ShouldSetTheCountAndNameTheOption(string option, string expectedText,
		string expectedMessage)
	{
		OptionsSlot<Quantifier> sut = new(Quantifier.Default);

		OptionsSlot<Quantifier> result = option switch
		{
			"Never" => sut.Never(),
			"Once" => sut.Once(),
			_ => sut.Twice(),
		};

		void Act() => sut.AtLeast(1);

		await That(result).IsSameAs(sut);
		await That(sut.Value.ToString()).IsEqualTo(expectedText);
		await That(Act).Throws<InvalidOperationException>()
			.WithMessage(expectedMessage)
			.Because("the conflict names the option like the user-facing option of the same name does");
	}
}
