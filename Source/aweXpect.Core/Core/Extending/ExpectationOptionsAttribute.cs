using System;

namespace aweXpect.Core.Extending;

/// <summary>
///     Marks the options parameter of a method marked with <see cref="GenerateCollectionExpectationAttribute" />.
/// </summary>
/// <remarks>
///     The marked parameter can be at any position after the subject and is no parameter of the generated expectation:
///     the expectation creates the options with their default, exposes them on the returned result and passes the
///     options of the evaluation to the method. Currently supported is <see cref="aweXpect.Options.StringEqualityOptions" />.
/// </remarks>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class ExpectationOptionsAttribute : Attribute;
