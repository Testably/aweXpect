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
		IncrementalValueProvider<(
			bool hasMsTest3,
			bool hasMsTest4,
			bool hasNunit,
			bool hasTUnit,
			bool hasXunit2,
			bool hasXunit3Core,
			bool hasXunit3Assert,
			bool hasDoesNotReturn,
			bool hasStackTraceHidden)> settings = context.CompilationProvider
			.Select((c, _) =>
			{
				bool hasMsTest3 =
					c.ReferencedAssemblyNames.Any(x => x.Name == "Microsoft.VisualStudio.TestPlatform.TestFramework");
				bool hasMsTest4 =
					c.ReferencedAssemblyNames.Any(x => x.Name == "MSTest.TestFramework");
				bool hasNunit = c.ReferencedAssemblyNames.Any(x => x.Name == "nunit.framework");
				bool hasTUnit = c.ReferencedAssemblyNames.Any(x => x.Name == "TUnit.Core") &&
				                c.ReferencedAssemblyNames.Any(x => x.Name == "TUnit.Assertions");
				bool hasXunit2 = c.ReferencedAssemblyNames.Any(x => x.Name == "xunit.assert");
				bool hasXunit3Core = c.ReferencedAssemblyNames.Any(x => x.Name == "xunit.v3.core");
				bool hasXunit3Assert = c.ReferencedAssemblyNames.Any(x => x.Name == "xunit.v3.assert");
				bool hasDoesNotReturn = HasAttribute(c, "System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute");
				bool hasStackTraceHidden = HasAttribute(c, "System.Diagnostics.StackTraceHiddenAttribute");
				return (hasMsTest3, hasMsTest4, hasNunit, hasTUnit, hasXunit2, hasXunit3Core, hasXunit3Assert,
					hasDoesNotReturn, hasStackTraceHidden);
			});

		// Generate the source from the captured values
		context.RegisterSourceOutput(settings, static (spc, opts) =>
		{
			string attributes = string.Empty;
			if (opts.hasDoesNotReturn)
			{
				attributes += "[System.Diagnostics.CodeAnalysis.DoesNotReturn]\n\t";
			}

			if (opts.hasStackTraceHidden)
			{
				attributes += "[System.Diagnostics.StackTraceHidden]\n\t";
			}

			if (opts.hasMsTest3 || opts.hasMsTest4)
			{
				spc.AddSource("MsTest.g.cs", MsTestAdapter(attributes));
			}

			if (opts.hasNunit)
			{
				spc.AddSource("Nunit.g.cs", NunitAdapter(attributes));
			}

			if (opts.hasTUnit)
			{
				spc.AddSource("TUnit.g.cs", TUnitAdapter(attributes));
			}

			if (opts.hasXunit2)
			{
				spc.AddSource("Xunit2.g.cs", Xunit2Adapter(attributes));
			}

			if (opts.hasXunit3Assert)
			{
				spc.AddSource("Xunit3.g.cs", Xunit3AssertAdapter(attributes));
			}
			else if (opts.hasXunit3Core)
			{
				spc.AddSource("Xunit3.g.cs", Xunit3CoreAdapter(attributes));
			}
		});
	}

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
