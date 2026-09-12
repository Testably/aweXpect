using System;

namespace aweXpect.Core;

/// <summary>
///     Marks an expectation that a <see langword="null" /> subject can never satisfy.
/// </summary>
/// <remarks>
///     The <c>aweXpect.Analyzers.IsNotNullSuppressor</c> uses this marker to suppress the nullability warnings for a
///     subject that a preceding expectation verified to be not <see langword="null" />.
///     <para />
///     Only apply it to an expectation that fails for a <see langword="null" /> subject regardless of its other
///     arguments. Expectations that a <see langword="null" /> subject can fulfil — e.g. <c>IsNotEmpty</c> on a
///     <see cref="string" /> or <c>IsEqualTo</c> with a <see langword="null" /> expected value — must not be marked.
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public sealed class GuaranteesNotNullAttribute : Attribute;
