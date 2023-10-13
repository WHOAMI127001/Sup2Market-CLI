using System;
using System.Collections.Generic;

class Product
{
    public string Name { get; set; } // nom du produit //
    public int Stock { get; set; } // Quantité du produit // 
    public string Category { get; set; }  // Catégories des produits // 
}

class StoreManager
{
    public Dictionary<string, Product> Products = new Dictionary<string, Product>();
    private string _filePath = @"C:\Users\Cheema\Desktop\DB_TXT\database.txt";

    public void AddProduct(string name, int stock, string category)
    { // Ajoute un produit au magasin et le sauvegarde dans le fichier.
        Products[name] = new Product { Name = name, Stock = stock, Category = category };
        SaveProductsToFile();
    }

    public void AddStock(string productName, int quantityToAdd) // classe pour ajouter de la quantité à un produit existant //
    {
        if (Products.ContainsKey(productName) && quantityToAdd > 0)
        {
            Products[productName].Stock += quantityToAdd;
            SaveProductsToFile();
            Console.WriteLine($"{quantityToAdd} unit(s) added to {productName}. New stock: {Products[productName].Stock}");
        }
        else
        {
            Console.WriteLine("Le produit n'existe pas  ou il n'y a pas asser de stock!");
        }
    }

    public void DisplayProducts()   // classe pour afficher les produits //
    {
        Console.WriteLine("Produits:");
        foreach (var product in Products.Values)
        {
            Console.WriteLine($"{product.Name}: {product.Stock} in stock, Category: {product.Category}");
        }
    }

    public void SaveProductsToFile() // classe pour sauvegarder les produits dans la database du txt //
    {
        using (var writer = new System.IO.StreamWriter(_filePath))
        {
            foreach (var product in Products.Values)
            {
                writer.WriteLine($"{product.Name};{product.Stock};{product.Category}");
            }
        }
    }

    public void LoadProductsFromFile()  // classe qui charge les produits depuis la bdd qui est stockée en local //
    {
        if (!System.IO.File.Exists(_filePath)) return;

        var lines = System.IO.File.ReadAllLines(_filePath);
        foreach (var line in lines)
        {
            var parts = line.Split(';');
            if (parts.Length == 3)
            {
                AddProduct(parts[0], int.Parse(parts[1]), parts[2]);
            }
        }
    }
}

class Caissier
{
    private StoreManager _storeManager;

    public Caissier(StoreManager storeManager)
    {
        _storeManager = storeManager;
    }

    public void Checkout(string productName, int quantity)
    {
        if (_storeManager.Products.ContainsKey(productName) && _storeManager.Products[productName].Stock >= quantity)
        {
            _storeManager.Products[productName].Stock -= quantity;
            _storeManager.SaveProductsToFile();
            Console.WriteLine($"{quantity} {productName}(s) vendu!");
        }
        else
        {
            Console.WriteLine("Il n'y a pas assez de stock ou le produit n'existe pas!");
        }
    }
}

class Program
{
    static void Main()
    {
        StoreManager storeManager = new StoreManager();
        storeManager.LoadProductsFromFile();
        Caissier caissier = new Caissier(storeManager);

        while (true)
        {
            Console.WriteLine("Choose role:\n1: Manager\n2: Caissier\n3: Exit");
            string roleChoice = Console.ReadLine();

            switch (roleChoice)
            {
                case "1":
                    ManagerMenu(storeManager);
                    break;
                case "2":
                    CaissierMenu(caissier, storeManager);
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Choix Invalide, veuillez réessayer!");
                    break;
            }
        }
    }

    static void ManagerMenu(StoreManager storeManager)
    {
        while (true)
        {
            Console.WriteLine("1: Ajouter un produit\n2: Afficher un produit\n3: Ajouter du stock\n4: Revenir en arrière");
            string choice = Console.ReadLine();

            switch (choice)  // utilisation du switchcase pour éviter de faire que des if et des else if //
            {
                case "1":
                    Console.Write("Nom Du Produit: ");
                    string name = Console.ReadLine();
                    Console.Write("Stock: ");
                    int stock;
                    while (!int.TryParse(Console.ReadLine(), out stock) || stock < 0)
                    {
                        Console.Write("Choix invalide, veuillez ajouter une quantité positive: ");
                    }
                    Console.Write("Catégorie: ");
                    string category = Console.ReadLine();
                    storeManager.AddProduct(name, stock, category);
                    break;
                case "2":
                    storeManager.DisplayProducts();
                    break;
                case "3":
                    Console.Write("Nom Du Produit: ");
                    string productName = Console.ReadLine();
                    Console.Write("Quantité à ajouter: ");
                    int quantityToAdd;
                    while (!int.TryParse(Console.ReadLine(), out quantityToAdd) || quantityToAdd <= 0)
                    {
                        Console.Write("Entrée invalide, entrez un nombre positif pour la quantité à ajouter: ");
                    }
                    storeManager.AddStock(productName, quantityToAdd);
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Choix invalide, veuillez réessayer!");
                    break;
            }
        }
    }

    static void CaissierMenu(Caissier caissier, StoreManager storeManager)
    {
        while (true)
        {
            Console.WriteLine("1: Checkout\n0: Back");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Nom Du Produit: ");
                    string productName = Console.ReadLine();
                    Console.Write("Quantité: ");
                    int quantity;
                    while (!int.TryParse(Console.ReadLine(), out quantity) || quantity <= 0)
                    {
                        Console.Write("Choix invalide, veuillez ajouter une quantité positive: ");
                    }
                    caissier.Checkout(productName, quantity);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Choix invalide, veuillez réessayer!");
                    break;
            }
        }
    }
}
