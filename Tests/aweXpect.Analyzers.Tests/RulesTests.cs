using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;

namespace aweXpect.Analyzers.Tests;

public class RulesTests
{
	[Test]
	public async Task AnalyzersPage_ShouldHaveASectionForEachRule()
	{
		string[] headings = File.ReadAllLines(Path.Combine(GetRepositoryDirectory(), "Docs", "pages", "07-analyzers.md"))
			.Where(line => line.StartsWith("## aweXpect", StringComparison.Ordinal))
			.Select(line => line.Substring(3).Trim())
			.ToArray();

		await That(headings).IsEqualTo(Descriptors().Select(descriptor => descriptor.Id).Distinct())
			.Because("the help link of each rule points to the section with the rule ID as heading");
	}

	[Test]
	public async Task Descriptors_ShouldLinkToTheirSectionOnTheAnalyzersPage()
	{
		foreach (DiagnosticDescriptor descriptor in Descriptors())
		{
			await That(descriptor.HelpLinkUri)
				.IsEqualTo($"https://docs.testably.org/aweXpect/analyzers#{descriptor.Id.ToLowerInvariant()}")
				.Because($"{descriptor.Id} should link to its section on the analyzers page");
		}
	}

	private static IEnumerable<DiagnosticDescriptor> Descriptors()
		=> typeof(Rules).GetFields(BindingFlags.Public | BindingFlags.Static)
			.Where(field => field.FieldType == typeof(DiagnosticDescriptor))
			.Select(field => (DiagnosticDescriptor)field.GetValue(null)!)
			.OrderBy(descriptor => descriptor.Id, StringComparer.Ordinal);

	private static string GetRepositoryDirectory([CallerFilePath] string path = "")
		=> Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path)!, "..", ".."));
}
