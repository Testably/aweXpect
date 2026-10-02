using System.IO;

namespace aweXpect.Tests;

public sealed partial class ThatStream
{
	public sealed class IsWritable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				ChunkedStreamContainer subject = new(new ChunkedStream(canWrite: false));

				async Task Act()
					=> await That(subject).Whose(c => c.Chunks, chunks => chunks.IsWritable());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Chunks are writable,
					             but Chunks were not
					             """);
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task WhenSubjectIsABufferedStream_ShouldAllowChainingIntoBufferedStreamExpectations()
			{
				using BufferedStream subject = new(new MemoryStream(new byte[3]), 4096);

				async Task Act()
					=> await That(subject).IsWritable().And.HasBufferSize(4096);

				await That(Act).DoesNotThrow();
			}
#endif

			[Fact]
			public async Task WhenSubjectIsNotWritable_ShouldFail()
			{
				Stream subject = new MyStream(canWrite: false);

				async Task Act()
					=> await That(subject).IsWritable();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is writable,
					             but it was not
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Stream? subject = null;

				async Task Act()
					=> await That(subject).IsWritable();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is writable,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsWritable_ShouldSucceed()
			{
				Stream subject = new MyStream(canWrite: true);

				async Task Act()
					=> await That(subject).IsWritable();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
