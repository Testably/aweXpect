using aweXpect.Results;
using aweXpect.Signaling;

namespace aweXpect.Core.Tests.Results;

public sealed partial class PropertyResultTests
{
	public sealed class StringTests
	{
		[Theory]
		[InlineData("foo", "foobar")]
		[InlineData("foo", "bar")]
		[InlineData("foo", "FOO")]
		public async Task Containing_ShouldFailWhenActualDoesNotContainExpected(string actual, string expected)
		{
			StringProperty sut = MyClass.HasStringValue(actual);

			async Task Act()
				=> await sut.Containing(expected);

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that subject
				              has string value containing "{expected}",
				              but it was "{actual}"*
				              """).AsWildcard();
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public async Task Containing_ShouldSupportIgnoringCase(bool ignoringCase)
		{
			StringProperty sut = MyClass.HasStringValue("something with foo in it");

			async Task Act()
				=> await sut.Containing("FOO").IgnoringCase(ignoringCase);

			await That(Act).Throws<XunitException>()
				.OnlyIf(!ignoringCase)
				.WithMessage("""
				             Expected that subject
				             has string value containing "FOO",
				             but it was "something with foo in it"
				             """);
		}

		[Fact]
		public async Task Containing_ShouldTriggerValidation()
		{
			Signaler<string?> signal = new();
			PropertyResult.String<string, string, IThat<string>> sut = new(new Dummy(), _ => "x", "y", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.Containing("foo");

			await That(signal).Signaled().With(e => e == "foo");
		}

		[Theory]
		[InlineData("foo", "foo")]
		[InlineData("foobar", "oob")]
		public async Task Containing_ShouldVerifyThatActualContainsExpected(string actual, string expected)
		{
			StringProperty sut = MyClass.HasStringValue(actual);

			MyClass? result = await sut.Containing(expected);

			await That(result?.StringValue).IsEqualTo(actual);
		}

		[Fact]
		public async Task Containing_WhenSubjectIsNull_ShouldFail()
		{
			StringProperty sut = MyClass.HasStringValueOfNullSubject();

			async Task Act()
				=> await sut.Containing("foo");

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has string value containing "foo",
				             but it was <null>
				             """);
		}

