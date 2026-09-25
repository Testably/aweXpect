using System.IO;

namespace aweXpect.Tests;

public sealed partial class ThatStream
{
	public sealed class IsNotSeekable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				ChunkedStreamContainer subject = new(new ChunkedStream(canSeek: true));

				async Task Act()
					=> await That(subject).Whose(c => c.Chunks, chunks => chunks.IsNotSeekable());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Chunks are not seekable,
					             but Chunks were
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNotSeekable_ShouldSucceed()
			{
				Stream subject = new MyStream(canSeek: false);

				async Task Act()
					=> await That(subject).IsNotSeekable();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Stream? subject = null;

				async Task Act()
					=> await That(subject).IsNotSeekable();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not seekable,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsSeekable_ShouldFail()
			{
				Stream subject = new MyStream(canSeek: true);

				async Task Act()
					=> await That(subject).IsNotSeekable();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not seekable,
					             but it was
					             """);
			}
		}
	}
}
