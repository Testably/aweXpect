using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class AssemblyFilters
{
	/// <summary>
	///     Filter for assemblies with attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	public static AssembliesWith With<TAttribute>(this Filtered.Assemblies @this)
		where TAttribute : Attribute
	{
		Filter.GenericFilter<Assembly> filter = Filter.Suffix<Assembly>(
			assembly => assembly.HasAttribute<TAttribute>(),
			() => $" with {Formatter.Format(typeof(TAttribute))}");
		return new AssembliesWith(@this, filter);
	}

	/// <summary>
	///     Filter for assemblies with attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	public static AssembliesWith With<TAttribute>(this Filtered.Assemblies @this,
		Func<TAttribute, bool>? predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		Filter.GenericFilter<Assembly> filter = Filter.Suffix<Assembly>(
			assembly => assembly.HasAttribute(predicate),
			() => $" with {Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()}");
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
		public AssembliesWith OrWith<TAttribute>()
			where TAttribute : Attribute
			=> new(_source, _filter.Extend((result, assembly) => result || assembly.HasAttribute<TAttribute>(),
				description
					=> $"{description} or with {Formatter.Format(typeof(TAttribute))}"));

		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" /> that
		///     matches the <paramref name="predicate" />.
		/// </summary>
		public AssembliesWith OrWith<TAttribute>(
			Func<TAttribute, bool>? predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
			where TAttribute : Attribute
			=> new(_source, _filter.Extend(
				(result, assembly) => result || assembly.HasAttribute(predicate),
				description
					=> $"{description} or with {Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()}"));
	}
}
