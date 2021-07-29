using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KeepCoding;

public class TwoNybblesTP: TPScript<TwoNybbles> {

    public override IEnumerator ForceSolve()
    {
        throw new NotImplementedException("Todo: Implement the autosolver!");
    }

    public override IEnumerator Process(string command)
    {
        string[] cmdArray = command.ToLowerInvariant().Split(" ");
        List<int> pressedButtons = new List<int>();

        if (!IsMatch(cmdArray[0], "(query|submit)"))
           yield break;

        yield return null;

        if (cmdArray.Length == 1)
        {
            yield return SendToChatError("Please specify which buttons to press!");
            yield break;
        }

        if (cmdArray.Length > 4)
        {
            yield return SendToChatError("Too many parameters!");
            yield break;
        }

        foreach (string i in cmdArray.Skip(1).ToArray())
        {
                int button;

                if (!int.TryParse(i.ToString(), out button))
                {
                    yield return SendToChatError(("Parameter '{0}' not a number!").Form(i));                   
                    yield break;
                }

                if (button < 1 || button > 12)
                {
                    yield return SendToChatError(("Parameter '{0}' out of range!").Form(i));
                    yield break;
                }

                pressedButtons.Add(button - 1);        
                  
        }

        yield return OnInteractSequence(Module.Buttons, 1.5f, pressedButtons.ToArray());
        yield return new WaitForSeconds(1.5f);
        int specialButton = !IsMatch(cmdArray[0], "query") ? 1 : 0;
        Module.SpecialButtons[specialButton].OnInteract();
    }
}
