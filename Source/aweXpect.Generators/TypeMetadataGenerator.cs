using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace aweXpect.Generators;

/// <summary>
///     The <see cref="IIncrementalGenerator" /> that registers the members of every type reaching an equivalency
///     comparison in <c>aweXpect.Core.Metadata.TypeMetadataRegistry</c>.
/// </summary>
/// <remarks>
///     A call site whose parameter or type parameter carries <c>RequiresMemberMetadataAttribute</c> seeds the static
///     type of its argument, and the walk follows the declared members transitively. Without the registration, the
///     comparison has to reflect over the members, which publishing with trimming or Native AOT enabled removes.
/// </remarks>
[Generator]
public class TypeMetadataGenerator : IIncrementalGenerator
{
	private const string MarkerAttribute = "aweXpect.Core.Metadata.RequiresMemberMetadataAttribute";
	private const string GenerateAttribute = "aweXpect.Core.Metadata.GenerateMetadataAttribute";
	private const string Registry = "global::aweXpect.Core.Metadata.TypeMetadataRegistry";

	private static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat
		.WithMiscellaneousOptions(SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions &
		                          ~SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

	void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValueProvider<bool> isSupported = context.CompilationProvider
			.Select(static (c, _) => SupportsRegistration(c) && HasModuleInitializer(c));

		IncrementalValueProvider<ImmutableArray<TypeRegistration>> fromCallSites = context.SyntaxProvider
			.CreateSyntaxProvider(
				static (node, _) => node is InvocationExpressionSyntax,
				static (ctx, cancellationToken) => FromCallSite(ctx, cancellationToken))
			.SelectMany(static (x, _) => x)
			.Collect();

		IncrementalValueProvider<ImmutableArray<TypeRegistration>> fromAssembly = context.CompilationProvider
			.Select(static (c, _) => FromAssemblyAttributes(c));

		context.RegisterSourceOutput(fromCallSites.Combine(fromAssembly).Combine(isSupported),
			static (spc, source) => Emit(spc, source.Left.Left, source.Left.Right, source.Right));
	}

	private static ImmutableArray<TypeRegistration> FromCallSite(GeneratorSyntaxContext context,
		CancellationToken cancellationToken)
	{
		InvocationExpressionSyntax invocation = (InvocationExpressionSyntax)context.Node;
		if (context.SemanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol is not IMethodSymbol method)
		{
			return ImmutableArray<TypeRegistration>.Empty;
		}

		IMethodSymbol constructed = method.GetConstructedReducedFrom() ?? method;
		IMethodSymbol definition = constructed.OriginalDefinition;
		int reductionOffset = method.ReducedFrom is null ? 0 : 1;
		MetadataWalker walker = new(context.SemanticModel.Compilation);
		bool hasMarkedParameter = false;

		for (int i = 0; i < definition.Parameters.Length; i++)
		{
			if (!IsMarked(definition.Parameters[i]))
			{
				continue;
			}

			hasMarkedParameter = true;
			ExpressionSyntax? argument = FindArgument(invocation, constructed.Parameters[i], i - reductionOffset);
			if (argument is not null)
			{
				walker.Seed(context.SemanticModel.GetTypeInfo(argument, cancellationToken).Type);
			}

			walker.Seed(constructed.Parameters[i].Type);
		}

		for (int i = 0; i < definition.TypeParameters.Length; i++)
		{
			if (IsMarked(definition.TypeParameters[i]))
			{
				walker.Seed(constructed.TypeArguments[i]);
			}
		}

		if (hasMarkedParameter && GetReceiverType(method, constructed) is INamedTypeSymbol { IsGenericType: true, } receiver)
		{
			foreach (ITypeSymbol typeArgument in receiver.TypeArguments)
			{
				walker.Seed(typeArgument);
			}
		}

		return walker.Registrations;
	}

	/// <remarks>
	///     The comparison fetches the members of the subject by name as well, so the subject type of the receiver
	///     has to be registered alongside the expected type.
	/// </remarks>
	private static ITypeSymbol? GetReceiverType(IMethodSymbol method, IMethodSymbol constructed)
	{
		if (constructed.IsExtensionMethod)
		{
			return constructed.Parameters[0].Type;
		}

		return method.IsStatic ? null : method.ContainingType;
	}

