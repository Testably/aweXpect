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
		IChangeableFilter<Assembly> filter = Filter.Suffix<Assembly>(
			assembly => assembly.HasAttribute<TAttribute>(inherit: inherit),
			$" with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))}");
		return new AssembliesWith(@this.Which(filter), filter);
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
		IChangeableFilter<Assembly> filter = Filter.Suffix<Assembly>(
			assembly => assembly.HasAttribute(predicate, inherit),
			$" with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimCommonWhiteSpace()}");
		return new AssembliesWith(@this.Which(filter), filter);
	}

	/// <summary>
	///     Additional filters on assemblies with an attribute.
	/// </summary>
	public class AssembliesWith(Filtered.Assemblies inner, IChangeableFilter<Assembly> filter)
		: Filtered.Assemblies(inner)
	{
		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" />.
		/// </summary>
		/// <remarks>
		///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
		///     the attribute can be inherited from a base type.
		/// </remarks>
		public AssembliesWith OrWith<TAttribute>(bool inherit = true)
			where TAttribute : Attribute
		{
			filter.UpdateFilter((result, assembly) => result || assembly.HasAttribute<TAttribute>(inherit: inherit),
				description
					=> $"{description} or with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))}");
			return this;
		}

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
		{
			filter.UpdateFilter(
				(result, assembly) => result || assembly.HasAttribute(predicate, inherit),
				description
					=> $"{description} or with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimCommonWhiteSpace()}");
			return this;
		}
	}
}
