using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed class CollectionIndexOptionsExtensionsTests
{
	[Test]
	public async Task WhenTheResultCopiesOnOption_ShouldKeepTheIndexSetSoFar()
	{
		CopyingResult sut = new(CollectionIndexOptions.Default);

		void Act() => sut.AtIndex(1).AtIndexFromEnd(1);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("AtIndexFromEnd cannot be combined with AtIndex.");
	}

	[Test]
	public async Task WhenTheResultCopiesOnOption_ShouldReplaceARepeatedOptionOnTheCopy()
	{
		CopyingResult sut = new(CollectionIndexOptions.Default);
		CopyingResult atIndex1 = sut.AtIndex(1);

		CopyingResult result = atIndex1.AtIndex(2);

		await That(result.Options.Value.Match.GetDescription()).IsEqualTo(" at index 2");
		await That(atIndex1.Options.Value.Match.GetDescription()).IsEqualTo(" at index 1")
			.Because("the repeated option must be set on a copy of the index options");
	}

	[Test]
	[Arguments("AtIndex", " at index 2")]
	[Arguments("AtIndexFromEnd", " at index 2 from end")]
	public async Task WhenTheResultCopiesOnOption_ShouldSetTheOptionOnACopy(string option, string expected)
	{
		CopyingResult sut = new(CollectionIndexOptions.Default);

		CopyingResult result = Apply(sut, option);

		await That(result).IsNotSameAs(sut);
		await That(result.Options).IsNotSameAs(sut.Options);
		await That(result.Options.Value.Match.GetDescription()).IsEqualTo(expected);
		await That(sut.Options.Value.Match.GetDescription()).IsEmpty()
			.Because("the option must be set on a copy of the index options");
	}

	[Test]
	[Arguments("AtIndex", " at index 2")]
	[Arguments("AtIndexFromEnd", " at index 2 from end")]
	public async Task WhenTheResultDoesNotCopyOnOption_ShouldSetTheOptionInPlace(string option, string expected)
	{
		InPlaceResult sut = new(CollectionIndexOptions.Default);
		OptionsHolder<CollectionIndexOptions> holder = sut.Options;

		InPlaceResult result = Apply(sut, option);

		await That(result).IsSameAs(sut);
		await That(result.Options).IsSameAs(holder);
		await That(holder.Value.Match.GetDescription()).IsEqualTo(expected);
	}

	private static TResult Apply<TResult>(TResult result, string option)
		where TResult : IOptionsProvider<CollectionIndexOptions>
		=> option switch
		{
			"AtIndex" => result.AtIndex(2),
			"AtIndexFromEnd" => result.AtIndexFromEnd(2),
			_ => throw new ArgumentOutOfRangeException(nameof(option)),
		};

	private sealed class CopyingResult(CollectionIndexOptions options)
		: IOptionsProvider<CollectionIndexOptions>,
			ICopyOnOption<CollectionIndexOptions, CopyingResult>
	{
		public OptionsHolder<CollectionIndexOptions> Options { get; } = new(options);

		public CopyingResult WithOptions(CollectionIndexOptions copiedOptions) => new(copiedOptions);
	}

	private sealed class InPlaceResult(CollectionIndexOptions options) : IOptionsProvider<CollectionIndexOptions>
	{
		public OptionsHolder<CollectionIndexOptions> Options { get; } = new(options);
	}
}
