#if NETSTANDARD2_0
namespace System.Runtime.CompilerServices;

/// <summary>
///     Polyfill for the compiler-recognized module initializer attribute, which netstandard2.0 does not provide.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class ModuleInitializerAttribute : Attribute;
#endif
