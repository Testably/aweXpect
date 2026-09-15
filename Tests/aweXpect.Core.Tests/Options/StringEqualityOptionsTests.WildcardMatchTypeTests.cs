using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class WildcardMatchTypeTests
	{
		[Fact]
		public async Task AsWildcard_ShouldReturnSameInstance()
		{
			StringEqualityOptions sut = new();

			StringEqualityOptions result = sut.AsWildcard();

			await That(result).IsSameAs(sut);
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task ShouldCompareCaseSensitive(bool ignoreCase)
		{
			string sut = "foo\nbar";

			async Task Act()
				=> await That(sut).IsEqualTo("FOO\nBAR").AsWildcard().IgnoringCase(ignoreCase);

			await That(Act).Throws<XunitException>().OnlyIf(!ignoreCase)
				.WithMessage("""
				             Expected that sut
				             matches "FOO\nBAR",
				             but it did not match:
				               ↓ (actual)
				               "foo\nbar"
				               "FOO\nBAR"
				               ↑ (wildcard pattern)

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
				=> await That(sut).IsEqualTo("bar").AsWildcard();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             matches "bar",
				             but it did not match:
				               ↓ (actual)
				               "foo"
				               "bar"
				               ↑ (wildcard pattern)

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
				=> await That(sut).IsEqualTo("\tsomething\r\nelse").AsWildcard();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             matches "\tsomething\r\nelse",
				             but it did not match:
				               ↓ (actual)
				               "foo\nbar"
				               "\tsomething\r\nelse"
				               ↑ (wildcard pattern)

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
					.AsWildcard();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that () => Task.FromException(exception)
				             throws an exception with Message matching "bar",
				             but it did not match:
				               ↓ (actual)
				               "foo"
				               "bar"
				               ↑ (wildcard pattern)

				             Message:
				             foo
				             """);
		}

		[Fact]
		public async Task WhenPatternIsNull_ShouldFail()
		{
			string sut = "foo";

			async Task Act()
				=> await That(sut).IsEqualTo(null).AsWildcard();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             matches <null>,
				             but could not compare the <null> wildcard pattern with "foo"

				             Actual:
				             foo
				             """);
		}

		[Fact]
		public async Task WhenSubjectAndPatternAreNull_ShouldFail()
		{
			string? sut = null;

			async Task Act()
				=> await That(sut).IsEqualTo(null).AsWildcard();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             matches <null>,
				             but it was <null>
				             """);
		}

		[Fact]
		public async Task WhenSubjectIsNull_ShouldFail()
		{
			string? sut = null;

			async Task Act()
				=> await That(sut).IsEqualTo("*").AsWildcard();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             matches "*",
				             but it was <null>
				             """);
		}
	}
}
