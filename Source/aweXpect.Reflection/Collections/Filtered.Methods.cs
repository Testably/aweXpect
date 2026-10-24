using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable MemberHidesStaticFromOuterClass

namespace aweXpect.Reflection.Collections;

public static partial class Filtered
{
	/// <summary>
	///     Container for a filterable collection of <see cref="MethodInfo" />.
	/// </summary>
	public class Methods : Filtered<MethodInfo, Methods>, IDescribableSubject
	{
		private readonly string _description;
		private readonly bool _includeOperators;
		private readonly MemberScope _memberScope;
		private readonly Types? _types;

		/// <summary>
		///     Container for a filterable collection of <see cref="MethodInfo" />.
		/// </summary>
		internal Methods(Types types, string description,
			MemberScope memberScope = MemberScope.DeclaredOnly)
			: this(types, description, memberScope, false)
		{
		}

		private Methods(Types types, string description, MemberScope memberScope, bool includeOperators)
			: base(types.SelectMany(type => type.GetDeclaredMethods(memberScope, includeOperators)))
		{
			_types = types;
			_description = description;
			_memberScope = memberScope;
			_includeOperators = includeOperators;
		}

		/// <summary>
		///     Container for a filterable collection of the given <paramref name="methods" />.
		/// </summary>
		internal Methods(IEnumerable<MethodInfo?> methods, string description)
			: base(methods.WhereNotNull())
		{
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="MethodInfo" />.
		/// </summary>
		private protected Methods(Methods inner) : this(inner, inner.Filters)
		{
		}

		private Methods(Methods inner, IReadOnlyList<IFilter<MethodInfo>> filters) : base(inner, filters)
		{
			_description = inner._description;
			_types = inner._types;
			_memberScope = inner._memberScope;
			_includeOperators = inner._includeOperators;
		}

		/// <inheritdoc />
		public string GetDescription()
		{
			string description = _description;
			foreach (IFilter<MethodInfo> filter in Filters)
			{
				description = filter.Describes(description);
			}

			if (_types is not null)
			{
				return description + _types.GetDescription().PrefixIn();
			}

			return description;
		}

		/// <inheritdoc />
		private protected override Methods CloneWith(IReadOnlyList<IFilter<MethodInfo>> filters)
			=> new(this, filters);

		/// <summary>
		///     Returns a copy of this collection that additionally includes operator special-name members, so that operator
		///     filters work even when operators are not opted in via <c>IncludedSpecialNameMembers</c>.
		/// </summary>
		/// <remarks>
		///     Operator inclusion is fixed when the underlying methods are gathered, so it cannot be toggled in place after
		///     construction. This rebuilds the collection from its source types - re-applying any filters already composed -
		///     with operators included. Collections that already include operators, or that were created from an explicit
		///     method list (and therefore have no source types to rebuild from), are returned unchanged.
		/// </remarks>
		internal Methods WithOperatorsIncluded()
		{
			if (_includeOperators || _types is null)
			{
				return this;
			}

			Methods rebuilt = new(_types, _description, _memberScope, true);
			return rebuilt.CloneWith([.. Filters,]);
		}

		/// <summary>
		///     Get all declaring types of the filtered methods.
		/// </summary>
		public Types DeclaringTypes() => new(this);

		/// <summary>
		///     A Container for a filterable collection of <see cref="MethodInfo" />,
		///     that also allows specifying string equality options via <see cref="StringEqualityOptionsExtensions" />.
		/// </summary>
		public class StringEqualityResult : Methods, IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions
		{
			private readonly StringEqualityOptions _options;

			internal StringEqualityResult(Methods inner, StringEqualityOptions options) : base(inner)
			{
				_options = options;
			}

			/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
			StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => _options;
		}
	}
}
