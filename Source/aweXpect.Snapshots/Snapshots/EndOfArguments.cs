using System.ComponentModel;

namespace aweXpect.Snapshots;

/// <summary>
///     Ends the arguments of <c>.NamedAfter(…)</c>, so that an additional argument, e.g. a second <see cref="string" />,
///     selects the overload with more arguments instead of being taken as the expression of the first one.
/// </summary>
/// <remarks>
///     It is never passed.
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Never)]
public readonly struct EndOfArguments;
