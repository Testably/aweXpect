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
			PropertyResult.String<MyClass?> sut = MyClass.HasStringValue(actual);

			async Task Act()
				=> await sut.Containing(expected);

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that subject
				              has string value containing "{expected}",
				              but it had string value "{actual}"
				              """);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public async Task Containing_ShouldSupportIgnoringCase(bool ignoringCase)
		{
			PropertyResult.String<MyClass?> sut = MyClass.HasStringValue("something with foo in it");

			async Task Act()
				=> await sut.Containing("FOO").IgnoringCase(ignoringCase);

			await That(Act).Throws<XunitException>()
				.OnlyIf(!ignoringCase)
				.WithMessage("""
				             Expected that subject
				             has string value containing "FOO",
				             but it had string value "something with foo in it"
				             """);
		}

		[Fact]
		public async Task Containing_ShouldTriggerValidation()
		{
			Signaler<string?> signal = new();
			PropertyResult.String<string> sut = new(new Dummy(), _ => "x", "y", (e, _) =>
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
			PropertyResult.String<MyClass?> sut = MyClass.HasStringValue(actual);

			MyClass? result = await sut.Containing(expected);

			await That(result?.StringValue).IsEqualTo(actual);
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
			PropertyResult.String<MyClass?> sut = MyClass.HasStringValue(actual);

			async Task Act()
				=> await sut.EqualTo(expected);

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that subject
				              has string value equal to "{expected}",
				              but it had string value "{actual}"
				              """);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public async Task EqualTo_ShouldSupportIgnoringCase(bool ignoringCase)
		{
			PropertyResult.String<MyClass?> sut = MyClass.HasStringValue("foo");

			async Task Act()
				=> await sut.EqualTo("FOO").IgnoringCase(ignoringCase);

			await That(Act).Throws<XunitException>()
				.OnlyIf(!ignoringCase)
				.WithMessage("""
				             Expected that subject
				             has string value equal to "FOO",
				             but it had string value "foo"
				             """);
		}

		[Fact]
		public async Task EqualTo_ShouldTriggerValidation()
		{
			Signaler<string?> signal = new();
			PropertyResult.String<string> sut = new(new Dummy(), _ => "x", "y", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.EqualTo("foo");

			await That(signal).Signaled().With(e => e == "foo");
		}

		[Fact]
		public async Task EqualTo_ShouldVerifyThatActualIsEqualToExpected()
		{
			PropertyResult.String<MyClass?> sut = MyClass.HasStringValue("foo");

			MyClass? result = await sut.EqualTo("foo");

			await That(result?.StringValue).IsEqualTo("foo");
		}

		[Fact]
		public async Task EqualTo_WhenSubjectIsNull_ShouldFail()
		{
			PropertyResult.String<MyClass?> sut = MyClass.HasStringValueOfNullSubject();

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
			PropertyResult.String<MyClass?> sut = MyClass.HasStringValue(actual);

			async Task Act()
				=> await sut.NotContaining(expected);

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that subject
				              has string value not containing "{expected}",
				              but it had string value "{actual}"
				              """);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public async Task NotContaining_ShouldSupportIgnoringCase(bool ignoringCase)
		{
			PropertyResult.String<MyClass?> sut = MyClass.HasStringValue("something with foo in it");

			async Task Act()
				=> await sut.NotContaining("FOO").IgnoringCase(ignoringCase);

			await That(Act).Throws<XunitException>()
				.OnlyIf(ignoringCase)
				.WithMessage("""
				             Expected that subject
				             has string value not containing "FOO" ignoring case,
				             but it had string value "something with foo in it"
				             """);
		}

		[Fact]
		public async Task NotContaining_ShouldTriggerValidation()
		{
			Signaler<string?> signal = new();
			PropertyResult.String<string> sut = new(new Dummy(), _ => "x", "y", (e, _) =>
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
			PropertyResult.String<MyClass?> sut = MyClass.HasStringValue(actual);

			MyClass? result = await sut.NotContaining(expected);

			await That(result?.StringValue).IsEqualTo(actual);
		}

		[Fact]
		public async Task NotEqualTo_ShouldFailWhenActualDoesNotEqualExpected()
		{
			PropertyResult.String<MyClass?> sut = MyClass.HasStringValue("foo");

			async Task Act()
				=> await sut.NotEqualTo("foo");

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has string value not equal to "foo",
				             but it had string value "foo"
				             """);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public async Task NotEqualTo_ShouldSupportIgnoringCase(bool ignoringCase)
		{
			PropertyResult.String<MyClass?> sut = MyClass.HasStringValue("foo");

			async Task Act()
				=> await sut.NotEqualTo("FOO").IgnoringCase(ignoringCase);

			await That(Act).Throws<XunitException>()
				.OnlyIf(ignoringCase)
				.WithMessage("""
				             Expected that subject
				             has string value not equal to "FOO" ignoring case,
				             but it had string value "foo"
				             """);
		}

		[Fact]
		public async Task NotEqualTo_ShouldTriggerValidation()
		{
			Signaler<string?> signal = new();
			PropertyResult.String<string> sut = new(new Dummy(), _ => "x", "y", (e, _) =>
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
			PropertyResult.String<MyClass?> sut = MyClass.HasStringValue(actual);

			MyClass? result = await sut.NotEqualTo(expected);

			await That(result?.StringValue).IsEqualTo(actual);
		}

		[Fact]
		public async Task NotEqualTo_WhenSubjectIsNullAndUnexpectedIsNull_ShouldFail()
		{
			PropertyResult.String<MyClass?> sut = MyClass.HasStringValueOfNullSubject();

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
			PropertyResult.String<MyClass?> sut = MyClass.HasStringValueOfNullSubject();

			async Task Act()
				=> await sut.NotEqualTo("foo");

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has string value not equal to "foo",
				             but it was <null>
				             """);
		}
	}
}
