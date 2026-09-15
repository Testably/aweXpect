using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class RegexMatchTypeTests
	{
		[Fact]
		public async Task AsRegex_ShouldReturnSameInstance()
		{
			StringEqualityOptions sut = new();

			StringEqualityOptions result = sut.AsRegex();

			await That(result).IsSameAs(sut);
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task ShouldCompareCaseSensitive(bool ignoreCase)
		{
			string sut = "foo\nbar";

			async Task Act()
				=> await That(sut).IsEqualTo("FOO\nBAR").AsRegex().IgnoringCase(ignoreCase);

			await That(Act).Throws<XunitException>().OnlyIf(!ignoreCase)
				.WithMessage("""
				             Expected that sut
				             matches regex "FOO\nBAR",
				             but it did not match:
				               ↓ (actual)
				               "foo\nbar"
				               "FOO\nBAR"
				               ↑ (regex pattern)

				             Actual:
				             foo
				             bar
				             
				             Expected:
				             FOO
				             BAR
				             """).IgnoringNewlineStyle();
		}

		[Fact]
		public async Task ShouldDisplayActualAndPatternUnderneathEachOther()
		{
			string sut = "foo";

			async Task Act()
				=> await That(sut).IsEqualTo("bar").AsRegex();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             matches regex "bar",
				             but it did not match:
				               ↓ (actual)
				               "foo"
				               "bar"
				               ↑ (regex pattern)

				             Actual:
				             foo
				             
				             Expected:
				             bar
				             """);
		}

		[Fact]
		public async Task ShouldReplaceNewlines()
		{
			string sut = "foo\nbar";

			async Task Act()
				=> await That(sut).IsEqualTo("\tsomething\r\nelse").AsRegex();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             matches regex "\tsomething\r\nelse",
				             but it did not match:
				               ↓ (actual)
				               "foo\nbar"
				               "\tsomething\r\nelse"
				               ↑ (regex pattern)

				             Actual:
				             foo
				             bar
				             
				             Expected:
				             	something
				             else
				             """).IgnoringNewlineStyle();
		}

		[Fact]
		public async Task ShouldSupportPassiveGrammaticalVoice()
		{
			Exception exception = new("foo");

			async Task Act()
				=> await That(() => Task.FromException(exception)).Throws().WithMessage("bar")
					.AsRegex();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that () => Task.FromException(exception)
				             throws an exception with Message matching regex "bar",
				             but it did not match:
				               ↓ (actual)
				               "foo"
				               "bar"
				               ↑ (regex pattern)

				             Message:
				             foo
				             """);
		}

		[Fact]
		public async Task WhenIgnoringCase_ShouldCompareCaseInsensitive()
		{
			string sut = "foo";

			async Task Act()
				=> await That(sut).IsEqualTo("FOO").AsRegex().IgnoringCase();

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenPatternIsNull_ShouldFail()
		{
			string sut = "foo";

			async Task Act()
				=> await That(sut).IsEqualTo(null).AsRegex();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             matches regex <null>,
				             but could not compare the <null> regex with "foo"

				             Actual:
				             foo
				             """);
		}

		[Fact]
		public async Task WhenSubjectAndPatternAreNull_ShouldFail()
		{
			string? sut = null;

			async Task Act()
				=> await That(sut).IsEqualTo(null).AsRegex();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             matches regex <null>,
				             but it was <null>
				             """);
		}

		[Fact]
		public async Task WhenSubjectIsNull_ShouldFail()
		{
			string? sut = null;

			async Task Act()
				=> await That(sut).IsEqualTo(".*").AsRegex();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             matches regex ".*",
				             but it was <null>
				             """);
		}
	}
}
