using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Snorlax.Prototype.Flocking
{
    [CustomEditor(typeof(CompositeBehavior))]
    public class CompositeBehaviorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //setup
            CompositeBehavior cb = (CompositeBehavior)target;

            //check for behaviors
            if (cb.behaviors == null || cb.behaviors.Length == 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("No behaviors in array.", MessageType.Warning);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Number", GUILayout.MinWidth(60f), GUILayout.MaxWidth(60f));
                EditorGUILayout.LabelField("Behaviors", GUILayout.MinWidth(60f));
                EditorGUILayout.LabelField("Weights", GUILayout.MinWidth(60f), GUILayout.MaxWidth(60f));
                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < cb.behaviors.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(i.ToString(), GUILayout.MinWidth(60f), GUILayout.MaxWidth(60f));
                    cb.behaviors[i] = (FlockBehavior)EditorGUILayout.ObjectField(cb.behaviors[i], typeof(FlockBehavior), false, GUILayout.MinWidth(60f));
                    cb.Weights[i] = EditorGUILayout.FloatField(cb.Weights[i], GUILayout.MinWidth(60f), GUILayout.MaxWidth(60f));
                    EditorGUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Add Behavior"))
            {
                AddBehavior(cb);
                EditorUtility.SetDirty(cb);
            }

            if (cb.behaviors != null && cb.behaviors.Length > 0)
            {
                if (GUILayout.Button("Remove Behavior"))
                {
                    RemoveBehavior(cb);
                    EditorUtility.SetDirty(cb);
                }
            }
        }

        #region Old Code
        /*
        public override void OnInspectorGUI()
        {
            CompositeBehavior cb = (CompositeBehavior)target;

            Rect r = EditorGUILayout.BeginHorizontal();
            r.height = EditorGUIUtility.singleLineHeight;

            if(cb.behaviors == null || cb.behaviors.Length == 0)
            {
                EditorGUILayout.HelpBox("No Behaviors in array.", MessageType.Warning);
                EditorGUILayout.EndHorizontal();
                r = EditorGUILayout.BeginHorizontal();
                r.height = EditorGUIUtility.singleLineHeight;
            }
            else
            {
                r.x = 30f;
                r.width = EditorGUIUtility.currentViewWidth - 95f;
                EditorGUI.LabelField(r, "Behaviors");
                r.x = EditorGUIUtility.currentViewWidth - 65f;
                r.width = 60f;
                EditorGUI.LabelField(r, "Weights");
                r.y += EditorGUIUtility.singleLineHeight * 1.2f;

                EditorGUI.BeginChangeCheck();
                for (int i = 0; i < cb.behaviors.Length; i++)
                {
                    r.x = 5f;
                    r.width = 20f;
                    EditorGUI.LabelField(r, i.ToString());
                    r.x = 30f;
                    r.width = EditorGUIUtility.currentViewWidth - 95f;
                    cb.behaviors[i] = (FlockBehavior)EditorGUI.ObjectField(r, cb.behaviors[i], typeof(FlockBehavior), false);
                    r.x = EditorGUIUtility.currentViewWidth - 65f;
                    r.width = 60f;
                    cb.Weights[i] = EditorGUI.FloatField(r, cb.Weights[i]);
                    r.y += EditorGUIUtility.singleLineHeight * 1.1f;
                }
                if(EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(cb);
                }

                EditorGUILayout.EndHorizontal();
                r.x = 5f;
                r.width = EditorGUIUtility.currentViewWidth - 10f;
                r.y += EditorGUIUtility.singleLineHeight * 0.5f;
            }

            if (GUI.Button(r, "Add Behavior"))
            {
                AddBehavior(cb);
                EditorUtility.SetDirty(cb);
            }

            r.y += EditorGUIUtility.singleLineHeight * 1.5f;
            if (cb.behaviors != null && cb.behaviors.Length > 0)
            {
                if (GUI.Button(r, "Remove Bheavior"))
                {
                    RemoveBehavior(cb);
                    EditorUtility.SetDirty(cb);
                }
            }
        }*/

        void AddBehavior(CompositeBehavior cb)
        {
            int oldCount = (cb.behaviors != null) ? cb.behaviors.Length : 0;
            FlockBehavior[] newBehaviors = new FlockBehavior[oldCount + 1];
            float[] newWeights = new float[oldCount + 1];

            for (int i = 0; i < oldCount; i++)
            {
                newBehaviors[i] = cb.behaviors[i];
                newWeights[i] = cb.Weights[i];
            }

            newWeights[oldCount] = 1f;
            cb.behaviors = newBehaviors;
            cb.Weights = newWeights;
        }

        void RemoveBehavior(CompositeBehavior cb)
        {
            int oldCount = cb.behaviors.Length;
            if(oldCount == 1)
            {
                cb.behaviors = null;
                cb.Weights = null;
                return;
            }

            FlockBehavior[] newBehaviors = new FlockBehavior[oldCount - 1];
            float[] newWeights = new float[oldCount - 1];

            for (int i = 0; i < oldCount - 1; i++)
            {
                newBehaviors[i] = cb.behaviors[i];
                newWeights[i] = cb.Weights[i];
            }

            cb.behaviors = newBehaviors;
            cb.Weights = newWeights;
        }
        #endregion
    }
}
