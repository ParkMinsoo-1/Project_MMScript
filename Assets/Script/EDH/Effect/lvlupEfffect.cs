using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lvlupEfffect : MonoBehaviour
{
    public GameObject levelupEff;
    public Button levelUPbtn;
    // Start is called before the first frame update
    void Start()
    {
        levelUPbtn.GetComponent<Button>().onClick.AddListener(() => levelupOnclickEffrct());
    }

    void levelupOnclickEffrct()
    {
        Instantiate(levelupEff, levelupEff.transform.position,Quaternion.identity);
    }
}
