using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIImage{
	/* Color.Lerp(white, black, darknessValue)
		*/
		float GetCurrentDarkness();/* range is from 0f to 1f */
		float GetDefaultDarkness();/* usually, 1f */
		float GetDarkenedDarkness();/* somewhere around .5f */
		void SetDarkness(float darkness);
		void SetLocalPosition(Vector2 pos);
		Vector2 GetCurPosInUIESpace(IUIElement targetUIE);
		void SetParentUIE(IUIElement parentUIE);
	}
}
