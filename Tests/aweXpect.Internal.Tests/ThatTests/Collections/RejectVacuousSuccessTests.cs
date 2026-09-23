using System.Collections;
using System.Collections.Generic;
using aweXpect.Helpers;

namespace aweXpect.Internal.Tests.ThatTests.Collections;

public class RejectVacuousSuccessTests
{
	[Fact]
	public async Task AreEqualTo_WhenEmptyEnumerableRejectsVacuousSuccess_ShouldFail()
	{
		IEnumerable subject = new NoItems();

		async Task Act()
			=> await That(subject).All().AreEqualTo(1);

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is equal to 1 for all items,
			             but none of 0 were

			             Collection:
			             []
			             """)
			.Because("a non-generic collection that rejects a vacuous success must fail `All()` while it is empty");
	}

	[Fact]
	public async Task ComplyWith_WhenEmptyEnumerableRejectsVacuousSuccess_ShouldFail()
	{
		IEnumerable subject = new NoItems();

		async Task Act()
			=> await That(subject).All().ComplyWith(it => it.IsEqualTo(1));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is equal to 1 for all items,
			             but none of 0 were

			             Collection:
			             []
			             """)
			.Because("a non-generic collection that rejects a vacuous success must fail `All()` while it is empty");
	}

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

	[Fact]
	public async Task Satisfy_WhenEmptyEnumerableRejectsVacuousSuccess_ShouldFail()
	{
		IEnumerable subject = new NoItems();

		async Task Act()
			=> await That(subject).All().Satisfy(_ => true);

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             satisfies _ => true for all items,
			             but none of 0 did

			             Collection:
			             []
			             """)
			.Because("a non-generic collection that rejects a vacuous success must fail `All()` while it is empty");
	}

	private sealed class NoItems : IEnumerable, IRejectVacuousSuccess
	{
		public IEnumerator GetEnumerator()
		{
			yield break;
		}
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
