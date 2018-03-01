using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/* ItemPrefabRegister
 *  Singleton class for preloading prefabs and keeping them
 *  in memory
 */
public class ItemPrefabRegister {

	public const string PREFAB_PATH = "Assets/Prefabs/Items/";

	private static ItemPrefabRegister instance;				// for singleton
	private static Dictionary<string, GameObject> prefabs;
	private static List<string> sceduled;


	private ItemPrefabRegister() { 							// for singleton
		prefabs  = new Dictionary<string, GameObject>();
		sceduled = new List<string>();
	}

	public static ItemPrefabRegister GetInstance() {		// for singleton
		if(instance == null)
			return instance = new ItemPrefabRegister();
		return instance;
	} // end : GetInstance


	/* LoadPrefabs()
	 *  Load the object data to all sceduled prefab specifiers.
	 *  Prefab load is sceduled via AddPrefab()
	 */
	public bool LoadPrefabs() {
		GameObject loaded = null;
		bool       errors = false;
		// Append all sceduled loads to prefabs
		foreach(string prefab in sceduled) {
			// try to load from dictionary
			loaded = (GameObject) AssetDatabase.LoadAssetAtPath(
				PREFAB_PATH + prefab + ".prefab", typeof(GameObject));
			if(loaded == null) {
				errors = true;
				Debug.LogErrorFormat("Prefab '{0}' could not be loaded at '{1}'", prefab, PREFAB_PATH);
			}
			else {
				// if loading was successful appending for next round
				prefabs.Add(prefab, loaded);
				loaded = null;
			}
		}

		// return status
		return !errors;
	} // end : LoadPrefabs


	/* GetClone()
	 *  Return a copy of the passed prefab specifier
	 */
	public GameObject GetClone(string prefabName) {
		return GameObject.Instantiate(GetPrefab(prefabName));
	} // end : GetClone


	/* AddPrefab()
	 *  Scedules the loading of prefab with passed specifier from PREFAB_PATH
	 *  (see above). The prefab will only be loaded after next call of LoadPrefabs()
	 */
	public void AddPrefab(string prefabName) {
		sceduled.Add(prefabName);
	} // end : AddPrefab


	/* GetPrefab()
	 *  Returns the prefab with the passed specifier.
	 *  Note: this is the original prefab and no copy
	 *  For a copy call GetClone()
	 */
	public GameObject GetPrefab(string prefabName) {

		if(prefabs.ContainsKey(prefabName))
			return prefabs[prefabName];
		else
			Debug.LogErrorFormat("Prefab '{0}' was not loaded yet.", prefabName);

		return null;
	} // end : GetPrefab
} // end : ItemPrefabRegister
