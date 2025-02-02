using UnityEngine;

public class BlocNoteManager : MonoBehaviour
{
    public GameObject blocNote; 
    public GameObject image1;

    public void ShowBlocNote()
    {
        blocNote.SetActive(true);
        image1.SetActive(false);
    }

    public void HideBlocNote()
    {
        blocNote.SetActive(false);
        image1.SetActive(true);
    }
}
