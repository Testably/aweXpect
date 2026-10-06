using aweXpect.Core.Helpers;

namespace aweXpect.Core.Tests.Core.Helpers;

public class ToleranceHelpersTests
{
	[Test]
	public async Task Format_Char_ShouldShowTheCodePoint()
	{
		string result = ToleranceHelpers.Format('a');

		await That(result).IsEqualTo(" ± 97")
			.Because("a char tolerance is a distance between code points");
	}

	[Test]
	public async Task Format_Double_ShouldShowTheValue()
	{
		string result = ToleranceHelpers.Format(0.25);

		await That(result).IsEqualTo(" ± 0.25");
	}

	[Test]
	public async Task Format_TimeSpan_ShouldShowTheFormattedDuration()
	{
		string result = ToleranceHelpers.Format(TimeSpan.FromSeconds(2));

		await That(result).IsEqualTo(" ± 0:02");
	}

	[Test]
	public async Task ThrowIfInvalid_WhenDecimalIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		void Act() => ToleranceHelpers.ThrowIfInvalid(-0.1m);

		await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be negative.").AsPrefix();
	}

	[Test]
	public async Task ThrowIfInvalid_WhenDoubleIsNaN_ShouldThrowArgumentOutOfRangeException()
	{
		void Act() => ToleranceHelpers.ThrowIfInvalid(double.NaN);

		await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be NaN.").AsPrefix();
	}

	[Test]
	public async Task ThrowIfInvalid_WhenFloatIsNaN_ShouldThrowArgumentOutOfRangeException()
	{
		void Act() => ToleranceHelpers.ThrowIfInvalid(float.NaN);

		await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be NaN.").AsPrefix();
	}

	[Test]
	public async Task ThrowIfInvalid_WhenIntIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		void Act() => ToleranceHelpers.ThrowIfInvalid(-1);

		await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be negative.").AsPrefix();
	}

	[Test]
	public async Task ThrowIfInvalid_WhenTimeSpanIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		void Act() => ToleranceHelpers.ThrowIfInvalid(TimeSpan.FromSeconds(-1));

		await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be negative.").AsPrefix();
	}

	[Test]
	public async Task ThrowIfInvalid_WhenZero_ShouldNotThrow()
	{
		void Act()
		{
			ToleranceHelpers.ThrowIfInvalid(0);
			ToleranceHelpers.ThrowIfInvalid(0.0);
			ToleranceHelpers.ThrowIfInvalid(TimeSpan.Zero);
		}

		await That(Act).DoesNotThrow();
	}
}
