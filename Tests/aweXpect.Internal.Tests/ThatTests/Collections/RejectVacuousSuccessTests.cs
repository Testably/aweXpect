using System.Collections;
using System.Collections.Generic;
using aweXpect.Helpers;

namespace aweXpect.Internal.Tests.ThatTests.Collections;

public class RejectVacuousSuccessTests
{
	[Fact]
	public async Task ComplyWith_WhenEmptyStringCollectionRejectsVacuousSuccess_ShouldFail()
	{
		IEnumerable<string?> subject = new NoStrings();

		async Task Act()
			=> await That(subject).All().ComplyWith(it => it.IsEqualTo("foo"));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is equal to "foo" for all items,
			             but none of 0 were

			             Collection:
			             []
			             """)
			.Because("a collection that rejects a vacuous success must fail `All()` while it is empty");
	}

	/// <remarks>
	///     Mirrors the projected sequence of recursive inner exceptions, which is the only production collection
	///     marked as <see cref="IRejectVacuousSuccess" /> and never carries strings.
	/// </remarks>
	private sealed class NoStrings : IEnumerable<string?>, IRejectVacuousSuccess
	{
		public IEnumerator<string?> GetEnumerator()
		{
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
