using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace aweXpect.Generators;

/// <summary>
///     The <see cref="IIncrementalGenerator" /> for generating test framework adapters.
/// </summary>
/// <remarks>
///     The generated code runs in the consumer's compilation, so it is written for C# 7.3, the default of net48,
///     and qualifies every name with <c>global::</c>, because a namespace under <c>aweXpect.</c> would shadow it.
/// </remarks>
[Generator]
public partial class FrameworkGenerator : IIncrementalGenerator
{
	/// <summary>
	///     Reported for each generated adapter that is never used, because it cannot register itself and the
	///     referenced aweXpect.Core does not scan the loaded assemblies for it.
	/// </summary>
	private static readonly DiagnosticDescriptor CannotRegisterAdapter = new(
		"aweXpect2002",
		"The test framework adapter cannot register itself",
		"The {0} cannot register itself, because module initializers need C# 9 or later. Set <LangVersion> to 9 or later, or call TestFrameworkRegistry.Register(new aweXpect.Frameworks.{0}()) before the first expectation.",
		"aweXpect.Generators",
		DiagnosticSeverity.Warning,
		true,
		"On .NET 8 or later, the generated test framework adapter is only used when its module initializer registers it. Without it, aweXpect throws its own exceptions, so a skipped or inconclusive test is reported as failed and a failure as an unexpected exception.",
		"https://docs.testably.org/aweXpect/analyzers#test-framework-adapter");

	void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValueProvider<bool> allowsExperimentalMsTest = context.AnalyzerConfigOptionsProvider
			.Select((options, _) =>
				options.GlobalOptions.TryGetValue("build_property.AweXpectExperimentalMSTest", out string? value) &&
				string.Equals(value, "true", StringComparison.OrdinalIgnoreCase));
		IncrementalValueProvider<Settings> settings = context.CompilationProvider
			.Combine(allowsExperimentalMsTest)
			.Select((x, _) => CreateSettings(x.Left, x.Right));

