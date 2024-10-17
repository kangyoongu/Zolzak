using UnityEngine;
using System.Collections;

public class BlurControl : MonoBehaviour {
	
	float _value; 
	
	// Use this for initialization
	void Start () {
		_value = 0.0f;
		transform.GetComponent<Renderer>().material.SetFloat("_blurSizeXY",_value);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButton("Up"))
		{
			_value = _value + Time.deltaTime;
			if (_value>20f) _value = 20f;
			transform.GetComponent<Renderer>().material.SetFloat("_blurSizeXY",_value);
		}
		else if(Input.GetButton("Down"))
		{
			_value = (_value - Time.deltaTime) % 20.0f;
			if (_value<0f) _value = 0f;
			transform.GetComponent<Renderer>().material.SetFloat("_blurSizeXY",_value);
		}		
	}
	
	void OnGUI () {
		GUI.TextArea(new Rect(10f,10f,200f,50f), "Press the 'Up' and 'Down' arrows \nto interact with the blur plane\nCurrent value: "+_value);
		}
}
