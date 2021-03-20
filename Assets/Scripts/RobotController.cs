using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Text;

public enum RobotState {
    Patrolling,
    Examining,
    Uploading,
    Backoff,
}
public class RobotController : MonoBehaviour {
    public GameObject patrolPointsParent;
    public GameObject cameraParent;
    private Camera cam;
    public GameObject focalPoint;
    public float speed;
    public float visionRange;
    public float examineDistance;
    public float visionWidth;
    public float visionHeight;
    public float rotationSpeed;
    public float rotationPerPicture;
    public float maxRotation;
    private List<Transform> patrolPoints = new List<Transform>();
    private int currentPatrolPointIndex;
    private float minDist = 0.5f;
    private RobotState state = RobotState.Patrolling;
    private Transform examinePoint;
    private MeshCollider currentCollider;
    private DamageScript examinationScript;
    private LayerMask damageOrHull;
    private int damageLayer;
    bool started = false;
    float rotation = 0;
    float pictureRotationCounter = 0;
    bool approaching = true;
    private int fileCounter = 0;
    public int botID = 0;
    bool uploading = false;
    bool finishedUploading = false;
    public List<string> images = new List<string>();
    private float initialY;
    private float initialDist;
    bool resetY = false;
    Vector3 examinePosition;

    // Start is called before the first frame update
    void Start() {
        initialY = transform.position.y;
        initialDist = Vector3.Distance(transform.position, focalPoint.transform.position);
        int numPoints = patrolPointsParent.transform.childCount;
        for (int i = 0; i < numPoints; i++) {
            patrolPoints.Add(patrolPointsParent.transform.GetChild(i));
        }
        currentPatrolPointIndex = 0;
        cam = cameraParent.GetComponent<Camera>();
        damageOrHull = (1 << LayerMask.NameToLayer("Hull")) | (1 << LayerMask.NameToLayer("HullDamage"));
        damageLayer = LayerMask.NameToLayer("HullDamage");
        started = true;
    }

    // Update is called once per frame
    void Update() {
        switch (state) {
            case RobotState.Patrolling:
                Patrol();
                break;
            case RobotState.Examining:
                Examine();
                break;
            case RobotState.Uploading:
                WaitForUpload();
                break;
            case RobotState.Backoff:
                BackOff();
                break;
            default:
                break;
        }

    }

    private void Patrol() {
        transform.position += (patrolPoints[currentPatrolPointIndex].position - transform.position).normalized * speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPointIndex].position) < minDist) {
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Count;
        }
        cameraParent.transform.LookAt(focalPoint.transform);
        CheckForDamage();
    }

    void WaitForUpload() {
        if (finishedUploading) {
            uploading = false;
            SwitchToBackoff();
        }
    }

    private void SwitchToBackoff() {
        state = RobotState.Backoff;
    }

    private void CheckForDamage() {
        RaycastHit[] hits = Physics.BoxCastAll(cameraParent.transform.position, new Vector3(visionWidth, visionHeight, 0.1f), cam.transform.forward, Quaternion.identity, visionRange);
        foreach (RaycastHit hit in hits) {
            if (hit.transform.gameObject.layer == damageLayer) {
                if (hit.transform != examinePoint) {
                    examinationScript = hit.transform.GetComponent<DamageScript>(); 
                    currentCollider = hit.transform.GetComponent<MeshCollider>();
                }
                if (examinationScript != null) {
                    if (!examinationScript.IsReported() && LOSCheck(hit.transform.gameObject, visionRange)) {
                        examinePoint = hit.transform;
                        SwitchToExamine();
                    }
                }

            }
        }
    }

    private bool LOSCheck(GameObject playerObj, float sightRange) {
        RaycastHit hit;
        bool environmentCheck = Physics.Raycast(
            cameraParent.transform.position,
            playerObj.transform.position - cameraParent.transform.position,
            out hit,
            sightRange,
            damageOrHull
        );
        examinePosition = hit.point;
        return !environmentCheck
            || hit.transform.gameObject.layer == LayerMask.NameToLayer("HullDamage");
    }

    private void SwitchToExamine() {
        state = RobotState.Examining;
        approaching = true;
        rotation = 0;
        pictureRotationCounter = 0;

    }
    private void SwitchToPatrol() {
        state = RobotState.Patrolling;

    }

    private void BackOff() {
        transform.position += (patrolPoints[currentPatrolPointIndex].position - transform.position).normalized * speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPointIndex].position) < minDist) {
            SwitchToPatrol();
        }
    }

    private void Examine() {
        if (approaching) {
            if (Vector3.Distance(transform.position, currentCollider.bounds.center) > examineDistance) {
                transform.position += (currentCollider.bounds.center - transform.position).normalized * speed * Time.deltaTime;
            }
            else {
                approaching = false;
            }
        }
        else {
            if (rotation < maxRotation) {
                transform.RotateAround(currentCollider.bounds.center, new Vector3(0, 1, 0), rotationSpeed * Time.deltaTime);
                rotation += rotationSpeed * Time.deltaTime;
                pictureRotationCounter -= rotationSpeed * Time.deltaTime;
                if (pictureRotationCounter < 0) {
                    CamCapture();
                    pictureRotationCounter = rotationPerPicture;
                }
            }
            else {
                examinePoint.gameObject.GetComponent<DamageScript>().SetReported(true);
                UploadScreenshots();
            }

        }



        cameraParent.transform.LookAt(currentCollider.bounds.center);
    }

    void UploadScreenshots() {
        StartCoroutine(Upload());
        uploading = true;
        finishedUploading = false;
        images.Clear();
        state = RobotState.Uploading;
    }

    IEnumerator Upload() {

        //WWW www = new WWW("http://localhost:8080/api/reports");
        //Hashtable headers = new Hashtable();
        //headers.Add("Content-Type", "application/json");
        Debug.Log(images[1].Length);
        byte[] postData = Encoding.UTF8.GetBytes("{" +
            "\"lat\":" + examinationScript.latitude + "," +
            "\"lng\":" + examinationScript.longitude +"," +
            "\"image\":" + "\"" + images[1] + "\"," +
            "\"estimatedSuverity\":" + examinationScript.severity  +
            "}");
        using (UnityWebRequest www = UnityWebRequest.Put("http://localhost:8080/api/reports", postData)) {
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");
            www.method = UnityWebRequest.kHttpVerbPOST;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) {
                Debug.Log(www.responseCode);
                Debug.Log(www.error);
                uploading = false;
                finishedUploading = true;
            }
            else {
                Debug.Log("Form upload complete!");
                uploading = false;
                finishedUploading = true;
            }
        }
    }


    void CamCapture() {

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;

        cam.Render();

        Texture2D Image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
        Image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;

        var Bytes = Image.EncodeToPNG();
        images.Add(System.Convert.ToBase64String(Bytes));
        Destroy(Image);
        //System.Convert.ToBase64String(Bytes);
        fileCounter++;
    }


    private void OnDrawGizmos() {
        if (started) {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(cameraParent.transform.position, cam.transform.forward * visionRange);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(cameraParent.transform.position + cam.transform.forward * visionRange, new Vector3(visionWidth, visionHeight, 0.1f));
            Gizmos.DrawLine(cameraParent.transform.position, cameraParent.transform.position + cam.transform.forward.normalized * visionRange);
        }
    }
}
