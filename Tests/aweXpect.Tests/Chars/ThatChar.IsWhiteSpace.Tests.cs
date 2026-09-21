namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed class IsWhiteSpace
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData('0')]
			[InlineData('1')]
			[InlineData('4')]
			[InlineData('9')]
			[InlineData('a')]
			[InlineData('d')]
			[InlineData('z')]
			[InlineData('A')]
			[InlineData('M')]
			[InlineData('Z')]
			[InlineData('\u4E50')]
			[InlineData('@')]
			[InlineData('[')]
			[InlineData(']')]
			[InlineData('{')]
			[InlineData('}')]
			public async Task WhenSubjectIsNotWhiteSpace_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).IsWhiteSpace();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is whitespace,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(' ')]
			[InlineData('\t')]
			[InlineData('\r')]
			[InlineData('\n')]
			public async Task WhenSubjectIsWhiteSpace_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).IsWhiteSpace();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData('0')]
			[InlineData('1')]
			[InlineData('4')]
			[InlineData('9')]
			[InlineData('a')]
			[InlineData('d')]
			[InlineData('z')]
			[InlineData('A')]
			[InlineData('M')]
			[InlineData('Z')]
			[InlineData('\u4E50')]
			[InlineData('@')]
			[InlineData('[')]
			[InlineData(']')]
			[InlineData('{')]
			[InlineData('}')]
			public async Task WhenSubjectIsNotWhiteSpace_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsWhiteSpace());

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(' ')]
			[InlineData('\t')]
			[InlineData('\r')]
			[InlineData('\n')]
			public async Task WhenSubjectIsWhiteSpace_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsWhiteSpace());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not whitespace,
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
