using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class ExactMatchTypeTests
	{
		[Fact]
		public async Task Exactly_ShouldReturnSameInstance()
		{
			StringEqualityOptions sut = new();

			StringEqualityOptions result = sut.Exactly();

			await That(result).IsSameAs(sut);
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task ShouldCompareCaseSensitive(bool ignoreCase)
		{
			string sut = "foo\nbar";

			async Task Act()
				=> await That(sut).IsEqualTo("FOO\nBAR").IgnoringCase(ignoreCase);

			await That(Act).Throws<XunitException>().OnlyIf(!ignoreCase)
				.WithMessage("""
				             Expected that sut
				             is equal to "FOO\nBAR",
				             but it was "foo\nbar" which differs on line 1 and column 1:
				                ↓ (actual)
				               "foo\nbar"
				               "FOO\nBAR"
				                ↑ (expected)

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
				=> await That(sut).IsEqualTo("bar");

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             is equal to "bar",
				             but it was "foo" which differs at index 0:
				                ↓ (actual)
				               "foo"
				               "bar"
				                ↑ (expected)

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
				=> await That(sut).IsEqualTo("\tsomething\r\nelse");

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             is equal to "\tsomething\r\nelse",
				             but it was "foo\nbar" which differs on line 1 and column 1:
				                ↓ (actual)
				               "foo\nbar"
				               "\tsomething\r\nelse"
				                ↑ (expected)

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
				=> await That(() => Task.FromException(exception)).Throws().WithMessage("bar");

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that () => Task.FromException(exception)
				             throws an exception with Message equal to "bar",
				             but it was "foo" which differs at index 0:
				                ↓ (actual)
				               "foo"
				               "bar"
				                ↑ (expected)

				             Message:
				             foo
				             """);
		}

		[Fact]
		public async Task WhenExpectedIsNull_ShouldFail()
		{
			string sut = "foo";

			async Task Act()
				=> await That(sut).IsEqualTo(null);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             is equal to <null>,
				             but it was "foo"

				             Actual:
				             foo
				             """);
		}

		[Fact]
		public async Task WhenIgnoringCase_ShouldCompareCaseInsensitive()
		{
			string sut = "foo";

			async Task Act()
				=> await That(sut).IsEqualTo("FOO").IgnoringCase();

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenSubjectAndExpectedAreNull_ShouldSucceed()
		{
			string? sut = null;

			async Task Act()
				=> await That(sut).IsEqualTo(null);

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenSubjectIsNull_ShouldFail()
		{
			string? sut = null;

			async Task Act()
				=> await That(sut).IsEqualTo("");

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             is equal to "",
				             but it was <null>
				             """);
		}
	}
}
