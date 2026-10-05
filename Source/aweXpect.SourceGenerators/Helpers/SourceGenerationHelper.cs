using Microsoft.CodeAnalysis.CSharp;

namespace aweXpect.SourceGenerators.Helpers;

internal static class SourceGenerationHelper
{
	public const string CreateExpectationOnAttribute =
		"""
		using System;

		namespace aweXpect.SourceGenerators;

		#nullable enable
		/// <summary>
		/// Create an assertion on the <typeparamref name="TTarget"/> attribute.
		/// </summary>
		/// <typeparam name="TTarget">The target type for the assertion</typeparam>
		[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
		internal class CreateExpectationOnAttribute<TTarget> : System.Attribute
		{
			public CreateExpectationOnAttribute(string name, string outcomeMethod)
			{
				TargetType = typeof(TTarget);
				PositiveName = name.Replace("{Not}", "");
				if (name.Contains("{Not}"))
				{
					NegativeName = name.Replace("{Not}", "Not");
				}
				OutcomeMethod = outcomeMethod;
			}

			public Type TargetType { get; }
			public string PositiveName { get; }
			public string? NegativeName { get; }
			public string OutcomeMethod { get; set; }
			public string? ExpectationText { get; set; }
			public string? PositiveExpectationText { get; set; }
			public string? NegativeExpectationText { get; set; }
			public string? Remarks { get; set; }
			/// <summary>The remarks of the negated overload, which gets none without them.</summary>
			public string? NegatedRemarks { get; set; }
			public string[] Using { get; set; } = [];
		}
		#nullable disable
		""";

	public const string CreateExpectationOnNullableAttribute =
		"""
		using System;

		namespace aweXpect.SourceGenerators;

		#nullable enable
		/// <summary>
		/// Create an assertion on the <typeparamref name="TTarget"/> attribute.
		/// </summary>
		/// <typeparam name="TTarget">The target type for the assertion</typeparam>
		[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
		internal class CreateExpectationOnNullableAttribute<TTarget> : System.Attribute
		{
			public CreateExpectationOnNullableAttribute(string name, string outcomeMethod)
			{
				TargetType = typeof(TTarget);
				PositiveName = name.Replace("{Not}", "");
				if (name.Contains("{Not}"))
				{
					NegativeName = name.Replace("{Not}", "Not");
				}
				OutcomeMethod = outcomeMethod;
			}

			public bool FailOnNull { get; set; } = true;
			public bool NegatedFailsOnNull { get; set; } = false;
			public Type TargetType { get; }
			public string PositiveName { get; }
			public string? NegativeName { get; }
			public string OutcomeMethod { get; set; }
			public string? ExpectationText { get; set; }
			public string? PositiveExpectationText { get; set; }
			public string? NegativeExpectationText { get; set; }
			public string? Summary { get; set; }
			public string? NegatedSummary { get; set; }
			public string? Remarks { get; set; }
			/// <summary>The remarks of the negated overload, which gets none without them.</summary>
			public string? NegatedRemarks { get; set; }
			public string[] Using { get; set; } = [];
		}
		#nullable disable
		""";

