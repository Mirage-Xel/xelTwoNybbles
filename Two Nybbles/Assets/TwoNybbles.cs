using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rnd = UnityEngine.Random;
using KModkit;
using KeepCoding;
public class TwoNybbles : ModuleScript
{
    public KMBombModule module;
    public KMAudio sound;

    public KMSelectable[] specialButtons;
    public KMSelectable[] buttons;
    public TextMesh display;

    string[] buttonTexts = new string[12];
    List<int> queriedButtons = new List<int>();
    bool processing;
    bool solved;


    void Start()
    {
        for (int i = 0; i < 12; i++)
        {
            buttonTexts[i] = Helper.Alphanumeric[rnd.Range(10, 36)] + Helper.Decimal[rnd.Range(0, 10)].ToString();
            buttons[i].GetComponentInChildren<TextMesh>().text = buttonTexts[i];
        }
        buttons.Assign(onInteract: x => OnButtonPress(ArrayHelper.IndexOf<KMSelectable>(buttons, x)));
        specialButtons.Assign(onInteract: x => OnSpecialButtonPress(ArrayHelper.IndexOf<KMSelectable>(specialButtons, x)));
    }

    void OnButtonPress(int index)
    {
        ButtonEffect(buttons[index], 1, KMSoundOverride.SoundEffect.ButtonPress);
        StartCoroutine(AnimKeys(buttons[index], new Vector3(0f, 0f, 0.001f)));
        if (!(solved || processing) && queriedButtons.Count < 3)
        {
            if (queriedButtons.Count == 0) display.text = "QUERY:";
            display.text += " " + buttonTexts[index]; ;
            queriedButtons.Add(index);
        }
    }

    void OnSpecialButtonPress(int index)
    {
        ButtonEffect(specialButtons[index], 1, KMSoundOverride.SoundEffect.ButtonPress);
        StartCoroutine(AnimKeys(specialButtons[index], new Vector3(0f, 0.001f, 0f)));
        if (!(solved || processing) && queriedButtons.Count != 0)
        {
            PlaySound("Processing");
            StartCoroutine(ProcessingAnimation(index));
        }
    }

    string GetQueryResult()
    {
        string result = "F4 20";
        string query = "";
        queriedButtons.ForEach(x => query += " " + buttonTexts[x]);
        Log("You queried{0}, which returned {1}.", query, result);
        return result;
    }

    void HandleSubmission()
    {
        //Dummy while module logic is not implemented.
        string query = "";
        queriedButtons.ForEach(x => query += " " + buttonTexts[x]);
        Log("You submitted{0}.", query);
        if (IsSumbissionCorrect())
        {
            Solve("That was correct. Module solved.");
            PlaySound(KMSoundOverride.SoundEffect.CorrectChime);
            display.text = "CORRECT. SOLVED";
            solved = true;
        }
        else
        {
            Strike("That was incorrect. Strike!");
            display.text = "WRONG. STRIKE.";
        }
    }

    bool IsSumbissionCorrect()
    {
        //Dummy while module logic is not implemented.
        return false;
    }

    IEnumerator ProcessingAnimation(int state)
    {
        int periodCount = 0;
        List<float> delayTimes = Enumerable.Repeat(0.416666667f, 12).ToList();
        for (int i = 0; i < 12; i += 2)
        {
            float param = rnd.Range(0f, 0.4f);
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

