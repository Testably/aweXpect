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
		/// Create the overloads of a collection expectation that delegate to a shared internal helper.
		/// </summary>
		[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
		internal class CreateCollectionExpectationAttribute : System.Attribute
		{
			/// <param name="name">The expectation name, where <c>{Not}</c> marks the negated variant.</param>
			/// <param name="helper">The internal method of the same class that holds the body.</param>
			/// <param name="collectionType">The subject collection type, where <c>{item}</c> marks the element.</param>
			public CreateCollectionExpectationAttribute(string name, string helper, string collectionType)
			{
				Name = name;
				Helper = helper;
				CollectionType = collectionType;
			}

			public string Name { get; }
			public string Helper { get; }
			public string CollectionType { get; }

			/// <summary>
			/// The class whose parameterless <c>Create*</c> methods define the supported element types. Each nullable
			/// element additionally yields an overload taking a non-nullable expected collection.
			/// </summary>
			public Type? Factory { get; set; }

			/// <summary>The fixed element type, when the overload is not generic and has no factory.</summary>
			public string? ElementType { get; set; }

			/// <summary>The type parameters of the generated method; the first one is used as the element type.</summary>
			public string[]? TypeParameters { get; set; }

			/// <summary>The expected parameter type, where <c>{item}</c> marks the element.</summary>
			public string? ExpectedType { get; set; }

			/// <summary>The <c>OverloadResolutionPriority</c>, or <c>0</c> for none.</summary>
			public int Priority { get; set; }

			public string? ConditionalOn { get; set; }
			public string? Summary { get; set; }
			public string? NegatedSummary { get; set; }
			public string? Remarks { get; set; }
			public string? NegatedRemarks { get; set; }
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
		bool isFirst = true;
		foreach (string method in families.SelectMany(x => x.Methods))
		{
			if (!isFirst && !method.StartsWith("#endif"))
			{
				builder.AppendLine();
			}

			isFirst = false;
			builder.AppendLine(method);
		}

		builder.AppendLine("}");
		builder.AppendLine("#nullable disable");
		return builder.ToString();
	}
}
