using TMPro;
using UnityEngine;
using UnityEngine.TextCore;

namespace Transmutable.TMPro
{
    /// <summary>
    /// Will render a single char from TMPRo SDF sprite atlas.
    /// Meant to be used with TMProSprite_SDF.shader
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [AddComponentMenu("Mesh/TMProSprite")]
    public class TMProSprite : MonoBehaviour
    {
        [SerializeField]
        TMP_FontAsset m_FontAsset = null;

        [SerializeField]
        Color m_Color = Color.white;

        [SerializeField]
        char m_Character = 'q';

        static Mesh k_Mesh = null;
        Renderer m_Renderer = null;

        MaterialPropertyBlock m_PropertyBlock = null;
        static readonly int InstancedUVOffsetsProperty = Shader.PropertyToID("_InstancedUVOffsets");
        static readonly int InstancedFaceColorProperty = Shader.PropertyToID("_InstancedFaceColor");

        Mesh sharedMesh
        {
            get
            {
                if (k_Mesh == null)
                {
                    // TMPro requires the mesh to have certain weird things set on normals, tangents and colors, so generate it. These values are just copied from what debugger showed in memory.
                    k_Mesh = new Mesh();
                    k_Mesh.hideFlags = HideFlags.HideAndDontSave;
                    k_Mesh.vertices = new[]
                    {
                        new Vector3(-1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, 1, 0), new Vector3(1, -1, 0)
                    };
                    k_Mesh.uv = new[]
                    {
                        new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0)
                    };
                    // The values put in this are not UV values they are some data that TMPro encodes into the UV to control presentation. Just copied the same values debugger showed in memory.
                    k_Mesh.uv2 = new[]
                    {
                        new Vector2(0, 0.000193548418f), new Vector2(511, 0.000193548418f), new Vector2(2093567, 0.000193548418f), new Vector2(2093567, 0.000193548418f)
                    };
                    k_Mesh.colors = new[]
                    {
                        Color.white, Color.white, Color.white, Color.white
                    };
                    k_Mesh.triangles = new[] { 0, 1, 2, 2, 3, 0 };
                    Vector3 normal = new Vector3(0, 0, -1);
                    k_Mesh.normals = new[] { normal, normal, normal, normal };
                    Vector4 tanget = new Vector4(-1, 0, 0, 1);
                    k_Mesh.tangents = new[] { tanget, tanget, tanget, tanget };
                    k_Mesh.RecalculateBounds();
                }

                return k_Mesh;
            }
        }

        void Start()
        {
            Initialize();
            UpdateSprite();
        }

        void Initialize()
        {
            m_PropertyBlock = new MaterialPropertyBlock();
            m_Renderer = GetComponent<MeshRenderer>();
            GetComponent<MeshFilter>().sharedMesh = sharedMesh;
        }

        void OnDrawGizmosSelected()
        {
            Initialize();
            UpdateSprite();
        }

        void UpdateSprite()
        {
            if (m_FontAsset != null && m_FontAsset.characterLookupTable.TryGetValue(m_Character, out var character))
            {
                GlyphRect glyphRect = character.glyph.glyphRect;
                float width = m_FontAsset.atlasWidth;
                float height = m_FontAsset.atlasHeight;
                Vector4 offsets = new Vector4((float)glyphRect.x / width, (float)glyphRect.y / height, (float)glyphRect.width / width, (float)glyphRect.height / height);

                m_Renderer.GetPropertyBlock(m_PropertyBlock);
                m_PropertyBlock.SetVector(InstancedUVOffsetsProperty, offsets);
                m_PropertyBlock.SetColor(InstancedFaceColorProperty, m_Color);
                m_Renderer.SetPropertyBlock(m_PropertyBlock);
            }
        }

        [ContextMenu("SetToAspectRatio")]
        void SetToAspectRatio()
        {
            if (m_FontAsset.characterLookupTable.TryGetValue(m_Character, out var character))
            {
                GlyphRect glyphRect = character.glyph.glyphRect;
                Vector3 newScale = transform.localScale;
                newScale.x = newScale.y * ((float)glyphRect.width / (float)glyphRect.height);
                transform.localScale = newScale;
            }
        }
    }
}
