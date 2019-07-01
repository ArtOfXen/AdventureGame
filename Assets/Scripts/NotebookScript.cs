using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookScript : MonoBehaviour
{
    public const int MAXIMUM_NUMBER_OF_NOTES = 8;

    public ActorData[] notes = new ActorData[MAXIMUM_NUMBER_OF_NOTES];

    public void addNote(ActorData newNote)
    {
        for (int i = 0; i < notes.Length; i++)
        {
            if (notes[i] == null)
            {
                notes[i] = newNote;
                return;
            }
        }
    }

    public void removeNote(ActorData noteToRemove)
    {
        for (int i = 1; i < notes.Length; i++)
        {
            if (notes[i] == noteToRemove)
            {
                notes[i] = null;
                return;
            }
        }
    }
}
