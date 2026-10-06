using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using aweXpect.Core;

namespace aweXpect.Docs.Tests;

[Explicit]
[TUnit.Core.Category(TestCategories.Slow)]
public sealed class DocsApiCoverageTests
{
	[Test]
	public async Task ExpectationMethodNames_ShouldFindTheExpectations()
	{
		List<string> names = ExpectationMethodNames();

		await That(names.Count).IsGreaterThan(100)
			.Because("the coverage check must not pass because it no longer finds the expectation methods");
	}

	[Test]
	public void ExpectationMethods_ShouldBeMentionedInTheDocs()
	{
		string text = string.Join("\n", Directory
			.EnumerateFiles(PagesDirectory(), "*.*", SearchOption.AllDirectories)
			.Where(path => path.EndsWith(".md") || path.EndsWith(".mdx"))
			.Select(File.ReadAllText));

		List<string> missing = ExpectationMethodNames()
			.Where(name => !Regex.IsMatch(text, $@"\b{name}\b"))
			.ToList();

		Fail.Unless(missing.Count == 0, "The following expectations are not mentioned in the docs:" +
		                                Environment.NewLine + string.Join(Environment.NewLine, missing));
	}

	/// <summary>
	///     The names of the public extension methods on <see cref="IThat{T}" /> in the <c>aweXpect</c> assembly, except
	///     the obsolete or hidden ones.
	/// </summary>
	private static List<string> ExpectationMethodNames()
		=> typeof(ThatString).Assembly.GetExportedTypes()
			.Where(type => type is { IsAbstract: true, IsSealed: true, })
			.SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
			.Where(method => method.IsDefined(typeof(ExtensionAttribute), false) &&
			                 !method.IsDefined(typeof(ObsoleteAttribute), false) &&
			                 method.GetCustomAttribute<EditorBrowsableAttribute>()?.State !=
			                 EditorBrowsableState.Never &&
			                 IsThat(method.GetParameters()[0].ParameterType))
			.Select(method => method.Name)
			.Distinct()
			.OrderBy(name => name, StringComparer.Ordinal)
			.ToList();

	private static bool IsThat(Type type)
		=> type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IThat<>);

	private static string PagesDirectory([CallerFilePath] string path = "")
		=> Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path)!, "..", "..", "Docs", "pages"));
}
