using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CleanUpAll : MonoBehaviour {

	// Use this for initialization
	void OnEnable () {
		//CleanupAll ();
	}


	public void CleanupScene(){
		OptionsDisplay[] optionsDisplay = FindObjectsByType<OptionsDisplay>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
		foreach (OptionsDisplay option in optionsDisplay) {
			option.GOCleanup();
		}
	}
}
