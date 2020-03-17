using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestoreProps : MonoBehaviour
{
    private List<Vector3> originPos = new List<Vector3>();
    private List<Quaternion> originRot = new List<Quaternion>();
    private List<GameObject> throwables = new List<GameObject>();

    private void Awake()
    {
        for(int i = 0; i < this.transform.childCount; ++i)
        {
            throwables.Add(this.transform.GetChild(i).gameObject);
            originPos.Add(this.transform.GetChild(i).transform.position);
            originRot.Add(this.transform.GetChild(i).transform.rotation);
        }
    }

    public void RestoreAll()
    {
        for(int i = 0; i < throwables.Count; ++i)
        {
            throwables[i].SetActive(true);
            //throwables[i].gameObject.transform.rotation= new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
            //throwables[i].gameObject.GetComponent<EquippableObject>().Rigidbody.velocity = Vector3.zero;
            throwables[i].transform.position = originPos[i];
            throwables[i].transform.transform.rotation = originRot[i];
        }
    }
}
