using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Options;

public sealed class QuantifierExtensionsTests
{
	[Test]
	public async Task WhenTheResultCopiesOnOption_ShouldKeepTheQuantifierSetSoFar()
	{
		CopyingResult sut = new(Quantifier.Default);

		void Act() => sut.Once().Twice();

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Twice cannot be combined with Once.");
	}

	[Test]
	public async Task WhenTheResultCopiesOnOption_ShouldReplaceARepeatedOptionOnTheCopy()
	{
		CopyingResult sut = new(Quantifier.Default);
		CopyingResult atLeastTwice = sut.AtLeast(2);

		CopyingResult result = atLeastTwice.AtLeast(3);

		await That(result.Options.ToString()).IsEqualTo("at least 3 times");
		await That(atLeastTwice.Options.ToString()).IsEqualTo("at least twice")
			.Because("the repeated option must be set on a copy of the quantifier");
	}

	[Test]
	[Arguments("AtLeast()", "at least twice")]
	[Arguments("AtLeast(Times)", "at least 3 times")]
	[Arguments("AtMost()", "at most twice")]
	[Arguments("AtMost(Times)", "at most 3 times")]
	[Arguments("Between", "between 1 and 3 times")]
	[Arguments("Exactly", "exactly 3 times")]
	[Arguments("LessThan()", "fewer than twice")]
	[Arguments("LessThan(Times)", "fewer than 3 times")]
	[Arguments("MoreThan()", "more than twice")]
	[Arguments("MoreThan(Times)", "more than 3 times")]
	[Arguments("Never", "never")]
	[Arguments("Once", "exactly once")]
	[Arguments("Twice", "exactly twice")]
	public async Task WhenTheResultCopiesOnOption_ShouldSetTheOptionOnACopy(string option, string expected)
	{
		CopyingResult sut = new(Quantifier.Default);

		CopyingResult result = Apply(sut, option);

		await That(result).IsNotSameAs(sut);
		await That(result.Options).IsNotSameAs(sut.Options);
		await That(result.Options.ToString()).IsEqualTo(expected);
		await That(sut.Options.ToString()).IsEqualTo("at least once")
			.Because("the option must be set on a copy of the quantifier");
	}

	[Test]
	[Arguments("AtLeast()", "at least twice")]
	[Arguments("AtLeast(Times)", "at least 3 times")]
	[Arguments("AtMost()", "at most twice")]
	[Arguments("AtMost(Times)", "at most 3 times")]
	[Arguments("Between", "between 1 and 3 times")]
	[Arguments("Exactly", "exactly 3 times")]
	[Arguments("LessThan()", "fewer than twice")]
	[Arguments("LessThan(Times)", "fewer than 3 times")]
	[Arguments("MoreThan()", "more than twice")]
	[Arguments("MoreThan(Times)", "more than 3 times")]
	[Arguments("Never", "never")]
	[Arguments("Once", "exactly once")]
	[Arguments("Twice", "exactly twice")]
	public async Task WhenTheResultDoesNotCopyOnOption_ShouldSetTheOptionInPlace(string option, string expected)
	{
		InPlaceResult sut = new(Quantifier.Default);
		OptionsHolder<Quantifier> holder = sut.Options;

		InPlaceResult result = Apply(sut, option);

		await That(result).IsSameAs(sut);
		await That(result.Options).IsSameAs(holder);
		await That(holder.ToString()).IsEqualTo(expected);
	}

	private static TResult Apply<TResult>(TResult result, string option)
		where TResult : IOptionsProvider<Quantifier>
		=> option switch
		{
			"AtLeast()" => result.AtLeast().Twice(),
			"AtLeast(Times)" => result.AtLeast(3),
			"AtMost()" => result.AtMost().Twice(),
			"AtMost(Times)" => result.AtMost(3),
			"Between" => result.Between(1).And(3),
			"Exactly" => result.Exactly(3),
			"LessThan()" => result.LessThan().Twice(),
			"LessThan(Times)" => result.LessThan(3),
			"MoreThan()" => result.MoreThan().Twice(),
			"MoreThan(Times)" => result.MoreThan(3),
			"Never" => result.Never(),
			"Once" => result.Once(),
			"Twice" => result.Twice(),
			_ => throw new ArgumentOutOfRangeException(nameof(option)),
		};

	private sealed class CopyingResult(Quantifier quantifier)
		: IOptionsProvider<Quantifier>,
			ICopyOnOption<Quantifier, CopyingResult>
	{
		public OptionsHolder<Quantifier> Options { get; } = new(quantifier);

		public CopyingResult WithOptions(Quantifier copiedOptions) => new(copiedOptions);
	}

	private sealed class InPlaceResult(Quantifier quantifier) : IOptionsProvider<Quantifier>
	{
		public OptionsHolder<Quantifier> Options { get; } = new(quantifier);
	}
}
