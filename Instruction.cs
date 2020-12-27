

public class Instruction
{
    int red_color = 15;
    public string op_code, dest_reg, src_reg, src_reg_2;
    // public string[] operands;
    public string operand_divider = "  ";
    // string[] ops;
    public Instruction(string line)
    {
        var parts = line.Split(new char[] { ' ' });
        op_code = parts.Length > 0 ? parts[0] : null;
        dest_reg = parts.Length > 1 ? parts[1] : null;
        src_reg = parts.Length > 2 ? parts[2] : null;
        src_reg_2 = parts.Length > 3 ? parts[3] : null;
    }
    public Instruction Get()
    {
        return this;
    }
    public override string ToString()
    {
        return op_code + " " + dest_reg + " " + src_reg + " " + src_reg_2;
    }
}