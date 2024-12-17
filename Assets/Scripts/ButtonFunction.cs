using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunction : MonoBehaviour
{
    public void Resume()
    {
        GameManager.instance.StateUnPause();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // Just reset the Time.timescale
        GameManager.instance.StateUnPause();
    }
    public void PauseMenu()
    {
        
    }
    public void SettingsMenu()
    {

    }
    public void ControlsMenu()
    {

    }
    public void InventoryMenu()
    {

    }


    public void Quit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }


}