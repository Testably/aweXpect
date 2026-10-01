namespace aweXpect.Tests;

public sealed partial class ThatException
{
	public sealed partial class HasInner
	{
#pragma warning disable CA2263 // these tests deliberately cover the Type overloads
		public sealed class Type
		{
			public sealed class ExpectationsTests
			{
				[Fact]
				public async Task WhenCombinedWithTheGenericOverloadAndInnerExceptionHasUnexpectedType_ShouldReportItOnce()
				{
					Exception subject = new("outer", new Exception("inner"));

					async Task Act()
						=> await That(subject)
							.HasInner(typeof(CustomException), e => e.HasMessage("foo"))
							.Or.HasInner<CustomException>(e => e.HasMessage("bar"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an inner ThatException.CustomException whose message is equal to "foo" or has an inner ThatException.CustomException whose message is equal to "bar",
						             but it had an inner Exception:
						               inner
						             """)
						.Because("both overloads skip the expectations on an inner exception of another type, so they report the same mismatch");
				}

				[Fact]
				public async Task WhenExpectationsAreEmpty_ShouldThrowArgumentException()
				{
					Exception subject = new("outer", new CustomException("inner"));

					async Task Act()
						=> await That(subject).HasInner(typeof(CustomException), _ => { });

					await That(Act).Throws<ArgumentException>()
						.WithMessage("You must add at least one expectation in the expectations callback.*").AsWildcard()
						.And.WithParamName("expectations");
				}

				[Fact]
				public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
				{
					Exception subject = new("outer", new CustomException("inner"));

					async Task Act()
						=> await That(subject).HasInner(typeof(CustomException), null!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expectations").And
						.WithMessage("The 'expectations' cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenInnerExceptionHasCorrectMessageButUnexpectedType_ShouldFail()
				{
					Exception subject = new("outer", new Exception("inner"));

					async Task Act()
						=> await That(subject).HasInner(typeof(CustomException), e => e.HasMessage("inner"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an inner ThatException.CustomException whose message is equal to "inner",
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
						=> await That(subject).HasInner(typeof(CustomException), e => e.HasMessage("inner"));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenInnerExceptionHasCorrectTypeButUnexpectedMessage_ShouldFail()
				{
					Exception subject = new("outer",
						new CustomException("inner"));

					async Task Act()
						=> await That(subject)
							.HasInner(typeof(CustomException), e => e.HasMessage("some other message"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an inner ThatException.CustomException whose message is equal to "some other message",
						             but message was "inner", which differs at index 0:
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
							.HasInner(typeof(CustomException), e => e.DoesNotComplyWith(i => i.HasMessage("foo")));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an inner ThatException.CustomException whose message is not equal to "foo",
						             but it had an inner Exception:
						               inner
						             """)
						.Because("the expectations on the inner exception are negated, even if they are not applied");
				}

				[Fact]
				public async Task WhenInnerExceptionIsOfAnOpenGenericType_ShouldApplyTheExpectations()
				{
					Exception subject = new("outer", new GenericException<int>("inner"));

					async Task Act()
						=> await That(subject).HasInner(typeof(GenericException<>), e => e.HasMessage("inner"));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTypeIsANamedArgument_ShouldSucceed()
				{
					Exception subject = new("outer",
						new CustomException("inner"));

					async Task Act()
						=> await That(subject).HasInner(type: typeof(CustomException),
							expectations: e => e.HasMessage("inner"));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTypeIsNotAnException_ShouldThrowArgumentException()
				{
					Exception subject = new("outer", new CustomException("inner"));

					async Task Act()
						=> await That(subject).HasInner(typeof(string), e => e.HasMessage("inner"));

					await That(Act).Throws<ArgumentException>()
						.WithParamName("type").And
						.WithMessage("The 'type' must be an exception type, but string is not.").AsPrefix()
						.Because("no exception could ever be a string");
				}

				[Fact]
				public async Task WhenTypeIsNull_ShouldThrowArgumentNullException()
				{
					Exception subject = new("outer", new CustomException("inner"));

					async Task Act()
						=> await That(subject).HasInner((System.Type)null!, e => e.HasMessage("inner"));

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("type").And
						.WithMessage("The 'type' cannot be null.").AsPrefix();
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
						=> await That(subject).HasInner(typeof(CustomException));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an inner ThatException.CustomException,
						             but it had an inner Exception:
						               inner
						             """);
				}


				[Fact]
				public async Task WhenInnerExceptionIsNotOfTheOpenGenericType_ShouldFail()
				{
					Exception subject = new("outer", new Exception("inner"));

					async Task Act()
						=> await That(subject).HasInner(typeof(GenericException<>));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an inner ThatException.GenericException<>,
						             but it had an inner Exception:
						               inner
						             """);
				}

				[Fact]
				public async Task WhenInnerExceptionIsOfAnOpenGenericType_ShouldSucceed()
				{
					Exception subject = new("outer", new GenericException<int>("inner"));

					async Task Act()
						=> await That(subject).HasInner(typeof(GenericException<>));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenInnerExceptionMeetsType_ShouldSucceed()
				{
					Exception subject = new("outer",
						new CustomException("inner"));

					async Task Act()
						=> await That(subject).HasInner(typeof(CustomException));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					Exception? subject = null;

					async Task Act()
						=> await That(subject).HasInner(typeof(CustomException));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an inner ThatException.CustomException,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenTypeIsANamedArgument_ShouldSucceed()
				{
					Exception subject = new("outer",
						new CustomException("inner"));

					async Task Act()
						=> await That(subject).HasInner(type: typeof(CustomException));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTypeIsNotAnException_ShouldThrowArgumentException()
				{
					Exception subject = new("outer", new CustomException("inner"));

					async Task Act()
						=> await That(subject).HasInner(typeof(string));

					await That(Act).Throws<ArgumentException>()
						.WithParamName("type").And
						.WithMessage("The 'type' must be an exception type, but string is not.").AsPrefix()
						.Because("no exception could ever be a string");
				}

				[Fact]
				public async Task WhenTypeIsNull_ShouldThrowArgumentNullException()
				{
					Exception subject = new("outer", new CustomException("inner"));

					async Task Act()
						=> await That(subject).HasInner((System.Type)null!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("type").And
						.WithMessage("The 'type' cannot be null.").AsPrefix();
				}
			}

			public sealed class NegatedExpectationsTests
			{
				[Fact]
				public async Task WhenInnerExceptionHasCorrectMessageButUnexpectedType_ShouldSucceed()
				{
					Exception subject = new("outer", new Exception("inner"));

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it
							=> it.HasInner(typeof(CustomException), e => e.HasMessage("inner")));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenInnerExceptionHasCorrectTypeAndMessage_ShouldFail()
				{
					Exception subject = new("outer",
						new CustomException("inner"));

					async Task Act()
						=> await That(subject)
							.DoesNotComplyWith(it => it.HasInner(typeof(CustomException), e => e.HasMessage("inner")));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have an inner ThatException.CustomException whose message is equal to "inner",
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
							=> it.HasInner(typeof(CustomException), e => e.HasMessage("some other message")));

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
						=> await That(subject).DoesNotComplyWith(it => it.HasInner(typeof(CustomException)));

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
						=> await That(subject).DoesNotComplyWith(it => it.HasInner(typeof(CustomException)));

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
#pragma warning restore CA2263
	}
}
