namespace aweXpect.Core.Sources;

/// <summary>
///     What was <see langword="null" />, when a subject has no value to check.
/// </summary>
internal enum NullSubjectKind
{
	None,
	NullDelegate,
	NullTaskReturned,
	NullTaskSubject,
}
