using Microsoft.CodeAnalysis;

namespace aweXpect.Frameworks;

/// <summary>
///     The <see cref="IIncrementalGenerator" /> for generating test framework adapters.
/// </summary>
[Generator]
public class FrameworkGenerator : IIncrementalGenerator
{
	void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValueProvider<Settings> settings = context.CompilationProvider
			.Select((c, _) => new Settings(
				HasMsTest: References(c, "Microsoft.VisualStudio.TestPlatform.TestFramework") ||
				           References(c, "MSTest.TestFramework"),
				HasNunit: References(c, "nunit.framework"),
				HasTUnit: References(c, "TUnit.Core") && References(c, "TUnit.Assertions"),
				HasXunit2: References(c, "xunit.assert"),
				HasXunit3Core: References(c, "xunit.v3.core"),
				HasXunit3Assert: References(c, "xunit.v3.assert"),
				HasDoesNotReturn: HasAttribute(c, "System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute"),
				HasStackTraceHidden: HasAttribute(c, "System.Diagnostics.StackTraceHiddenAttribute"),
				HasTestFrameworkRegistry: SupportsAdapterRegistration(c),
				HasModuleInitializer: HasAttribute(c, "System.Runtime.CompilerServices.ModuleInitializerAttribute")));

		context.RegisterSourceOutput(settings, Emit);
	}

	private static void Emit(SourceProductionContext context, Settings settings)
	{
		string attributes = string.Empty;
		if (settings.HasDoesNotReturn)
		{
			attributes += "[System.Diagnostics.CodeAnalysis.DoesNotReturn]\n\t";
		}

		if (settings.HasStackTraceHidden)
		{
			attributes += "[System.Diagnostics.StackTraceHidden]\n\t";
		}

		List<string> registeredAdapters = [];

		if (settings.HasMsTest)
		{
			context.AddSource("MsTest.g.cs", MsTestAdapter(attributes));
			registeredAdapters.Add("MsTestAdapter");
		}

		if (settings.HasNunit)
		{
			context.AddSource("Nunit.g.cs", NunitAdapter(attributes));
			registeredAdapters.Add("NunitAdapter");
		}

		if (settings.HasTUnit)
		{
			context.AddSource("TUnit.g.cs", TUnitAdapter(attributes));
			registeredAdapters.Add("TUnitAdapter");
		}

		if (settings.HasXunit2)
		{
			context.AddSource("Xunit2.g.cs", Xunit2Adapter(attributes));
			registeredAdapters.Add("Xunit2Adapter");
		}

		if (settings.HasXunit3Assert)
		{
			context.AddSource("Xunit3.g.cs", Xunit3AssertAdapter(attributes));
			registeredAdapters.Add("Xunit3Adapter");
		}
		else if (settings.HasXunit3Core)
		{
			context.AddSource("Xunit3.g.cs", Xunit3CoreAdapter(attributes));
			registeredAdapters.Add("Xunit3Adapter");
		}

		// Without `ModuleInitializerAttribute` the target framework cannot be trimmed or AOT-published anyway,
		// and emitting a polyfill would collide with generators like PolySharp, which are invisible here.
		if (!settings.HasTestFrameworkRegistry || !settings.HasModuleInitializer || registeredAdapters.Count == 0)
		{
			return;
		}

		foreach (string adapter in registeredAdapters)
		{
			context.AddSource($"{adapter}.Registration.g.cs", AdapterRegistration(adapter));
		}
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

	private readonly record struct Settings(
		bool HasMsTest,
		bool HasNunit,
		bool HasTUnit,
		bool HasXunit2,
		bool HasXunit3Core,
		bool HasXunit3Assert,
		bool HasDoesNotReturn,
		bool HasStackTraceHidden,
		bool HasTestFrameworkRegistry,
		bool HasModuleInitializer);

	private static string AdapterRegistration(string adapterName) =>
		$$"""
		  namespace aweXpect.Frameworks;

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
		  	[System.Runtime.CompilerServices.ModuleInitializer]
		  	internal static void Register()
		  		=> aweXpect.Core.Adapters.TestFrameworkRegistry.Register(new {{adapterName}}(), overwrite: false);
		  }
		  """;

	private static string MsTestAdapter(string attributes) =>
		$$"""
		  using aweXpect.Core.Adapters;

		  namespace aweXpect.Frameworks;

		  internal class MsTestAdapter() : ITestFrameworkAdapter
		  {
		  	/// <inheritdoc cref="ITestFrameworkAdapter.IsAvailable" />
		  	public bool IsAvailable { get; } = true;

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Skip(string)" />
		  	{{attributes}}public void Skip(string message)
		  		=> throw new Microsoft.VisualStudio.TestTools.UnitTesting.AssertInconclusiveException(message);

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Fail(string)" />
		  	{{attributes}}public void Fail(string message)
		  		=> throw new Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException(message);

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Fail(string, System.Exception)" />
		  	{{attributes}}public void Fail(string message, System.Exception innerException)
		  		=> throw new Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException(message, innerException);

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Inconclusive(string)" />
		  	{{attributes}}public void Inconclusive(string message)
		  		=> throw new Microsoft.VisualStudio.TestTools.UnitTesting.AssertInconclusiveException(message);
		  }
		  """;

	private static string NunitAdapter(string attributes) =>
		$$"""
		  using aweXpect.Core.Adapters;

		  namespace aweXpect.Frameworks;

		  internal class NunitAdapter() : ITestFrameworkAdapter
		  {
		  	/// <inheritdoc cref="ITestFrameworkAdapter.IsAvailable" />
		  	public bool IsAvailable { get; } = true;

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Skip(string)" />
		  	{{attributes}}public void Skip(string message)
		  		=> throw new NUnit.Framework.IgnoreException(message);

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Fail(string)" />
		  	{{attributes}}public void Fail(string message)
		  		=> throw new NUnit.Framework.AssertionException(message);

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Fail(string, System.Exception)" />
		  	{{attributes}}public void Fail(string message, System.Exception innerException)
		  		=> throw new NUnit.Framework.AssertionException(message, innerException);

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Inconclusive(string)" />
		  	{{attributes}}public void Inconclusive(string message)
		  		=> throw new NUnit.Framework.InconclusiveException(message);
		  }
		  """;

	private static string TUnitAdapter(string attributes) =>
		$$"""
		  using aweXpect.Core.Adapters;

		  namespace aweXpect.Frameworks;

		  internal class TUnitAdapter() : ITestFrameworkAdapter
		  {
		  	/// <inheritdoc cref="ITestFrameworkAdapter.IsAvailable" />
		  	public bool IsAvailable { get; } = true;

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Skip(string)" />
		  	{{attributes}}public void Skip(string message)
		  		=> throw new TUnit.Core.Exceptions.SkipTestException(message);

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Fail(string)" />
		  	{{attributes}}public void Fail(string message)
		  		=> throw new TUnit.Assertions.Exceptions.AssertionException(message);

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Fail(string, System.Exception)" />
		  	{{attributes}}public void Fail(string message, System.Exception innerException)
		  		=> throw new TUnit.Assertions.Exceptions.AssertionException(message, innerException);

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Inconclusive(string)" />
		  	{{attributes}}public void Inconclusive(string message)
		  		=> throw new TUnit.Core.Exceptions.InconclusiveTestException(message, null);
		  }
		  """;

	private static string Xunit2Adapter(string attributes) =>
		$$"""
		  using aweXpect.Core.Adapters;

		  namespace aweXpect.Frameworks;

		  internal class Xunit2Adapter() : ITestFrameworkAdapter
		  {
		  	/// <inheritdoc cref="ITestFrameworkAdapter.IsAvailable" />
		  	public bool IsAvailable { get; } = true;

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Skip(string)" />
		  	{{attributes}}public void Skip(string message)
		  		=> throw new SkipException($"SKIPPED: {message} (xunit v2 does not support skipping test)");

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Fail(string)" />
		  	{{attributes}}public void Fail(string message)
		  		=> throw new Xunit.Sdk.XunitException(message);

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Fail(string, System.Exception)" />
		  	{{attributes}}public void Fail(string message, System.Exception innerException)
		  		=> throw new Xunit.Sdk.XunitException(message, innerException);

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Inconclusive(string)" />
		  	{{attributes}}public void Inconclusive(string message)
		  		=> throw new InconclusiveException(message);
		  }
		  """;

	private static string Xunit3CoreAdapter(string attributes) =>
		$$"""
		  using System;
		  using aweXpect;
		  using aweXpect.Core.Adapters;

		  namespace aweXpect.Frameworks;

		  internal class Xunit3Adapter() : ITestFrameworkAdapter
		  {
		  	/// <inheritdoc cref="ITestFrameworkAdapter.IsAvailable" />
		  	public bool IsAvailable { get; } = true;

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Skip(string)" />
		  	{{attributes}}public void Skip(string message)
		  		=> throw new SkipException($"$XunitDynamicSkip${message}");

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Fail(string)" />
		  	{{attributes}}public void Fail(string message)
		  		=> throw new XunitException(message);

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Fail(string, System.Exception)" />
		  	{{attributes}}public void Fail(string message, System.Exception innerException)
		  		=> throw new XunitException(message, innerException);

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Inconclusive(string)" />
		  	{{attributes}}public void Inconclusive(string message)
		  		=> throw new XunitTimeoutException(message);
		  		
		  	/// <summary>
		  	///     Interface is required by xunit v3 to identify an assertion exception.
		  	/// </summary>
		  	private interface IAssertionException;

		  #pragma warning disable S3871 // Exception types should be "public"
		  	private sealed class XunitException : Exception, IAssertionException
		  	{
		  		public XunitException(string message) : base(message) { }

		  		public XunitException(string message, Exception innerException) : base(message, innerException) { }
		  	}
		  #pragma warning restore S3871 // Exception types should be "public"

		  #pragma warning disable S3871 // Exception types should be "public"
		  	private sealed class XunitTimeoutException(string message)
		  		: InconclusiveException(message), ITestTimeoutException;
		  #pragma warning restore S3871 // Exception types should be "public"
		  	private interface ITestTimeoutException;
		  }
		  """;

	private static string Xunit3AssertAdapter(string attributes) =>
		$$"""
		  using System;
		  using aweXpect;
		  using aweXpect.Core.Adapters;

		  namespace aweXpect.Frameworks;

		  internal class Xunit3Adapter() : ITestFrameworkAdapter
		  {
		  	/// <inheritdoc cref="ITestFrameworkAdapter.IsAvailable" />
		  	public bool IsAvailable { get; } = true;

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Skip(string)" />
		  	{{attributes}}public void Skip(string message)
		  		=> throw new SkipException($"$XunitDynamicSkip${message}");

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Fail(string)" />
		  	{{attributes}}public void Fail(string message)
		  		=> throw new Xunit.Sdk.XunitException(message);

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Fail(string, System.Exception)" />
		  	{{attributes}}public void Fail(string message, System.Exception innerException)
		  		=> throw new Xunit.Sdk.XunitException(message, innerException);

		  	/// <inheritdoc cref="ITestFrameworkAdapter.Inconclusive(string)" />
		  	{{attributes}}public void Inconclusive(string message)
		  		=> throw new XunitTimeoutException(message);
		  		
		  #pragma warning disable S3871 // Exception types should be "public"
		  	private sealed class XunitTimeoutException(string message)
		  		: InconclusiveException(message), ITestTimeoutException;
		  #pragma warning restore S3871 // Exception types should be "public"
		  	private interface ITestTimeoutException;
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
}
