using UnityEngine;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using TMPro;
using UnityEngine.UI;

[ExecuteAlways]
public class Quizz : MonoBehaviour
{
    [Serializable]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public struct Question
    {
        public string questionText;
        public string answerA;
        public string answerB;
        public string answerX;
        public string answerY;
        public string[] correctAnswers;
        public string feedbackCorrect;
        public string feedbackWrong;

        [JsonPropertyName("question")] public string Text { get => questionText; set => questionText = value; }
        [JsonPropertyName("a")] public string AnswerA { get => answerA; set => answerA = value; }
        [JsonPropertyName("b")] public string AnswerB { get => answerB; set => answerB = value; }
        [JsonPropertyName("x")] public string AnswerX { get => answerX; set => answerX = value; }
        [JsonPropertyName("y")] public string AnswerY { get => answerY; set => answerY = value; }
        [JsonPropertyName("answer")] public string[] CorrectAnswers { get => correctAnswers; set => correctAnswers = value; }
        [JsonPropertyName("right")] public string FeedbackCorrect { get => feedbackCorrect; set => feedbackCorrect = value; }
        [JsonPropertyName("wrong")] public string FeedbackWrong { get => feedbackWrong; set => feedbackWrong = value; }
    }

    public float startDelay = 2f;
    public Question[] questions;
    public int currentQuestionIndex;
    public float answerTime;
    public float feedbackTime;
    public float transitionTime;
    public Color normalColor = Color.white;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    [Header("GUI")]
    public TMP_Text questionDisplay;
    public Slider timerSlider;
    public TMP_Text answerDisplayA;
    public TMP_Text answerDisplayB;
    public TMP_Text answerDisplayX;
    public TMP_Text answerDisplayY;
    [Header("JSON")]
    public string fileName = "quizz.json";

    private MiniGameScore score;
    private SliderCounter aSlider;
    private SliderCounter bSlider;
    private SliderCounter xSlider;
    private SliderCounter ySlider;
    private int startingValueA;
    private int startingValueB;
    private int startingValueX;
    private int startingValueY;
    private float timer;
    private bool correct;

    static public JsonSerializerOptions JsonOptions => new JsonSerializerOptions()
    {
        WriteIndented = true,        
    };

    private void Awake()
    {
        score = GetComponent<MiniGameScore>();
        if (answerDisplayA != null) aSlider = answerDisplayA.transform.parent.GetComponentInChildren<SliderCounter>(true);
        if (answerDisplayB != null) bSlider = answerDisplayB.transform.parent.GetComponentInChildren<SliderCounter>(true);
        if (answerDisplayX != null) xSlider = answerDisplayX.transform.parent.GetComponentInChildren<SliderCounter>(true);
        if (answerDisplayY != null) ySlider = answerDisplayY.transform.parent.GetComponentInChildren<SliderCounter>(true);
        LoadQuizz();
        timer = -startDelay;
    }

    private void Update()
    {
        if (timer < 0f)
        {
            ShowStartScreen();
            ResetAnswerCount();
            if (Application.isPlaying) timer += Time.deltaTime;
        }
        else if (currentQuestionIndex >= 0 && currentQuestionIndex < questions.Length)
        {
            Question current = questions[currentQuestionIndex];
            if (Application.isPlaying) timer += Time.deltaTime;
            if (timer < answerTime) ShowQuestion(current);
            else if (timer < answerTime + feedbackTime) ShowFeedback(current);
            else if (timer < answerTime + feedbackTime + transitionTime) ShowTransition();
            else NextQuestion();
        }
        else
            NoQuestion();
    }

    private void ResetAnswerCount()
    {
        startingValueA = AudienceInputSource.Current.GetButton("a").totalPresses;
        startingValueB = AudienceInputSource.Current.GetButton("b").totalPresses;
        startingValueX = AudienceInputSource.Current.GetButton("x").totalPresses;
        startingValueY = AudienceInputSource.Current.GetButton("y").totalPresses;
        aSlider.value = 0;
        bSlider.value = 0;
        xSlider.value = 0;
        ySlider.value = 0;
    }

    private void SliderBalance()
    {
        int valueSum = 0;
        valueSum += ySlider.value;
        valueSum += xSlider.value;
        valueSum += bSlider.value;
        valueSum += aSlider.value;
        ySlider.maxValue = valueSum;
        xSlider.maxValue = valueSum;
        bSlider.maxValue = valueSum;
        aSlider.maxValue = valueSum;
    }

    private void SaveQuizz()
    {
        string dataString = JsonSerializer.Serialize(questions, JsonOptions);
        File.WriteAllText(Application.dataPath + "/" + fileName, dataString);
    }

    private void LoadQuizz()
    {
        string dataString = File.ReadAllText(Application.dataPath + "/" + fileName);
        object dataObject = JsonSerializer.Deserialize(dataString, typeof(Question[]), JsonOptions);
        if (dataObject != null && dataObject is Question[]) questions = dataObject as Question[];
    }

