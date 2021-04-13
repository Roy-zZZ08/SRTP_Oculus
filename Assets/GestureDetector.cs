using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.Events;

using UnityEngine.Video;

using UnityEngine.UI;





[System.Serializable]

public struct Gesture

{
    public string name;
    public List<Vector3> fingerDatas;
    public UnityEvent onRecognized;

}

public class GestureDetector : MonoBehaviour

{
    public float threshold = 0.1f;
    public OVRSkeleton skeleton;
    public List<Gesture> gestures;
    public bool debugMode = true;
    public List<OVRBone> fingerBones;
    private Gesture previousGesture;
    public VideoController MyVideo;

    // Start is called before the first frame update
    void Start()
    {
        previousGesture = new Gesture();
    }
    // Update is called once per frame
    void Update()
    {
        bool down = Input.GetKeyDown(KeyCode.Space);
        if (down)
        {
            Debug.Log("Hello Space");
            Save();
        }
        Gesture currentgesture = Recognize();
        bool hasRecognized = !currentgesture.Equals(new Gesture());
        //check if new Gesture

        if (hasRecognized && !currentgesture.Equals(previousGesture))
        {
            print("New Gesture Found : " + currentgesture.name);
            previousGesture = currentgesture;
            currentgesture.onRecognized.Invoke();
            if ((currentgesture.name == "Good")||(currentgesture.name =="Left_Good"))
            {
                MyVideo.Video_Play();
            }
            else if ((currentgesture.name == "Cut")||(currentgesture.name =="Left_Cut"))
            {

                MyVideo.Video_Pause();

            }
            // else if (currentgesture.name == "Right")

            // {

            //     sliderVideo.value = sliderVideo.value + numBer;

            // }

            // else if (currentgesture.name == "Left")
            // {

            //     sliderVideo.value = sliderVideo.value - numBer;

            // }

        }
    }
    void Save()
    {

        fingerBones = new List<OVRBone>();

        foreach (var bone in skeleton.Bones)

        {

            fingerBones.Add(bone);

        }



        Gesture g = new Gesture();

        g.name = "New Gesture";

        List<Vector3> data = new List<Vector3>();

        foreach (var bone in fingerBones)

        {



            data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));

            print("1");

        }



        g.fingerDatas = data;

        gestures.Add(g);

    }



    Gesture Recognize()
    {



        fingerBones = new List<OVRBone>();

        foreach (var bone in skeleton.Bones)

        {

            fingerBones.Add(bone);

        }



        Gesture currentgesture = new Gesture();

        float currentMin = Mathf.Infinity;//找到最相近的手势



        foreach (var gesture in gestures)

        {

            float sumDistance = 0;

            bool isDiscarded = false;

            for (int i = 0; i < fingerBones.Count; i++)
            {

                Vector3 currentData = skeleton.transform.InverseTransformPoint(fingerBones[i].Transform.position);

                float distance = Vector3.Distance(currentData, gesture.fingerDatas[i]);

                if (distance > threshold)

                {

                    isDiscarded = true;

                    break;

                }



                sumDistance += distance;



            }



            if (!isDiscarded && sumDistance < currentMin)
            {

                currentMin = sumDistance;

                currentgesture = gesture;

            }

        }



        return currentgesture;

    }


}

