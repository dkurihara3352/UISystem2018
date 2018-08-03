using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIInputHandler{
		/* Releasing
			pointer up =>
				OnRelease
				if deltaP over thresh
					OnSwipe
				else
					if stays in-bound && within time frame
						OnTap

		*/
		void OnTouch( int touchCount);
		void OnDelayedTouch();
		void OnRelease();
		void OnDelayedRelease();
		/* called after both OnRelease and OnTap */
		void OnTap( int tapCount);
		void OnBeginDrag(ICustomEventData eventData);
		void OnDrag( ICustomEventData eventData);
		void OnHold( float deltaT);
		/* called every frame from pointer down to up */
		void OnSwipe( ICustomEventData eventData);
	}	
}
