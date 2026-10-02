using System.IO;

namespace aweXpect.Tests;

public sealed partial class ThatStream
{
	public sealed class IsNotWriteOnly
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				ChunkedStreamContainer subject = new(new ChunkedStream(canRead: false, canWrite: true));

				async Task Act()
					=> await That(subject).Whose(c => c.Chunks, chunks => chunks.IsNotWriteOnly());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Chunks are not write-only,
					             but Chunks were
					             """);
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task WhenSubjectIsABufferedStream_ShouldAllowChainingIntoBufferedStreamExpectations()
			{
				using BufferedStream subject = new(new MemoryStream(new byte[3]), 4096);

				async Task Act()
					=> await That(subject).IsNotWriteOnly().And.HasBufferSize(4096);

				await That(Act).DoesNotThrow();
			}
#endif

			[Theory]
			[InlineData(false, false)]
			[InlineData(true, false)]
			[InlineData(true, true)]
			public async Task WhenSubjectIsNotWriteOnly_ShouldSucceed(bool canRead, bool canWrite)
			{
				Stream subject = new MyStream(canRead: canRead, canWrite: canWrite);

				async Task Act()
					=> await That(subject).IsNotWriteOnly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Stream? subject = null;

				async Task Act()
					=> await That(subject).IsNotWriteOnly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not write-only,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsWriteOnly_ShouldFail()
			{
				Stream subject = new MyStream(canRead: false, canWrite: true);

				async Task Act()
					=> await That(subject).IsNotWriteOnly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not write-only,
					             but it was
					             """);
			}
		}
	}
}
