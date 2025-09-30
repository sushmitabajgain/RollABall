using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject endMenuPanel;                 // Assign the EndMenuPanel GameObject
    public TMPro.TextMeshProUGUI resultText;        // Assign the ResultText (inside the panel)

    void Start()
    {
        if (endMenuPanel != null) endMenuPanel.SetActive(false);
    }

    public void ShowEndMenu(string message)
    {
        if (resultText != null) resultText.text = message;
        if (endMenuPanel != null) endMenuPanel.SetActive(true);
        // Optional: Pause game world if you want
        // Time.timeScale = 0f;
    }

    public void ReplayGame()
    {
        // Time.timeScale = 1f; // If you paused
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
