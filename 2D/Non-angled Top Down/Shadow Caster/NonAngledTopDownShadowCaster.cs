using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NonAngledTopDownShadowCaster : MonoBehaviour
{
    #region Variables
    [Header("Settings")]
    public bool _updateShadowSpritesAtRuntime;
    private static Color _shadowColor = new Color(0, 0, 0, 0.375f); 
    private static Vector2 _shadowOffset = new Vector2(-0.125f, -0.125f);
    
    [Header("Properties")]
    private static string _shadowSortingLayerName = "Shadow";
    private static Shader _GUITextShader;
    private static string _GUITextShaderPath = "GUI/Text Shader";

    [Header("Validation")]
    private static bool _attemptedRequirmentValidation;
    private static bool _requirementsValidated;

    [Header("References")]
    private List<Transform> _shadowTransform = new List<Transform>();
    private List<SpriteRenderer> _shadowSpriteRenderer = new List<SpriteRenderer>();
    private List<Transform> _selfSpriteRendererTransform = new List<Transform>();
    private List<SpriteRenderer> _selfSpriteRenderer = new List<SpriteRenderer>();

    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        ValidateRequirements();

        if (!_requirementsValidated)
            return;

        GetRequiredComponents();
    }

    private void Start()
    {
        CreateShadowForEachSpriteRenderer();
    }

    private void LateUpdate()
    {
        UpdateAllShadows();
    }
    #endregion

    #region  Initialization
    public void GetRequiredComponents()
    {
        _selfSpriteRenderer = GetComponentsInChildren<SpriteRenderer>().ToList();

        for (int i = 0; i < _selfSpriteRenderer.Count; i++)
        {
            _selfSpriteRendererTransform.Add(_selfSpriteRenderer[i].transform);
        }
    }

    public void ValidateRequirements()
    {
        if (_attemptedRequirmentValidation)
            return;

        _attemptedRequirmentValidation = true;

        if (_GUITextShader == null)
        {
            _GUITextShader = Shader.Find(_GUITextShaderPath);

            if (_GUITextShader == null)
            {
                Debug.LogError($"'{_GUITextShaderPath}' Shader Path is not valid!");
                this.enabled = false;

                return;
            }
        }

        if (!SortingLayer.IsValid(SortingLayer.NameToID(_shadowSortingLayerName)))
        {
            Debug.LogError($"'{_shadowSortingLayerName}' Sorting Layer is not valid!");
            this.enabled = false;

            return;
        }

        _requirementsValidated = true;
    }
    #endregion

    #region  Functionality
    public void UpdateAllShadows()
    {
        for (int i = 0; i < _selfSpriteRenderer.Count; i++)
        {
            _shadowTransform[i].transform.position = _selfSpriteRendererTransform[i].position + (Vector3)_shadowOffset;

            if (_updateShadowSpritesAtRuntime)
            {
                _shadowSpriteRenderer[i].sprite = _selfSpriteRenderer[i].sprite;
            }
        }
    }

    public void CreateShadowForEachSpriteRenderer()
    {
        for (int i = 0; i < _selfSpriteRenderer.Count; i++)
        {
            _shadowTransform.Add(new GameObject($"{_selfSpriteRenderer[i].gameObject.name}'s Shadow", typeof(SpriteRenderer)).transform);
            _shadowSpriteRenderer.Add(_shadowTransform[i].GetComponent<SpriteRenderer>());

            _shadowTransform[i].transform.parent = _selfSpriteRenderer[i].gameObject.transform;

            _shadowTransform[i].position = _selfSpriteRendererTransform[i].position + (Vector3)_shadowOffset;
            _shadowTransform[i].localRotation = Quaternion.identity;
            _shadowTransform[i].transform.localScale = Vector3.one;
            
            _shadowSpriteRenderer[i].sprite = _selfSpriteRenderer[i].sprite;
            _shadowSpriteRenderer[i].color = _shadowColor;
            _shadowSpriteRenderer[i].material.shader = _GUITextShader;
            _shadowSpriteRenderer[i].sortingLayerName = _shadowSortingLayerName;
        }
    }
    #endregion
}
