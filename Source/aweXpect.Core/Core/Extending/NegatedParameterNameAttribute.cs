using System;

namespace aweXpect.Core.Extending;

/// <summary>
///     Gives the marked parameter of a method marked with <see cref="GenerateExpectationAttribute" /> or
///     <see cref="GenerateCollectionExpectationAttribute" /> the <paramref name="name" /> in the negated expectation,
///     e.g. <c>unexpected</c> for a parameter <c>expected</c>.
/// </summary>
/// <remarks>
///     The name applies to the generated negated expectation and to the negated overload directly on the collection
///     (<see cref="GenerateCollectionExpectationAttribute.NegatedName" />): to its signature, its argument checks and
///     its derived documentation. A <c>&lt;paramref&gt;</c> to the parameter in the remarks of a collection expectation
///     follows the name in the negated overload as well. The placeholders of the expectation texts keep naming the
///     parameter of the marked method.
/// </remarks>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class NegatedParameterNameAttribute(string name) : Attribute
{
	/// <summary>
	///     The name of the parameter in the negated expectation.
	/// </summary>
	public string Name { get; } = name;
}
