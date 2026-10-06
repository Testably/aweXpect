using System.Linq;
using aweXpect.Customization;

namespace aweXpect.Core.Tests.Customization;

public sealed class CustomizeFormattingTests
{
	[Test]
	public async Task Formatting_ShouldReturnSameInstance()
	{
		AwexpectCustomization.FormattingCustomization formatting1 = Customize.aweXpect.Formatting();
		AwexpectCustomization.FormattingCustomization formatting2 = Customize.aweXpect.Formatting();

		await That(formatting1).IsSameAs(formatting2);
	}

	[Test]
	public async Task MaximumNumberOfCollectionItems_ShouldBeUsedInFormatter()
	{
		int[] items = Enumerable.Range(1, 6).ToArray();
		using (IDisposable _ = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Set(3))
		{
			await That(ValueFormatters.Format(Formatter, items)).IsEqualTo("[1, 2, 3, (… and 3 more)]");
		}

		using (IDisposable _ = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Set(5))
		{
			await That(ValueFormatters.Format(Formatter, items)).IsEqualTo("[1, 2, 3, 4, 5, (… and 1 more)]");
		}

		await That(ValueFormatters.Format(Formatter, items)).IsEqualTo("[1, 2, 3, 4, 5, 6]");
	}

	[Test]
	[Arguments(0)]
	[Arguments(-1)]
	public async Task MaximumNumberOfCollectionItems_WhenNotPositive_ShouldThrowArgumentOutOfRangeException(int count)
	{
		void Act() => Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Set(count);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("count").And
			.WithMessage("The maximum number of collection items must be positive.").AsPrefix()
			.Because("the maximum also bounds how many items some expectations read, so zero would read them all");
		await That(Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get()).IsEqualTo(10);
	}

	[Test]
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

	[Test]
	public async Task MaximumStringLength_WhenNegative_ShouldThrowArgumentOutOfRangeException()
	{
		void Act() => Customize.aweXpect.Formatting().MaximumStringLength.Set(-1);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("length").And
			.WithMessage("The maximum string length must not be negative.").AsPrefix();
		await That(Customize.aweXpect.Formatting().MaximumStringLength.Get()).IsEqualTo(100);
	}

	[Test]
	public async Task MaximumStringLength_WhenZero_ShouldOnlyShowTheEllipsis()
	{
		using (IDisposable _ = Customize.aweXpect.Formatting().MaximumStringLength.Set(0))
		{
			await That(Formatter.Format("foo")).IsEqualTo("\"…\"");
		}
	}

	[Test]
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
				             but it was "this is some text with lots…", which differs at index 8:
				                        ↓ (actual)
				               "this is some text with…"
				               "this is another text with…"
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
			             but it was "this is some text with lots…", which differs at index 8:
			                        ↓ (actual)
			               "this is some text with lots of words after the first difference to…"
			               "this is another text with lots of words after the first difference…"
			                        ↑ (expected)

			             Actual:
			             this is some text with lots of words after the first difference to verify the customization setting
			             
			             Expected:
			             this is another text with lots of words after the first difference to verify the customization setting
			             """);
	}

	[Test]
	public async Task MinimumNumberOfCharactersAfterStringDifference_WhenNegative_ShouldThrowArgumentOutOfRangeException()
	{
		void Act() => Customize.aweXpect.Formatting().MinimumNumberOfCharactersAfterStringDifference.Set(-1);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("count").And
			.WithMessage("The minimum number of characters after the string difference must not be negative.")
			.AsPrefix();
		await That(Customize.aweXpect.Formatting().MinimumNumberOfCharactersAfterStringDifference.Get()).IsEqualTo(45);
	}
}
