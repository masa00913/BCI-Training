using System.Collections;
using System.Collections.Generic;
using System.IO;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TrainingManager : MonoBehaviour
{
    [Header("ビデオプレイヤー")]private VideoPlayer videoPlayer;
    [Header("トレーニング中か")]private bool isTraining;
    [Header("キャンバス")]private GameObject canavs;
    [Header("トレーニング設定用のUIのオブジェ")]private GameObject settingUIObj;
    [Header("トレーニング用のUIのオブジェ")]private GameObject trainingUIObj;
    [Header("レティクルのオブジェ")]private GameObject reticleObj;
    [Header("トレーニングの名前を表示するTMP")]private TMP_Text trainingNameText;

    [Header("安静が何秒か")]private float restTime = 2f;
    [Header("指示が何秒か")]private float instituteTime = 1f;
    [Header("合図が何秒か")]private float signTime = 1f;

    [Header("トレーニングビデオの時間")]private float trainingVideoTime;
    [Header("全部のトレーニングビデオの時間")]private float[] allTrainingVideoTime;
    [Header("トレーニングの経過時間")]private float trainingTime;
    [Header("何回トレーニングしたか")]private int trainingCount;
    [Header("何回トレーニングするか")]private int maxTrainingCount;
    [Header("全部のトレーニングの名前")]private string[] allTrainingName;
    [Header("全部のトレーニングのビデオリンク")]private string[] allTraiingVideoLink;

    [Header("全体トレーニングを何回繰り返すか")]private int allTrainNum;
    [Header("トレーニングの順番")]private int[] trainOrder;

    [Header("全体のトレーニングか")]private bool isAllTrain;

    [Header("SendのInputField")]private TMP_InputField sendPortInputField;
    [Header("ReceiveのInputField")]private TMP_InputField receivePortInputField;
    [Header("全体を何回やるか")]private TMP_InputField allTrainingNumInputField;
    private UdpSender udpSender;
    private UdpReceiver udpReceiver;
    
    private WarningManager warningManager;
    private SettingManager settingManager;
    private SaveManager saveManager;

    [Header("どのファイルを選んでいるか表示")]private TMP_Text selectFileNameText;


    private string fileName;
    private string filePath;
    private SaveData saveData;
    

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GameObject.Find("VideoPlayer").GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += LoopPointReached;
        
        videoPlayer.source = VideoSource.Url;
        canavs = GameObject.Find("Canvas");
        settingUIObj = canavs.transform.Find("Setting").gameObject;
        trainingUIObj = canavs.transform.Find("Training").gameObject;
        reticleObj = canavs.transform.Find("Training/Reticle").gameObject;
        trainingNameText = canavs.transform.Find("Training/TrainingName").GetComponent<TMP_Text>();

        sendPortInputField = settingUIObj.transform.Find("PortNumbers/SendPortNumInput").GetComponent<TMP_InputField>();
        receivePortInputField = settingUIObj.transform.Find("PortNumbers/ReceivePortNumInput").GetComponent<TMP_InputField>();
        allTrainingNumInputField = settingUIObj.transform.Find("AllTrain/AllTrainNumInput").GetComponent<TMP_InputField>();

        udpSender = GameObject.Find("UDPSender").GetComponent<UdpSender>();
        udpReceiver = GameObject.Find("UDPReceiver").GetComponent<UdpReceiver>();

        warningManager = GameObject.Find("WarningManager").GetComponent<WarningManager>();
        settingManager = GameObject.Find("SettingManager").GetComponent<SettingManager>();

        selectFileNameText = GameObject.Find("Canvas").transform.Find("Setting/SelectFileName").GetComponent<TMP_Text>();

        saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();

        selectFileNameText.text = Path.GetFileName(saveManager.GetFolderPath());

        

        FirstSave();
    }

    // Update is called once per frame
    void Update()
    {
        TrainingProcess();
        StopTraining();
    }

    /// <summary>
    /// トレーニングの開始
    /// </summary>
    /// <param name="trainingName"></param>
    /// <param name="trainingVideoLink"></param>
    /// <param name="maxTrainingCount"></param>
    public void StartTraining(string trainingName,string trainingVideoLink, float trainingVideoTime,int maxTrainingCount){
        if(sendPortInputField.text == "") {
            Debug.LogWarning("Send Portを指定して下さい");
            warningManager.ShowWarningText("The sending port must be specified.");
            return;
        }
        isTraining = true;
        isAllTrain = false;

        settingUIObj.SetActive(false);
        trainingUIObj.SetActive(true);
        
        trainingNameText.text = trainingName;

        videoPlayer.url = "file://" + trainingVideoLink;

        this.trainingVideoTime = trainingVideoTime;

        this.maxTrainingCount = maxTrainingCount;

        trainingTime = 0;
        trainingCount = 0;
        trainingTime = 0f;
    }

    public void StartAllTraining(){
        if(sendPortInputField.text == "") {
            Debug.LogWarning("Send Portを指定して下さい");
            warningManager.ShowWarningText("The sending port must be specified.");
            return;
        }
        if(allTrainingNumInputField.text == ""){
            Debug.LogWarning("Number of training sessions must be specified.");
            warningManager.ShowWarningText("Number of training sessions must be specified.");
            return;
        }

        isAllTrain = true;

        allTrainNum = int.Parse(allTrainingNumInputField.text);
        allTrainingName = new string[settingManager.GetTrainingTypeNum()];
        allTraiingVideoLink = new string[settingManager.GetTrainingTypeNum()];
        allTrainingVideoTime = new float[settingManager.GetTrainingTypeNum()];

        for(int i=0; i<allTrainingName.Length; i++){
            allTrainingName[i] = settingManager.GetTrainingName(i);
            allTraiingVideoLink[i] = "file://" + settingManager.GetTrainingVideoLink(i);            
            allTrainingVideoTime[i] = settingManager.GetTrainingVideoTime(i);
            Debug.Log(i + "番目は" + allTrainingVideoTime[i] + "秒");
        }
        
        trainOrder = ShuffleTrainingOrder(allTrainNum);

        for(int i=0; i<trainOrder.Length; i++){
            Debug.Log(trainOrder[i] );
        }
        trainingNameText.text = allTrainingName[trainOrder[0]];
        videoPlayer.url = allTraiingVideoLink[trainOrder[0]];
        trainingVideoTime = allTrainingVideoTime[trainOrder[0]]; 

        isTraining = true;

        settingUIObj.SetActive(false);
        trainingUIObj.SetActive(true);

        maxTrainingCount = trainOrder.Length;

        trainingTime = 0;
        trainingCount = 0;
        trainingTime = 0f;
    }

    private int[] ShuffleTrainingOrder(int allTrainNum){
        var trainTypeNum = settingManager.GetTrainingTypeNum();
        var num = new int[trainTypeNum];
        

        for(int i=0; i<trainTypeNum; i++){
            num[i] = i;
        } 

        trainOrder = new int[trainTypeNum * allTrainNum];

        for(int i=0; i<allTrainNum; i++){
            //シャッフル
            for(int j=0; j<trainTypeNum; j++){
                int temp = num[j]; // 現在の要素を預けておく
                int randomIndex = Random.Range(0, num.Length); // 入れ替える先をランダムに選ぶ
                num[j] = num[randomIndex]; // 現在の要素に上書き
                num[randomIndex] = temp; // 入れ替え元に預けておいた要素を与える
            }

            //更新
            for(int k=0; k<trainTypeNum; k++){
                trainOrder[i * trainTypeNum + k] = num[k];
                Debug.Log(num[k]);
            }
        }
    
        return trainOrder;
    }

    /// <summary>
    /// トレーニング中の処理
    /// </summary>
    private void TrainingProcess(){
        if(!isTraining) return;
        trainingTime += Time.deltaTime;
        if(trainingTime < restTime){
            //安静
        }else if(trainingTime < restTime + instituteTime){
            //指示
            trainingNameText.gameObject.SetActive(true);
        }else if(trainingTime < restTime + instituteTime + signTime){
            //合図
            trainingNameText.gameObject.SetActive(false);
            reticleObj.SetActive(true);
        }else{
            //トレーニング
            if(trainingUIObj.activeSelf){
                //最初の処理
                Debug.Log((trainingCount + 1) + "回目 : トレーニング開始");
                Debug.Log(trainingVideoTime + "秒継続");
                //udpSender.SendMessages(trainingNameText.text);
            }
            trainingUIObj.SetActive(false);
            reticleObj.SetActive(false);
            videoPlayer.Play();
            if(trainingTime >= restTime + instituteTime + signTime + trainingVideoTime){
                //トレーニング動画が終わった時
                Debug.Log((trainingCount + 1) + "回目 : トレーニング終了");
                trainingTime = 0;
                trainingUIObj.SetActive(true);
                trainingCount++;                

                if(trainingCount >= maxTrainingCount){
                    isTraining = false;
                    settingUIObj.SetActive(true);
                    trainingUIObj.SetActive(false);
                }else{
                    if(isAllTrain){
                        trainingNameText.text = allTrainingName[trainOrder[trainingCount]];
                        videoPlayer.url = allTraiingVideoLink[trainOrder[trainingCount]];
                        trainingVideoTime = allTrainingVideoTime[trainOrder[trainingCount]];
                    }
                }
            }
        }
    }

    /// <summary>
    /// トレーニングの中止
    /// </summary>
    private void StopTraining(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            isTraining = false;
            videoPlayer.Stop();
            settingUIObj.SetActive(true);
            trainingUIObj.SetActive(false);
            trainingTime = 0;
            trainingCount = 0;
        }
        
    }

    

    /// <summary>
    /// トレーニングビデオが一周終わった時の処理
    /// </summary>
    /// <param name="videoPlayer"></param>
    private void LoopPointReached(VideoPlayer videoPlayer){
        videoPlayer.Stop();
    }

    

    /// <summary>
    /// InputFieldが変更された際に呼び出す
    /// </summary>
    public void SetUDPReceiverPort(){
        if(receivePortInputField.text == "") return;
        udpReceiver.SetPortNum(int.Parse(receivePortInputField.text));
        Save();
    }

    /// <summary>
    /// InputFieldが変更された際に呼び出す
    /// </summary>
    public void SetUDPSenderPort(){
        if(sendPortInputField.text == "") return;
        udpSender.SetPortNum(int.Parse(sendPortInputField.text));
        Save();
    }

    /// <summary>
    /// 何週トレーニングするか
    /// </summary>
    public void SetAllTrainNum(){
        this.allTrainNum = int.Parse(allTrainingNumInputField.text);
        Save();
    }

    public void ResetInfo(){
        this.udpReceiver.SetPortNum(0);
        this.udpSender.SetPortNum(0);
        allTrainNum = 0;
        UpdateData();
    }


    public void FirstSave(){
        // fileName = fileName + name + ".json";
        fileName =  "TrainingManagerData" + ".json";
        filePath = saveManager.GetFolderPath() + "/" + fileName;
        saveData = new SaveData();
        // ファイルがないとき、ファイル作成
        if (!File.Exists(filePath)) {
            Save();
        }

        // ファイルを読み込んでdataに格納
        saveData = Load(filePath);

        this.udpReceiver.SetPortNum(saveData.receivePortNum);
        this.udpSender.SetPortNum(saveData.sendPortNum);
        allTrainNum = saveData.allTrainNum;

        UpdateData();
    }

    private void UpdateData(){
        sendPortInputField.text = udpSender.port.ToString();
        receivePortInputField.text = udpReceiver.port.ToString();
        allTrainingNumInputField.text = allTrainNum.ToString();
    }

    //-------------------------------------------------------------------
    // jsonとしてデータを保存
    public void Save()
    {
        saveData.receivePortNum = udpReceiver.port;
        saveData.sendPortNum = udpSender.port;
        saveData.allTrainNum = allTrainNum;
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
        [Header("Receiveのポート番号")]public int receivePortNum;
        [Header("Sendのポート番号")]public int sendPortNum;
        [Header("何回トレーニングするか")]public int allTrainNum;
    }
}
