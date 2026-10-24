using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

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
		public VersionComponentResult WithMajor => new(this, "major", version => version.Major);

		/// <summary>
		///     Compares the <see cref="Version.Minor" /> version component.
		/// </summary>
		public VersionComponentResult WithMinor => new(this, "minor", version => version.Minor);

		/// <summary>
		///     Compares the <see cref="Version.Build" /> version component.
		/// </summary>
		/// <remarks>The build component is <c>-1</c> when it is absent from the version.</remarks>
		public VersionComponentResult WithBuild => new(this, "build", version => version.Build);

		/// <summary>
		///     Compares the <see cref="Version.Revision" /> version component.
		/// </summary>
		/// <remarks>The revision component is <c>-1</c> when it is absent from the version.</remarks>
		public VersionComponentResult WithRevision => new(this, "revision", version => version.Revision);

		internal AssembliesWithVersion Extend(Func<bool, Assembly, bool> predicate, Func<string, string> description)
			=> new(_source, _filter.Extend(predicate, description));
	}

	/// <summary>
	///     Comparison of a single component of the <see cref="AssemblyName.Version" />.
	/// </summary>
	/// <remarks>
	///     The comparison operators, e.g. <c>EqualTo(…)</c> or <c>Between(…).And(…)</c>, are extension methods in
	///     <see cref="IntPropertyResultExtensions" />.
	/// </remarks>
	public sealed class VersionComponentResult : IIntPropertyResult<AssembliesWithVersion>
	{
		private readonly string _component;
		private readonly AssembliesWithVersion _owner;
		private readonly Func<Version, int> _selector;

		internal VersionComponentResult(
			AssembliesWithVersion owner,
			string component,
			Func<Version, int> selector)
		{
			_owner = owner;
			_component = component;
			_selector = selector;
		}

		/// <inheritdoc cref="IIntPropertyResult{TResult}.Validation" />
		Action<int?, string>? IIntPropertyResult<AssembliesWithVersion>.Validation => null;

		/// <inheritdoc cref="IIntPropertyResult{TResult}.Compare(PropertyComparison{int})" />
		AssembliesWithVersion IIntPropertyResult<AssembliesWithVersion>.Compare(PropertyComparison<int> comparison)
			=> _owner.Extend(
				(result, assembly) => result && assembly.GetName().Version is { } version &&
				                      comparison.Evaluate(_selector(version)) == Outcome.Success,
				description =>
				{
					StringBuilder stringBuilder = new(description);
					stringBuilder.Append(" with ").Append(_component).Append(" version ");
					if (comparison.IsNegated)
					{
						stringBuilder.Append("not ");
					}

					comparison.AppendExpectation(stringBuilder);
					return stringBuilder.ToString();
				});
	}
}
