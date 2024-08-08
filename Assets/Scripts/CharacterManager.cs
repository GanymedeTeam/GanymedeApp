using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    private GameObject actualCharacter;
    public GameObject characterPrefab;
    public GameObject content;

    void Start()
    {
        // Load existing saves
        DirectoryInfo saveDir = new DirectoryInfo($"{Application.persistentDataPath}/Saves");
        DirectoryInfo[] saveDirectories = saveDir.GetDirectories();
        foreach (DirectoryInfo directory in saveDirectories)
        {
            GameObject character = Instantiate(characterPrefab);
            character.transform.SetParent(transform.Find("Group"));
            character.name = directory.Name;
            character.transform.Find("CharacterInfo/CharacterName").GetComponent<TMP_Text>().text = character.name;
            character.transform.Find("CharacterInfo/LoadCharacter").GetComponent<Button>().onClick.AddListener(
                delegate {
                    SelectSaveProfile(character);
                }
            );
            character.transform.Find("CharacterInfo/RemoveCharacter").GetComponent<Button>().onClick.AddListener(
                delegate {
                    RemoveProfile(character);
                }
            );
        }

        List<Transform> children = transform.Find("Group").GetComponentsInChildren<Transform>().Where(t => t != transform.Find("Group").transform).ToList();
        Transform foundChild = children.FirstOrDefault(child => child.name.Equals(FindObjectOfType<SaveManager>().saveName));
        actualCharacter = foundChild.gameObject;
        actualCharacter.transform.Find("CharacterInfo/SelectedBg").gameObject.SetActive(true);
        actualCharacter.transform.Find("CharacterInfo/LoadCharacter").gameObject.SetActive(false);
        actualCharacter.transform.Find("CharacterInfo/RemoveCharacter").gameObject.SetActive(false);
        StartCoroutine(ResetContentSize());
        Debug.Log($"Loaded save: {actualCharacter.name}");
    }

    void SelectSaveProfile(GameObject character)
    {
        //Reset previous actual profile
        actualCharacter.transform.Find("CharacterInfo/SelectedBg").gameObject.SetActive(false);
        actualCharacter.transform.Find("CharacterInfo/LoadCharacter").gameObject.SetActive(true);
        actualCharacter.transform.Find("CharacterInfo/RemoveCharacter").gameObject.SetActive(true);
        //Set actual profile
        actualCharacter = character;
        actualCharacter.transform.Find("CharacterInfo/SelectedBg").gameObject.SetActive(true);
        actualCharacter.transform.Find("CharacterInfo/LoadCharacter").gameObject.SetActive(false);
        actualCharacter.transform.Find("CharacterInfo/RemoveCharacter").gameObject.SetActive(false);
        PlayerPrefs.SetString("CharacterNameSave", actualCharacter.name);
        FindObjectOfType<SaveManager>().saveName = actualCharacter.name;
        StartCoroutine(FindObjectOfType<SaveManager>().ProgressLoadJsonToClass());
        Debug.Log($"Loaded save: {actualCharacter.name}");
    }

    public void RemoveProfile(GameObject character)
    {
        //if it is actual, do nothing
        if (!character.Equals(actualCharacter))
        {
            Destroy(character);
            Directory.Delete($"{Application.persistentDataPath}/Saves/{character.name}", true);
        }
    }

    public void CreateProfile(string characterName)
    {
        DirectoryInfo saveDir = new DirectoryInfo($"{Application.persistentDataPath}/Saves");
        DirectoryInfo[] saveDirectories = saveDir.GetDirectories();
        if (saveDirectories.Any(d => d.Name == characterName))
        {
            Debug.Log($"Profile {characterName} already exists!");
        }
        else
        {
            GameObject character = Instantiate(characterPrefab);
            character.transform.SetParent(transform.Find("Group"));
            character.name = characterName;
            character.transform.Find("CharacterInfo/CharacterName").GetComponent<TMP_Text>().text = character.name;
            character.transform.Find("CharacterInfo/LoadCharacter").GetComponent<Button>().onClick.AddListener(
                delegate {
                    SelectSaveProfile(character);
                }
            );
            character.transform.Find("CharacterInfo/RemoveCharacter").GetComponent<Button>().onClick.AddListener(
                delegate {
                    RemoveProfile(character);
                }
            );
            if (!Directory.Exists($"{Application.persistentDataPath}/Saves/{characterName}"))
                Directory.CreateDirectory($"{Application.persistentDataPath}/Saves/{characterName}");
            StartCoroutine(ResetContentSize());
        }
    }

    private IEnumerator ResetContentSize()
    {
        content.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return 0;
        content.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }
}
