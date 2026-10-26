using Microsoft.CodeAnalysis;

namespace aweXpect.Generators;

public partial class FrameworkGenerator
{
	private static FrameworkSupport Xunit3Support(Compilation c)
	{
		INamedTypeSymbol? xunitContext = GetUsableType(c, "Xunit.ITestContext");
		INamedTypeSymbol? xunitTest = GetUsableType(c, "Xunit.v3.IXunitTest");
		bool hasTest =
			HasProperties(GetUsableType(c, "Xunit.TestContext"), true, ("Current", "global::Xunit.ITestContext")) &&
			HasProperties(xunitContext, false, ("Test", null));
		return new FrameworkSupport(
			hasTest &&
			HasProperties(xunitTest, false, ("TestMethodArguments", "object[]"), ("UniqueID", "string"),
				("TestDisplayName", "string")) &&
			HasProperties(GetPropertyType(xunitTest, "TestCase"), false, ("TestClassName", "string"),
				("TestMethodName", "string")),
			false,
			hasTest &&
			HasProperties(xunitContext, false, ("CancellationToken", "global::System.Threading.CancellationToken")),
			hasTest && HasMethod(xunitContext, "AddAttachment", false, "string", "byte[]", "string"));
	}

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
}
