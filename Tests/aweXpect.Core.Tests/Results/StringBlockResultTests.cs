using aweXpect.Core.Extending;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed class StringBlockResultTests
{
	[Test]
	public async Task AsBlock_ShouldInterpretExpectedAsBlock()
	{
		StringEqualityOptions options = new("expected");
		StringEqualityTypeResult<string?, IThat<string?>> sut = CreateSut("foo", options);

		StringBlockResult<string?, IThat<string?>> result = sut.AsBlock();

		await That(options.ToString()).IsEqualTo(" as block");
		await That(result).IsNot<IOptionsProvider<StringEqualityOptions>>()
			.Because("a block compares the lines on its own, so it offers only the casing and a comparer");
	}

	[Test]
	public async Task AsBlock_WhenTheSubjectIsTheBlockIndentedAsAWhole_ShouldSucceed()
	{
		string subject = "  a\n    b";

		async Task Act()
			=> await That(subject).IsEqualTo("a\n  b").AsBlock();

		await That(Act).DoesNotThrow()
			.Because("the block may be indented as a whole, but keeps its relative indentation");
	}

	[Test]
	public async Task AsBlock_WhenTheSubjectIsTheBlockWithAdditionalLines_ShouldFail()
	{
		string subject = "  a\n  b\n  c";

		async Task Act()
			=> await That(subject).IsEqualTo("a\nb").AsBlock();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             matches "a\nb" as block,
			             but it was "  a\n  b\n  c"
			             """)
			.Because("unlike Contains, the whole subject has to match the block");
	}

	[Test]
	public async Task IgnoringCase_ShouldSetOption()
	{
		StringEqualityOptions options = new("expected");
		StringBlockResult<string?, IThat<string?>> sut = CreateSut("foo", options).AsBlock();

		StringBlockResult<string?, IThat<string?>> result = sut.IgnoringCase();

		await That(result).IsSameAs(sut);
		await That(options.ToString()).IsEqualTo(" as block ignoring case");
	}

	[Test]
	public async Task Using_ShouldSetComparer()
	{
		StringEqualityOptions options = new("expected");
		StringBlockResult<string?, IThat<string?>> sut = CreateSut("foo", options).AsBlock();

		StringBlockResult<string?, IThat<string?>> result = sut.Using(StringComparer.OrdinalIgnoreCase);

		await That(result).IsSameAs(sut);
		await That(options.ToString()).StartsWith(" as block using ");
	}

	private static StringEqualityTypeResult<string?, IThat<string?>> CreateSut(string? subject,
		StringEqualityOptions options)
	{
#pragma warning disable aweXpect0001
		IThat<string?> source = That(subject);
#pragma warning restore aweXpect0001
		return new StringEqualityTypeResult<string?, IThat<string?>>(source.Get().ExpectationBuilder.AddConstraint((it, _) => new DummyConstraint(it)),
			source,
			options);
	}
}
