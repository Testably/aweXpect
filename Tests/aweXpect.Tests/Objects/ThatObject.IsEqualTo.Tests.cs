namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsEqualTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task SubjectToItself_ShouldSucceed()
			{
				object subject = new MyClass();

				async Task Act()
					=> await That(subject).IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task SubjectToSomeOtherValue_ShouldFail()
			{
				object subject = new MyClass();
				object expected = new MyClass();

				async Task Act()
					=> await That(subject).IsEqualTo(expected)
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to ThatObject.MyClass {
					                 Value = 0
					               }, because we want to test the failure,
					             but it was ThatObject.MyClass {
					                 Value = 0
					               }
					             """);
			}

			[Fact]
			public async Task WhenEqualsThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("equals failed");
				ThrowingEqualsClass subject = new(exception);
				ThrowingEqualsClass expected = new(exception);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to ThatObject.ThrowingEqualsClass { },
					             but it did throw an InvalidOperationException:
					               equals failed
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("an Equals that throws fails the expectation instead of aborting its evaluation");
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreNull_ShouldSucceed()
			{
				MyClass? subject = null;
				MyClass? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				MyClass? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo(new MyClass());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to ThatObject.MyClass {
					                 Value = 0
					               },
					             but it was <null>
					             """);
			}
		}

		public sealed class StructTests
		{
			[Fact]
			public async Task SubjectToItself_ShouldSucceed()
			{
				MyStruct subject = new()
				{
					Value = 1,
				};

				async Task Act()
					=> await That(subject).IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task SubjectToSomeOtherValue_ShouldFail()
			{
				MyStruct subject = new()
				{
					Value = 1,
				};
				MyStruct expected = new()
				{
					Value = 2,
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected)
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to ThatObject.MyStruct {
					                 Value = 2
					               }, because we want to test the failure,
					             but it was ThatObject.MyStruct {
					                 Value = 1
					               }
					             """);
			}
		}

		public sealed class NumericTests
		{
			[Fact]
			public async Task WhenSubjectAndExpectedAreDifferentNumericsWithDifferentValues_ShouldFail()
			{
				object subject = -1;

				async Task Act()
					=> await That(subject).IsEqualTo(uint.MaxValue);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 4294967295,
					             but it was -1
					             """)
					.Because("-1 does not fit into a uint and must not wrap around to uint.MaxValue");
			}

			[Theory]
			[MemberData(nameof(GetValues), DisableDiscoveryEnumeration = true)]
			public async Task WhenSubjectAndExpectedAreDifferentNumericsWithSameValues_ShouldSucceed(object subject,
				object expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			public static TheoryData<object, object> GetValues() => new()
			{
				{
					1.0, 1
				},
				{
					2, 2.0
				},
				{
					3, (long)3
				},
				{
					(sbyte)4, (ulong)4
				},
				{
					(float)5.0, 5.0
				},
				{
					(decimal)6.1, (float)6.1
				},
				{
					(byte)7, (short)7
				},
				{
					(ushort)8, (uint)8
				},
#if NET8_0_OR_GREATER
				{
					(Int128)9, (UInt128)9
				},
				{
					(nint)10, 10
				},
				{
					(nuint)11, 11.0
				},
				{
					(Half)10, (float)10
				},
#endif
			};
		}

		public sealed class NullableStructTests
		{
			[Fact]
			public async Task SubjectToItself_ShouldSucceed()
			{
				MyStruct? subject = new()
				{
					Value = 1,
				};

				async Task Act()
					=> await That(subject).IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task SubjectToSomeOtherValue_ShouldFail()
			{
				MyStruct? subject = new()
				{
					Value = 1,
				};
				MyStruct? expected = new()
				{
					Value = 2,
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected)
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to ThatObject.MyStruct {
					                 Value = 2
					               }, because we want to test the failure,
					             but it was ThatObject.MyStruct {
					                 Value = 1
					               }
					             """);
			}

			[Fact]
			public async Task WhenSubjectAndExpectedIsNull_ShouldSucceed()
			{
				MyStruct? subject = null;
				MyStruct? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				MyStruct? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo(new MyStruct());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to ThatObject.MyStruct {
					                 Value = 0
					               },
					             but it was <null>
					             """);
			}
		}

		public sealed class TypeEqualsTests
		{
			[Fact]
			public async Task WhenValuesAreDifferent_ShouldFail()
			{
				Type sut = typeof(long);
				Type expected = typeof(int);

				async Task Act() => await That(sut).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that sut
					             is equal to int,
					             but it was long
					             """);
			}

			[Fact]
			public async Task WhenValuesAreSame_ShouldSucceed()
			{
				Type sut = typeof(float);
				Type expected = typeof(float);

				async Task Act() => await That(sut).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
