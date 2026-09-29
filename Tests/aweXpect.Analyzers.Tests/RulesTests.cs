using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Xunit;

namespace aweXpect.Analyzers.Tests;

public class RulesTests
{
	[Fact]
	public async Task AnalyzersPage_ShouldHaveASectionForEachRule()
	{
		string[] headings = File.ReadAllLines(Path.Combine(GetRepositoryDirectory(), "Docs", "pages", "10-analyzers.md"))
			.Where(line => line.StartsWith("## "))
			.Select(line => line.Substring(3).Trim())
			.ToArray();

		await Expect.That(headings).IsEqualTo(Descriptors().Select(descriptor => descriptor.Id).Distinct())
			.Because("the help link of each rule points to the section with the rule ID as heading");
	}

	[Fact]
	public async Task Descriptors_ShouldLinkToTheirSectionOnTheAnalyzersPage()
	{
		foreach (DiagnosticDescriptor descriptor in Descriptors())
		{
			await Expect.That(descriptor.HelpLinkUri)
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
