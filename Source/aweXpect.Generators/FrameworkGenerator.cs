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
public class FrameworkGenerator : IIncrementalGenerator
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

		INamedTypeSymbol? msTest = GetUsableType(c, "Microsoft.VisualStudio.TestTools.UnitTesting.TestContext");
		INamedTypeSymbol? nunit = GetUsableType(c, "NUnit.Framework.TestContext");
		INamedTypeSymbol? nunitTest = GetUsableType(c, "NUnit.Framework.TestContext+TestAdapter");
		INamedTypeSymbol? nunitSuite = GetUsableType(c, "NUnit.Framework.Interfaces.ITest");
		INamedTypeSymbol? tunit = GetUsableType(c, "TUnit.Core.TestContext");
		INamedTypeSymbol? tunitDetails = GetUsableType(c, "TUnit.Core.TestDetails");
		INamedTypeSymbol? xunitContext = GetUsableType(c, "Xunit.ITestContext");
		INamedTypeSymbol? xunitTest = GetUsableType(c, "Xunit.v3.IXunitTest");
		ITypeSymbol? tunitExecution = GetPropertyType(tunit, "Execution");

		bool hasNunitTest = HasProperties(nunit, true, ("CurrentContext", "global::NUnit.Framework.TestContext")) &&
		                    HasProperties(nunit, false, ("Test", "global::NUnit.Framework.TestContext.TestAdapter")) &&
		                    HasProperties(nunitTest, false, ("MethodName", "string"));
		bool hasTUnitContext = HasProperties(tunit, true, ("Current", "global::TUnit.Core.TestContext"));
		bool hasMsTestContext = GetMembers(msTest, "Current").OfType<IPropertySymbol>().Any(x =>
			x.IsStatic && IsUsable(x, allowsExperimentalMsTest) &&
			x.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ==
			"global::Microsoft.VisualStudio.TestTools.UnitTesting.TestContext");
		bool hasXunit3Test =
			HasProperties(GetUsableType(c, "Xunit.TestContext"), true, ("Current", "global::Xunit.ITestContext")) &&
			HasProperties(xunitContext, false, ("Test", null));
		return new TestContextSupport(true, allowsExperimentalMsTest,
			new FrameworkSupport(
				hasMsTestContext &&
				HasProperties(msTest, false, ("FullyQualifiedTestClassName", "string"), ("TestName", "string"),
					("TestData", "object[]")),
				false,
				hasMsTestContext &&
				HasProperties(msTest, false, ("CancellationToken", "global::System.Threading.CancellationToken")),
				hasMsTestContext && HasMethod(msTest, "AddResultFile", false, "string")),
			new FrameworkSupport(
				hasNunitTest &&
				HasProperties(nunitTest, false, ("ClassName", "string"), ("Arguments", "object[]"), ("ID", "string"),
					("Name", "string")) &&
				HasProperties(nunit, false, ("CurrentRepeatCount", "int")),
				hasNunitTest &&
				HasProperties(nunitTest, false, ("Parent", "global::NUnit.Framework.Interfaces.ITest")) &&
				HasProperties(nunitSuite, false, ("Parent", "global::NUnit.Framework.Interfaces.ITest"),
					("Method", null), ("Arguments", "object[]")),
				hasNunitTest &&
				HasProperties(nunit, false, ("CancellationToken", "global::System.Threading.CancellationToken")),
				hasNunitTest && HasMethod(nunit, "AddTestAttachment", true, "string", "string")),
			new FrameworkSupport(
				hasTUnitContext &&
				HasProperties(GetPropertyType(tunit, "Metadata"), false, ("TestDetails", "global::TUnit.Core.TestDetails"),
					("DisplayName", "string")) &&
				HasProperties(tunitDetails, false, ("ClassType", "global::System.Type"), ("MethodName", "string"),
					("TestMethodArguments", "object[]"), ("TestId", "string")) &&
				HasProperties(tunitExecution, false, ("CurrentRetryAttempt", "int")),
				HasProperties(tunitDetails, false, ("TestClassArguments", "object[]")),
				hasTUnitContext &&
				HasProperties(tunitExecution, false, ("CancellationToken", "global::System.Threading.CancellationToken")),
				hasTUnitContext &&
				HasMethod(GetPropertyType(tunit, "Output"), "AttachArtifact", false, "string", "string?", "string?")),
			new FrameworkSupport(
				hasXunit3Test &&
				HasProperties(xunitTest, false, ("TestMethodArguments", "object[]"), ("UniqueID", "string"),
					("TestDisplayName", "string")) &&
				HasProperties(GetPropertyType(xunitTest, "TestCase"), false, ("TestClassName", "string"),
					("TestMethodName", "string")),
				false,
				hasXunit3Test &&
				HasProperties(xunitContext, false, ("CancellationToken", "global::System.Threading.CancellationToken")),
				hasXunit3Test && HasMethod(xunitContext, "AddAttachment", false, "string", "byte[]", "string")));
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
	///     that is marked as experimental.
	/// </remarks>
	private static bool IsUsable(ISymbol symbol, bool allowsExperimental = false)
		=> symbol.DeclaredAccessibility == Accessibility.Public &&
		   !IsExperimentalOrObsolete(symbol, allowsExperimental) &&
		   (symbol.ContainingType is { } containingType
			   ? IsUsable(containingType, allowsExperimental)
			   : symbol.ContainingAssembly is null ||
			     !IsExperimentalOrObsolete(symbol.ContainingAssembly, allowsExperimental));

	private static bool IsExperimentalOrObsolete(ISymbol symbol, bool allowsExperimental)
		=> symbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() switch
		{
			"System.Diagnostics.CodeAnalysis.ExperimentalAttribute" => !allowsExperimental,
			"System.ObsoleteAttribute" => true,
			_ => false,
		});

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

	/// <remarks>
	///     MSTest marks <c>TestContext.Current</c> as experimental, so the members only use it when the consumer allows
	///     it with the MSBuild property <c>AweXpectExperimentalMSTest</c>, which suppresses its diagnostic.
	/// </remarks>
	private static string MsTestTestContext(bool isDeclared, FrameworkSupport support, bool allowsExperimental)
	{
		string members = TestContextMembers(isDeclared, support.CurrentTest
				? """


				  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.TryGetCurrentTest(out global::aweXpect.Core.Adapters.RunningTest)" />
				  		public bool TryGetCurrentTest(out global::aweXpect.Core.Adapters.RunningTest currentTest)
				  		{
				  			try
				  			{
				  				global::Microsoft.VisualStudio.TestTools.UnitTesting.TestContext context = global::Microsoft.VisualStudio.TestTools.UnitTesting.TestContext.Current;
				  				if (context == null || context.FullyQualifiedTestClassName == null || context.TestName == null)
				  				{
				  					currentTest = null;
				  					return false;
				  				}

				  				currentTest = new global::aweXpect.Core.Adapters.RunningTest(new global::aweXpect.Core.Adapters.TestIdentity(context.FullyQualifiedTestClassName, new object[0], context.TestName, context.TestData ?? new object[0]), null, null, 0);
				  				return true;
				  			}
				  			catch (global::System.Exception)
				  			{
				  				currentTest = null;
				  				return false;
				  			}
				  		}
				  """
				: null,
			support.CancellationToken
				? """


				  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.TryGetCancellationToken(out global::System.Threading.CancellationToken)" />
				  		public bool TryGetCancellationToken(out global::System.Threading.CancellationToken cancellationToken)
				  		{
				  			try
				  			{
				  				global::Microsoft.VisualStudio.TestTools.UnitTesting.TestContext context = global::Microsoft.VisualStudio.TestTools.UnitTesting.TestContext.Current;
				  				if (context == null)
				  				{
				  					cancellationToken = global::System.Threading.CancellationToken.None;
				  					return false;
				  				}

				  				cancellationToken = context.CancellationToken;
				  				return true;
				  			}
				  			catch (global::System.Exception)
				  			{
				  				cancellationToken = global::System.Threading.CancellationToken.None;
				  				return false;
				  			}
				  		}
				  """
				: null,
			support.Attachment
				? """


				  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.TryAddAttachment(string, string)" />
				  		public bool TryAddAttachment(string filePath, string description)
				  		{
				  			try
				  			{
				  				global::Microsoft.VisualStudio.TestTools.UnitTesting.TestContext context = global::Microsoft.VisualStudio.TestTools.UnitTesting.TestContext.Current;
				  				if (context == null || !global::System.IO.File.Exists(filePath))
				  				{
				  					return false;
				  				}

				  				context.AddResultFile(filePath);
				  				return true;
				  			}
				  			catch (global::System.Exception)
				  			{
				  				return false;
				  			}
				  		}
				  """
				: null);
		return allowsExperimental && members.Length > 0
			? "\n#pragma warning disable MSTESTEXP" + members + "\n#pragma warning restore MSTESTEXP"
			: members;
	}

	/// <remarks>
	///     Outside of a test, e.g. in a one-time setup, the current context of NUnit describes the fixture, which has no
	///     method.
	/// </remarks>
	private static string NunitTestContext(bool isDeclared, FrameworkSupport support)
		=> TestContextMembers(isDeclared, support.CurrentTest
				? $$"""


				  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.TryGetCurrentTest(out global::aweXpect.Core.Adapters.RunningTest)" />
				  		public bool TryGetCurrentTest(out global::aweXpect.Core.Adapters.RunningTest currentTest)
				  		{
				  			try
				  			{
				  				global::NUnit.Framework.TestContext context = global::NUnit.Framework.TestContext.CurrentContext;
				  				global::NUnit.Framework.TestContext.TestAdapter test = context == null ? null : context.Test;
				  				if (test == null || test.ClassName == null || test.MethodName == null)
				  				{
				  					currentTest = null;
				  					return false;
				  				}

				  {{(support.ClassArguments ? NunitFixture : "")}}
				  				currentTest = new global::aweXpect.Core.Adapters.RunningTest(new global::aweXpect.Core.Adapters.TestIdentity(test.ClassName, {{(support.ClassArguments ? "fixture == null || fixture.Arguments == null ? new object[0] : fixture.Arguments" : "new object[0]")}}, test.MethodName, test.Arguments ?? new object[0]), test.ID, test.Name, context.CurrentRepeatCount);
				  				return true;
				  			}
				  			catch (global::System.Exception)
				  			{
				  				currentTest = null;
				  				return false;
				  			}
				  		}
				  """
				: null,
			support.CancellationToken
				? """


				  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.TryGetCancellationToken(out global::System.Threading.CancellationToken)" />
				  		public bool TryGetCancellationToken(out global::System.Threading.CancellationToken cancellationToken)
				  		{
				  			try
				  			{
				  				global::NUnit.Framework.TestContext context = global::NUnit.Framework.TestContext.CurrentContext;
				  				if (context == null || context.Test == null || context.Test.MethodName == null)
				  				{
				  					cancellationToken = global::System.Threading.CancellationToken.None;
				  					return false;
				  				}

				  				cancellationToken = context.CancellationToken;
				  				return true;
				  			}
				  			catch (global::System.Exception)
				  			{
				  				cancellationToken = global::System.Threading.CancellationToken.None;
				  				return false;
				  			}
				  		}
				  """
				: null,
			support.Attachment
				? """


				  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.TryAddAttachment(string, string)" />
				  		public bool TryAddAttachment(string filePath, string description)
				  		{
				  			try
				  			{
				  				global::NUnit.Framework.TestContext context = global::NUnit.Framework.TestContext.CurrentContext;
				  				if (context == null || context.Test == null || context.Test.MethodName == null)
				  				{
				  					return false;
				  				}

				  				global::NUnit.Framework.TestContext.AddTestAttachment(filePath, description);
				  				return true;
				  			}
				  			catch (global::System.Exception)
				  			{
				  				return false;
				  			}
				  		}
				  """
				: null);

	/// <summary>
	///     Finds the fixture of the test, whose arguments are those of the test class, above the suite of a
	///     parameterized test method, which has a method, but no method name.
	/// </summary>
	private const string NunitFixture = """
	                                    				global::NUnit.Framework.Interfaces.ITest fixture = test.Parent;
	                                    				while (fixture != null && fixture.Method != null)
	                                    				{
	                                    					fixture = fixture.Parent;
	                                    				}

	                                    """;

	/// <remarks>
	///     The id is the one of the test, not of the <c>TestContext</c>, so that it stays the same when the test is
	///     retried. TUnit attaches a missing file without an error, so the adapter checks that it exists.
	/// </remarks>
	private static string TUnitTestContext(bool isDeclared, FrameworkSupport support)
		=> TestContextMembers(isDeclared, support.CurrentTest
				? $$"""


				  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.TryGetCurrentTest(out global::aweXpect.Core.Adapters.RunningTest)" />
				  		public bool TryGetCurrentTest(out global::aweXpect.Core.Adapters.RunningTest currentTest)
				  		{
				  			try
				  			{
				  				global::TUnit.Core.TestContext context = global::TUnit.Core.TestContext.Current;
				  				global::TUnit.Core.TestDetails details = context == null || context.Metadata == null ? null : context.Metadata.TestDetails;
				  				if (details == null || details.ClassType == null || details.MethodName == null)
				  				{
				  					currentTest = null;
				  					return false;
				  				}

				  				currentTest = new global::aweXpect.Core.Adapters.RunningTest(new global::aweXpect.Core.Adapters.TestIdentity(details.ClassType.FullName ?? details.ClassType.Name, {{(support.ClassArguments ? "details.TestClassArguments ?? new object[0]" : "new object[0]")}}, details.MethodName, details.TestMethodArguments ?? new object[0]), details.TestId, context.Metadata.DisplayName, context.Execution == null ? 0 : context.Execution.CurrentRetryAttempt);
				  				return true;
				  			}
				  			catch (global::System.Exception)
				  			{
				  				currentTest = null;
				  				return false;
				  			}
				  		}
				  """
				: null,
			support.CancellationToken
				? """


				  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.TryGetCancellationToken(out global::System.Threading.CancellationToken)" />
				  		public bool TryGetCancellationToken(out global::System.Threading.CancellationToken cancellationToken)
				  		{
				  			try
				  			{
				  				global::TUnit.Core.TestContext context = global::TUnit.Core.TestContext.Current;
				  				if (context == null || context.Execution == null)
				  				{
				  					cancellationToken = global::System.Threading.CancellationToken.None;
				  					return false;
				  				}

				  				cancellationToken = context.Execution.CancellationToken;
				  				return true;
				  			}
				  			catch (global::System.Exception)
				  			{
				  				cancellationToken = global::System.Threading.CancellationToken.None;
				  				return false;
				  			}
				  		}
				  """
				: null,
			support.Attachment
				? """


				  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.TryAddAttachment(string, string)" />
				  		public bool TryAddAttachment(string filePath, string description)
				  		{
				  			try
				  			{
				  				global::TUnit.Core.TestContext context = global::TUnit.Core.TestContext.Current;
				  				if (context == null || context.Output == null || !global::System.IO.File.Exists(filePath))
				  				{
				  					return false;
				  				}

				  				context.Output.AttachArtifact(filePath, null, description);
				  				return true;
				  			}
				  			catch (global::System.Exception)
				  			{
				  				return false;
				  			}
				  		}
				  """
				: null);

	/// <remarks>
	///     The arguments are those of the test, as a theory whose data cannot be enumerated up front runs all its rows in
	///     one test case. xUnit v3 has no retries and attaches the content instead of the file, so the file is read
	///     immediately, under its name.
	/// </remarks>
	private static string Xunit3TestContext(bool isDeclared, FrameworkSupport support)
		=> TestContextMembers(isDeclared, support.CurrentTest
				? """


				  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.TryGetCurrentTest(out global::aweXpect.Core.Adapters.RunningTest)" />
				  		public bool TryGetCurrentTest(out global::aweXpect.Core.Adapters.RunningTest currentTest)
				  		{
				  			try
				  			{
				  				global::Xunit.ITestContext context = global::Xunit.TestContext.Current;
				  				global::Xunit.v3.IXunitTest test = context == null ? null : context.Test as global::Xunit.v3.IXunitTest;
				  				if (test == null || test.TestCase == null || test.TestCase.TestClassName == null || test.TestCase.TestMethodName == null)
				  				{
				  					currentTest = null;
				  					return false;
				  				}

				  				currentTest = new global::aweXpect.Core.Adapters.RunningTest(new global::aweXpect.Core.Adapters.TestIdentity(test.TestCase.TestClassName, new object[0], test.TestCase.TestMethodName, test.TestMethodArguments ?? new object[0]), test.UniqueID, test.TestDisplayName, 0);
				  				return true;
				  			}
				  			catch (global::System.Exception)
				  			{
				  				currentTest = null;
				  				return false;
				  			}
				  		}
				  """
				: null,
			support.CancellationToken
				? """


				  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.TryGetCancellationToken(out global::System.Threading.CancellationToken)" />
				  		public bool TryGetCancellationToken(out global::System.Threading.CancellationToken cancellationToken)
				  		{
				  			try
				  			{
				  				global::Xunit.ITestContext context = global::Xunit.TestContext.Current;
				  				if (context == null || context.Test == null)
				  				{
				  					cancellationToken = global::System.Threading.CancellationToken.None;
				  					return false;
				  				}

				  				cancellationToken = context.CancellationToken;
				  				return true;
				  			}
				  			catch (global::System.Exception)
				  			{
				  				cancellationToken = global::System.Threading.CancellationToken.None;
				  				return false;
				  			}
				  		}
				  """
				: null,
			support.Attachment
				? """


				  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.TryAddAttachment(string, string)" />
				  		public bool TryAddAttachment(string filePath, string description)
				  		{
				  			try
				  			{
				  				global::Xunit.ITestContext context = global::Xunit.TestContext.Current;
				  				if (context == null || context.Test == null)
				  				{
				  					return false;
				  				}

				  				context.AddAttachment(global::System.IO.Path.GetFileName(filePath), global::System.IO.File.ReadAllBytes(filePath), "application/octet-stream");
				  				return true;
				  			}
				  			catch (global::System.Exception)
				  			{
				  				return false;
				  			}
				  		}
				  """
				: null);

	private static string MsTestAdapter(string attributes, string testContextMembers) =>
		$$"""
		  	internal class MsTestAdapter : global::aweXpect.Core.Adapters.ITestFrameworkAdapter
		  	{
		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.IsAvailable" />
		  		public bool IsAvailable { get; } = true;

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Skip(string)" />
		  		{{attributes}}public void Skip(string message)
		  			=> throw new global::Microsoft.VisualStudio.TestTools.UnitTesting.AssertInconclusiveException(message);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string)" />
		  		{{attributes}}public void Fail(string message)
		  			=> throw new global::Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException(message);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string, global::System.Exception)" />
		  		{{attributes}}public void Fail(string message, global::System.Exception innerException)
		  			=> throw new global::Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException(message, innerException);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Inconclusive(string)" />
		  		{{attributes}}public void Inconclusive(string message)
		  			=> throw new global::Microsoft.VisualStudio.TestTools.UnitTesting.AssertInconclusiveException(message);{{testContextMembers}}
		  	}
		  """;

	private static string NunitAdapter(string attributes, string testContextMembers) =>
		$$"""
		  	internal class NunitAdapter : global::aweXpect.Core.Adapters.ITestFrameworkAdapter
		  	{
		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.IsAvailable" />
		  		public bool IsAvailable { get; } = true;

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Skip(string)" />
		  		{{attributes}}public void Skip(string message)
		  			=> throw new global::NUnit.Framework.IgnoreException(message);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string)" />
		  		{{attributes}}public void Fail(string message)
		  			=> throw new global::NUnit.Framework.AssertionException(message);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string, global::System.Exception)" />
		  		{{attributes}}public void Fail(string message, global::System.Exception innerException)
		  			=> throw new global::NUnit.Framework.AssertionException(message, innerException);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Inconclusive(string)" />
		  		{{attributes}}public void Inconclusive(string message)
		  			=> throw new global::NUnit.Framework.InconclusiveException(message);{{testContextMembers}}
		  	}
		  """;

	/// <remarks>
	///     A project that uses aweXpect instead of TUnit's assertions references only TUnit.Core, which is enough to skip
	///     a test, so a failure then throws the exception of aweXpect.
	/// </remarks>
	private static string TUnitAdapter(string attributes, bool hasAssertions, string testContextMembers)
	{
		string failException = hasAssertions
			? "global::TUnit.Assertions.Exceptions.AssertionException"
			: "global::aweXpect.FailException";
		return $$"""
		         	internal class TUnitAdapter : global::aweXpect.Core.Adapters.ITestFrameworkAdapter
		         	{
		         		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.IsAvailable" />
		         		public bool IsAvailable { get; } = true;

		         		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Skip(string)" />
		         		{{attributes}}public void Skip(string message)
		         			=> throw new global::TUnit.Core.Exceptions.SkipTestException(message);

		         		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string)" />
		         		{{attributes}}public void Fail(string message)
		         			=> throw new {{failException}}(message);

		         		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string, global::System.Exception)" />
		         		{{attributes}}public void Fail(string message, global::System.Exception innerException)
		         			=> throw new {{failException}}(message, innerException);

		         		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Inconclusive(string)" />
		         		{{attributes}}public void Inconclusive(string message)
		         			=> throw new global::TUnit.Core.Exceptions.InconclusiveTestException(message, null);{{testContextMembers}}
		         	}
		         """;
	}

	private static string Xunit2Adapter(string attributes, string testContextMembers) =>
		$$"""
		  	internal class Xunit2Adapter : global::aweXpect.Core.Adapters.ITestFrameworkAdapter
		  	{
		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.IsAvailable" />
		  		public bool IsAvailable { get; } = true;

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Skip(string)" />
		  		{{attributes}}public void Skip(string message)
		  			=> throw new global::aweXpect.SkipException($"SKIPPED: {message} (xunit v2 does not support skipping test)");

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string)" />
		  		{{attributes}}public void Fail(string message)
		  			=> throw new global::Xunit.Sdk.XunitException(message);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string, global::System.Exception)" />
		  		{{attributes}}public void Fail(string message, global::System.Exception innerException)
		  			=> throw new global::Xunit.Sdk.XunitException(message, innerException);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Inconclusive(string)" />
		  		{{attributes}}public void Inconclusive(string message)
		  			=> throw new global::aweXpect.InconclusiveException(message);{{testContextMembers}}
		  	}
		  """;

	private static string Xunit3CoreAdapter(string attributes, string testContextMembers) =>
		$$"""
		  	internal class Xunit3Adapter : global::aweXpect.Core.Adapters.ITestFrameworkAdapter
		  	{
		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.IsAvailable" />
		  		public bool IsAvailable { get; } = true;

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Skip(string)" />
		  		{{attributes}}public void Skip(string message)
		  			=> throw new global::aweXpect.SkipException($"$XunitDynamicSkip${message}");

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string)" />
		  		{{attributes}}public void Fail(string message)
		  			=> throw new XunitException(message);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string, global::System.Exception)" />
		  		{{attributes}}public void Fail(string message, global::System.Exception innerException)
		  			=> throw new XunitException(message, innerException);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Inconclusive(string)" />
		  		{{attributes}}public void Inconclusive(string message)
		  			=> throw new XunitTimeoutException(message);{{testContextMembers}}

		  		/// <summary>
		  		///     Interface is required by xunit v3 to identify an assertion exception.
		  		/// </summary>
		  		private interface IAssertionException { }

		  #pragma warning disable S3871 // Exception types should be "public"
		  		private sealed class XunitException : global::System.Exception, IAssertionException
		  		{
		  			public XunitException(string message) : base(message) { }

		  			public XunitException(string message, global::System.Exception innerException) : base(message, innerException) { }
		  		}
		  #pragma warning restore S3871 // Exception types should be "public"

		  #pragma warning disable S3871 // Exception types should be "public"
		  		private sealed class XunitTimeoutException : global::aweXpect.InconclusiveException, ITestTimeoutException
		  		{
		  			public XunitTimeoutException(string message) : base(message) { }
		  		}
		  #pragma warning restore S3871 // Exception types should be "public"

		  		private interface ITestTimeoutException { }
		  	}
		  """;

	private static string Xunit3AssertAdapter(string attributes, string testContextMembers) =>
		$$"""
		  	internal class Xunit3Adapter : global::aweXpect.Core.Adapters.ITestFrameworkAdapter
		  	{
		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.IsAvailable" />
		  		public bool IsAvailable { get; } = true;

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Skip(string)" />
		  		{{attributes}}public void Skip(string message)
		  			=> throw new global::aweXpect.SkipException($"$XunitDynamicSkip${message}");

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string)" />
		  		{{attributes}}public void Fail(string message)
		  			=> throw new global::Xunit.Sdk.XunitException(message);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string, global::System.Exception)" />
		  		{{attributes}}public void Fail(string message, global::System.Exception innerException)
		  			=> throw new global::Xunit.Sdk.XunitException(message, innerException);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Inconclusive(string)" />
		  		{{attributes}}public void Inconclusive(string message)
		  			=> throw new XunitTimeoutException(message);{{testContextMembers}}

		  #pragma warning disable S3871 // Exception types should be "public"
		  		private sealed class XunitTimeoutException : global::aweXpect.InconclusiveException, ITestTimeoutException
		  		{
		  			public XunitTimeoutException(string message) : base(message) { }
		  		}
		  #pragma warning restore S3871 // Exception types should be "public"

		  		private interface ITestTimeoutException { }
		  	}
		  """;

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
