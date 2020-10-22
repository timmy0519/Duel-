using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private float lineSize;
    private GameObject lineObj;
    public LineDrawer(float lineSize = 0.2f)
    {
         lineObj = new GameObject("LineObj");
        lineRenderer = lineObj.AddComponent<LineRenderer>();
        //Particles/Additive
        lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));

        this.lineSize = lineSize;
    }
    public void Setparent(Transform parent)
    {
        lineObj.transform.SetParent(parent);
    }
    private void init(float lineSize = 0.2f)
    {
        if (lineRenderer == null)
        {
             lineObj = new GameObject("LineObj");
            lineRenderer = lineObj.AddComponent<LineRenderer>();
            //Particles/Additive
            lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));

            this.lineSize = lineSize;
        }
    }

    //Draws lines through the provided vertices
    public void DrawLineInGameView(Vector3 start, Vector3 end, Color color)
    {
        if (lineRenderer == null)
        {
            init(0.2f);
        }

        //Set color
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        //Set width
        lineRenderer.startWidth = lineSize;
        lineRenderer.endWidth = lineSize;

        //Set line count which is 2
        lineRenderer.positionCount = 2;

        //Set the postion of both two lines
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    public void Destroy()
    {
        if(lineObj!=null)
        {
            Destroy(lineObj);
        }
        if (lineRenderer != null)
        {
            UnityEngine.Object.Destroy(lineRenderer.gameObject);
        }
    }
}

