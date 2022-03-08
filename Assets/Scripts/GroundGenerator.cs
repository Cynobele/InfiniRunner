using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//-----------References
//Game loop basics (Start(), Update() etc): https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
//Working with prefabs: https://sharpcoderblog.com/blog/unity-3d-working-with-prefabs
//3D Camera stuff: https://www.youtube.com/watch?v=MFQhpwc6cKE , https://learn.unity.com/tutorial/controlling-unity-camera-behaviour
//Unity docs: https://docs.unity3d.com/ScriptReference/
//Lists in C#: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=net-6.0


public class GroundGenerator : MonoBehaviour
{
    public Camera camera_main;
    public Transform start_point; //Point from where ground tiles will start
    public PlatformTile tile_prefab;
    public float mov_speed = 12.0f;
    public int prespawn_amount = 10; //How many tiles should be spawned ahead of the player
    public int warmup_amount = 3; //How many tiles should pass before obstacles spawn

    List<PlatformTile> spawned_tiles = new List<PlatformTile>(); // list containing all current activated tiles
    [HideInInspector] // hides vars from inspector view
    public bool game_over = false;
    static bool game_started = false;
    float score = 0;
    float time_survived = 0; //times how long player has lived
    bool timer_running = false; //flag indicating if timer is running
    string timer_display_string;
    private GUIStyle gui_style = new GUIStyle();

    public static GroundGenerator gg_instance; //instantiate ground generator object



    // Start is called before the first frame update
    void Start()
    {
        gg_instance = this; //instantiate ground generator
        Vector3 spawn_position = start_point.position; //set the original spawn pos-xyz

        SetGUIElements(); //sets elements for GUI text to access
        SetGameClock();

        int warmup_temp = warmup_amount;
        for (int i = 0; i < prespawn_amount; i++)
        {
            spawn_position -= tile_prefab.startPoint.localPosition;

            PlatformTile spawned_tile = Instantiate(tile_prefab, spawn_position, //spawned_tile is the tile BEING spawned
                    Quaternion.identity) as PlatformTile;
           
            if (warmup_temp > 0) // if there are warmup tiles remaining, deactivate the obstacles
            {
                spawned_tile.DeactivateAllObstacles();
                warmup_temp--;
            }
            else //activate a random set of obstacles on the tile
            {
                spawned_tile.ActivateRandomObstacle();
            }

            spawn_position = spawned_tile.endPoint.position; //set next spawn to the end of this tile
            spawned_tile.transform.SetParent(transform);
            spawned_tiles.Add(spawned_tile);
        }
    }


    // Update is called once per frame
    void Update()
    {
        // Move the object upward in world space x unit/second.
        //Increase speed the higher score we get
        if (!game_over && game_started)
        {
            //the tiles are moving down the z axis (behind the camera, towards negative)
            transform.Translate(-spawned_tiles[0].transform.forward * Time.deltaTime * (mov_speed + (score / 500)), Space.World);
            
            score += Time.deltaTime * mov_speed; //add to score
            time_survived += Time.deltaTime;     //add to timer

            DisplayTimeSurvived(time_survived);
        }

        if (camera_main.WorldToViewportPoint(spawned_tiles[0].endPoint.position).z < 0)
        {
            //Move the tile to the front if it's behind the Camera
            PlatformTile tile_temp = spawned_tiles[0];
            spawned_tiles.RemoveAt(0);
            tile_temp.transform.position = spawned_tiles[spawned_tiles.Count - 1].endPoint.position - tile_temp.startPoint.localPosition;
            tile_temp.ActivateRandomObstacle();
            spawned_tiles.Add(tile_temp);
        }

        if (game_over || !game_started)
        {
            if (Input.GetKeyDown(KeyCode.Space))// press space to start game
            {
                if (game_over)
                {
                    //Restart current scene
                    Scene scene = SceneManager.GetActiveScene();
                    SceneManager.LoadScene(scene.name);
                }
                else
                {
                    //Start the game
                    game_started = true;
                }
            }
        }
    }


    void SetGameClock()
    {
        timer_running = true; //start timer
        time_survived = 0;    //make sure timer is 0
    }
    void DisplayTimeSurvived(float x) // x is the time to display
    {
        float mins = Mathf.FloorToInt(x / 60);
        float secs = Mathf.FloorToInt(x % 60);

        //format values into string   {mins:format} : {secs:format}
        timer_display_string = string.Format("{0:00}:{1:00}", mins, secs);
    }

    void SetGUIElements()
    {
        gui_style.fontSize = 40; //set font size
        gui_style.normal.textColor = Color.blue; // font colour
    }

    void OnGUI()
    {
        if (game_over)
        {
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 450, 300), "Game Over\nYour score is: " + ((int)score) + "\nPress 'Space' to restart", gui_style);
        }
        else
        {
            if (!game_started)
            {
                GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 450, 300), "Press 'Space' to start", gui_style);
            }

            if (timer_running) //game and timer are running
            {
                GUI.Label(new Rect(5, 5, 200, 25), "Score: " + ((int)score), gui_style);
                GUI.Label(new Rect(5, 45, 200, 25), "Time: " + timer_display_string, gui_style);
            }
        }
    }
}
