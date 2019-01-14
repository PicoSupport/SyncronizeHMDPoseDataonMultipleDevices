using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    public Text UILog;
    public Transform HeadTransform;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UILog.text = "" + HeadTransform.transform.localPosition;
        UILog.text += "/n" + HeadTransform.transform.position;
    }
}
