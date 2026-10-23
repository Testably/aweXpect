using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class AssemblyFilters
{
	/// <summary>
	///     Filter for assemblies with a <see cref="AssemblyName.Version" /> that satisfies the
	///     <paramref name="predicate" />.
	/// </summary>
	public static Filtered.Assemblies WithVersion(this Filtered.Assemblies @this,
		Func<Version, bool> predicate,
		[CallerArgumentExpression(nameof(predicate))]
		string doNotPopulateThisValue = "")
	{
		IChangeableFilter<Assembly> filter = Filter.Suffix<Assembly>(
			assembly => assembly.GetName().Version is { } version && UserCode.Invoke(predicate, version, "the predicate"),
			$" with version matching {doNotPopulateThisValue.TrimExpression()}");
		return @this.Which(filter);
	}

	/// <summary>
	///     Filter for assemblies by individual components of their <see cref="AssemblyName.Version" />.
	/// </summary>
	public static AssembliesWithVersion WithVersion(this Filtered.Assemblies @this)
		=> new(@this, Filter.Suffix<Assembly>(_ => true, ""));

	/// <summary>
	///     Filter for assemblies by individual components of their <see cref="AssemblyName.Version" />.
	/// </summary>
	public class AssembliesWithVersion : Filtered.Assemblies
	{
		private readonly Filter.GenericFilter<Assembly> _filter;
		private readonly Filtered.Assemblies _source;

		internal AssembliesWithVersion(Filtered.Assemblies source, Filter.GenericFilter<Assembly> filter)
			: base(source.Which(filter))
		{
			_source = source;
			_filter = filter;
		}

		/// <summary>
		///     Compares the <see cref="Version.Major" /> version component.
		/// </summary>
		public VersionComponent WithMajor => new(this, "major", version => version.Major);

		/// <summary>
		///     Compares the <see cref="Version.Minor" /> version component.
		/// </summary>
		public VersionComponent WithMinor => new(this, "minor", version => version.Minor);

		/// <summary>
		///     Compares the <see cref="Version.Build" /> version component.
		/// </summary>
		/// <remarks>The build component is <c>-1</c> when it is absent from the version.</remarks>
		public VersionComponent WithBuild => new(this, "build", version => version.Build);

		/// <summary>
		///     Compares the <see cref="Version.Revision" /> version component.
		/// </summary>
		/// <remarks>The revision component is <c>-1</c> when it is absent from the version.</remarks>
		public VersionComponent WithRevision => new(this, "revision", version => version.Revision);

		internal AssembliesWithVersion Extend(Func<bool, Assembly, bool> predicate, Func<string, string> description)
			=> new(_source, _filter.Extend(predicate, description));
	}

	/// <summary>
	///     Comparison of a single component of the <see cref="AssemblyName.Version" />.
	/// </summary>
	public class VersionComponent
	{
		private readonly string _component;
		private readonly AssembliesWithVersion _owner;
		private readonly Func<Version, int> _selector;

		internal VersionComponent(
			AssembliesWithVersion owner,
			string component,
			Func<Version, int> selector)
		{
			_owner = owner;
			_component = component;
			_selector = selector;
		}

		/// <summary>
		///     The component is greater than the <paramref name="expected" /> value.
		/// </summary>
		public AssembliesWithVersion GreaterThan(int expected)
			=> Apply(value => value > expected, $"greater than {expected}");

		/// <summary>
		///     The component is greater than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public AssembliesWithVersion GreaterThanOrEqualTo(int expected)
			=> Apply(value => value >= expected, $"greater than or equal to {expected}");

		/// <summary>
		///     The component is less than the <paramref name="expected" /> value.
		/// </summary>
		public AssembliesWithVersion LessThan(int expected)
			=> Apply(value => value < expected, $"less than {expected}");

		/// <summary>
		///     The component is less than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public AssembliesWithVersion LessThanOrEqualTo(int expected)
			=> Apply(value => value <= expected, $"less than or equal to {expected}");

		/// <summary>
		///     The component is equal to the <paramref name="expected" /> value.
		/// </summary>
		public AssembliesWithVersion EqualTo(int expected)
			=> Apply(value => value == expected, $"equal to {expected}");

		/// <summary>
		///     The component is not equal to the <paramref name="expected" /> value.
		/// </summary>
		public AssembliesWithVersion NotEqualTo(int expected)
			=> Apply(value => value != expected, $"not equal to {expected}");

		private AssembliesWithVersion Apply(Func<int, bool> comparison, string comparisonText)
			=> _owner.Extend(
				(result, assembly)
					=> result && assembly.GetName().Version is { } version && comparison(_selector(version)),
				description => $"{description} with {_component} version {comparisonText}");
	}
}
