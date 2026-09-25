using System.IO;

namespace aweXpect.Tests;

public sealed partial class ThatStream
{
	public sealed class IsNotReadOnly
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				ChunkedStreamContainer subject = new(new ChunkedStream(canRead: true, canWrite: false));

				async Task Act()
					=> await That(subject).Whose(c => c.Chunks, chunks => chunks.IsNotReadOnly());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Chunks are not read-only,
					             but Chunks were
					             """);
			}

			[Theory]
			[InlineData(false, false)]
			[InlineData(false, true)]
			[InlineData(true, true)]
			public async Task WhenSubjectIsNotReadOnly_ShouldSucceed(bool canRead, bool canWrite)
			{
				Stream subject = new MyStream(canRead: canRead, canWrite: canWrite);

				async Task Act()
					=> await That(subject).IsNotReadOnly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Stream? subject = null;

				async Task Act()
					=> await That(subject).IsNotReadOnly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not read-only,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsReadOnly_ShouldFail()
			{
				Stream subject = new MyStream(canRead: true, canWrite: false);

				async Task Act()
					=> await That(subject).IsNotReadOnly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not read-only,
					             but it was
					             """);
			}
		}
	}
}
