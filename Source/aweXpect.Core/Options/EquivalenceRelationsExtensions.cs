namespace aweXpect.Options;

internal static class EquivalenceRelationsExtensions
{
	/// <summary>
	///     Checks if the <paramref name="equivalenceRelations" /> include the <paramref name="relation" />.
	/// </summary>
	/// <remarks>
	///     Unlike <see cref="System.Enum.HasFlag(System.Enum)" />, this does not box on .NET Framework, where the
	///     matchers check the relations for every item.
	/// </remarks>
	public static bool Includes(this CollectionMatchOptions.EquivalenceRelations equivalenceRelations,
		CollectionMatchOptions.EquivalenceRelations relation)
		=> (equivalenceRelations & relation) == relation;
}
