using System;
using System.Collections.Generic;
using System.Linq;

// Simulador del algoritmo de planificación LCFS (Last Come, First Served)
// No apropiativo: una vez que un proceso empieza a ejecutarse, corre
// hasta terminar. Entre los procesos que ya llegaron y esperan en cola,
// se elige siempre el que llegó más tarde (comportamiento de pila / LIFO).
//
// En esta versión, los procesos se ingresan por teclado al ejecutar el programa.

class Proceso
{
    public string Nombre;
    public int Llegada;
    public int Duracion;
    public int Inicio;
    public int Fin;

    public Proceso(string nombre, int llegada, int duracion)
    {
        Nombre = nombre;
        Llegada = llegada;
        Duracion = duracion;
    }

    public int Retorno => Fin - Llegada;
    public int Espera => Retorno - Duracion;
}

class Program
{
    static List<(string nombre, int inicio, int fin)> Lcfs(List<Proceso> procesos)
    {
        // Orden original de llegada (para desempatar cuando dos procesos
        // llegan en el mismo instante: se respeta el orden en que fueron
        // ingresados).
        var pendientes = procesos.OrderBy(p => p.Llegada).ToList();
        var pila = new Stack<Proceso>(); // procesos que ya llegaron y esperan
        int tiempo = 0;
        var gantt = new List<(string, int, int)>();

        while (pendientes.Count > 0 || pila.Count > 0)
        {
            // Meter a la pila todos los procesos que ya llegaron
            while (pendientes.Count > 0 && pendientes[0].Llegada <= tiempo)
            {
                pila.Push(pendientes[0]);
                pendientes.RemoveAt(0);
            }

            if (pila.Count == 0)
            {
                // No hay nadie listo: avanzar el tiempo hasta la próxima llegada
                tiempo = pendientes[0].Llegada;
                continue;
            }

            // LCFS: se toma el último que entró a la pila (el más reciente)
            var actual = pila.Pop();

            if (tiempo < actual.Llegada)
                tiempo = actual.Llegada;

            actual.Inicio = tiempo;
            tiempo += actual.Duracion;
            actual.Fin = tiempo;
            gantt.Add((actual.Nombre, actual.Inicio, actual.Fin));

            // Incorporar procesos que llegaron DURANTE la ejecución de "actual"
            while (pendientes.Count > 0 && pendientes[0].Llegada <= tiempo)
            {
                pila.Push(pendientes[0]);
                pendientes.RemoveAt(0);
            }
        }

        return gantt;
    }

    static void MostrarResultados(List<Proceso> procesos, List<(string nombre, int inicio, int fin)> gantt)
    {
        Console.WriteLine();
        Console.WriteLine("Orden de ejecución (Diagrama de Gantt):");
        Console.WriteLine(string.Join(" | ", gantt.Select(g => $"{g.nombre}({g.inicio}-{g.fin})")));
        Console.WriteLine();

        Console.WriteLine($"{"Proceso",-10}{"T. Llegada",-12}{"Duración",-12}{"T. Espera",-12}{"T. Ejecución",-14}");

        var procesosOrden = procesos.OrderBy(p => p.Nombre).ToList();

        foreach (var p in procesosOrden)
        {
            Console.WriteLine($"{p.Nombre,-10}{p.Llegada,-12}{p.Duracion,-12}{p.Espera,-12}{p.Retorno,-14}");
        }
    }

    // Lee un entero validando que no falle si el usuario escribe cualquier cosa
    static int LeerEntero(string mensaje)
    {
        int valor;
        while (true)
        {
            Console.Write(mensaje);
            string entrada = Console.ReadLine();
            if (int.TryParse(entrada, out valor) && valor >= 0)
                return valor;
            Console.WriteLine("  -> Ingresá un número entero válido (mayor o igual a 0).");
        }
    }

    static List<Proceso> IngresarProcesos()
    {
        int cantidad = LeerEntero("¿Cuántos procesos vas a ingresar? ");

        var procesos = new List<Proceso>();
        for (int i = 1; i <= cantidad; i++)
        {
            Console.WriteLine();
            Console.WriteLine($"--- Proceso {i} ---");

            Console.Write("Nombre del proceso (ej: P1): ");
            string nombre = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nombre))
                nombre = $"P{i}";

            int llegada = LeerEntero("Tiempo de llegada (To): ");
            int duracion;
            do
            {
                duracion = LeerEntero("Duración (D): ");
                if (duracion <= 0)
                    Console.WriteLine("  -> La duración debe ser mayor a 0.");
            } while (duracion <= 0);

            procesos.Add(new Proceso(nombre, llegada, duracion));
        }

        return procesos;
    }

    static void Main()
    {
        Console.WriteLine("=== Simulador de planificación LCFS ===");
        var procesos = IngresarProcesos();

        var gantt = Lcfs(procesos);
        MostrarResultados(procesos, gantt);

        Console.WriteLine();
        Console.WriteLine("Presioná ENTER para salir...");
        Console.ReadLine();
    }
}