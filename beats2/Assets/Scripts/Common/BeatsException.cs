using System;
using System.Collections.Generic;
using Beats2;
using Beats2.Common;

namespace Beats2.Common {
	///	This is mainly to differentiate a custom game-related exception
	public class BeatsException: System.Exception {
		public BeatsException() {}
		public BeatsException(string tag, string message) : base(String.Format("{0}: {1}", tag, message)) {
			Logger.Error(tag, message);
		}
	}
}
