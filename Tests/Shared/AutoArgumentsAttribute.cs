using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Kernel;

namespace aweXpect.TestHelpers;

/// <summary>
///     Provides the arguments of a test from the given <c>values</c>, followed by values that AutoFixture generates
///     for the remaining parameters.
/// </summary>
public sealed class AutoArgumentsAttribute(params object?[]? values) : UntypedDataSourceGeneratorAttribute
{
	// A single `null` argument binds to the array itself instead of to its first element.
	private readonly object?[] _values = values ?? [null,];

	/// <inheritdoc />
	protected override IEnumerable<Func<object?[]?>> GenerateDataSources(
		DataGeneratorMetadata dataGeneratorMetadata)
	{
		if (dataGeneratorMetadata.TestInformation is { } test)
		{
			Fixture fixture = new();
			yield return () => _values
				.Concat(test.Parameters
					.Skip(_values.Length)
					.Select(parameter => new SpecimenContext(fixture).Resolve(parameter.Type)))
				.ToArray();
		}
	}
}
