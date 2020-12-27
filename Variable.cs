/*
    In BitNaughts, all variables are "analog" values (floats) from -999 to 999.
*/
public class Variable 
{
    public float value, min, max;

    public Variable(float value) {
        Set(value);
    }
    public void Set(float value) {
        this.value = value;
        if (value > max) max = value;
        if (value < min) min = value;
    }
    public void Increment() {
        Set(value + 1);
    }
    public void Add(float value) {
        Set(this.value + value);
    }
    public void Subtract(float value) {
        Set(this.value - value);
    }
    public void Multiply(float value) {
        Set(this.value * value);
    }
   public void Divide(float value) {
        Set(this.value / value);
    }
    public override string ToString() 
    {
        return value.ToString(); // To be expanded on.
    }
}