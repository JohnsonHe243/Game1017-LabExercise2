using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

[XmlRoot("GameData")] // Allows us to set the name of the root tag.
public class GameData
{
    public int killCount;

    [XmlArray("TurretPositions")] // Allows us to set the name of the parent array tag.
    [XmlArrayItem("Position")] // Name of each child element in XML file.
    public List<Vector3> turretPositions;

    public GameData() // I like constructors. <3
    {
        killCount = 0;
        turretPositions = new List<Vector3>();
    }
}

public class ClickDragScript : MonoBehaviour
{
    [SerializeField] private GameObject turretPrefab;
    [SerializeField] private Transform spawnArea;
    // Drag data.
    private bool isDragging = false;
    private Vector2 offset;
    private Rigidbody2D currentlyDraggedObject;
    // GameObject data members.
    private List<GameObject> turrets;
    private GameData gameData;

    private void Start()
    {;
        turrets = new List<GameObject>();
        gameData = new GameData();
        // If the file exists. Fill in for LE2.
        if (File.Exists("gameData.xml"))
        {
            // Deserialze the XML file to the gameData.
            gameData = DeserializeFromXml();
            // Update the kill count UI, access the set kill count method of enemy spawner
            EnemySpawner.Instance.SetKillCount(gameData.killCount);
            // Spawn new turrets based on the loaded positions. Method call.
            SpawnTurrets(gameData.turretPositions);
        }

    }

    void SerializeToXml(GameData gameData)
    {   // From gameData object to xml
        // See Week 4 notes for help. Fill in for LE2.
        // Update the kill count into the data object.
        gameData.killCount = EnemySpawner.Instance.GetKillCount();
        // Clear the original turret positions in the GameData.
        gameData.turretPositions.Clear();
        // For each turret GameObject, store their position in the turret positions List.
        foreach (GameObject turret in turrets)
        {
            gameData.turretPositions.Add(turret.transform.position);
        }
        // Make an object of XmlSerializer.
        XmlSerializer serializer = new XmlSerializer(typeof(GameData));
        // Serialize the data using a StreamWriter object.
        using (StreamWriter streamWriter = new StreamWriter("gameData.xml"))
        {
            serializer.Serialize(streamWriter, gameData);
        }


    }

    private GameData DeserializeFromXml()
    {   // From xml
        // Create an XmlSerializer for the GameData type.
        XmlSerializer serializer = new XmlSerializer(typeof(GameData));
        // Deserialize the GameData from XML.
        using (StreamReader streamReader = new StreamReader("gameData.xml"))
        {
            return (GameData)serializer.Deserialize(streamReader);
        }
    }

    private void SpawnTurrets(List<Vector3> turretPositions)
    {
        foreach (Vector3 position in turretPositions)
        {
            // Spawn a new turret based on turretPositions.
            SpawnTurret(position);
        }
    }

    private void SpawnTurret(Vector3 position)
    {
        GameObject turretInst = GameObject.Instantiate(turretPrefab, position, Quaternion.identity);
        turrets.Add(turretInst);
        // optinonally add new turretPostion in gameData
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SpawnTurret(spawnArea.position);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Clear the kill count, turrets, and delete the XML file.
            gameData.killCount = 0; 
            EnemySpawner.Instance.SetKillCount(gameData.killCount);
            gameData.turretPositions.Clear(); // positions are not GameObjects in the scene.
            foreach (GameObject turret in turrets)
            {
                Destroy(turret);
            }
            turrets.Clear();
            DeleteFile();
        }
        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            // Raycast to check if the mouse is over a collider.
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null && hit.transform.gameObject.tag == "Turret")
            {
                // Check if the clicked GameObject has a Rigidbody2D.
                Rigidbody2D rb2d = hit.collider.GetComponent<Rigidbody2D>();
                if (rb2d != null)
                {
                    // Start dragging only if no object is currently being dragged.
                    isDragging = true;
                    currentlyDraggedObject = rb2d;
                    offset = rb2d.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Stop dragging.
            isDragging = false;
            currentlyDraggedObject = null;
        }

        if (isDragging && currentlyDraggedObject != null)
        {
            // Move the dragged GameObject based on the mouse position.
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentlyDraggedObject.MovePosition(mousePosition + offset);
        }
    }

    private void DeleteFile()
    {
        if (File.Exists("gameData.xml"))
        {
            File.Delete("gameData.xml");
            Debug.Log("XML file deleted.");
        }
    }

    private void OnApplicationQuit()
    {
        SerializeToXml(gameData);
    }
}
