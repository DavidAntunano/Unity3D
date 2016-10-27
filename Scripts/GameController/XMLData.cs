using System.Xml.Serialization;

public class XMLData{
	[XmlElement("gameData")]
	public string gameData;
	[XmlElement("playerHealth")]
	public int playerHealth;
    [XmlElement("currentLevel")]
	public int currentLevel;
	[XmlElement("currentCheckPoint")]
	public int currentCheckPoint;
	[XmlElement("gold")]
	public int gold;
	[XmlElement("effectsVolume")]
	public float effectsVolume;
	[XmlElement("musicVolume")]
	public float musicVolume;
	[XmlElement("vibration")]
	public bool vibration;
	[XmlElement("qualityLevel")]
	public int qualityLevel;
}