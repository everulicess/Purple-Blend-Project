using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CombatPlayerController : MonoBehaviour
{
    private CombatController combatController;

    // Start is called before the first frame update
    void Start()
    {
        combatController = gameObject.GetComponent<CombatController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClickToAttack();
        }
    }
    private void ClickToAttack()
    {
        // Converts click on the screen to a position in the game world.
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            combatController.point = hit.point;
        }
        combatController.Attack();
    }
}
