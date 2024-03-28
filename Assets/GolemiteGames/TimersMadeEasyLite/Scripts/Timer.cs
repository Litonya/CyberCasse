using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;


public class Timer : MonoBehaviour
{
    public UnityEvent onTimerEnd;

    [Range(0, 23)]
    public int hours;
    [Range(0, 59)]
    public int minutes;
    [Range(0, 59)]
    public int seconds;
    
    public enum CountMethod
    {
        CountDown,
        CountUp
    };
    
    public enum SeperatorType
    {
        Colon,
        Bullet,
        Slash
    };
    public enum OutputType
    {
        None,
        StandardText,
        TMPro,
        HorizontalSlider,
        Dial
    };

    [Tooltip("If checked, runs the timer on play")]
    public bool startAtRuntime = true;

    [Tooltip("Select what to display")]
    public bool hoursDisplay = false;
    public bool minutesDisplay = true;
    public bool secondsDisplay = true;

    [Space]
    
    [Tooltip("Select to count up or down")]
    public CountMethod countMethod;

    [Tooltip("Select the output type")]
    public OutputType outputType;
    public Text standardText;
    public TextMeshProUGUI textMeshProText;
    public Slider standardSlider;
    public Image dialSlider;

    bool timerRunning = false;
    bool timerPaused = false;
    public double timeRemaining;
    

    private void Awake()
    {
        if(!standardText)
        if(GetComponent<Text>())
        {
            standardText = GetComponent<Text>();
        }
        if(!textMeshProText)
        if(GetComponent<TextMeshProUGUI>())
        {
            textMeshProText = GetComponent<TextMeshProUGUI>();
        }
        if(!dialSlider)
        if(GetComponent<Image>())
        {
            dialSlider = GetComponent<Image>();
        }
        if(dialSlider)
        {
            if (countMethod == CountMethod.CountDown)
            {
                dialSlider.fillAmount = 1f;
            }
            else
            {
                dialSlider.fillAmount = 0f;
            }
        }
    }
    void Start()
    {
        if(startAtRuntime)
        {
            StartTimer();
        }
        else
        {
            if(countMethod == CountMethod.CountDown)
            {
                if(standardText)
                {
                    standardText.text = DisplayFormattedTime(ReturnTotalSeconds());
                }
                if(textMeshProText)
                {
                    textMeshProText.text = DisplayFormattedTime(ReturnTotalSeconds());
                }
            }
            else
            {
                if (standardText)
                {
                    standardText.text = DisplayFormattedTime(0);
                }
                if (textMeshProText)
                {
                    textMeshProText.text = DisplayFormattedTime(0);
                }
            }
        }
    }
    void Update()
    {
        if (GameManager.instance.GetCurrentPhase() == GameStates.Preparation) return;
        if(timerRunning)
        {
            if(countMethod == CountMethod.CountDown)
            {
                CountDown();
                
                if(dialSlider)
                {
                    DialSliderDown();
                }
            }
        }
    }

    private void CountDown()
    {
        /*If you choose to edit this back to 0 for 100% accuracy,
        1 frame at the end of the timer will display maximum numbers as it takes time to switch to the else statement
        which sets the time remaining to 0. This is accurate up to 20 milliseconds or 0.02 of a second.*/  
        if (timeRemaining > 0.02)
        {
            timeRemaining -= Time.deltaTime;
            DisplayInTextObject();
        }
        else
        {
            //Timer has ended from counting downwards
            timeRemaining = 0;
            timerRunning = false;
            onTimerEnd.Invoke();
            DisplayInTextObject();
        }
    }

    private void DialSliderDown()
    {
        // Calcul de la valeur normalisée du temps restant
        float timeRangeClamped = Mathf.InverseLerp(ReturnTotalSeconds(), 0, (float)timeRemaining);

        // Interpolation pour la couleur du slider (du vert au rouge)
        dialSlider.color = Color.Lerp(Color.red, Color.green, 1 - timeRangeClamped);

        // Mise à jour de la remplissage du slider
        dialSlider.fillAmount = Mathf.Lerp(1, 0, timeRangeClamped);
    }

    private void DisplayInTextObject()
    {
        if (standardText)
        {
            standardText.text = DisplayFormattedTime(timeRemaining);
        }
        if (textMeshProText)
        {
            textMeshProText.text = DisplayFormattedTime(timeRemaining);
        }
    }

