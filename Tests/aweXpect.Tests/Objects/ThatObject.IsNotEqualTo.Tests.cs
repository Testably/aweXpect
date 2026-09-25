namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task SubjectToItself_ShouldFail()
			{
				object subject = new MyClass();

				async Task Act()
					=> await That(subject).IsNotEqualTo(subject)
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to ThatObject.MyClass {
					                 Value = 0
					               }, because we want to test the failure,
					             but it was ThatObject.MyClass {
					                 Value = 0
					               }
					             """);
			}

			[Fact]
			public async Task SubjectToSomeOtherValue_ShouldSucceed()
			{
				object subject = new MyClass();
				object expected = new MyClass();

				async Task Act()
					=> await That(subject).IsNotEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEqualsThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("equals failed");
				ThrowingEqualsClass subject = new(exception);
				ThrowingEqualsClass unexpected = new(exception);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to ThatObject.ThrowingEqualsClass { },
					             but Equals of ThatObject.ThrowingEqualsClass did throw an InvalidOperationException:
					               equals failed
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("an Equals that threw answered nothing, so the negation fails as well");
			}

			[Fact]
			public async Task WhenSubjectAndExpectedIsNull_ShouldFail()
			{
				MyClass? subject = null;
				MyClass? expected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to <null>,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectAndUnexpectedAreDifferentNumericsWithDifferentValues_ShouldSucceed()
			{
				object subject = -1;

				async Task Act()
					=> await That(subject).IsNotEqualTo(uint.MaxValue);

				await That(Act).DoesNotThrow()
					.Because("-1 does not fit into a uint and must not wrap around to uint.MaxValue");
			}

			[Fact]
			public async Task WhenSubjectAndUnexpectedAreNull_ShouldFail()
			{
				MyClass? subject = null;
				MyClass? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to <null>,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				MyClass? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(new MyClass());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class StructTests
		{
			[Fact]
			public async Task SubjectToItself_ShouldFail()
			{
				MyStruct subject = new()
				{
					Value = 1,
				};
				MyStruct unexpected = new()
				{
					Value = 1,
				};

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected)
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to ThatObject.MyStruct {
					                 Value = 1
					               }, because we want to test the failure,
					             but it was ThatObject.MyStruct {
					                 Value = 1
					               }
					             """);
			}

			[Fact]
			public async Task SubjectToSomeOtherValue_ShouldSucceed()
			{
				MyStruct subject = new()
				{
					Value = 1,
				};
				MyStruct unexpected = new()
				{
					Value = 2,
				};

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NullableStructTests
		{
			[Fact]
			public async Task SubjectToItself_ShouldFail()
			{
				MyStruct? subject = new()
				{
					Value = 1,
				};
				MyStruct? unexpected = new()
				{
					Value = 1,
				};

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected)
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to ThatObject.MyStruct {
					                 Value = 1
					               }, because we want to test the failure,
					             but it was ThatObject.MyStruct {
					                 Value = 1
					               }
					             """);
			}

			[Fact]
			public async Task SubjectToSomeOtherValue_ShouldSucceed()
			{
				MyStruct? subject = new()
				{
					Value = 1,
				};
				MyStruct? unexpected = new()
				{
					Value = 2,
				};

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectAndExpectedIsNull_ShouldFail()
			{
				MyStruct? subject = null;
				MyStruct? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to <null>,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				MyStruct? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(new MyStruct());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class TypeEqualsTests
		{
			[Fact]
			public async Task WhenValuesAreDifferent_ShouldSucceed()
			{
				Type sut = typeof(long);
				Type unexpected = typeof(int);

				async Task Act() => await That(sut).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenValuesAreSame_ShouldFail()
			{
				Type sut = typeof(float);
				Type unexpected = typeof(float);

				async Task Act() => await That(sut).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that sut
					             is not equal to float,
					             but it was float
					             """);
			}
		}
	}
}
