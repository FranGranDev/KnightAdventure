using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerascript : MonoBehaviour
{
    public bool Static;
    public Vector3 offset = new Vector3(0f, 0, -3f);
    public Transform Target;
    public Transform Character;
    private Transform PrevTarget;
    private Coroutine LastShow;
    private float prevSpeed;
    private float prevZoom;
    private float prevMouse;
    private bool CanShow;
    private Vector3 Effect;
    private Animator anim;
    [HideInInspector]
    public float Blur;
    [HideInInspector]
    public float Size;
    [HideInInspector]
    private float Zoom = 1;
    public float MaxZoom;
    public float MouseZoom = 0;
    [HideInInspector]
    public float CameraSpeed = 0.1f;

    void Start()
    {
        anim = GetComponent<Animator>();
        Size = transform.GetComponent<Camera>().orthographicSize;
    }

    public void StartDialog(Transform obj)
    {
        Zoom = 2;
        
        Target = obj;
    }
    public void EndDialog()
    {
        CanShow = true;
        Zoom = 1;
        CameraSpeed = 0.1f;
        Target = Character;
        StopAllCoroutines();
    }
    public void ShowTarget(Transform obj)
    {
        Target = obj;
    }
    public void ReturnTarget()
    {
        CanShow = true;
        MouseZoom = 0;
        Zoom = 1;
        Target = Character;
    }
    public void ShowTargetForTime(Transform target, float time)
    {
        if (Static)
            return;
        if(LastShow != null)
        {
            StopCoroutine(LastShow);
            CameraSpeed = prevSpeed;
            Zoom = prevZoom;
            MouseZoom = prevMouse;
            Target = Character;
        }
        LastShow = StartCoroutine(ShowTargetForTimeCour(target, time));
    }
    IEnumerator ShowTargetForTimeCour(Transform obj, float time)
    {
        Target = obj;
        prevSpeed = CameraSpeed;
        prevZoom = Zoom;
        prevMouse = MouseZoom;

        Target = obj;
        CameraSpeed = 0.03f;
        Target = obj;
        Zoom = 0.75f;
        MouseZoom = -3;
        yield return new WaitForSeconds(time);
        CameraSpeed = prevSpeed;
        Zoom = prevZoom;
        MouseZoom = prevMouse;
        Target = Character;
        LastShow = null;
        yield break;
    }
    public void ShowTargets(GameObject[] obj, float AllTime)
    {
        StartCoroutine(ShowTargetsCour(obj, AllTime));
    }
    IEnumerator ShowTargetsCour(GameObject[] obj, float time)
    {
        prevSpeed = CameraSpeed;
        prevZoom = Zoom;
        prevMouse = MouseZoom;
        CameraSpeed = 0.03f;
        Zoom = 0.75f;
        MouseZoom = -3;
        while (!CanShow)
        {
            yield return new WaitForFixedUpdate();
        }
        CanShow = false;
        for (int i = 0; i < obj.Length; i++)
        {
            Target = obj[i].transform;
            yield return new WaitForSeconds(time);
        }
        CameraSpeed = prevSpeed;
        Zoom = prevZoom;
        MouseZoom = prevMouse;
        Target = Character;
        CanShow = true;
        yield break;
    }

    public void GetEffect(int Effect, int Power)
    {
        switch(Effect)
        {
            case 0:
                break;
            case 1:
                anim.SetBool("Drunk", true);
                anim.SetInteger("DrunkPower", Power);
                break;
            case 2:
                break;
        }
    }
    public void EndEffect(int Effect)
    {
        switch (Effect)
        {
            case 0:
                break;
            case 1:
                anim.SetBool("Drunk", false);
                anim.SetInteger("DrunkPower", 0);
                break;
            case 2:
                break;
        }
    }
    public void EndEffect()
    {
        anim.SetBool("Drunk", false);
        anim.SetInteger("DrunkPower", 0);
    }

    void GetOffset(Vector3 pos)
    {
        Effect = pos;
    }
    void GetZoom(float zoom)
    {
        Zoom = zoom;
    }
    public void GetMouseZoom(int Zoom)
    {
        MouseZoom = Zoom;
    }

    void FixedUpdate()
    {
        if (!Static)
        {
            transform.GetComponent<Camera>().orthographicSize =
            Mathf.Lerp(transform.GetComponent<Camera>().orthographicSize, (Size + MouseZoom) / Zoom, 0.05f);
        }
        Vector3 CurrentPos = Target.position + offset + Effect;
        transform.position = Vector3.Lerp(transform.position, CurrentPos, CameraSpeed);

        if (MouseZoom > 0)
        {
            MouseZoom -= 0.15f;
        }
        Shader.SetGlobalFloat("_Blur", Blur);
    }
    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            if (MouseZoom - Input.mouseScrollDelta.y < MaxZoom &&
               MouseZoom - Input.mouseScrollDelta.y > -8)
               MouseZoom = MouseZoom - Input.mouseScrollDelta.y;
        }
    }
}
