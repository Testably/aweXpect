using System.Reflection;
using System.Runtime.CompilerServices;

namespace aweXpect.Reflection.Tests.TestHelpers;

public static class MethodInfoExtensions
{
	/// <summary>
	///     Checks if the <paramref name="methodInfo" /> is asynchronous (declared with the <see langword="async" /> keyword).
	/// </summary>
	/// <param name="methodInfo">The <see cref="MethodInfo" /> to check.</param>
	public static bool IsReallyAsync(this MethodInfo? methodInfo)
		=> methodInfo?.GetCustomAttribute<AsyncStateMachineAttribute>() is not null;

	/// <summary>
	///     Checks if the <paramref name="methodInfo" /> is an extension method (whose first parameter is declared with the
	///     <see langword="this" /> modifier).
	/// </summary>
	/// <param name="methodInfo">The <see cref="MethodInfo" /> to check.</param>
	public static bool IsReallyExtensionMethod(this MethodInfo? methodInfo)
		=> methodInfo?.IsDefined(typeof(ExtensionAttribute), false) == true;
}
