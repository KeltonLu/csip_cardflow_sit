using System;

public abstract class Light 
{ 
    public abstract void TurnOn(); 
    public abstract void TurnOff(); 
}

public class BulbLight : Light
{
    public override void TurnOn()
    {
        Console.WriteLine("Bulb Light is Turned on");
    }

    public override void TurnOff()
    {
        Console.WriteLine("Bulb Light is Turned off");
    }
}

public class TubeLight : Light
{
    public override void TurnOn()
    {
        Console.WriteLine("Tube Light is Turned on");
    }
    public override void TurnOff()
    {
        Console.WriteLine("Tube Light is Turned off");
    }
}

public class LightSimpleFactory
{
    public Light Create(string LightType)
    {
        if (LightType == "Bulb") return new BulbLight();
        else if (LightType == "Tube") return new TubeLight();
        else return null;
    }
}

public class Client
{
    public static void Main()
    {
        LightSimpleFactory lsf = new LightSimpleFactory();
        Light l = lsf.Create("Bulb");
        l.TurnOn();
        l.TurnOff();
        Console.WriteLine("-----------------");
        l = lsf.Create("Tube");
        l.TurnOn();
        l.TurnOff();
    }
}
