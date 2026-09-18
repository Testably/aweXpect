using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsExactly
	{
		public sealed class GenericTests
		{
			[Theory]
			[AutoData]
			public async Task WhenAwaited_ShouldReturnTypedResult(int value)
			{
				object subject = new MyClass
				{
					Value = value,
				};

				MyClass result = await That(subject).IsExactly<MyClass>();

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task WhenCalledFromGenericMethodWithUnconstrainedT_AndTypeMismatch_ShouldFail()
			{
				async Task Act()
					=> await AssertIsExactlyString(42);

				await That(Act).Throws<XunitException>();

				static async Task AssertIsExactlyString<T>(T value)
					=> await That(value).IsExactly<string>();
			}

			[Fact]
			public async Task WhenCalledFromGenericMethodWithUnconstrainedT_ShouldResolve()
			{
				async Task Act()
					=> await AssertIsExactlyString("foo");

				await That(Act).DoesNotThrow();

				static async Task AssertIsExactlyString<T>(T value)
					=> await That(value).IsExactly<string>();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				object? subject = null;

				async Task Act()
					=> await That(subject).IsExactly<MyClass>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is exactly type ThatObject.MyClass,
					             but it was <null>
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenTypeDoesNotMatch_ShouldFail(int value)
			{
				object subject = new MyClass
				{
					Value = value,
				};

				async Task Act()
					=> await That(subject).IsExactly<OtherClass>()
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage($$"""
					               Expected that subject
					               is exactly type ThatObject.OtherClass, because we want to test the failure,
					               but it was ThatObject.MyClass

					               Actual:
					               ThatObject.MyClass {
					                 Value = {{value}}
					               }
					               """);
			}

			[Theory]
			[AutoData]
			public async Task WhenTypeIsSubtype_ShouldFail(int value)
			{
				object subject = new MyClass
				{
					Value = value,
				};

				async Task Act()
					=> await That(subject).IsExactly<MyBaseClass>()
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage($$"""
					               Expected that subject
					               is exactly type ThatObject.MyBaseClass, because we want to test the failure,
					               but it was ThatObject.MyClass

					               Actual:
					               ThatObject.MyClass {
					                 Value = {{value}}
					               }
					               """);
			}

			[Theory]
			[AutoData]
			public async Task WhenTypeIsSupertype_ShouldFail(int value, string reason)
			{
				object subject = new MyBaseClass
				{
					Value = value,
				};

				async Task Act()
					=> await That(subject).IsExactly<MyClass>()
						.Because(reason);

				await That(Act).Throws<XunitException>()
					.WithMessage($$"""
					               Expected that subject
					               is exactly type ThatObject.MyClass, because {{reason}},
					               but it was ThatObject.MyBaseClass

					               Actual:
					               ThatObject.MyBaseClass {
					                 Value = {{value}}
					               }
					               """);
			}

			[Fact]
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
			[Theory]
			[AutoData]
			public async Task WhenAwaited_ShouldReturnTypedResult(int value)
			{
				object subject = new MyClass
				{
					Value = value,
				};

				object? result = await That(subject).IsExactly(typeof(MyClass));

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				object? subject = null;

				async Task Act()
					=> await That(subject).IsExactly(typeof(MyClass));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is exactly type ThatObject.MyClass,
					             but it was <null>
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenTypeDoesNotMatch_ShouldFail(int value)
			{
				object subject = new MyClass
				{
					Value = value,
				};

				async Task Act()
					=> await That(subject).IsExactly(typeof(OtherClass))
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage($$"""
					               Expected that subject
					               is exactly type ThatObject.OtherClass, because we want to test the failure,
					               but it was ThatObject.MyClass

					               Actual:
					               ThatObject.MyClass {
					                 Value = {{value}}
					               }
					               """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldThrowArgumentNullException()
			{
				object subject = new MyClass();

				async Task Act()
					=> await That(subject).IsExactly(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("type").And
					.WithMessage("The type cannot be null.").AsPrefix();
			}

			[Theory]
			[AutoData]
			public async Task WhenTypeIsSubtype_ShouldSucceed(int value)
			{
				object subject = new MyClass
				{
					Value = value,
				};

				async Task Act()
					=> await That(subject).IsExactly(typeof(MyBaseClass))
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage($$"""
					               Expected that subject
					               is exactly type ThatObject.MyBaseClass, because we want to test the failure,
					               but it was ThatObject.MyClass

					               Actual:
					               ThatObject.MyClass {
					                 Value = {{value}}
					               }
					               """);
			}

			[Theory]
			[AutoData]
			public async Task WhenTypeIsSupertype_ShouldFail(int value, string reason)
			{
				object subject = new MyBaseClass
				{
					Value = value,
				};

				async Task Act()
					=> await That(subject).IsExactly(typeof(MyClass))
						.Because(reason);

				await That(Act).Throws<XunitException>()
					.WithMessage($$"""
					               Expected that subject
					               is exactly type ThatObject.MyClass, because {{reason}},
					               but it was ThatObject.MyBaseClass

					               Actual:
					               ThatObject.MyBaseClass {
					                 Value = {{value}}
					               }
					               """);
			}

			[Fact]
			public async Task WhenTypeMatches_ShouldSucceed()
			{
				object subject = new MyClass();

				async Task Act()
					=> await That(subject).IsExactly(typeof(MyClass));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMatchingOpenGenericInterfaceType_ShouldFail()
			{
				List<string> subject = new();

				async Task Act()
					=> await That(subject).IsExactly(typeof(IList<>));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is exactly type IList<>,
					             but it was List<string>

					             Actual:
					             []
					             """);
			}

			[Fact]
			public async Task WithMatchingOpenGenericType_ShouldSucceed()
			{
				List<string> subject = new();

				async Task Act()
					=> await That(subject).IsExactly(typeof(List<>));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithNotMatchingOpenGenericInterfaceType_ShouldFail()
			{
				List<string> subject = new();

				async Task Act()
					=> await That(subject).IsExactly(typeof(IDictionary<,>));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is exactly type IDictionary<, >,
					             but it was List<string>

					             Actual:
					             []
					             """);
			}
		}
	}
}
