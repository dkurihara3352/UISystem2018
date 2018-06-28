using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IGhostificationStateHandler{
		void Ghostify();
		void Deghostify();
		bool IsGhostified();
	}
	public interface IGhostificationStateEngine: IGhostificationStateHandler{
	}
}
