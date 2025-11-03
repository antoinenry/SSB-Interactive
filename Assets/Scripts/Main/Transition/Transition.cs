using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class Transition : MonoBehaviour
{
    public Image image;
    public string radiusParameterName = "_Radius";
    public bool open = false;
    public float openRadius = 2000f;
    public float speed = 500f;

    public Material TransitionMaterial => image != null ? image.material : null;

    private StageLoader stageLoader;

    private void Awake()
    {
        stageLoader = FindObjectOfType<StageLoader>(true);
    }

    private void Update()
    {
        if (stageLoader != null) open = stageLoader.LoadedStage != null && stageLoader.LoadedMoment >= 0 && stageLoader.LoadedMoment < stageLoader.LoadedStage.MomentCount;
        if (TransitionMaterial == null) return;
        float radius;
        if (Application.isPlaying)
            radius = Mathf.MoveTowards(TransitionMaterial.GetFloat(radiusParameterName), open ? openRadius : 0f, speed * Time.unscaledDeltaTime);
        else
            radius = open ? openRadius : 0f;
        TransitionMaterial.SetFloat(radiusParameterName, radius);
    }
}
