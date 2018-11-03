using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class ArtController2 : MonoBehaviour {

    private int numImages;

    public Sprite[] img;

    public string[] titles;
    public string[] descriptions;
    private Vector3[] posSave = new Vector3[4];

    public Transform showSprite; //Empty SpriteRender object

    public int dist = 50; //Radius of circle
    public int viewDist = 20; //Dist b/n edge of circle and camera
    Vector3 centrePoint;

    public float artLerpTime = 0.5f;        
    float currentLerpTime = 0;

    private int moveOrder = 0;
    private int moveCmd = 0;
    private int currLoc = 0;
	

    GameObject canvas;
    GameObject textTitle;
    GameObject textDesc;

    Canvas myCanvas;
    Text text;
    RectTransform rectTransform;
    Font font;

	void Start () {
        numImages = img.Length;
        titles = new string[numImages];
        descriptions = new string[numImages];

        centrePoint = new Vector3(0,16,dist+viewDist);

        for (int i = 0; i < numImages; i++) {
            Transform artPiece = Instantiate(showSprite, Degree(i), Quaternion.identity);
            artPiece.transform.parent = this.transform;
            artPiece.name = "artPiece " + i.ToString();
            artPiece.GetComponent<SpriteRenderer>().sprite = img[i];           
        }




        //Create Canvas
        canvas = new GameObject();
        canvas.transform.parent = this.transform;
        canvas.name = "canvas";
        canvas.AddComponent<Canvas>();

        myCanvas = canvas.GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();

        font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

        textTitle = new GameObject();
        textDesc = new GameObject();

        fillWords(); //TEMP
        writer();
        reWriter(titles[0], descriptions[0]);

        posSave[0] = textTitle.transform.localPosition;
        posSave[1] = textDesc.transform.localPosition;
        posSave[2] = posSave[0] + new Vector3(0,600,0);
        posSave[3] = posSave[1] + new Vector3(0,-400,0);
	}
	


	void Update () {
        if(Input.GetKeyDown(KeyCode.LeftArrow)){moveOrder += 1;} 
        if(Input.GetKeyDown(KeyCode.RightArrow)){moveOrder += -1;}

        if (Sign(moveOrder) != 0) {moveCmd = moveOrder;} //to prevent move getting stuck mid lerp

        if (moveCmd != 0){
            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > artLerpTime){
                currentLerpTime = artLerpTime;
            }

            float perc = currentLerpTime / artLerpTime;

            for (int i = 0; i < numImages; i++) { //Lerp Sprites
                GameObject.Find("artPiece " + i).transform.position = Vector3.Lerp(Degree(i+currLoc), Degree(i+currLoc+Sign(moveCmd)), perc);
            }

            if (perc <= 0.5) { //Lerp words
                textTitle.transform.localPosition = Vector3.Lerp(posSave[0], posSave[2], perc);
                textDesc.transform.localPosition = Vector3.Lerp(posSave[1], posSave[3], perc);
            }
            else {
                textTitle.transform.localPosition = Vector3.Lerp(posSave[2], posSave[0], perc);
                textDesc.transform.localPosition = Vector3.Lerp(posSave[3], posSave[1], perc);
            }         

            if (perc >= 1) {                
                currLoc += Sign(moveCmd);
                moveOrder += -Sign(moveCmd);
                moveCmd += -Sign(moveCmd);
                currentLerpTime = 0;
            }          
        }                   
        reWriter(titles[spin(currLoc*-1)],descriptions[spin(currLoc*-1)]);
    }

    void fillWords(){
        for (int i = 0; i < numImages; i++) {
            titles[i] = "Art Piece #" + i.ToString();
            descriptions[i] = "this amazing art piece is art number " + i.ToString() + " and is about stuff.";
        }
    }

    void writer(){
        // Text Title       
        textTitle.transform.parent = canvas.transform;
        textTitle.name = "Title";

        text = textTitle.AddComponent<Text>();
        text.font = font;
        text.text = "Art";
        text.fontSize = 100;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;

        // Text position
        rectTransform = text.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0, 150, 1);
        rectTransform.sizeDelta = new Vector2(800, 200);

        ////
        // Text Description
        textDesc.transform.parent = canvas.transform;
        textDesc.name = "Desc";

        text = textDesc.AddComponent<Text>();
        text.font = font;
        text.text = "This is art of a character blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah";
        text.fontSize = 20;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;

        // Text position
        rectTransform = text.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0, -175, 0);
        rectTransform.sizeDelta = new Vector2(400, 200);
    }

    void reWriter(string title, string desc){
        textTitle.GetComponent<Text>().text = title;
        textDesc.GetComponent<Text>().text = desc;
    }



    int spin(int i){
        if (i<0){i = spin(i + numImages);}
        if (i >= numImages) {i = spin(i - numImages);}
        return i;
    }

    Vector3 Degree(int i){ //Calculate object location around circle
        Vector3 spot = new Vector3(0, 0, 0);
        float angleD = 360 / numImages * i;
        float angle = angleD * Mathf.PI / 180;

        spot.x = centrePoint.x + (int)Mathf.Round(dist * Mathf.Sin(angle));
        spot.y = centrePoint.y - (int)Mathf.Round(dist * Mathf.Cos(angle)) / 3;
        spot.z = centrePoint.z - (int)Mathf.Round(dist * Mathf.Cos(angle));

        return spot;
    }

    public static int Sign(int i){//Math.Signf but int and with 0 (why does this not exist?)        
        if(i < 0){return -1;}
        if(i > 0){return 1;}
        return 0;
    }
}