using System;

namespace aweXpect.Reflection.Collections;

/// <summary>
///     The access modifiers.<br />
///     <see href="https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/access-modifiers" />
/// </summary>
[Flags]
public enum AccessModifiers
{
	/// <summary>
	///     The <c>internal</c> access modifier.
	/// </summary>
	/// <remarks>
	///     <see href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/internal" />
	/// </remarks>
	Internal = 1,

	/// <summary>
	///     The <c>protected</c> access modifier.
	/// </summary>
	/// <remarks>
	///     <see href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/protected" />
	/// </remarks>
	Protected = 2,

	/// <summary>
	///     The <c>private</c> access modifier.
	/// </summary>
	/// <remarks>
	///     <see href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/private" />
	/// </remarks>
	Private = 4,

	/// <summary>
	///     The <c>public</c> access modifier.
	/// </summary>
	/// <remarks>
	///     <see href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/public" />
	/// </remarks>
	Public = 8,

	/// <summary>
	///     The <c>protected internal</c> access modifier.
	/// </summary>
	/// <remarks>
	///     <see href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/protected-internal" />
	/// </remarks>
	ProtectedInternal = 16,

	/// <summary>
	///     The <c>private protected</c> access modifier.
	/// </summary>
	/// <remarks>
	///     <see href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/private-protected" />
	/// </remarks>
	PrivateProtected = 32,

	/// <summary>
	///     Any access modifier.
	/// </summary>
	Any = Internal | Protected | Private | Public | ProtectedInternal | PrivateProtected,
}
