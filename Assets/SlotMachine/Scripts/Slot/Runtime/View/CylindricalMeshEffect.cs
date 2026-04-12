using UnityEngine;
using UnityEngine.UI;

namespace SlotMachine.Slot.View
{
    [RequireComponent(typeof(Graphic))]
    public class CylindricalMeshEffect : BaseMeshEffect
    {
        [SerializeField] private float m_ReelSide;

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
                return;

            UIVertex vert = default;
            int vertCount = vh.currentVertCount;

            for (int i = 0; i < vertCount; i++)
            {
                vh.PopulateUIVertex(ref vert, i);
                vert.uv1 = new Vector4(0f, m_ReelSide, 0f, 0f);
                vh.SetUIVertex(vert, i);
            }
        }
    }
}
