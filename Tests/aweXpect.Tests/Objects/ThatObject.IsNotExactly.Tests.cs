namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed class IsNotExactly
	{
		public sealed class GenericTests
		{
			[Theory]
			[AutoData]
			public async Task WhenAwaited_ShouldReturnObjectResult(int value)
			{
				object subject = new MyClass
				{
					Value = value,
				};

				object? result = await That(subject).IsNotExactly<OtherClass>();

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task WhenCalledFromGenericMethodWithUnconstrainedT_AndTypeMatches_ShouldFail()
			{
				async Task Act()
					=> await AssertIsNotExactlyString("foo");

				await That(Act).Throws<XunitException>();

				static async Task AssertIsNotExactlyString<T>(T value)
					=> await That(value).IsNotExactly<string>();
			}

			[Fact]
			public async Task WhenCalledFromGenericMethodWithUnconstrainedT_AndTypeMismatch_ShouldResolve()
			{
				async Task Act()
					=> await AssertIsNotExactlyString(42);

				await That(Act).DoesNotThrow();

				static async Task AssertIsNotExactlyString<T>(T value)
					=> await That(value).IsNotExactly<string>();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				object? subject = null;

				async Task Act()
					=> await That(subject).IsNotExactly<MyClass>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not exactly type ThatObject.MyClass,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeDoesNotMatch_ShouldSucceed()
			{
				object subject = new MyClass();

				async Task Act()
					=> await That(subject).IsNotExactly<OtherClass>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsSubtype_ShouldSucceed()
			{
				object subject = new MyClass();

				async Task Act()
					=> await That(subject).IsNotExactly<MyBaseClass>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsSupertype_ShouldSucceed()
			{
				object subject = new MyBaseClass();

				async Task Act()
					=> await That(subject).IsNotExactly<MyClass>();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenTypeMatches_ShouldFail(int value, string reason)
			{
				object subject = new MyClass
				{
					Value = value,
				};

				async Task Act()
					=> await That(subject).IsNotExactly<MyClass>()
						.Because(reason);

				await That(Act).Throws<XunitException>()
					.WithMessage($$"""
					               Expected that subject
					               is not exactly type ThatObject.MyClass, because {{reason}},
					               but it was ThatObject.MyClass {
					                   Value = {{value}}
					                 }
					               """);
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

				object? result = await That(subject).IsNotExactly(typeof(OtherClass));

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				object? subject = null;

				async Task Act()
					=> await That(subject).IsNotExactly(typeof(MyClass));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeDoesNotMatch_ShouldSucceed()
			{
				object subject = new MyClass();

				async Task Act()
					=> await That(subject).IsNotExactly(typeof(OtherClass));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldThrowArgumentNullException()
			{
				object subject = new MyClass();

				async Task Act()
					=> await That(subject).IsNotExactly(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("type").And
					.WithMessage("The type cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenTypeIsSubtype_ShouldSucceed()
			{
				object subject = new MyClass();

				async Task Act()
					=> await That(subject).IsNotExactly(typeof(MyBaseClass));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsSupertype_ShouldSucceed()
			{
				object subject = new MyBaseClass();

				async Task Act()
					=> await That(subject).IsNotExactly(typeof(MyClass));

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenTypeMatches_ShouldFail(int value, string reason)
			{
				object subject = new MyClass
				{
					Value = value,
				};

				async Task Act()
					=> await That(subject).IsNotExactly(typeof(MyClass))
						.Because(reason);

				await That(Act).Throws<XunitException>()
					.WithMessage($$"""
					               Expected that subject
					               is not exactly type ThatObject.MyClass, because {{reason}},
					               but it was ThatObject.MyClass {
					                   Value = {{value}}
					                 }
					               """);
			}
		}
	}
}
