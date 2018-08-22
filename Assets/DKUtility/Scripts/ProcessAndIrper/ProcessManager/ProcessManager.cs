using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility.CurveUtility;

namespace DKUtility{
	public class ProcessManager: MonoBehaviour, IProcessManager{
		void Awake(){
			thisRunningProcesses = new List<IProcess>();
			springCalculator = new NormalizedSpringValueCalculator(100);
		}
		void Update(){
			UpdateAllRegisteredProcesses(Time.deltaTime);
		}
		List<IProcess> thisRunningProcesses;
		public void AddRunningProcess(IProcess process){
			if(!thisRunningProcesses.Contains(process)){
				List<IProcess> newList = new List<IProcess>(thisRunningProcesses);
				newList.Add(process);
				thisRunningProcesses = newList;
			}
		}
		public void RemoveRunningProcess(IProcess process){
			List<IProcess> newList = new List<IProcess>(thisRunningProcesses);
			newList.Remove(process);
			thisRunningProcesses = newList;
		}
		public void UpdateAllRegisteredProcesses(float deltaTime){
			foreach(IProcess process in thisRunningProcesses){
				process.UpdateProcess(deltaTime);
			}
		}
		public bool RunningProcessesContains(IProcess process){
			return thisRunningProcesses.Contains(process);
		}
		/*  */
		public float quantityAnimationProcessExpireTime;
		public float GetQuantityAnimationProcessExpireTime(){
			return quantityAnimationProcessExpireTime;
		}
		public float imageEmptificationExpireTime;
		public float GetImageEmptificationExpireTime(){
			return imageEmptificationExpireTime;
		}
		public float visualPickednessProcessExpireTime;
		public float GetVisualPickednessProcessExpireTime(){
			return visualPickednessProcessExpireTime;
		}
		NormalizedSpringValueCalculator springCalculator;
		public float GetSpringT(float normalizedT){
			return springCalculator.GetSpringValue(normalizedT);
		}
		public float alphaActivatorUIEActivationProcessExpireT; 
		public float GetAlphaActivatorUIEActivationProcessExpireT(){
			return alphaActivatorUIEActivationProcessExpireT;
		}
		public float nonActivatorUIEActivationProcessExpireT;
		public float GetNonActivatorUIEActivationProcessExpireT(){
			return nonActivatorUIEActivationProcessExpireT;
		}
		public float scrollerElementSnapProcessDiffThreshold;
		public float GetScrollerElementSnapProcessDiffThreshold(){
			return scrollerElementSnapProcessDiffThreshold;
		}
		public float scrollerElementSnapSpringCoefficient;
		public float GetScrollerElementSnapSpringCoefficient(){
			return scrollerElementSnapSpringCoefficient;
		}
		public float scrollerElementSnapProcessStopDelta;
		public float GetScrollerElementSnapProcessStopDelta(){
			return scrollerElementSnapProcessStopDelta;
		}
		public float inertialScrollDeceleration;
		public float GetInertialScrollDeceleration(){
			return inertialScrollDeceleration;
		}
		public float imageColorTurnProcessExpireTime = .2f;
		public float GetImageColorTurnProcessExpireTime(){
			return imageColorTurnProcessExpireTime;
		}
		public float imageFlashTime = .4f;
		public float GetImageFlashTime(){
			return imageFlashTime;
		}
		public float alhpaPopUpExpireTime;
		public float GetAlphaPopUpExpireTime(){
			return alhpaPopUpExpireTime;
		}
		/*  */
		public float uiaWaitForTapProcesExpireTime;
		public float GetUIAWaitForTapProcessExpireTime(){
			return uiaWaitForTapProcesExpireTime;
		}
		public float uiaWaitForNextTouchProcessExpireTime;
		public float GetUIAWaitForNextTouchProcessExpireTime(){
			return uiaWaitForNextTouchProcessExpireTime;
		}
	}
}
