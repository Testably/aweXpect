using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatType
{
	[GenerateExpectation("Is{Not}Instantiable", "is {not} instantiable",
		Result = nameof(DescribeInstantiableType),
		Summary = "Verifies that the <see cref=\"Type\" /> is instantiable.",
		NegatedSummary = "Verifies that the <see cref=\"Type\" /> is not instantiable.",
		Remarks = "Abstract types, static types, interfaces and open generic type definitions are not considered instantiable.",
		NegatedRemarks = "Abstract types, static types, interfaces and open generic type definitions are not considered instantiable.")]
	private static bool IsInstantiableType(Type type) => type.IsReallyInstantiable();

	private static string DescribeInstantiableType(Type type)
		=> ResultTexts.Describe(type.IsReallyInstantiable()
			? "was instantiable"
			: $"was {GetNonInstantiableDescription(type)}", type);

	private static string GetNonInstantiableDescription(Type type)
	{
		if (type.IsInterface)
		{
			return "an interface";
		}

		if (type.IsReallyStatic())
		{
			return "static";
		}

		if (type.IsReallyAbstract())
		{
			return "abstract";
		}

		return "a generic type definition";
	}
}
