using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotebookScript : MonoBehaviour
{
    public const int MAXIMUM_NUMBER_OF_NOTES = 8;

    public NoteData[] notes = new NoteData[MAXIMUM_NUMBER_OF_NOTES];
    public GameObject[] noteButtonObjects = new GameObject[MAXIMUM_NUMBER_OF_NOTES];

    public void addNote(NoteData newNote)
    {
        for (int i = 0; i < notes.Length; i++)
        {
            if (notes[i] == null)
            {
                notes[i] = newNote;
                noteButtonObjects[i].SetActive(true);
                noteButtonObjects[i].GetComponent<Button>().enabled = true;
                noteButtonObjects[i].GetComponentInChildren<Text>().text = newNote.actorName;
                return;
            }
        }
    }

    public void removeNote(NoteData noteToRemove)
    {
        for (int i = 1; i < notes.Length; i++)
        {
            if (notes[i] == noteToRemove)
            {
                notes[i] = null;
                noteButtonObjects[i].GetComponent<Button>().enabled = false;
                noteButtonObjects[i].SetActive(false);

                // fill in gap in notebook left by removed note
                for (int j = i + 1; j < notes.Length; j++)
                {
                    notes[j - 1] = notes[j];
                    noteButtonObjects[j - 1].GetComponentInChildren<Text>().text = notes[j].actorName;
                    if (j + 1 <= notes.Length)
                    {
                        if (notes[j + 1] == null)
                        {
                            notes[j] = null;
                            noteButtonObjects[j].SetActive(false);
                        }
                    }
                }
                return;
            }
        }
    }

    public void noteButtonPressed(int buttonIndex)
    {
        closeNotebook();

        // conversation in progress
        if (FindObjectOfType<GameManagerScript>().ConversationUIOpen)
        {
            FindObjectOfType<ConversationScript>().showInventoryItem(notes[buttonIndex]);
        }

        // no conversation in progress
        else
        {
            FindObjectOfType<PlayerInputScript>().enableExamineObjectText(notes[buttonIndex].examineText);
        }
    }

    public void closeNotebook()
    {
        FindObjectOfType<PlayerInputScript>().closeNotebook();
    }
}
