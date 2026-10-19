#if !NET9_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices;

/// <summary>
///     Specifies the priority of a member in overload resolution, which C# 13 and later respect.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property,
	Inherited = false)]
[ExcludeFromCodeCoverage]
internal sealed class OverloadResolutionPriorityAttribute(int priority) : Attribute
{
	/// <summary>
	///     The priority; a higher value wins over the applicable overloads with a lower one.
	/// </summary>
	public int Priority { get; } = priority;
}
#endif
