using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface ISceneObjectElementGroupAdaptor: IUIElementGroupAdaptor{}
	public class SceneObjectElementGroupAdaptor: /* AbsUIElementGroupAdaptor *//* AbsTwoConstraintsGroupAdaptor */AbsScrollerConstrainedGroupAdaptor, ISceneObjectElementGroupAdaptor{
		protected override List<IUIElement> GetGroupElements(){
			return GetChildUIEs();
		}
	}
}

