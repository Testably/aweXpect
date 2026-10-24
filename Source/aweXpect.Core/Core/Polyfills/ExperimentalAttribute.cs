#if !NET8_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis;

/// <summary>
///     Indicates that an API is experimental and it may change in the future.
/// </summary>
/// <remarks>
///     The compiler reports a use of the API with the <see cref="DiagnosticId" />.
/// </remarks>
[AttributeUsage(
	AttributeTargets.Assembly |
	AttributeTargets.Module |
	AttributeTargets.Class |
	AttributeTargets.Struct |
	AttributeTargets.Enum |
	AttributeTargets.Constructor |
	AttributeTargets.Method |
	AttributeTargets.Property |
	AttributeTargets.Field |
	AttributeTargets.Event |
	AttributeTargets.Interface |
	AttributeTargets.Delegate,
	Inherited = false)]
[ExcludeFromCodeCoverage]
internal sealed class ExperimentalAttribute : Attribute
{
	/// <summary>
	///     Initializes a new instance of the <see cref="ExperimentalAttribute" /> class with the specified diagnostic ID.
	/// </summary>
	/// <param name="diagnosticId">The ID that the compiler reports for a use of the experimental API.</param>
	public ExperimentalAttribute(string diagnosticId)
	{
		DiagnosticId = diagnosticId;
	}

	/// <summary>
	///     The ID that the compiler reports for a use of the experimental API.
	/// </summary>
	public string DiagnosticId { get; }
}
#endif
