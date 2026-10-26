using Microsoft.CodeAnalysis;

namespace aweXpect.Generators;

public partial class FrameworkGenerator
{
	private static FrameworkSupport MsTestSupport(Compilation c, bool allowsExperimental)
	{
		INamedTypeSymbol? msTest = GetUsableType(c, "Microsoft.VisualStudio.TestTools.UnitTesting.TestContext");
		bool hasContext = GetMembers(msTest, "Current").OfType<IPropertySymbol>().Any(x =>
			x.IsStatic && IsUsable(x, allowsExperimental) &&
			x.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ==
			"global::Microsoft.VisualStudio.TestTools.UnitTesting.TestContext");
		return new FrameworkSupport(
			hasContext &&
			HasProperties(msTest, false, ("FullyQualifiedTestClassName", "string"), ("TestName", "string"),
				("TestData", "object[]")),
			false,
			hasContext &&
			HasProperties(msTest, false, ("CancellationToken", "global::System.Threading.CancellationToken")),
			hasContext && HasMethod(msTest, "AddResultFile", false, "string"));
	}

	private static bool IsExperimentalOrObsolete(ISymbol symbol, bool allowsExperimental)
		=> symbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() switch
		{
			"System.Diagnostics.CodeAnalysis.ExperimentalAttribute" => !allowsExperimental ||
			                                                           a.ConstructorArguments.FirstOrDefault().Value as string !=
			                                                           "MSTESTEXP",
			"System.ObsoleteAttribute" => true,
			_ => false,
		});

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
		return allowsExperimental && (support.CurrentTest || support.CancellationToken || support.Attachment)
			? "\n#pragma warning disable MSTESTEXP" + members + "\n#pragma warning restore MSTESTEXP"
			: members;
	}

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
}
