using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UdpReceiver : MonoBehaviour
{
    private UdpClient udpClient;
    private Thread receiveThread;
    public int port = 12345; // 受信側のポート番号

    void Start()
    {
        
    }

    public void StartReceive(){
        // UdpClientを指定したポート番号で初期化
        udpClient = new UdpClient(port);
        // データ受信用のスレッドを開始
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log("UDP receiver started");
    }

    void ReceiveData()
    {
        // 受信するエンドポイントを指定
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        while (true)
        {
            try
            {
                // データを受信
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                int message = data[0];
                Debug.Log(message);
            }
            catch (Exception e)
            {
                Debug.LogError("Error receiving data: " + e.Message);
            }
        }
    }

    public void SetPortNum(int port){
        this.port = port;
    }

    void OnApplicationQuit()
    {
        if(!(receiveThread != null && udpClient != null)) return;
        // アプリケーション終了時にスレッドとUdpClientを閉じる
        receiveThread.Abort();
        udpClient.Close();
    }
}
