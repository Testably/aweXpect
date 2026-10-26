using Microsoft.CodeAnalysis;

namespace aweXpect.Generators;

public partial class FrameworkGenerator
{
	private static FrameworkSupport TUnitSupport(Compilation c)
	{
		INamedTypeSymbol? tunit = GetUsableType(c, "TUnit.Core.TestContext");
		INamedTypeSymbol? tunitDetails = GetUsableType(c, "TUnit.Core.TestDetails");
		ITypeSymbol? tunitExecution = GetPropertyType(tunit, "Execution");
		bool hasContext = HasProperties(tunit, true, ("Current", "global::TUnit.Core.TestContext"));
		return new FrameworkSupport(
			hasContext &&
			HasProperties(GetPropertyType(tunit, "Metadata"), false, ("TestDetails", "global::TUnit.Core.TestDetails"),
				("DisplayName", "string")) &&
			HasProperties(tunitDetails, false, ("ClassType", "global::System.Type"), ("MethodName", "string"),
				("TestMethodArguments", "object[]"), ("TestId", "string")) &&
			HasProperties(tunitExecution, false, ("CurrentRetryAttempt", "int")),
			HasProperties(tunitDetails, false, ("TestClassArguments", "object[]")),
			hasContext &&
			HasProperties(tunitExecution, false, ("CancellationToken", "global::System.Threading.CancellationToken")),
			hasContext &&
			HasMethod(GetPropertyType(tunit, "Output"), "AttachArtifact", false, "string", "string?", "string?"));
	}
	/// <remarks>
	///     The id is the one of the test, not of the <c>TestContext</c>, so that it stays the same when the test is
	///     retried. The name of the class is its <c>ToString()</c>, as its <c>FullName</c> names the assembly and the version
	///     of the runtime for each type argument of a generic test class. TUnit attaches a missing file without an error,
	///     so the adapter checks that it exists.
	/// </remarks>
	private static string TUnitTestContext(bool isDeclared, FrameworkSupport support)
	{
		string classArguments = support.ClassArguments
			? "details.TestClassArguments ?? new object[0]"
			: "new object[0]";
		return TestContextMembers(isDeclared, support.CurrentTest
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

				    				currentTest = new global::aweXpect.Core.Adapters.RunningTest(new global::aweXpect.Core.Adapters.TestIdentity(details.ClassType.ToString(), {{classArguments}}, details.MethodName, details.TestMethodArguments ?? new object[0]), details.TestId, context.Metadata.DisplayName, context.Execution == null ? 0 : context.Execution.CurrentRetryAttempt);
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
	}

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
}
