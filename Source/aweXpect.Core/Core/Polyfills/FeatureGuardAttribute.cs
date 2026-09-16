#if NET8_0_OR_GREATER && !NET9_0_OR_GREATER
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
	/// <summary>
	///     Indicates that the specified public static boolean get-only property guards access to the specified
	///     feature.
	/// </summary>
	/// <remarks>
	///     A trimmer that knows the attribute treats the property as <see langword="false" /> when the guarded
	///     feature is unavailable, so the code behind the guard is removed.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	[ExcludeFromCodeCoverage]
	internal sealed class FeatureGuardAttribute : Attribute
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="FeatureGuardAttribute" /> class with the specified feature
		///     type.
		/// </summary>
		public FeatureGuardAttribute(Type featureType)
		{
			FeatureType = featureType;
		}

		/// <summary>
		///     The type that represents the feature guarded by the property.
		/// </summary>
		public Type FeatureType { get; }
	}

	/// <summary>
	///     Indicates that the specified public static boolean get-only property corresponds to a feature switch
	///     that can be controlled at runtime.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = false)]
	[ExcludeFromCodeCoverage]
	internal sealed class FeatureSwitchDefinitionAttribute : Attribute
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="FeatureSwitchDefinitionAttribute" /> class with the
		///     specified switch name.
		/// </summary>
		public FeatureSwitchDefinitionAttribute(string switchName)
		{
			SwitchName = switchName;
		}

		/// <summary>
		///     The name of the feature switch.
		/// </summary>
		public string SwitchName { get; }
	}
}

#endif
