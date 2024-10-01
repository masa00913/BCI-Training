using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using SFB;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    private string folderPath;

    private string filePath;
    private SaveData saveData;
    private string fileName;

    private Button selectFolderButton;

    private static SaveManager instance;

    private void Awake() {
        if (instance == null)
        {
            Debug.Log("こっち");
            folderPath = Application.dataPath + "/SaveData/FirstData";
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Debug.Log("あっち");
            
            Destroy(this.gameObject);
        }

        FirstSave();        
    }
    // Start is called before the first frame update
    void Start()
    {
        selectFolderButton = GameObject.Find("Canvas/Setting/SelectFileButton").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public string GetFolderPath(){
        return folderPath;
    }

    public void ChangeFolder(string folderPath){

        //保存先のフォルダを変更
        this.folderPath = folderPath;
        //フォルダを変更
        Save();
        SceneManager.LoadScene("TrainingScene");
        
    }

    



    public void FirstSave(){
        fileName =  "SaveManager" + ".json";
        filePath = Application.dataPath + "/SaveData/"  + fileName;
        Debug.Log(filePath);
        saveData = new SaveData();
        // ファイルがないとき、ファイル作成
        if (!File.Exists(filePath)) {
            Save();
        }

        // ファイルを読み込んでdataに格納
        saveData = Load(filePath);

        folderPath = saveData.folderPath;
    }

    //-------------------------------------------------------------------
    // jsonとしてデータを保存
    public void Save()
    {
        saveData.folderPath = folderPath;
        Debug.Log(folderPath + "に変更");
        string json = JsonUtility.ToJson(saveData,true);                 // jsonとして変換
        StreamWriter wr = new StreamWriter(filePath, false);    // ファイル書き込み指定
        wr.WriteLine(json);                                     // json変換した情報を書き込み
        wr.Flush();                                  
        wr.Close();                                             // ファイル閉じ
    }

    // jsonファイル読み込み
    SaveData Load(string path)
    {
        StreamReader rd = new StreamReader(path);               // ファイル読み込み指定
        string json = rd.ReadToEnd();                           // ファイル内容全て読み込む
        rd.Close();                                             // ファイル閉じる
                                                                
        return JsonUtility.FromJson<SaveData>(json);            // jsonファイルを型に戻して返す
    }

    [System.Serializable]
    public class SaveData{
        public string folderPath;
    }
    
}
