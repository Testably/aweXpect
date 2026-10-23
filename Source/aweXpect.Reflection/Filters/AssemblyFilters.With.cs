using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class AssemblyFilters
{
	private const string DirectText = "direct ";

	/// <summary>
	///     Filter for assemblies with attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static AssembliesWith With<TAttribute>(this Filtered.Assemblies @this, bool inherit = true)
		where TAttribute : Attribute
	{
		Filter.GenericFilter<Assembly> filter = Filter.Suffix<Assembly>(
			assembly => assembly.HasAttribute<TAttribute>(inherit: inherit),
			() => $" with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))}");
		return new AssembliesWith(@this, filter);
	}

	/// <summary>
	///     Filter for assemblies with attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static AssembliesWith With<TAttribute>(this Filtered.Assemblies @this,
		Func<TAttribute, bool>? predicate,
		bool inherit = true,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		Filter.GenericFilter<Assembly> filter = Filter.Suffix<Assembly>(
			assembly => assembly.HasAttribute(predicate, inherit),
			() => $" with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()}");
		return new AssembliesWith(@this, filter);
	}

	/// <summary>
	///     Additional filters on assemblies with an attribute.
	/// </summary>
	public class AssembliesWith : Filtered.Assemblies
	{
		private readonly Filter.GenericFilter<Assembly> _filter;
		private readonly Filtered.Assemblies _source;

		internal AssembliesWith(Filtered.Assemblies source, Filter.GenericFilter<Assembly> filter)
			: base(source.Which(filter))
		{
			_source = source;
			_filter = filter;
		}

		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" />.
		/// </summary>
		/// <remarks>
		///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
		///     the attribute can be inherited from a base type.
		/// </remarks>
		public AssembliesWith OrWith<TAttribute>(bool inherit = true)
			where TAttribute : Attribute
			=> new(_source, _filter.Extend((result, assembly) => result || assembly.HasAttribute<TAttribute>(inherit: inherit),
				description
					=> $"{description} or with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))}"));

		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" /> that
		///     matches the <paramref name="predicate" />.
		/// </summary>
		/// <remarks>
		///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
		///     the attribute can be inherited from a base type.
		/// </remarks>
		public AssembliesWith OrWith<TAttribute>(
			Func<TAttribute, bool>? predicate,
			bool inherit = true,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
			where TAttribute : Attribute
			=> new(_source, _filter.Extend(
				(result, assembly) => result || assembly.HasAttribute(predicate, inherit),
				description
					=> $"{description} or with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()}"));
	}
}
