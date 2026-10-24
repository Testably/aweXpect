using System;

namespace aweXpect.Core.Helpers;

/// <summary>
///     Carries a <see cref="MatchResult.NotComparable(string, Exception)" /> answer of a matcher inside a
///     <see cref="UserCodeException" />, so that every expectation and every collection treats it like code of the caller
///     that answered nothing, and fails in both polarities.
/// </summary>
#pragma warning disable S3871 // Only the evaluation catches it, wrapped in the UserCodeException
internal sealed class NotComparableException(string reason, Exception? cause)
	: Exception(reason, cause);
#pragma warning restore S3871
