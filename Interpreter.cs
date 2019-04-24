using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interpreter {

    private GameObject obj;
    private string[] script;

    private int pointer;

    private int[] 


    public Interpreter (string[] script, GameObject obj) {
        if (script == null) script = new string[] { };
        this.script = script;
        this.obj = obj;
    }

    /* Parsing each line of text into code (a.k.a. where the magic happens) */
    public bool interpretLine () {

        string line = script[pointer];
        string[] line_parts = line.Split (' ');

        switch (line_parts[0]) {
            case Operators.EMPTY:
                break;
          

          ///.......





            default:
                break;
        }

        if (pointer > script.length) return true;
        else {
            pointer++;
            return false;
        }
    }

    public override string ToString () {
        string output = debugger + "\n\n";
        // output += compiler.ToString ();
        debugger = Operators.EMPTY;
        output += scope.ToString ();
        return output;
    }
}