using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

public class Interpreter
{
    public List<Instruction> instructions; // Size restricted by static memory
    public Dictionary<string, Variable> variables; // Size restricted by random access memory
    
    public const string Set = "set",
        /* Operations Format: $0 $1 $2                 */
        /* $0 == opcode                                */
        /* $1 == variable reference                    */
        /* $2 == variable reference, or float constant */

        /* Variable Declarations */
        
        /* Arithmetic */
        Add = "add",
        Abs = "abs", 
        Subtract = "sub", 
        Multiply = "mul",
        Divide = "div",
        Modulo = "mod",
        /* Trigonometry */
        Sine = "sin",
        Cosine = "cos",
        Tangent = "tan",
        /* Stack Manipulation */
        Jump = "jum",
        Jump_If_Greater = "jig",
        Jump_If_Equal = "jie",
        Jump_If_Not_Equal = "jin",
        Jump_If_Less = "jil",

        /* Interactivity */
        Component = "com",
        Get = "get",
        Print = "pri";

    public const char Space = ' ',
        New_Line = '\n';

    public const string Variables = "var",
        Pointer = "ptr",
        Result = "res",
        /* Input */
        Input_W = "inw",
        Input_A = "ina",
        Input_S = "ins",
        Input_D = "ind",
        Input_Horz = "inh",
        Input_Vert = "inv",
        Random = "rnd";

    public Interpreter(string[] instructions) {
        
        variables = new Dictionary<string, Variable>();
        variables.Add(Pointer, new Variable(0));
        variables.Add(Result, new Variable(0));

        this.instructions = new List<Instruction>();
        foreach (var inst in instructions) if (inst != "") this.instructions.Add(new Instruction(inst));
    }

    public void Iterate(Dictionary<string, ComponentController> components) {

        if (instructions == null) Init(debug_instructions);

        var inst = GetInstruction(Mathf.RoundToInt(variables[Pointer].value));
        pointer = variables[Pointer].value;
        switch (inst?.op_code)
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
            case Abs:
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
            case Sine:
                variables[inst.dest_reg].Set(Mathf.Sin(Parse(inst.src_reg)));
                break;
            case Cosine:
                variables[inst.dest_reg].Set(Mathf.Cos(Parse(inst.src_reg)));
                break;
            case Tangent:
                variables[inst.dest_reg].Set(Mathf.Tan(Parse(inst.src_reg)));
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
    }

    public Instruction GetInstruction(int index) 
    {
        if (index < 0 || index >= instructions.Count) return null;
        return instructions[index].Get();   
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
            case Input_W:
                return Input.GetKey(KeyCode.W) ? parse_value : 0;
            case Input_A:
                return Input.GetKey(KeyCode.A) ? parse_value : 0;
            case Input_S:
                return Input.GetKey(KeyCode.S) ? parse_value : 0;
            case Input_D:
                return Input.GetKey(KeyCode.D) ? parse_value : 0;
            case Input_Vert:
                return Input.GetAxis("Vertical");
            case Input_Horz:
                return Input.GetAxis("Horizontal");
            case Random:
                var random = new System.Random();
                // print ((float)(random.NextDouble() * 2000 - 1000));
                return (float)(random.NextDouble() * 2000 - 1000); 
            default:
                for (int i = 0; i < instructions.Count; i++) 
                    if (input == instructions[i].label) return i;
                if (variables.ContainsKey(input))
                    return parse_value * variables[input].value;
                return 0;                
        }
    }
}