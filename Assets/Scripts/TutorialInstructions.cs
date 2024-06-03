using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialInstructions : MonoBehaviour
{

    private int dialogueNum;
    [SerializeField] private List<string> instructions;
    [SerializeField] private List<string> buttonLabels;
    [SerializeField] private string defaultButtonLabel;
    [SerializeField] private TMPro.TextMeshProUGUI my_tmp;
    [SerializeField] private TMPro.TextMeshProUGUI button_tmp;
    private int screenOwnID = -1;
    [SerializeField] private int myOwnID;
    [SerializeField] private List<TutorialInstructions> ti_list;

    // Start is called before the first frame update
    void Start()
    {
        dialogueNum = -1;
    }

    public void incrDialogueNum() {

        if (screenOwnID != myOwnID && screenOwnID != -1) {
            return;
        }

        if (screenOwnID == -1) {
            screenOwnID = myOwnID;
        }

        if (dialogueNum < instructions.Count - 1) { 
            dialogueNum++;
            my_tmp.SetText(instructions[dialogueNum]);
            button_tmp.SetText(buttonLabels[dialogueNum]);
        } else {
            // Presume exceed
            dialogueNum = -1;
            my_tmp.SetText("");
            button_tmp.SetText(defaultButtonLabel);
        }

        if (my_tmp.text == "") {
            screenOwnID = -1;
        }

        foreach (TutorialInstructions ti in ti_list) {
            ti.setScreenID(screenOwnID);
        }

        /*
        if (dialogueNum == -1) {
            buttonTxt.SetText("click here for instructions");
        } else if (dialogueNum == instructions.Count - 1) {
            buttonTxt.SetText("phew, last click!");
        } else {
            buttonTxt.SetText("keep clicking here");
        }
        */
    }

    public void decrDialogueNum() {
        if (dialogueNum > 0) { 
            dialogueNum--;
            my_tmp.SetText(instructions[dialogueNum]);
        }
    }

    public void setScreenID(int sid) {
        screenOwnID = sid;
    }

}
