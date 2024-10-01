using System.Collections;
using System.Collections.Generic;
using System.IO;
using SFB;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class SettingManager : MonoBehaviour
{

    [Header("トレーニングの種類の数")]private int trainingTypeNum;

    private TrainingButton[] trainingButtons = new TrainingButton[5];

    private Color[] colors = new Color[5];
    private int[] colorNumEmptyInfo = new int[5];

    private TrainingButton activeTrainingButton;
    private VideoPlayer videoPlayer;
    private bool needToSetVideoTime;

    private SaveManager saveManager;
    private WarningManager warningManager;

    private string fileName;
    private string filePath;
    private SaveData saveData;

    // Start is called before the first frame update
    void Start()
    {
        var canavs = GameObject.Find("Canvas");
        var settingUIObj = canavs.transform.Find("Setting").gameObject;

        for(int i=0; i<trainingButtons.Length; i++){
            trainingButtons[i] = settingUIObj.transform.Find("Buttons/Button" + i.ToString()).GetComponent<TrainingButton>();
        }

        colors[0] = Color.blue;
        colors[1] = Color.green;
        colors[2] = Color.red;
        colors[3] = Color.yellow;
        colors[4] = Color.magenta;

        videoPlayer = GameObject.Find("VideoPlayer").GetComponent<VideoPlayer>();
        videoPlayer.prepareCompleted += PrepareProcess;
        saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        warningManager = GameObject.Find("WarningManager").GetComponent<WarningManager>();
        FirstSave();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 動画の準備の処理
    /// </summary>
    /// <param name="videoPlayer"></param>
    private void PrepareProcess(VideoPlayer videoPlayer){
        Debug.Log(videoPlayer.length);
        var trainingVideoTime = (float)videoPlayer.length;
        if(activeTrainingButton != null && needToSetVideoTime){
            activeTrainingButton.SetTrainingVideoTime(trainingVideoTime);
            needToSetVideoTime = false;
        }
    }

    public void SetNeedToSetVideoTime(bool needToSetVideoTime){
        this.needToSetVideoTime = needToSetVideoTime;
    }

    /// <summary>
    /// どれかのトレーニングを消した際の処理
    /// </summary>
    public void DeleteProcess(TrainingButton trainingButton){
        SetTrainingTypeNum(trainingTypeNum-1);
        DeleteEmptyColorNum(trainingButton.GetTrainingColor());
        if(trainingButton.GetTrainingNum() == trainingTypeNum){
            Debug.Log("一番下を消した");
           
        }else{
            var deleteNum = trainingButton.GetTrainingNum();
            Debug.Log("中途半端なところを消した");
            for(int i=deleteNum; i<trainingTypeNum; i++){
                Debug.Log(i+1 + "から" + i + "番目に移動");
                trainingButtons[i].SetTrainingInfo(trainingButtons[i+1]);
            }
            trainingButtons[trainingTypeNum].DeleteInfo();
        }
        Save();
    }

    public void SetActiveTrainingButton(TrainingButton activeButton){
        activeTrainingButton = activeButton;
    }

    public TrainingButton GetActiveTrainingButton(){
        return activeTrainingButton;
    }
    

    /// <summary>
    /// 空いているカラー番号を取得
    /// </summary>
    /// <returns></returns>
    private int GetEmptyColorNum(){
        var num = 0;
        for(int i=0; i<colorNumEmptyInfo.Length; i++){
            num = i;
            if(colorNumEmptyInfo[i] == 0){
                colorNumEmptyInfo[i] = 1;
                break;
            }
        }
        Debug.Log(num + "が開いている");
        Save();
        return num;
    }

    /// <summary>
    /// 空いているカラーを取得
    /// </summary>
    /// <returns></returns>
    public Color GetEmptyTrainingColor(){
        return colors[GetEmptyColorNum()];
    }

    /// <summary>
    /// 空いているカラー番号を設定する
    /// </summary>
    /// <param name="trainingColor"></param>
    public void DeleteEmptyColorNum(Color trainingColor){
        for(int i=0; i<colors.Length; i++){
            if(colors[i] == trainingColor){
                colorNumEmptyInfo[i] = 0;
                Debug.Log(i + "番目を0にして消した");
                break;
            }
        }
        Save();
    }    

    /// <summary>
    /// トレーニングが何種類登録されているか取得
    /// </summary>
    /// <returns></returns>
    public int GetTrainingTypeNum(){
        return trainingTypeNum;
    }

    

    /// <summary>
    /// トレーニングの種類の数を設定し、トレーニングを受け付けられるようにする
    /// </summary>
    /// <param name="trainingTypeNum"></param>
    public void SetTrainingTypeNum(int trainingTypeNum){
        
        this.trainingTypeNum = trainingTypeNum;
        if(this.trainingTypeNum < 0){
            this.trainingTypeNum = 0;
        }
        Debug.Log(this.trainingTypeNum + "種類のトレーニングが存在");
        for(int i=0; i<trainingButtons.Length; i++){
            if(i == this.trainingTypeNum){
                trainingButtons[i].SetCanSetTraining(true);
            }else{
                trainingButtons[i].SetCanSetTraining(false);
            }
        }
        Save();
    }

    public string GetTrainingName(int trainingNum){
        return trainingButtons[trainingNum].GetTrainingName();
    }

    public string GetTrainingVideoLink(int trainingNum){
        return trainingButtons[trainingNum].GetTrainingVideoLink();
    } 

    public float GetTrainingVideoTime(int trainingNum){
        return trainingButtons[trainingNum].GetTrainingVideoTime();
    }

    public void OnClickSelectFileButton(){
        var saveDataFolder = Application.dataPath + "/SaveData";
        var paths = StandaloneFileBrowser.OpenFolderPanel("Select Folder", saveDataFolder,false);
        
        if (paths.Length > 0)
        {

            string folderPath = paths[0];
            folderPath = folderPath.Replace("\\","/");
            if (folderPath.StartsWith(saveDataFolder + "/"))
            {
                // ここでフォルダ内の操作を行う
                saveManager.ChangeFolder(folderPath);
            }
            else
            {
                Debug.LogWarning("指定されたフォルダの外です。ルートフォルダ内のフォルダを選択してください。");
                warningManager.ShowWarningText("Outside of the SaveData folder; select the folder in the SaveFolder.");
                // 必要に応じて警告メッセージをユーザーに表示
            }
            
        }
    }

    public void FirstSave(){
        // fileName = fileName + name + ".json";
        fileName =  "SettingManagerData" + ".json";
        filePath = saveManager.GetFolderPath() + "/" + fileName;
        Debug.Log(filePath);
        saveData = new SaveData();
        // ファイルがないとき、ファイル作成
        if (!File.Exists(filePath)) {
            Save();
        }

        // ファイルを読み込んでdataに格納
        saveData = Load(filePath);

        this.trainingTypeNum = saveData.trainingTypeNum;
        this.colorNumEmptyInfo = saveData.colorNumEmptyInfo;

        UpdateData();
    }

    private void UpdateData(){
        SetTrainingTypeNum(trainingTypeNum);
    }

    //-------------------------------------------------------------------
    // jsonとしてデータを保存
    public void Save()
    {
        saveData.trainingTypeNum = trainingTypeNum;
        saveData.colorNumEmptyInfo = colorNumEmptyInfo;
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
        [Header("トレーニングの種類の数")] public int trainingTypeNum;
        [Header("どのカラーが開いているか")]public int[] colorNumEmptyInfo = new int[5];
    }
}
