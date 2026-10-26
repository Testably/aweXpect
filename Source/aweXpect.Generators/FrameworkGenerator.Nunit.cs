using Microsoft.CodeAnalysis;

namespace aweXpect.Generators;

public partial class FrameworkGenerator
{
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

	private static FrameworkSupport NunitSupport(Compilation c)
	{
		INamedTypeSymbol? nunit = GetUsableType(c, "NUnit.Framework.TestContext");
		INamedTypeSymbol? nunitTest = GetUsableType(c, "NUnit.Framework.TestContext+TestAdapter");
		INamedTypeSymbol? nunitSuite = GetUsableType(c, "NUnit.Framework.Interfaces.ITest");
		bool hasTest = HasProperties(nunit, true, ("CurrentContext", "global::NUnit.Framework.TestContext")) &&
		               HasProperties(nunit, false, ("Test", "global::NUnit.Framework.TestContext.TestAdapter")) &&
		               HasProperties(nunitTest, false, ("MethodName", "string"));
		return new FrameworkSupport(
			hasTest &&
			HasProperties(nunitTest, false, ("ClassName", "string"), ("Arguments", "object[]"), ("ID", "string"),
				("Name", "string")) &&
			HasProperties(nunit, false, ("CurrentRepeatCount", "int")),
			hasTest &&
			HasProperties(nunitTest, false, ("Parent", "global::NUnit.Framework.Interfaces.ITest")) &&
			HasProperties(nunitSuite, false, ("Parent", "global::NUnit.Framework.Interfaces.ITest"), ("Method", null),
				("Arguments", "object[]")),
			hasTest && HasProperties(nunit, false, ("CancellationToken", "global::System.Threading.CancellationToken")),
			hasTest && HasMethod(nunit, "AddTestAttachment", true, "string", "string"));
	}

	/// <remarks>
	///     Outside of a test, e.g. in a one-time setup, the current context of NUnit describes the fixture, which has no
	///     method.
	/// </remarks>
	private static string NunitTestContext(bool isDeclared, FrameworkSupport support)
	{
		string fixture = support.ClassArguments ? NunitFixture : "";
		string classArguments = support.ClassArguments
			? "fixture == null || fixture.Arguments == null ? new object[0] : fixture.Arguments"
			: "new object[0]";
		return TestContextMembers(isDeclared, support.CurrentTest
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

				    {{fixture}}
				    				currentTest = new global::aweXpect.Core.Adapters.RunningTest(new global::aweXpect.Core.Adapters.TestIdentity(test.ClassName, {{classArguments}}, test.MethodName, test.Arguments ?? new object[0]), test.ID, test.Name, context.CurrentRepeatCount);
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
	}

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
}
