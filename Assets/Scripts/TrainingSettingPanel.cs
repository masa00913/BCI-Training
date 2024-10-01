using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using SFB;
using UnityEngine.Video;

public class TrainingSettingPanel : MonoBehaviour
{
    [Header("選択されたトレーニングボタンのオブジェ")]private TrainingButton trainingButton;
    [Header("トレーニングの名前のInputField")]private TMP_InputField nameInputField;
    [Header("トレーニングカウントのInputField")]private TMP_InputField countInputField;
    [Header("トレーニングビデオのリンク")]private TMP_Text trainingVideoText;
    private WarningManager warningManager;
    private TrainingManager trainingManager;
    private SettingManager settingManager;

    private VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Awake()
    {
        nameInputField = transform.Find("Panel/TrainingNameInput").GetComponent<TMP_InputField>();
        countInputField = transform.Find("Panel/TrainingCountInput").GetComponent<TMP_InputField>();
        trainingVideoText = transform.Find("Panel/VideoLink").GetComponent<TMP_Text>();
        
        trainingManager = GameObject.Find("TrainingManager").GetComponent<TrainingManager>();
        settingManager = GameObject.Find("SettingManager").GetComponent<SettingManager>();
        warningManager = GameObject.Find("WarningManager").GetComponent<WarningManager>();
     
        videoPlayer = GameObject.Find("VideoPlayer").GetComponent<VideoPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void OpenFile()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "mp4", false);
        if (paths.Length > 0)
        {
            string filePath = paths[0];
            trainingVideoText.text = filePath;
        }
    }

    /// <summary>
    /// 選択されたトレーニングボタンを設定
    /// </summary>
    /// <param name="trainingButton"></param>
    public void SetTrainingButton(TrainingButton trainingButton){
        this.trainingButton = trainingButton;        
    }

    public void OnClickOKButton(){
        if(nameInputField.text == "" || countInputField.text == "" || trainingVideoText.text == "Video link"){
            Debug.LogWarning("空欄があります。");
            warningManager.ShowWarningText("There is a blank space.");
        }else{
            if(!trainingButton.GetIsConcludeTraining()){
                //初めて入れる場合
                trainingButton.SetTrainingColor(settingManager.GetEmptyTrainingColor());
            }
            settingManager.SetNeedToSetVideoTime(true);
            trainingButton.SetTrainingInfo(nameInputField.text, countInputField.text, trainingVideoText.text);
            videoPlayer.url = "file://" + trainingButton.GetTrainingVideoLink();
            
            videoPlayer.Prepare();
            ResetProcess();
        }        
    }

    public void OnClickDeleteButton(){
        ResetProcess();
    }

    private void ResetProcess(){
        gameObject.SetActive(false);
        nameInputField.text = null;
        countInputField.text = null;
        trainingVideoText.text = "Video link";
    }

    

    public void SetTrainingName(string trainingName){
        nameInputField.text = trainingName;
    }

    public void SetTrainingCount(int trainingCount){
        countInputField.text = trainingCount.ToString();
    }

    public void SetTrainingVideoLink(string trainingVideoLink){
        trainingVideoText.text = trainingVideoLink;
    }
}
