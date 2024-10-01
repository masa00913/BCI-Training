using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UdpSender : MonoBehaviour
{
    private UdpClient udpClient;
    public string ipAddress = "127.0.0.1"; // 受信側のIPアドレス
    public int port = 12345; // 受信側のポート番号

    void Start()
    {
        // UdpClientを初期化
        udpClient = new UdpClient();
    }

    void Update()
    {
        // スペースキーが押されたらメッセージを送信
        // if(Input.GetKeyDown(KeyCode.Alpha1)){
        //     SendMessages(1);
        // }else if(Input.GetKeyDown(KeyCode.Alpha2)){
        //     SendMessages(2);
        // }else if(Input.GetKeyDown(KeyCode.Alpha3)){
        //     SendMessages(3);
        // }else if(Input.GetKeyDown(KeyCode.Alpha4)){
        //     SendMessages(4);
        // }else if(Input.GetKeyDown(KeyCode.Alpha5)){
        //     SendMessages(5);
        // }
    }

    public void SendMessages(string message)
    {
        // 文字列をバイト配列に変換
        byte[] data = Encoding.UTF8.GetBytes(message);
        // データを送信
        udpClient.Send(data, data.Length, ipAddress, port);
        Debug.Log("Message sent: " + message);
    }

    public void SendMessages(int message)
    {
        // 文字列をバイト配列に変換
        byte[] data = BitConverter.GetBytes(message);
        // データを送信
        udpClient.Send(data, data.Length, ipAddress, port);
        Debug.Log("Message sent: " + message);
    }

    public void SetPortNum(int port){
        this.port = port;
    }

    void OnApplicationQuit()
    {
        // アプリケーション終了時にUdpClientを閉じる
        udpClient.Close();
    }
}
