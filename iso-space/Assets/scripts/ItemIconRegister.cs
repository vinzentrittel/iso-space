using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/* ItemIconRegister
 *  Singleton class for preloading sprites and keeping them
 *  in memory
 */
public class ItemIconRegister {

	private const string ICON_PATH = "Assets/Sprites/Items/";

	private static ItemIconRegister instance;				// for singleton
	private static Dictionary<string, Sprite> icons;
	private static List<string> sceduled;


	private ItemIconRegister() { 							// for singleton
		icons  = new Dictionary<string, Sprite>();
		sceduled = new List<string>();
	}

	public static ItemIconRegister GetInstance() {			// for singleton
		if(instance == null)
			return instance = new ItemIconRegister();
		return instance;
	} // end : GetInstance


	/* LoadIcons()
	 *  Load the image data to all sceduled icon specifiers.
	 *  Icon load is sceduled via AddIcon()
	 */
	public bool LoadIcons() {
		Sprite loaded = null;
		bool   errors = false;
		// Append all sceduled loads to icons
		foreach(string icon in sceduled) {
			// try to load from dictionary
			loaded = (Sprite) AssetDatabase.LoadAssetAtPath(
				ICON_PATH + icon + ".png", typeof(Sprite));
			if(loaded == null) {
				errors = true;
				Debug.LogErrorFormat("Icon '{0}' could not be loaded at '{1}'", icon, ICON_PATH);
			}
			else {
				// if loading was successful appending for next round
				icons.Add(icon, loaded);
				loaded = null;
			}
		}

		// return status
		return !errors;
	} // end : LoadIcons


	/* AddIcon()
	 *  Scedules the loading of icon with passed specifier from ICON_PATH
	 *  (see above). The icon will only be loaded after next call of LoadIcons()
	 */
	public void AddIcon(string iconName) {
		sceduled.Add(iconName);
	} // end : AddIcon


	/* GetIcon()
	 *  Returns the icon with the passed specifier.
	 */
	public Sprite GetIcon(string iconName) {

		if(icons.ContainsKey(iconName))
			return icons[iconName];
		else
			Debug.LogErrorFormat("Icon '{0}' was not loaded yet.", iconName);

		return null;
	} // end : GetIcon
} // end : ItemIconRegister
