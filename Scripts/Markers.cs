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
    public GameObject[] p_markers;
    public Int32 port = 8010;
    Byte[] bytes;
    String data;
    Vector3 [] markers;

    void serverListen()
    {
        // double[,] markers = new double[33, 4];
        for(;;){
            TcpClient client = server.AcceptTcpClient();
            // 132
            NetworkStream stream = client.GetStream();

            // Debug.Log(bytes.Length);

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
        p_markers = new GameObject[33];
        for(int i = 0; i < 33; i++)
            p_markers[i] = Instantiate(prefab) as GameObject;

        Thread serverThread;
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        markers = new Vector3[33];

        server = new TcpListener(localAddr, port);
        server.Start();
        bytes = new Byte[256*256];
        serverThread = new Thread(serverListen);
        serverThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < 33; i++){
            p_markers[i].transform.position = - new Vector3(
                markers[i].x,
                markers[i].y - 1,
                markers[i].z
            );
        }
    }
}