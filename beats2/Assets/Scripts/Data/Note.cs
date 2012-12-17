using UnityEngine;
using System;
using System.Collections.Generic;
using Beats2;
using Beats2.Common;

// TODO - implement
// Last updated 2012/12/13
// ~Keripo
namespace Beats2.Data {
	
	public enum NoteTypes {
		TAP,
		HOLD_START,
		HOLD_FINISH,
		ROLL_START,
		ROLL_FINISH,
		SLIDER_START,
		SLIDER_FINISH,
		MINE
	}
	
	public enum NoteState {
		ALIVE,
		TAPPED,
		HELD,
		RELEASED,		
		MISSED,
		DEAD
	}
	
	public class Note {
	
	}
}
