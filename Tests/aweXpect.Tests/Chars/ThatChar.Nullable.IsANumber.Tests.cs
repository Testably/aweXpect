namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed partial class Nullable
	{
		public sealed class IsANumber
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
				public async Task WhenSubjectIsANumber_ShouldSucceed(char? subject)
				{
					async Task Act()
						=> await That(subject).IsANumber();

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
				public async Task WhenSubjectIsNotANumber_ShouldFail(char? subject)
				{
					async Task Act()
						=> await That(subject).IsANumber();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is a number,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).IsANumber();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is a number,
						             but it was <null>
						             """);
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
				public async Task WhenSubjectIsANumber_ShouldFail(char? subject)
				{
					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsANumber());

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
				public async Task WhenSubjectIsNotANumber_ShouldSucceed(char? subject)
				{
					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsANumber());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsANumber());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not a number,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
