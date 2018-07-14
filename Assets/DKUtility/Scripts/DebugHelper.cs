using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DKUtility{
	public static class DebugHelper {
		public static void PrintInRed(string text){
			Debug.Log("<color=#ff0000ff>" + text + "</color>");
		}
	}
}
