using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rnd = UnityEngine.Random;
using KModkit;
using KeepCoding;
public class TwoNybbles : ModuleScript
{
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    [SerializeField] internal KMSelectable[] SpecialButtons;
    [SerializeField] internal KMSelectable[] Buttons;
    [SerializeField] private TextMesh display; 

    private string[] buttonTexts = new string[12];
    private List<int> queriedButtons = new List<int>();
    bool processing;

    void Start()
    {
        for (int i = 0; i < 12; i++)
        {
            buttonTexts[i] = Alphabet.PickRandom().ToString() + Helper.Decimal.PickRandom();
            Buttons[i].GetComponentInChildren<TextMesh>().text = buttonTexts[i];
        }
        Buttons.Assign(onInteract: x => OnButtonPress(Buttons.IndexOf<KMSelectable>(x)));
        SpecialButtons.Assign(onInteract: x => OnSpecialButtonPress(SpecialButtons.IndexOf<KMSelectable>(x)));
    }

    void OnButtonPress(int index)
    {
        ButtonEffect(Buttons[index], 1, Sound.ButtonPress);
        StartCoroutine(AnimKeys(Buttons[index], new Vector3(0f, 0f, 0.001f)));

        if (IsSolved || processing || queriedButtons.Count >= 3)
            return;

            if (queriedButtons.Count == 0)
                display.text = "QUERY:";
       
        display.text += " " + buttonTexts[index];
        queriedButtons.Add(index);
    }

    void OnSpecialButtonPress(int index)
    {
        ButtonEffect(Buttons[index], 1, Sound.ButtonPress);
        StartCoroutine(AnimKeys(SpecialButtons[index], new Vector3(0f, 0.001f, 0f)));

        if (IsSolved || processing || queriedButtons.Count == 0)
            return;    

            PlaySound("Processing");
            StartCoroutine(ProcessingAnimation(index));    
    }

    string GetQueryResult()
    {
        string result = "F4 20";
        List<String> query = new List<String>();
        queriedButtons.ForEach(x => query.Add(buttonTexts[x]));
        Log("You queried {0}, which returned {1}.", query, result);
        return result;
    }

    void HandleSubmission()
    {
        //Dummy while module logic is not implemented.
        List<String> query = new List<String>();
        queriedButtons.ForEach(x => query.Add(buttonTexts[x]));
        Log("You submitted {0}.", query.UnwrapToString(false, " "));

        if (IsSubmissionCorrect())
        {
            Solve("That was correct. Module solved.");
            PlaySound(KMSoundOverride.SoundEffect.CorrectChime);
            display.text = "CORRECT. SOLVED";
            return;
        }

            Strike("That was incorrect. Strike!");
            display.text = "WRONG. STRIKE.";
    }

    bool IsSubmissionCorrect()
    {
        //Dummy while module logic is not implemented.
        return false;
    }

    IEnumerator ProcessingAnimation(int state)
    {
        Debug.Log(queriedButtons.Count());
        int periodCount = 0;
        List<float> delayTimes = Enumerable.Repeat(5 / 12f, 12).ToList();

        for (int i = 0; i < 12; i += 2)
        {
            float param = Rnd.Range(0f, 0.4f);
            delayTimes[i] += param;
            delayTimes[i + 1] -= param;
        }

        delayTimes.Shuffle();

        for (int i = 0; i < 12; i++)
        {
            display.text = "PROCCESING" + (Enumerable.Repeat(".", periodCount)).Join();
            periodCount++;
            if (periodCount == 4) periodCount = 0;
            yield return new WaitForSeconds(delayTimes[i]);
        }

        switch (state)
        {
            case 0:
                display.text = "RESULT: " + GetQueryResult();
                break;
            case 1:
                HandleSubmission();
                break;
        }

        queriedButtons.Clear();
    }

    IEnumerator AnimKeys(KMSelectable button, Vector3 vec)
    {
        {
            for (int i = 0; i < 3; i++)
            {
                button.transform.localPosition -= vec;
                yield return new WaitForSeconds(0.02f);
            }

            for (int i = 0; i < 3; i++)
            {
                button.transform.localPosition += vec;
                yield return new WaitForSeconds(0.02f);
            }
        }
    }
}

