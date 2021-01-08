﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ForgetsUltimateShowdownModule;
using rnd = UnityEngine.Random;

public partial class ForgetsUltimateShowdownScript : MonoBehaviour
{
    public KMBombModule Module;
    public KMAudio Audio;
    public KMBombInfo Info;

    public KMSelectable[] NumberButtons; // 1-9 then 0

    public TextMesh[] ModuleTexts; //0 small display, 1 big display

    public GameObject[] ForgetMeNotObjects;
    public GameObject[] ForgetEverythingObjects;
    public GameObject[] ForgetMeLaterObjects;
    public GameObject[] SimonsStagesObjects;
    public GameObject[] ForgetInfinityObjects;
    public GameObject[] ANDObjects;
    public GameObject[] ForgetMeNowObjects;

    public AudioClip[] SFX; //0 FML
    
    public GameObject EncProcess;
    public GameObject ButtonsObject;

    private FUSLogger _logger;
    
    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private bool _isSolved;
    private bool _animating;

    private bool _interactable
    {
        get
        {
            return !(_isSolved || _animating);
        }
    }
    
    private static readonly List<Color> _forgetEverythingLedColors = new List<Color>
    {
        new Color(0.7f, 0, 0, 1),
        new Color(0.5f, 0.5f, 0, 1),
        new Color(0, 0.6f, 0, 1),
        new Color(0, 0.3f, 1, 1)
    };
    
    private static readonly List<char> _andLogicGateChars = new List<char>
    {
        '∧','∨','⊻','|','↓','↔','→','←','¬'
    };

    private string _initialNumber;
    private string _bottomNumber;
    private string _answer; 
    private List<IFUSComponentSolver> _usedMethods = new List<IFUSComponentSolver>();
    private List<GameObject> _usedObjects = new List<GameObject>();

    private Pair<string, string> _initialNumbers;

    private Coroutine _forgetMeLaterCycle;
    private bool _fmlCycleRunning;
    
    private Coroutine _andCycle;
    private bool _andCycleRunning;

    private bool _showingInfo;
    private bool _hasBeenStarted;

    private string _currentInputtedText = "------------";

    private int _pressIndex;
    private List<int> _presses = new List<int>();

    private const string _version = "1.02";

    // Use this for initialization
    void Start()
    {
        _moduleId = _moduleIdCounter++;
        _logger = new FUSLogger(_moduleId);
        Module.OnActivate += Activate;
        Audio.PlaySoundAtTransform(SFX[0].name, Module.transform);
    }

    private void Activate()
    {
        _logger.LogMessage("Running version {0}", _version);
        for (var i = 0; i < NumberButtons.Length; i++)
        {
            NumberButtons[i].OnInteract += ButtonHandler(i);
        }

        ResetAndGenerate();
    }

