using System;

public class Proceso
{
    public int Id, Llegada, Rafaga, Espera, Retorno;
}

public class Program
{
    public static void Fcfs(Proceso[] p, int n)
    {
        int tiempoActual = 0;
        for (int i = 0; i < n; i++)
        {
            if (tiempoActual < p[i].Llegada)
                tiempoActual = p[i].Llegada;

            p[i].Espera = tiempoActual - p[i].Llegada;
            tiempoActual += p[i].Rafaga;
            p[i].Retorno = p[i].Espera + p[i].Rafaga;
        }
    }

    public static void Main()
    {
        Proceso[] procesos = new Proceso[]
        {
            new Proceso { Id = 1, Llegada = 0, Rafaga = 4 },
            new Proceso { Id = 2, Llegada = 1, Rafaga = 3 },
            new Proceso { Id = 3, Llegada = 2, Rafaga = 2 }
        };

        Fcfs(procesos, procesos.Length);

        foreach (var p in procesos)
        {
            Console.WriteLine($"P{p.Id}: Llegada= {p.Llegada}, Rafaga= {p.Rafaga}, Espera= {p.Espera}, Retorno={p.Retorno}");
        }
    }
}