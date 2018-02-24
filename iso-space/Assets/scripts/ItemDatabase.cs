using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

/* ItemDatabase
 *  Loads item data from Json database. Corresponding prefabs
 *  will be loaded via ItemPrefabRegister.
 */
public class ItemDatabase : MonoBehaviour {

	/*********** Constants *********/
	// file name to item database, relative to '/Assets'
	private const string ITEM_FILE = "/StreamingAssets/Items.json";
	// identifier for item attributes
	private const string _ID = "id";
	private const string _NAME = "name";
	private const string _HANDLE = "handle";
	private const string _PREFAB = "prefab";
	private const string _DESCRIPTION = "descr";

	/*********** Members **********/
	private static Dictionary<int, Item> database;


	void Awake() {
		bool   success;
		string filename = Application.dataPath + ITEM_FILE;

		database = InvokeDatabase(filename);

		// load every prefab data to the database
		foreach(Item item in database.Values) {
			ItemPrefabRegister.GetInstance().AddPrefab(item.Prefab);
			ItemIconRegister.GetInstance().AddIcon(item.Handle);
		}
		success = ItemPrefabRegister.GetInstance().LoadPrefabs();
		success &= ItemIconRegister.GetInstance().LoadIcons();
		if(!success)
			Debug.Log("Some item prefabs could not be loaded. See Error Log!");
	} // end : Awake


	/* FetchItemById()
	 *  Very fast access to item
	 *  throws KeyNotFoundException, if id is invalid
	 */
	public static Item FetchItemById(int id) {
		try {
			return database[id];
		} catch (KeyNotFoundException) {
			throw new KeyNotFoundException("ID: " + id + " was not found in ItemDatabase");
		}
	} // end : FetchItemById()


	/* FetchItemByName()
	 *  Slow access to item, searches in name and handle for the passed
	 *  parameter. Only use in development stage or on loading screen.
	 *  Use FetchItemById in time critical situations.
	 *  throws KeyNotFoundException, if 'name' is invalid
	 */
	public static Item FetchItemByName(string name) {
		// find the object with name, or handle 'name' and return it
		foreach(Item item in database.Values)
			if(item.Name == name || item.Handle == name)
				return item;

		// throw exception
		throw new KeyNotFoundException("'" + name + "' was not found in ItemDatabase");
	}


	/* InvokeDatabase()
	 *  Reads in the JSON object from the destination 'Assets/' filename.
	 *  Every object in JSON file will be mapped to an Item object.
	 *  The key value of the return object matches the value stored
	 *  in the _ID attribute.
	 */
	private Dictionary<int, Item> InvokeDatabase(string filename) {
		
		Item item;
		JsonData itemData;
		database = new Dictionary<int, Item>();

		try {
			// maps the whole Json structure to a generic object
			itemData = JsonMapper.ToObject(File.ReadAllText(filename));

			// map every object to an individual Item object, append the
			// <id, item> pair to database
			foreach(JsonData element in itemData) {
				// map the current item
				item = new Item(
					(int)    element[_ID],
					(string) element[_NAME],
					(string) element[_HANDLE],
					(string) element[_PREFAB],
					(string) element[_DESCRIPTION]
				);
				database.Add(item.ID, item);
			}
		} catch (JsonException) {
			Debug.LogErrorFormat("ItemDatabase could not be mapped. Check '{0}' for syntax errors", filename);
		} catch (FileNotFoundException) {
			Debug.LogErrorFormat("ItemDatabase could not be mapped. '{0}' does not exist", filename);
		}

		// return database (redundant)
		return database;
	} // end : InvokeDatabase


	/* Item
	 *  Storage for item data loaded from the database
	 */
	public class Item {

		public const int INVALID_ID = 0;

		public int ID { get; private set; }
		public string Name { get; private set; }
		public string Handle { get; private set; }
		public string Prefab { get; private set; }
		public string Description { get; private set; }

		// Default constructor builds invalid Item
		// with Item.id = -1
		public Item() {
			this.ID = INVALID_ID;
		}
			
		public Item(int id, string name, string handle, string prefab, string description) {
			this.ID = id;
			this.Name = name;
			this.Handle = handle;
			this.Prefab = prefab;
			this.Description = description;
		}
			

		public bool isValid() { return this.ID != INVALID_ID; }
	} // end : Item
} // end : ItemDatabase


