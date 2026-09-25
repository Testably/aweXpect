using System.IO;

namespace aweXpect.Tests;

public sealed partial class ThatStream
{
	public sealed class IsReadOnly
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				ChunkedStreamContainer subject = new(new ChunkedStream(canRead: true, canWrite: true));

				async Task Act()
					=> await That(subject).Whose(c => c.Chunks, chunks => chunks.IsReadOnly());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Chunks are read-only,
					             but Chunks were not
					             """);
			}

			[Theory]
			[InlineData(false, false)]
			[InlineData(false, true)]
			[InlineData(true, true)]
			public async Task WhenSubjectIsNotReadOnly_ShouldFail(bool canRead, bool canWrite)
			{
				Stream subject = new MyStream(canRead: canRead, canWrite: canWrite);

				async Task Act()
					=> await That(subject).IsReadOnly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is read-only,
					             but it was not
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Stream? subject = null;

				async Task Act()
					=> await That(subject).IsReadOnly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is read-only,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsReadOnly_ShouldSucceed()
			{
				Stream subject = new MyStream(canRead: true, canWrite: false);

				async Task Act()
					=> await That(subject).IsReadOnly();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
