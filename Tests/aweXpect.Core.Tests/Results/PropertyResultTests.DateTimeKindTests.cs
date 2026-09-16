using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed partial class PropertyResultTests
{
	public sealed class DateTimeKindTests
	{
		public sealed class GrammarTests
		{
			[Fact]
			public async Task WhenActive_ShouldUseTheActiveVoice()
			{
				PropertyResult.DateTimeKind<MyClass?, MyClass?, IThat<MyClass?>> sut =
					MyClass.HasDateTimeKindValue(DateTimeKind.Utc, ExpectationGrammars.Active);

				async Task Act()
					=> await sut.EqualTo(DateTimeKind.Local);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             with kind value equal to Local,
					             but it had kind value Utc
					             """);
			}
		}
	}
}