	private static ExpressionSyntax? FindArgument(InvocationExpressionSyntax invocation, IParameterSymbol parameter,
		int position)
	{
		if (position < 0)
		{
			return (invocation.Expression as MemberAccessExpressionSyntax)?.Expression;
		}

		SeparatedSyntaxList<ArgumentSyntax> arguments = invocation.ArgumentList.Arguments;
		foreach (ArgumentSyntax argument in arguments)
		{
			if (argument.NameColon?.Name.Identifier.ValueText == parameter.Name)
			{
				return argument.Expression;
			}
		}

		return position < arguments.Count && arguments[position].NameColon is null
			? arguments[position].Expression
			: null;
	}

	private static ImmutableArray<TypeRegistration> FromAssemblyAttributes(Compilation compilation)
	{
		MetadataWalker walker = new(compilation);
		foreach (AttributeData attribute in compilation.Assembly.GetAttributes())
		{
			if (attribute.AttributeClass?.ToDisplayString() == GenerateAttribute &&
			    attribute.ConstructorArguments.Length == 1 &&
			    attribute.ConstructorArguments[0].Value is ITypeSymbol type)
			{
				walker.Seed(type);
			}
		}

		return walker.Registrations;
	}

	private static bool IsMarked(ISymbol symbol)
		=> symbol.GetAttributes().Any(x => x.AttributeClass?.ToDisplayString() == MarkerAttribute);

	/// <remarks>
	///     Registering against an older aweXpect.Core would not compile, so anything but a matching registry
	///     degrades to reflection.
	/// </remarks>
	private static bool SupportsRegistration(Compilation compilation)
		=> compilation
			.GetTypeByMetadataName("aweXpect.Core.Metadata.TypeMetadataRegistry")
			?.GetMembers("RegisterProperty")
			.OfType<IMethodSymbol>()
			.Any(x => x.IsStatic && x.DeclaredAccessibility == Accessibility.Public && x.Parameters.Length == 3) ==
		   true;

	/// <remarks>
	///     Without <c>ModuleInitializerAttribute</c> the target framework cannot be trimmed or AOT-published anyway.
	/// </remarks>
	private static bool HasModuleInitializer(Compilation compilation)
	{
		INamedTypeSymbol? attribute =
			compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.ModuleInitializerAttribute");
		return attribute != null &&
		       (attribute.DeclaredAccessibility == Accessibility.Public ||
		        (attribute.DeclaredAccessibility == Accessibility.Internal &&
		         SymbolEqualityComparer.Default.Equals(attribute.ContainingAssembly, compilation.Assembly)));
	}

	private static void Emit(SourceProductionContext context, ImmutableArray<TypeRegistration> fromCallSites,
		ImmutableArray<TypeRegistration> fromAssembly, bool isSupported)
	{
		if (!isSupported)
		{
			return;
		}

		List<TypeRegistration> registrations = fromCallSites.Concat(fromAssembly)
			.GroupBy(x => x.Key, StringComparer.Ordinal)
			.Select(x => x.First())
			.OrderBy(x => x.Key, StringComparer.Ordinal)
			.ToList();
		if (registrations.Count == 0)
		{
			return;
		}

		StringBuilder sb = new();
		sb.AppendLine("""
		              // <auto-generated />
		              #nullable disable
		              namespace aweXpect.Generators;

		              internal static class TypeMetadataRegistration
		              {
		              	/// <summary>
		              	///     Registers the members of the types that reach an equivalency comparison when the assembly is loaded.
		              	/// </summary>
		              	/// <remarks>
		              	///     Without this registration, the members could only be found by reflection, which fails when the
		              	///     application is published with trimming or Native AOT enabled.
		              	/// </remarks>
		              	[System.Runtime.CompilerServices.ModuleInitializer]
		              	internal static void Register()
		              	{
		              """);
		for (int i = 0; i < registrations.Count; i++)
		{
			sb.Append("\t\tRegister").Append(i).AppendLine("();");
		}

		sb.AppendLine("\t}");
		for (int i = 0; i < registrations.Count; i++)
		{
			sb.AppendLine();
			sb.Append("\t// ").AppendLine(registrations[i].Key);
			sb.Append("\tprivate static void Register").Append(i).AppendLine("()");
			sb.AppendLine("\t{");
			sb.Append(registrations[i].Source);
			sb.AppendLine("\t}");
		}

		sb.AppendLine("}");
		context.AddSource("TypeMetadataRegistration.g.cs", sb.ToString());
	}

	private readonly record struct TypeRegistration(string Key, string Source);

