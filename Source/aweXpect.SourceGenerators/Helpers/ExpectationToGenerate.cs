using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace aweXpect.SourceGenerators.Helpers;

internal readonly record struct ExpectationToGenerate
{
	public ExpectationToGenerate(INamedTypeSymbol classSymbol,
		INamedTypeSymbol targetType,
		string positiveName,
		string? negativeName,
		string outcomeMethod,
		AttributeData attributeData)
	{
		Namespace = classSymbol.ContainingNamespace.IsGlobalNamespace
			? null
			: classSymbol.ContainingNamespace.ToString();
		ClassName = classSymbol.Name;
		Accessibility = SyntaxFacts.GetText(classSymbol.DeclaredAccessibility);
		TargetType = targetType.ToDisplayString();
		NotNullTargetType = TargetType;
		Name = positiveName;
		NegatedName = negativeName;
		IncludeNegated = negativeName is not null;
		OutcomeMethod = outcomeMethod;

		string? positiveExpectationText = null;
		string? negativeExpectationText = null;
		foreach (KeyValuePair<string, TypedConstant> namedArgument in attributeData.NamedArguments)
		{
			switch (namedArgument.Key)
			{
				case "ExpectationText":
					string? expectationText = namedArgument.Value.Value?.ToString();
					positiveExpectationText = expectationText?.Replace("{not}", "").Replace("  ", " ");
					negativeExpectationText = expectationText?.Replace("{not}", " not ").Replace("  ", " ");
					break;
				case "PositiveExpectationText":
					positiveExpectationText = namedArgument.Value.Value?.ToString();
					break;
				case "NegativeExpectationText":
					negativeExpectationText = namedArgument.Value.Value?.ToString();
					break;
				case "Summary":
					Summary = namedArgument.Value.Value?.ToString();
					break;
				case "NegatedSummary":
					NegatedSummary = namedArgument.Value.Value?.ToString();
					break;
				case "Remarks":
					Remarks = namedArgument.Value.Value?.ToString();
					break;
				case "FailOnNull":
					FailOnNull = namedArgument.Value.Value as bool? ?? true;
					break;
				case "NegatedFailsOnNull":
					NegatedFailsOnNull = namedArgument.Value.Value as bool? ?? false;
					break;
				case "Using":
					Usings = new EquatableArray<string>(
						namedArgument.Value.Values.Select(x => x.Value?.ToString()).Where(x => x != null).ToArray()!);
					break;
			}
		}

		IsNullable = attributeData.AttributeClass!.Name ==
		             nameof(SourceGenerationHelper.CreateExpectationOnNullableAttribute);
		if (IsNullable)
		{
			TargetType += "?";
		}

		ExpectationText = positiveExpectationText ?? positiveName;
		NegatedExpectationText = negativeExpectationText ?? $"not {positiveName}";
		FileName = Namespace is null ? $"{ClassName}.{Name}.g.cs" : $"{Namespace}.{ClassName}.{Name}.g.cs";
	}

	public bool FailOnNull { get; } = true;
	public bool NegatedFailsOnNull { get; }
	public EquatableArray<string> Usings { get; } = new([]);
	public string FileName { get; }
	public bool IncludeNegated { get; }
	public bool IsNullable { get; }
	public string? NegatedName { get; }
	public string? Namespace { get; }
	public string ClassName { get; }
	public string Accessibility { get; }
	public string NotNullTargetType { get; }
	public string TargetType { get; }
	public string Name { get; }
	public string OutcomeMethod { get; }
	public string ExpectationText { get; }
	public string NegatedExpectationText { get; }
	public string? Summary { get; }
	public string? NegatedSummary { get; }
	public string? Remarks { get; }

	public string AppendRemarks()
	{
		if (string.IsNullOrEmpty(Remarks))
		{
			return "";
		}

		return $$"""

		         /// <remarks>
		         ///     {{Remarks!.Replace("\n", "\n///     ")}}
		         /// </remarks>
		         """.Replace("\n", "\n\t");
	}
}
