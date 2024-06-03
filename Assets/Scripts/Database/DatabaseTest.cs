using UnityEngine;
using SQLite;
using System.IO;

public class DatabaseTest : MonoBehaviour
{
    private SQLiteConnection db;

    void Start()
    {
        string databasePath = Path.Combine(Application.persistentDataPath, "MyDatabase.db");
        db = new SQLiteConnection(databasePath);

        // Création de la table Guide si elle n'existe pas déjà
        db.CreateTable<Guide>();

        // Ajouter des utilisateurs pour tester
        db.Insert(new Guide { Name = "Turquoise", Status = "DRAFT" });
        db.Insert(new Guide { Name = "Emeraude", Status = "PUBLISHED" });

        var guides = db.Table<Guide>();
        foreach (var guide in guides)
        {
            Debug.Log($"Guide ID: {guide.Id}, Name: {guide.Name}, Status: {guide.Status}");
        }
    }

    void OnApplicationQuit()
    {
        db.Close();
    }
}
