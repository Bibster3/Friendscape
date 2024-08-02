using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class StoryManager : MonoBehaviour
{
    public static StoryManager instance { get; private set; }

    [Header("References")] [SerializeField]
    private TextMeshProUGUI messageText;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    [Header("Settings")] [SerializeField] private bool animateText = true;

    [Range(0.1f, 1f)] [SerializeField] private float textAnimationSpeed = 0.5f;
    [SerializeField] private float startDelay = 2.0f; // Delay before showing the first sentence

    private int currentSentenceIndex;
    private bool typing;
    private string currentMessage;
    public List<string> sentences = new List<string>();

    [Header("Buttons")]
    private Coroutine nextButtonAnimationCoroutine;
    private Animator nextButtonAnimator;
    public GameObject StartButtonHighlighter; 


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


        nextButtonAnimator = nextButton.GetComponent<Animator>();

        // Add listeners to buttons
        nextButton.onClick.AddListener(NextSentence);
        prevButton.onClick.AddListener(PreviousSentence);
    }

    private void Start()
    {
        // Hide buttons initially
        nextButton.gameObject.SetActive(false);
        prevButton.gameObject.SetActive(false);
        if (sentences != null && sentences.Count > 0)
        {
            StartCoroutine(StartStoryWithDelay());
        }
        else
        {
            Debug.Log(sentences.Count + "***");
        }
    }

    private IEnumerator StartStoryWithDelay()
    {
        yield return new WaitForSeconds(startDelay);
        ShowCurrentSentence();
    }

    public void StartStory(List<string> storySentences)
    {
        sentences = storySentences;
        currentSentenceIndex = 0;
        ShowCurrentSentence();
    }

    public void NextSentence()
    {
        if (typing)
        {
            StopAllCoroutines();
            typing = false;
            messageText.text = currentMessage;
        }
        else
        {
            if (nextButtonAnimationCoroutine != null)
            {
                StopCoroutine(nextButtonAnimationCoroutine);
                nextButtonAnimationCoroutine = null;
            }

            if (currentSentenceIndex < sentences.Count - 1)
            {
                currentSentenceIndex++;
                ShowCurrentSentence();
            }

            nextButton.gameObject.SetActive(currentSentenceIndex < sentences.Count - 1);

            // Stop the animation when the button is clicked
            if (nextButtonAnimator != null)
            {
                nextButtonAnimator.enabled = false;
            }
        }
    }

    public void PreviousSentence()
    {
        if (typing)
        {
            StopAllCoroutines();
            typing = false;
            messageText.text = currentMessage;
        }
        else
        {
            if (currentSentenceIndex > 0)
            {
                currentSentenceIndex--;
                ShowCurrentSentence();
            }

            prevButton.gameObject.SetActive(currentSentenceIndex > 0);
        }
    }

    private void ShowCurrentSentence()
    {
        currentMessage = sentences[currentSentenceIndex];

        if (animateText)
        {
            StartCoroutine(WriteTextToTextmesh(currentMessage, messageText));
        }
        else
        {
            messageText.text = currentMessage;
        }

        // Update button visibility
        prevButton.gameObject.SetActive(currentSentenceIndex > 0);
        nextButton.gameObject.SetActive(currentSentenceIndex < sentences.Count - 1);
        if (currentSentenceIndex < sentences.Count - 1 == false)
        {
            StartButtonHighlighter.SetActive(true);
        }
    }

    IEnumerator WriteTextToTextmesh(string _text, TextMeshProUGUI _textMeshObject)
    {
        typing = true;

        // Pause the next button animation while typing
        if (nextButtonAnimator != null)
        {
            nextButtonAnimator.enabled = false;
        }

        _textMeshObject.text = "";
        char[] _letters = _text.ToCharArray();

        float _speed = 1f - textAnimationSpeed;

        foreach (char _letter in _letters)
        {
            _textMeshObject.text += _letter;

            if (_textMeshObject.text.Length == _letters.Length)
            {
                typing = false;

                // Resume the next button animation after typing is finished
                if (nextButtonAnimator != null && currentSentenceIndex < sentences.Count - 1)
                {
                    nextButtonAnimator.enabled = true;
                }
            }

            yield return new WaitForSeconds(0.1f * _speed);
        }
    }

}