    private void ResetAndGenerate()
    {
        _hasBeenStarted = false;
        var methods = new List<IFUSComponentSolver>();
        var componentInfo = new ComponentInfo();
        _usedObjects.ForEach(x => x.SetActive(false));
        _usedObjects = new List<GameObject>();
        _presses = new List<int>();
        var possible = new List<IFUSComponentSolver>
        {
            new ForgetMeNotComponent(),
            new ForgetMeLaterComponent(),
            new ANDComponent(),
            new ForgetMeNowComponent(),
            new ForgetInfinityComponent(),
            new ForgetEverythingComponent(),
            new SimonsStagesComponent()
        };
        
        for (var i = 0; i < 6; i++)
        {
            var method = possible.PickRandom();
            possible.Remove(method);
            methods.Add(method);
        }

        _usedMethods = methods;
        _initialNumber = GenerateNumber();
        _bottomNumber = GenerateNumber();

        _logger.LogMessage("The initial number is: {0}", _initialNumber);
        _logger.LogMessage("The bottom number is: {0}", _bottomNumber);
        _logger.LogMessage("The chosen methods are: {0}", methods.Select(x => x.Name).Join(", "));

        for (var i = 0; i < 6; i++)
        {
            var obj = GetObject(_usedMethods[i], i);
            obj.SetActive(true);
            _usedObjects.Add(obj);
            
            switch (_usedMethods[i].Id)
            {
                case MethodId.ForgetMeNot:
                    _logger.LogMessage("Forget Me Not:");
                    _logger.LogMessage("There isn't anything you need to know here...");
                    break;
                case MethodId.ForgetInfinity:
                    var ignored1 = rnd.Range(1, 13);
                    var ignored2 = rnd.Range(1, 13);
                    _logger.LogMessage("Forget Infinity:");
                    _logger.LogMessage("The two ignores numbers were {0} and {1}.", ignored1, ignored2);
                    while (ignored1 == ignored2)
                    {
                        ignored2 = rnd.Range(1, 13);
                    }
                    componentInfo.IgnoredNumbers = new Pair<int, int>(ignored1, ignored2);
                    obj.GetComponentInChildren<TextMesh>().text = string.Format("{0} / {1}", ignored1, ignored2);
                    break;
                case MethodId.ForgetMeNow:
                    var startingNumber = rnd.Range(0, 100);
                    var startingDigit = DigitalRoot(startingNumber);
                    _logger.LogMessage("Forget Me Now:");
                    _logger.LogMessage("The starting number is {0}, which causes the number used to start the module to be {1}.", startingNumber.ToString("D2"), startingDigit);
                    componentInfo.StartingNumber = startingDigit;
                    obj.GetComponentInChildren<TextMesh>().text = startingNumber.ToString("D2");
                    break;
                case MethodId.SimonsStages:
                    var chosenColor = (SimonsStagesColor) rnd.Range(0, 6);
                    obj.GetComponentInChildren<TextMesh>().text = chosenColor.ToString().First().ToString();
                    componentInfo.SimonsStagesColor = chosenColor;
                    _logger.LogMessage("Simon's Stages:");
                    _logger.LogMessage("The rule chosen is {0}.", (chosenColor == SimonsStagesColor.Red ? "Red(nice)" : chosenColor.ToString()));
                    var lights = obj.GetComponentsInChildren<Light>();
                    var scalar = transform.lossyScale.x;
                    foreach (var l in lights)
                    {
                        l.range *= scalar;
                        l.enabled = false;
                    }

                    switch (chosenColor)
                    {
                        case SimonsStagesColor.Blue:
                            lights[1].enabled = true;
                            break;
                        case SimonsStagesColor.Cyan:
                            lights[8].enabled = true;
                            break;
                        case SimonsStagesColor.Lime:
                            lights[7].enabled = true;
                            break;
                        case SimonsStagesColor.Pink:
                            lights[6].enabled = true;
                            break;
                        case SimonsStagesColor.Red:
                            lights[0].enabled = true;
                            break;
                        case SimonsStagesColor.White:
                            lights[9].enabled = true;
                            break;
                    }
                    break;
                case MethodId.ForgetMeLater:
                    var ruleNums = Enumerable.Range(0, 60).ToList().Shuffle().Take(12).ToList();
                    //ruleNums = new List<int>{57, 57, 57, 57,57, 57,57,57,57, 57,57,57};
                    _logger.LogMessage("Forget Me Later:");
                    _logger.LogMessage("The chosen rules are: {0}.", ruleNums.Join(", "));
                    componentInfo.Rules = ruleNums;
                    if (_fmlCycleRunning)
                    {
                        _fmlCycleRunning = false;
                        StopCoroutine(_forgetMeLaterCycle);
                    }
                    _forgetMeLaterCycle = null;
                    _forgetMeLaterCycle =
                        StartCoroutine(ForgetMeLaterCycle(ruleNums, obj.GetComponentInChildren<TextMesh>()));
                    break;
                case MethodId.AND:
                    var logicGates = new List<ANDLogicGate>();
                    for (var j = 0; j < 12; j++)
                    {
                        logicGates.Add((ANDLogicGate)rnd.Range(0,9));
                    }
                    componentInfo.LogicGates = logicGates;
                    
                    _logger.LogMessage("A>N<D:");
                    _logger.LogMessage("The chosen logic gates are: {0}", logicGates.Join(", "));
                    
                    if (_andCycleRunning)
                    {
                        _andCycleRunning = false;
                        StopCoroutine(_andCycle);
                    }
                    _andCycle = null;
                    _andCycle =
                        StartCoroutine(ANDCycle(logicGates, obj.GetComponentInChildren<TextMesh>()));
                    break;
                case MethodId.ForgetEverything:
                    var colors = new List<ForgetEverythingColor>
                    {
                        (ForgetEverythingColor) rnd.Range(0, 4), 
                        (ForgetEverythingColor) rnd.Range(0, 4),
                        (ForgetEverythingColor) rnd.Range(0, 4)
                    };
                    
                    _logger.LogMessage("Forget Everything:");
                    _logger.LogMessage("The chosen colors are: {0}", colors.Join(", "));
                    componentInfo.FEColors = colors;
                    obj.GetComponent<MeshRenderer>().materials[1].color = _forgetEverythingLedColors[(int)colors[0]];
                    obj.GetComponent<MeshRenderer>().materials[2].color = _forgetEverythingLedColors[(int)colors[1]];
                    obj.GetComponent<MeshRenderer>().materials[3].color = _forgetEverythingLedColors[(int)colors[2]];
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Invalid id {0}", _usedMethods[i].Id));
            }
        }
        
        var numberInfo = new NumberInfo(_initialNumber, _bottomNumber, _logger);
        _initialNumbers = new Pair<string, string>(numberInfo.Number, numberInfo.BottomNumber);

        for (var i = 0; i < 6; i++)
        {
            numberInfo.Number = methods[i].Encrypt(Info, componentInfo, numberInfo);
            _logger.LogMessage("After step {0}({1}) of the encryption, the number is: {2}", i + 1, methods[i].Name, numberInfo.Number);
            if (numberInfo.Number.Length != 12)
            {
                throw new InvalidOperationException("Invalid length of a number, please report this logfile to Marksam!");
            }
        }
        _answer = numberInfo.Number;
        _logger.LogMessage("The final answer to submit is: {0}", _answer);
    }

