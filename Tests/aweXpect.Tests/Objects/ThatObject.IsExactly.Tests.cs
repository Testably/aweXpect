using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsExactly
	{
		public sealed class GenericTests
		{
			[Test]
			[AutoArguments]
			public async Task WhenAwaited_ShouldReturnTypedResult(int value)
			{
				object subject = new MyClass
				{
					Value = value,
				};

				MyClass result = await That(subject).IsExactly<MyClass>();

				await That(result).IsSameAs(subject);
			}

			[Test]
			public async Task WhenCalledFromGenericMethodWithUnconstrainedT_AndTypeMismatch_ShouldFail()
			{
				async Task Act()
					=> await AssertIsExactlyString(42);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that value
					             is exactly of type string,
					             but it was int

					             Actual:
					             42
					             """);

				static async Task AssertIsExactlyString<T>(T value)
					=> await That(value).IsExactly<string>();
			}

			[Test]
			public async Task WhenCalledFromGenericMethodWithUnconstrainedT_ShouldResolve()
			{
				async Task Act()
					=> await AssertIsExactlyString("foo");

				await That(Act).DoesNotThrow();

				static async Task AssertIsExactlyString<T>(T value)
					=> await That(value).IsExactly<string>();
			}

			[Test]
			public async Task WhenCombinedWithOr_AndOnlyLeftIsMet_ShouldReturnNull()
			{
				object subject = new MyClass();

#pragma warning disable aweXpect0008
				OtherClass result = await That(subject).IsExactly<MyClass>().Or.IsExactly<OtherClass>();
#pragma warning restore aweXpect0008

				await That(result).IsNull();
			}

			[Test]
			public async Task WhenCombinedWithOr_AndOnlyRightIsMet_ShouldReturnSubject()
			{
				object subject = new MyClass();

#pragma warning disable aweXpect0008
				MyClass result = await That(subject).IsExactly<OtherClass>().Or.IsExactly<MyClass>();
#pragma warning restore aweXpect0008

				await That(result).IsSameAs(subject);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				object? subject = null;

				async Task Act()
					=> await That(subject).IsExactly<MyClass>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is exactly of type ThatObject.MyClass,
					             but it was <null>
					             """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenTypeDoesNotMatch_ShouldFail(int value)
			{
				object subject = new MyClass
				{
					Value = value,
				};

				async Task Act()
					=> await That(subject).IsExactly<OtherClass>()
						.Because("we want to test the failure");

				await That(Act).Throws<FailException>()
					.WithMessage($$"""
					               Expected that subject
					               is exactly of type ThatObject.OtherClass, because we want to test the failure,
					               but it was ThatObject.MyClass

					               Actual:
					               ThatObject.MyClass {
					                 Value = {{value}}
					               }
					               """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenTypeIsSubtype_ShouldFail(int value)
			{
				object subject = new MyClass
				{
					Value = value,
				};

				async Task Act()
					=> await That(subject).IsExactly<MyBaseClass>()
						.Because("we want to test the failure");

				await That(Act).Throws<FailException>()
					.WithMessage($$"""
					               Expected that subject
					               is exactly of type ThatObject.MyBaseClass, because we want to test the failure,
					               but it was ThatObject.MyClass

					               Actual:
					               ThatObject.MyClass {
					                 Value = {{value}}
					               }
					               """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenTypeIsSupertype_ShouldFail(int value, string reason)
			{
				object subject = new MyBaseClass
				{
					Value = value,
				};

				async Task Act()
					=> await That(subject).IsExactly<MyClass>()
						.Because(reason);

				await That(Act).Throws<FailException>()
					.WithMessage($$"""
					               Expected that subject
					               is exactly of type ThatObject.MyClass, because {{reason}},
					               but it was ThatObject.MyBaseClass

					               Actual:
					               ThatObject.MyBaseClass {
					                 Value = {{value}}
					               }
					               """);
			}

			[Test]
			public async Task WhenTypeMatches_ShouldSucceed()
			{
				object subject = new MyClass();

				async Task Act()
					=> await That(subject).IsExactly<MyClass>();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class TypeTests
		{
			[Test]
			[AutoArguments]
			public async Task WhenAwaited_ShouldReturnTypedResult(int value)
			{
				object subject = new MyClass
				{
					Value = value,
				};

				object? result = await That(subject).IsExactly(typeof(MyClass));

				await That(result).IsSameAs(subject);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				object? subject = null;

				async Task Act()
					=> await That(subject).IsExactly(typeof(MyClass));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is exactly of type ThatObject.MyClass,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNullableValueType_ShouldSucceedForTheUnderlyingType()
			{
				int? subject = 5;

				async Task Act()
					=> await That(subject).IsExactly(typeof(int));

				await That(Act).DoesNotThrow()
					.Because("a value-type subject can be checked against a runtime type without a cast to object");
			}

			[Test]
			[AutoArguments]
			public async Task WhenSubjectIsTyped_ShouldAllowChainingOnTheSubjectType(int value)
			{
				MyClass subject = new()
				{
					Value = value,
				};

				async Task Act()
					=> await That(subject).IsExactly(typeof(MyClass))
						.And.Whose(x => x.Value, x => x.IsEqualTo(value));

				await That(Act).DoesNotThrow()
					.Because("the chain must continue with the subject type instead of widening it to object");
			}

			[Test]
			[AutoArguments]
			public async Task WhenSubjectIsTyped_ShouldReturnTheSubjectType(int value)
			{
				MyClass subject = new()
				{
					Value = value,
				};

				MyClass? result = await That(subject).IsExactly(typeof(MyClass));

				await That(result).IsSameAs(subject)
					.Because("the awaited result must keep the subject type instead of widening it to object");
			}

			[Test]
			[AutoArguments]
			public async Task WhenTypeDoesNotMatch_ShouldFail(int value)
			{
				object subject = new MyClass
				{
					Value = value,
				};

				async Task Act()
					=> await That(subject).IsExactly(typeof(OtherClass))
						.Because("we want to test the failure");

				await That(Act).Throws<FailException>()
					.WithMessage($$"""
					               Expected that subject
					               is exactly of type ThatObject.OtherClass, because we want to test the failure,
					               but it was ThatObject.MyClass

					               Actual:
					               ThatObject.MyClass {
					                 Value = {{value}}
					               }
					               """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldThrowArgumentNullException()
			{
				object subject = new MyClass();

				async Task Act()
					=> await That(subject).IsExactly(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("type").And
					.WithMessage("The 'type' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenTypeIsNullableAndSubjectHasAValue_ShouldSucceed()
			{
				int? subject = 5;

				async Task Act()
					=> await That(subject).IsExactly(typeof(int?));

				await That(Act).DoesNotThrow()
					.Because("a boxed nullable value has the underlying type, so the nullable type is compared by it");
			}

			[Test]
			[AutoArguments]
			public async Task WhenTypeIsSubtype_ShouldFail(int value)
			{
				object subject = new MyClass
				{
					Value = value,
				};

				async Task Act()
					=> await That(subject).IsExactly(typeof(MyBaseClass))
						.Because("we want to test the failure");

				await That(Act).Throws<FailException>()
					.WithMessage($$"""
					               Expected that subject
					               is exactly of type ThatObject.MyBaseClass, because we want to test the failure,
					               but it was ThatObject.MyClass

					               Actual:
					               ThatObject.MyClass {
					                 Value = {{value}}
					               }
					               """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenTypeIsSupertype_ShouldFail(int value, string reason)
			{
				object subject = new MyBaseClass
				{
					Value = value,
				};

				async Task Act()
					=> await That(subject).IsExactly(typeof(MyClass))
						.Because(reason);

				await That(Act).Throws<FailException>()
					.WithMessage($$"""
					               Expected that subject
					               is exactly of type ThatObject.MyClass, because {{reason}},
					               but it was ThatObject.MyBaseClass

					               Actual:
					               ThatObject.MyBaseClass {
					                 Value = {{value}}
					               }
					               """);
			}

			[Test]
			public async Task WhenTypeMatches_ShouldSucceed()
			{
				object subject = new MyClass();

				async Task Act()
					=> await That(subject).IsExactly(typeof(MyClass));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMatchingOpenGenericInterfaceType_ShouldFail()
			{
				List<string> subject = new();

				async Task Act()
					=> await That(subject).IsExactly(typeof(IList<>));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is exactly of type IList<>,
					             but it was List<string>

					             Actual:
					             []
					             """);
			}

			[Test]
			public async Task WithMatchingOpenGenericType_ShouldSucceed()
			{
				List<string> subject = new();

				async Task Act()
					=> await That(subject).IsExactly(typeof(List<>));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithNotMatchingOpenGenericInterfaceType_ShouldFail()
			{
				List<string> subject = new();

				async Task Act()
					=> await That(subject).IsExactly(typeof(IDictionary<,>));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is exactly of type IDictionary<,>,
					             but it was List<string>

					             Actual:
					             []
					             """);
			}

			[Test]
			public async Task WithOpenGenericBaseType_ShouldFail()
			{
				object subject = new MyGenericBaseClass();

				async Task Act()
					=> await That(subject).IsExactly(typeof(List<>))
						.Because("exactly means the type itself, so the base type chain is not walked");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is exactly of type List<>, because exactly means the type itself, so the base type chain is not walked,
					             but it was ThatObject.MyGenericBaseClass

					             Actual:
					             []
					             """);
			}
		}
	}
}
