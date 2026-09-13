namespace aweXpect.Tests;

public sealed partial class ThatException
{
	public sealed partial class HasInner
	{
		public sealed class Generic
		{
			public sealed class ExpectationsTests
			{
				[Fact]
				public async Task WhenExpectationsAreTypedAtTheInnerExceptionType_ShouldSucceed()
				{
					Exception subject = new("outer", new CustomException("inner"));

					async Task Act()
						=> await That(subject)
							.HasInner<CustomException>(e => e.Satisfies(i => i?.Message == "inner"));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenInnerExceptionHasCorrectMessageButUnexpectedType_ShouldFail()
				{
					Exception subject = new("outer", new Exception("inner"));

					async Task Act()
						=> await That(subject).HasInner<CustomException>(e => e.HasMessage("inner"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an inner ThatException.CustomException whose Message is equal to "inner",
						             but it was an Exception:
						               inner
						             """);
				}

				[Fact]
				public async Task WhenInnerExceptionHasCorrectTypeAndMessage_ShouldSucceed()
				{
					Exception subject = new("outer",
						new CustomException("inner"));

					async Task Act()
						=> await That(subject).HasInner<CustomException>(e => e.HasMessage("inner"));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenInnerExceptionHasCorrectTypeButUnexpectedMessage_ShouldFail()
				{
					Exception subject = new("outer",
						new CustomException("inner"));

					async Task Act()
						=> await That(subject).HasInner<CustomException>(e => e.HasMessage("some other message"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an inner ThatException.CustomException whose Message is equal to "some other message",
						             but it was "inner" which differs at index 0:
						                ↓ (actual)
						               "inner"
						               "some other message"
						                ↑ (expected)

						             Message:
						             inner
						             """);
				}

				[Fact]
				public async Task WhenInnerExceptionIsNullAndExpectationsAreTypedAtTheInnerExceptionType_ShouldFail()
				{
					Exception subject = new("outer");

					async Task Act()
						=> await That(subject)
							.HasInner<CustomException>(e => e.Satisfies(i => i?.Message == "inner"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an inner ThatException.CustomException whose satisfies i => i?.Message == "inner",
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenInnerExceptionTypeAndMessageAreUnexpected_ShouldOnlyReportTheType()
				{
					Exception subject = new("outer", new Exception("other"));

					async Task Act()
						=> await That(subject).HasInner<CustomException>(e => e.HasMessage("inner"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an inner ThatException.CustomException whose Message is equal to "inner",
						             but it was an Exception:
						               other
						             """);
				}

				[Fact]
				public async Task
					WhenInnerExceptionTypeIsUnexpectedAndExpectationsAreTypedAtTheInnerExceptionType_ShouldFail()
				{
					Exception subject = new("outer", new Exception("inner"));

					async Task Act()
						=> await That(subject)
							.HasInner<CustomException>(e => e.Satisfies(i => i?.Message == "inner"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an inner ThatException.CustomException whose satisfies i => i?.Message == "inner",
						             but it was an Exception:
						               inner
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNullAndExpectationsAreTypedAtTheInnerExceptionType_ShouldFail()
				{
					Exception? subject = null;

					async Task Act()
						=> await That(subject)
							.HasInner<CustomException>(e => e.Satisfies(i => i?.Message == "inner"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an inner ThatException.CustomException whose satisfies i => i?.Message == "inner",
						             but it was <null>
						             """);
				}
			}

			public sealed class TypeTests
			{
				[Fact]
				public async Task WhenInnerExceptionIsNotOfTheExpectedType_ShouldFail()
				{
					Exception subject = new("outer",
						new Exception("inner"));

					async Task Act()
						=> await That(subject).HasInner<CustomException>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an inner ThatException.CustomException,
						             but it was an Exception:
						               inner
						             """);
				}


				[Fact]
				public async Task WhenInnerExceptionMeetsType_ShouldSucceed()
				{
					Exception subject = new("outer",
						new CustomException("inner"));

					async Task Act()
						=> await That(subject).HasInner<CustomException>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					Exception? subject = null;

					async Task Act()
						=> await That(subject).HasInner<CustomException>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an inner ThatException.CustomException,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedExpectationsTests
			{
				[Fact]
				public async Task WhenExpectationsAreTypedAtTheInnerExceptionType_ShouldFail()
				{
					Exception subject = new("outer", new CustomException("inner"));

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it
							=> it.HasInner<CustomException>(e => e.Satisfies(i => i?.Message == "inner")));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have an inner ThatException.CustomException whose satisfies i => i?.Message == "inner",
						             but it had
						             """);
				}

				[Fact]
				public async Task WhenInnerExceptionHasCorrectMessageButUnexpectedType_ShouldSucceed()
				{
					Exception subject = new("outer", new Exception("inner"));

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it
							=> it.HasInner<CustomException>(e => e.HasMessage("inner")));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenInnerExceptionHasCorrectTypeAndMessage_ShouldFail()
				{
					Exception subject = new("outer",
						new CustomException("inner"));

					async Task Act()
						=> await That(subject)
							.DoesNotComplyWith(it => it.HasInner<CustomException>(e => e.HasMessage("inner")));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have an inner ThatException.CustomException whose Message is equal to "inner",
						             but it had
						             
						             Message:
						             inner
						             """);
				}

				[Fact]
				public async Task WhenInnerExceptionHasCorrectTypeButUnexpectedMessage_ShouldSucceed()
				{
					Exception subject = new("outer",
						new CustomException("inner"));

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it
							=> it.HasInner<CustomException>(e => e.HasMessage("some other message")));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task
					WhenInnerExceptionTypeIsUnexpectedAndExpectationsAreTypedAtTheInnerExceptionType_ShouldSucceed()
				{
					Exception subject = new("outer", new Exception("inner"));

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it
							=> it.HasInner<CustomException>(e => e.Satisfies(i => i?.Message == "inner")));

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NegatedTypeTests
			{
				[Fact]
				public async Task WhenInnerExceptionIsNotOfTheExpectedType_ShouldSucceed()
				{
					Exception subject = new("outer",
						new Exception("inner"));

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.HasInner<CustomException>());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenInnerExceptionIsNull_ShouldSucceed()
				{
					Exception subject = new("outer");

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.HasInner<CustomException>());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenInnerExceptionMeetsType_ShouldFail()
				{
					Exception subject = new("outer",
						new CustomException("inner"));

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.HasInner<CustomException>());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have an inner ThatException.CustomException,
						             but it had
						             """);
				}
			}
		}
	}
}
