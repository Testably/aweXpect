using System.IO;

namespace aweXpect.Tests;

public sealed partial class ThatStream
{
	public sealed class IsNotReadable
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				ChunkedStreamContainer subject = new(new ChunkedStream(true));

				async Task Act()
					=> await That(subject).Whose(c => c.Chunks, chunks => chunks.IsNotReadable());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             whose Chunks are not readable,
					             but Chunks were
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNotReadable_ShouldSucceed()
			{
				Stream subject = new MyStream(canRead: false);

				async Task Act()
					=> await That(subject).IsNotReadable();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Stream? subject = null;

				async Task Act()
					=> await That(subject).IsNotReadable();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not readable,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsReadable_ShouldFail()
			{
				Stream subject = new MyStream(canRead: true);

				async Task Act()
					=> await That(subject).IsNotReadable();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not readable,
					             but it was
					             """);
			}
		}
	}
}
