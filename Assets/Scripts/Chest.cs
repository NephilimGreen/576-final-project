using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    public Canvas canvas;
    private CanvasManager canvasManager;

    private GameObject equationPanel;
    private GameObject chestPanel;

    private ItemSlot[] equationSlots;
    private ItemSlot[] chestSlots;

    private Button submitButton;

    private Camera playerCamera;
    private float maxDistance = 5f;

    // a random 3 digit integer in range [100,999]
    int solution;

    // The chest has 2 inventories: one is the storage, and one is the place where you solve the equation

    // Start is called before the first frame update
    void Start()
    {
        solution = Random.Range(100, 1000);

        playerCamera = Camera.main;
        canvasManager = canvas.GetComponent<CanvasManager>();
        equationPanel = canvasManager.equationPanel;
        equationSlots = equationPanel.GetComponentsInChildren<ItemSlot>();

        chestPanel = canvasManager.chestPanel;
        chestSlots = chestPanel.GetComponentsInChildren<ItemSlot>();

        submitButton = canvasManager.submitButton.GetComponent<Button>();
        submitButton.onClick.AddListener(Submit);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    canvas.GetComponent<CanvasManager>().OpenChestMenu();
                    Debug.Log("opened");
                }
            }
        }
        //PrintContents();
    }

    void Submit()
    {
        // generate a string for the equation
        string equation = "";
        for (int i = 0; i < equationSlots.Length; i++)
        {
            if (!equationSlots[i].IsEmpty())
            {
                string text = equationSlots[i].transform.GetChild(0).GetComponent<TMP_Text>().text;
                equation += text;
            }
        }
        Debug.Log(equation);
        Debug.Log(Equation.CalculateSolution(equation));
        if (Equation.CheckProblem(equation, solution))
        {
            Debug.Log("Correct");
        }
        else
        {
            Debug.Log("Incorrect");
        }
    }

    void PrintContents()
    {
        string str = "";
        for (int i = 0; i < equationSlots.Length; i++)
        {
            if (equationSlots[i].IsEmpty()) str += "1";
            else
            {
                 str += "0";
            }
        }
        Debug.Log("Equation: " + str);

        str = "";
        for (int i = 0; i < chestSlots.Length; i++)
        {
            if (chestSlots[i].IsEmpty()) str += "1";
            else
            {
                str += "0";
            }
        }
        Debug.Log("Chest: " + str);
    }
}
