namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed class IsLowerCased
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData('a')]
			[InlineData('m')]
			[InlineData('z')]
			[InlineData('\u00E4')]
			[InlineData('\u03C9')]
			public async Task WhenSubjectIsLowerCased_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData('A')]
			[InlineData('Z')]
			[InlineData('\u00C4')]
			[InlineData('1')]
			[InlineData(' ')]
			[InlineData('@')]
			[InlineData('\u4E50')]
			public async Task WhenSubjectIsNotLowerCased_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is lower-cased,
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData('a')]
			[InlineData('m')]
			[InlineData('z')]
			[InlineData('\u00E4')]
			[InlineData('\u03C9')]
			public async Task WhenSubjectIsLowerCased_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsLowerCased());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not lower-cased,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData('A')]
			[InlineData('Z')]
			[InlineData('\u00C4')]
			[InlineData('1')]
			[InlineData(' ')]
			[InlineData('@')]
			[InlineData('\u4E50')]
			public async Task WhenSubjectIsNotLowerCased_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsLowerCased());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
