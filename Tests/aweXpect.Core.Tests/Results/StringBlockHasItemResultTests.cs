using System.Collections.Generic;
using aweXpect.Core.Extending;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed class StringBlockHasItemResultTests
{
	[Test]
	public async Task AsBlock_ShouldInterpretExpectedAsBlock()
	{
		StringEqualityOptions options = new("expected");
		StringHasItemResult<IEnumerable<string>> sut = CreateSut(options);

		StringBlockHasItemResult<IEnumerable<string>> result = sut.AsBlock();

		await That(options.ToString()).IsEqualTo(" as block");
		await That(result).IsNot<IOptionsProvider<StringEqualityOptions>>()
			.Because("a block compares the lines on its own, so it offers only the casing and a comparer");
	}

	[Test]
	public async Task AsBlock_WhenTheItemAtTheIndexIsTheBlockIndentedAsAWhole_ShouldSucceed()
	{
		string[] subject = ["a", "  a\n  b",];

		async Task Act()
			=> await That(subject).HasItem("a\nb").AsBlock().AtIndex(1);

		await That(Act).DoesNotThrow()
			.Because("the index can still be specified after the block");
	}

	[Test]
	public async Task AsBlock_WhenTheItemAtTheIndexIsNoBlockMatch_ShouldFail()
	{
		string[] subject = ["  a\n  b", "a",];

		async Task Act()
			=> await That(subject).HasItem("a\nb").AsBlock().AtIndex(1);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             has an item matching "a\nb" as block at index 1,
			             but it had item "a" at index 1

			             Collection:
			             [
			               "  a\n  b",
			               "a"
			             ]
			             """);
	}

	[Test]
	public async Task IgnoringCase_ShouldSetOption()
	{
		StringEqualityOptions options = new("expected");
		StringBlockHasItemResult<IEnumerable<string>> sut = CreateSut(options).AsBlock();

		StringBlockHasItemResult<IEnumerable<string>> result = sut.IgnoringCase();

		await That(result).IsSameAs(sut);
		await That(options.ToString()).IsEqualTo(" as block ignoring case");
	}

	[Test]
	public async Task Using_ShouldSetComparer()
	{
		StringEqualityOptions options = new("expected");
		StringBlockHasItemResult<IEnumerable<string>> sut = CreateSut(options).AsBlock();

		StringBlockHasItemResult<IEnumerable<string>> result = sut.Using(StringComparer.OrdinalIgnoreCase);

		await That(result).IsSameAs(sut);
		await That(options.ToString()).StartsWith(" as block using ");
	}

	private static StringHasItemResult<IEnumerable<string>> CreateSut(StringEqualityOptions options)
	{
#pragma warning disable aweXpect0001
		IThat<IEnumerable<string>?> source = That<IEnumerable<string>?>(Array.Empty<string>());
#pragma warning restore aweXpect0001
		return new StringHasItemResult<IEnumerable<string>>(source.Get().ExpectationBuilder.AddConstraint((it, _) => new DummyConstraint(it)),
			source,
			new CollectionIndexOptions(),
			options);
	}
}