    public void ShowStartScreen()
    {
        if (questionDisplay != null) questionDisplay.text = "";
        if (answerDisplayA != null)
        {
            answerDisplayA.text = "";
            answerDisplayA.color = normalColor;
        }
        if (answerDisplayB != null)
        {
            answerDisplayB.text = "";
            answerDisplayB.color = normalColor;
        }
        if (answerDisplayX != null)
        {
            answerDisplayX.text = "";
            answerDisplayX.color = normalColor;
        }
        if (answerDisplayY != null)
        {
            answerDisplayY.text = "";
            answerDisplayY.color = normalColor;
        }
        ySlider.value = 0;
        xSlider.value = 0;
        bSlider.value = 0;
        aSlider.value = 0;
    }

    public void ShowQuestion(Question question)
    {
        if (questionDisplay != null) questionDisplay.text = question.questionText;
        if (answerDisplayA != null)
        {
            answerDisplayA.text = question.answerA;
            answerDisplayA.color = normalColor;
        }
        if (answerDisplayB != null)
        {
            answerDisplayB.text = question.answerB;
            answerDisplayB.color = normalColor;
        }
        if (answerDisplayX != null)
        {
            answerDisplayX.text = question.answerX;
            answerDisplayX.color = normalColor;
        }
        if (answerDisplayY != null)
        {
            answerDisplayY.text = question.answerY;
            answerDisplayY.color = normalColor;
        }
        ySlider.value = AudienceInputSource.Current.GetButton("y").totalPresses - startingValueY;
        xSlider.value = AudienceInputSource.Current.GetButton("x").totalPresses - startingValueX;
        bSlider.value = AudienceInputSource.Current.GetButton("b").totalPresses - startingValueB;
        aSlider.value = AudienceInputSource.Current.GetButton("a").totalPresses - startingValueA;
        SliderBalance();

        if (timerSlider != null)
        {
            timerSlider.maxValue = answerTime;
            timerSlider.value = timer;
        }
    }

    public void ShowFeedback(Question question)
    {
        correct = false;
        int maxValue = Mathf.Max(ySlider.value, xSlider.value, aSlider.value, bSlider.value);        
        if (question.correctAnswers != null)
        {
            foreach (string correctAnswer in question.correctAnswers)
            {
                if (correctAnswer == "a" && maxValue == aSlider.value)
                {
                    correct = true;
                    answerDisplayA.color = correctColor;
                }
                if (correctAnswer == "b" && maxValue == bSlider.value)
                {
                    correct = true;
                    answerDisplayB.color = correctColor;
                }
                if (correctAnswer == "x" && maxValue == xSlider.value)
                {
                    correct = true;
                    answerDisplayX.color = correctColor;
                }
                if (correctAnswer == "y" && maxValue == ySlider.value)
                {
                    correct = true;
                    answerDisplayY.color = correctColor;
                }
            }
        }
        if (correct)
        {
            if (questionDisplay != null) questionDisplay.text = question.feedbackCorrect;
        }
        else
        {
            if (questionDisplay != null) questionDisplay.text = question.feedbackWrong;
            if (maxValue == aSlider.value) answerDisplayA.color = wrongColor;
            if (maxValue == bSlider.value) answerDisplayB.color = wrongColor;
            if (maxValue == xSlider.value) answerDisplayX.color = wrongColor;
            if (maxValue == ySlider.value) answerDisplayY.color = wrongColor;
        }

        if (timerSlider != null)
        {
            timerSlider.maxValue = feedbackTime;
            timerSlider.value = feedbackTime - timer + answerTime;
        }
    }

    public void ShowTransition()
    {
        if (questionDisplay != null) questionDisplay.text = "";
        if (answerDisplayA != null) answerDisplayA.text = "";
        if (answerDisplayB != null) answerDisplayB.text = "";
        if (answerDisplayX != null) answerDisplayX.text = "";
        if (answerDisplayY != null) answerDisplayY.text = "";

        if (timerSlider != null) timerSlider.value = 0f;
    }

    public void NextQuestion()
    {
        if (currentQuestionIndex < questions.Length)
        {
            currentQuestionIndex++;
            ResetAnswerCount();
            timer = 0f;
            if (correct)
            {
                score.unitValue++;
                correct = false;
            }
        }
    }

    public void NoQuestion()
    {
        if (questionDisplay != null) questionDisplay.text = "Fin";
        if (answerDisplayA != null)
        {
            answerDisplayA.text = "";
            answerDisplayA.color = normalColor;
        }
        if (answerDisplayB != null)
        {
            answerDisplayB.text = "";
            answerDisplayB.color = normalColor;
        }
        if (answerDisplayX != null)
        {
            answerDisplayX.text = "";
            answerDisplayX.color = normalColor;
        }
        if (answerDisplayY != null)
        {
            answerDisplayY.text = "";
            answerDisplayY.color = normalColor;
        }
        ySlider.value = 0;
        xSlider.value = 0;
        bSlider.value = 0;
        aSlider.value = 0;
        SliderBalance();
    }
}
