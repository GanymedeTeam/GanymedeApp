using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class Notepad : MonoBehaviour
{
    public GameObject notePrefab;
    public GameObject content;
    public GameObject rootMenu;
    public GameObject editMenu;

    public TMP_InputField nameInput;
    public TMP_InputField contentInput;
    public Note targetedNote = null;

    public int currentNoteId;
    private string FilePath;

    [Serializable]
    public class NoteSerialize
    {
        public string noteName;
        public string noteContent;
        public int noteId;
    }

    [Serializable]
    private class NotepadSerialize
    {
        public List<NoteSerialize> notes;
    }

    public void OnEnable()
    {
        FilePath = Application.persistentDataPath + "/notepadData.json";
        GoToRootMenu();
    }

    private void CreateNotepadJson()
    {
        string jsonToWrite = "{\"notes\": []}";
        System.IO.File.WriteAllText(FilePath, jsonToWrite);
    }

    public void EditOrCreateNote()
    {
        string jsonToRead;
        NotepadSerialize serializedNotes;

        try
        {
            jsonToRead = File.ReadAllText(FilePath);
            serializedNotes = JsonUtility.FromJson<NotepadSerialize>(jsonToRead);
        }
        catch
        {
            CreateNotepadJson();
            jsonToRead = File.ReadAllText(FilePath);
        }

        serializedNotes = JsonUtility.FromJson<NotepadSerialize>(jsonToRead);

        NoteSerialize note = new NoteSerialize
        {
            noteName = nameInput.text,
            noteContent = contentInput.text,
            noteId = -1
        };

        if (targetedNote == null)
        {
            note.noteId = serializedNotes.notes.Count();
            serializedNotes.notes.Add(note);
        }
        else
        {
            note.noteId = targetedNote.noteId;
            serializedNotes.notes[note.noteId] = note;
        }
        string jsonToWrite = JsonUtility.ToJson(serializedNotes);
        System.IO.File.WriteAllText(FilePath, jsonToWrite);
        GoToRootMenu();
    }

    private IEnumerator ResizeContentGridLayout()
    {
        yield return 0;
        content.GetComponent<GridLayoutGroup>().cellSize = new Vector2(content.GetComponent<RectTransform>().rect.width - 20f, 32f);
    }

    public void GoToRootMenu()
    {
        StartCoroutine(ResizeContentGridLayout());
        targetedNote = null;
        editMenu.SetActive(false);
        rootMenu.SetActive(true);
        ShowNotes();
    }

    public void ShowNotes()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
        try
        {
            string jsonToRead = File.ReadAllText(FilePath);
            NotepadSerialize serializedNotes = JsonUtility.FromJson<NotepadSerialize>(jsonToRead);
            foreach (NoteSerialize note in serializedNotes.notes)
            {
                SpawnNote(note);
            }
        }
        catch { }
    }

    public void SpawnNote(NoteSerialize note)
    {
        GameObject gameobjectNote = Instantiate(notePrefab, content.transform);
        gameobjectNote.GetComponent<Note>().noteName = note.noteName;
        gameobjectNote.GetComponent<Note>().noteContent = note.noteContent;
        gameobjectNote.GetComponent<Note>().noteId = note.noteId;
        gameobjectNote.GetComponent<Note>().Notepad = gameObject;
        gameobjectNote.transform.Find("NoteButton/NoteName").GetComponent<TMP_Text>().text = note.noteName;
        gameobjectNote.transform.Find("NoteButton/NotePreview").GetComponent<TMP_Text>().text = note.noteContent;
        gameobjectNote.transform.Find("NoteCopyButton").GetComponent<Button>().onClick.AddListener(
            delegate {
                gameobjectNote.GetComponent<Note>().CopyContent(); 
            }
        );
        gameobjectNote.transform.Find("NoteDeleteButton").GetComponent<Button>().onClick.AddListener(
            delegate {
                RemoveNote(gameobjectNote); 
            }
        );
        gameobjectNote.transform.Find("NoteButton").GetComponent<Button>().onClick.AddListener(
            delegate {
                gameobjectNote.GetComponent<Note>().OnClickModifyCurrentID();
                GoToEditMenu(); 
            }
        );
    }

    public void RemoveNote(GameObject note)
    {
        string jsonToRead = File.ReadAllText(FilePath);
        NotepadSerialize serializedNotes = JsonUtility.FromJson<NotepadSerialize>(jsonToRead);
        serializedNotes.notes.RemoveAt(note.GetComponent<Note>().noteId);
        foreach (NoteSerialize tmpNote in serializedNotes.notes)
        {
            tmpNote.noteId = serializedNotes.notes.IndexOf(tmpNote);
        }
        string jsonToWrite = JsonUtility.ToJson(serializedNotes);
        System.IO.File.WriteAllText(FilePath, jsonToWrite);
        ShowNotes();
    }


    public void GoToEditMenu()
    {
        rootMenu.SetActive(false);
        editMenu.SetActive(true);
        try{
            nameInput.text = targetedNote.noteName;
            contentInput.text = targetedNote.noteContent;
        }catch
        {
            nameInput.text = "";
            contentInput.text = "";
        }
    }
}
