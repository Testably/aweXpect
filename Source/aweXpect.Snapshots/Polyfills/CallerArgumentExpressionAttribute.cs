#if !NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices;

/// <summary>
///     Indicates that a parameter captures the expression passed for another parameter as a string.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
[ExcludeFromCodeCoverage]
internal sealed class CallerArgumentExpressionAttribute(string parameterName) : Attribute
{
	/// <summary>
	///     The name of the parameter whose expression is captured.
	/// </summary>
	public string ParameterName { get; } = parameterName;
}
#endif
