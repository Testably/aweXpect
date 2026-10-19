#if NETFRAMEWORK
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
	/// <summary>
	///     Polyfill required so that <c>init</c>-only setters can be compiled for .NET Framework targets.
	/// </summary>
	internal static class IsExternalInit;
}
#endif
