using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Interpreter
{
    //12/24 test script:
    /*
START
set thrust 0
com thruster1 thrust
LOOP
add thrust 1
com thruster1 thrust
jum LOOP
    */
    public const char Space = ' ',
        New_Line = '\n';
    public const string Variables = "var",
        Pointer = "ptr",
        Result = "res",
        /* Input */
        Input_Q = "inq",
        Input_W = "inw",
        Input_E = "ine",
        Input_A = "ina",
        Input_S = "ins",
        Input_D = "ind",
        Input_Z = "inz",
        Input_X = "inx",
        Input_C = "inc",
        Input_Horz = "inh",
        Input_Vert = "inv",
        Random = "rnd";
    public List<Instruction> instructions; // Size restricted by static memory
    public Dictionary<string, Variable> variables; // Size restricted by random access memory
    public const string Set = "set", Set_Text = "Set", Set_Description = "\n <b>Set</b> a variable\n to a value;\n\n var = value;",
        /* Operations Format: $0 $1 $2                 */
        /* $0 == opcode                                */
        /* $1 == variable reference                    */
        /* $2 == variable reference, or float constant */
        /* Arithmetic https://en.wikipedia.org/wiki/Arithmetic#Arithmetic_operations */
        Add         = "add", Add_Text         = "Add",         Add_Description         = "\n <b>Add</b> a variable \n by a value;\n\n var += value;",
        Subtract    = "sub", Subtract_Text    = "Subtract",    Subtract_Description    = "\n <b>Subtract</b> a variable \n by a value;\n\n var -= val;", 
        Absolute    = "abs", Absolute_Text    = "Absolute",    Absolute_Description    = "\n <b>Absolute</b> a variable \n by a value;\n\n var = |val|;", 
        Multiply    = "mul", Multiply_Text    = "Multiply",    Multiply_Description    = "\n <b>Multiply</b> a variable\n by a value;\n\n var *= val;",
        Divide      = "div", Divide_Text      = "Divide",      Divide_Description      = "\n <b>Divide</b> a variable\n by a value;\n\n var /= val;",
        Modulo      = "mod", Modulo_Text      = "Modulo",      Modulo_Description      = "\n <b>Modulo</b> a variable\n by a value;\n\n var %= val;",
        Exponential = "exp", Exponential_Text = "Exponential", Exponential_Description = "\n <b>Exponent</b> a value\n to a variable;\n\n var ^= val;",
        Root        = "roo", Root_Text        = "Root",        Root_Description        = "\n <b>Root</b> a value\n to a variable;\n\n var √= val;",
        /* Trigonometry https://en.wikipedia.org/wiki/Trigonometry#Mnemonics */
        Sine       = "sin", Sine_Text       = "Sine",       Sine_Description       = "\n <b>Sine</b> a value\n to a variable;\n\n var = sin(val);",
        Cosine     = "cos", Cosine_Text     = "Cosine",     Cosine_Description     = "\n <b>Cosine</b> a value\n to a variable;\n\n var = cos(val);",
        Tangent    = "tan", Tangent_Text    = "Tangent",    Tangent_Description    = "\n <b>Tangent</b> a value\n to a variable;\n\n var = tan(val);",
        Secant     = "sec", Secant_Text     = "Secant",     Secant_Description     = "\n <b>Secant</b> a value\n to a variable;\n\n var = sec(val);",
        Cosecant   = "csc", Cosecant_Text   = "Cosecant",   Cosecant_Description   = "\n <b>Cosecant</b> a value\n to a variable;\n\n var = csc(val);",
        Cotangent  = "cot", Cotangent_Text  = "Cotangent",  Cotangent_Description  = "\n <b>Cotangent</b> a value\n to a variable;\n\n var = cot(val);",
        Arcsine    = "acs", Arcsine_Text    = "Arcsine",    Arcsine_Description    = "\n <b>Arcsine</b> a value\n to a variable;\n\n var = acs(val);",
        Arccosine  = "acc", Arccosine_Text  = "Arccosine",  Arccosine_Description  = "\n <b>Arccosine</b> a value\n to a variable;\n\n var = acc(val);",
        Arctangent = "act", Arctangent_Text = "Arctangent", Arctangent_Description = "\n <b>Arctangent</b> a value\n to a variable;\n\n var = act(val);",
        /* Boolean https://en.wikipedia.org/wiki/Boolean_algebra */
        And  = "and", And_Text = "And",   And_Description  = "\n <b>And</b> a value\n to a variable;",
        Or   = "orr", Or_Text = "Or",     Or_Description   = "\n <b>Or</b> a value\n to a variable;",
        Not  = "not", Not_Text = "Not",   Not_Description  = "\n <b>Not</b> a value\n to a variable;",
        Nand = "nnd", Nand_Text = "Nand", Nand_Description = "\n <b>Nand</b> a value\n to a variable;",
        Nor  = "nor", Nor_Text = "Nor",   Nor_Description  = "\n <b>Nor</b> a value\n to a variable;",
        Xor  = "xor", Xor_Text = "Xor",   Xor_Description  = "\n <b>Xor</b> a value\n to a variable;",
        Xnor = "xnr", Xnor_Text = "Xnor", Xnor_Description = "\n <b>Xnor</b> a value\n to a variable;",
        /* Stack Pointer https://en.wikipedia.org/wiki/Call_stack#STACK-POINTER */
        Jump_Label        = "LABEL", Jump_Label_Text = "Jump Label",               Jump_Label_Description = "\n Define a <b>Jump Label</b>\n to <i>Jump</i> to;",
        Jump              = "jum",   Jump_Text = "Jump",                           Jump_Description = "\n <b>Jump</b> to a defined\n <i>Jump Label</i>;",
        Jump_If_Equal     = "jie",   Jump_If_Equal_Text = "Jump If Equal",         Jump_If_Equal_Description = "\n <b>Jump</b> if a value\n equals a value;",
        Jump_If_Not_Equal = "jin",   Jump_If_Not_Equal_Text = "Jump If Not", Jump_If_Not_Equal_Description = "\n <b>Jump</b> if a value\n doesn't equal a value;",
        Jump_If_Greater   = "jig",   Jump_If_Greater_Text = "Jump If Greater",     Jump_If_Greater_Description = "\n <b>Jump</b> if a value\n is greater than a value;",
        Jump_If_Less      = "jil",   Jump_If_Less_Text = "Jump If Less",           Jump_If_Less_Description = "\n <b>Jump</b> if a value\n is less than a value;",
        /* Interactivity */
        Component = "obj", Component_Text = "Objects", Component_Description = "\n Call <b>Component(Input)</b>,\n Returns value to <i>res</i> variable;";
    public const string Arithmetic = "Arithmetic",
        Flow_Control = "Flow Control",
        Boolean = "Boolean",
        Trigonometry = "Trigonometry",
        Interactivity = "Interactivity";
    public static string[] Arithmetic_Operations = { 
        Set_Text, Add_Text, Subtract_Text, Absolute_Text, Multiply_Text, Divide_Text, Modulo_Text, Exponential_Text, Root_Text
    };
    public static string[] OperationTypes = new string[] {Arithmetic, Flow_Control, Boolean, Trigonometry, Interactivity};
    
    public static Tuple<string, string, string, string>[] OperationsArray = new Tuple<string, string, string, string>[]{
        /* Arithmetic https://en.wikipedia.org/wiki/Arithmetic#Arithmetic_operations */
        Tuple.Create(Arithmetic,  Set_Text,         Set,         Set_Description),
        Tuple.Create(Arithmetic,  Add_Text,         Add,         Add_Description),
        Tuple.Create(Arithmetic,  Subtract_Text,    Subtract,    Subtract_Description),
        Tuple.Create(Arithmetic,  Absolute_Text,    Absolute,    Absolute_Description),
        Tuple.Create(Arithmetic,  Multiply_Text,    Multiply,    Multiply_Description),
        Tuple.Create(Arithmetic,  Divide_Text,      Divide,      Divide_Description),
        Tuple.Create(Arithmetic,  Modulo_Text,      Modulo,      Modulo_Description),
        Tuple.Create(Arithmetic,  Exponential_Text, Exponential, Exponential_Description),
        Tuple.Create(Arithmetic,  Root_Text,        Root,        Root_Description),
        /* Flow Control https://en.wikipedia.org/wiki/Call_stack#STACK-POINTER */
        Tuple.Create(Flow_Control, Jump_Label_Text,        Jump_Label,        Jump_Label_Description),
        Tuple.Create(Flow_Control, Jump_Text,              Jump,              Jump_Description),
        Tuple.Create(Flow_Control, Jump_If_Equal_Text,     Jump_If_Equal,     Jump_If_Equal_Description),
        Tuple.Create(Flow_Control, Jump_If_Not_Equal_Text, Jump_If_Not_Equal, Jump_If_Not_Equal_Description),
        Tuple.Create(Flow_Control, Jump_If_Greater_Text,   Jump_If_Greater,   Jump_If_Greater_Description),
        Tuple.Create(Flow_Control, Jump_If_Less_Text,      Jump_If_Less,      Jump_If_Less_Description),
        /* Boolean https://en.wikipedia.org/wiki/Boolean_algebra */
        Tuple.Create(Boolean, And_Text,  And,  And_Description),
        Tuple.Create(Boolean, Or_Text,   Or,   Or_Description),
        Tuple.Create(Boolean, Not_Text,  Not,  Not_Description),
        Tuple.Create(Boolean, Nand_Text, Nand, Nand_Description),
        Tuple.Create(Boolean, Nor_Text,  Nor,  Nor_Description),
        Tuple.Create(Boolean, Xor_Text,  Xor,  Xor_Description),
        Tuple.Create(Boolean, Xnor_Text, Xnor, Xnor_Description),
        /* Trigonometry https://en.wikipedia.org/wiki/Trigonometry#Mnemonics */
        Tuple.Create(Trigonometry, Sine_Text,       Sine,       Sine_Description),
        Tuple.Create(Trigonometry, Cosine_Text,     Cosine,     Cosine_Description),
        Tuple.Create(Trigonometry, Tangent_Text,    Tangent,    Tangent_Description),
        Tuple.Create(Trigonometry, Arcsine_Text,    Arcsine,    Arcsine_Description),
        Tuple.Create(Trigonometry, Arccosine_Text,  Arccosine,  Arccosine_Description),
        Tuple.Create(Trigonometry, Arctangent_Text, Arctangent, Arctangent_Description),
        /* Interactivity */
        Tuple.Create(Interactivity, Component_Text, Component, Component_Description)
    };

    public static bool IsCategory(string category)
    {
        foreach (var category_type in OperationTypes)
        {
            if (category == category_type) return true;
        }
        return false;
    } 

    public static string[] GetOperations(string category)
    {
        List<string> ops = new List<string>();
        foreach (var operation in OperationsArray)
        {
            if (operation.Item1 == category) ops.Add(operation.Item2);
        }
        return ops.ToArray();
    }
    public static string GetTextCode(string text)
    {
        if (text.Contains(" ")) text = text.Substring(2);
        foreach (var operation in OperationsArray)
        {
            if (operation.Item2 == text) return operation.Item3;
        }
        return Jump_Label;
    }
    public static string GetTextDescription(string text)
    {
        switch (text)
        {
            case "Syntax": return "\n <b>Syntax</b> is a branch\n of semiotics dealing \n with relations between\n signs and expressions\n in abstraction from\n their signification and\n their interpreters;\n\n Use menu options to\n manipulate processor\n assembly code;";
            case "Scroll Up": return "\n <b>Scrolling</b> moves\n the edit pointer;";
            case "Scroll Down": return "\n <b>Scrolling</b> moves\n the edit pointer;";
            case "Add Above": return "\n <b>Adding</b> inserts\n a new instruction to\n the instruction set;";
            case "Add Below": return "\n <b>Adding</b> inserts\n a new instruction to\n the instruction set;";
            case "Delete": return "\n <b>Deleting</b> removes\n an instruction from\n the instruction set;";
            case "Constant": return "\n <b>Constant</b> values;";
            case "Variable": return "\n <b>Variable</b> values;";
            case "User Input": return "\n <b>User Input</b> enables\n manual control of ships;\n\n *Note: User-controlled ships\n cannot be submitted\n to global leaderboard;";
            case Arithmetic: return "\n <b>Arithmetic</b> ops\n compute analog\n values;";
            case Flow_Control: return "\n <b>Flow Control</b> ops\n set the stack\n pointer to \n control code\n execution;";
            case Boolean: return "\n <b>Boolean</b> ops\n compute boolean \n values;";
            case Trigonometry: return "\n <b>Trigonometry</b> ops\n compute trig \n values;";
            case Interactivity: return "\n <b>Interactivity</b> ops\n control connected\n components;";
        }
        foreach (var operation in OperationsArray)
        {
            if (operation.Item2 == text) return operation.Item4;
        }
        return Jump_Label_Description;
    }
    public static bool IsInstruction(string code)
    {
        foreach (var operation in OperationsArray)
        {
            if (operation.Item3 == code) return true;
        }
        return false;
    }

    public static string GetTextCategory(string text)
    {
        foreach (var operation in OperationsArray)
        {
            if (operation.Item2 == text) return operation.Item1;
        }
        return Flow_Control;
    }

    public static string GetInstructionCategory(Instruction instruction)
    {
        foreach (var operation in OperationsArray)
        {
            if (operation.Item3 == instruction.op_code) return operation.Item1;
        }
        return Flow_Control;
    }

    public static string GetInstructionText(Instruction instruction)
    {
        foreach (var operation in OperationsArray)
        {
            if (operation.Item3 == instruction.op_code) return operation.Item2;
        }
        return Jump_Label_Text;
    }

    public Interpreter(string[] instructions) {
        
        variables = new Dictionary<string, Variable>();
        variables.Add(Pointer, new Variable(0));
        variables.Add(Result, new Variable(0));

        this.instructions = new List<Instruction>();
        foreach (var inst in instructions) if (inst != "") this.instructions.Add(new Instruction(inst));
    }

    public int GetPointer() {
        return Mathf.RoundToInt(variables[Pointer].value);
    }

    public string Iterate(Dictionary<string, ComponentController> components, int edit_line) {

        // if (instructions == null) Init(debug_instructions);

        var inst = GetInstruction(Mathf.RoundToInt(variables[Pointer].value));
        var pointer = variables[Pointer].value;
        if (inst != null && inst.dest_reg != null) switch (inst?.op_code)
        {
            case Set:
                if (variables.ContainsKey(inst.dest_reg))
                    variables[inst.dest_reg].Set(Parse(inst.src_reg));
                else
                    variables.Add(inst.dest_reg, new Variable(Parse(inst.src_reg)));
                break;
            case Add:
                variables[inst.dest_reg].Add(Parse(inst.src_reg));
                break;
            case Absolute:
                variables[inst.dest_reg].Set(Mathf.Abs(Parse(inst.dest_reg)));
                break;
            case Subtract:
                variables[inst.dest_reg].Subtract(Parse(inst.src_reg));
                break;
            case Multiply:
                variables[inst.dest_reg].Multiply(Parse(inst.src_reg));
                break;
            case Divide:
                variables[inst.dest_reg].Divide(Parse(inst.src_reg));
                break;
            case Not:
                variables[inst.dest_reg].Set(Convert.ToInt32(!(Parse(inst.src_reg) >= 1)));
                break;
            case And:
                variables[inst.dest_reg].Set(Convert.ToInt32(variables[inst.dest_reg].value >= 1 && Parse(inst.src_reg) >= 1));
                break;
            case Nand:
                variables[inst.dest_reg].Set(Convert.ToInt32(!(variables[inst.dest_reg].value >= 1 && Parse(inst.src_reg) >= 1)));
                break;
            case Or:
                variables[inst.dest_reg].Set(Convert.ToInt32(variables[inst.dest_reg].value >= 1 || Parse(inst.src_reg) >= 1));
                break;
            case Nor:
                variables[inst.dest_reg].Set(Convert.ToInt32(!(variables[inst.dest_reg].value >= 1 || Parse(inst.src_reg) >= 1)));
                break;
            case Xor:
                variables[inst.dest_reg].Set(Convert.ToInt32(variables[inst.dest_reg].value >= 1 ^ Parse(inst.src_reg) >= 1));
                break;
            case Xnor:
                variables[inst.dest_reg].Set(Convert.ToInt32(!(variables[inst.dest_reg].value >= 1 ^ Parse(inst.src_reg) >= 1)));
                break;
            case Sine:
                variables[inst.dest_reg].Set(Mathf.Sin(Parse(inst.src_reg)));
                break;
            case Cosine:
                variables[inst.dest_reg].Set(Mathf.Cos(Parse(inst.src_reg)));
                break;
            case Tangent:
                variables[inst.dest_reg].Set(Mathf.Tan(Parse(inst.src_reg)));
                break;
            case Arcsine:
                variables[inst.dest_reg].Set(Mathf.Asin(Parse(inst.src_reg)));
                break;
            case Arccosine:
                variables[inst.dest_reg].Set(Mathf.Acos(Parse(inst.src_reg)));
                break;
            case Arctangent:
                variables[inst.dest_reg].Set(Mathf.Atan(Parse(inst.src_reg)));
                break;
            case Jump:
                variables[Pointer].Set(Parse(inst.dest_reg));
                break;
            case Jump_If_Greater:
                if (Parse(inst.dest_reg) > Parse(inst.src_reg))
                    variables[Pointer].Set(Parse(inst.src_reg_2));
                break;
            case Jump_If_Equal:
                if (Parse(inst.dest_reg) == Parse(inst.src_reg))
                    variables[Pointer].Set(Parse(inst.src_reg_2));
                break;
            case Jump_If_Not_Equal:
                if (Parse(inst.dest_reg) != Parse(inst.src_reg))
                    variables[Pointer].Set(Parse(inst.src_reg_2));
                break;
            case Jump_If_Less:
                if (Parse(inst.dest_reg) < Parse(inst.src_reg))
                    variables[Pointer].Set(Parse(inst.src_reg_2));
                break;
            case Component:
                if (components.ContainsKey(inst.dest_reg)) {
                    variables[Result].Set(components[inst.dest_reg].Action(Parse(inst.src_reg)));
                }
                break;
        }
        if (pointer == variables[Pointer].value) variables[Pointer].Increment(); // If line pointer is unchanged, step to next instruction
        
        string output = " Code:\n";
        int i = 0;
        foreach (var instruction in instructions) 
        {
            string raw_line = instruction.ToString();
            if (i == Mathf.RoundToInt(pointer)) raw_line = "<i>" + raw_line + "</i>";
            if (i++ == edit_line) raw_line = "<b>" + raw_line + "</b>";
            
            output += " " + raw_line + "\n";
        }
        output += "\n Memory:\n";
        foreach (var variable in variables)
        {
            output += "- " + variable.Key + ": " + variable.Value.ToString() + "\n";
        }
        return output;
    }

    public Instruction GetInstruction(int index) 
    {
        if (index < 0 || index >= instructions.Count) return null;
        return instructions[index].Get();   
    }

    public string GetNextLabel() 
    {
        int count = 1;
        bool duplicate = true;
        string label = "";
        while (duplicate) 
        {
            label = "LABEL" + count++;
            duplicate = false;
            foreach (var instruction in instructions)
            {
                if (instruction.op_code == label) 
                {
                    duplicate = true;
                    break;
                }
            }
        }
        return label;
    }
    
    public string[] GetLabels()
    {
        List<string> labels = new List<string> ();
        foreach (var instruction in instructions) if (!IsInstruction(instruction.op_code)) labels.Add(instruction.op_code);
        return labels.ToArray();
    }
    public string GetVariablesString() {
        var output = "";
        int count = 0;
        foreach (var variable in variables)
        {
            count++;
            if (count < variables.Count) output += "\n  ┣ " + variable.Key + ": " + variable.Value.ToString();
            else output += "\n  ┗ " + variable.Key + ": " + variable.Value.ToString();
        }
        return output;
    }
    public string[] GetVariables()
    {
        List<string> vars = new List<string> {Pointer, Result };
        foreach (var instruction in instructions) if (instruction.op_code == Set) vars.Add(instruction.dest_reg);
        return vars.ToArray();
    }
    public bool isVariable(string variable_in)
    {
        if (variable_in.Contains("\t")) variable_in = variable_in.Substring(2);
        foreach (var variable in GetVariables()) {
            if (variable == variable_in) return true;
        }
        return false;
    }
    public bool isLabel(string label)
    {
        foreach (var l in GetLabels())
        {
            if (label == l) return true;
        }
        return false;
    }
    public string GetNextVariable() 
    {
        int count = 1;
        bool duplicate = true;
        string var = "";
        while (duplicate) 
        {
            var = "var" + count++;
            duplicate = false;
            foreach (var instruction in instructions)
            {
                if (instruction.dest_reg == var) 
                {
                    duplicate = true;
                    break;
                }
            }
        }
        return var;
    }

    float parse_value = 0f;
    private float Parse(string input)
    {
        if (input == null) return 0;
        if (float.TryParse(input, out parse_value))
        {
            return parse_value;
        }
        parse_value = 1f;
        if (input[0] == '-')
        {
            parse_value = -1f;
            input = input.Substring(1);
        }
        switch (input)
        {
            case Input_Q:
                return Input.GetKey(KeyCode.Q) ? parse_value : 0;
            case Input_W:
                return Input.GetKey(KeyCode.W) ? parse_value : 0;
            case Input_E:
                return Input.GetKey(KeyCode.E) ? parse_value : 0;
            case Input_A:
                return Input.GetKey(KeyCode.A) ? parse_value : 0;
            case Input_S:
                return Input.GetKey(KeyCode.S) ? parse_value : 0;
            case Input_D:
                return Input.GetKey(KeyCode.D) ? parse_value : 0;
            case Input_Z:
                return Input.GetKey(KeyCode.Z) ? parse_value : 0;
            case Input_X:
                return Input.GetKey(KeyCode.X) ? parse_value : 0;
            case Input_C:
                return Input.GetKey(KeyCode.C) ? parse_value : 0;
            case Input_Vert:
                return Input.GetAxis("Vertical");
            case Input_Horz:
                return Input.GetAxis("Horizontal");
            case Random:
                var random = new System.Random();
                // print ((float)(random.NextDouble() * 2000 - 1000));
                return (float)(random.NextDouble() * 200 - 100); 
            default:
                for (int i = 0; i < instructions.Count; i++) 
                    if (input == instructions[i].op_code) return i;
                if (variables.ContainsKey(input))
                    return parse_value * variables[input].value;
                return 0;                
        }
    }
}