		context.RegisterSourceOutput(settings, Emit);
	}

	private static Settings CreateSettings(Compilation c, bool allowsExperimentalMsTest)
		=> new(
			References(c, "Microsoft.VisualStudio.TestPlatform.TestFramework") ||
			References(c, "MSTest.TestFramework"),
			References(c, "nunit.framework"),
			References(c, "TUnit.Core"),
			References(c, "TUnit.Assertions"),
			References(c, "xunit.assert"),
			References(c, "xunit.v3.core"),
			References(c, "xunit.v3.assert"),
			HasAttribute(c, "System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute"),
			HasAttribute(c, "System.Diagnostics.StackTraceHiddenAttribute"),
			SupportsAdapterRegistration(c),
			HasAttribute(c, "System.Runtime.CompilerServices.ModuleInitializerAttribute"),
			HasModuleInitializerLanguageVersion(c),
			CoreScansForAdapters(c),
			TestContext(c, allowsExperimentalMsTest));

	private static void Emit(SourceProductionContext context, Settings settings)
	{
		string attributes = string.Empty;
		if (settings.HasDoesNotReturn)
		{
			attributes += "[global::System.Diagnostics.CodeAnalysis.DoesNotReturn]\n\t\t";
		}

		if (settings.HasStackTraceHidden)
		{
			attributes += "[global::System.Diagnostics.StackTraceHidden]\n\t\t";
		}

		List<(string File, string Name, string Declaration)> adapters = [];
		TestContextSupport testContext = settings.TestContextSupport;

		if (settings.HasMsTest)
		{
			adapters.Add(("MsTest", "MsTestAdapter", MsTestAdapter(attributes,
				MsTestTestContext(testContext.IsDeclared, testContext.MsTest, testContext.AllowsExperimentalMsTest))));
		}

		if (settings.HasNunit)
		{
			adapters.Add(("Nunit", "NunitAdapter", NunitAdapter(attributes,
				NunitTestContext(testContext.IsDeclared, testContext.Nunit))));
		}

		if (settings.HasTUnit)
		{
			adapters.Add(("TUnit", "TUnitAdapter", TUnitAdapter(attributes, settings.HasTUnitAssertions,
				TUnitTestContext(testContext.IsDeclared, testContext.TUnit))));
		}

		if (settings.HasXunit2)
		{
			adapters.Add(("Xunit2", "Xunit2Adapter", Xunit2Adapter(attributes,
				TestContextMembers(testContext.IsDeclared, null, null, null))));
		}

		if (settings.HasXunit3Assert)
		{
			adapters.Add(("Xunit3", "Xunit3Adapter", Xunit3AssertAdapter(attributes,
				Xunit3TestContext(testContext.IsDeclared, testContext.Xunit3))));
		}
		else if (settings.HasXunit3Core)
		{
			adapters.Add(("Xunit3", "Xunit3Adapter", Xunit3CoreAdapter(attributes,
				Xunit3TestContext(testContext.IsDeclared, testContext.Xunit3))));
		}

		bool isRegistered = RegistersAdapters(context, settings, adapters.Select(x => x.Name));
		foreach ((string file, string name, string declaration) in adapters)
		{
			context.AddSource($"{file}.g.cs",
				isRegistered ? AdapterRegistration(name, declaration) : InFrameworksNamespace(declaration));
		}
	}

	/// <summary>
	///     Whether the <paramref name="adapters" /> register themselves, reporting each one that would have to but cannot.
	/// </summary>
	private static bool RegistersAdapters(SourceProductionContext context, Settings settings,
		IEnumerable<string> adapters)
	{
		// Without `ModuleInitializerAttribute` the target framework cannot be trimmed or AOT-published anyway,
		// and emitting a polyfill would collide with generators like PolySharp, which are invisible here.
		if (!settings.HasTestFrameworkRegistry || !settings.HasModuleInitializerAttribute)
		{
			return false;
		}

		if (!settings.CanCompileModuleInitializer)
		{
			if (!settings.IsAdapterFoundByScan)
			{
				foreach (string adapter in adapters)
				{
					context.ReportDiagnostic(Diagnostic.Create(CannotRegisterAdapter, Location.None, adapter));
				}
			}

			return false;
		}

		return true;
	}

	private static bool References(Compilation compilation, string assemblyName)
		=> compilation.ReferencedAssemblyNames.Any(x => x.Name == assemblyName);

	/// <remarks>
	///     Registering against an older aweXpect.Core would not compile, so anything but an exact match degrades to
	///     the assembly scan.
	/// </remarks>
	private static bool SupportsAdapterRegistration(Compilation compilation)
		=> compilation
			.GetTypeByMetadataName("aweXpect.Core.Adapters.TestFrameworkRegistry")
			?.GetMembers("Register")
			.OfType<IMethodSymbol>()
			.Any(x => x.IsStatic &&
			          x.DeclaredAccessibility == Accessibility.Public &&
			          x.Parameters.Length == 2 &&
			          x.Parameters[1].Type.SpecialType == SpecialType.System_Boolean) == true;

	/// <remarks>
	///     A consumer pinned below C# 9 could not compile the module initializer.
	/// </remarks>
	private static bool HasModuleInitializerLanguageVersion(Compilation compilation)
		=> compilation is CSharpCompilation { LanguageVersion: >= LanguageVersion.CSharp9, };

	/// <remarks>
	///     Only the builds of aweXpect.Core for .NET 8 or later no longer scan the loaded assemblies for an adapter. A
	///     consumer on .NET 5 to .NET 7 has a module initializer, but resolves the .NET Standard build.
	/// </remarks>
	private static bool CoreScansForAdapters(Compilation compilation)
	{
		const string netCoreApp = ".NETCoreApp,Version=v";
		string? targetFramework = compilation
			.GetTypeByMetadataName("aweXpect.Core.Adapters.TestFrameworkRegistry")?.ContainingAssembly
			.GetAttributes()
			.FirstOrDefault(x => x.AttributeClass?.ToDisplayString() ==
			                     "System.Runtime.Versioning.TargetFrameworkAttribute")?
			.ConstructorArguments.FirstOrDefault().Value as string;
		return targetFramework?.StartsWith(netCoreApp, StringComparison.Ordinal) != true ||
		       !Version.TryParse(targetFramework.Substring(netCoreApp.Length), out Version? version) ||
		       version.Major < 8;
	}

	/// <remarks>
	///     The adapter is nested in its registration, because a project that sees the internals of another project
	///     with its own generated adapter would otherwise resolve the name to both and warn with CS0436.
	/// </remarks>
	private static string AdapterRegistration(string adapterName, string adapterDeclaration) =>
		$$"""
		  namespace aweXpect.Frameworks
		  {
		  	internal static class {{adapterName}}Registration
		  	{
		  		/// <summary>
		  		///     Registers the <see cref="{{adapterName}}" /> when the assembly is loaded.
		  		/// </summary>
		  		/// <remarks>
		  		///     Without this registration, the adapter could only be found by scanning the loaded assemblies, which
		  		///     fails when the application is published with trimming or Native AOT enabled.<br />
		  		///     It does not overwrite an explicitly registered adapter.
		  		/// </remarks>
		  		[global::System.Runtime.CompilerServices.ModuleInitializer]
		  		internal static void Register()
		  			=> global::aweXpect.Core.Adapters.TestFrameworkRegistry.Register(new {{adapterName}}(), overwrite: false);

		  {{Indent(adapterDeclaration)}}
		  	}
		  }
		  """;

	private static string InFrameworksNamespace(string adapterDeclaration) =>
		$$"""
		  namespace aweXpect.Frameworks
		  {
		  {{adapterDeclaration}}
		  }
		  """;

	private static string Indent(string declaration)
		=> string.Join("\n", declaration.Split('\n')
			.Select(line => line.TrimEnd('\r').Length == 0 ? line : "\t" + line));

	/// <summary>
	///     Which test frameworks expose the running test, its cancellation token and attachments with the members that
	///     the generated adapter uses, when the referenced aweXpect.Core declares them.
	/// </summary>
	/// <remarks>
	///     Each framework is checked for the members themselves, as older versions lack them, e.g. MSTest before 4.2
	///     has no <c>TestContext.Current</c> and NUnit 3 no <c>CancellationToken</c>, so the adapter of such a version
	///     returns <see langword="false" /> instead. Every member and every named type that the generated code uses is
	///     checked, with the type of each property, so that a test framework that removes or changes one degrades the
	///     adapter instead of breaking the build of the consumer. A member or type that is marked as experimental, like
	///     <c>TestContext.Current</c> of MSTest 4, or as obsolete, also on a containing type or the assembly, counts as
	///     missing, as using it warns in the generated code, which fails the build of a consumer that treats warnings as
	///     errors. Only the experimental members of MSTest are used when the consumer sets the MSBuild property
	///     <c>AweXpectExperimentalMSTest</c>.
	/// </remarks>
	private static TestContextSupport TestContext(Compilation c, bool allowsExperimentalMsTest)
	{
		if (c.GetTypeByMetadataName("aweXpect.Core.Adapters.ITestFrameworkAdapter")?.GetMembers("TryGetCurrentTest")
			    .IsEmpty != false ||
		    c.GetTypeByMetadataName("aweXpect.Core.Adapters.RunningTest") is null)
		{
			return default;
		}

		return new TestContextSupport(true, allowsExperimentalMsTest, MsTestSupport(c, allowsExperimentalMsTest),
			NunitSupport(c), TUnitSupport(c), Xunit3Support(c));
	}

	/// <summary>
	///     The type with the <paramref name="typeName" />, when the generated code may name it.
	/// </summary>
	private static INamedTypeSymbol? GetUsableType(Compilation c, string typeName)
		=> c.GetTypeByMetadataName(typeName) is { } type && IsUsable(type) ? type : null;

	/// <summary>
	///     The type of the instance property, when the generated code may access it.
	/// </summary>
	private static ITypeSymbol? GetPropertyType(ITypeSymbol? type, string propertyName)
		=> GetMembers(type, propertyName).OfType<IPropertySymbol>().FirstOrDefault(x => !x.IsStatic && IsUsable(x))
			?.Type;

	/// <remarks>
	///     Each property has the given type, written without its nullable annotation, as the generated code assigns or
	///     passes it, or any type when it is <see langword="null" />, as the generated code only reads its members, which
	///     are checked themselves.
	/// </remarks>
	private static bool HasProperties(ITypeSymbol? type, bool isStatic,
		params (string Name, string? Type)[] properties)
		=> type is not null && properties.All(property => GetMembers(type, property.Name)
			.OfType<IPropertySymbol>().Any(x => x.IsStatic == isStatic && IsUsable(x) &&
			                                    (property.Type is null || property.Type ==
				                                    x.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))));

	/// <remarks>
	///     The <paramref name="parameterTypes" /> are compared without their nullable annotation, as an older version
	///     of the test framework may not annotate them.
	/// </remarks>
	private static bool HasMethod(ITypeSymbol? type, string methodName, bool isStatic,
		params string[] parameterTypes)
		=> GetMembers(type, methodName).OfType<IMethodSymbol>()
			.Any(x => x.IsStatic == isStatic && IsUsable(x) &&
			          x.Parameters.Select(p => p.Type.WithNullableAnnotation(NullableAnnotation.None).ToDisplayString())
				          .SequenceEqual(parameterTypes.Select(t => t.TrimEnd('?'))));

	/// <summary>
	///     The members with the <paramref name="name" />, including the inherited ones, as the generated code accesses
	///     them through the <paramref name="type" />.
	/// </summary>
	/// <remarks>
	///     A class is only searched with its base classes, as a member of an interface that it implements explicitly
	///     cannot be accessed through it, and one that it implements implicitly is a member of the class itself.
	/// </remarks>
	private static IEnumerable<ISymbol> GetMembers(ITypeSymbol? type, string name)
	{
		if (type is null)
		{
			yield break;
		}

		for (ITypeSymbol? current = type; current is not null; current = current.BaseType)
		{
			foreach (ISymbol member in current.GetMembers(name))
			{
				yield return member;
			}
		}

		if (type.TypeKind != TypeKind.Interface)
		{
			yield break;
		}

		foreach (INamedTypeSymbol @interface in type.AllInterfaces)
		{
			foreach (ISymbol member in @interface.GetMembers(name))
			{
				yield return member;
			}
		}
	}

	/// <remarks>
	///     A member is only usable when its containing types are, and the compiler also reports a type of an assembly
	///     that is marked as experimental. With <paramref name="allowsExperimental" />, only the diagnostic
	///     <c>MSTESTEXP</c> of MSTest is allowed, as the generated code suppresses only this one.
	/// </remarks>
	private static bool IsUsable(ISymbol symbol, bool allowsExperimental = false)
		=> symbol.DeclaredAccessibility == Accessibility.Public &&
		   !IsExperimentalOrObsolete(symbol, allowsExperimental) &&
		   (symbol.ContainingType is { } containingType
			   ? IsUsable(containingType, allowsExperimental)
			   : symbol.ContainingAssembly is null ||
			     !IsExperimentalOrObsolete(symbol.ContainingAssembly, allowsExperimental));

	/// <summary>
	///     The members of an adapter that read the running test, when the referenced aweXpect.Core declares them, each
	///     returning <see langword="false" /> without the implementation of its test framework.
	/// </summary>
	/// <remarks>
	///     The implementations catch every exception of the test framework and return <see langword="false" />, as the
	///     members are documented to never throw.
	/// </remarks>
	private static string TestContextMembers(bool isDeclared, string? tryGetCurrentTest,
		string? tryGetCancellationToken, string? tryAddAttachment)
	{
		if (!isDeclared)
		{
			return string.Empty;
		}

		return (tryGetCurrentTest ?? """


		                             		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.TryGetCurrentTest(out global::aweXpect.Core.Adapters.RunningTest)" />
		                             		public bool TryGetCurrentTest(out global::aweXpect.Core.Adapters.RunningTest currentTest)
		                             		{
		                             			currentTest = null;
		                             			return false;
		                             		}
		                             """) +
		       (tryGetCancellationToken ?? """


		                                   		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.TryGetCancellationToken(out global::System.Threading.CancellationToken)" />
		                                   		public bool TryGetCancellationToken(out global::System.Threading.CancellationToken cancellationToken)
		                                   		{
		                                   			cancellationToken = global::System.Threading.CancellationToken.None;
		                                   			return false;
		                                   		}
		                                   """) +
		       (tryAddAttachment ?? """


		                            		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.TryAddAttachment(string, string)" />
		                            		public bool TryAddAttachment(string filePath, string description)
		                            			=> false;
		                            """);
	}

	private static bool HasAttribute(Compilation c, string attributeName)
	{
		INamedTypeSymbol? attributeSymbol = c.GetTypeByMetadataName(attributeName);
		return attributeSymbol != null &&
		       (attributeSymbol.DeclaredAccessibility == Accessibility.Public ||
		        (attributeSymbol.DeclaredAccessibility == Accessibility.Internal &&
		         SymbolEqualityComparer.Default.Equals(attributeSymbol.ContainingAssembly, c.Assembly)));
	}

	private readonly record struct Settings(
		bool HasMsTest,
		bool HasNunit,
		bool HasTUnit,
		bool HasTUnitAssertions,
		bool HasXunit2,
		bool HasXunit3Core,
		bool HasXunit3Assert,
		bool HasDoesNotReturn,
		bool HasStackTraceHidden,
		bool HasTestFrameworkRegistry,
		bool HasModuleInitializerAttribute,
		bool CanCompileModuleInitializer,
		bool IsAdapterFoundByScan,
		TestContextSupport TestContextSupport);

	/// <summary>
	///     Whether aweXpect.Core declares the members of an adapter that read the running test, and what each test
	///     framework exposes, see <see cref="TestContext(Compilation, bool)" />.
	/// </summary>
	private readonly record struct TestContextSupport(
		bool IsDeclared,
		bool AllowsExperimentalMsTest,
		FrameworkSupport MsTest,
		FrameworkSupport Nunit,
		FrameworkSupport TUnit,
		FrameworkSupport Xunit3);

	/// <summary>
	///     Whether a test framework exposes the running test, the arguments of its test class, its cancellation token
	///     and attachments.
	/// </summary>
	private readonly record struct FrameworkSupport(
		bool CurrentTest,
		bool ClassArguments,
		bool CancellationToken,
		bool Attachment);
}
