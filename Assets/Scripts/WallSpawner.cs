using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WallSpawner : MonoBehaviour
{
    [SerializeField] GameObject wall;
    GameObject lastSpawn;
    public static float scale{get;set;}
    List<GameObject> selects = new List<GameObject>();
    Dictionary<GameObject, bool> dic = new Dictionary<GameObject, bool>();
    Stack<GameObject> st = new Stack<GameObject>();
    UnityEngine.Color inRandom;
    public static event Action<GameObject> wrongTouch;
    bool isEntrenceDone = false;

    private void Start() {
        
        Vector2 screenPos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        //it can be assign randomly
        scale = .2f;
        int cubesCountX = (int)((screenPos.x*2)/(scale + scale/10))-1;
        float cubesSpaceX = cubesCountX*scale + cubesCountX*(scale/10);
        float distX = screenPos.x*2 - cubesSpaceX;

        int cubesCountY = (int)(((screenPos.y-0.5f)*2)/(scale + scale/10));
        float cubesSpaceY = cubesCountY*scale + cubesCountY*(scale/10);
        float distY = screenPos.y*2 - cubesSpaceY;

        inRandom = new Vector4(UnityEngine.Random.Range(0,0.99f), UnityEngine.Random.Range(0,0.99f), UnityEngine.Random.Range(0,0.99f), 1);

        List<List<GameObject>> pos = new List<List<GameObject>>();
        float posY = -screenPos.y+distY/2;

        for(float i = 0; i <= cubesCountY; i++){
            
            float posX = -screenPos.x+distX/2;
            List<GameObject> list = new List<GameObject>();
            for(float j = 0; j <= cubesCountX; j++){
                
                GameObject clone = Instantiate(wall, new Vector2(posX,posY), Quaternion.identity);
                posX += scale+scale/10;
                list.Add(clone);
            }
            posY += scale+scale/10;
            pos.Add(list);
        }

        int randomCubeCount = UnityEngine.Random.Range(10, 30);



        int randomCubeOut = UnityEngine.Random.Range(0,pos.Count);
        int randomCubeIn = UnityEngine.Random.Range(0,pos[0].Count);
        GameObject lastCube = pos[randomCubeOut][randomCubeIn];
        int randomScale = UnityEngine.Random.Range(20,100);

        for(int j = 0; j < randomScale; j++){

            if(!dic.ContainsKey(lastCube)) dic.Add(lastCube,false);
            if(!st.Contains(lastCube)) st.Push(lastCube);
            lastCube.GetComponent<SpriteRenderer>().color = inRandom;
            selects.Add(lastCube);
            int tempRandom = UnityEngine.Random.Range(1,4);
            if(tempRandom == 1 && randomCubeOut > 0){
                lastCube = pos[randomCubeOut--][randomCubeIn];
            }
            else if(tempRandom == 2 && randomCubeOut < pos.Count-1){
                lastCube = pos[randomCubeOut++][randomCubeIn];
            }
            else if(tempRandom == 3 && randomCubeIn > 0){
                lastCube = pos[randomCubeOut][randomCubeIn--];
            }
            else if(tempRandom == 4 && randomCubeIn < pos[0].Count-1){
                lastCube = pos[randomCubeOut][randomCubeIn++];
            }

        }

        StartCoroutine("ColorUnselect");

    }
    private void Update() {

        if(Input.touchCount > 0){

            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D ray = Physics2D.Raycast(touchPos, Vector2.one);
            if(ray.collider != null){
                
                if(dic.ContainsKey(ray.collider.gameObject) && dic[ray.collider.gameObject] == false && isEntrenceDone){
                
                    ray.collider.gameObject.GetComponent<SpriteRenderer>().color = inRandom;
                    dic[ray.collider.gameObject] = true;
                }
                else if(!dic.ContainsKey(ray.collider.gameObject) && isEntrenceDone){
                    wrongTouch(ray.collider.gameObject);
                }
             
                if(dic.Values.Where(x => x == false).ToArray().Count() == 0){
                    Debug.Log("LEVEL DONE!");
                }
            }
        }

    }

    IEnumerator ColorUnselect(){

        yield return new WaitForSeconds(1f);
        int n = st.Count;
        for(int i = 0; i < n; i++){

            GameObject lastPop = st.Pop();
            lastPop.GetComponent<SpriteRenderer>().color = new Vector4(0,0,0,1);
            yield return new WaitForSeconds(0.05f);
        }
        isEntrenceDone = true;
    }
}
