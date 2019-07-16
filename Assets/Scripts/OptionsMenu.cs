using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    public Toggle toggle;
    // Start is called before the first frame update
    void Start()
    {
        toggle.isOn = Options.affectiveEnabled;
    }

    public void OnToggleChange()
    {
        Options.affectiveEnabled = toggle.isOn;
        Debug.Log(Options.affectiveEnabled);
    }
    
    

    // Update is called once per frame
}
