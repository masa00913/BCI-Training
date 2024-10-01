using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WarningManager : MonoBehaviour
{
    private GameObject warningPanel;
    private TMP_Text warningText;
    // Start is called before the first frame update
    void Start()
    {
        warningPanel = GameObject.Find("Canvas").transform.Find("WarningPanel").gameObject;
        warningText = warningPanel.transform.Find("WarningWindow/WarningText").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowWarningText(string warningText){
        warningPanel.SetActive(true);
        this.warningText.text = warningText;
    }

    public void OnClickDeleteButton(){
        warningPanel.SetActive(false);
    }
}
