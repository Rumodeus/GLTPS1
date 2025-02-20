using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteraction : MonoBehaviour
{
    [SerializeField] private PlayerInputInitialize playerInputScript;
    private GameObject kanazawa;
    [SerializeField] private GameObject chito;
    [SerializeField] private GameObject node;
    private bool isInteractable;
    public bool triggerDialogue;
    public Animator dialogueBoxAnim;
    public CharacterPossession characterPossessionScript;
    void OnEnable()
    {
        playerInputScript.actions.Player.Interact.performed += Interact;
    }
    void OnDisable()
    {
        playerInputScript.actions.Player.Interact.performed -= Interact;
    }

    private void Interact(InputAction.CallbackContext context)
    {
        if (context.performed && triggerDialogue)
        {
            kanazawa.GetComponent<DialogueManager>().NextLine();
        }

        if (context.performed && isInteractable && !triggerDialogue && dialogueBoxAnim.GetCurrentAnimatorStateInfo(0).IsTag("Close"))
        {
            // stop agent patrol
            kanazawa.GetComponent<NavMeshAgentPatrol>().patrol = false;

            // rotate agent to player
            kanazawa.GetComponent<RotateToPlayer>().enabled = true;
            kanazawa.GetComponent<RotateToPlayer>().player = transform;

            // set agent destination to in place
            kanazawa.GetComponent<NavMeshAgentPatrol>().agent.destination = kanazawa.transform.position;

            // trigger dialogue
            kanazawa.GetComponent<DialogueManager>().enabled = true;
            kanazawa.GetComponent<DialogueManager>().ShowDialogueBox();
            triggerDialogue = true;

            // disable switch character
            characterPossessionScript.enabled = false;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Kanazawa") && this.enabled)
        {
            kanazawa = other.gameObject;
            kanazawa.GetComponent<DialogueManager>().NPCInteractionScript = this;
            kanazawa.GetComponent<UnlockDialogueItem>().dialogueItems[0].reference[0] = chito;
            kanazawa.GetComponent<UnlockDialogueItem>().dialogueItems[1].reference[0] = chito;
            node.GetComponent<KanazawaInstanceManager>().firstKanazawa = kanazawa;
            isInteractable = true;
        }

        if (other.gameObject.CompareTag("Node") && other.gameObject.GetComponent<Node>() != null)
        {
            if (other.gameObject.GetComponent<Node>().isActive)
            {
                node = other.gameObject;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Kanazawa") && this.enabled)
        {
            isInteractable = false;

            // start agent patrol
            other.gameObject.GetComponent<NavMeshAgentPatrol>().patrol = true;
            other.gameObject.GetComponent<RotateToPlayer>().enabled = false;
            StartCoroutine(other.gameObject.GetComponent<NavMeshAgentPatrol>().SetDestination());

            //reset dialogue
            if (!dialogueBoxAnim.GetCurrentAnimatorStateInfo(0).IsTag("Close"))
            {
                other.gameObject.GetComponent<DialogueManager>().audioSource.Stop();
                other.gameObject.GetComponent<DialogueManager>().HideDialogueBox();
            }
        }
    }
}
