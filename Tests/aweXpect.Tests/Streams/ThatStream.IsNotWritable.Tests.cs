using System.IO;

namespace aweXpect.Tests;

public sealed partial class ThatStream
{
	public sealed class IsNotWritable
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				ChunkedStreamContainer subject = new(new ChunkedStream(canWrite: true));

				async Task Act()
					=> await That(subject).Whose(c => c.Chunks, chunks => chunks.IsNotWritable());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             whose Chunks are not writable,
					             but Chunks were
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNotWritable_ShouldSucceed()
			{
				Stream subject = new MyStream(canWrite: false);

				async Task Act()
					=> await That(subject).IsNotWritable();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Stream? subject = null;

				async Task Act()
					=> await That(subject).IsNotWritable();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not writable,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsWritable_ShouldFail()
			{
				Stream subject = new MyStream(canWrite: true);

				async Task Act()
					=> await That(subject).IsNotWritable();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not writable,
					             but it was
					             """);
			}
		}
	}
}