    private KMSelectable.OnInteractHandler ButtonHandler(int i)
    {
        return delegate
        {
            if (!_interactable)
            {
                return false;
            }
            
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, NumberButtons[i].transform);
            NumberButtons[i].AddInteractionPunch(0.2f);

            switch (i)
            {
                case 9: //0
                    if (_showingInfo)
                    {
                        StartCoroutine(SubmitModeAnimation());
                    }
                    else if (!_hasBeenStarted)
                    {
                        StartCoroutine(StartAnimation());
                    }
                    else
                    {
                        HandlePress(0);   
                    }
                    break;
                default:
                    if (!_hasBeenStarted)
                    {
                        break;
                    }
                    HandlePress(i+1);
                    break;
            }
            return false;
        };
    }

    private void HandlePress(int i)
    {
        _logger.LogMessage("You pressed {0}, expected {1}.", i, _answer[_pressIndex].ToString());
        _presses.Add(i);
        if (_presses[_pressIndex] != int.Parse(_answer[_pressIndex].ToString()))
        {
            StartCoroutine(ClearText());
            _logger.LogMessage("That is incorrect, strike and reset!");
            _pressIndex = 0;
            Module.HandleStrike();
            Audio.PlaySoundAtTransform(SFX[6].name, Module.transform);
            _currentInputtedText = "------------";
            ResetAndGenerate();
            return;
        }
        if (_pressIndex == 0)
        {
            Audio.PlaySoundAtTransform(SFX[5].name, Module.transform);
        }
        _logger.LogMessage("That is correct.");
        AddToDashedText(i, _pressIndex);
        _pressIndex++;

        if (_presses.Join("") == _answer)
        {
            _logger.LogMessage("Module solved!");
            _isSolved = true;
            Module.HandlePass();
            Audio.PlaySoundAtTransform(SFX[rnd.Range(3,5)].name, Module.transform);
            StartCoroutine(DoSmallDisplaySolve());
            StartCoroutine(ClearText());
        }
    }

    private GameObject GetObject(IFUSComponentSolver method, int position)
    {
        switch (method.Id)
        {
            case MethodId.ForgetMeNot:
                return ForgetMeNotObjects[position];
            case MethodId.ForgetInfinity:
                return ForgetInfinityObjects[position];
            case MethodId.ForgetMeNow:
                return ForgetMeNowObjects[position];
            case MethodId.SimonsStages:
                return SimonsStagesObjects[position];
            case MethodId.ForgetMeLater:
                return ForgetMeLaterObjects[position];
            case MethodId.AND:
                return ANDObjects[position];
            case MethodId.ForgetEverything:
                return ForgetEverythingObjects[position];
            default:
                throw new InvalidOperationException(string.Format("Invalid name {0}", method.Name));
        }
    }

    private static string GenerateNumber()
    {
        var initialNumber = rnd.Range(0, 10).ToString();
        for (int i = 0; i < 11; i++)
        {
            initialNumber += rnd.Range(0, 10);
        }
        return initialNumber;
    }

    private void AddToDashedText(int number, int inputIndex)
    {
        var text = _currentInputtedText.ToArray().Select(x => x.ToString()).ToArray();

        text[inputIndex] = number.ToString();
        _currentInputtedText = text.Join("");
        ModuleTexts[1].text = _currentInputtedText.Insert(3, " ").Insert(7, " ").Insert(11, " ");
    }
    
    private static int DigitalRoot(int n)
    {
        var root = 0;
        while (n > 0 || root > 9)
        {
            if (n == 0)
            {
                n = root;
                root = 0;
            }

            root += n % 10;
            n /= 10;
        }
        return root;
    }

    private string GetNumberDisplay(Pair<string, string> numbers)
    {
        var builder = new StringBuilder();
        var initial = numbers.Item1;
        var bottom = numbers.Item2;

        for (int i = 0; i < 4; i++)
        {
            builder.Append(initial.Take(3).Join(""));
            initial = initial.Remove(0, 3);
            builder.Append(" ");
        }

        builder.AppendLine();
        
        for (int i = 0; i < 4; i++)
        {
            builder.Append(bottom.Take(3).Join(""));
            bottom = bottom.Remove(0, 3);
            builder.Append(" ");
        }

        return builder.ToString();
    }

    private string GetDashDisplay()
    {
        var builder = new StringBuilder("---");
        for (int i = 0; i < 3; i++)
        {
            builder.Append(" ");
            builder.Append("---");
        }
        
        return builder.ToString();
    }

    private IEnumerator ForgetMeLaterCycle(List<int> rules, TextMesh textMesh)
    {
        _fmlCycleRunning = true;
        while (true)
        {
            textMesh.text = string.Empty;
            yield return new WaitForSeconds(0.8f);
            for (var i = 0; i < 12; i++)
            {
                textMesh.text = rules[i].ToString();
                yield return new WaitForSeconds(0.8f);
            }
        }
    }
    
    private IEnumerator ANDCycle(List<ANDLogicGate> gates, TextMesh textMesh)
    {
        _andCycleRunning = true;
        while (true)
        {
            textMesh.text = string.Empty;
            yield return new WaitForSeconds(1f);
            for (var i = 0; i < 12; i++)
            {
                textMesh.text = _andLogicGateChars[(int)gates[i]].ToString();
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private IEnumerator StartAnimation()
    {
        _animating = true;
        _showingInfo = true;
        _hasBeenStarted = true;
        Audio.PlaySoundAtTransform(SFX[1].name, Module.transform);
        StartCoroutine(DoSmallDisplayShuffle());
        EncProcess.transform.localPosition = new Vector3(-0.02323988f, 0.0119f, 0.02150016f);

        for (var i = 0; i < 100; i++)
        {
            ButtonsObject.transform.localPosition -= new Vector3(0f, 0.0000659584f, 0f);
            yield return new WaitForSeconds(.01f);
        }

        EncProcess.SetActive(true);
        ButtonsObject.SetActive(false);
        yield return new WaitForSeconds(1.5f);

        for (var i = 0; i < 100; i++)
        {
            EncProcess.transform.localPosition += new Vector3(0f, 0.0000609584f, 0f);
            yield return new WaitForSeconds(.01f);
        }
        yield return new WaitForSeconds(1f);
        StartCoroutine(WriteText());
    }

    private IEnumerator SubmitModeAnimation()
    {
        _animating = true;
        _showingInfo = false;
        Audio.PlaySoundAtTransform(SFX[1].name, Module.transform);
        StartCoroutine(DoSmallDisplayShuffle());
        for (var i = 0; i < 100; i++)
        {
            EncProcess.transform.localPosition -= new Vector3(0f, 0.0000609584f, 0f);
            yield return new WaitForSeconds(.01f);
        }

        EncProcess.SetActive(false);
        ButtonsObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        for (var i = 0; i < 100; i++)
        {
            ButtonsObject.transform.localPosition += new Vector3(0f, 0.0000659584f, 0f);
            yield return new WaitForSeconds(.01f);
        }
        
        yield return new WaitForSeconds(1f);
        StartCoroutine(ClearOnSumbitModeText());
    }

    private IEnumerator DoSmallDisplayShuffle()
    {
        var cycle = Enumerable.Range(0,100).ToList().Shuffle().Select(x => x.ToString("D2")).ToList();
        for (int i = 0; i < 350; i++)
        {
            ModuleTexts[0].text = cycle[i % 100];
            yield return new WaitForSeconds(.01f);
        }
        ModuleTexts[0].text = "--";
    }

    private IEnumerator DoSmallDisplaySolve()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(1f);
            ModuleTexts[0].gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
            ModuleTexts[0].gameObject.SetActive(true);
        }
        ModuleTexts[0].text = string.Empty;
    }

    private IEnumerator WriteText()
    {
        var cycle = GetNumberDisplay(_initialNumbers);
        var text = string.Empty;
        Audio.PlaySoundAtTransform(SFX[2].name, Module.transform);
        foreach (var t in cycle)
        {
            text = text + t;
            ModuleTexts[1].text = text;
            yield return new WaitForSeconds(.03f);
        }

        _animating = false;
    }

    private IEnumerator ClearText()
    {
        _animating = true;
        var currentText = ModuleTexts[1].text;
        var length = currentText.Length;
        Audio.PlaySoundAtTransform(SFX[2].name, Module.transform);
        for (var i = 0; i < length; i++)
        {
            currentText = currentText.Substring(0, currentText.Length - 1);
            ModuleTexts[1].text = currentText;
            yield return new WaitForSeconds(.03f);
        }

        _animating = false;
    }

    private IEnumerator ClearOnSumbitModeText()
    {
        var currentText = GetNumberDisplay(_initialNumbers);
        var length = currentText.Length;
        Audio.PlaySoundAtTransform(SFX[2].name, Module.transform);
        for (var i = 0; i < length; i++)
        {
            currentText = currentText.Substring(0, currentText.Length - 1);
            ModuleTexts[1].text = currentText;
            yield return new WaitForSeconds(.03f);
            if (i == 23)
            {
                Audio.PlaySoundAtTransform(SFX[2].name, Module.transform);
            }
        }

        var dashText = GetDashDisplay();
        var text = string.Empty;
        foreach (var t in dashText)
        {
            text = text + t;
            ModuleTexts[1].text = text;
            yield return new WaitForSeconds(.05f);
        }
        
        _animating = false;
    }
}