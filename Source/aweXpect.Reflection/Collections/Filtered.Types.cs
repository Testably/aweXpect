using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;

// ReSharper disable MemberHidesStaticFromOuterClass

namespace aweXpect.Reflection.Collections;

public static partial class Filtered
{
	/// <summary>
	///     Container for a filterable collection of <see cref="Type" />.
	/// </summary>
	public class Types : Filtered<Type, Types>,
		IDescribableSubject,
		IMembers.IPrivate,
		IMembers.IProtected,
		ILimitedAbstractSealedMembers
	{
		private const string DeclaringTypesPrefix = "declaring types of ";

		private readonly Assemblies? _assemblies;
		private readonly string _description;

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		public Types(IEnumerable<Type?> source, string description)
			: base(source.WhereNotNull())
		{
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Assemblies assemblies, string description) : base(
			assemblies.SelectMany(GetLoadableTypes))
		{
			_assemblies = assemblies;
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Constructors constructors) : base(constructors
			.Select(constructorInfo => constructorInfo.DeclaringType)
			.WhereNotNull()
			.Distinct())
		{
			_description = DeclaringTypesPrefix + constructors.GetDescription();
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Properties properties) : base(properties
			.Select(propertyInfo => propertyInfo.DeclaringType)
			.WhereNotNull()
			.Distinct())
		{
			_description = DeclaringTypesPrefix + properties.GetDescription();
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Methods methods) : base(methods
			.Select(methodInfo => methodInfo.DeclaringType)
			.WhereNotNull()
			.Distinct())
		{
			_description = DeclaringTypesPrefix + methods.GetDescription();
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Fields fields) : base(fields
			.Select(fieldInfo => fieldInfo.DeclaringType)
			.WhereNotNull()
			.Distinct())
		{
			_description = DeclaringTypesPrefix + fields.GetDescription();
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Events events) : base(events
			.Select(eventInfo => eventInfo.DeclaringType)
			.WhereNotNull()
			.Distinct())
		{
			_description = DeclaringTypesPrefix + events.GetDescription();
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		protected Types(Types inner) : this(inner, inner.Filters)
		{
		}

		private Types(Types inner, List<IFilter<Type>> filters) : base(inner, filters)
		{
			_description = inner._description;
			_assemblies = inner._assemblies;
		}

		/// <inheritdoc />
		private protected override Types CloneWith(List<IFilter<Type>> filters)
			=> new(this, filters);

		/// <summary>
		///     Filters for public members.
		/// </summary>
		public IMembers Public => new TypesMemberBuilder(this, MemberFilterState.Empty.WithAccess(AccessModifiers.Public));

		/// <summary>
		///     Filters for private members.
		/// </summary>
		public IMembers.IPrivate Private
			=> new TypesMemberBuilder(this, MemberFilterState.Empty.WithAccess(AccessModifiers.Private));

		/// <summary>
		///     Filters for protected members.
		/// </summary>
		public IMembers.IProtected Protected
			=> new TypesMemberBuilder(this, MemberFilterState.Empty.WithAccess(AccessModifiers.Protected));

		/// <summary>
		///     Filters for internal members.
		/// </summary>
		public IMembers Internal
			=> new TypesMemberBuilder(this, MemberFilterState.Empty.WithAccess(AccessModifiers.Internal));

		/// <inheritdoc />
		public string GetDescription()
		{
			string description = _description;
			foreach (IFilter<Type> filter in Filters)
			{
				description = filter.Describes(description);
			}

			if (_assemblies is not null)
			{
				return description + _assemblies.GetDescription();
			}

			return description;
		}

		/// <summary>
		///     Filters for static members.
		/// </summary>
		public IMemberSelectors Static => new TypesMemberBuilder(this, MemberFilterState.Empty.WithStatic());

		/// <summary>
		///     Filters for abstract members.
		/// </summary>
		public ILimitedAbstractSealedMembers Abstract
			=> new TypesMemberBuilder(this, MemberFilterState.Empty.WithAbstract());

		/// <summary>
		///     Filters for sealed members.
		/// </summary>
		public ILimitedAbstractSealedMembers Sealed
			=> new TypesMemberBuilder(this, MemberFilterState.Empty.WithSealed());

		/// <inheritdoc cref="IMembers.IPrivate.Protected" />
		IMembers IMembers.IPrivate.Protected
			=> new TypesMemberBuilder(this, MemberFilterState.Empty.WithAccess(AccessModifiers.PrivateProtected));

		/// <summary>
		///     Get all constructors in the filtered types.
		/// </summary>
		public Constructors Constructors() => new TypesMemberBuilder(this, MemberFilterState.Empty).Constructors();

		/// <summary>
		///     Get all events in the filtered types, including inherited members or only those declared directly
		///     on the type according to the <paramref name="memberScope" />.
		/// </summary>
		public Events Events(MemberScope memberScope = MemberScope.DeclaredOnly)
			=> new TypesMemberBuilder(this, MemberFilterState.Empty).Events(memberScope);

		/// <summary>
		///     Get all fields in the filtered types, including inherited members or only those declared directly
		///     on the type according to the <paramref name="memberScope" />.
		/// </summary>
		public Fields Fields(MemberScope memberScope = MemberScope.DeclaredOnly)
			=> new TypesMemberBuilder(this, MemberFilterState.Empty).Fields(memberScope);

		/// <summary>
		///     Get all methods in the filtered types, including inherited members or only those declared directly
		///     on the type according to the <paramref name="memberScope" />.
		/// </summary>
		public Methods Methods(MemberScope memberScope = MemberScope.DeclaredOnly)
			=> new TypesMemberBuilder(this, MemberFilterState.Empty).Methods(memberScope);

		/// <summary>
		///     Get all properties in the filtered types, including inherited members or only those declared directly
		///     on the type according to the <paramref name="memberScope" />.
		/// </summary>
		public Properties Properties(MemberScope memberScope = MemberScope.DeclaredOnly)
			=> new TypesMemberBuilder(this, MemberFilterState.Empty).Properties(memberScope);

		/// <inheritdoc cref="IMembers.IProtected.Internal" />
		IMembers IMembers.IProtected.Internal
			=> new TypesMemberBuilder(this, MemberFilterState.Empty.WithAccess(AccessModifiers.ProtectedInternal));

		/// <summary>
		///     Gets the types of the <paramref name="assembly" />, ignoring those that fail to load.
		/// </summary>
		/// <remarks>
		///     <see cref="Assembly.GetTypes()" /> throws a <see cref="ReflectionTypeLoadException" /> when any type cannot be
		///     loaded (e.g. an unresolvable dependency). The exception still exposes the types that loaded successfully via
		///     <see cref="ReflectionTypeLoadException.Types" />, so we fall back to those instead of failing the whole query.
		/// </remarks>
		private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
		{
			IEnumerable<Type> types;
			try
			{
				types = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException exception)
			{
				types = exception.Types.Where(type => type is not null)!;
			}

			CompilerGeneratedMembers included = Customize.aweXpect.Reflection()
				.IncludedCompilerGeneratedMembers().Get();
			return types.Where(type => !type.IsCompilerGenerated() ||
			                           included.HasFlag(CompilerGeneratedMembers.Types));
		}

		/// <summary>
		///     Get all assemblies of the filtered types.
		/// </summary>
		public Assemblies Assemblies() => new(this);

		/// <summary>
		///     A Container for a filterable collection of <see cref="Type" />,
		///     that also allows specifying string equality options.
		/// </summary>
		public class StringEqualityResult : Types
		{
			private readonly StringEqualityOptions _options;

			internal StringEqualityResult(Types inner, StringEqualityOptions options) : base(inner)
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
		///     A Container for a filterable collection of <see cref="Type" />,
		///     that also allows specifying string equality options and types.
		/// </summary>
		public class StringEqualityResultType : StringEqualityResult
		{
			private readonly StringEqualityOptions _options;

			internal StringEqualityResultType(Types inner, StringEqualityOptions options) : base(inner, options)
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

		/// <summary>
		///     A filtered collection of <see cref="System.Type" /> from a namespace-based dependency filter, allowing
		///     to widen the targeted/allowed namespaces and to opt out of sub-namespace matching.
		/// </summary>
		/// <remarks>
		///     Like all filtered collections, this is an immutable value object: <see cref="OrOn" /> and
		///     <see cref="ExcludingSubNamespaces" /> do not mutate this instance but rebuild a fresh filter from the
		///     original base collection, so deriving multiple views from the same instance cannot corrupt each other.
		/// </remarks>
		public sealed class NamespaceDependencyFilterResult : Types
		{
			private readonly Func<NamespaceDependencyOptions, Types> _build;
			private readonly NamespaceDependencyOptions _options;

			internal NamespaceDependencyFilterResult(
				NamespaceDependencyOptions options,
				Func<NamespaceDependencyOptions, Types> build)
				: base(build(options))
			{
				_options = options;
				_build = build;
			}

			/// <summary>
			///     Widens the filter by the given <paramref name="namespaces" /> (including sub-namespaces unless
			///     <see cref="ExcludingSubNamespaces" /> is used).
			/// </summary>
			public NamespaceDependencyFilterResult OrOn(params IEnumerable<string> namespaces)
			{
				NamespaceDependencyOptions widened = _options.Copy();
				widened.OrOn(namespaces);
				return new NamespaceDependencyFilterResult(widened, _build);
			}

			/// <summary>
			///     Excludes sub-namespaces from matching for the whole filter (including any <see cref="OrOn" /> additions).
			/// </summary>
			/// <remarks>
			///     Without this call, a namespace matches itself and all its sub-namespaces (so <c>Foo.Bar</c> includes
			///     <c>Foo.Bar.Baz</c> but not <c>Foo.BarBaz</c>).
			/// </remarks>
			public NamespaceDependencyFilterResult ExcludingSubNamespaces()
			{
				NamespaceDependencyOptions refined = _options.Copy();
				refined.ExcludingSubNamespaces();
				return new NamespaceDependencyFilterResult(refined, _build);
			}
		}

		/// <summary>
		///     A filtered collection of <see cref="System.Type" /> from a namespace-based depends-only-on filter,
		///     allowing to widen the allowed namespaces and to opt out of sub-namespace matching — for the allowed
		///     namespaces and for the type's own namespace.
		/// </summary>
		/// <remarks>
		///     Like all filtered collections, this is an immutable value object: <see cref="OrOn" />,
		///     <see cref="ExcludingSubNamespaces" /> and <see cref="ExcludingOwnSubNamespaces" /> do not mutate this
		///     instance but rebuild a fresh filter from the original base collection, so deriving multiple views from
		///     the same instance cannot corrupt each other.
		/// </remarks>
		public sealed class NamespaceDependencyOnlyOnFilterResult : Types
		{
			private readonly Func<NamespaceDependencyOptions, Types> _build;
			private readonly NamespaceDependencyOptions _options;

			internal NamespaceDependencyOnlyOnFilterResult(
				NamespaceDependencyOptions options,
				Func<NamespaceDependencyOptions, Types> build)
				: base(build(options))
			{
				_options = options;
				_build = build;
			}

			/// <summary>
			///     Widens the filter by the given <paramref name="namespaces" /> (including sub-namespaces unless
			///     <see cref="ExcludingSubNamespaces" /> is used).
			/// </summary>
			public NamespaceDependencyOnlyOnFilterResult OrOn(params IEnumerable<string> namespaces)
			{
				NamespaceDependencyOptions widened = _options.Copy();
				widened.OrOn(namespaces);
				return new NamespaceDependencyOnlyOnFilterResult(widened, _build);
			}

			/// <summary>
			///     Excludes sub-namespaces of the allowed namespaces from matching for the whole filter (including any
			///     <see cref="OrOn" /> additions).
			/// </summary>
			/// <remarks>
			///     Without this call, a namespace matches itself and all its sub-namespaces (so <c>Foo.Bar</c> includes
			///     <c>Foo.Bar.Baz</c> but not <c>Foo.BarBaz</c>).
			///     <para />
			///     The type's own namespace is always allowed, and its sub-namespaces stay allowed unless
			///     <see cref="ExcludingOwnSubNamespaces" /> is also used.
			/// </remarks>
			public NamespaceDependencyOnlyOnFilterResult ExcludingSubNamespaces()
			{
				NamespaceDependencyOptions refined = _options.Copy();
				refined.ExcludingSubNamespaces();
				return new NamespaceDependencyOnlyOnFilterResult(refined, _build);
			}

			/// <summary>
			///     Excludes sub-namespaces of the type's own namespace from being implicitly allowed (so a <c>Foo</c>
			///     type referencing <c>Foo.Bar</c> is filtered out unless <c>Foo.Bar</c> is explicitly allowed).
			/// </summary>
			/// <remarks>
			///     The type's own namespace itself is always allowed.
			/// </remarks>
			public NamespaceDependencyOnlyOnFilterResult ExcludingOwnSubNamespaces()
			{
				NamespaceDependencyOptions refined = _options.Copy();
				refined.ExcludingOwnSubNamespaces();
				return new NamespaceDependencyOnlyOnFilterResult(refined, _build);
			}
		}

		/// <summary>
		///     A filtered collection of <see cref="System.Type" /> within a namespace, that also allows clarifying
		///     the assembly source once (it defaults to all loaded assemblies).
		/// </summary>
		/// <remarks>
		///     The clarification methods return a plain <see cref="Types" /> collection rooted at the given source,
		///     so the source can only be specified once and must be specified before applying further filters.
		/// </remarks>
		public sealed class InNamespaceResult : Types
		{
			private readonly string _namespace;

			internal InNamespaceResult(string @namespace)
				: base(In.AllLoadedAssemblies().Types().WithinNamespace(@namespace))
			{
				_namespace = @namespace;
			}

			/// <summary>
			///     Clarifies that the types within the namespace are searched in all loaded assemblies from the
			///     current <see cref="System.AppDomain.CurrentDomain" /> (the default).
			/// </summary>
			public Types InAllLoadedAssemblies()
				=> From(In.AllLoadedAssemblies());

			/// <summary>
			///     Clarifies that the types within the namespace are searched in the given <paramref name="assemblies" />.
			/// </summary>
			public Types InAssemblies(params IEnumerable<Assembly?> assemblies)
				=> From(In.Assemblies(assemblies));

			/// <summary>
			///     Clarifies that the types within the namespace are searched in the assembly that contains
			///     the <typeparamref name="TType" />.
			/// </summary>
			public Types InAssemblyContaining<TType>()
				=> From(In.AssemblyContaining<TType>());

			/// <summary>
			///     Clarifies that the types within the namespace are searched in the assembly that contains
			///     the <paramref name="type" />.
			/// </summary>
			public Types InAssemblyContaining(Type type)
				=> From(In.AssemblyContaining(type));

			private Types From(Assemblies assemblies)
				=> assemblies.Types().WithinNamespace(_namespace);
		}

		/// <summary>
		///     A filtered collection of <see cref="System.Type" /> from a dependency filter whose targets are filtered
		///     collections of types, allowing to widen the targeted/allowed collections.
		/// </summary>
		/// <remarks>
		///     Like all filtered collections, this is an immutable value object: <see cref="OrOn" /> does not mutate
		///     this instance but rebuilds a fresh filter from the original base collection, so deriving multiple views
		///     from the same instance cannot corrupt each other.
		/// </remarks>
		public sealed class TypeSetDependencyFilterResult : Types
		{
			private readonly Func<TypeSetDependencyOptions, Types> _build;
			private readonly TypeSetDependencyOptions _options;

			internal TypeSetDependencyFilterResult(
				TypeSetDependencyOptions options,
				Func<TypeSetDependencyOptions, Types> build)
				: base(build(options))
			{
				_options = options;
				_build = build;
			}

			/// <summary>
			///     Widens the filter by the given <paramref name="targets" />.
			/// </summary>
			public TypeSetDependencyFilterResult OrOn(params Filtered.Types[] targets)
			{
				TypeSetDependencyOptions widened = _options.Copy();
				widened.OrOn(targets);
				return new TypeSetDependencyFilterResult(widened, _build);
			}
		}

		/// <summary>
		///     A filtered collection of <see cref="System.Type" /> from a depends-only-on filter whose allowed
		///     targets are filtered collections of types, allowing to widen the allowed collections and to opt out
		///     of the implicit allowance of the type's own sub-namespaces.
		/// </summary>
		/// <remarks>
		///     Like all filtered collections, this is an immutable value object: <see cref="OrOn" /> and
		///     <see cref="ExcludingOwnSubNamespaces" /> do not mutate this instance but rebuild a fresh filter from
		///     the original base collection, so deriving multiple views from the same instance cannot corrupt each
		///     other.
		/// </remarks>
		public sealed class TypeSetDependencyOnlyOnFilterResult : Types
		{
			private readonly Func<TypeSetDependencyOptions, Types> _build;
			private readonly TypeSetDependencyOptions _options;

			internal TypeSetDependencyOnlyOnFilterResult(
				TypeSetDependencyOptions options,
				Func<TypeSetDependencyOptions, Types> build)
				: base(build(options))
			{
				_options = options;
				_build = build;
			}

			/// <summary>
			///     Widens the filter by the given <paramref name="targets" />.
			/// </summary>
			public TypeSetDependencyOnlyOnFilterResult OrOn(params Filtered.Types[] targets)
			{
				TypeSetDependencyOptions widened = _options.Copy();
				widened.OrOn(targets);
				return new TypeSetDependencyOnlyOnFilterResult(widened, _build);
			}

			/// <summary>
			///     Excludes sub-namespaces of the type's own namespace from being implicitly allowed (so a <c>Foo</c>
			///     type referencing <c>Foo.Bar</c> is filtered out unless <c>Foo.Bar</c> types are part of an allowed
			///     collection).
			/// </summary>
			/// <remarks>
			///     The type's own namespace itself is always allowed.
			/// </remarks>
			public TypeSetDependencyOnlyOnFilterResult ExcludingOwnSubNamespaces()
			{
				TypeSetDependencyOptions refined = _options.Copy();
				refined.ExcludingOwnSubNamespaces();
				return new TypeSetDependencyOnlyOnFilterResult(refined, _build);
			}
		}
	}
}
