using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class Is
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task ShouldAllowChainingFurtherTypeChecks()
			{
				object subject = "foo";

				async Task Act()
					=> await That(subject).Is<string>().And.IsNot<int>();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenAwaited_ShouldReturnTypedResult(int value)
			{
				object subject = new MyClass
				{
					Value = value,
				};

				MyClass result = await That(subject).Is<MyClass>();

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task WhenCalledFromGenericMethodWithUnconstrainedT_AndTypeMismatch_ShouldFail()
			{
				async Task Act()
					=> await AssertIsString(42);

				await That(Act).Throws<XunitException>();

				static async Task AssertIsString<T>(T value)
					=> await That(value).Is<string>();
			}

			[Fact]
			public async Task WhenCalledFromGenericMethodWithUnconstrainedT_ShouldResolve()
			{
				async Task Act()
					=> await AssertIsString("foo");

				await That(Act).DoesNotThrow();

				static async Task AssertIsString<T>(T value)
					=> await That(value).Is<string>();
			}

			[Fact]
			public async Task WhenNestedInHasInner_ShouldFail()
			{
				Exception subject = new("outer", new ArgumentException("inner"));

				async Task Act()
					=> await That(subject).HasInner(it => it.Is<InvalidCastException>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an inner exception that is of type InvalidCastException,
					             but it was ArgumentException

					             Actual:
					             ArgumentException: inner
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				object? subject = null;

				async Task Act()
					=> await That(subject).Is<MyClass>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type ThatObject.MyClass,
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
					=> await That(subject).Is<OtherClass>()
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage($$"""
					               Expected that subject
					               is of type ThatObject.OtherClass, because we want to test the failure,
					               but it was ThatObject.MyClass

					               Actual:
					               ThatObject.MyClass {
					                 Value = {{value}}
					               }
					               """);
			}

			[Fact]
			public async Task WhenTypeIsSubtype_ShouldSucceed()
			{
				object subject = new MyClass();

				async Task Act()
					=> await That(subject).Is<MyBaseClass>();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).Is<MyClass>()
						.Because(reason);

				await That(Act).Throws<XunitException>()
					.WithMessage($$"""
					               Expected that subject
					               is of type ThatObject.MyClass, because {{reason}},
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
					=> await That(subject).Is<MyClass>();

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

				object? result = await That(subject).Is(typeof(MyClass));

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task WhenSingleItemDoesNotMatch_ShouldFail()
			{
				object[] subject = [new List<int>(),];

				async Task Act()
					=> await That(subject).HasSingle().Which.Is(typeof(IDictionary<,>));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item that is of type IDictionary<,>,
					             but it was List<int>

					             Actual:
					             []
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				object? subject = null;

				async Task Act()
					=> await That(subject).Is(typeof(MyClass));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type ThatObject.MyClass,
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
					=> await That(subject).Is(typeof(OtherClass))
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage($$"""
					               Expected that subject
					               is of type ThatObject.OtherClass, because we want to test the failure,
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
					=> await That(subject).Is(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("type").And
					.WithMessage("The 'type' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenTypeIsSubtype_ShouldSucceed()
			{
				object subject = new MyClass();

				async Task Act()
					=> await That(subject).Is(typeof(MyBaseClass));

				await That(Act).DoesNotThrow();
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
					=> await That(subject).Is(typeof(MyClass))
						.Because(reason);

				await That(Act).Throws<XunitException>()
					.WithMessage($$"""
					               Expected that subject
					               is of type ThatObject.MyClass, because {{reason}},
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
					=> await That(subject).Is(typeof(MyClass));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMatchingOpenGenericBaseType_ShouldSucceed()
			{
				object subject = new MyGenericBaseClass();

				async Task Act()
					=> await That(subject).Is(typeof(List<>));

				await That(Act).DoesNotThrow()
					.Because("a subclass of List<int> is assignable to the open generic List<>");
			}

			[Fact]
			public async Task WithMatchingOpenGenericGrandBaseType_ShouldSucceed()
			{
				object subject = new MyGenericDerivedClass();

				async Task Act()
					=> await That(subject).Is(typeof(List<>));

				await That(Act).DoesNotThrow()
					.Because("the whole base type chain is walked, not only the direct base type");
			}

			[Fact]
			public async Task WithMatchingOpenGenericInterfaceOfDerivedType_ShouldSucceed()
			{
				object subject = new MyGenericDerivedClass();

				async Task Act()
					=> await That(subject).Is(typeof(IEnumerable<>));

				await That(Act).DoesNotThrow()
					.Because("the generic interfaces are inherited from the generic base type");
			}

			[Fact]
			public async Task WithMatchingOpenGenericInterfaceType_ShouldSucceed()
			{
				List<string> subject = new();

				async Task Act()
					=> await That(subject).Is(typeof(IList<>));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMatchingOpenGenericTwoParameterBaseType_ShouldSucceed()
			{
				object subject = new MyDictionaryClass();

				async Task Act()
					=> await That(subject).Is(typeof(Dictionary<,>));

				await That(Act).DoesNotThrow()
					.Because("a subclass of a closed generic base type matches its generic definition");
			}

			[Fact]
			public async Task WithMatchingOpenGenericType_ShouldSucceed()
			{
				List<string> subject = new();

				async Task Act()
					=> await That(subject).Is(typeof(List<>));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMatchingOpenGenericValueType_ShouldSucceed()
			{
				object subject = new KeyValuePair<string, int>("foo", 1);

				async Task Act()
					=> await That(subject).Is(typeof(KeyValuePair<,>));

				await That(Act).DoesNotThrow()
					.Because("a boxed value type matches its own generic definition although its base types are not generic");
			}

			[Fact]
			public async Task WithNotMatchingOpenGenericBaseType_ShouldFail()
			{
				object subject = new MyGenericDerivedClass();

				async Task Act()
					=> await That(subject).Is(typeof(Dictionary<,>))
						.Because("an unrelated open generic type is not in the base type chain");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type Dictionary<,>, because an unrelated open generic type is not in the base type chain,
					             but it was ThatObject.MyGenericDerivedClass

					             Actual:
					             []
					             """);
			}

			[Fact]
			public async Task WithNotMatchingOpenGenericInterfaceType_ShouldFail()
			{
				List<string> subject = new();

				async Task Act()
					=> await That(subject).Is(typeof(IDictionary<,>));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type IDictionary<,>,
					             but it was List<string>

					             Actual:
					             []
					             """);
			}
		}
	}
}
