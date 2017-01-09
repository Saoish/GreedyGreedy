using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LerpQueue {
    ObjectController OC;
    Queue<Vector2> processing_queue;

    float LerpTime = 0.1f;

    private bool Lerping = false;

    public LerpQueue(ObjectController OC) {
        processing_queue = new Queue<Vector2>();
        this.OC = OC;
    }
    public void Add(Vector2 TargetPosition) {
        float Distance = Mathf.Abs(Vector2.Distance(OC.Position, TargetPosition));
        if (Distance == 0)
            return;
        else if (Distance < 0.3) {
            processing_queue.Enqueue(TargetPosition);
            GameManager.instance.StartCoroutine(ProcessingLerp());
        } else {//Fuck this guy lag as hell
            OC.Position = TargetPosition;
        }
    }
    IEnumerator ProcessingLerp() {
        Lerping = true;
        float StartTime = Time.time;
        float EndTime = StartTime + LerpTime;
        while (Time.time < EndTime && OC.Alive) {
            float timeProgressed = (Time.time - StartTime) / LerpTime;
            OC.Position = Vector2.Lerp(OC.Position, processing_queue.Peek(), timeProgressed);
            yield return new WaitForFixedUpdate();
        }
        processing_queue.Dequeue();
        if (Size > 0)
            GameManager.instance.StartCoroutine(ProcessingLerp());
        else
            Lerping = false;
    }
    
    int Size {
        get { return processing_queue.Count; }
    }

}
