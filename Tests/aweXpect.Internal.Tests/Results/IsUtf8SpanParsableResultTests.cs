#if NET8_0_OR_GREATER
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect.Internal.Tests.Results;

public sealed class IsUtf8SpanParsableResultTests
{
	[Test]
	public async Task Which_WhenSubjectIsNotParsable_ShouldUseTheDefaultValue()
	{
		IsUtf8SpanParsableResult<int> sut = CreateSut("abc"u8.ToArray());

		async Task Act()
			=> await sut.Which.IsEqualTo(0);

		await That(Act).DoesNotThrow()
			.Because("without a preceding parsability check, a subject that cannot be parsed yields the default value");
	}

	private static IsUtf8SpanParsableResult<int> CreateSut(byte[] subject)
	{
#pragma warning disable aweXpect0001
		IThat<SpanWrapper<byte>> source = That(subject.AsSpan());
#pragma warning restore aweXpect0001
		return new IsUtf8SpanParsableResult<int>(source.Get().ExpectationBuilder, source, null);
	}
}
#endif