	private readonly record struct Member(string Name, ITypeSymbol Type, bool IsField);

	/// <summary>
	///     Walks a type graph the way the equivalency comparison does, and collects a registration for every type
	///     whose members it would compare.
	/// </summary>
	private sealed class MetadataWalker(Compilation compilation)
	{
		private readonly ImmutableArray<TypeRegistration>.Builder _registrations =
			ImmutableArray.CreateBuilder<TypeRegistration>();

		private readonly HashSet<ITypeSymbol> _visited = new(SymbolEqualityComparer.Default);

		public ImmutableArray<TypeRegistration> Registrations => _registrations.ToImmutable();

		public void Seed(ITypeSymbol? type)
		{
			if (type is IArrayTypeSymbol array)
			{
				Seed(array.ElementType);
				return;
			}

			if (type is not INamedTypeSymbol named ||
			    named.TypeKind is TypeKind.Error or TypeKind.Delegate or TypeKind.Pointer or TypeKind.FunctionPointer)
			{
				return;
			}

			if (named.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
			{
				Seed(named.TypeArguments[0]);
				return;
			}

			if (named.IsTupleType)
			{
				named = named.TupleUnderlyingType ?? named;
			}

			if (!_visited.Add(named) || named.SpecialType == SpecialType.System_Object || IsComparedByValue(named) ||
			    ContainsTypeParameter(named) || IsExpectation(named))
			{
				return;
			}

			if (IsEnumerable(named))
			{
				foreach (INamedTypeSymbol enumerable in named.AllInterfaces.Prepend(named).Where(x
					         => x.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T))
				{
					Seed(enumerable.TypeArguments[0]);
				}

				return;
			}

			List<Member> members = CollectMembers(named);
			foreach (Member member in members)
			{
				Seed(member.Type);
			}

			if (members.Count == 0 || named.IsAbstract || named.IsStatic ||
			    !compilation.IsSymbolAccessibleWithin(named, compilation.Assembly))
			{
				return;
			}

			string? source = EmitRegistration(named, members);
			if (source is not null)
			{
				_registrations.Add(new TypeRegistration(named.ToDisplayString(TypeFormat), source));
			}
		}

		/// <remarks>
		///     Mirrors the reflection over <c>BindingFlags.Public | BindingFlags.Instance</c>: inherited members are
		///     included, a shadowed member is taken from the most derived type only, and indexers are skipped because
		///     their accessors take arguments.
		/// </remarks>
		private static List<Member> CollectMembers(INamedTypeSymbol type)
		{
			HashSet<string> names = new(StringComparer.Ordinal);
			List<Member> members = [];
			for (INamedTypeSymbol? current = type;
			     current is not null && current.SpecialType != SpecialType.System_Object;
			     current = current.BaseType)
			{
				foreach (ISymbol member in current.GetMembers())
				{
					switch (member)
					{
						case IFieldSymbol
						{
							IsStatic: false, IsConst: false, DeclaredAccessibility: Accessibility.Public,
						} field when field.CorrespondingTupleField is null ||
						             SymbolEqualityComparer.Default.Equals(field, field.CorrespondingTupleField):
							if (names.Add(field.Name) && CanBeMemberType(field.Type))
							{
								members.Add(new Member(field.Name, field.Type, true));
							}

							break;
						case IPropertySymbol
						{
							IsStatic: false, IsIndexer: false, RefKind: RefKind.None,
							GetMethod.DeclaredAccessibility: Accessibility.Public,
						} property:
							if (names.Add(property.Name) && CanBeMemberType(property.Type))
							{
								members.Add(new Member(property.Name, property.Type, false));
							}

							break;
					}
				}
			}

			return members;
		}

		private static string? EmitRegistration(INamedTypeSymbol type, List<Member> members)
		{
			StringBuilder sb = new();
			if (IsNameable(type))
			{
				string typeName = type.ToDisplayString(TypeFormat);
				foreach (Member member in members)
				{
					sb.Append("\t\t").Append(Registry).Append(member.IsField ? ".RegisterField<" : ".RegisterProperty<")
						.Append(typeName).Append(", ").Append(member.Type.ToDisplayString(TypeFormat)).Append(">(\"")
						.Append(member.Name).Append("\", o => o.").Append(member.Name).AppendLine(");");
				}

				return sb.ToString();
			}

			string? probe = ProbeExpression(type);
			if (probe is null)
			{
				return null;
			}

			sb.Append("\t\tvar probe = ").Append(probe).AppendLine(";");
			foreach (Member member in members)
			{
				sb.Append("\t\t").Append(Registry).Append(member.IsField ? ".RegisterField(" : ".RegisterProperty(")
					.Append("probe, \"").Append(member.Name).Append("\", o => o.").Append(member.Name).AppendLine(");");
			}

			return sb.ToString();
		}

		/// <remarks>
		///     An anonymous type has no name that can be written in source, so the type argument is inferred from an
		///     instance instead: two anonymous creation expressions with the same members unify to one type within an
		///     assembly.
		/// </remarks>
		private static string? ProbeExpression(ITypeSymbol type)
		{
			if (IsNameable(type))
			{
				return $"default({type.ToDisplayString(TypeFormat)})";
			}

			switch (type)
			{
				case IArrayTypeSymbol { IsSZArray: true, } array:
				{
					string? element = ProbeExpression(array.ElementType);
					return element is null ? null : $"new[] {{ {element}, }}";
				}
				case INamedTypeSymbol { IsAnonymousType: true, } anonymous:
				{
					StringBuilder sb = new("new { ");
					foreach (IPropertySymbol property in anonymous.GetMembers().OfType<IPropertySymbol>())
					{
						string? value = ProbeExpression(property.Type);
						if (value is null)
						{
							return null;
						}

						sb.Append(property.Name).Append(" = ").Append(value).Append(", ");
					}

					return sb.Append('}').ToString();
				}
				default:
					return null;
			}
		}

		private static bool IsNameable(ITypeSymbol type)
			=> type switch
			{
				IArrayTypeSymbol array => IsNameable(array.ElementType),
				INamedTypeSymbol { IsAnonymousType: true, } => false,
				INamedTypeSymbol named => named.TypeArguments.All(IsNameable) &&
				                          (named.ContainingType is null || IsNameable(named.ContainingType)),
				_ => true,
			};

		private static bool CanBeMemberType(ITypeSymbol type)
			=> type.TypeKind is not (TypeKind.Pointer or TypeKind.FunctionPointer) && !type.IsRefLikeType &&
			   type.SpecialType != SpecialType.System_Void;

		private static bool ContainsTypeParameter(ITypeSymbol type)
			=> type switch
			{
				ITypeParameterSymbol => true,
				IArrayTypeSymbol array => ContainsTypeParameter(array.ElementType),
				INamedTypeSymbol named => named.TypeArguments.Any(ContainsTypeParameter) ||
				                          (named.ContainingType is not null &&
				                           ContainsTypeParameter(named.ContainingType)),
				_ => false,
			};

		/// <remarks>
		///     An expectation such as <c>It.Is&lt;T&gt;()</c> is evaluated against the actual value instead of being
		///     compared member by member, so its own members are never enumerated.
		/// </remarks>
		private static bool IsExpectation(INamedTypeSymbol type)
		{
			for (INamedTypeSymbol? current = type; current is not null; current = current.BaseType)
			{
				if (current.ToDisplayString() == "aweXpect.Results.Expectation")
				{
					return true;
				}
			}

			return false;
		}

		private static bool IsEnumerable(INamedTypeSymbol type)
			=> type.AllInterfaces.Prepend(type)
				.Any(x => x.SpecialType == SpecialType.System_Collections_IEnumerable);

		/// <remarks>
		///     Mirrors <c>EquivalencyDefaults.DefaultComparisonType</c>: these types are compared by value, so their
		///     members are never enumerated.
		/// </remarks>
		private static bool IsComparedByValue(INamedTypeSymbol type)
			=> type.TypeKind == TypeKind.Enum ||
			   type.SpecialType is SpecialType.System_Boolean or SpecialType.System_Char or SpecialType.System_SByte
				   or SpecialType.System_Byte or SpecialType.System_Int16 or SpecialType.System_UInt16
				   or SpecialType.System_Int32 or SpecialType.System_UInt32 or SpecialType.System_Int64
				   or SpecialType.System_UInt64 or SpecialType.System_IntPtr or SpecialType.System_UIntPtr
				   or SpecialType.System_Single or SpecialType.System_Double or SpecialType.System_String
				   or SpecialType.System_Decimal or SpecialType.System_DateTime ||
			   type.ToDisplayString() is "System.DateTimeOffset" or "System.TimeSpan" or "System.Guid";
	}
}
