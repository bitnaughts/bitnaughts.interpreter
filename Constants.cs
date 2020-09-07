/* "Analog" Assembly Language */
public const string Operations = "ops",
    /* Operations Format: $0 $1 $2                 */
    /* $0 == opcode                                */
    /* $1 == variable reference                    */
    /* $2 == variable reference, or float constant */

    /* Variable Declarations */
    Set = "set",
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


public const char Space = ' ',
    New_Line = '\n';