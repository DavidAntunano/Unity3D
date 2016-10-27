using UnityEngine;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("XMLData")]
public class XMLManager : Singleton<XMLManager>
{
	protected XMLManager() { }

	XMLData xmlData = new XMLData();
	GameManager gameManager;
    string path; //UNITY 5.4 PATCHED

	void Awake() {
		gameManager = GameManager.Instance;
        path = Path.Combine(Application.dataPath, "SAVE.xml"); //SOLVED
    }

	void Save(XMLData xmlData) {
		XmlSerializer serializer = new XmlSerializer(typeof(XMLData));
		StreamWriter writer = new StreamWriter(path);
		serializer.Serialize(writer.BaseStream, xmlData);
		writer.Close();
	}

	XMLData Load() {
		if (!File.Exists(path)) {
			NewData();
		}
		XmlSerializer serializer = new XmlSerializer(typeof(XMLData));
		StreamReader reader = new StreamReader(path);
		XMLData deserialized = (XMLData)serializer.Deserialize(reader.BaseStream);
		reader.Close();
		return deserialized;
	}

	void NewData() {
		xmlData.gameData = "SAVE";
		xmlData.playerHealth = 100;
		xmlData.currentLevel = 0;
		xmlData.currentCheckPoint = 0;
		xmlData.gold = 0;

		xmlData.musicVolume = 0.75f;
		xmlData.effectsVolume = 0.80f;
		xmlData.vibration = false;
		xmlData.qualityLevel = 5;
		Save(xmlData);
		LoadData();
	}

    public void NewGame()
    {
        xmlData.gameData = "SAVE";
        xmlData.playerHealth = 100;
        xmlData.currentLevel = 1;  //0 is logo level
        xmlData.currentCheckPoint = 0;
        xmlData.gold = 0;

        xmlData.musicVolume = gameManager.preferences.musicVolume;
        xmlData.effectsVolume = gameManager.preferences.effectsVolume;
        xmlData.vibration = gameManager.preferences.vibration;
        xmlData.qualityLevel = gameManager.preferences.qualityLevel;

        Save(xmlData);
        LoadData();
    }

    public void LoadData() {
		xmlData = Load();

		gameManager.playerStatus.playerHealth = xmlData.playerHealth;
        gameManager.playerStatus.currentLevel = xmlData.currentLevel;
		gameManager.playerStatus.currentCheckPoint = xmlData.currentCheckPoint;
        gameManager.playerStatus.gold = xmlData.gold;

		gameManager.preferences.musicVolume = xmlData.musicVolume;
		gameManager.preferences.effectsVolume = xmlData.effectsVolume;
		gameManager.preferences.vibration = xmlData.vibration;
		gameManager.preferences.qualityLevel = xmlData.qualityLevel;
	}

	public void SaveData() {	
		xmlData.gameData = "SAVE";
		xmlData.playerHealth = gameManager.playerStatus.playerHealth;
        xmlData.currentLevel = gameManager.playerStatus.currentLevel;
		xmlData.currentCheckPoint = gameManager.playerStatus.currentCheckPoint;
        xmlData.gold = gameManager.playerStatus.gold;

        xmlData.musicVolume = gameManager.preferences.musicVolume;
		xmlData.effectsVolume = gameManager.preferences.effectsVolume;
		xmlData.vibration = gameManager.preferences.vibration;
		xmlData.qualityLevel = gameManager.preferences.qualityLevel;

		Save(xmlData);
	}
}