using System;
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
    public GameObject[] ForgetUsNotObjects;

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

    private static readonly List<List<Pair<Vector3, float>>> _forgetEverythingPositions = new List<List<Pair<Vector3, float>>>
    {
        new List<Pair<Vector3, float>> { new Pair<Vector3, float>(new Vector3(0f, -0.3f, 0.783f), -90f), new Pair<Vector3, float>(new Vector3(-0.594f, -0.3f, 0.012f), 180f) }, //tl
        new List<Pair<Vector3, float>> { new Pair<Vector3, float>(new Vector3(0f, -0.3f, 0.783f), -90f), new Pair<Vector3, float>(new Vector3(0.594f, -0.3f, 0.012f), 0f) }, //tr
        new List<Pair<Vector3, float>> { new Pair<Vector3, float>(new Vector3(0f, -0.3f, -0.783f), 90f), new Pair<Vector3, float>(new Vector3(-0.594f, -0.3f, -0.12f), 180f) }, //bl
        new List<Pair<Vector3, float>> { new Pair<Vector3, float>(new Vector3(0f, -0.3f, -0.783f), 90f), new Pair<Vector3, float>(new Vector3(0.594f, -0.3f, -0.12f), 0f) } //br
    };
    
    private static readonly List<char> _andLogicGateChars = new List<char>
    {
        '∧','∨','⊻','|','↓','↔','→','←','¬'
    };

    private string _initialNumber;
    private string _bottomNumber;
    private string _answer;
    private SimonsStagesColor _chosenColor;
    private readonly List<IFUSComponentSolver> _usedMethods = new List<IFUSComponentSolver>();

    private Pair<string, string> _initialNumbers;

    private bool _showingInfo;
    private bool _hasBeenStarted;

    private string _currentInputtedText = "------------";

    private int _pressIndex;
    private List<int> _presses = new List<int>();

    private const string _version = "1.3";

    // Use this for initializatihon
    void Start()
    {
        _moduleId = _moduleIdCounter++;
        _logger = new FUSLogger(_moduleId);
        var possible = new List<IFUSComponentSolver>
        {
            new ForgetMeNotComponent(),
            new ForgetMeLaterComponent(),
            new ANDComponent(),
            new ForgetMeNowComponent(),
            new ForgetInfinityComponent(),
            new ForgetEverythingComponent(),
            new SimonsStagesComponent(),
            new ForgetUsNotComponent()
        };
        var methods = new List<int>();
        for (var i = 0; i < 4; i++)
        {
            var method = rnd.Range(0, 8);
            while(methods.Contains(method) || method == 1 && methods.Contains(2) || method == 2 && methods.Contains(1))
            {
                method = rnd.Range(0, 8);
            }
            methods.Add(method);
        }

        var simonStagesColor = (SimonsStagesColor) rnd.Range(0, 6);
        if ((methods.Contains(7) && methods.IndexOf(7) > 1))
        {
            var method = rnd.Range(0, 2);
            var previous = methods.IndexOf(7);
            var first = methods[method];
            methods[method] = methods[previous];
            methods[previous] = first;
        }

        if (methods.Contains(6) && new[]{SimonsStagesColor.Blue, SimonsStagesColor.Lime}.Contains(simonStagesColor) && methods.IndexOf(6) > 1)
        {
            var method = rnd.Range(0, 2);
            if (methods[method] == 7)
            {
                method = method == 0 ? 1 : 0;
            }
            var previous = methods.IndexOf(6);
            var first = methods[method];
            methods[method] = methods[previous];
            methods[previous] = first;
        }
        for (var i = 0; i < 4; i++)
        {
            _usedMethods.Add(possible[methods[i]]);
        }

        if (_usedMethods.Select(x => x.Id).Contains(MethodId.SimonsStages))
        {
            Audio.PlaySoundAtTransform(_usedMethods.Select(x => x.Id).Contains(MethodId.AND) ? SFX[rnd.Range(0, 2) == 0 ? 0 : 7].name : SFX[0].name, Module.transform);
        }
        else
        {
            Audio.PlaySoundAtTransform(SFX[7].name, Module.transform);            
        }

        _chosenColor = simonStagesColor;
        Module.OnActivate += Activate;
    }

    private void Activate()
    {
        _logger.LogMessage("Running version {0}", _version);
        for (var i = 0; i < NumberButtons.Length; i++)
        {
            NumberButtons[i].OnInteract += ButtonHandler(i);
        }

        Generate();
    }

    private void Generate()
    {
        var componentInfo = new ComponentInfo();
        _initialNumber = GenerateNumber();
        _bottomNumber = GenerateNumber();

        _logger.LogMessage("The initial number is: {0}", _initialNumber);
        _logger.LogMessage("The bottom number is: {0}", _bottomNumber);
        _logger.LogMessage("The chosen methods are: {0}", _usedMethods.Select(x => x.Name).Join(", "));

        for (var i = 0; i < 4; i++)
        {
            var obj = GetObject(_usedMethods[i], i);
            obj.SetActive(true);

            switch (_usedMethods[i].Id)
            {
                case MethodId.ForgetMeNot:
                    _logger.LogMessage("Forget Me Not:");
                    _logger.LogMessage("There isn't anything you need to know here...");
                    break;
                case MethodId.ForgetInfinity:
                    var ignored = new Pair<int,int>(rnd.Range(1, 13), rnd.Range(1, 13));
                    _logger.LogMessage("Forget Infinity:");
                    while (ignored.Item1 == ignored.Item2)
                    {
                        ignored.Item2 = rnd.Range(1, 13);
                    }
                    _logger.LogMessage("The two ignored numbers are {0} and {1}.", ignored.Item1, ignored.Item2);
                    componentInfo.IgnoredNumbers = ignored;
                    obj.GetComponentInChildren<TextMesh>().text = string.Format("{0} / {1}", ignored.Item1, ignored.Item2);
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
                    var chosenColor = _chosenColor;
                    obj.GetComponentInChildren<TextMesh>().text = chosenColor.ToString().First().ToString();
                    componentInfo.SimonsStagesColor = chosenColor;
                    _logger.LogMessage("Simon's Stages:");
                    _logger.LogMessage("The rule chosen is {0}.", chosenColor.ToString());
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
                    //testing purposes
                    //ruleNums = new List<int>{57, 57, 57, 57,57, 57,57,57,57, 57,57,57};
                    _logger.LogMessage("Forget Me Later:");
                    _logger.LogMessage("The chosen rules are: {0}.", ruleNums.Join(", "));
                    componentInfo.Rules = ruleNums;
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
                    var rotation = _forgetEverythingPositions[i].PickRandom();
                    obj.transform.localPosition = rotation.Item1;
                    var eulerAngles = obj.transform.localEulerAngles;
                    eulerAngles = new Vector3(
                        eulerAngles.x,
                        rotation.Item2,
                        eulerAngles.z
                    );
                    obj.transform.localEulerAngles = eulerAngles;

                    componentInfo.FEColors = colors;
                    for (int j = 0; j < 3; j++)
                    {
                        obj.GetComponent<MeshRenderer>().materials[j + 1].color = _forgetEverythingLedColors[(int)colors[j]];
                    }
                    break;
                case MethodId.ForgetUsNot:
                    var positions = Enumerable.Range(1, 12).ToList().Shuffle();
                    componentInfo.Positions = positions;
                    _logger.LogMessage("Forget Us Not:");
                    _logger.LogMessage("The positions are: {0}", positions.Join(", "));
                    StartCoroutine(ForgetUsNotCycle(positions, obj.GetComponentInChildren<TextMesh>()));
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Invalid id {0}", _usedMethods[i].Id));
            }
        }
        
        var numberInfo = new NumberInfo(_initialNumber, _bottomNumber, _logger);
        _initialNumbers = new Pair<string, string>(numberInfo.Number, numberInfo.BottomNumber);

        for (var i = 0; i < 4; i++)
        {
            numberInfo.Number = _usedMethods[i].Encrypt(Info, componentInfo, numberInfo);
            _logger.LogMessage("After step {0}({1}) of the encryption, the number is: {2}", i + 1, _usedMethods[i].Name, numberInfo.Number);
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
            if (_usedMethods.Select(x => x.Id).Contains(MethodId.ForgetMeNow))
            {
                Audio.PlaySoundAtTransform(SFX[6].name, Module.transform);   
            }
            _currentInputtedText = "------------";
            _hasBeenStarted = false;
            _presses = new List<int>();
            return;
        }
        if (_pressIndex == 0)
        {
            if (_usedMethods.Select(x => x.Id).Contains(MethodId.ForgetMeNow))
            {
                Audio.PlaySoundAtTransform(SFX[5].name, Module.transform);   
            }
        }
        AddToDashedText(i, _pressIndex);
        _pressIndex++;

        if (_presses.Join("") == _answer)
        {
            _logger.LogMessage("Module solved!");
            _isSolved = true;
            Module.HandlePass();
            if (_usedMethods.Select(x => x.Id).Contains(MethodId.SimonsStages))
            {
                Audio.PlaySoundAtTransform(_usedMethods.Select(x => x.Id).Contains(MethodId.AND) ? SFX[rnd.Range(3, 5)].name : SFX[4].name, Module.transform);
            }
            else
            {
                Audio.PlaySoundAtTransform(SFX[3].name, Module.transform);
            }

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
            case MethodId.ForgetUsNot:
                return ForgetUsNotObjects[position];
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

    private IEnumerator ForgetMeLaterCycle(List<int> rules, TextMesh textMesh)
    {
        while (true)
        {
            textMesh.text = string.Empty;
            yield return new WaitForSeconds(0.8f);
            for (var i = 0; i < 12; i++)
            {
                textMesh.text = rules[i].ToString();
                yield return new WaitForSeconds(0.8f);
            }
            if (_isSolved)
            {
                yield break;
            }
        }
    }
    
    private IEnumerator ForgetUsNotCycle(List<int> positions, TextMesh textMesh)
    {
        while (true)
        {
            textMesh.text = string.Empty;
            yield return new WaitForSeconds(0.8f);
            for (var i = 0; i < 12; i++)
            {
                textMesh.text = positions[i].ToString();
                yield return new WaitForSeconds(0.83f);
            }
            if (_isSolved)
            {
                yield break;
            }
        }
    }
    
    private IEnumerator ANDCycle(List<ANDLogicGate> gates, TextMesh textMesh)
    {
        while (true)
        {
            textMesh.text = string.Empty;
            yield return new WaitForSeconds(1f);
            for (var i = 0; i < 12; i++)
            {
                textMesh.text = _andLogicGateChars[(int)gates[i]].ToString();
                yield return new WaitForSeconds(1f);
            }

            if (_isSolved)
            {
                yield break;
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
        
        for (float x = 0; x <= 1; x = Mathf.Min(x + 1 * Time.deltaTime, 1))
        {
            var localPosition = ButtonsObject.transform.localPosition;
            var posx = localPosition.x;
            var posz = localPosition.z;
            ButtonsObject.transform.localPosition = new Vector3(posx, 0.01799584f * (1 - x) + 0.01140002f * x, posz);
            if (x >= 1)
            {
                break;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
        
        EncProcess.SetActive(true);
        ButtonsObject.SetActive(false);
        yield return new WaitForSeconds(1.5f);

        for (float x = 0; x <= 1; x = Mathf.Min(x + 1 * Time.deltaTime, 1))
        {
            var localPosition = EncProcess.transform.localPosition;
            var posx = localPosition.x;
            var posz = localPosition.z;
            EncProcess.transform.localPosition = new Vector3(posx, 0.0119f * (1-x) + 0.01799588f * x, posz);
            if (x >= 1)
            {
                break;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return new WaitForSeconds(.6f);
        StartCoroutine(WriteText());
    }

    private IEnumerator SubmitModeAnimation()
    {
        _animating = true;
        _showingInfo = false;
        Audio.PlaySoundAtTransform(SFX[1].name, Module.transform);
        StartCoroutine(DoSmallDisplayShuffle());

        for (float x = 0; x <= 1; x = Mathf.Min(x + 1 * Time.deltaTime, 1))
        {
            var localPosition = EncProcess.transform.localPosition;
            var posx = localPosition.x;
            var posz = localPosition.z;
            EncProcess.transform.localPosition = new Vector3(posx, 0.01799588f * (1 - x) + 0.0119f * x, posz);
            if (x >= 1)
            {
                break;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }

        EncProcess.SetActive(false);
        ButtonsObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        for (float x = 0; x <= 1; x = Mathf.Min(x + 1 * Time.deltaTime, 1))
        {
            var localPosition = ButtonsObject.transform.localPosition;
            var posx = localPosition.x;
            var posz = localPosition.z;
            ButtonsObject.transform.localPosition = new Vector3(posx, 0.01140002f * (1 - x) + 0.01799584f * x, posz);
            if (x >= 1)
            {
                break;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
        
        yield return new WaitForSeconds(0.6f);
        StartCoroutine(ClearOnSumbitModeText());
    }

    private IEnumerator DoSmallDisplayShuffle()
    {
        var cycle = Enumerable.Range(0,100).ToList().Shuffle().Select(x => x.ToString("D2")).ToList();
        var targetTime = 5f;
        var elapsed = 0f;
        var cycleNum = 0;
        while (elapsed<targetTime)
        {
            ModuleTexts[0].text = cycle[cycleNum % 100];
            cycleNum++;
            elapsed += Time.deltaTime;
            yield return null;
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

        var dashText = "--- --- --- ---";
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