using Microsoft.CodeAnalysis;

namespace aweXpect.SourceGenerators.Helpers;

internal readonly record struct ExpectationToGenerate
{
	public ExpectationToGenerate(string @namespace,
		string className,
		INamedTypeSymbol targetType,
		string positiveName,
		string? negativeName,
		string outcomeMethod,
		AttributeData attributeData)
	{
		Namespace = @namespace;
		ClassName = className;
		TargetType = targetType.ToDisplayString();
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
					Usings =
						namedArgument.Value.Values.Select(x => x.Value?.ToString()).Where(x => x != null).ToArray()!;
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
		FileName = $"{ClassName}.{Name}.g.cs";
	}

	public bool FailOnNull { get; } = true;
	public bool NegatedFailsOnNull { get; }
	public string[] Usings { get; } = [];
	public string FileName { get; }
	public bool IncludeNegated { get; }
	public bool IsNullable { get; }
	public string? NegatedName { get; }
	public string Namespace { get; }
	public string ClassName { get; }
	public string TargetType { get; }
	public string Name { get; }
	public string OutcomeMethod { get; }
	public string ExpectationText { get; }
	public string NegatedExpectationText { get; }
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
