using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    const int NUM_LEVELS = 3;

    public Ball ball { get; private set; }
    public Paddle paddle { get; private set; }
    public Brick[] bricks { get; private set; }

    public int level = 1;
    public int score = 0;
    public int lives = 3;

    private void Awake()
    {
        //we want this scene to persist across scenes 
            //in unity, when you load a new scene it'll unload the previous scene and delete all of those game objects
        DontDestroyOnLoad(gameObject);

        //sceneLoaded is a SystemAction (callback) in the SceneManager that we can attach a function to. In our case we're attaching OnLevelLoaded to it, and we're grabbing the ball, paddle and bricks
        SceneManager.sceneLoaded += OnLevelLoaded; 
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        score = 0;
        lives = 3;

        LoadLevel(1);
    }

    private void LoadLevel(int level)
    {
        this.level = level;

        if (level > NUM_LEVELS)
        {
            // Start over again at level 1 once you have beaten all the levels
            // You can also load a "Win" scene instead
            LoadLevel(1);
            return;
        }

        SceneManager.LoadScene("Level" + level);
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        ball = FindObjectOfType<Ball>();
        paddle = FindObjectOfType<Paddle>();
        bricks = FindObjectsOfType<Brick>();
    }

    public void Miss()
    {
        lives--;

        if (lives > 0) {
            ResetLevel();
        } else {
            GameOver();
        }
    }

    private void ResetLevel()
    {
        paddle.ResetPaddle();
        ball.ResetBall();

        // Resetting the bricks is optional
        for (int i = 0; i < bricks.Length; i++) {
            bricks[i].ResetBrick();
        }
    }

    private void GameOver()
    {
        // Start a new game immediately
        // You can also load a "GameOver" scene instead
        NewGame();
    }

    public void Hit(Brick brick)
    {
        score += brick.points;
        
        //check if we cleared the board
        if (Cleared()) {
            LoadLevel(level + 1); //load by name instead of build index because the build index can change in the build menu
        }
    }

    private bool Cleared()
    {
        for (int i = 0; i < bricks.Length; i++)
        {
            if (bricks[i].gameObject.activeInHierarchy && !bricks[i].unbreakable) {
                return false;
            }
        }

        return true;
    }

}