using System;
using System.Collections.Generic;

public enum VerschijningsPeriode
{
    Dagelijks,
    Wekelijks,
    Maandelijks
}

public class Boek
{
    public string Isbn { get; set; }
    public string Naam { get; set; }
    public string Uitgever { get; set; }
    private decimal prijs;

    public decimal Prijs
    {
        get => prijs;
        set
        {
            if (value < 5 || value > 50)
                throw new ArgumentOutOfRangeException("Prijs moet tussen 5 en 50 euro zijn.");
            prijs = value;
        }
    }

    public Boek(string isbn, string naam, string uitgever, decimal prijs)
    {
        Isbn = isbn;
        Naam = naam;
        Uitgever = uitgever;
        Prijs = prijs;
    }

    public override string ToString()
    {
        return $"{Naam} (ISBN: {Isbn}) - {Uitgever} - €{Prijs}";
    }

    public virtual void Lees()
    {
        Console.WriteLine(ToString());
    }
}

public class Tijdschrift : Boek
{
    public VerschijningsPeriode Periode { get; set; }

    public Tijdschrift(string isbn, string naam, string uitgever, decimal prijs, VerschijningsPeriode periode)
        : base(isbn, naam, uitgever, prijs)
    {
        Periode = periode;
    }

    public override string ToString()
    {
        return base.ToString() + $" - Verschijnt {Periode}";
    }

    public override void Lees()
    {
        Console.WriteLine(ToString());
    }
}

public class Bestelling<T>
{
    private static int uniekeId = 0;
    public int Id { get; private set; }
    public T Item { get; set; }
    public DateTime Datum { get; set; }
    public int Aantal { get; set; }
    public VerschijningsPeriode? AbonnementPeriode { get; set; }

    public Bestelling(T item, int aantal, VerschijningsPeriode? abonnementPeriode = null)
    {
        Id = ++uniekeId;
        Item = item;
        Datum = DateTime.Now;
        Aantal = aantal;
        AbonnementPeriode = abonnementPeriode;
    }


    public (string Isbn, int Aantal, decimal TotalePrijs) BestelBoek(Boek boek)
    {
        decimal totalePrijs = boek.Prijs * Aantal;
        BestellingGeplaatst?.Invoke(this, EventArgs.Empty);
        return (boek.Isbn, Aantal, totalePrijs);
    }


    public static event EventHandler BestellingGeplaatst;
}

class Program
{
    static void Main(string[] args)
    {

        Bestelling<Boek>.BestellingGeplaatst += (sender, e) =>
        {
            Console.WriteLine("Bestelling is geplaatst!");
        };

        Boek boek1 = new Boek("978-3-16-148410-0", "De naam van de roos", "Uitgeverij X", 20);
        Boek boek2 = new Boek("978-1-4028-9462-6", "1984", "Uitgeverij Y", 15);

        Tijdschrift tijdschrift1 = new Tijdschrift("123-4-567-89012-3", "Wetenschap Vandaag", "Uitgeverij Z", 8, VerschijningsPeriode.Maandelijks);
        Tijdschrift tijdschrift2 = new Tijdschrift("234-5-678-90123-4", "Technologie Nu", "Uitgeverij A", 10, VerschijningsPeriode.Wekelijks);

        Bestelling<Boek> bestellingBoek = new Bestelling<Boek>(boek1, 3);
        var bestelInfo = bestellingBoek.BestelBoek(boek1);
        Console.WriteLine($"Besteld: ISBN {bestelInfo.Isbn}, Aantal: {bestelInfo.Aantal}, Totale prijs: €{bestelInfo.TotalePrijs}");

        Bestelling<Tijdschrift> bestellingTijdschrift = new Bestelling<Tijdschrift>(tijdschrift1, 1, VerschijningsPeriode.Maandelijks);
        bestellingTijdschrift.BestelBoek(tijdschrift1);

        Console.ReadLine();
    }
}