using System.Linq;
using aweXpect.Delegates;

namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
	public sealed class OnlyIfTests
	{
		[Fact]
		public async Task WhenContinuingWithAndOrOr_ShouldNotBeOffered()
		{
			Type[] continuations = typeof(ThatDelegateThrows<Exception>).GetMethods()
				.SelectMany(method => new[]
				{
					method.ReturnType.GetProperty("And"), method.ReturnType.GetProperty("Or"),
				})
				.Where(property => property is not null)
				.Select(property => property!.PropertyType)
				.ToArray();

			await That(continuations).IsNotEmpty().And
				.All().Satisfy(type => type.GetMember(nameof(ThatDelegateThrows<Exception>.OnlyIf)).Length == 0)
				.Because("OnlyIf switches the whole Throws expectation and must not read like a further condition");
		}
	}
}
