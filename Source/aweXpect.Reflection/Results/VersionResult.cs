using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of verifying the <see cref="AssemblyName.Version" /> of an <see cref="Assembly" /> or of a collection
///     of <see cref="Assembly" />, allowing comparisons on the individual version components.
/// </summary>
public sealed class VersionResult<TType> : AndOrResult<TType, IThat<TType?>>
{
	private readonly List<VersionComponentCheck> _checks = [];

	internal VersionResult(IThat<TType?> subject,
		Func<ExpectationBuilder, List<VersionComponentCheck>, ExpectationBuilder> addConstraint)
		: this(subject, subject.Get().ExpectationBuilder, addConstraint)
	{
	}

	private VersionResult(IThat<TType?> subject, ExpectationBuilder expectationBuilder,
		Func<ExpectationBuilder, List<VersionComponentCheck>, ExpectationBuilder> addConstraint)
		: base(expectationBuilder, subject)
	{
		addConstraint(expectationBuilder, _checks);
	}

	/// <summary>
	///     Compares the <see cref="Version.Major" /> version component.
	/// </summary>
	public VersionComponentResult<TType> WithMajor => new(this, "major version", version => version.Major);

	/// <summary>
	///     Compares the <see cref="Version.Minor" /> version component.
	/// </summary>
	public VersionComponentResult<TType> WithMinor => new(this, "minor version", version => version.Minor);

	/// <summary>
	///     Compares the <see cref="Version.Build" /> version component.
	/// </summary>
	/// <remarks>The build component is <c>-1</c> when it is absent from the version.</remarks>
	public VersionComponentResult<TType> WithBuild => new(this, "build version", version => version.Build);

	/// <summary>
	///     Compares the <see cref="Version.Revision" /> version component.
	/// </summary>
	/// <remarks>The revision component is <c>-1</c> when it is absent from the version.</remarks>
	public VersionComponentResult<TType> WithRevision
		=> new(this, "revision version", version => version.Revision);

	internal VersionResult<TType> Add(string component, Func<Version, int> selector,
		PropertyComparison<int> comparison)
	{
		_checks.Add(new VersionComponentCheck(component, selector, comparison));
		return this;
	}
}
