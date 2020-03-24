using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TEM_Effects;

public class DestroyGameObjectNode : Node
{
    public GameObject goDestroy = default;

    public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle inPointStyle, GUIStyle outPointStyle,
        Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode,
        NodeEffectType nodeEffect)
    {
        base.Initialise(position, nodeSkin, inPointStyle, outPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, nodeEffect);

        //Override the rect size
        rect.height = 150f;
        rect.width = 244.9f;
    }

    public override void Draw()
    {
        inPoint.Draw();
        outPoint.Draw();
        title = rect.position.ToString();
        GUI.Box(rect, title, nodeSkin.style);
        temEffectDescription = EditorGUI.TextArea(new Rect(rect.x + 10f, rect.y + 50f, rect.width - 20f, 30f), temEffectDescription);

        //Draw a object field for inputting  the gameobject to destroy
        goDestroy = (GameObject)EditorGUI.ObjectField(new Rect(rect.x+10,rect.y +100f,rect.width-20,20f), goDestroy, typeof(GameObject),true);

    }

    public override void SaveEffectData()
    {
        DestroyGameObject baseEffectCopy = new DestroyGameObject();

        //Then transfer reference this node holds to the new base effect
        baseEffectCopy.targetObject = goDestroy;
        baseEffectCopy.description = temEffectDescription;

        //record this in temp after all transfer of values or references
        baseEffect = baseEffectCopy;
    }

    public override void ConnectNextEffect()
    {
        //Link the next effect to the next node's base effect
        //if(outPoint.nextNode != null)
        baseEffect.nextEffect = outPoint.nextNode.baseEffect;

    }


}
