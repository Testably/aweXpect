namespace aweXpect.Core.Tests.Formatting;

public class FormattingOptionsTests
{
	[Test]
	public async Task Indented_ShouldDefaultToTwoSpaces()
	{
		FormattingOptions options = FormattingOptions.Indented();

		await That(options.UseLineBreaks).IsTrue();
		await That(options.Indentation).IsEqualTo("  ");
	}

	[Test]
	public async Task Indented_ShouldNotBeASnapshot()
	{
		FormattingOptions options = FormattingOptions.Indented();

		await That(options.IsSnapshot).IsFalse()
			.Because("only the snapshot option lifts the limits of a failure message");
	}

	[Test]
	public async Task Indented_ShouldUseLineBreaks()
	{
		FormattingOptions options = FormattingOptions.Indented("    ");

		await That(options.UseLineBreaks).IsTrue();
		await That(options.Indentation).IsEqualTo("    ");
	}

	[Test]
	public async Task MultipleLines_ShouldUseLineBreaks()
	{
		FormattingOptions options = FormattingOptions.MultipleLines;

		await That(options.UseLineBreaks).IsTrue();
		await That(options.Indentation).IsEmpty();
	}

	[Test]
	public async Task SingleLine_ShouldNotUseLineBreaks()
	{
		FormattingOptions options = FormattingOptions.SingleLine;

		await That(options.UseLineBreaks).IsFalse();
		await That(options.Indentation).IsEmpty();
	}

	[Test]
	public async Task Snapshot_ShouldBeASnapshotOnASingleLine()
	{
		FormattingOptions options = FormattingOptions.Snapshot;

		await That(options.IsSnapshot).IsTrue();
		await That(options.UseLineBreaks).IsFalse();
		await That(options.IncludeType).IsFalse();
		await That(options.Indentation).IsEmpty();
	}
}
