using UnityEngine;

public class DashLine
{
    LineRenderer lineRenderer;
    Transform lineOrigin;

    public DashLine(LineRenderer lineRenderer)
    {
        this.lineOrigin = lineRenderer.transform;
        this.lineRenderer = lineRenderer;
    }

    public void setDash(Vector2 dashVector)
    {
        if (dashVector == Vector2.zero)
        {
            lineRenderer.gameObject.SetActive(false);
        }

        if (dashVector.magnitude > 0)
        {
            lineRenderer.gameObject.SetActive(true);
        }

        Vector2 charPosition = lineOrigin.position;

        Vector2 frontEnd = charPosition + dashVector;
        Vector2 backEnd = charPosition - dashVector;

        lineRenderer.SetPosition(0, frontEnd);
        lineRenderer.SetPosition(1, backEnd);
    }
}