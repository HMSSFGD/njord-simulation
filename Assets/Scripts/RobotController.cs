using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public enum RobotState {
    Patrolling,
    Examining,
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
    public float rotationSpeed;
    public float rotationPerPicture;
    private List<Transform> patrolPoints = new List<Transform>();
    private int currentPatrolPointIndex;
    private float minDist = 0.5f;
    private RobotState state = RobotState.Patrolling;
    private Transform examinePoint;
    private DamageScript examinationScript;
    private LayerMask damageOrHull;
    private int damageLayer;
    bool started = false;
    float rotation = 0;
    float pictureRotationCounter = 0;
    bool approaching = true;
    private int fileCounter = 0;
    public int botID = 0;
    // Start is called before the first frame update
    void Start() {

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

    private void CheckForDamage() {
        RaycastHit[] hits = Physics.BoxCastAll(cameraParent.transform.position, new Vector3(visionWidth, visionWidth, 0.1f), cam.transform.forward, Quaternion.identity, visionRange);
        foreach (RaycastHit hit in hits) {
            if (hit.transform.gameObject.layer == damageLayer) {
                if (hit.transform != examinePoint) {
                    examinationScript = hit.transform.GetComponent<DamageScript>();
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
            playerObj.transform.position-cameraParent.transform.position,
            out hit,
            sightRange,
            damageOrHull
        );
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

    private void Examine() {
        if (approaching) {
            if (Vector3.Distance(transform.position, examinePoint.position) > examineDistance) {
                transform.position += (examinePoint.position - transform.position).normalized * speed * Time.deltaTime;
            }
            else {
                approaching = false;
            }
        }
        else {
            if (rotation < 120) {
                transform.RotateAround(examinePoint.position, new Vector3(0, 1, 0), rotationSpeed * Time.deltaTime);
                rotation += rotationSpeed * Time.deltaTime;
                pictureRotationCounter -= rotationSpeed * Time.deltaTime;
                if (pictureRotationCounter < 0) {
                    CamCapture();
                    pictureRotationCounter = rotationPerPicture;
                }
            }
            else {
                examinePoint.gameObject.GetComponent<DamageScript>().SetReported(true);
                SwitchToPatrol();
            }
            
        }
        
        
        
        cameraParent.transform.LookAt(examinePoint.transform);
    }

    void UploadScreenshots() {
        
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
        Destroy(Image);
        //System.Convert.ToBase64String(Bytes);
        File.WriteAllBytes(Application.dataPath + "/BotScreenshots/bot" + botID + "/" + fileCounter + ".png", Bytes);
        fileCounter++;
    }


    private void OnDrawGizmos() {
        if (started) {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(cameraParent.transform.position, cam.transform.forward * visionRange);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(cameraParent.transform.position + cam.transform.forward * visionRange, new Vector3(visionWidth, visionWidth, 0.1f));
            Gizmos.DrawLine(cameraParent.transform.position, cameraParent.transform.position + cam.transform.forward.normalized*visionRange);
        }
    }
}
