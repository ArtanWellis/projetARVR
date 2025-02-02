using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSVblocNote : MonoBehaviour
{
    public Dropdown studentsDropdown;
    public Dropdown objectsDropdown;
    public Dropdown roomsDropdown;
    public InputField blocNoteInput;
    
    private List<string> studentsList = new List<string>();
    private List<string> objectsList = new List<string>();
    private List<string> roomsList = new List<string> { "Room01","Room02","Room03","Room07","Room25", "Room42", "Room260","Room420","Room507","Room682" };
    public TimeSelector dataReader;

       void Start()
    {
        // Charger les données des étudiants et des objets depuis le dataReader
        studentsList = dataReader.getListOfSuspects().ConvertAll(s => s.getName());
        objectsList = dataReader.getListOfObjects().ConvertAll(o => o.oName);

        // Remplir les Dropdowns
        PopulateDropdown(studentsDropdown, studentsList);
        PopulateDropdown(objectsDropdown, objectsList);
        PopulateDropdown(roomsDropdown, roomsList);

        // Ajouter les listeners pour mettre à jour l'InputField
        studentsDropdown.onValueChanged.AddListener(delegate { UpdateBlocNote(studentsDropdown); });
        objectsDropdown.onValueChanged.AddListener(delegate { UpdateBlocNote(objectsDropdown); });
        roomsDropdown.onValueChanged.AddListener(delegate { UpdateBlocNote(roomsDropdown); });
    }

    void PopulateDropdown(Dropdown dropdown, List<string> options)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }

    void UpdateBlocNote(Dropdown dropdown)
    {
        blocNoteInput.text += dropdown.options[dropdown.value].text +" ";
    }
}
