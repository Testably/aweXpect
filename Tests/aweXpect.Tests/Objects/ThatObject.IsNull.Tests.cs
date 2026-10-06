namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed class IsNull
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMultiLineStringIsVerifiedInThatAll_ShouldEscapeIt()
			{
				object subject = "a\nb";

				async Task Act()
					=> await ThatAll(That(subject).IsNull());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected all of the following to succeed:
					              [01] Expected that subject is null
					             but
					              [01] it was "a\nb"
					             """)
					.Because("a raw line break would continue the value at the start of the next line");
			}

			[Test]
			public async Task WhenSubjectIsAMultiLineString_ShouldEscapeItLikeAStringSubject()
			{
				object subject = "say \"hi\"\nbye";

				async Task Act()
					=> await That(subject).IsNull();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is null,
					             but it was "say \"hi\"\nbye"
					             """)
					.Because("a string held by an object is formatted like the subject of a string expectation");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				object? subject = null;

				async Task Act()
					=> await That(subject).IsNull();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsObject_ShouldFail()
			{
				object subject = new MyClass();

				async Task Act()
					=> await That(subject).IsNull()
						.Because("we want to test the failure");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is null, because we want to test the failure,
					             but it was ThatObject.MyClass {
					                 Value = 0
					               }
					             """);
			}
		}

		public sealed class StructTests
		{
			[Test]
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).IsNull();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsObject_ShouldFail()
			{
				int? subject = 1;

				async Task Act()
					=> await That(subject).IsNull()
						.Because("we want to test the failure");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is null, because we want to test the failure,
					             but it was 1
					             """);
			}
		}
	}
}
