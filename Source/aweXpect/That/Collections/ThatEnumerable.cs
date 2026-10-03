using System.Collections.Generic;
using aweXpect.SourceGenerators;

namespace aweXpect;

/// <summary>
///     Expectations on <see cref="IEnumerable{TItem}" />.
/// </summary>
[CollectionSubjects("System.Collections.Immutable.ImmutableArray<{item}>")]
public static partial class ThatEnumerable;
