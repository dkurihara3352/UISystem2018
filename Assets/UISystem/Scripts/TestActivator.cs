using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISystem;
public class TestActivator : MonoBehaviour {

	public UIAdaptor rootUIAdaptor;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey("space"))
			rootUIAdaptor.ActivateUIElement();
	}
}
