

public class Instruction
{
    int red_color = 15;
    public string label = ""; //If "", default to showing line number
    public string label_divider = ":";
    public string op_code;
    public string dest_reg, src_reg, src_reg_2;
    // public string[] operands;
    public string operand_divider = "  ";
    // string[] ops;
    public Instruction(string line)
    {
        var parts = line.Split(new char[] { ' ' });
        label = parts.Length > 0 ? parts[0] : null;
        op_code = parts.Length > 1 ? parts[1] : null;
        dest_reg = parts.Length > 2 ? parts[2] : null;
        src_reg = parts.Length > 3 ? parts[3] : null;
        src_reg_2 = parts.Length > 4 ? parts[4] : null;
    }
    public Instruction Get()
    {
        red_color = 0;
        return this;
    }
    public override string ToString()
    {
        string base16 = "0123456789ABCDEF";
        string output = "<color=#F" + base16[red_color] + base16[red_color] + ">" + label + " " + op_code + " " + dest_reg + " " + src_reg + " " + src_reg_2 + "</color>";
        red_color += 3;
        if (red_color > 15) red_color = 15;
        return output;
    }
}