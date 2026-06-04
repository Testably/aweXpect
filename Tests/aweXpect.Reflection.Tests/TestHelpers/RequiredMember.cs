#if NETFRAMEWORK
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
	/// <summary>
	///     Polyfill required so that <c>required</c> members can be compiled for .NET Framework targets.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property,
		AllowMultiple = false, Inherited = false)]
	internal sealed class RequiredMemberAttribute : Attribute;

	/// <summary>
	///     Polyfill required so that <c>required</c> members can be compiled for .NET Framework targets.
	/// </summary>
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
	internal sealed class CompilerFeatureRequiredAttribute(string featureName) : Attribute
	{
		// ReSharper disable once UnusedAutoPropertyAccessor.Local
		public string FeatureName { get; } = featureName;
	}
}
#endif
