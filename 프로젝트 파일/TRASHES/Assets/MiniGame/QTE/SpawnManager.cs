using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    public GameObject Password_MiniGame;

    public GameObject Prefab;
    public GameObject SpinPrefab;
    public GameObject Key_ZPrefab;

    public Text press_Text;

    private void Start()
    {
        Password_MiniGame = GameObject.Find("Password_MiniGame");
    }

    public void Spawn()
    {
        StartCoroutine(spawnCorutine());
    }
    
    IEnumerator spawnCorutine()
    {
        yield return new WaitForSeconds(Random.Range(2, 6));
        GameObject newPrefab;
        GameObject newSpinPrefab;
        GameObject newKey_ZPrefab;
        Text newpress_Text;

        Quaternion randZrotation = Quaternion.Euler(0, 0, Random.Range(165, 450));
        transform.rotation = randZrotation;
        newPrefab = Instantiate(Prefab, new Vector2(500, 0), transform.rotation);
        newPrefab.transform.SetParent(GameObject.Find("QTE").transform, false);

        transform.rotation = Quaternion.Euler(0, 0, 0);
        newSpinPrefab = Instantiate(SpinPrefab, new Vector2(500, 0), transform.rotation);
        newSpinPrefab.transform.SetParent(GameObject.Find("QTE").transform, false);

        transform.rotation = Quaternion.Euler(0, 0, 0);
        newKey_ZPrefab = Instantiate(Key_ZPrefab, new Vector2(500, -300), transform.rotation);
        newKey_ZPrefab.transform.SetParent(GameObject.Find("QTE").transform, false);

        newpress_Text = Instantiate(press_Text, new Vector2(400, -300), transform.rotation);
        newpress_Text.transform.SetParent(GameObject.Find("QTE").transform, false);
    }

}
