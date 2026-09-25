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
