using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> is only used (referenced in their signature) by types in the filtered
	///     collections of types <paramref name="users" /> or <paramref name="additional" />, or by itself, within the
	///     types passed to <see cref="OnlyUsedByResult{TResult}.Within" />.
	/// </summary>
	/// <remarks>
	///     The collections are resolved once per assertion; every type within the scope that is not a member of the
	///     union of the allowed collections (by <see cref="Type" /> identity; a generic type definition in a
	///     collection matches any construction of it) must not depend on the type. A generic type definition as
	///     subject is also used by a dependency on any construction of it. Unlike <c>DependsOnlyOn</c>, neither the
	///     own namespace nor framework types are allowed implicitly.
	/// </remarks>
	[GuaranteesNotNull]
	public static OnlyUsedByResult<AndOrResult<Type, IThat<Type?>>> IsOnlyUsedBy(
		this IThat<Type?> subject, Filtered.Types users, params Filtered.Types[] additional)
	{
		TypeSetDependencyOptions allowed = TypeUsageOptions.CreateAllowed(users, additional);
		return new OnlyUsedByResult<AndOrResult<Type, IThat<Type?>>>(within
			=> new AndOrResult<Type, IThat<Type?>>(subject.Get().ExpectationBuilder
					.AddConstraint(new TypeUsageOptions(allowed, within),
						static (options, it, grammars)
							=> new IsOnlyUsedByConstraint(it, grammars, options)),
				subject));
	}

	private sealed class IsOnlyUsedByConstraint(
		string it,
		ExpectationGrammars grammars,
		TypeUsageOptions options)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IAsyncConstraint<Type?>
	{
		private IReadOnlyList<string> _users = [];

		public async ValueTask<ConstraintResult> IsMetBy(Type? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			ResolvedTypeUsages usages = await options.Resolve(cancellationToken);
			_users = usages.GetDisallowedUsers(actual);
			Outcome = _users.Count == 0 ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is only used by ").Append(options.Describe());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had disallowed users");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not only used by ").Append(options.Describe());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was only used by the allowed types");

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (Outcome == Outcome.Failure && !IsNegated)
			{
				contexts.AddDependenciesContext("Disallowed users", _users);
			}
		}
	}
}
