using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManagerScript : MonoBehaviour
{
    public UpdateGameSettings gameSettings;
    public string[] scenes;
    PhotonView view;
    GameObject player_holder;
    int my_id;
    public GameObject playerScorePrefab;
    GameObject my_player;
    public int[] game_scores;
    public bool[] player_dead_stats;
    public bool first_game = true;
    public int map_idx;
    public int gamemode_idx;

    void Start()
    {
        view = GetComponent<PhotonView>();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    [PunRPC]
    void StartGame()
    {
        if (first_game)
            PrepSceneChange();
        PhotonNetwork.AutomaticallySyncScene = true;
        if (view.IsMine)
            PhotonNetwork.LoadLevel(scenes[map_idx]);
        if(!first_game && !view.IsMine)
        {
            PhotonNetwork.LoadLevel(scenes[map_idx]);
        }
    }

    void PrepSceneChange()
    { 
        //DontDestroyOnLoad(gameSettings);
        DontDestroyOnLoad(gameObject);
        player_holder = GameObject.Find("PlayerHolder");
        DontDestroyOnLoad(player_holder);
        map_idx = gameSettings.mapIdx;
        gamemode_idx = gameSettings.gamemodeIdx;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "LobbyScene")
            StartCoroutine(PrepGame());
    }

    IEnumerator PrepGame()
    {
        Debug.Log("Begin setup for game...");
        GameObject spawn_points = GameObject.Find("SpawnPoints");

        my_id = -1;
        GameObject player = null;
        player_holder.transform.position = Vector3.zero;
        int i = 0;
        if(first_game)
        {
            player_dead_stats = new bool[player_holder.transform.childCount];
            game_scores = new int[player_holder.transform.childCount];
        } else
        {
            UpdateAllScores();
        }

        
        

        foreach (Transform child in player_holder.transform)
        {
            PhotonView childView = child.GetComponent<PhotonView>();
            
            player_dead_stats[i] = false;
            if(first_game)
                game_scores[i] = 0;
            if (childView != null && childView.IsMine)
            {
                my_id = childView.ViewID;
                player = child.gameObject;
                my_player = child.gameObject;
                Debug.Log("Found my player with viewId: " + my_id);
                //break;
            }
            child.transform.position = Vector3.zero;
            
            i++;
        }

        yield return new WaitForSeconds(0f); // Wait for half a second to ensure all objects are loaded

        UpdatePosition();

    }

    void UpdatePosition()
    {
        StartCoroutine(UpdatePositionCoroutine());
    }
    IEnumerator UpdatePositionCoroutine()
    {
        GameObject player_obj = null;
        string name_to_search = "Point (" + my_id / 1000 + ")";
        GameObject point = null;

        // Wait until the player object and point are found
        while (player_obj == null || point == null)
        {
            player_obj = PhotonView.Find(my_id + 1)?.gameObject; // Use null-conditional operator to prevent null reference
            point = GameObject.Find(name_to_search);

            if (player_obj != null && point != null)
            {
                // Disable the synchronization and movement components
                PhotonTransformViewClassic photonTransformView = player_obj.GetComponent<PhotonTransformViewClassic>();
                PlayerMovementAdvanced2 playerMovement = player_obj.GetComponent<PlayerMovementAdvanced2>();
                Climbing climbing = player_obj.GetComponent<Climbing>();
                Rigidbody rb = player_obj.GetComponent<Rigidbody>();

                photonTransformView.enabled = false;
                playerMovement.enabled = false;
                climbing.enabled = false;
                rb.isKinematic = true;

                // Update the position for a few frames
                for (int i = 0; i < 5; i++)
                {
                    player_obj.transform.position = point.transform.position;
                    yield return new WaitForEndOfFrame(); // Wait for the end of the frame
                }

                // Re-enable the components after updating the position
                rb.isKinematic = false;
                climbing.enabled = true;
                photonTransformView.enabled = true;
                playerMovement.enabled = true;

                Debug.Log($"Player {my_id} position updated to {point.transform.position}");
            }
            else
            {
                Debug.LogWarning($"Waiting for player object or spawn point. Player: {player_obj != null}, Point: {point != null}");
                yield return new WaitForSeconds(0.1f); // Wait for 0.1 seconds before trying again
            }
        }
        UpdateScoresColor();
    }


    void UpdateScoresColor ()
    {
        Transform content = GameObject.Find("Content").transform;
        if (content == null)
            Debug.Log("didnt find content");

        foreach (Transform pl in player_holder.transform)
        {
            GameObject player_score = Instantiate(playerScorePrefab, content);
            player_score.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 275 - ((pl.GetComponent<PhotonView>().ViewID / 1000 - 1) * 50), 0);
            Material[] suit_mat = pl.transform.GetChild(1).GetChild(0).GetComponent<MeshRenderer>().materials;
            Color pl_suit_col = Color.black;
            foreach(Material mat in suit_mat)
            {
                if(mat.name == "suit_material (Instance)")
                {
                    pl_suit_col = mat.color;
                }
            }

            player_score.transform.GetChild(0).GetComponent<TMP_Text>().color = pl_suit_col;
        }

        my_player.transform.GetChild(1).GetComponent<PlayerStatsHandler>().UpdateRefs();
        UpdateAllScores();
    }

    public void UpdateAllScores()
    {
        Debug.Log("Updating scores...");
        Transform content = GameObject.Find("Content").transform;
        if (content == null)
            Debug.Log("didnt find content");

        for(int i = 0; i < content.transform.childCount; i++)
        {
            Transform current_text = content.GetChild(i).transform;
            int player_idx = (275 - (int)current_text.GetComponent<RectTransform>().anchoredPosition.y) / 50;
            current_text.GetComponentInChildren<TMP_Text>().text = game_scores[player_idx].ToString();
        }
    }

    public void ResetGame()
    {

        StartCoroutine(WaitAndStartGame());
    }

    private IEnumerator WaitAndStartGame()
    {
        // Wait for a few frames (e.g., 3 frames)
        int framesToWait = 3;
        for (int i = 0; i < framesToWait; i++)
        {
            yield return null; // Wait for the next frame
        }

        // Call StartGame method on all clients
        Debug.Log("CHANGING SCENE");
        view.RPC("StartGame", RpcTarget.All);
    }







}
