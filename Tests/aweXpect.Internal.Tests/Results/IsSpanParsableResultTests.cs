#if NET8_0_OR_GREATER
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect.Internal.Tests.Results;

public sealed class IsSpanParsableResultTests
{
	[Test]
	public async Task Which_WhenSubjectIsNotParsable_ShouldUseTheDefaultValue()
	{
		IsSpanParsableResult<int> sut = CreateSut("abc");

		async Task Act()
			=> await sut.Which.IsEqualTo(0);

		await That(Act).DoesNotThrow()
			.Because("without a preceding parsability check, a subject that cannot be parsed yields the default value");
	}

	private static IsSpanParsableResult<int> CreateSut(string subject)
	{
#pragma warning disable aweXpect0001
		IThat<SpanWrapper<char>> source = That(subject.AsSpan());
#pragma warning restore aweXpect0001
		return new IsSpanParsableResult<int>(source.Get().ExpectationBuilder, source, null);
	}
}
#endif