	public static string GenerateExtensionClass(ExpectationToGenerate expectationToGenerate)
	{
		bool failsOnNull = expectationToGenerate.IsNullable && expectationToGenerate.FailOnNull;
		string guaranteesNotNull = failsOnNull ? "\n\t[GuaranteesNotNull]" : "";
		string resultType = failsOnNull ? expectationToGenerate.NotNullTargetType : expectationToGenerate.TargetType;
		// When the subject is not checked for null, the outcome method decides: `Is{Not}NullOrEmpty` is
		// fulfilled by a null subject, so only its negated counterpart guarantees a not-null subject.
		bool negatedFailsOnNull =
			failsOnNull || (expectationToGenerate.IsNullable && expectationToGenerate.NegatedFailsOnNull);
		string negatedGuaranteesNotNull = negatedFailsOnNull ? "\n\t[GuaranteesNotNull]" : "";
		string negatedResultType = negatedFailsOnNull
			? expectationToGenerate.NotNullTargetType
			: expectationToGenerate.TargetType;
		string summary = expectationToGenerate.Summary ??
		                 $"Verifies that the subject {EscapeXml(expectationToGenerate.ExpectationText)}.";
		string namespaceDeclaration = expectationToGenerate.Namespace is null
			? ""
			: $$"""


			    namespace {{expectationToGenerate.Namespace}};
			    """;
		string result = $$"""
		                  {{string.Join("\n", expectationToGenerate.Usings.Select(x => $"using {x};"))}}
		                  using aweXpect.Core;
		                  using aweXpect.Core.Constraints;
		                  using aweXpect.Helpers;
		                  using aweXpect.Results;{{namespaceDeclaration}}

		                  #nullable enable
		                  {{expectationToGenerate.Accessibility}} static partial class {{expectationToGenerate.ClassName}}
		                  {
		                  	/// <summary>
		                  	///     {{ContinueDocumentation(summary)}}
		                  	/// </summary>{{expectationToGenerate.AppendRemarks()}}{{guaranteesNotNull}}
		                  	public static AndOrResult<{{resultType}}, IThat<{{expectationToGenerate.TargetType}}>> {{expectationToGenerate.Name}}(this IThat<{{expectationToGenerate.TargetType}}> subject)
		                  		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
		                  			new {{expectationToGenerate.Name}}Constraint(it, grammars)),
		                  		subject);


		                  """;
		if (expectationToGenerate.IncludeNegated)
		{
			string negatedSummary = expectationToGenerate.NegatedSummary ??
			                        $"Verifies that the subject {EscapeXml(expectationToGenerate.NegatedExpectationText)}.";
			result += $$"""
			            	/// <summary>
			            	///     {{ContinueDocumentation(negatedSummary)}}
			            	/// </summary>{{expectationToGenerate.AppendNegatedRemarks()}}{{negatedGuaranteesNotNull}}
			            	public static AndOrResult<{{negatedResultType}}, IThat<{{expectationToGenerate.TargetType}}>> {{expectationToGenerate.NegatedName}}(this IThat<{{expectationToGenerate.TargetType}}> subject)
			            		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
			            			new {{expectationToGenerate.Name}}Constraint(it, grammars).Invert()),
			            		subject);


			            """;
		}

		if (failsOnNull)
		{
			result += $$"""
			            	private sealed class {{expectationToGenerate.Name}}Constraint(string it, ExpectationGrammars grammars)
			            		: ConstraintResult.WithNotNullValue<{{expectationToGenerate.TargetType}}>(it, grammars),
			            			IValueConstraint<{{expectationToGenerate.TargetType}}>
			            	{
			            		public ConstraintResult IsMetBy({{expectationToGenerate.TargetType}} actual)
			            		{
			            			Actual = actual;
			            			Outcome = actual is not null && {{expectationToGenerate.OutcomeMethod.Replace("{value}", "actual")}} ? Outcome.Success : Outcome.Failure;
			            			return this;
			            		}
			            	
			            		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			            			=> stringBuilder.Append({{Verb(expectationToGenerate.ExpectationText)}});
			            	
			            		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			            		{
			            			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			            			Formatter.Format(stringBuilder, Actual);
			            		}
			            	
			            		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			            			=> stringBuilder.Append({{Verb(expectationToGenerate.NegatedExpectationText)}});
			            	
			            		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			            			=> AppendNormalResult(stringBuilder, indentation);
			            	}
			            """;
		}
		else
		{
			result += $$"""
			            	private sealed class {{expectationToGenerate.Name}}Constraint(string it, ExpectationGrammars grammars)
			            		: ConstraintResult.WithValue<{{expectationToGenerate.TargetType}}>(it, grammars),
			            			IValueConstraint<{{expectationToGenerate.TargetType}}>
			            	{
			            		public ConstraintResult IsMetBy({{expectationToGenerate.TargetType}} actual)
			            		{
			            			Actual = actual;
			            			Outcome = {{expectationToGenerate.OutcomeMethod.Replace("{value}", "actual")}} ? Outcome.Success : Outcome.Failure;
			            			return this;
			            		}
			            	
			            		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			            			=> stringBuilder.Append({{Verb(expectationToGenerate.ExpectationText)}});
			            	
			            		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			            		{
			            			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			            			Formatter.Format(stringBuilder, Actual);
			            		}
			            	
			            		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			            			=> stringBuilder.Append({{Verb(expectationToGenerate.NegatedExpectationText)}});
			            	
			            		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			            			=> AppendNormalResult(stringBuilder, indentation);
			            	}
			            """;
		}

		result += """
		          }
		          #nullable disable
		          """;
		return result.TrimStart();
	}

	/// <summary>
	///     The expression that selects the singular <paramref name="text" /> or its plural form by the grammars.
	/// </summary>
	private static string Verb(string text)
	{
		int space = text.IndexOf(' ');
		string head = space < 0 ? text : text.Substring(0, space);
		string tail = space < 0 ? "" : text.Substring(space);
		string plural = head switch
		{
			"is" => "are" + tail,
			"has" => "have" + tail,
			"does" => "do" + tail,
			_ => text,
		};
		return $"Grammars.Verb({SymbolDisplay.FormatLiteral(text, true)}, {SymbolDisplay.FormatLiteral(plural, true)})";
	}

	/// <remarks>
	///     The expectation texts are plain text, whereas a summary given in the attribute is already XML.
	/// </remarks>
	private static string EscapeXml(string text)
		=> text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

	private static string ContinueDocumentation(string text)
		=> text.Replace("\n", "\n\t///     ");
}
