using UnityEngine;
using TMPro;
namespace LEM_Effects
{


    [AddComponentMenu("")] public class  ReWordTextMeshPro : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<TMP_Text, string>
#endif
    {
        //target
        [Tooltip("The TextMeshPro you want to reword")]
        [SerializeField] TMP_Text m_TargetText = default;

        [Tooltip("The text you want to insert into the target text")]
        [SerializeField] string m_NewText = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;



        public override void OnInitialiseEffect()
        {
            //Retext 
            m_TargetText.text = m_NewText;
        }

#if UNITY_EDITOR
        public void SetUp(TMP_Text t1, string t2)
        {
            m_TargetText = t1;
            m_NewText = t2;
        }
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            ReWordTextMeshPro t = go.AddComponent<ReWordTextMeshPro>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetText, out t.m_NewText);
            return t;
        }
        public void UnPack(out TMP_Text t1, out string t2)
        {
            t1 = m_TargetText;
            t2 = m_NewText;
        }
#endif
    }
}