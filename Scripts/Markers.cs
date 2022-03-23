using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Net.Sockets;
using System.Net;

public class Markers : MonoBehaviour
{
    TcpListener server;
    public string logger;
    public GameObject prefab;
    public Material materialLine;
    public GameObject[] pMarkers;
    public GameObject[] lMarkers;
    public Int32 port = 8010;
    Byte[] bytes;
    String data;
    Vector3 [] markers;

    void Rays(int startIndex, int endIndex){
        LineRenderer lr = pMarkers[startIndex].GetComponent<LineRenderer>();
        Vector3 [] positions = new Vector3[]{
            pMarkers[startIndex].transform.position, 
            pMarkers[endIndex].transform.position
        };
        lr.SetPositions(positions);
    }

    void ServerListen()
    {
        // double[,] markers = new double[33, 4];
        for(;;){
            TcpClient client = server.AcceptTcpClient();
            // 132
            NetworkStream stream = client.GetStream();

            while((stream.Read(bytes, 0, bytes.Length)) != 0){}
            
            for(int i = 0; i < 33; i++){
                markers[i].x = (float)BitConverter.ToDouble(bytes, 4*(8 * i) + 8);
                markers[i].y = (float)BitConverter.ToDouble(bytes, 4*(8 * i) + 16);
                markers[i].z = (float)BitConverter.ToDouble(bytes, 4*(8 * i) + 24);
            }
            client.Close();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        pMarkers = new GameObject[33];
        for(int i = 0; i < 33; i++){
            pMarkers[i] = Instantiate(prefab, Vector3.zero, Quaternion.identity, gameObject.transform);
            pMarkers[i].name = $"Marker{i:D2}";
        }

        lMarkers = new GameObject[35];
        for(int i = 0; i < 35; i++){
            lMarkers[i] = new GameObject();
            // lMarkers[i] = Instantiate(empty, Vector3.zero, Quaternion.identity, gameObject.transform);
            lMarkers[i].name = $"Line Marker{i:D2}";
            lMarkers[i].transform.SetParent(gameObject.transform);
            LineRenderer lr = lMarkers[i].AddComponent<LineRenderer>();
            lr.startWidth = 0.01f;
            lr.endWidth = 0.01f;
            lr.material = materialLine;
        }

        Thread serverThread;
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        markers = new Vector3[33];

        server = new TcpListener(localAddr, port);
        server.Start();
        bytes = new Byte[256*256];
        serverThread = new Thread(ServerListen);
        serverThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < 33; i++){
            pMarkers[i].transform.position = - new Vector3(
                markers[i].x,
                markers[i].y - 1,
                markers[i].z
            );
        }
        

        int [,] indexList = new int[,]{
            {8, 6}, {6, 5}, {5, 4}, {4, 0}, {0, 1}, {1, 2},
            {2, 3}, {3, 7}, {9, 10}, {11, 12}, {11, 23},
            {12, 24}, {12, 14}, {14, 16}, {16, 18}, {18, 20},
            {16, 20}, {16, 22}, {23, 24}, {11, 13}, {13, 15},
            {15, 17}, {15, 21}, {15, 19}, {17, 19}, {23, 25}, 
            {25, 27}, {27, 29}, {27, 31}, {29, 31}, {24, 26}, 
            {26, 28}, {28, 30}, {28, 32}, {30, 32}
        };

        for(int i = 0; i < 35; i++){
            int startIndex = indexList[i, 0];
            int endIndex = indexList[i, 1];
            LineRenderer lr = lMarkers[i].GetComponent<LineRenderer>();
            Vector3 [] positions = new Vector3[]{
                pMarkers[startIndex].transform.position,
                pMarkers[endIndex].transform.position
            };
            lr.SetPositions(positions);
        }

        // Rays(11, 12);
        // Rays(12, 24);
        // Rays(11, 23);
        // Rays(23, 24);
        // Rays(24, 26);
        // Rays(23, 25);
        // Rays(26, 28);
        // Rays(28, 30);
        // Rays(28, 32);
        // Rays(30, 32);
        // Rays(25, 27);
        // Rays(27, 29);
        // Rays(27, 31);
        // Rays(29, 31);
        // Rays(12, 14);
        // Rays(14, 16);
        // Rays(16, 22);
        // Rays(16, 20);
        // Rays(16, 18);
        // Rays(18, 20);
        // Rays(11, 13);
        // Rays(13, 15);
        // Rays(15, 21);
        // Rays(15, 19);
        // Rays(15, 17);
        // Rays(17, 19);
        // Rays(9, 10);
        // Rays(8, 6);
        // Rays(6, 5);
        // Rays(5, 4);
        // Rays(4, 0);
        // Rays(0, 1);
        // Rays(1, 2);
        // Rays(2, 3);
        // Rays(3, 7);
    }
}