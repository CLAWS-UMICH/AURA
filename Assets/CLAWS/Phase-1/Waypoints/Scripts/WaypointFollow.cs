// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class WaypointFollow : MonoBehaviour
// {
//     GameObject player;
//     [SerializeField] float minScale = 1f;
//     [SerializeField] float distanceAway = 5f;
//     [SerializeField] float coneAngle = 130f;

//     GameObject body;
//     GameObject title;
//     GameObject button;
//     GameObject icon;
//     GameObject ring;

//     float distance;
//     [SerializeField] float distanceForTitle = 2.5f;
//     [SerializeField] float distanceToSetYLvl = 2.5f;
//     [SerializeField] float distanceForButton = 1.25f;
//     bool isVisible;
//     float updateDistance;

//     bool levelHasBeenUpdated;

//     private void Awake()
//     {
//         // TODO: Update these debendencies to actually find the correct objects in the scene
//         player = GameObject.Find("Main Camera");
//         body = gameObject.transform.Find("Body").gameObject;
//         title = body.transform.Find("Title").gameObject;
//         icon = body.transform.Find("Quad").gameObject;
//         ring = body.transform.Find("Ring").gameObject;
//         button = body.transform.Find("DeleteButton").gameObject;

//         distance = Vector3.Distance(body.transform.position, player.transform.position);
//         isVisible = !(distance > distanceAway);

//         levelHasBeenUpdated = false;
//     }

//     // Below should all be the same
//     private void Start()
//     {
//         title.SetActive(false);
//         icon.SetActive(false);
//         button.SetActive(false);
//         ring.SetActive(false);

//         StartCoroutine(CheckDistance());
//     }

//     IEnumerator CheckDistance()
//     {
//         while (true)
//         {
//             yield return new WaitForSeconds(0.5f);
//             distance = Vector3.Distance(body.transform.position, player.transform.position);

//             // Get the direction from the player to the sign object
//             Vector3 directionToSign = body.transform.position - player.transform.position;

//             // Get the dot product between the direction to the sign and the player's forward direction
//             float dotProduct = Vector3.Dot(directionToSign.normalized, player.transform.forward);

//             if (distance <= distanceAway || (dotProduct > Mathf.Cos(coneAngle * Mathf.Deg2Rad * 0.5f)))
//             {
//                 isVisible = true;
//                 icon.SetActive(true);

//                 if (distance < distanceForTitle)
//                 {
//                     title.SetActive(true);
//                     icon.SetActive(false);
//                     ring.SetActive(true);
//                 }

//                 if (distance < distanceForButton)
//                 {
//                     //button.SetActive(true);
//                 }
                
//             }
//             else
//             {
//                 isVisible = false;
//                 icon.SetActive(false);
//                 title.SetActive(false);
//                 ring.SetActive(false);
//                 //button.SetActive(false);
//             }

//             if (distance >= distanceForTitle)
//             {
//                 title.SetActive(false);
//                 icon.SetActive(true);
//                 ring.SetActive(false);
//             }

//             if (distance >= distanceForButton)
//             {
//                 //button.SetActive(false);
//             }

//             if (!levelHasBeenUpdated && distance < distanceToSetYLvl)
//             {
//                 levelHasBeenUpdated = true;
//             }
//         }
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (isVisible)
//         {
//             if (!levelHasBeenUpdated)
//             {
//                 body.transform.position = new Vector3(body.transform.position.x, player.transform.position.y, body.transform.position.z);
//             }
//             updateDistance = Vector3.Distance(body.transform.position, player.transform.position);
//             body.transform.rotation = Quaternion.Euler(0, player.transform.eulerAngles.y, 0);
//             float scale = updateDistance / 10f;
//             scale = Mathf.Max(scale, minScale);
//             icon.transform.localScale = Vector3.one * scale;
//         }
//     }
// }
