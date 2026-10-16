using System;

namespace aweXpect.Core.Extending;

/// <summary>
///     Marks the <see cref="System.Threading.CancellationToken" /> parameter of a method marked with
///     <see cref="GenerateExpectationAttribute" /> or <see cref="GenerateCollectionExpectationAttribute" /> that
///     receives the token of the evaluation.
/// </summary>
/// <remarks>
///     The marked parameter can be at any position after the subject and is no parameter of the generated expectation.
///     A <see cref="System.Threading.CancellationToken" /> parameter that is not marked is a parameter of the
///     expectation like any other.
/// </remarks>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class EvaluationCancellationTokenAttribute : Attribute;
