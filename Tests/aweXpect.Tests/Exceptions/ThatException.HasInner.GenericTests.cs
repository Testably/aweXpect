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
				public async Task WhenExpectationsAreCombinedWithAnd_ShouldApplyAllOfThem()
				{
					Exception subject = new("outer", new CustomException("inner"));

					async Task Act()
						=> await That(subject)
							.HasInner<CustomException>(e => e.HasMessage("inner").And.HasMessage("other"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an inner ThatException.CustomException whose Message is equal to "inner" and whose Message is equal to "other",
						             but Message was "inner" which differs at index 0:
						                ↓ (actual)
						               "inner"
						               "other"
						                ↑ (expected)

						             Message:
						             inner
						             """)
						.Because("both expectations inspect the same Message, which is only appended once");
				}

				[Fact]
				public async Task WhenExpectationsAreCombinedWithOr_ShouldApplyEitherOfThem()
				{
					Exception subject = new("outer", new CustomException("inner"));

					async Task Act()
						=> await That(subject)
							.HasInner<CustomException>(e => e.HasMessage("other").Or.HasMessage("inner"));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenExpectationsAreEmpty_ShouldThrowArgumentException()
				{
					Exception subject = new("outer", new CustomException("inner"));

					async Task Act()
						=> await That(subject).HasInner<CustomException>(_ => { });

					await That(Act).Throws<ArgumentException>()
						.WithMessage("You must add at least one expectation in the expectations callback.*").AsWildcard()
						.And.WithParamName("expectations");
				}

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
						             but it had an inner Exception:
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
						             but Message was "inner" which differs at index 0:
						                ↓ (actual)
						               "inner"
						               "some other message"
						                ↑ (expected)

						             Message:
						             inner
						             """);
				}

				[Fact]
				public async Task WhenInnerExceptionHasUnexpectedTypeAndNegatedExpectations_ShouldKeepTheNegation()
				{
					Exception subject = new("outer", new Exception("inner"));

					async Task Act()
						=> await That(subject)
							.HasInner<CustomException>(e => e.DoesNotComplyWith(i => i.HasMessage("foo")));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an inner ThatException.CustomException whose Message is not equal to "foo",
						             but it had an inner Exception:
						               inner
						             """)
						.Because("the expectations on the inner exception are negated, even if they are not applied");
				}

				[Fact]
				public async Task WhenInnerExceptionIsNotEquivalent_ShouldFail()
				{
					Exception subject = new("outer", new CustomException("inner"));

					async Task Act()
						=> await That(subject)
							.HasInner<CustomException>(e => e.IsEquivalentTo(new
							{
								Message = "other",
							}));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an inner ThatException.CustomException which is equivalent to {
						                 Message = "other"
						               },
						             but it was not:
						               Property Message differed:
						                    Found: "inner"
						                 Expected: "other"

						             Equivalency options:
						              - include public fields and properties
						             """);
				}

				[Fact]
				public async Task WhenInnerExceptionIsNotOfTheNestedType_ShouldFail()
				{
					Exception subject = new("outer", new Exception("inner"));

					async Task Act()
						=> await That(subject).HasInner<Exception>(e => e.Is<CustomException>());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an inner exception which is of type ThatException.CustomException,
						             but it was Exception

						             Actual:
						             Exception: inner
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
						             has an inner ThatException.CustomException which satisfies i => i?.Message == "inner",
						             but it had no inner exception
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
						             but it had an inner Exception:
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
						             has an inner ThatException.CustomException which satisfies i => i?.Message == "inner",
						             but it had an inner Exception:
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
						             has an inner ThatException.CustomException which satisfies i => i?.Message == "inner",
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
						             but it had an inner Exception:
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
				public async Task WhenExpectationsAreCombinedWithAnd_ShouldSucceed()
				{
					Exception subject = new("outer", new CustomException("inner"));

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it
							=> it.HasInner<CustomException>(e => e.HasMessage("inner").And.HasMessage("other")));

					await That(Act).DoesNotThrow();
				}

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
						             does not have an inner ThatException.CustomException which satisfies i => i?.Message == "inner",
						             but it had an inner ThatException.CustomException:
						               inner
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
						             but it had an inner ThatException.CustomException:
						               inner
						             
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
						             but it had an inner ThatException.CustomException:
						               inner
						             """);
				}
			}
		}
	}
}
