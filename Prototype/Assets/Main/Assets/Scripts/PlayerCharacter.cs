
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Yarn.Unity.Example {
	public class PlayerCharacter : MonoBehaviour {

		public float minPosition;
		public float maxPosition;

		public float moveSpeed;

		public float interactionRadius;

		public float movementFromButtons {get;set;}

        public float positionX;
        public float positionY;

        private Vector3 mousePos;

        void Start()
        {
            transform.position = new Vector3(positionX, positionY, 0);
            mousePos = transform.position;
        }

        // Draw the range at which we'll start talking to people.
        void OnDrawGizmosSelected() {
			Gizmos.color = Color.blue;

			// Flatten the sphere into a disk, which looks nicer in 2D games
			Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1,1,0));

			// Need to draw at position zero because we set position in the line above
			Gizmos.DrawWireSphere(Vector3.zero, interactionRadius);
		}
                
		// Update is called once per frame
		void Update () {
            // Remove all player control when we're in dialogue
            if (FindObjectOfType<DialogueRunner>().isDialogueRunning == true) {
				return;
			}

			// Move the player, clamping them to within the boundaries 
			// of the level.            
    
            if (Input.GetMouseButtonDown(0)) {
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Debug.Log(Camera.main.ScreenToWorldPoint(mousePos));
                mousePos.x = Mathf.Clamp(mousePos.x, minPosition, maxPosition);
                mousePos.y = positionY;
                mousePos.z = 0;
            }

            transform.position = Vector3.MoveTowards(transform.position, mousePos, moveSpeed * Time.deltaTime);

            // Detect if we want to start a conversation

            if (Input.GetKeyDown(KeyCode.Space)) {
				CheckForNearbyNPC ();
			}
		}



		public void CheckForNearbyNPC ()
		{
			// Find all DialogueParticipants, and filter them to
			// those that have a Yarn start node and are in range; 
			// then start a conversation with the first one
			var allParticipants = new List<NPC> (FindObjectsOfType<NPC> ());
			var target = allParticipants.Find (delegate (NPC p) {
				return string.IsNullOrEmpty (p.talkToNode) == false && // has a conversation node?
				(p.transform.position - this.transform.position)// is in range?
				.magnitude <= interactionRadius;
			});
			if (target != null) {
				// Kick off the dialogue at this node.
				FindObjectOfType<DialogueRunner> ().StartDialogue (target.talkToNode);
			}
		}
	}



}
