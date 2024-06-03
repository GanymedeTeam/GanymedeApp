using UnityEngine;
using SQLite;
using System.IO;

public class DatabaseTest : MonoBehaviour
{
    private SQLiteConnection db;

    void Start()
    {
        string databasePath = Path.Combine(Application.persistentDataPath, "GanyBase.db");
        db = new SQLiteConnection(databasePath);

        // Création de la table Guide si elle n'existe pas déjà
        db.CreateTable<Guides>();

        // Ajouter des utilisateurs pour tester
        db.Insert(new Guides { name = "Turquoise", status = "DRAFT" });
        db.Insert(new Guides { name = "Emeraude", status = "PUBLISHED" });

        var guides = db.Table<Guides>();
        foreach (var guide in guides)
        {
            Debug.Log($"Guide ID: {guide.id}, Name: {guide.name}, Status: {guide.status}");
        }
    }

    void OnApplicationQuit()
    {
        db.Close();
    }
}
