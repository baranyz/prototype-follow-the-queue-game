using UnityEngine;

public class Wall : MonoBehaviour
{
    float failCount {get; set;}
    SpriteRenderer spr;
    bool isFail {get; set;}

    private void Start() {
        
        spr = GetComponent<SpriteRenderer>();
        WallSpawner.wrongTouch += Wrong;
        transform.localScale = new Vector3(WallSpawner.scale,WallSpawner.scale);
        
    }
    private void Update() {
        if(isFail){
            failCount += Time.deltaTime;
            float t = Mathf.PingPong(Time.time*3,1);
            spr.color = new Vector4(default,default,default,t);
            if(failCount > 1) {
                spr.color = new Vector4(default,default,default,1);
                failCount = 0;
                isFail = false;
            }
        }
    }
    private void Wrong(GameObject x){
        if(x == gameObject){
            isFail = true;
        }
    }
}
