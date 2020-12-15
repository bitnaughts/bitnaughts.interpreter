using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

public class Interpreter
{
    private List<Instruction> instructions; // Size restricted by static memory
    private Dictionary<string, Variable> variables; // Size restricted by random access memory



    public Interpreter(string[] instructions) {
        
        variables = new Dictionary<string, Variable>();
        variables.Add(Processor.Pointer, new Variable(0));
        variables.Add(Processor.Result, new Variable(0));

        this.instructions = new List<Instruction>();
        foreach (var inst in instructions) if (inst != "") this.instructions.Add(new Instruction(inst));
    }

    public void Iterate() {

        if (instructions == null) Init(debug_instructions);

        var inst = GetInstruction(Mathf.RoundToInt(variables[Processor.Pointer].value));
        pointer = variables[Processor.Pointer].value;
        switch (inst?.op_code)
        {
            case Processor.Set:
                if (variables.ContainsKey(inst.dest_reg))
                    variables[inst.dest_reg].Set(Parse(inst.src_reg));
                else
                    variables.Add(inst.dest_reg, new Variable(Parse(inst.src_reg)));
                break;
            case Processor.Add:
                variables[inst.dest_reg].Add(Parse(inst.src_reg));
                break;
            case Processor.Abs:
                variables[inst.dest_reg].Set(Mathf.Abs(Parse(inst.dest_reg)));
                break;
            case Processor.Subtract:
                variables[inst.dest_reg].Subtract(Parse(inst.src_reg));
                break;
            case Processor.Multiply:
                variables[inst.dest_reg].Multiply(Parse(inst.src_reg));
                break;
            case Processor.Divide:
                variables[inst.dest_reg].Divide(Parse(inst.src_reg));
                break;
            case Processor.Sine:
                variables[inst.dest_reg].Set(Mathf.Sin(Parse(inst.src_reg)));
                break;
            case Processor.Cosine:
                variables[inst.dest_reg].Set(Mathf.Cos(Parse(inst.src_reg)));
                break;
            case Processor.Tangent:
                variables[inst.dest_reg].Set(Mathf.Tan(Parse(inst.src_reg)));
                break;
            case Processor.Jump:
                variables[Processor.Pointer].Set(Parse(inst.dest_reg));
                break;
            case Processor.Jump_If_Greater:
                if (Parse(inst.dest_reg) > Parse(inst.src_reg))
                    variables[Processor.Pointer].Set(Parse(inst.src_reg_2));
                break;
            case Processor.Jump_If_Equal:
                if (Parse(inst.dest_reg) == Parse(inst.src_reg))
                    variables[Processor.Pointer].Set(Parse(inst.src_reg_2));
                break;
            case Processor.Jump_If_Not_Equal:
                if (Parse(inst.dest_reg) != Parse(inst.src_reg))
                    variables[Processor.Pointer].Set(Parse(inst.src_reg_2));
                break;
            case Processor.Jump_If_Less:
                if (Parse(inst.dest_reg) < Parse(inst.src_reg))
                    variables[Processor.Pointer].Set(Parse(inst.src_reg_2));
                break;
            case Processor.Component:
                if (components.ContainsKey(inst.dest_reg)) {
                    variables[Processor.Result].Set(components[inst.dest_reg].Action(Parse(inst.src_reg)));
                }
                break;
        }
        if (pointer == variables[Processor.Pointer].value) variables[Processor.Pointer].Increment(); // If line pointer is unchanged, step to next instruction
    }

    public float JumpTo(float line) {
        if (line < 0) line = 0;
        if (line > variables[Processor.Pointer]) line =
        variables[Processor.Pointer].Set(line);
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