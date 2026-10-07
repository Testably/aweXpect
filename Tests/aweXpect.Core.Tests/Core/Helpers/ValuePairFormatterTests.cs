using aweXpect.Core.Helpers;

namespace aweXpect.Core.Tests.Core.Helpers;

public sealed class ValuePairFormatterTests
{
	[Test]
	public async Task AppendRuntimeType_ForNull_ShouldKeepTheText()
	{
		string result = ValuePairFormatter.AppendRuntimeType("<null>", null);

		await That(result).IsEqualTo("<null>")
			.Because("null has no runtime type");
	}

	[Test]
	public async Task Format_WhenBothAreNull_ShouldKeepTheTexts()
	{
		(string actual, string expected) = ValuePairFormatter.Format(null, null);

		await That(actual).IsEqualTo("<null>");
		await That(expected).IsEqualTo("<null>");
	}

	[Test]
	public async Task Format_WhenBothAreTheSameString_ShouldKeepTheTexts()
	{
		(string actual, string expected) = ValuePairFormatter.Format("foo", "foo");

		await That(actual).IsEqualTo("\"foo\"");
		await That(expected).IsEqualTo("\"foo\"")
			.Because("equal strings have no difference to show them from");
	}

	[Test]
	public async Task Format_WhenTheStringsWouldBeShownFromWithinASurrogatePair_ShouldStartBeforeThePair()
	{
		string prefix = new string('a', 99) + "\U0001F600" + new string('b', 9);

		(string actual, string expected) = ValuePairFormatter.Format(prefix + "x", prefix + "y");

		await That(actual).IsEqualTo("\"…\U0001F600bbbbbbbbbx\"");
		await That(expected).IsEqualTo("\"…\U0001F600bbbbbbbbby\"")
			.Because("the shown text must not start with the second half of a surrogate pair");
	}
}
