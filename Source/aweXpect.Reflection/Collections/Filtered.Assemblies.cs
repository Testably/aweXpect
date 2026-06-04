using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Options;

// ReSharper disable MemberHidesStaticFromOuterClass

namespace aweXpect.Reflection.Collections;

public static partial class Filtered
{
	/// <summary>
	///     Container for a filterable collection of <see cref="Assembly" />.
	/// </summary>
	public class Assemblies : Filtered<Assembly, Assemblies>,
		IDescribableSubject,
		ITypeAssemblies.IProtected,
		ITypeAssemblies.IPrivate
	{
		private readonly string _description;

		/// <summary>
		///     Container for a filterable collection of <see cref="Assembly" />.
		/// </summary>
		public Assemblies(IEnumerable<Assembly?> source, string description)
			: base(source.WhereNotNull())
		{
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Assembly" />.
		/// </summary>
		public Assemblies(Assembly? source, string description) : this([source,], description)
		{
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Assembly" />.
		/// </summary>
		internal Assemblies(Types types) : base(types.Select(type => type.Assembly).Distinct())
		{
			_description = "assemblies containing " + types.GetDescription();
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Assembly" />.
		/// </summary>
		protected Assemblies(Assemblies inner) : this(inner, inner.Filters)
		{
		}

		private Assemblies(Assemblies inner, List<IFilter<Assembly>> filters) : base(inner, filters)
		{
			_description = inner._description;
		}

		/// <inheritdoc />
		private protected override Assemblies CloneWith(List<IFilter<Assembly>> filters)
			=> new(this, filters);

		/// <summary>
		///     Filters for public types.
		/// </summary>
		public ITypeAssemblies Public => NewBuilder(MemberFilterState.Empty.WithAccess(AccessModifiers.Public));

		/// <summary>
		///     Filters for private types.
		/// </summary>
		public ITypeAssemblies.IPrivate Private => NewBuilder(MemberFilterState.Empty.WithAccess(AccessModifiers.Private));

		/// <summary>
		///     Filters for protected types.
		/// </summary>
		public ITypeAssemblies.IProtected Protected
			=> NewBuilder(MemberFilterState.Empty.WithAccess(AccessModifiers.Protected));

		/// <summary>
		///     Filters for internal types.
		/// </summary>
		public ITypeAssemblies Internal => NewBuilder(MemberFilterState.Empty.WithAccess(AccessModifiers.Internal));

		/// <inheritdoc />
		public string GetDescription()
		{
			string description = _description;
			foreach (IFilter<Assembly> filter in Filters)
			{
				description = filter.Describes(description);
			}

			return description;
		}

		/// <inheritdoc cref="ITypeAssemblies.IPrivate.Protected" />
		ITypeAssemblies ITypeAssemblies.IPrivate.Protected
			=> NewBuilder(MemberFilterState.Empty.WithAccess(AccessModifiers.PrivateProtected));

		/// <inheritdoc cref="ITypeAssemblies.Abstract" />
		public ILimitedAbstractSealedTypeAssemblies<ILimitedAbstractSealedTypeAssemblies> Abstract
			=> NewBuilder(MemberFilterState.Empty).Abstract;

		/// <inheritdoc cref="ITypeAssemblies.Sealed" />
		public ILimitedAbstractSealedTypeAssemblies<ILimitedAbstractSealedTypeAssemblies> Sealed
			=> NewBuilder(MemberFilterState.Empty).Sealed;

		/// <inheritdoc cref="ITypeAssemblies.Static" />
		public ILimitedStaticTypeAssemblies<ILimitedStaticTypeAssemblies> Static
			=> NewBuilder(MemberFilterState.Empty).Static;

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies{TLimitedTypeAssemblies}.Generic" />
		public ITypeAssemblies Generic => NewBuilder(MemberFilterState.Empty).Generic;

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies{TLimitedTypeAssemblies}.Nested" />
		public ITypeAssemblies Nested => NewBuilder(MemberFilterState.Empty).Nested;

		/// <inheritdoc cref="ITypeAssemblies.IProtected.Internal" />
		ITypeAssemblies ITypeAssemblies.IProtected.Internal
			=> NewBuilder(MemberFilterState.Empty.WithAccess(AccessModifiers.ProtectedInternal));

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Types(AccessModifiers)" />
		public Types Types(AccessModifiers accessModifier = AccessModifiers.Any)
			=> NewBuilder(MemberFilterState.Empty).Types(accessModifier);

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Classes(AccessModifiers)" />
		public Types Classes(AccessModifiers accessModifier = AccessModifiers.Any)
			=> NewBuilder(MemberFilterState.Empty).Classes(accessModifier);

		/// <inheritdoc cref="ILimitedAbstractSealedTypeAssemblies.Records(AccessModifiers)" />
		public Types Records(AccessModifiers accessModifier = AccessModifiers.Any)
			=> NewBuilder(MemberFilterState.Empty).Records(accessModifier);

		/// <inheritdoc cref="ITypeAssemblies.RecordStructs(AccessModifiers)" />
		public Types RecordStructs(AccessModifiers accessModifier = AccessModifiers.Any)
			=> NewBuilder(MemberFilterState.Empty).RecordStructs(accessModifier);

		/// <inheritdoc cref="ITypeAssemblies.Structs(AccessModifiers)" />
		public Types Structs(AccessModifiers accessModifier = AccessModifiers.Any)
			=> NewBuilder(MemberFilterState.Empty).Structs(accessModifier);

		/// <inheritdoc cref="ITypeAssemblies.Interfaces(AccessModifiers)" />
		public Types Interfaces(AccessModifiers accessModifier = AccessModifiers.Any)
			=> NewBuilder(MemberFilterState.Empty).Interfaces(accessModifier);

		/// <inheritdoc cref="ITypeAssemblies.Enums(AccessModifiers)" />
		public Types Enums(AccessModifiers accessModifier = AccessModifiers.Any)
			=> NewBuilder(MemberFilterState.Empty).Enums(accessModifier);

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Constructors()" />
		public Constructors Constructors() => NewBuilder(MemberFilterState.Empty).Constructors();

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Events()" />
		public Events Events() => NewBuilder(MemberFilterState.Empty).Events();

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Fields()" />
		public Fields Fields() => NewBuilder(MemberFilterState.Empty).Fields();

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Methods()" />
		public Methods Methods() => NewBuilder(MemberFilterState.Empty).Methods();

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Properties()" />
		public Properties Properties() => NewBuilder(MemberFilterState.Empty).Properties();

		private AssembliesTypeBuilder NewBuilder(MemberFilterState memberState)
			=> new(this, memberState, new List<Func<Type, bool>>(), "");

		/// <summary>
		///     A Container for a filterable collection of <see cref="Assembly" />,
		///     that also allows specifying string equality options.
		/// </summary>
		public class StringEqualityResult : Assemblies
		{
			private readonly StringEqualityOptions _options;

			internal StringEqualityResult(Assemblies inner, StringEqualityOptions options) : base(inner)
			{
				_options = options;
			}

			/// <summary>
			///     Ignores casing when comparing the <see langword="string" />s,
			///     according to the <paramref name="ignoreCase" /> parameter.
			/// </summary>
			public StringEqualityResult IgnoringCase(bool ignoreCase = true)
			{
				_options.IgnoringCase(ignoreCase);
				return this;
			}

			/// <summary>
			///     Ignores leading white-space when comparing <see langword="string" />s,
			///     according to the <paramref name="ignoreLeadingWhiteSpace" /> parameter.
			/// </summary>
			/// <remarks>
			///     Note:<br />
			///     This affects the index of first mismatch, as the removed whitespace is also ignored for the index calculation!
			/// </remarks>
			public StringEqualityResult IgnoringLeadingWhiteSpace(bool ignoreLeadingWhiteSpace = true)
			{
				_options.IgnoringLeadingWhiteSpace(ignoreLeadingWhiteSpace);
				return this;
			}

			/// <summary>
			///     Ignores trailing white-space when comparing <see langword="string" />s,
			///     according to the <paramref name="ignoreTrailingWhiteSpace" /> parameter.
			/// </summary>
			public StringEqualityResult IgnoringTrailingWhiteSpace(bool ignoreTrailingWhiteSpace = true)
			{
				_options.IgnoringTrailingWhiteSpace(ignoreTrailingWhiteSpace);
				return this;
			}

			/// <summary>
			///     Uses the provided <paramref name="comparer" /> for comparing <see langword="string" />s.
			/// </summary>
			public StringEqualityResult Using(IEqualityComparer<string> comparer)
			{
				_options.UsingComparer(comparer);
				return this;
			}
		}

		/// <summary>
		///     A Container for a filterable collection of <see cref="Assembly" />,
		///     that also allows specifying string equality options and types.
		/// </summary>
		public class StringEqualityResultType : StringEqualityResult
		{
			private readonly StringEqualityOptions _options;

			internal StringEqualityResultType(Assemblies inner, StringEqualityOptions options) : base(inner, options)
			{
				_options = options;
			}

			/// <summary>
			///     Interprets the expected <see langword="string" /> to be exactly equal.
			/// </summary>
			public StringEqualityResult Exactly()
			{
				_options.Exactly();
				return this;
			}

			/// <summary>
			///     Interprets the expected <see langword="string" /> as a prefix, so that the actual value starts with it.
			/// </summary>
			public StringEqualityResult AsPrefix()
			{
				_options.AsPrefix();
				return this;
			}

			/// <summary>
			///     Interprets the expected <see langword="string" /> as <see cref="Regex" /> pattern.
			/// </summary>
			public StringEqualityResult AsRegex()
			{
				_options.AsRegex();
				return this;
			}

			/// <summary>
			///     Interprets the expected <see langword="string" /> as a suffix, so that the actual value ends with it.
			/// </summary>
			public StringEqualityResult AsSuffix()
			{
				_options.AsSuffix();
				return this;
			}

			/// <summary>
			///     Interprets the expected <see langword="string" /> as wildcard pattern.<br />
			///     Supports * to match zero or more characters and ? to match exactly one character.
			/// </summary>
			public StringEqualityResult AsWildcard()
			{
				_options.AsWildcard();
				return this;
			}
		}
	}
}
