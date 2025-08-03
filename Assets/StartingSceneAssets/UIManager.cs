using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public float duration = 0.5f; // Duration of the movement
    public GameObject Slider;
    public GameObject CharacterSlider;
    public GameObject SkipButton;
    public GameObject PreviousButton;

    private bool isMoving = false;
    public AudioSource LaughingAudio;
    public AudioSource distroyingAudio;
    public AudioSource spellingAudio;
    public GameObject determination;

    void Update()
    {
        // Only run sound logic if MainPanel is NOT active
        if (!determination.activeInHierarchy)
        {
            float x = Slider.transform.position.x;

            bool isAt23 = Mathf.Abs(x - 23f) < 0.01f;
            bool isAt0 = Mathf.Abs(x - 0f) < 0.01f;
            bool isAtMinus23 = Mathf.Abs(x + 23f) < 0.01f;

            if (isAt23)
            {
                if (!LaughingAudio.isPlaying)
                    LaughingAudio.Play();
            }
            else
            {
                LaughingAudio.Stop();
            }

            if (isAt0)
            {
                if (!distroyingAudio.isPlaying)
                    distroyingAudio.Play();
            }
            else
            {
                distroyingAudio.Stop();
            }

            if (isAtMinus23)
            {
                if (!spellingAudio.isPlaying)
                    spellingAudio.Play();
            }
            else
            {
                spellingAudio.Stop();
            }
        }
        else
        {
            // If main panel is active, stop all sounds
            LaughingAudio.Stop();
            distroyingAudio.Stop();
            spellingAudio.Stop();
        }
    }

    public void StartSkip()
    {
        if (!isMoving && Slider.transform.position.x > -23f)
            StartCoroutine(Skip());
        if (Slider.transform.position.x <= -23f)
        {
            determination.SetActive(true);
        }
    }

    public void StartPrevious()
    {
        if (!isMoving && Slider.transform.position.x < 23f)
            StartCoroutine(Previous());

    }

    public IEnumerator Skip()
    {
        isMoving = true;

        Vector3 startPosition = Slider.transform.position;
        Vector3 endPosition = startPosition + new Vector3(-23f, 0f, 0f);
        Vector3 characterStartPosition = CharacterSlider.transform.position;
        Vector3 characterEndPosition = characterStartPosition + new Vector3(0f, 10f, 0f);

        SkipButton.SetActive(false);
        PreviousButton.SetActive(false);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            Slider.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            CharacterSlider.transform.position = Vector3.Lerp(characterStartPosition, characterEndPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it snaps to the exact final position
        Slider.transform.position = endPosition;
        CharacterSlider.transform.position = characterEndPosition;

        SkipButton.SetActive(true);
        PreviousButton.SetActive(true);
        isMoving = false;
    }

    public IEnumerator Previous()
    {
        isMoving = true;

        Vector3 startPosition = Slider.transform.position;
        Vector3 endPosition = startPosition + new Vector3(23f, 0f, 0f);
        Vector3 characterStartPosition = CharacterSlider.transform.position;
        Vector3 characterEndPosition = characterStartPosition + new Vector3(0f, -10f, 0f);

        SkipButton.SetActive(false);
        PreviousButton.SetActive(false);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            Slider.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            CharacterSlider.transform.position = Vector3.Lerp(characterStartPosition, characterEndPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Slider.transform.position = endPosition;
        CharacterSlider.transform.position = characterEndPosition;

        SkipButton.SetActive(true);
        PreviousButton.SetActive(true);
        isMoving = false;
    }
    public void LoadNextScene()
    {
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}
