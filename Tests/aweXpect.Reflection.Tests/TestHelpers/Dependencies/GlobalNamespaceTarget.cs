// ReSharper disable All

#pragma warning disable CS0169 // field is never used
#pragma warning disable CS8618 // non-nullable field must contain a non-null value
// This type intentionally lives in the global namespace to exercise global-namespace dependency assertions.
#pragma warning disable CA1050 // Declare types in namespaces

using System;

/// <summary>
///     A type in the global namespace, used to test dependency assertions against global-namespace types.
/// </summary>
public class GlobalNamespaceTarget
{
	private string _value;
}
