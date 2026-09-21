using System.Text;

namespace aweXpect.SourceGenerators.Helpers;

internal static class CollectionToleranceSources
{
	public const string Attribute =
		"""
		using System;

		namespace aweXpect.SourceGenerators;

		#nullable enable
		/// <summary>
		/// Create the tolerance overloads of a collection expectation.
		/// </summary>
		[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
		internal class CreateCollectionToleranceExpectationsAttribute : System.Attribute
		{
			/// <param name="name">The expectation name, where <c>{Not}</c> marks the negated variant.</param>
			/// <param name="helper">The internal method of the same class that holds the body.</param>
			/// <param name="collectionType">The unbound generic collection type, without the arity suffix.</param>
			public CreateCollectionToleranceExpectationsAttribute(string name, string helper, string collectionType)
			{
				Name = name;
				Helper = helper;
				CollectionType = collectionType;
			}

			public string Name { get; }
			public string Helper { get; }
			public string CollectionType { get; }

			/// <summary>
			/// The class whose parameterless <c>Create*</c> methods define the supported element types.
			/// </summary>
			public Type? Factory { get; set; }
			public string? ConditionalOn { get; set; }
			public string? Summary { get; set; }
			public string? NegatedSummary { get; set; }
		}
		#nullable disable
		""";

	public static string GenerateExtensionClass(List<CollectionToleranceFamily> families)
	{
		CollectionToleranceFamily first = families[0];
		StringBuilder builder = new();
		builder.Append("namespace ").Append(first.Namespace).AppendLine(";").AppendLine();
		builder.AppendLine("#nullable enable");
		// The .editorconfig severities do not reach generated documents, so the repo-wide decision on the
		// optional-parameter back-compat rule has to be restated here.
		builder.AppendLine("#pragma warning disable RS0026");
		builder.Append("public static partial class ").AppendLine(first.ClassName);
		builder.AppendLine("{");
		bool isFirstMethod = true;
		foreach (CollectionToleranceFamily family in families)
		{
			if (family.ConditionalOn != null)
			{
				builder.Append("#if ").AppendLine(family.ConditionalOn);
			}

			foreach (string method in family.Methods)
			{
				if (!isFirstMethod)
				{
					builder.AppendLine();
				}

				isFirstMethod = false;
				builder.AppendLine(method);
			}

			if (family.ConditionalOn != null)
			{
				builder.AppendLine("#endif");
			}
		}

		builder.AppendLine("}");
		builder.AppendLine("#nullable disable");
		return builder.ToString();
	}
}
