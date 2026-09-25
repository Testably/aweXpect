using System.Threading;

namespace aweXpect.Signaling;

/// <summary>
///     The result when waiting in a <see cref="Signaler" />.
/// </summary>
public class SignalerResult(bool isSuccess, int counter)
{
	/// <summary>
	///     The number of times the callback was triggered.
	/// </summary>
	public int Count { get; } = counter;

	/// <summary>
	///     Flag, indicating if the waiting was successful or not.
	/// </summary>
	/// <remarks>
	///     This flag will be <see langword="false" /> if the timeout expired
	///     or the <see cref="CancellationToken" /> was canceled prior to enough triggered callbacks;
	///     otherwise <see langword="true" />.
	/// </remarks>
	public bool IsSuccess { get; } = isSuccess;
}

/// <summary>
///     The result when waiting in a <see cref="Signaler{TParameter}" />.
/// </summary>
public class SignalerResult<TParameter>(bool isSuccess, TParameter[] parameters)
	: SignalerResult(isSuccess, parameters.Length)
{
	/// <summary>
	///     The parameters provided while triggering the callback.
	/// </summary>
	public TParameter[] Parameters { get; } = parameters;
}
