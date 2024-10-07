using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TrainingButton : MonoBehaviour
{
    [SerializeField][Header("トレーニング番号")]private int trainingNum;

    //保存するデータ群
    [Header("Trainingが指定されているか")]private bool isConcludeTraining;
    [Header("Trainingの名前")]private string trainingName;
    [Header("何回Trainingするか")]private int trainingCount;
    [Header("トレーニングビデオのリンク")]private string trainingVideoLink;
    [Header("トレーニングの時間")]private float trainingVideoTime;
    [Header("現在の総トレーニング数")]private int currentTrainingNum;

    [Header("トレーニング回数のテキストオブジェ")]private TMP_Text trainingCountText;
    [Header("トレーニング回数のテキストオブジェ")]private TMP_Text trainingNameText;
    [Header("現在の総トレーニング数のテキスト")]private TMP_Text currentTrainingNumText;
    [Header("トレーニングのUIが入ったオブジェクト")]private GameObject trainingUIs;
    [Header("ごみ箱アイコンのオブジェ")]private GameObject trashObj;
    [Header("トレインボタンのオブジェ")]private GameObject trainObj;
    [Header("現在の総トレーニング数が入っているオブジェクト")]private GameObject currentTrainingNumObj;
    [Header("トレーニングを設定するボタン")]private Button trainingNameButton;
    private Image trainingColor;

    [Header("トレーニングを設定するパネルのオブジェ")]private TrainingSettingPanel trainingSettingPanel;

    [Header("トレーニングマネージャー")]private TrainingManager trainingManager;
    private SettingManager settingManager;
    private SaveManager saveManager;
    private bool canSetTraining;

     

    

    private string fileName;
    private string filePath;
    private SaveData saveData;

    
    // Start is called before the first frame update
    void Awake()
    {
        trainingNameText = transform.Find("TrainingNameButton/TrainingNameText").GetComponent<TMP_Text>();
        trainingCountText = transform.Find("TrainingCount").GetComponent<TMP_Text>();
        trainingUIs = transform.Find("TrainingUIs").gameObject;
        currentTrainingNumText = trainingUIs.transform.Find("CurrentTrainingNumBox/CurrentTrainingNum").GetComponent<TMP_Text>();
        
        currentTrainingNumObj = trainingUIs.transform.Find("CurrentTrainingNumBox").gameObject;
        trashObj = trainingUIs.transform.Find("TrashButton").gameObject;
        trainObj = trainingUIs.transform.Find("TrainButton").gameObject;
        trainingSettingPanel = GameObject.Find("Canvas/Setting").transform.Find("TrainingSettingObj").GetComponent<TrainingSettingPanel>();

        trainingManager = GameObject.Find("TrainingManager").GetComponent<TrainingManager>();
        settingManager = GameObject.Find("SettingManager").GetComponent<SettingManager>();

        trainingNameButton = transform.Find("TrainingNameButton").GetComponent<Button>();
        trainingColor = transform.Find("TrainingColor").GetComponent<Image>();
        

        saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        
    }

    private void Start() {
        FirstSave();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void AddCurrentTrainingNum(){
        this.currentTrainingNum++;
        currentTrainingNumText.text = currentTrainingNum.ToString();
        Save();
    }

    /// <summary>
    /// トレーニングを設定できるか設定
    /// 
    /// </summary>
    /// <param name="canSetTraining"></param>
    public void SetCanSetTraining(bool canSetTraining){
        this.canSetTraining = canSetTraining;
        if(canSetTraining){
            trainingCountText.gameObject.SetActive(true);
            this.trainingCountText.text = "+";
            trainingNameButton.interactable = true;
        }else{
            if(trainingNum < settingManager.GetTrainingTypeNum()){
                trainingNameButton.interactable = true;
            }else{  
                trainingCountText.gameObject.SetActive(false);
                trainingNameButton.interactable = false;
            }
        }   
    }

    /// <summary>
    /// トレーニングの識別番号を取得
    /// </summary>
    /// <returns></returns>
    public int GetTrainingNum(){
        return trainingNum;
    }

    /// <summary>
    /// トレーニングビデオの長さ
    /// </summary>
    /// <returns></returns>
    public float GetTrainingVideoTime(){
        return trainingVideoTime;
    }

    public void SetTrainingVideoTime(float videoTime){
        trainingVideoTime = videoTime;
        Save();
    }

    /// <summary>
    /// トレーニングの情報を設定
    /// </summary>
    /// <param name="trainingName"></param>
    /// <param name="trainingCount"></param>
    /// <param name="trainingVideoLink"></param>
    public void SetTrainingInfo(string trainingName, string trainingCount,string trainingVideoLink){
        if(isConcludeTraining){
            //既にデータが存在し、更新する場合
            
        }else{
            //新しくデータを入れる場合
            if(canSetTraining){
                settingManager.SetTrainingTypeNum(trainingNum+1);
            }
        }

        this.isConcludeTraining = true;
        this.trainingName = trainingName;
        this.trainingNameText.text = trainingName;
        this.trainingCount = int.Parse(trainingCount);
        this.trainingCountText.text = trainingCount;
        this.trainingCountText.gameObject.SetActive(true);
        
        this.trainingVideoLink = trainingVideoLink;
        this.currentTrainingNum = 0;
        trainingUIs.SetActive(true);
        Save();
    }

    /// <summary>
    /// トレーニングの情報を設定
    /// </summary>
    /// <param name="trainingName"></param>
    /// <param name="trainingCount"></param>
    /// <param name="trainingVideoLink"></param>
    public void SetTrainingInfo(TrainingButton trainingButton){
        if(isConcludeTraining){
            //既にデータが存在し、更新する場合
            
        }else{
            //新しくデータを入れる場合
            if(canSetTraining){
                settingManager.SetTrainingTypeNum(trainingNum+1);
            }
        }

        this.isConcludeTraining = true;
        this.trainingName = trainingButton.GetTrainingName();
        this.trainingNameText.text = trainingName;
        this.trainingCount = trainingButton.GetTrainingCount();
        this.trainingCountText.text = this.trainingCount.ToString();
        this.trainingCountText.gameObject.SetActive(true);
        trainingUIs.SetActive(true);
        this.trainingVideoLink = trainingButton.GetTrainingVideoLink();
        this.trainingColor.color = trainingButton.GetTrainingColor();
        this.trainingVideoTime = trainingButton.GetTrainingVideoTime();
        this.currentTrainingNum = trainingButton.GetCurrentTrainingNum();
        this.currentTrainingNumText.text = this.currentTrainingNum.ToString();
        Save();
    }

    public bool GetIsConcludeTraining(){
        return isConcludeTraining; 
    }

    public void SetTrainingColor(Color color){
        trainingColor.color = color;
    }

    

    public string GetTrainingName(){
        return trainingName;
    }

    public int GetTrainingCount(){
        return trainingCount;
    }

    public string GetTrainingVideoLink(){
        return trainingVideoLink;
    }

    public Color GetTrainingColor(){
        return trainingColor.color;

    }

    public int GetCurrentTrainingNum(){
        return currentTrainingNum;
    }

    /// <summary>
    /// トレーニングの名前が書いてあるボタンを押したときの処理
    /// </summary>
    public void OnClickTrainingNameButton(){
        settingManager.SetActiveTrainingButton(this);
        trainingSettingPanel.SetTrainingButton(this);
        trainingSettingPanel.gameObject.SetActive(true);
        if(isConcludeTraining){
            trainingSettingPanel.SetTrainingName(trainingName);
            trainingSettingPanel.SetTrainingCount(trainingCount);
            trainingSettingPanel.SetTrainingVideoLink(trainingVideoLink);
        }
    }

    /// <summary>
    /// ごみ箱アイコンを押したときの処理
    /// </summary>
    public void OnClickTrashButton(){
        Debug.Log("ごみ箱ボタン押した＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝");
        settingManager.DeleteEmptyColorNum(trainingColor.color);
        DeleteInfo();
        settingManager.DeleteProcess(this);
        Save();
    }

    public void OnClickResetButton(){
        currentTrainingNum = 0;
        currentTrainingNumText.text = "0";
    }

    public void DeleteInfo(){
        isConcludeTraining = false;
        trainingName = "None";
        currentTrainingNum = 0;
        currentTrainingNumText.text = "0";
        this.trainingNameText.text = trainingName;
        trainingUIs.SetActive(false);

        trainingColor.color = Color.gray;

        Save();
        Debug.Log(trainingNum + "をNoneにした");
    }

    /// <summary>
    /// トレインボタンを押したときの処理
    /// </summary>
    public void OnClickTrainButton(){
        trainingManager.StartTraining(trainingName, trainingVideoLink, trainingVideoTime,trainingCount,trainingNum);
    }

    public void ResetInfo(){
        this.isConcludeTraining = false;
        this.trainingCount = 0;
        this.trainingName = "None";
        this.trainingVideoLink = null;
        this.canSetTraining = false;
        this.trainingColor.color = Color.gray;
        this.trainingVideoTime = 0;
        UpdateData();
    }


    /// <summary>
    /// 最初のセーブとロード
    /// </summary>
    public void FirstSave(){
        // fileName = fileName + name + ".json";
        fileName =  "TrainingData" + trainingNum + ".json";
        filePath = saveManager.GetFolderPath() + "/" + fileName;
        saveData = new SaveData();
        // ファイルがないとき、ファイル作成
        if (!File.Exists(filePath)) {
            Save();
        }

        // ファイルを読み込んでdataに格納
        saveData = Load(filePath);

        this.isConcludeTraining = saveData.isConcludeTraining;
        this.trainingCount = saveData.trainingCount;
        this.trainingName = saveData.trainingName;
        this.trainingVideoLink = saveData.trainingVideoLink;
        this.canSetTraining = saveData.canSetTraining;
        trainingColor.color = saveData.trainingColor;
        this.trainingVideoTime = saveData.trainingVideoTime;
        currentTrainingNum = saveData.currentTrainingNum;
        UpdateData();
    }


    /// <summary>
    /// 最初に更新するべき要素を更新
    /// </summary>
    private void UpdateData(){
        if(isConcludeTraining){
            this.trainingNameText.text = trainingName;
            this.trainingCountText.text = trainingCount.ToString();
            this.currentTrainingNumText.text = currentTrainingNum.ToString();
            trainingUIs.SetActive(true);
        }else{
            this.trainingNameText.text = "None";
            this.trainingCountText.text = "+";
            trainingUIs.SetActive(false);
            trainingColor.color = Color.gray;
        }

        
    }

    //-------------------------------------------------------------------
    // jsonとしてデータを保存
    public void Save()
    {
        saveData.isConcludeTraining = this.isConcludeTraining;
        saveData.trainingCount = this.trainingCount;
        saveData.trainingName = this.trainingName;
        saveData.trainingVideoLink = this.trainingVideoLink;
        saveData.canSetTraining = this.canSetTraining;
        saveData.trainingColor = trainingColor.color;
        saveData.trainingVideoTime = trainingVideoTime;
        saveData.currentTrainingNum = currentTrainingNum;
        string json = JsonUtility.ToJson(saveData,true);                 // jsonとして変換
        StreamWriter wr = new StreamWriter(filePath, false);    // ファイル書き込み指定
        wr.WriteLine(json);                                     // json変換した情報を書き込み
        wr.Flush();                                  
        wr.Close();                                             // ファイル閉じ
    }

    // jsonファイル読み込み
    private SaveData Load(string path)
    {
        StreamReader rd = new StreamReader(path);               // ファイル読み込み指定
        string json = rd.ReadToEnd();                           // ファイル内容全て読み込む
        rd.Close();                                             // ファイル閉じる
                                                                
        return JsonUtility.FromJson<SaveData>(json);            // jsonファイルを型に戻して返す
    }

    [System.Serializable]
    public class SaveData{
        [Header("Trainingが指定されているか")]public bool isConcludeTraining;
        [Header("Trainingの名前")]public string trainingName;
        [Header("何回Trainingするか")]public int trainingCount;
        [Header("トレーニングビデオのリンク")]public string trainingVideoLink;
        [Header("トレーニングを設定できるか")]public bool canSetTraining;
        [Header("トレーニングカラー")]public Color trainingColor;
        [Header("トレーニングビデオの長さ")]public float trainingVideoTime;
        [Header("現在の総トレーニング数")]public int currentTrainingNum;
    }
}
