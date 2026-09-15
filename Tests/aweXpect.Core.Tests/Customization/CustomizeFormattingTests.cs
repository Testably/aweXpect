using System.Linq;
using aweXpect.Customization;

namespace aweXpect.Core.Tests.Customization;

public sealed class CustomizeFormattingTests
{
	[Fact]
	public async Task Formatting_ShouldReturnSameInstance()
	{
		AwexpectCustomization.FormattingCustomization formatting1 = Customize.aweXpect.Formatting();
		AwexpectCustomization.FormattingCustomization formatting2 = Customize.aweXpect.Formatting();

		await That(formatting1).IsSameAs(formatting2);
	}

	[Fact]
	public async Task MaximumNumberOfCollectionItems_ShouldBeUsedInFormatter()
	{
		int[] items = Enumerable.Range(1, 6).ToArray();
		using (IDisposable _ = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Set(3))
		{
			await That(ValueFormatters.Format(Formatter, items)).IsEqualTo("[1, 2, 3, …]");
		}

		using (IDisposable _ = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Set(5))
		{
			await That(ValueFormatters.Format(Formatter, items)).IsEqualTo("[1, 2, 3, 4, 5, …]");
		}

		await That(ValueFormatters.Format(Formatter, items)).IsEqualTo("[1, 2, 3, 4, 5, 6]");
	}

	[Fact]
	public async Task MaximumStringLength_ShouldBeUsedInFormatter()
	{
		string stringWith100Chars =
			"Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt util";
		using (IDisposable _ = Customize.aweXpect.Formatting().MaximumStringLength.Set(6))
		{
			await That(Formatter.Format(stringWith100Chars)).IsEqualTo("\"Lorem …\"");
		}

		using (IDisposable _ = Customize.aweXpect.Formatting().MaximumStringLength.Set(99))
		{
			await That(Formatter.Format(stringWith100Chars)).IsEqualTo(
				"\"Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt uti…\"");
		}

		await That(Formatter.Format(stringWith100Chars)).IsEqualTo($"\"{stringWith100Chars}\"");
	}

	[Fact]
	public async Task MinimumNumberOfCharactersAfterStringDifference_ShouldBeUsedInStringDifference()
	{
		string actual =
			"this is some text with lots of words after the first difference to verify the customization setting";
		string expected =
			"this is another text with lots of words after the first difference to verify the customization setting";

		async Task Act() => await That(actual).IsEqualTo(expected);
		using (IDisposable _ = Customize.aweXpect.Formatting().MinimumNumberOfCharactersAfterStringDifference.Set(3))
		{
			await That(Act).Throws()
				.WithMessage("""
				             Expected that actual
				             is equal to "this is another text with…",
				             but it was "this is some text with lots…" which differs at index 8:
				                        ↓ (actual)
				               "this is some text…"
				               "this is another…"
				                        ↑ (expected)

				             Actual:
				             this is some text with lots of words after the first difference to verify the customization setting
				             
				             Expected:
				             this is another text with lots of words after the first difference to verify the customization setting
				             """);
		}

		await That(Act).Throws()
			.WithMessage("""
			             Expected that actual
			             is equal to "this is another text with…",
			             but it was "this is some text with lots…" which differs at index 8:
			                        ↓ (actual)
			               "this is some text with lots of words after the first…"
			               "this is another text with lots of words after the first…"
			                        ↑ (expected)

			             Actual:
			             this is some text with lots of words after the first difference to verify the customization setting
			             
			             Expected:
			             this is another text with lots of words after the first difference to verify the customization setting
			             """);
	}
}
