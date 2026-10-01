using aweXpect.Core.Helpers;

namespace aweXpect.Customization;

public partial class AwexpectCustomization
{
	private ReflectionCustomization? _reflection;

	/// <summary>
	///     Customize the reflection settings.
	/// </summary>
	public ReflectionCustomization Reflection()
	{
		_reflection ??= new ReflectionCustomization(this);
		return _reflection;
	}

	/// <summary>
	///     Customize the reflection settings.
	/// </summary>
	public class ReflectionCustomization
	{
		private const string KeyPrefix = "aweXpect.Reflection.";

		internal ReflectionCustomization(IAwexpectCustomization awexpectCustomization)
		{
			ExcludedAssemblyPrefixes = new CopiedOnGet(new CustomizationValue<string[]>(awexpectCustomization,
				KeyPrefix + nameof(ExcludedAssemblyPrefixes),
				[
					"mscorlib",
					"System",
					"Microsoft",
					"netstandard",
					"WindowsBase",
					"JetBrains",
					"xunit",
					"Castle",
					"DynamicProxyGenAssembly2",
				],
				prefixes => prefixes.ThrowIfNull()));
		}

		/// <summary>
		///     The assembly namespace prefixes that are excluded during reflection.
		/// </summary>
		/// <remarks>
		///     Defaults to<br />
		///     - mscorlib<br />
		///     - System<br />
		///     - Microsoft<br />
		///     - netstandard<br />
		///     - WindowsBase<br />
		///     - JetBrains<br />
		///     - xunit<br />
		///     - Castle<br />
		///     - DynamicProxyGenAssembly2
		/// </remarks>
		public ICustomizationValueSetter<string[]> ExcludedAssemblyPrefixes { get; }

		/// <summary>
		///     Returns a copy of the stored array, so that changing it cannot bypass the scoping of the setting.
		/// </summary>
		private sealed class CopiedOnGet(ICustomizationValueSetter<string[]> inner) : ICustomizationValueSetter<string[]>
		{
			/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Get()" />
			public string[] Get() => (string[])inner.Get().Clone();

			/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Set(TValue)" />
			public CustomizationLifetime Set(string[] value) => inner.Set(value);
		}
	}
}
