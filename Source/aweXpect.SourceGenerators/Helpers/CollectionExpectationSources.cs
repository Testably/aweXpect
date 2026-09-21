using System.Text;

namespace aweXpect.SourceGenerators.Helpers;

internal static class CollectionExpectationSources
{
	public const string Attribute =
		"""
		using System;

		namespace aweXpect.SourceGenerators;

		#nullable enable
		/// <summary>
		/// Create the overloads of a collection expectation from the annotated helper, which holds the body.
		/// </summary>
		/// <remarks>
		/// The helper's signature is the declaration: its return type, its subject and expected parameters and its
		/// type parameters are emitted verbatim, and a helper behind an <c>#if</c> generates nothing where it does
		/// not exist. Only what the signature cannot state belongs here.
		/// </remarks>
		[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
		internal class CreateCollectionExpectationAttribute : System.Attribute
		{
			/// <param name="name">The expectation name, where <c>{Not}</c> marks the negated variant.</param>
			public CreateCollectionExpectationAttribute(string name)
			{
				Name = name;
			}

			public string Name { get; }

			/// <summary>
			/// The class whose parameterless <c>Create*</c> methods define the supported element types. Each nullable
			/// element additionally yields an overload taking a non-nullable expected collection.
			/// </summary>
			public Type? Factory { get; set; }

			/// <summary>
			/// The expected parameter type, when it differs from the helper's second parameter. <c>{subject}</c>
			/// stands for the subject collection type and <c>{item}</c> for its element.
			/// </summary>
			public string? ExpectedType { get; set; }

			/// <summary>
			/// Emit this family once per collection type listed in <c>[CollectionSubjects]</c> on the containing
			/// class, substituting the helper's <c>TCollection</c>.
			/// </summary>
			public bool PerSubject { get; set; }

			/// <summary>Mark the overloads with <c>[GuaranteesNotNull]</c>.</summary>
			public bool GuaranteesNotNull { get; set; }

			/// <summary>The <c>OverloadResolutionPriority</c>, or <c>0</c> for none.</summary>
			public int Priority { get; set; }

			public string? Summary { get; set; }
			public string? NegatedSummary { get; set; }
			public string? Remarks { get; set; }
			public string? NegatedRemarks { get; set; }
		}

		/// <summary>
		/// The collection types of this class that cannot reach an expectation through the covariance of
		/// <c>IThat&lt;out T&gt;</c> - value types such as <c>ImmutableArray&lt;T&gt;</c> - and therefore need their
		/// own overloads. Adding one here adds it to every <c>PerSubject</c> family of the class; a type that does
		/// not exist in the current compilation is skipped.
		/// </summary>
		[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
		internal class CollectionSubjectsAttribute : System.Attribute
		{
			/// <param name="collectionTypes">The collection types, where <c>{item}</c> marks the element.</param>
			public CollectionSubjectsAttribute(params string[] collectionTypes)
			{
				CollectionTypes = collectionTypes;
			}

			public string[] CollectionTypes { get; }
		}
		#nullable disable
		""";

	public static string GenerateExtensionClass(List<CollectionExpectationFamily> families)
	{
		CollectionExpectationFamily first = families[0];
		StringBuilder builder = new();
		builder.Append("namespace ").Append(first.Namespace).AppendLine(";").AppendLine();
		builder.AppendLine("#nullable enable");
		// The .editorconfig severities do not reach generated documents, so the repo-wide decision on the
		// optional-parameter back-compat rule has to be restated here.
		builder.AppendLine("#pragma warning disable RS0026");
		builder.Append("public static partial class ").AppendLine(first.ClassName);
		builder.AppendLine("{");
		builder.AppendLine(string.Join("\n\n", families.SelectMany(x => x.Methods)));
		builder.AppendLine("}");
		builder.AppendLine("#nullable disable");
		return builder.ToString();
	}
}
