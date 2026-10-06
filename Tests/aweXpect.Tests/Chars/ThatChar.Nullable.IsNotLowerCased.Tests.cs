namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed partial class Nullable
	{
		public sealed class IsNotLowerCased
		{
			public sealed class Tests
			{
				[Test]
				[Arguments('a')]
				[Arguments('m')]
				[Arguments('z')]
				[Arguments('\u00E4')]
				[Arguments('\u03C9')]
				public async Task WhenSubjectIsLowerCased_ShouldFail(char? subject)
				{
					async Task Act()
						=> await That(subject).IsNotLowerCased();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not lower-cased,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				[Arguments('A')]
				[Arguments('Z')]
				[Arguments('\u00C4')]
				[Arguments('1')]
				[Arguments(' ')]
				[Arguments('@')]
				[Arguments('\u4E50')]
				public async Task WhenSubjectIsNotLowerCased_ShouldSucceed(char? subject)
				{
					async Task Act()
						=> await That(subject).IsNotLowerCased();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).IsNotLowerCased();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not lower-cased,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				[Arguments('a')]
				[Arguments('m')]
				[Arguments('z')]
				[Arguments('\u00E4')]
				[Arguments('\u03C9')]
				public async Task WhenSubjectIsLowerCased_ShouldSucceed(char? subject)
				{
					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsNotLowerCased());

					await That(Act).DoesNotThrow();
				}

				[Test]
				[Arguments('A')]
				[Arguments('Z')]
				[Arguments('\u00C4')]
				[Arguments('1')]
				[Arguments(' ')]
				[Arguments('@')]
				[Arguments('\u4E50')]
				public async Task WhenSubjectIsNotLowerCased_ShouldFail(char? subject)
				{
					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsNotLowerCased());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is lower-cased,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsNotLowerCased());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is lower-cased,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
