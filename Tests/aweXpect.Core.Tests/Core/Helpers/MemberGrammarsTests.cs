using System.Collections;
using System.Collections.Generic;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Tests.Core.Helpers;

public sealed class MemberGrammarsTests
{
	[Test]
	public async Task ForMember_ForAGenericDictionaryInterface_ShouldBeSingular()
	{
		ExpectationGrammars result = ExpectationGrammars.Plural.ForMember<IDictionary<int, int>>();

		await That(result).IsEqualTo(ExpectationGrammars.None)
			.Because("a dictionary reads as a single lookup, even through an interface without the non-generic one");
	}

	[Test]
	public async Task ForMember_ForAList_ShouldBePlural()
	{
		ExpectationGrammars result = ExpectationGrammars.None.ForMember<List<int>>();

		await That(result).IsEqualTo(ExpectationGrammars.Plural);
	}

#if NET8_0_OR_GREATER
	[Test]
	public async Task ForMember_ForAnAsyncEnumerable_ShouldBePlural()
	{
		ExpectationGrammars result = ExpectationGrammars.None.ForMember<IAsyncEnumerable<int>>();

		await That(result).IsEqualTo(ExpectationGrammars.Plural);
	}
#endif

	[Test]
	public async Task ForMember_ForANonGenericDictionary_ShouldBeSingular()
	{
		ExpectationGrammars result = ExpectationGrammars.Plural.ForMember<Hashtable>();

		await That(result).IsEqualTo(ExpectationGrammars.None);
	}

	[Test]
	public async Task ForMember_ForAReadOnlyDictionaryInterface_ShouldBeSingular()
	{
		ExpectationGrammars result = ExpectationGrammars.Plural.ForMember<IReadOnlyDictionary<int, int>>();

		await That(result).IsEqualTo(ExpectationGrammars.None);
	}

	[Test]
	public async Task ForMember_ForAString_ShouldBeSingular()
	{
		ExpectationGrammars result = ExpectationGrammars.Plural.ForMember<string>();

		await That(result).IsEqualTo(ExpectationGrammars.None)
			.Because("a string is a single value, although it enumerates its characters");
	}
}
