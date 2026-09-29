namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed class IsNotANumber
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData('0')]
			[InlineData('1')]
			[InlineData('4')]
			[InlineData('9')]
			[InlineData('\u00BD')]
			[InlineData('\u2163')]
			[InlineData('\u00B2')]
			public async Task WhenSubjectIsANumber_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).IsNotANumber();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not a number,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData('a')]
			[InlineData('d')]
			[InlineData('z')]
			[InlineData('A')]
			[InlineData('M')]
			[InlineData('Z')]
			[InlineData('\u4E50')]
			[InlineData('\t')]
			[InlineData('@')]
			[InlineData('[')]
			[InlineData(']')]
			[InlineData('{')]
			[InlineData('}')]
			public async Task WhenSubjectIsNotANumber_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).IsNotANumber();

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
			[InlineData('\u00BD')]
			[InlineData('\u2163')]
			[InlineData('\u00B2')]
			public async Task WhenSubjectIsANumber_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotANumber());

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData('a')]
			[InlineData('d')]
			[InlineData('z')]
			[InlineData('A')]
			[InlineData('M')]
			[InlineData('Z')]
			[InlineData('\u4E50')]
			[InlineData('\t')]
			[InlineData('@')]
			[InlineData('[')]
			[InlineData(']')]
			[InlineData('{')]
			[InlineData('}')]
			public async Task WhenSubjectIsNotANumber_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotANumber());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is a number,
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
