using System.Threading.Tasks;
using Godot;

public static class WaitClass
{
	/// <summary>
	/// Create a Godot built-in timer, and wait for it's <c>timeout</c> signal. Usage example: <c>await this.Wait(seconds);</c>
	/// </summary>
	/// <param name="n"></param>
	/// <param name="time">The timer length in seconds.</param>
	/// <returns></returns>
	public static SignalAwaiter Wait(this Node n, float time)
	{
		return n.ToSignal(n.GetTree().CreateTimer(time), "timeout");
	}
}