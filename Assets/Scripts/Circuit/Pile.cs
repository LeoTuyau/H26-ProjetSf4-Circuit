public class Pile : Composante
{
    public float Tension { get; private set; }

    public Pile(string nom, float tension) : base(nom)
    {
        Tension = tension;
    }
}