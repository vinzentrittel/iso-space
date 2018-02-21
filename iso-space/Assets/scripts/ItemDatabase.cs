using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class ItemDatabase : MonoBehaviour {

	/*********** Constants *********/
	// file name to item database, relative to '/Assets'
	private const string ITEM_FILE = "/StreamingAssets/Items.json";
	// identifier for item attributes
	private const string _ID = "id";
	private const string _NAME = "name";

	/*********** Members **********/
	private static Dictionary<int, Item> database;


	void Awake() {
		string filename = Application.dataPath + ITEM_FILE;

		database = InvokeDatabase(filename);
	}


	// fast access to Item
	// throws KeyNotFoundException, if ID is invalid
	public static Item FetchItemById(int id) {
		try {
		return database[id];
		} catch (KeyNotFoundException) {
			throw new KeyNotFoundException("ID: " + id + " was not found in ItemDatabase");
		}
	}


	// slow access to Item (don't use at in-game)
	// throws KeyNotFoundException, if 'name' is invalid
	public static Item FetchItemByName(string name) {
		// find the object with name 'name' and return it
		foreach(Item item in database.Values)
			if(item.Name == name)
				return item;

		// throw exception
		throw new KeyNotFoundException("'" + name + "' was not found in ItemDatabase");
	}

		
	// Reads in the JSON object from the destination 'Assets/'filename.
	// Every object in JSON file will be maps to an Item object.
	// The key value of the return object matches the value stored
	// in the _ID attribute.
	private Dictionary<int, Item> InvokeDatabase(string filename) {
		
		Item item;
		JsonData itemData;

		database = new Dictionary<int, Item>();
		// maps the whole Json structure to a generic object
		itemData = JsonMapper.ToObject(File.ReadAllText(filename));
		// map every object to an individual Item object, append the
		// <id, item> pair to database
		foreach(JsonData element in itemData) {
			// parse the current item
			item = new Item((int) element[_ID], (string) element[_NAME]);
			database.Add(item.ID, item);
		}

		// return database (redundant)
		return database;
	}

	public class Item {

		public const int INVALID_ID = 0;

		public int ID { get; private set; }
		public string Name { get; private set; }

		// Default constructor builds invalid Item
		// with Item.id = -1
		public Item() {
			this.ID = INVALID_ID;
		}

		public Item(int id, string name) {
			this.ID = id;
			this.Name = name;
		}

		public bool isValid() { return this.ID != INVALID_ID; }
	}
}