    public double GetRemainingSeconds()
    {
        return timeRemaining;
    }
    public void StartTimer()
    {
        if (!timerRunning && !timerPaused)
        {
            ResetTimer();
            timerRunning = true;
            if (countMethod == CountMethod.CountDown)
            {
                ConvertToTotalSeconds(hours, minutes, seconds);
            }
            else
            {
                StartTimerCustom(0);
            }
        }
    }
    private void StartTimerCustom(double timeToSet)
    {
        if(!timerRunning && !timerPaused)
        {
            timeRemaining = timeToSet;
            timerRunning = true;
        }
    }
    public void StopTimer()
    {
        timerRunning = false;
        ResetTimer();
    }

    private void ResetTimer()
    {
        timerPaused = false;
        
        if (countMethod == CountMethod.CountDown)
        {
            timeRemaining = ReturnTotalSeconds();
            DisplayInTextObject();
            if(standardSlider)
            {
                standardSlider.maxValue = ReturnTotalSeconds();
                standardSlider.value = standardSlider.maxValue;
            }
            if(dialSlider)
            {
                dialSlider.fillAmount = 1f;
            }
        }
        else
        {
            timeRemaining = 0;
            DisplayInTextObject();
            if (standardSlider)
            {
                standardSlider.maxValue = ReturnTotalSeconds();
                standardSlider.value = standardSlider.minValue;
            }
            if (dialSlider)
            {
                dialSlider.fillAmount = 0f;
            }
        }
    }

    public float ReturnTotalSeconds()
    {
        float totalTimeSet;
        totalTimeSet = hours * 60 * 60;
        totalTimeSet += minutes * 60;
        totalTimeSet += seconds;
        return totalTimeSet;
    }
   
    public double ConvertToTotalSeconds(float hours, float minutes, float seconds)
    {
        timeRemaining = hours * 60 * 60;
        timeRemaining += minutes * 60;
        timeRemaining += seconds;

        DisplayFormattedTime(timeRemaining);
        return timeRemaining;
    }
    public string DisplayFormattedTime(double remainingSeconds)
    {
        string convertedNumber;
        float hours, minutes, seconds;
        RemainingSecondsToHHMMSSMMM(remainingSeconds, out hours, out minutes, out seconds);

        string HoursFormat()
        {
            if (hoursDisplay)
            {
                string hoursFormatted;
                hoursFormatted = string.Format("{0:00}", hours);
                if (minutesDisplay || secondsDisplay)
                    hoursFormatted += ":";
                return hoursFormatted;
            }
            return null;
        }
        string MinutesFormat()
        {
            if (minutesDisplay)
            {
                string minutesFormatted;
                minutesFormatted = string.Format("{0:00}", minutes);
                if (secondsDisplay)
                    minutesFormatted += ":";
                return minutesFormatted;
            }
            return null;
        }
        string SecondsFormat()
        {
            if (secondsDisplay)
            {
                string secondsFormatted; 
                secondsFormatted = string.Format("{0:00}", seconds);              
                return secondsFormatted;
            }
            return null;
        }
        

        convertedNumber = HoursFormat() + MinutesFormat() + SecondsFormat();

        return convertedNumber;
    }

    public void AddTime(float additionalHours, float additionalMinutes, float additionalSeconds)
    {
        double additionalTimeInSeconds = additionalHours * 3600 + additionalMinutes * 60 + additionalSeconds;

        // Ajouter le temps au temps restant
        timeRemaining += additionalTimeInSeconds;

        // Mettre à jour l'affichage du temps
        StartTimerCustom(timeRemaining);
        DisplayInTextObject();
    }

    public void SetTimerTimeStart(float startingTime)
    {
        double totalHours = Mathf.Floor(startingTime / 3600f);
        double totalMinutes = Mathf.Floor((startingTime - (float)totalHours * 3600f) / 60f);
        double totalSeconds = startingTime - (float)totalHours * 3600f - (float)totalMinutes * 60f;

        // Mise à jour des variables hours, minutes et seconds
        hours = (int)totalHours;
        minutes = (int)totalMinutes;
        seconds = (int)totalSeconds;

        // Définir le temps de départ
        timeRemaining = startingTime;

        // Mettre à jour l'affichage du temps
        DisplayInTextObject();
    }


    private static void RemainingSecondsToHHMMSSMMM(double totalSeconds, out float hours, out float minutes, out float seconds)
    {
        hours = Mathf.FloorToInt((float)totalSeconds / 60 / 60);
        minutes = Mathf.FloorToInt(((float)totalSeconds / 60) - ((float)hours * 60));
        seconds = Mathf.FloorToInt((float)totalSeconds - ((float)hours * 60 * 60) - ((float)minutes * 60));
    }
    private void OnValidate()
    {
        timeRemaining = ConvertToTotalSeconds(hours, minutes, seconds);
    }
}
