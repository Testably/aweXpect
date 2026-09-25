using System.IO;

namespace aweXpect.Tests;

public sealed partial class ThatStream
{
	public sealed class IsReadable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				ChunkedStreamContainer subject = new(new ChunkedStream(canRead: false));

				async Task Act()
					=> await That(subject).Whose(c => c.Chunks, chunks => chunks.IsReadable());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Chunks are readable,
					             but Chunks were not
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNotReadable_ShouldFail()
			{
				Stream subject = new MyStream(canRead: false);

				async Task Act()
					=> await That(subject).IsReadable();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is readable,
					             but it was not
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Stream? subject = null;

				async Task Act()
					=> await That(subject).IsReadable();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is readable,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsReadable_ShouldSucceed()
			{
				Stream subject = new MyStream(canRead: true);

				async Task Act()
					=> await That(subject).IsReadable();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
