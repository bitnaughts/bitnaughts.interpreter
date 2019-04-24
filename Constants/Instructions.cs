public static class Instructions {
    public const string EMPTY = "",

        /* Boolean Logic */
        AND = "and",
        NOR = "nor",
        OR = "or",

        /* Register Manipulation */
        MOVE_FROM_HIGH = "mfhi",
        MOVE_FROM_LOW = "mflo",
        MOVE_FROM_CONTROL = "mfco",

        /* Stack Manipulation */
        STORE_BYTE = "sb",
        STORE_CONDITIONAL = "sc",
        STORE_HALFWORD = "sh",
        STORE_WORD = "sw",
        STORE_FP = "swcl",
        LOAD_LINKED = "ll",
        LOAD_WORD = "lw",
        LOAD_FP = "lwcl",

        /* Pointer Manipulation */
        JUMP = "j",
        JUMP_AND_LINK = "jal",
        JUMP_REGISTER = "jr",
        BRANCH_ON_EQUAL = "beq",
        BRANCH_ON_NOT_EQUAL = "bne",
        BRANCH_ON_FP_TRUE = "bclt",
        BRANCH_ON_FP_FALSE = "bclf",

        /* Integer Math */
        ADD = "add",
        SUBSTRACT = "sub",
        DIVIDE = "div",
        MULTIPLY = "mult",
        SET_LESS_THAN = "slt",
        SHIFT_LEFT_LOGICAL = "sll",
        SHIFT_RIGHT_LOGICAL = "srl",

        /* Floating Point Math */
        FP_ADD = "add.s",
        FP_COMPARE = "...",
        FP_DIVIDE = "div.s",
        FP_MULTIPLY = "mul.s",
        FP_SUBSTRACT = "sub.s",

}