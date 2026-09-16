namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed partial class Nullable
	{
		public sealed class IsNotUpperCased
		{
			public sealed class Tests
			{
				[Theory]
				[InlineData('a')]
				[InlineData('z')]
				[InlineData('\u00E4')]
				[InlineData('1')]
				[InlineData(' ')]
				[InlineData('@')]
				[InlineData('\u4E50')]
				public async Task WhenSubjectIsNotUpperCased_ShouldSucceed(char? subject)
				{
					async Task Act()
						=> await That(subject).IsNotUpperCased();

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData('A')]
				[InlineData('M')]
				[InlineData('Z')]
				[InlineData('\u00C4')]
				[InlineData('\u03A9')]
				public async Task WhenSubjectIsUpperCased_ShouldFail(char? subject)
				{
					async Task Act()
						=> await That(subject).IsNotUpperCased();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not upper-cased,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).IsNotUpperCased();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not upper-cased,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Theory]
				[InlineData('a')]
				[InlineData('z')]
				[InlineData('\u00E4')]
				[InlineData('1')]
				[InlineData(' ')]
				[InlineData('@')]
				[InlineData('\u4E50')]
				public async Task WhenSubjectIsNotUpperCased_ShouldFail(char? subject)
				{
					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsNotUpperCased());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is upper-cased,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Theory]
				[InlineData('A')]
				[InlineData('M')]
				[InlineData('Z')]
				[InlineData('\u00C4')]
				[InlineData('\u03A9')]
				public async Task WhenSubjectIsUpperCased_ShouldSucceed(char? subject)
				{
					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsNotUpperCased());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsNotUpperCased());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is upper-cased,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
