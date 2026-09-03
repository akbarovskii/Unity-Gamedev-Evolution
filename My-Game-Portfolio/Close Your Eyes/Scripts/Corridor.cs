using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor : MonoBehaviour
{
    public GameObject startRoom;
    public GameObject[] possibleRooms;
    public int segmentsCount = 10;
    public GameObject lastCorridor;

    private Transform lastExitPoint;

    void Start()
    {
        CreateCorridor();
    }

    void CreateCorridor()
    {
        GameObject start = Instantiate(startRoom, Vector3.zero, Quaternion.identity, transform);
        Room startRoomScript = start.GetComponent<Room>();
        lastExitPoint = startRoomScript.exitPoint;

        for (int i = 0; i < segmentsCount - 1; i++)
        {
            GameObject roomPrefab = possibleRooms[Random.Range(0, possibleRooms.Length)];
            GameObject newRoom = Instantiate(roomPrefab, lastExitPoint.position, lastExitPoint.rotation, transform);
            Room newRoomScript = newRoom.GetComponent<Room>();
            Vector3 offset = newRoomScript.entryPoint.position - newRoom.transform.position;
            newRoom.transform.position = lastExitPoint.position - offset;
            lastExitPoint = newRoomScript.exitPoint;
        }

        GameObject lastRoom = Instantiate(lastCorridor, lastExitPoint.position, lastExitPoint.rotation, transform);
        Room lastRoomScript = lastRoom.GetComponent<Room>();
        Vector3 lastOffset = lastRoomScript.entryPoint.position - lastRoom.transform.position;
        lastRoom.transform.position = lastExitPoint.position - lastOffset;
    }
}
