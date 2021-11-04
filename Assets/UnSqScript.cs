using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnSqScript : MonoBehaviour {

    public KMAudio Audio;
    public KMBombModule module;
    public Transform modtr;
    public Renderer modbg;
    public List<KMSelectable> buttons;
    public KMColorblindMode cbmode;
    public TextMesh[] cbtext;
    private readonly int[,] order = new int[5, 24] {
        { 9, 16, 21, 13, 18, 10, 3, 1, 5, 22, 17, 15, 0, 6, 4, 11, 20, 7, 23, 24, 2, 8, 14, 19},
        { 18, 5, 17, 19, 7, 4, 3, 0, 16, 10, 14, 6, 8, 21, 24, 15, 2, 20, 22, 9, 11, 1, 23, 13},
        { 23, 1, 15, 20, 10, 7, 22, 11, 2, 6, 4, 3, 19, 18, 16, 14, 17, 24, 9, 13, 21, 5, 8, 0},
        { 0, 4, 20, 11, 15, 6, 18, 23, 9, 17, 22, 16, 5, 13, 2, 19, 14, 21, 3, 8, 10, 7, 24, 1},
        { 7, 21, 2, 24, 9, 20, 17, 6, 13, 19, 0, 23, 1, 10, 16, 14, 3, 8, 11, 15, 18, 4, 5, 22} }; 
    private readonly float[][] cols = new float[5][] { new float[3] { 1, 0, 0 },  new float[3] { 1, 1, 0 }, new float[3] { 0, 1, 0.5f }, new float[3] { 0, 0.5f, 1 }, new float[3] { 0.5f, 0, 1 } };

    private readonly int[,] grid = new int[16, 16]
        {
            { 1, 0, 0, 3, 4, 4, 3, 2, 2, 3, 0, 2, 1, 1, 4, 0},
            { 2, 3, 2, 4, 2, 0, 0, 4, 1, 2, 4, 0, 3, 1, 3, 1},
            { 2, 1, 0, 3, 0, 4, 3, 3, 2, 0, 2, 2, 1, 4, 1, 4},
            { 0, 3, 0, 2, 3, 1, 4, 2, 0, 1, 1, 4, 3, 0, 2, 4},
            { 3, 1, 4, 0, 2, 3, 0, 4, 4, 1, 0, 3, 3, 2, 1, 2},
            { 4, 2, 4, 1, 1, 3, 4, 3, 1, 3, 4, 0, 2, 2, 0, 0},
            { 1, 3, 3, 4, 0, 1, 2, 4, 3, 2, 1, 4, 0, 1, 4, 2},
            { 1, 2, 1, 2, 4, 0, 3, 4, 0, 4, 3, 2, 3, 1, 0, 1},
            { 2, 2, 0, 4, 0, 2, 3, 3, 4, 0, 1, 3, 1, 4, 1, 2},
            { 3, 3, 0, 4, 2, 1, 0, 4, 2, 4, 1, 1, 2, 0, 1, 3},
            { 0, 2, 2, 3, 4, 4, 1, 3, 4, 1, 3, 0, 1, 0, 2, 0},
            { 4, 3, 1, 3, 2, 0, 2, 1, 1, 4, 3, 4, 0, 2, 0, 4},
            { 1, 0, 4, 4, 3, 2, 0, 3, 4, 1, 2, 1, 3, 0, 1, 2},
            { 3, 0, 3, 4, 1, 3, 3, 0, 1, 2, 1, 4, 2, 4, 0, 3},
            { 4, 4, 2, 0, 2, 4, 1, 1, 3, 0, 4, 3, 2, 0, 3, 1},
            { 2, 3, 2, 1, 3, 0, 2, 4, 0, 1, 0, 4, 4, 3, 2, 1}
        };
    private int ind;
    private int[] coord = new int[2];
    private int[,] subgrid = new int[5, 5];
    private float[,,] rgb = new float[3, 7, 7];
    private int[] ans;
    private int submission;
    private bool cb;

    private static int moduleIDCounter;
    private int moduleID;
    private bool moduleSolved;

    private void Awake()
    {
        moduleID = ++moduleIDCounter;
        cb = cbmode.ColorblindModeActive;
        ans = new int[24] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 }.Shuffle().Take(3).ToArray();
        for (int i = 0; i < 2; i++)
            coord[i] = Random.Range(0, 12);
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
                subgrid[i, j] = grid[i + coord[0], j + coord[1]];
        ind = subgrid[2, 2];
        for (int i = 0; i < 3; i++)
        {
            subgrid[ans[i] / 5, ans[i] % 5] += Random.Range(1, 5);
            subgrid[ans[i] / 5, ans[i] % 5] %= 5;
        }
        Debug.LogFormat("[Unpleasant Squares #{0}] The grid shown is: \n[Unpleasant Squares #{0}] {1}", moduleID, string.Join("\n[Unpleasant Squares #" + moduleID + "] ", Enumerable.Range(0, 5).Select(x => string.Join(" ", Enumerable.Range(0, 5).Select(y => "RYJZV"[subgrid[x, y]].ToString()).ToArray())).ToArray()));
        Debug.LogFormat("[Unpleasant Squares #{0}] The central {1} cell is located at {2}{3} of the large grid.", moduleID, new string[] { "red", "yellow", "jade", "azure", "violet"}[subgrid[2, 2]], "CDEFGHIJKLMN"[coord[1]], coord[0] + 3);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
                for (int k = 0; k < 5; k++)
                    rgb[i, j + 1, k + 1] = cols[subgrid[j, k]][i];
            rgb[i, 3, 3] = 1;
        }
        ans = ans.OrderBy(x => order[ind, x > 12 ? x - 1 : x]).ToArray();
        Debug.LogFormat("[Unpleasant Squares #{0}] Select the cells {1} in order.", moduleID, string.Join(", ", ans.Select(x => "ABCDE"[x % 5] + "" + ((x / 5) + 1).ToString()).ToArray()));
        foreach(KMSelectable button in buttons)
        {
            int b = buttons.IndexOf(button);
            b += b / 12;
            button.OnInteract = delegate () {
                if (!moduleSolved)
                {
                    button.AddInteractionPunch(0.25f);
                    Debug.LogFormat("[Unpleasant Squares #{0}] Selected {1}", moduleID, "ABCDE"[b % 5] + ((b / 5) + 1).ToString());
                    if (b == ans[submission])
                    {
                        Audio.PlaySoundAtTransform("Correct" + submission, buttons[b > 12 ? b - 1 : b].transform);
                        if (submission == 2)
                        {
                            moduleSolved = true;
                            module.HandlePass();
                            StopCoroutine("Locate");
                            StartCoroutine(Solve());
                        }
                        else
                            submission++;
                    }
                    else
                        module.HandleStrike();
                }
                return false; };
        }
        if (cb)
            for (int i = 0; i < 3; i++)
                cbtext[i].text = "RGB"[i].ToString();
        StartCoroutine("Locate");
    }

    private IEnumerator Locate()
    {
        while (!moduleSolved)
        {
            Vector3 yaxis = modtr.forward;
            float[] ang = new float[2] { yaxis.y, -yaxis.x };
            for (int i = 0; i < 2; i++)
                ang[i] = Mathf.Clamp((ang[i] + 1) * 3, 0.01f, 5.99f);
            float[][,] quad = new float[3][,];
            for (int i = 0; i < 3; i++)
                quad[i] = new float[2, 2] { { rgb[i, (int)ang[0], (int)ang[1]], rgb[i, (int)ang[0] + 1, (int)ang[1]] }, { rgb[i, (int)ang[0], (int)ang[1] + 1], rgb[i, (int)ang[0] + 1, (int)ang[1] + 1] } };
            float[] t = new float[2] { ang[0] % 1f, ang[1] % 1f };
            float[] c = new float[3];
            for (int i = 0; i < 3; i++)
            {
                c[i] = Mathf.Lerp(Mathf.Lerp(quad[i][0, 0], quad[i][0, 1], t[0]), Mathf.Lerp(quad[i][1, 0], quad[i][1, 1], t[0]), t[1]);
                if (cb)
                    cbtext[i].color = new Color(0, 0, 0, c[i]);
            }
            modbg.material.color = new Color(c[0], c[1], c[2]);
            yield return null;
        }
    }

    private IEnumerator Solve()
    {
        Color o = modbg.material.color;
        for(int i = 0; i < 60; i++)
        {
            float[] c = new float[3] { Mathf.Lerp(o.r, 0, i / 59f), Mathf.Lerp(o.g, 1, i / 59f), Mathf.Lerp(o.b, 0, i / 59f) };
            modbg.material.color = new Color (c[0], c[1], c[2]);
            if (cb)
                for (int j = 0; j < 3; j++)
                    cbtext[j].color = new Color(0, 0, 0, c[j]);
            yield return new WaitForSeconds(1 / 30f);
        }
        if(cb)
           for (int i = 0; i < 3; i++)
               cbtext[i].text = i == 1 ? "GG" : "";
    }

#pragma warning disable 414
    private string TwitchHelpMessage = "!{0} <A-E><1-5> [Selects cell]";
#pragma warning restore 414

    private IEnumerator ProcessTwitchCommand(string command)
    {
        if(command.Length != 2)
        {
            yield return "sendtochaterror!f Invalid command length.";
            yield break;
        }
        if(!"ABCDE".Contains(command[0]) || !"12345".Contains(command[1]))
        {
            yield return "sendtochaterror!f \"" + command + "\" is an invalid coordinate.";
            yield break;
        }
        int b = ("12345".IndexOf(command[1]) * 5) + "ABCDE".IndexOf(command[0]);
        if(b == 12)
        {
            yield return "sendtochaterror!f C3 is not selectable.";
            yield break;
        }
        if (b > 12)
            b--;
        yield return null;
        buttons[b].OnInteract();
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        while (!moduleSolved)
        {
            int a = ans[submission];
            if (a > 12)
                a--;
            yield return null;
            buttons[a].OnInteract();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
