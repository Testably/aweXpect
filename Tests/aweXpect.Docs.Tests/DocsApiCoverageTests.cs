using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Signaling;

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
		string text = string.Join("\n", Pages());

		List<string> missing = ExpectationMethodNames()
			.Where(name => !Regex.IsMatch(text, $@"\b{name}\b"))
			.ToList();

		Fail.Unless(missing.Count == 0, "The following expectations are not mentioned in the docs:" +
		                                Environment.NewLine + string.Join(Environment.NewLine, missing));
	}

	[Test]
	public async Task UserFacingMembers_ShouldFindTheMembers()
	{
		List<(string Type, string Name, bool IsStatic)> members = UserFacingMembers();

		await That(members.Count).IsGreaterThan(15)
			.Because("the coverage check must not pass because it no longer finds the user-facing members");
	}

	[Test]
	public void UserFacingMembers_ShouldBeMentionedInTheDocs()
	{
		List<string> pages = Pages();

		List<string> missing = UserFacingMembers()
			.Where(member => !pages.Any(page => IsMentioned(page, member)))
			.Select(member => $"{member.Type}.{member.Name}")
			.ToList();

		Fail.Unless(missing.Count == 0, "The following members are not mentioned in the docs:" +
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

	/// <summary>
	///     The types a test uses directly instead of through an expectation: the static entry points, and the signaler
	///     with its results.
	/// </summary>
	/// <remarks>
	///     The types for extension authors are not listed, because the pages under <c>11-extending</c> describe them
	///     with samples that are compiled against aweXpect.Core.
	/// </remarks>
	private static readonly Type[] UserFacingTypes =
	[
		typeof(Expect), typeof(Fail), typeof(Skip),
		typeof(Signaler), typeof(Signaler<>), typeof(SignalerResult), typeof(SignalerResult<>),
	];

	/// <summary>
	///     The public methods and properties that the <see cref="UserFacingTypes" /> declare, except the obsolete or
	///     hidden ones.
	/// </summary>
	private static List<(string Type, string Name, bool IsStatic)> UserFacingMembers()
		=> UserFacingTypes
			.SelectMany(type => type
				.GetMembers(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
				            BindingFlags.DeclaredOnly)
				.Where(member => member is MethodInfo { IsSpecialName: false, } or PropertyInfo &&
				                 !member.IsDefined(typeof(ObsoleteAttribute), false) &&
				                 member.GetCustomAttribute<EditorBrowsableAttribute>()?.State !=
				                 EditorBrowsableState.Never)
				.Select(member => (Type: type.Name.Split('`')[0], member.Name,
					IsStatic: type is { IsAbstract: true, IsSealed: true, })))
			.Distinct()
			.OrderBy(member => $"{member.Type}.{member.Name}", StringComparer.Ordinal)
			.ToList();

	/// <summary>
	///     A static member has to be mentioned together with its type (e.g. <c>Fail.When</c>), and an instance member
	///     as a member access (e.g. <c>.WaitAsync</c>) on a page that names its type, because names like <c>Test</c>
	///     or <c>Count</c> are too common to be searched for on their own.
	/// </summary>
	private static bool IsMentioned(string page, (string Type, string Name, bool IsStatic) member)
		=> member.IsStatic
			? Regex.IsMatch(page, $@"\b{member.Type}\.{member.Name}\b")
			: Regex.IsMatch(page, $@"\b{member.Type}\b") && Regex.IsMatch(page, $@"\.{member.Name}\b");

	private static List<string> Pages()
		=> Directory
			.EnumerateFiles(PagesDirectory(), "*.*", SearchOption.AllDirectories)
			.Where(path => path.EndsWith(".md") || path.EndsWith(".mdx"))
			.Select(File.ReadAllText)
			.ToList();

	private static string PagesDirectory([CallerFilePath] string path = "")
		=> Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path)!, "..", "..", "Docs", "pages"));
}