		[Theory]
		[InlineData("foo", "bar")]
		[InlineData("foo", "FOO")]
		[InlineData("foo", "foo2")]
		[InlineData("foo", "2foo")]
		[InlineData("foo2", "foo")]
		[InlineData("2foo", "foo")]
		public async Task EqualTo_ShouldFailWhenActualDoesNotEqualExpected(string actual, string expected)
		{
			StringProperty sut = MyClass.HasStringValue(actual);

			async Task Act()
				=> await sut.EqualTo(expected);

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that subject
				              has string value equal to "{expected}",
				              but it was "{actual}"*
				              """).AsWildcard();
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public async Task EqualTo_ShouldSupportIgnoringCase(bool ignoringCase)
		{
			StringProperty sut = MyClass.HasStringValue("foo");

			async Task Act()
				=> await sut.EqualTo("FOO").IgnoringCase(ignoringCase);

			await That(Act).Throws<XunitException>()
				.OnlyIf(!ignoringCase)
				.WithMessage("""
				             Expected that subject
				             has string value equal to "FOO",
				             but it was "foo" which differs at index 0:
				                ↓ (actual)
				               "foo"
				               "FOO"
				                ↑ (expected)
				             """);
		}

		[Fact]
		public async Task EqualTo_ShouldSupportMatchTypes()
		{
			StringProperty sut = MyClass.HasStringValue("foo-bar");

			MyClass? result = await sut.EqualTo("foo*").AsWildcard();

			await That(result?.StringValue).IsEqualTo("foo-bar")
				.Because("the continuation exposes the As… family of StringEqualityTypeResult");
		}

		[Fact]
		public async Task EqualTo_ShouldTriggerValidation()
		{
			Signaler<string?> signal = new();
			PropertyResult.String<string, string, IThat<string>> sut = new(new Dummy(), _ => "x", "y", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.EqualTo("foo");

			await That(signal).Signaled().With(e => e == "foo");
		}

		[Fact]
		public async Task EqualTo_ShouldVerifyThatActualIsEqualToExpected()
		{
			StringProperty sut = MyClass.HasStringValue("foo");

			MyClass? result = await sut.EqualTo("foo");

			await That(result?.StringValue).IsEqualTo("foo");
		}

		[Fact]
		public async Task EqualTo_WhenSubjectIsNull_ShouldFail()
		{
			StringProperty sut = MyClass.HasStringValueOfNullSubject();

			async Task Act()
				=> await sut.EqualTo("foo");

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has string value equal to "foo",
				             but it was <null>
				             """);
		}

		[Theory]
		[InlineData("foo", "foo")]
		[InlineData("foobar", "oob")]
		public async Task NotContaining_ShouldFailWhenActualContainsExpected(string actual, string expected)
		{
			StringProperty sut = MyClass.HasStringValue(actual);

			async Task Act()
				=> await sut.NotContaining(expected);

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that subject
				              has string value not containing "{expected}",
				              but it was "{actual}"
				              """);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public async Task NotContaining_ShouldSupportIgnoringCase(bool ignoringCase)
		{
			StringProperty sut = MyClass.HasStringValue("something with foo in it");

			async Task Act()
				=> await sut.NotContaining("FOO").IgnoringCase(ignoringCase);

			await That(Act).Throws<XunitException>()
				.OnlyIf(ignoringCase)
				.WithMessage("""
				             Expected that subject
				             has string value not containing "FOO" ignoring case,
				             but it was "something with foo in it"
				             """);
		}

		[Fact]
		public async Task NotContaining_ShouldTriggerValidation()
		{
			Signaler<string?> signal = new();
			PropertyResult.String<string, string, IThat<string>> sut = new(new Dummy(), _ => "x", "y", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.NotContaining("foo");

			await That(signal).Signaled().With(e => e == "foo");
		}

		[Theory]
		[InlineData("foo", "foobar")]
		[InlineData("foo", "bar")]
		[InlineData("foo", "FOO")]
		public async Task NotContaining_ShouldVerifyThatActualDoesNotContainExpected(string actual, string expected)
		{
			StringProperty sut = MyClass.HasStringValue(actual);

			MyClass? result = await sut.NotContaining(expected);

			await That(result?.StringValue).IsEqualTo(actual);
		}

		[Fact]
		public async Task NotEqualTo_ShouldFailWhenActualDoesNotEqualExpected()
		{
			StringProperty sut = MyClass.HasStringValue("foo");

			async Task Act()
				=> await sut.NotEqualTo("foo");

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has string value not equal to "foo",
				             but it was "foo"
				             """);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public async Task NotEqualTo_ShouldSupportIgnoringCase(bool ignoringCase)
		{
			StringProperty sut = MyClass.HasStringValue("foo");

			async Task Act()
				=> await sut.NotEqualTo("FOO").IgnoringCase(ignoringCase);

			await That(Act).Throws<XunitException>()
				.OnlyIf(ignoringCase)
				.WithMessage("""
				             Expected that subject
				             has string value not equal to "FOO" ignoring case,
				             but it was "foo"
				             """);
		}

		[Fact]
		public async Task NotEqualTo_ShouldTriggerValidation()
		{
			Signaler<string?> signal = new();
			PropertyResult.String<string, string, IThat<string>> sut = new(new Dummy(), _ => "x", "y", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.NotEqualTo("foo");

			await That(signal).Signaled().With(e => e == "foo");
		}

		[Theory]
		[InlineData("foo", "bar")]
		[InlineData("foo", "FOO")]
		[InlineData("foo", "foo2")]
		[InlineData("foo", "2foo")]
		[InlineData("foo2", "foo")]
		[InlineData("2foo", "foo")]
		public async Task NotEqualTo_ShouldVerifyThatActualIsNotEqualToExpected(string actual, string expected)
		{
			StringProperty sut = MyClass.HasStringValue(actual);

			MyClass? result = await sut.NotEqualTo(expected);

			await That(result?.StringValue).IsEqualTo(actual);
		}

		[Fact]
		public async Task NotEqualTo_WhenSubjectIsNullAndUnexpectedIsNull_ShouldFail()
		{
			StringProperty sut = MyClass.HasStringValueOfNullSubject();

			async Task Act()
				=> await sut.NotEqualTo(null);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has string value not equal to <null>,
				             but it was <null>
				             """)
				.Because("a null subject has no string value to compare, whatever the unexpected value is");
		}

		[Fact]
		public async Task NotEqualTo_WhenSubjectIsNull_ShouldFail()
		{
			StringProperty sut = MyClass.HasStringValueOfNullSubject();

			async Task Act()
				=> await sut.NotEqualTo("foo");

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has string value not equal to "foo",
				             but it was <null>
				             """);
		}

		public sealed class ContextTests
		{
			[Fact]
			public async Task WhenIncludedTwiceWithTheSameValue_ShouldAppendTheValueOnlyOnce()
			{
				IThat<MyClass?> source = MyClass.WithStringValue("foo-bar");
				IThat<MyClass?> afterFirstProperty = MyClass.StringValueOf(source, true).Containing("foo").And;

				async Task Act()
					=> await MyClass.StringValueOf(afterFirstProperty, true).Containing("baz");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has string value containing "foo" and has string value containing "baz",
					             but it was "foo-bar"

					             string value:
					             foo-bar
					             """)
					.Because("a chained expectation over the same property must not repeat the identical block");
			}

			[Fact]
			public async Task WhenIncluded_ShouldAppendTheValue()
			{
				StringProperty sut = MyClass.StringValueOf(MyClass.WithStringValue("foo"), true);

				async Task Act()
					=> await sut.EqualTo("bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has string value equal to "bar",
					             but it was "foo"*

					             string value:
					             foo
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNotIncluded_ShouldNotAppendTheValue()
			{
				StringProperty sut = MyClass.StringValueOf(MyClass.WithStringValue("foo"));

				async Task Act()
					=> await sut.EqualTo("bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has string value equal to "bar",
					             but it was "foo" which differs at index 0:
					                ↓ (actual)
					               "foo"
					               "bar"
					                ↑ (expected)
					             """)
					.Because("the value is only appended as context when the property asks for it");
			}

			[Fact]
			public async Task WhenTheValueIsEmpty_ShouldNotAppendTheValue()
			{
				StringProperty sut = MyClass.StringValueOf(MyClass.WithStringValue(""), true);

				async Task Act()
					=> await sut.EqualTo("bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has string value equal to "bar",
					             but it was "" with a length of 0 which is shorter than the expected length of 3 and misses:
					               "bar"
					             """)
					.Because("an empty value would only add an empty block");
			}
		}

		public sealed class GrammarTests
		{
			[Fact]
			public async Task WhenActive_ShouldUseTheActiveVoice()
			{
				PropertyResult.String<MyClass?, MyClass?, IThat<MyClass?>> sut =
					MyClass.HasStringValue("foo", ExpectationGrammars.Active);

				async Task Act()
					=> await sut.EqualTo("bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             with string value equal to "bar",
					             but it was "foo"*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNested_ShouldReadAsAStatementAboutTheProperty()
			{
				PropertyResult.String<MyClass?, MyClass?, IThat<MyClass?>> sut =
					MyClass.HasStringValue("foo", ExpectationGrammars.Nested);

				async Task Act()
					=> await sut.EqualTo("bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             string value is equal to "bar",
					             but it was "foo"*
					             """).AsWildcard();
			}
		}

		public sealed class NarrowedTypeTests
		{
			[Fact]
			public async Task WhenTheMapperIsTypedAtTheBaseType_ShouldFailForAMismatch()
			{
				async Task Act()
					=> await MyClass.HasStringValueOfNarrowedSubject("foo").EqualTo("bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has string value equal to "bar",
					             but it was "foo"*
					             """).AsWildcard()
					.Because("a constraint typed at the narrowed type would silently never be matched");
			}

			[Fact]
			public async Task WhenTheMapperIsTypedAtTheBaseType_ShouldReturnTheNarrowedType()
			{
				MyDerivedClass? result = await MyClass.HasStringValueOfNarrowedSubject("foo").EqualTo("foo");

				await That(result?.StringValue).IsEqualTo("foo");
			}
		}
	}
}
