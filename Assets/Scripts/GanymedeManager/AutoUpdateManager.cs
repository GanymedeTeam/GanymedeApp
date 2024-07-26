using System.Collections;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class AutoUpdateManager : MonoBehaviour
{
    public string githubReleasesApiUrl = "https://api.github.com/repos/GanymedeTeam/GanymedeApp/releases/latest";
    private string updateFilePath;

    private void Start()
    {
        StartCoroutine(CheckForUpdates());
    }

    private IEnumerator CheckForUpdates()
    {
        string currentVersion = Application.version;
        string latestVersion = null;

        // Obtenir la dernière version depuis GitHub
        yield return StartCoroutine(GetLatestVersionTag(tag => latestVersion = tag));

        // if (latestVersion != null && !currentVersion.Equals(latestVersion.Replace("v", "")))
        if (latestVersion != null && !currentVersion.Equals(latestVersion))
        {
            UnityEngine.Debug.Log("Nouvelle version disponible ! Déclenchement de la mise à jour...");
            // Appel à la fonction pour télécharger et appliquer la mise à jour
            yield return StartCoroutine(DownloadUpdate());
        }
        else
        {
            UnityEngine.Debug.Log("Aucune mise à jour disponible.");
        }
    }

    private IEnumerator GetLatestVersionTag(System.Action<string> onSuccess)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(githubReleasesApiUrl))
        {
            request.SetRequestHeader("User-Agent", "UnityGame");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                string tagName = ExtractTagNameFromJson(json);
                onSuccess?.Invoke(tagName);
            }
            else
            {
                UnityEngine.Debug.LogError($"Erreur lors de la récupération des informations de release : {request.error}");
            }
        }
    }

    private string ExtractTagNameFromJson(string json)
    {
        var jsonObj = JsonUtility.FromJson<GithubRelease>(json);
        return jsonObj.tag_name;
    }

    private IEnumerator DownloadUpdate()
    {
        string tempFilePath = Path.Combine(Application.persistentDataPath, "update.zip");

        using UnityWebRequest request = UnityWebRequest.Get("https://github.com/GanymedeTeam/GanymedeApp/releases/latest/download/UnityPackage.zip");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            File.WriteAllBytes(tempFilePath, request.downloadHandler.data);
            updateFilePath = tempFilePath;
            UnityEngine.Debug.Log("Téléchargement terminé. Application de la mise à jour...");
            StartCoroutine(ApplyUpdate());
        }
        else
        {
            UnityEngine.Debug.LogError($"Erreur lors du téléchargement de la mise à jour : {request.error}");
            UnityEngine.Debug.LogError($"Code de réponse HTTP : {request.responseCode}");
            UnityEngine.Debug.LogError($"URL demandée : {request.url}");
        }
    }

    private IEnumerator ApplyUpdate()
    {
        string tempUpdateFolder = Path.Combine(Application.temporaryCachePath, "TempUpdate");
        
        // Assurez-vous que le dossier temporaire existe
        if (!Directory.Exists(tempUpdateFolder))
        {
            Directory.CreateDirectory(tempUpdateFolder);
        }

        // Extraire les fichiers du ZIP
        ExtractZipFile(updateFilePath, tempUpdateFolder);

        // Déplacer les fichiers extraits vers le répertoire de votre jeu
        // Assurez-vous que vous avez les droits nécessaires pour écrire dans le répertoire du jeu
        string gameDirectory = Path.GetDirectoryName(Application.dataPath);
        MoveFilesRecursively(tempUpdateFolder, gameDirectory);

        // Nettoyage
        Directory.Delete(tempUpdateFolder, true);

        // Lancer le jeu après la mise à jour ou effectuer d'autres actions si nécessaire
        UnityEngine.Debug.Log("Mise à jour appliquée avec succès.");
        Application.Quit();
        yield return null;
    }

        private static void ExtractZipFile(string zipFilePath, string extractionPath)
    {
        // Assurez-vous que le répertoire d'extraction existe
        if (!Directory.Exists(extractionPath))
        {
            Directory.CreateDirectory(extractionPath);
        }

        // Ouvrir le fichier ZIP
        using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
        {
            // Parcourir les entrées dans le ZIP
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                // Déterminer le chemin complet pour l'extraction
                string destinationPath = Path.Combine(extractionPath, entry.FullName);

                // Si l'entrée est un répertoire, créer le répertoire
                if (entry.FullName.EndsWith("/"))
                {
                    if (!Directory.Exists(destinationPath))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }
                }
                else // Si l'entrée est un fichier, extraire le fichier
                {
                    // Créer le répertoire parent si nécessaire
                    string directoryPath = Path.GetDirectoryName(destinationPath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Extraire le fichier
                    using (var entryStream = entry.Open())
                    using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                    {
                        entryStream.CopyTo(fileStream);
                    }
                }
            }
        }
    }

    private static void MoveFilesRecursively(string sourceDir, string destDir)
    {
        // Assurez-vous que le répertoire de destination existe
        if (!Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
        }

        foreach (var file in Directory.GetFiles(sourceDir))
        {
            string destFile = Path.Combine(destDir, Path.GetFileName(file));
            File.Copy(file, destFile, true);
        }

        foreach (var directory in Directory.GetDirectories(sourceDir))
        {
            string destDirectory = Path.Combine(destDir, Path.GetFileName(directory));
            MoveFilesRecursively(directory, destDirectory);
        }
    }

    [System.Serializable]
    private class GithubRelease
    {
        public string tag_name;
    }
}
