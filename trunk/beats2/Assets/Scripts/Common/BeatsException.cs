
// This is mainly to differentiate a custom game-related exception
namespace Beats2.Common {
	public class BeatsException: System.Exception {
		public BeatsException() {}
		public BeatsException(string message): base(message) {}
	}
}
