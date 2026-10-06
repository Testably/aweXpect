using System.IO;

namespace aweXpect.Tests;

public sealed partial class ThatStream
{
	public sealed class IsWriteOnly
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				ChunkedStreamContainer subject = new(new ChunkedStream(canRead: true, canWrite: true));

				async Task Act()
					=> await That(subject).Whose(c => c.Chunks, chunks => chunks.IsWriteOnly());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             whose Chunks are write-only,
					             but Chunks were not
					             """);
			}

			[Test]
			[Arguments(false, false)]
			[Arguments(true, false)]
			[Arguments(true, true)]
			public async Task WhenSubjectIsNotWriteOnly_ShouldFail(bool canRead, bool canWrite)
			{
				Stream subject = new MyStream(canRead: canRead, canWrite: canWrite);

				async Task Act()
					=> await That(subject).IsWriteOnly();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is write-only,
					             but it was not
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Stream? subject = null;

				async Task Act()
					=> await That(subject).IsWriteOnly();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is write-only,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsWriteOnly_ShouldSucceed()
			{
				Stream subject = new MyStream(canRead: false, canWrite: true);

				async Task Act()
					=> await That(subject).IsWriteOnly();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
