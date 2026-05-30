using System;
using System.Threading.Tasks;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

/// <summary>
///     A <see cref="IFilter{Type}" /> that counts the members of a single <see cref="Type" /> matching an inner
///     member-filter, so that the same counting and description logic can be shared between the
///     <see cref="Filtered.Types" /> filters and the corresponding assertions.
/// </summary>
internal interface IContainedMembersFilter : IFilter<Type>
{
	/// <summary>
	///     The description of the matched members (e.g. <c>methods with FactAttribute </c>).
	/// </summary>
	string MembersDescription { get; }

	/// <summary>
	///     Counts the members of the <paramref name="value" /> that match the inner member-filter.
	/// </summary>
#if NET8_0_OR_GREATER
	ValueTask<int> CountMatchingMembers(Type value);
#else
	Task<int> CountMatchingMembers(Type value);
#endif
}
